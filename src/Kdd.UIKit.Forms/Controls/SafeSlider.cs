using Kdd.Common;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kdd.UIKit.Forms.Controls
{
    public class SafeSlider : View, ISliderController
    {
        public static readonly BindableProperty ValueRangeProperty = BindableProperty.Create(nameof(ValueRange), typeof(DoubleRange), typeof(SafeSlider), new DoubleRange(0, 1));
        public static readonly BindableProperty ValueDecimalsProperty = BindableProperty.Create(nameof(ValueDecimals), typeof(int), typeof(SafeSlider), 3);
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(double), typeof(SafeSlider), 0.5, BindingMode.TwoWay, propertyChanged: OnValuePropertyChanged);

        public static readonly BindableProperty MinimumTrackColorProperty = BindableProperty.Create(nameof(MinimumTrackColor), typeof(Color), typeof(SafeSlider), Color.SkyBlue);
        public static readonly BindableProperty MaximumTrackColorProperty = BindableProperty.Create(nameof(MaximumTrackColor), typeof(Color), typeof(SafeSlider), Color.LightGray);
        public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(SafeSlider), Color.DeepSkyBlue);
        public static readonly BindableProperty ThumbImageSourceProperty = BindableProperty.Create(nameof(ThumbImageSource), typeof(ImageSource), typeof(SafeSlider));

        public static readonly BindableProperty DragStartedCommandProperty = BindableProperty.Create(nameof(DragStartedCommand), typeof(ICommand), typeof(SafeSlider));
        public static readonly BindableProperty DragCompletedCommandProperty = BindableProperty.Create(nameof(DragCompletedCommand), typeof(ICommand), typeof(SafeSlider));

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        public event EventHandler DragStarted;
        public event EventHandler DragCompleted;

        public DoubleRange ValueRange
        {
            get => (DoubleRange)GetValue(ValueRangeProperty);
            set => SetValue(ValueRangeProperty, value);
        }

        public int ValueDecimals
        {
            get => (int)GetValue(ValueDecimalsProperty);
            set => SetValue(ValueDecimalsProperty, value);
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public Color MinimumTrackColor
        {
            get => (Color)GetValue(MinimumTrackColorProperty);
            set => SetValue(MinimumTrackColorProperty, value);
        }

        public Color MaximumTrackColor
        {
            get => (Color)GetValue(MaximumTrackColorProperty);
            set => SetValue(MaximumTrackColorProperty, value);
        }

        public Color ThumbColor
        {
            get => (Color)GetValue(ThumbColorProperty);
            set => SetValue(ThumbColorProperty, value);
        }

        public ImageSource ThumbImageSource
        {
            get => (ImageSource)GetValue(ThumbImageSourceProperty);
            set => SetValue(ThumbImageSourceProperty, value);
        }

        public ICommand DragStartedCommand
        {
            get => (ICommand)GetValue(DragStartedCommandProperty);
            set => SetValue(DragStartedCommandProperty, value);
        }

        public ICommand DragCompletedCommand
        {
            get => (ICommand)GetValue(DragCompletedCommandProperty);
            set => SetValue(DragCompletedCommandProperty, value);
        }

        private static void OnValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((SafeSlider)bindable).OnValueChanged((double)oldValue, (double)newValue);
        }

        void ISliderController.SendDragStarted()
        {
            SendDragCompleted();
        }

        void ISliderController.SendDragCompleted()
        {
            SendDragCompleted();
        }

        protected virtual void SendDragStarted()
        {
            if (!IsEnabled)
            {
                return;
            }

            DragStartedCommand?.Execute(null);
            DragStarted?.Invoke(this, null);
        }

        protected virtual void SendDragCompleted()
        {
            if (!IsEnabled)
            {
                return;
            }

            DragCompletedCommand?.Execute(null);
            DragCompleted?.Invoke(this, null);
        }

        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(oldValue, newValue));
        }
    }
}