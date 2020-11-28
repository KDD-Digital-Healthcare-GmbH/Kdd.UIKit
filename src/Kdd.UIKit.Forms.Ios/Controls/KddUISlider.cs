using CoreGraphics;
using UIKit;

namespace Kdd.UIKit.Forms.Ios.Controls
{
    public class KddUISlider : UISlider
    {
        protected const float TrackY = 0;
        protected const float ThumbTopOffset = 28;
        protected const float ThumbLeftOffset = -11;
        protected const float ThumbRightOffset = 19;

        protected virtual float TrackLeftOffset => TrackOffset;

        protected virtual float TrackRightOffset => TrackOffset * -2;

        public virtual float TrackHeight { get; set; }

        public virtual float TrackOffset { get; set; }

        public override CGRect TrackRectForBounds(CGRect forBounds)
        {
            return new CGRect(forBounds.X + TrackLeftOffset, TrackY, forBounds.Width + TrackRightOffset, TrackHeight);
        }

        public override CGRect ThumbRectForBounds(CGRect bounds, CGRect trackRect, float value)
        {
            trackRect = new CGRect(trackRect.X + ThumbLeftOffset, trackRect.Y + ThumbTopOffset, trackRect.Width + ThumbRightOffset, trackRect.Height);
            return base.ThumbRectForBounds(bounds, trackRect, value);
        }
    }
}