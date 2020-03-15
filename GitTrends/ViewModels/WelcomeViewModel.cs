using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace GitTrends
{
    public class Section
    {
        public string Header { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
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
                    Header = "Referring Sites",
                }
            };
        }
    }
}
