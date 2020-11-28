using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Kdd.UIKit.Forms.Controls
{
    public class ChippedSelector : ContentView
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(ChippedSelector), propertyChanged: OnItemsSourcePropertyChanged);
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(ChippedSelector), defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty ChipHeightRequestProperty = BindableProperty.Create(nameof(ChipHeightRequest), typeof(double), typeof(ChippedSelector), 40.0);
        public static readonly BindableProperty ChipWidthRequestProperty = BindableProperty.Create(nameof(ChipWidthRequest), typeof(double?), typeof(ChippedSelector));
        public static readonly BindableProperty ChipsSpacingProperty = BindableProperty.Create(nameof(ChipsSpacing), typeof(double), typeof(ChippedSelector), 10.0);
        public static readonly BindableProperty ChipCornerRadiusProperty = BindableProperty.Create(nameof(ChipCornerRadius), typeof(double?), typeof(ChippedSelector));
        public static readonly BindableProperty ChipColorProperty = BindableProperty.Create(nameof(ChipColor), typeof(Color), typeof(ChippedSelector), Color.LightGray);
        public static readonly BindableProperty SelectedChipColorProperty = BindableProperty.Create(nameof(SelectedChipColor), typeof(Color), typeof(ChippedSelector), Color.DeepSkyBlue);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ChippedSelector), Color.DarkGray);
        public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(ChippedSelector), Color.White);
        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(ChippedSelector), FontAttributes.None);
        public static readonly BindableProperty SelectedFontAttributesProperty = BindableProperty.Create(nameof(SelectedFontAttributes), typeof(FontAttributes), typeof(ChippedSelector), FontAttributes.None);
        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(ChippedSelector), Label.FontFamilyProperty.DefaultValue);
        public static readonly BindableProperty SelectedFontFamilyProperty = BindableProperty.Create(nameof(SelectedFontFamily), typeof(string), typeof(ChippedSelector), Label.FontFamilyProperty.DefaultValue);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(ChippedSelector), 15.0);
        public static readonly BindableProperty SelectedFontSizeProperty = BindableProperty.Create(nameof(SelectedFontSize), typeof(double), typeof(ChippedSelector), 15.0);
        public static readonly BindableProperty IsDeselectionEnabledProperty = BindableProperty.Create(nameof(IsDeselectionEnabled), typeof(bool), typeof(ChippedSelector), true);

        private readonly List<object> _items = new List<object>();
        private readonly List<Chip> _chips = new List<Chip>();
        private readonly FlexLayout _flexLayout = new FlexLayout();

        public ChippedSelector()
        {
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Start;

            Content = _flexLayout;

            _flexLayout.Wrap = FlexWrap.Wrap;
        }

        public event EventHandler ItemTapped;
        public event EventHandler ItemSelected;

        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty) as IEnumerable;
            set => SetValue(ItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public double ChipHeightRequest
        {
            get => (double)GetValue(ChipHeightRequestProperty);
            set => SetValue(ChipHeightRequestProperty, value);
        }

        public double? ChipWidthRequest
        {
            get => GetValue(ChipWidthRequestProperty) as double?;
            set => SetValue(ChipWidthRequestProperty, value);
        }

        public double ChipsSpacing
        {
            get => (double)GetValue(ChipsSpacingProperty);
            set => SetValue(ChipsSpacingProperty, value);
        }

        public double? ChipCornerRadius
        {
            get => GetValue(ChipCornerRadiusProperty) as double?;
            set => SetValue(ChipCornerRadiusProperty, value);
        }

        public Color ChipColor
        {
            get => (Color)GetValue(ChipColorProperty);
            set => SetValue(ChipColorProperty, value);
        }

        public Color SelectedChipColor
        {
            get => (Color)GetValue(SelectedChipColorProperty);
            set => SetValue(SelectedChipColorProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public Color SelectedTextColor
        {
            get => (Color)GetValue(SelectedTextColorProperty);
            set => SetValue(SelectedTextColorProperty, value);
        }

        public FontAttributes FontAttributes
        {
            get => (FontAttributes)GetValue(FontAttributesProperty);
            set => SetValue(FontAttributesProperty, value);
        }

        public FontAttributes SelectedFontAttributes
        {
            get => (FontAttributes)GetValue(SelectedFontAttributesProperty);
            set => SetValue(SelectedFontAttributesProperty, value);
        }

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public string SelectedFontFamily
        {
            get => (string)GetValue(SelectedFontFamilyProperty);
            set => SetValue(SelectedFontFamilyProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public double SelectedFontSize
        {
            get => (double)GetValue(SelectedFontSizeProperty);
            set => SetValue(SelectedFontSizeProperty, value);
        }

        public bool IsDeselectionEnabled
        {
            get => (bool)GetValue(IsDeselectionEnabledProperty);
            set => SetValue(IsDeselectionEnabledProperty, value);
        }

        private static void OnItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((ChippedSelector)bindable).OnItemsSourceChanged(oldValue as IEnumerable, newValue as IEnumerable);
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            UpdateViews();
        }

        protected virtual void OnItemTapped(object sender, EventArgs args)
        {
            var tappedIndex = _chips.IndexOf((Chip)sender);
            var tappedItem = _items[tappedIndex];

            ItemTapped?.Invoke(this, EventArgs.Empty);

            if (Equals(tappedItem, SelectedItem) && IsDeselectionEnabled)
            {
                SelectedItem = null;
                return;
            }

            SelectedItem = tappedItem;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            var itemsLazy = new Lazy<List<object>>(() => _items.ToList());

            switch (propertyName)
            {
                case nameof(SelectedItem) when SelectedItem != null:
                    ItemSelected?.Invoke(this, EventArgs.Empty);
                    UpdateChipsAppearance(itemsLazy.Value);
                    break;
                case nameof(SelectedItem):
                case nameof(ChipHeightRequest):
                case nameof(ChipWidthRequest):
                case nameof(ChipsSpacing):
                case nameof(ChipCornerRadius):
                case nameof(ChipColor):
                case nameof(SelectedChipColor):
                case nameof(TextColor):
                case nameof(SelectedTextColor):
                case nameof(FontAttributes):
                case nameof(SelectedFontAttributes):
                case nameof(FontFamily):
                case nameof(SelectedFontFamily):
                case nameof(FontSize):
                case nameof(SelectedFontSize):
                    UpdateChipsAppearance(itemsLazy.Value);
                    break;
            }
        }

        protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= OnItemsSourceCollectionChanged;
            }

            _items.Clear();
            _items.AddRange(ItemsSource?.Cast<object>().ToList() ?? new List<object>());

            UpdateViews();

            if (newValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += OnItemsSourceCollectionChanged;
            }
        }

        protected virtual void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            _items.Clear();
            _items.AddRange(ItemsSource?.Cast<object>().ToList() ?? new List<object>());

            UpdateViews();
        }

        protected virtual void UpdateViews()
        {
            _items.Clear();
            _items.AddRange(ItemsSource?.Cast<object>().ToList() ?? new List<object>());

            var items = _items.ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var chip in _chips)
                {
                    chip.GestureRecognizers.OfType<TapGestureRecognizer>().First().Tapped -= OnItemTapped;
                    chip.GestureRecognizers.Clear();
                }

                _chips.Clear();
                _flexLayout.Children.Clear();

                if (Parent is null)
                {
                    return;
                }

                foreach (var item in items)
                {
                    var chip = new Chip(item?.ToString(), OnItemTapped);
                    _chips.Add(chip);
                    _flexLayout.Children.Add(chip);
                }

                UpdateChipsAppearance(items);
            });
        }

        private void UpdateChipsAppearance(List<object> items)
        {
            if (Parent is null)
            {
                return;
            }

            var selectedItem = SelectedItem;
            var chipHeightRequest = ChipHeightRequest;
            var chipWidthRequest = ChipWidthRequest;
            var chipsSpacing = ChipsSpacing;
            var chipCornerRadius = ChipCornerRadius ?? chipHeightRequest / 2;
            var chipColor = ChipColor;
            var selectedChipColor = SelectedChipColor;
            var textColor = TextColor;
            var selectedTextColor = SelectedTextColor;
            var fontAttributes = FontAttributes;
            var selectedFontAttributes = SelectedFontAttributes;
            var fontFamily = FontFamily;
            var selectedFontFamily = SelectedFontFamily;
            var fontSize = FontSize;
            var selectedFontSize = SelectedFontSize;

            for (var i = 0; i < items.Count; ++i)
            {
                var item = items[i];
                var chip = _chips[i];

                var isItemSelected = Equals(selectedItem, item);

                chip.HeightRequest = chipHeightRequest;

                if (chipWidthRequest.HasValue)
                {
                    chip.WidthRequest = chipWidthRequest.Value;
                }

                chip.MinimumWidthRequest = chipHeightRequest;
                chip.Margin = chipsSpacing / 2.0;
                chip.CornerRadius = (float)chipCornerRadius;
                chip.BackgroundColor = isItemSelected ? selectedChipColor : chipColor;
                chip.TextColor = isItemSelected ? selectedTextColor : textColor;
                chip.FontAttributes = isItemSelected ? selectedFontAttributes : fontAttributes;
                chip.FontFamily = isItemSelected ? selectedFontFamily : fontFamily;
                chip.FontSize = isItemSelected ? selectedFontSize : fontSize;
            }

            _flexLayout.Padding = -chipsSpacing / 2.0;
        }

        private class Chip : Xamarin.Forms.Frame
        {
            private readonly Label _label;

            public Chip(string text, EventHandler itemTappedHandler)
            {
                HasShadow = false;
                BorderColor = Color.Transparent;
                Padding = 0;

                _label = new Label
                {
                    Text = text,
                    MaxLines = 1,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };

                Content = _label;

                var recognizer = new TapGestureRecognizer();
                recognizer.Tapped += itemTappedHandler;
                GestureRecognizers.Add(recognizer);
            }

            protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                base.OnPropertyChanged(propertyName);

                switch (propertyName)
                {
                    case nameof(CornerRadius):
                        _label.Margin = new Thickness(CornerRadius * 0.9, 0.0);
                        break;
                }
            }

            public Color TextColor
            {
                get => _label.TextColor;
                set => _label.TextColor = value;
            }

            public FontAttributes FontAttributes
            {
                get => _label.FontAttributes;
                set => _label.FontAttributes = value;
            }

            public string FontFamily
            {
                get => _label.FontFamily;
                set => _label.FontFamily = value;
            }

            public double FontSize
            {
                get => _label.FontSize;
                set => _label.FontSize = value;
            }
        }
    }
}
