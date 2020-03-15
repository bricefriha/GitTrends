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
        private ObservableCollection<StackLayout> _carouselItems;
        public ObservableCollection<StackLayout> CarouselItems
        {
            get
            {
                return _carouselItems;
            }
            set
            {
                _carouselItems = value;
                OnPropertyChanged();
            }
        }
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
                    Header = "How to use the charts",
                    ImageUrl = "",
                },
                new Section
                {
                    Header = "Referring Sites",
                }
            };
        }
    }
}
