using System;
using Xamarin.Forms;

namespace TestSkia
{
    public class StrokeDashAnimation
    {
        StrokeDash _currStrokeDash;

        public StrokeDash From { get; }
        public StrokeDash To { get; }
        public TimeSpan Duration { get; }
        public Easing Easing { get; }

        public StrokeDashAnimation(StrokeDash from, StrokeDash to, TimeSpan duration)
        {
            From = from;
            To = to;
            Duration = duration;
        }

        public void Start(Action<StrokeDash> onValueCallback)
        {
            _currStrokeDash = From;

            var anim = new Animation(_ => onValueCallback(_currStrokeDash))
            {
                {
                    0,
                    0.5,
                    new Animation(v => _currStrokeDash.Phase = (float)v, From.Phase, To.Phase, Easing)
                },
                {
                    0,
                    0.5,
                    new Animation(v => _currStrokeDash.Intervals[0] = (float)v, From.Intervals[0], To.Intervals[0], Easing)
                },
                {
                    0,
                    0.5,
                    new Animation(v => _currStrokeDash.Intervals[1] = (float)v, From.Intervals[1], To.Intervals[1], Easing)
                }
            };
            anim.Commit(Application.Current.MainPage,"highlightAnimation",length: (uint)Duration.TotalMilliseconds);
        }
    }
}
