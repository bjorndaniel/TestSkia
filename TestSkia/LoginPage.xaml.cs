using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TestSkia
{
    public partial class LoginPage : ContentPage
    {
       

        public LoginPage()
        {
            InitializeComponent();
        }
        void Username_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            throw new NotImplementedException();
        }
        void Handle_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            throw new NotImplementedException();
        }

       async void Handle_Clicked(object sender, System.EventArgs e)
        {
           await Navigation.PushAsync(new MainPage());
        }
    }
}
