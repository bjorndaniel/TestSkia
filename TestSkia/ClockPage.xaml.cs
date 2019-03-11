using Xamarin.Forms;
using SkiaSharp;
using System;

namespace TestSkia
{
    public partial class ClockPage : ContentPage
    {
        SKPaint blackFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black
        };
        SKPaint whiteLines = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 2,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };

        SKPaint whiteFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.White
        };

        SKPaint greenFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.PaleGreen
        };

        SKPaint blackStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 20,
            StrokeCap = SKStrokeCap.Round
        };

        SKPaint greyFill = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Gray
        };


        SKPath catEar = new SKPath();
        SKPath catEye = new SKPath();
        SKPath catPupil = new SKPath();
        SKPath catTail = new SKPath();
        SKPath hourPath = SKPath.ParseSvgPathData("M 0 60 C 0 -30 20 -30 5 -20 L 5 0 C 5 7.5 -5 0 L -5 -20 C -20 -30 0 -30 0");

        public ClockPage()
        {
            InitializeComponent();
            catEar.MoveTo(0, 0);
            catEar.LineTo(0, 75);
            catEar.LineTo(100, 75);
            catEar.Close();

            catEye.MoveTo(0, 0);
            catEye.ArcTo(50, 50, 0, SKPathArcSize.Small, SKPathDirection.Clockwise, 50, 0);
            catEye.ArcTo(50, 50, 0, SKPathArcSize.Small, SKPathDirection.Clockwise, 0, 0);
            catEye.Close();

            catPupil.MoveTo(25, -5);
            catPupil.ArcTo(6, 6, 0, SKPathArcSize.Small, SKPathDirection.Clockwise, 25, 5);
            catPupil.ArcTo(6, 6, 0, SKPathArcSize.Small, SKPathDirection.Clockwise,25, -5);
            catPupil.Close();

            catTail.MoveTo(0, 100);
            catTail.CubicTo(50, 200, 0, 250, -50, 200);


            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
              {
                  canvasView.InvalidateSurface();
                  return true;
              });
        }

        void Handle_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);
            var width = e.Info.Width;
            var height = e.Info.Height;
            canvas.Translate(width / 2, height / 2);
            canvas.Scale(Math.Min(width/210f, height / 520f));

            var date = DateTime.Now;
            //Background
            canvas.DrawCircle(0, 0, 100, blackFill);

            //Hour and minute marks
            for(int angle = 0; angle < 360; angle += 6)
            {
                canvas.DrawCircle(0, -90, angle % 30 == 0 ? 4 : 2, whiteFill);
                canvas.RotateDegrees(6);
            }

            //Head
            canvas.DrawCircle(0, -160,  70, blackFill);
            for (int i = 0; i < 2; i++)
            {
                canvas.Save();
                canvas.Scale(2 * i - 1, 1);

                canvas.Save();
                canvas.Translate(-65, -255);
                canvas.DrawPath(catEar, blackFill);
                canvas.Restore();

                canvas.Save();
                canvas.Translate(10, -170);
                canvas.DrawPath(catEye, greenFill);
                canvas.DrawPath(catPupil, blackFill);
                canvas.Restore();

                canvas.DrawLine(10, -120, 100, -100, whiteLines);
                canvas.DrawLine(10, -125, 100, -120, whiteLines);
                canvas.DrawLine(10, -130, 100, -140, whiteLines);
                canvas.DrawLine(10, -135, 100, -160, whiteLines);

                canvas.Restore();
            }

            //Tail
            canvas.DrawPath(catTail, blackStroke);

            //Hour hand
            canvas.Save();
            canvas.RotateDegrees(30 * date.Hour + date.Minute / 2f);
            whiteLines.StrokeWidth = 15;
            canvas.DrawLine(0, 0, 0, -50, whiteLines);
            canvas.Restore();

            //Minute hand
            canvas.Save();
            canvas.RotateDegrees(6 * date.Minute + date.Second/10f);
            whiteLines.StrokeWidth = 10;
            canvas.DrawLine(0, 0, 0, -70, whiteLines);
            canvas.Restore();

            //Second hand
            canvas.Save();
            canvas.RotateDegrees(6 * date.Second + date.Millisecond / 1000f);
            whiteLines.StrokeWidth = 2;
            canvas.DrawLine(0, 0, 0, -80, whiteLines);
            canvas.Restore();

        }
    }
}
