using System;
using SkiaSharp.Extended.Svg;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace TestSkia
{
    public class Icon : Frame
    {
        private readonly SKCanvasView _canvasView = new SKCanvasView();

        public Icon()
        {
            Padding = new Thickness(10);
            HasShadow = false;
            BackgroundColor = Color.Transparent;
            Content = _canvasView;
            _canvasView.PaintSurface += OnPaintSurface;
        }

        public string ResourceId
        {
            get => (string)GetValue(ResourceIdProperty);
            set => SetValue(ResourceIdProperty, value);
        }

        public static readonly BindableProperty ResourceIdProperty = BindableProperty.Create(
            nameof(ResourceId),
            typeof(string),
            typeof(Icon),
            default(string),
            BindingMode.TwoWay,
            propertyChanged: ResourceIdChanged
        );

        private static void ResourceIdChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var icon = bindable as Icon;
            icon?._canvasView.InvalidateSurface();
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            if (string.IsNullOrEmpty(ResourceId))
            {
                return;
            }
            using (var stream = GetType().Assembly.GetManifestResourceStream(ResourceId))
            {
                var svg = new SKSvg();
                svg.Load(stream);

                var info = e.Info;
                canvas.Translate(info.Width / 2f, info.Height / 2f);
                var bounds = svg.ViewBox;
                var xRatio = info.Width / bounds.Width;
                var yRatio = info.Height / bounds.Height;
                var ratio = Math.Min(xRatio, yRatio);
                canvas.Scale(ratio);
                canvas.Translate(-bounds.MidX, -bounds.MidY);

                canvas.DrawPicture(svg.Picture);
            }
        }
    }
}
