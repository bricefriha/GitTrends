using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using GitTrends.Mobile.Shared;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace GitTrends
{
    public class Section
    {
        public string Header { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public ContentView ActionView { get; set; }
        public bool ConnectionRequire { get; set; }
    };
    public class WelcomeViewModel : BaseViewModel
    {
       
        private ObservableCollection<Section> _sections;
        public ObservableCollection<Section> Sections
        {
            get
            {
                return _sections;
            }
            set
            {
                _sections = value;
                OnPropertyChanged();
            }
        }
        public event EventHandler<string?> GitHubLoginUrlRetrieved
        {
            add => _gitHubLoginUrlRetrievedEventManager.AddEventHandler(value);
            remove => _gitHubLoginUrlRetrievedEventManager.RemoveEventHandler(value);
        }

        public ICommand DemoButtonCommand { get; }
        public IAsyncCommand LoginButtonCommand { get; }

        public bool IsDemoButtonVisible => !IsAuthenticating
                                            && LoginButtonText is GitHubLoginButtonConstants.ConnectWithGitHub
                                            && GitHubAuthenticationService.Alias != DemoDataConstants.Alias;
        readonly WeakEventManager<string?> _gitHubLoginUrlRetrievedEventManager = new WeakEventManager<string?>();
        readonly GitHubAuthenticationService _gitHubAuthenticationService;
        readonly TrendsChartSettingsService _trendsChartSettingsService;

        string _gitHubUserImageSource = string.Empty;
        string _gitHubUserNameLabelText = string.Empty;
        string _gitHubButtonText = string.Empty;
        bool _isAuthenticating;
        public string LoginButtonText
        {
            get => _gitHubButtonText;
            set => SetProperty(ref _gitHubButtonText, value, () => OnPropertyChanged(nameof(IsDemoButtonVisible)));
        }

        public string GitHubAvatarImageSource
        {
            get => _gitHubUserImageSource;
            set => SetProperty(ref _gitHubUserImageSource, value);
        }

        public string GitHubAliasLabelText
        {
            get => _gitHubUserNameLabelText;
            set => SetProperty(ref _gitHubUserNameLabelText, value);
        }

        public bool IsAuthenticating
        {
            get => _isAuthenticating;
            set => SetProperty(ref _isAuthenticating, value, () =>
            {
                OnPropertyChanged(nameof(IsDemoButtonVisible));
                MainThread.InvokeOnMainThreadAsync(LoginButtonCommand.RaiseCanExecuteChanged).SafeFireAndForget(ex => Debug.WriteLine(ex));
            });
        }
        public WelcomeViewModel(AnalyticsService analyticsService) : base(analyticsService)
        {

            _sections = new ObservableCollection<Section>()
            {
                new Section
                {
                    Header = "Welcome on GitTrend",
                    ImageUrl = "GitTrends.png",
                    Content = "GitTrends is a tool allowing you to keep track of your Github repos." +
                                " With it, you can now stay on top of which repos are trending and ensure its code doesn't go stale!",
                },
                new Section
                {
                    Header = "Get an overview of your repos!",
                    ImageUrl = "https://user-images.githubusercontent.com/13558917/75208368-5091ca80-5730-11ea-8602-8c63244bf229.gif",
                    Content = "Here you can get charts which show all traffics on your repos:\n\n" +
                                " - Zoom in/out to see accurately what you need to know\n\n" +
                                " - A long press on the chart allow you to see a precise numeric value ",
                },
                new Section
                {
                    Header = "Try it yourself!",
                    Content = "The only thing you need to get started is a Github account",
                    ActionView = new GitHubSettingsView(),
                }
            };
        }
        void HandleAuthorizeSessionCompleted(object sender, AuthorizeSessionCompletedEventArgs e)
        {
            SetGitHubValues();

            IsAuthenticating = false;
        }

        void HandleAuthorizeSessionStarted(object sender, EventArgs e) => IsAuthenticating = true;

        void SetGitHubValues()
        {
            GitHubAliasLabelText = _gitHubAuthenticationService.IsAuthenticated ? GitHubAuthenticationService.Name : string.Empty;
            LoginButtonText = _gitHubAuthenticationService.IsAuthenticated ? $"{GitHubLoginButtonConstants.Disconnect}" : $"{GitHubLoginButtonConstants.ConnectWithGitHub}";

            GitHubAvatarImageSource = "DefaultProfileImage";

            if (Connectivity.NetworkAccess is NetworkAccess.Internet && !string.IsNullOrWhiteSpace(GitHubAuthenticationService.AvatarUrl))
                GitHubAvatarImageSource = GitHubAuthenticationService.AvatarUrl;
        }

        async Task ExecuteLoginButtonCommand()
        {
            AnalyticsService.Track("GitHub Button Tapped", nameof(GitHubAuthenticationService.IsAuthenticated), _gitHubAuthenticationService.IsAuthenticated.ToString());

            if (_gitHubAuthenticationService.IsAuthenticated)
            {
                await _gitHubAuthenticationService.LogOut().ConfigureAwait(false);

                SetGitHubValues();
            }
            else
            {
                IsAuthenticating = true;

                try
                {
                    var loginUrl = await _gitHubAuthenticationService.GetGitHubLoginUrl().ConfigureAwait(false);
                    OnGitHubLoginUrlRetrieved(loginUrl);
                }
                catch (Exception e)
                {
                    AnalyticsService.Report(e);

                    OnGitHubLoginUrlRetrieved(null);
                    IsAuthenticating = false;
                }
            }
        }

        void ExecuteDemoButtonCommand()
        {
            AnalyticsService.Track("Demo Button Tapped");

            GitHubAuthenticationService.Name = DemoDataConstants.Name;
            GitHubAuthenticationService.AvatarUrl = DemoDataConstants.AvatarUrl;
            GitHubAuthenticationService.Alias = DemoDataConstants.Alias;

            SetGitHubValues();
        }

        void OnGitHubLoginUrlRetrieved(string? loginUrl) => _gitHubLoginUrlRetrievedEventManager.HandleEvent(this, loginUrl, nameof(GitHubLoginUrlRetrieved));
    }
}
