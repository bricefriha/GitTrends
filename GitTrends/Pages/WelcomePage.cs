
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

            //Remove BaseContentPageBackground
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
                Label nameLabel = new Label () {
                    FontSize = 24,
                    HorizontalOptions = LayoutOptions.Center,
                };
                nameLabel.SetBinding(Label.TextProperty, "Header");

                Image image = new Image { HorizontalOptions = LayoutOptions.Center };
                image.SetBinding(Image.SourceProperty, "ImageUrl");

                //Label locationLabel = new Label { ... };
                //locationLabel.SetBinding(Label.TextProperty, "Location");

                Label contentsLabel = new Label { HorizontalOptions = LayoutOptions.CenterAndExpand };
                contentsLabel.SetBinding(Label.TextProperty, "Content");

                

                StackLayout rootStackLayout = new StackLayout
                {
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