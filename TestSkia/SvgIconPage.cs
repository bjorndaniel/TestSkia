using System;

using Xamarin.Forms;

namespace TestSkia
{
    public class SvgIconPage : ContentPage
    {
        public SvgIconPage()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

