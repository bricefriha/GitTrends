
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

            var absoluteLayout = new StackLayout()
            {
                Padding = 150,
            };

            // instanciate the carousel view
            CarouselView carouselView = new CarouselView() 
            {
                PeekAreaInsets = 50,
            };
            carouselView.SetBinding(ItemsView.ItemsSourceProperty, "Sections");
            carouselView.ItemTemplate = new DataTemplate(() =>
            {
                // Title
                Label nameLabel = new Label () {
                    FontSize = 24,
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                };
                nameLabel.SetBinding(Label.TextProperty, "Header");

                // Image
                Image image = new Image {
                    MinimumHeightRequest = 40,
                    MinimumWidthRequest = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    
                };
                image.SetBinding(Image.SourceProperty, "ImageUrl");

                // Content
                Label contentsLabel = new Label { 
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    FontSize = 15,
                };
                contentsLabel.SetBinding(Label.TextProperty, "Content");

                

                StackLayout rootStackLayout = new StackLayout
                {
                    Spacing = 15,
                    Children = { nameLabel, image, contentsLabel }
                    
                };
                Frame frame = new Frame()
                {
                    Margin = 8,
                    HeightRequest = 480,
                    Content = rootStackLayout,
                    BackgroundColor = Color.White,
                };

                return frame;
            });

            absoluteLayout.Children.Add (carouselView);


            Content = carouselView;
        }
    }
}