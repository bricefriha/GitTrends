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
            //_carouselItems = new ObservableCollection<StackLayout>()
            //{
            //    new StackLayout
            //    {
            //        Orientation = StackOrientation.Vertical,
            //        VerticalOptions = LayoutOptions.Center,
            //        HorizontalOptions = LayoutOptions.Center,

            //        Children =
            //        {
            //            new Label
            //            {
            //                Text = "Welcome on GitTrend",
            //                VerticalOptions = LayoutOptions.Center,
            //                FontSize = 45,
                            
            //            }
            //        }
            //    }
            //};

            _sections = new ObservableCollection<Section>()
            {
                new Section
                {
                    Header = "Welcome on GitTrend",
                },
                new Section
                {
                    Header = "Repo Traffic",
                },
                new Section
                {
                    Header = "Referring Sites",
                },
                new Section
                {
                    Header = "GitHub Login",
                }
            };
        }
    }
}
