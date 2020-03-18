
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace GitTrends
{
    class WelcomePage : BaseContentPage<WelcomeViewModel>
    {
        public WelcomePage(WelcomeViewModel welcomeViewModel, AnalyticsService analyticsService) : base("Welcome!", welcomeViewModel, analyticsService, false)
        {
            ViewModel.GitHubLoginUrlRetrieved += HandleGitHubLoginUrlRetrieved;

            var mainLayout = new StackLayout
            {
                //Padding = 150,
                Visual = VisualMarker.Material,
                VerticalOptions = LayoutOptions.Fill,
                Spacing = 1,
                
            };
            // set the carousel view
            CarouselView carouselView = new CarouselView();
            carouselView.SetBinding(ItemsView.ItemsSourceProperty, "Sections");
            carouselView.ItemTemplate = new DataTemplate(() =>
            {
                // Title
                Label titleLabel = new Label () {
                    FontSize = 24,
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                };
                titleLabel.SetBinding(Label.TextProperty, nameof(Section.Header));
                titleLabel.SetDynamicResource(Label.TextColorProperty, nameof(BaseTheme.TextColor));

                // Image
                Image image = new Image {
                    MinimumHeightRequest = 40,
                    MinimumWidthRequest = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    
                };
                image.SetBinding(Image.SourceProperty, nameof(Section.ImageUrl));

                // Content
                Label contentsLabel = new Label { 
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    FontSize = 15,
                };
                contentsLabel.SetBinding(Label.TextProperty, nameof(Section.Content));
                contentsLabel.SetDynamicResource(Label.TextColorProperty, nameof(BaseTheme.TextColor));

                // View to sign in with github account
                ContentView gitHubSettingsView = new ContentView();

                gitHubSettingsView.SetBinding(ContentView.ContentProperty, nameof(Section.ActionView));


                StackLayout contentStackLayout = new StackLayout
                {
                    Spacing = 15,
                    Children = { titleLabel, image, contentsLabel, gitHubSettingsView }

                };

                Frame frame = new Frame()
                {
                    Margin = 8,
                    HeightRequest = 600,
                    Content = contentStackLayout,
                    CornerRadius = 10,

                };
                frame.SetDynamicResource(Frame.BackgroundColorProperty, nameof(BaseTheme.NavigationBarTextColor));

                StackLayout rootStackLayout = new StackLayout
                {
                    Children = { frame },
                    VerticalOptions = LayoutOptions.Center,
                    
                };

                return rootStackLayout;
            });

            mainLayout.Children.Add (carouselView);

            var skipButton = new Button()
            {
                Text = "Skip the introduction",
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                CornerRadius = 200,
                Margin = 10,
            };
            skipButton.SetDynamicResource(Button.BackgroundColorProperty, nameof(BaseTheme.NavigationBarTextColor));
            skipButton.SetDynamicResource(Button.TextColorProperty, nameof(BaseTheme.TextColor));

            // Add a skip button 
            mainLayout.Children.Add (skipButton);
            Content = mainLayout;
        }


        async void HandleGitHubLoginUrlRetrieved(object sender, string? loginUrl)
        {
            if (!string.IsNullOrWhiteSpace(loginUrl))
                await OpenBrowser(loginUrl);
            else
                await DisplayAlert("Error", "Couldn't connect to GitHub Login. Check your internet connection and try again", "OK");
        }
    }
}