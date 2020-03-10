
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

            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Welcome !" }
                }
            };
        }
    }
}