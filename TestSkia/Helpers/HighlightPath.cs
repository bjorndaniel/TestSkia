﻿using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace TestSkia
{
    public class HighlightPath
    {
        readonly Dictionary<int, StrokeDash> _strokeDashList = new Dictionary<int, StrokeDash>();

        public StrokeDash FirstDash => _strokeDashList.Values.FirstOrDefault();

        public int DashCount => _strokeDashList.Count;

        public SKPath Path { get; private set; }

        public StrokeDash GetDashForView(IList<View> layoutChildren, View view) => _strokeDashList[GetViewId(layoutChildren, view)];

        public StrokeDash GetDashForView(IList<View> layoutChildren, int viewId) => _strokeDashList[viewId];

        public StrokeDash GetDash(int i) => _strokeDashList.ElementAt(i).Value;

        public void SetDashForView(IList<View> layoutChildren, View view, StrokeDash strokeDash) => _strokeDashList[GetViewId(layoutChildren, view)] = strokeDash;

        public int GetViewId(IList<View> layoutChildren, View view) => layoutChildren.IndexOf(view);

        public View GetView(IList<View> layoutChildren, int currHighlightedViewId) => layoutChildren.ElementAt(currHighlightedViewId);

        public static HighlightPath Create(SKCanvasView skCanvasView, IList<View> layoutChildren, double strokeWidth)
        {
            var path = new SKPath();
            var highlightPath = new HighlightPath()
            {
                Path = path
            };

            strokeWidth = (float)skCanvasView.FromPixels(new Point(0, strokeWidth)).Y;

            foreach (var view in layoutChildren)
            {
                var iStrokeDashCount = highlightPath.DashCount;
                var viewBounds = skCanvasView.FromPixels(view.Bounds);

                if (!(view is Entry) && !(view is Button))
                {
                    continue;
                }

                if (path.Points.Length == 0)
                {
                    // Move path point at the left and below of the view
                    path.MoveTo((float)viewBounds.X, (float)viewBounds.Y + (float)viewBounds.Height + (float)strokeWidth / 2);
                }

                var xCurr = path.LastPoint.X;
                var yCurr = path.LastPoint.Y;

                // Add arch for views except first one
                if (iStrokeDashCount > 0)
                {
                    var d = iStrokeDashCount % 2 == 0 ? -1 : 1;
                    var arcHeight = (float)viewBounds.Y + (float)viewBounds.Height - path.LastPoint.Y + (float)strokeWidth / 2;
                    path.ArcTo(new SKRect(xCurr - arcHeight / 2, yCurr, xCurr + arcHeight / 2, yCurr + arcHeight), -90, 180 * d, false);
                }

                var dashOffset = new SKPathMeasure(path).Length;
                // Add line below the view
                // If it's not the first view, the start point is the end from arc end point 
                //  and line direction is either to view start or view end
                path.LineTo((float)viewBounds.X + (float)viewBounds.Width * (iStrokeDashCount % 2 == 0 ? 1 : 0), path.LastPoint.Y);

                if (view is Button)
                {
                    xCurr = path.LastPoint.X;
                    yCurr = path.LastPoint.Y;

                    // Draw arc from below button to above button
                    var d = iStrokeDashCount % 2 == 0 ? -1 : 1;
                    var arcHeight = (float)viewBounds.Height + (float)strokeWidth;
                    path.ArcTo(new SKRect(xCurr - arcHeight / 2, yCurr - arcHeight, xCurr + arcHeight / 2, yCurr), 90, 180 * d, false);

                    // Draw horizontal line above the button
                    path.LineTo(xCurr + (float)viewBounds.Width * d, path.LastPoint.Y);

                    // Draw arc pointing down
                    xCurr = path.LastPoint.X;
                    yCurr = path.LastPoint.Y;
                    path.ArcTo(new SKRect(xCurr - arcHeight / 2, yCurr, xCurr + arcHeight / 2, yCurr + arcHeight), -90, 180 * d, false);
                }

                var solidDashLength = new SKPathMeasure(path).Length - dashOffset;
                var strokeDash = new StrokeDash(new float[] { solidDashLength, 0 }, -dashOffset);
                highlightPath.SetDashForView(layoutChildren, view, strokeDash);
            }

            // Compute the 2nd value of interval, which is the length of remaining path
            var pathLength = new SKPathMeasure(path).Length;

            for (int i = 0; i < highlightPath.DashCount; ++i)
            {
                var d = highlightPath.GetDash(i);
                d.Intervals[1] = pathLength - d.Intervals[0];
            }

            return highlightPath;
        }
    }
}