using Kdd.Common;
using Kdd.Common.Extensions;
using Kdd.UIKit.Forms.Controls;
using Kdd.UIKit.Forms.Ios.Controls;
using Kdd.UIKit.Forms.Ios.Renderers;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SafeSlider), typeof(SafeSliderRenderer))]

namespace Kdd.UIKit.Forms.Ios.Renderers
{
    public class SafeSliderRenderer : ViewRenderer<SafeSlider, KddUISlider>
    {
        private const float TrackHeight = 9;
        private const float TrackOffset = 20;

        protected virtual UIColor DefaultMinTrackColor { get; set; }
        protected virtual UIColor DefaultMaxTrackColor { get; set; }
        protected virtual UIColor DefaultThumbColor { get; set; }
        protected virtual UITapGestureRecognizer SliderTapRecognizer { get; set; }
        protected virtual DoubleRange SliderValueRange { get; set; }
        protected virtual bool IsUserInteracting { get; set; }
        protected virtual bool IsDisposed { get; set; }

        protected override void OnElementChanged(ElementChangedEventArgs<SafeSlider> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement is null)
            {
                return;
            }

            if (Control == null)
            {
                var slider = new KddUISlider
                {
                    Continuous = true,
                    TrackHeight = TrackHeight,
                    TrackOffset = TrackOffset
                };
                SetNativeControl(slider);
                Control.SizeToFit();

                DefaultMinTrackColor = Control.MinimumTrackTintColor;
                DefaultMaxTrackColor = Control.MaximumTrackTintColor;
                DefaultThumbColor = Control.ThumbTintColor;

                Control.AddTarget(OnControlValueChanged, UIControlEvent.ValueChanged);
                Control.AddTarget(OnTouchDownControlEvent, UIControlEvent.TouchDown);
                Control.AddTarget(OnTouchUpControlEvent, UIControlEvent.TouchUpInside | UIControlEvent.TouchUpOutside);
            }

            UpdateValueRange();
            UpdateProgress();
            UpdateSliderStyle();
            UpdateTapRecognizer();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case nameof(SafeSlider.ValueRange):
                case nameof(SafeSlider.ValueDecimals):
                    UpdateValueRange();
                    UpdateProgress();
                    break;
                case nameof(SafeSlider.Value) when !IsUserInteracting:
                    UpdateProgress();
                    break;
                case nameof(SafeSlider.MinimumTrackColor):
                    UpdateMinimumTrackColor();
                    break;
                case nameof(SafeSlider.MaximumTrackColor):
                    UpdateMaximumTrackColor();
                    break;
                case nameof(SafeSlider.ThumbColor):
                case nameof(SafeSlider.ThumbImageSource):
                    UpdateThumb();
                    break;
            }
        }

        protected virtual void UpdateSliderStyle()
        {
            UpdateMinimumTrackColor();
            UpdateMaximumTrackColor();
            UpdateThumb();
        }

        protected virtual void UpdateTapRecognizer()
        {
            if (SliderTapRecognizer != null)
            {
                Control.RemoveGestureRecognizer(SliderTapRecognizer);
            }

            SliderTapRecognizer = new UITapGestureRecognizer(OnTrackTapped);
            Control.AddGestureRecognizer(SliderTapRecognizer);
        }

        protected virtual void UpdateMinimumTrackColor()
        {
            Control.MinimumTrackTintColor = Element.MinimumTrackColor == Color.Default ? DefaultMinTrackColor : Element.MinimumTrackColor.ToUIColor();
        }

        protected virtual void UpdateMaximumTrackColor()
        {
            Control.MaximumTrackTintColor = Element.MaximumTrackColor == Color.Default ? DefaultMaxTrackColor : Element.MaximumTrackColor.ToUIColor();
        }

        protected virtual void UpdateThumb()
        {
            Control.ThumbTintColor = Element.ThumbColor == Color.Default ? DefaultThumbColor : Element.ThumbColor.ToUIColor();

            if (Element.ThumbImageSource is null)
            {
                return;
            }

            var thumbImage = CreateUIImage(Element.ThumbImageSource);
            Control?.SetThumbImage(thumbImage, UIControlState.Normal);
            Control?.SetThumbImage(thumbImage, UIControlState.Highlighted);
            ((IVisualElementController)Element).NativeSizeChanged();
        }

        protected virtual void OnTrackTapped(UITapGestureRecognizer recognizer)
        {
            if (Control is null)
            {
                return;
            }

            var tappedLocation = recognizer.LocationInView(Control);
            if (tappedLocation != null)
            {
                var range = Control.MaxValue - Control.MinValue;
                var coefficient = range / (Control.Frame.Size.Width - TrackOffset * 2);
                var tapPosition = tappedLocation.X - Control.Frame.X - TrackOffset;
                var value = tapPosition * coefficient + Control.MinValue;

                value = (nfloat)Math.Clamp(value, Control.MinValue, Control.MaxValue);
                ((IElementController)Element).SetValueFromRenderer(SafeSlider.ValueProperty, value);
            }
        }

        protected virtual void OnControlValueChanged(object sender, EventArgs eventArgs)
        {
            ((IElementController)Element).SetValueFromRenderer(SafeSlider.ValueProperty, Control.Value);
        }

        protected virtual void OnTouchDownControlEvent(object sender, EventArgs e)
        {
            IsUserInteracting = true;
            ((ISliderController)Element)?.SendDragStarted();
        }

        protected virtual void OnTouchUpControlEvent(object sender, EventArgs e)
        {
            IsUserInteracting = false;
            ((ISliderController)Element)?.SendDragCompleted();
        }

        protected virtual void UpdateValueRange()
        {
            Control.MaxValue = (float)Element.ValueRange.Maximum;
            Control.MinValue = (float)Element.ValueRange.Minimum;

            SliderValueRange = Element.ValueRange;
        }

        protected virtual void UpdateProgress()
        {
            Control.Value = (float)Element.Value.Clamp(SliderValueRange);
        }

        protected virtual UIImage CreateUIImage(ImageSource imageSource)
        {
            if (imageSource is FileImageSource fileImageSource)
            {
                return UIImage.FromBundle(fileImageSource.File);
            }

            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (Control != null)
                {
                    if (SliderTapRecognizer != null)
                    {
                        Control.RemoveGestureRecognizer(SliderTapRecognizer);
                        SliderTapRecognizer = null;
                    }

                    Control.RemoveTarget(OnControlValueChanged, UIControlEvent.ValueChanged);
                    Control.RemoveTarget(OnTouchDownControlEvent, UIControlEvent.TouchDown);
                    Control.RemoveTarget(OnTouchUpControlEvent, UIControlEvent.TouchUpInside | UIControlEvent.TouchUpOutside);
                }
            }

            base.Dispose(disposing);
            IsDisposed = true;
        }
    }
}