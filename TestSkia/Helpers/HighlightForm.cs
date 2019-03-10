using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace TestSkia
{
    public class HighlightForm
    {
        readonly HighlightSettings _highlightSettings;
        SKPaint _skPaint;
        HighlightState _highlightState;

        public HighlightForm(HighlightSettings highlightSettings)
        {
            _highlightSettings = highlightSettings;
        }

        public void Draw(SKCanvasView skCanvasView, SKCanvas skCanvas)
        {
            skCanvas.Clear();

            if (_highlightState == null)
            {
                return;
            }
            if (_skPaint == null)
            {
                _skPaint = CreateHighlightSkPaint(skCanvasView, _highlightSettings, _highlightState.HighlightPath);
            }
            var strokeDash = _highlightState.StrokeDash;
            // Comment the next line to see whole path without dash effect
            _skPaint.PathEffect = SKPathEffect.CreateDash(strokeDash.Intervals, strokeDash.Phase);
            skCanvas.DrawPath(_highlightState.HighlightPath.Path, _skPaint);
        }

        public void Invalidate(SKCanvasView skCanvasView, Layout<View> formLayout)
        {
            if (_highlightState == null)
            {
                return;
            }

            var viewToHighlight = _highlightState.HighlightPath.GetView(formLayout.Children, _highlightState.CurrHighlightedViewId);
            _highlightState = null;
            HighlightElement(viewToHighlight, skCanvasView, formLayout);
        }

        public void HighlightElement(View viewToHighlight, SKCanvasView skCanvasView, Layout<View> formLayout)
        {
            var layoutChildren = formLayout.Children;

            if (_highlightState == null)
            {
                _highlightState = new HighlightState()
                {
                    HighlightPath = HighlightPath.Create(skCanvasView, layoutChildren, _highlightSettings.StrokeWidth)
                };
            }

            var highlightPath = _highlightState.HighlightPath;
            var currHighlightViewId = _highlightState.CurrHighlightedViewId;
            var iViewIdToHighlight = highlightPath.GetViewId(layoutChildren, viewToHighlight);
            if (currHighlightViewId == iViewIdToHighlight)
            {
                return;
            }

            var fromDash = currHighlightViewId != -1
                ? _highlightState.StrokeDash
                : new StrokeDash(highlightPath.GetDashForView(layoutChildren, iViewIdToHighlight));
            _highlightState.CurrHighlightedViewId = iViewIdToHighlight;

            var toDash = new StrokeDash(highlightPath.GetDashForView(layoutChildren, viewToHighlight));
            DrawDash(skCanvasView, fromDash, toDash);
        }

        void DrawDash(SKCanvasView skCanvasView, StrokeDash fromDash, StrokeDash toDash)
        {
            if (fromDash != null)
            {
                var anim = new StrokeDashAnimation(fromDash, toDash, _highlightSettings.AnimationDuration);
                anim.Start((strokeDashToDraw) => RequestDraw(skCanvasView, strokeDashToDraw));
            }
            else
            {
                RequestDraw(skCanvasView, toDash);
            }
        }

        void RequestDraw(SKCanvasView skCanvasView, StrokeDash strokeDashToDraw)
        {
            _highlightState.StrokeDash = strokeDashToDraw;
            skCanvasView.InvalidateSurface();
        }

        static SKPaint CreateHighlightSkPaint(SKCanvasView skCanvasView, HighlightSettings highlightSettings, HighlightPath highlightPath)
        {
            var skPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = (float)skCanvasView.FromPixels(new Point(0, highlightSettings.StrokeWidth)).Y
            };

            var firstDashIntervalOn = highlightPath.FirstDash.Intervals[0];
            skPaint.Shader = SKShader.CreateLinearGradient(
                                new SKPoint(firstDashIntervalOn * 0.30f, 0),
                                new SKPoint(firstDashIntervalOn, 0),
                                new SKColor[] 
                                {
                                    highlightSettings.StrokeStartColor.ToSKColor(),
                                    highlightSettings.StrokeEndColor.ToSKColor()
                                },
                                new float[] { 0, 1 },
                                SKShaderTileMode.Clamp
                            );

            return skPaint;
        }
    }
}
