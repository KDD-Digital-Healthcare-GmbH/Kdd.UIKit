
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Kdd.Common;
using Kdd.Common.Extensions;
using Kdd.UIKit.Forms.Controls;
using Kdd.UIKit.Forms.Droid.Controls;
using Kdd.UIKit.Forms.Droid.Extensions;
using Kdd.UIKit.Forms.Droid.Renderers;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(SafeSlider), typeof(SafeSliderRenderer))]

namespace Kdd.UIKit.Forms.Droid.Renderers
{
    public class SafeSliderRenderer : FormsSeekBar, IVisualElementRenderer, SeekBar.IOnSeekBarChangeListener
    {
        protected const int SeekBarDelta = 1000;

        protected readonly Drawable _defaultThumbDrawable;
        protected readonly ColorFilter _defaultThumbColorFilter;

        protected readonly ColorStateList _defaultProgressTintList;
        protected readonly PorterDuff.Mode _defaultProgressTintMode;

        protected readonly ColorStateList _defaultProgressBackgroundTintList;
        protected readonly PorterDuff.Mode _defaultProgressBackgroundTintMode;

        protected virtual int? DefaultLabelFor { get; set; }
        protected virtual bool IsDisposed { get; set; }
        protected virtual bool IsUserInteracting { get; set; }
        protected virtual DoubleRange SliderValueRange { get; set; }

        public SafeSliderRenderer(Context context) : base(context)
        {
            _defaultThumbDrawable = Thumb;
            _defaultThumbColorFilter = Thumb.GetColorFilter();

            _defaultProgressTintList = ProgressTintList;
            _defaultProgressTintMode = ProgressTintMode;

            _defaultProgressBackgroundTintList = ProgressBackgroundTintList;
            _defaultProgressBackgroundTintMode = ProgressBackgroundTintMode;

            Max = SeekBarDelta;
            DuplicateParentStateEnabled = false;
            SetOnSeekBarChangeListener(this);
        }

        public virtual event EventHandler<VisualElementChangedEventArgs> ElementChanged;
        public virtual event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

        VisualElement IVisualElementRenderer.Element => Element;

        private SafeSlider _element;
        protected virtual SafeSlider Element
        {
            get => _element;
            set
            {
                if (IsDisposed || _element == value)
                {
                    return;
                }

                var elementChangedEventArgs = new ElementChangedEventArgs<SafeSlider>(_element, value);
                _element = value;

                OnElementChanged(elementChangedEventArgs);
            }
        }

        public virtual VisualElementTracker Tracker { get; private set; }

        public virtual ViewGroup ViewGroup => null;

        public virtual AView View => this;

        protected virtual ISliderController SliderController => Element;

        protected virtual IElementController ElementController => Element;

        void IOnSeekBarChangeListener.OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            OnProgressChanged(seekBar, progress, fromUser);
        }

        void IOnSeekBarChangeListener.OnStartTrackingTouch(SeekBar seekBar)
        {
            OnStartTrackingTouch(seekBar);
        }

        void IOnSeekBarChangeListener.OnStopTrackingTouch(SeekBar seekBar)
        {
            OnStopTrackingTouch(seekBar);
        }

        protected virtual void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            if (!IsUserInteracting)
            {
                return;
            }

            var sliderValue = CalculateSliderValue(Progress, Element.ValueDecimals);
            ElementController?.SetValueFromRenderer(SafeSlider.ValueProperty, sliderValue);
        }

        protected virtual void OnStartTrackingTouch(SeekBar seekBar)
        {
            IsUserInteracting = true;
            SliderController?.SendDragStarted();
        }

        protected virtual void OnStopTrackingTouch(SeekBar seekBar)
        {
            UpdateProgress();
            IsUserInteracting = false;
            SliderController?.SendDragCompleted();
        }

        public virtual SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            Measure(widthConstraint, heightConstraint);
            return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), new Size());
        }

        public virtual void SetLabelFor(int? id)
        {
            DefaultLabelFor ??= LabelFor;
            LabelFor = id ?? DefaultLabelFor.Value;
        }

        public virtual void UpdateLayout()
        {
            Tracker?.UpdateLayout();
        }

        public virtual void SetElement(VisualElement element)
        {
            if (!(element is SafeSlider slider))
            {
                throw new ArgumentException($"{nameof(element)} should inherit {nameof(SafeSlider)}");
            }

            Element = slider;
        }

        protected virtual void OnElementChanged(ElementChangedEventArgs<SafeSlider> e)
        {
            ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));

            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;
            }

            if (e.NewElement is null || IsDisposed)
            {
                return;
            }

            Tracker ??= new VisualElementTracker(this);

            SetContent();
            e.NewElement.PropertyChanged += OnElementPropertyChanged;
        }

        protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            ElementPropertyChanged?.Invoke(this, e);

            switch (e.PropertyName)
            {
                case nameof(SafeSlider.ValueRange):
                case nameof(SafeSlider.ValueDecimals):
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

        protected virtual void SetContent()
        {
            UpdateProgress();
            UpdateMinimumTrackColor();
            UpdateMaximumTrackColor();
            UpdateThumb();
        }

        protected virtual void UpdateProgress()
        {
            SliderValueRange = Element.ValueRange;
            Progress = CalculateSeekBarProgress(Element.Value);
        }

        protected virtual void UpdateMinimumTrackColor()
        {
            if (Element is null)
            {
                return;
            }

            if (Element.MinimumTrackColor.IsDefault)
            {
                ProgressTintList = _defaultProgressTintList;
                ProgressTintMode = _defaultProgressTintMode;
                return;
            }

            ProgressTintList = ColorStateList.ValueOf(Element.MinimumTrackColor.ToAndroid());
            ProgressTintMode = PorterDuff.Mode.SrcIn;
        }

        protected virtual void UpdateMaximumTrackColor()
        {
            if (Element is null)
            {
                return;
            }

            if (Element.MaximumTrackColor.IsDefault)
            {
                ProgressBackgroundTintList = _defaultProgressBackgroundTintList;
                ProgressBackgroundTintMode = _defaultProgressBackgroundTintMode;
                return;
            }

            ProgressBackgroundTintList = ColorStateList.ValueOf(Element.MaximumTrackColor.ToAndroid());
            ProgressBackgroundTintMode = PorterDuff.Mode.SrcIn;
        }

        protected virtual void UpdateThumb()
        {
            var thumbImage = Element.ThumbImageSource;
            if (thumbImage is null || thumbImage.IsEmpty)
            {
                Thumb.SetColorFilter(Element.ThumbColor, _defaultThumbColorFilter, BlendMode.SrcIn);
                return;
            }
            
            FormsResourceManager.ApplyDrawableAsync(this, SafeSlider.ThumbImageSourceProperty, Context, drawable =>
            {
                SetThumb(drawable ?? _defaultThumbDrawable);
            });
        }

        protected virtual int CalculateSeekBarProgress(double sliderValue)
        {
            sliderValue = sliderValue.Clamp(SliderValueRange);
            var relativeSliderValue = sliderValue - SliderValueRange.Minimum;
            var relativeSeekBarProgress = relativeSliderValue / SliderValueRange.Delta;
            var seekBarProgress = relativeSeekBarProgress * SeekBarDelta;

            return seekBarProgress.RoundToInt();
        }

        protected virtual double CalculateSliderValue(int seekBarProgress, int decimals)
        {
            var relativeSeekBarProgress = (double)seekBarProgress / SeekBarDelta;
            var relativeSliderValue = relativeSeekBarProgress * SliderValueRange.Delta;
            var sliderValue = SliderValueRange.Minimum + relativeSliderValue;

            return Math.Round(sliderValue, decimals);
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (Element != null)
                {
                    Element.PropertyChanged -= OnElementPropertyChanged;
                }

                if (Tracker != null)
                {
                    Tracker.Dispose();
                    Tracker = null;
                }
            }

            IsDisposed = true;

            base.Dispose(disposing);
        }
    }
}