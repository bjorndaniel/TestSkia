using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestSkia
{
    public partial class LoginPage : ContentPage
    {
        private HighlightForm _highlightForm;

        public LoginPage()
        {
            InitializeComponent();

            var settings = new HighlightSettings()
            {
                StrokeWidth = 6,
                StrokeStartColor = Color.FromHex("#FF4600"),
                StrokeEndColor = Color.FromHex("#CC00AF"),
                AnimationDuration = TimeSpan.FromMilliseconds(900),
                AnimationEasing = Easing.CubicInOut,
            };

            _highlightForm = new HighlightForm(settings);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Username.Focus();
        }

        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            _highlightForm.HighlightElement((View)sender, CanvasView, FormLayout);
        }

        private void Handle_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            _highlightForm.Draw(CanvasView, e.Surface.Canvas);
        }

        private void Handle_SizeChanged(object sender, EventArgs e)
        {
            _highlightForm.Invalidate(CanvasView, FormLayout);
        }

        private void Handle_Clicked(object sender, EventArgs e)
        {
            _highlightForm.HighlightElement((View)sender, CanvasView, FormLayout);
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Task.Factory.StartNew(async() => {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PushAsync(new MainPage());
                    });
                   });
                return false;
            });
        }
    }
}
