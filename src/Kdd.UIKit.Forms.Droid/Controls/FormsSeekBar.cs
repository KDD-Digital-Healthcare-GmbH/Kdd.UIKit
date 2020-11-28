using Android.Content;
using Android.Views;
using Android.Widget;

namespace Kdd.UIKit.Forms.Droid.Controls
{
    // NOTE Copied from Xamarin.Forms source code
    public class FormsSeekBar : SeekBar
    {
        private bool _isTouching;

        public FormsSeekBar(Context context) : base(context)
        {
        }

        public override bool Pressed
        {
            get => base.Pressed;
            set
            {
                if (!_isTouching)
                {
                    return;
                }

                base.Pressed = value;
                _isTouching = value;
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    _isTouching = true;
                    break;
                case MotionEventActions.Up:
                    Pressed = false;
                    break;
            }

            return base.OnTouchEvent(e);
        }
    }
}