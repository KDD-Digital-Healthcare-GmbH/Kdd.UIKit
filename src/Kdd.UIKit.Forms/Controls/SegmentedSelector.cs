using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Kdd.UIKit.Forms.Controls
{
    public class SegmentedSelector : AbsoluteLayout
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(SegmentedSelector), propertyChanged: OnItemsSourcePropertyChanged);
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SegmentedSelector), defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty ItemTappedCommandProperty = BindableProperty.Create(nameof(ItemTappedCommand), typeof(ICommand), typeof(SegmentedSelector));
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(SegmentedSelector), 4.0);
        public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create(nameof(StrokeThickness), typeof(double), typeof(SegmentedSelector), 2.0);
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(SegmentedSelector), Color.LightGray);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SegmentedSelector), Color.Gray);
        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(SegmentedSelector), Color.DeepSkyBlue);
        public static readonly BindableProperty FillColorProperty = BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(SegmentedSelector), Color.White);
        public static readonly BindableProperty DividerVerticalPaddingProperty = BindableProperty.Create(nameof(DividerVerticalPadding), typeof(double), typeof(SegmentedSelector), 10.0);
        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(SegmentedSelector), FontAttributes.None);
        public static readonly BindableProperty SelectedFontAttributesProperty = BindableProperty.Create(nameof(SelectedFontAttributes), typeof(FontAttributes), typeof(SegmentedSelector), FontAttributes.None);
        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(SegmentedSelector), Label.FontFamilyProperty.DefaultValue);
        public static readonly BindableProperty SelectedFontFamilyProperty = BindableProperty.Create(nameof(SelectedFontFamily), typeof(string), typeof(SegmentedSelector), Label.FontFamilyProperty.DefaultValue);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SegmentedSelector), 15.0);
        public static readonly BindableProperty SelectedFontSizeProperty = BindableProperty.Create(nameof(SelectedFontSize), typeof(double), typeof(SegmentedSelector), 15.0);
        public static readonly BindableProperty IsDeselectionEnabledProperty = BindableProperty.Create(nameof(IsDeselectionEnabled), typeof(bool), typeof(SegmentedSelector), true);

        private readonly List<object> _items = new List<object>();
        private readonly List<BoxView> _dividerViews = new List<BoxView>();
        private readonly List<Label> _labels = new List<Label>();
        private readonly BoxView _containerOuterView;
        private readonly BoxView _containerInnerView;
        private readonly AbsoluteLayout _dividersContainer;
        private readonly AbsoluteLayout _labelsContainer;
        private readonly BoxView _selectedItemOuterView;
        private readonly BoxView _selectedItemInnerView;

        public SegmentedSelector()
        {
            HeightRequest = 40;

            _containerOuterView = new BoxView
            {
                CornerRadius = CornerRadius + StrokeThickness / 2,
                Color = StrokeColor
            };

            _containerInnerView = new BoxView
            {
                CornerRadius = CornerRadius - StrokeThickness / 2,
                Color = FillColor,
                Margin = StrokeThickness
            };

            Children.Add(_containerOuterView);
            Children.Add(_containerInnerView);

            _dividersContainer = new AbsoluteLayout();
            Children.Add(_dividersContainer, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            _selectedItemOuterView = new BoxView
            {
                CornerRadius = CornerRadius + StrokeThickness / 2,
                Color = SelectedColor
            };

            _selectedItemInnerView = new BoxView
            {
                CornerRadius = CornerRadius - StrokeThickness / 2,
                Color = FillColor,
                Margin = StrokeThickness
            };

            Children.Add(_selectedItemOuterView);
            Children.Add(_selectedItemInnerView);

            _labelsContainer = new AbsoluteLayout();
            Children.Add(_labelsContainer, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
        }

        public event EventHandler<object> ItemTapped;

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

        public ICommand ItemTappedCommand
        {
            get => GetValue(ItemTappedCommandProperty) as ICommand;
            set => SetValue(ItemTappedCommandProperty, value);
        }

        public double CornerRadius
        {
            get => (double)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public Color StrokeColor
        {
            get => (Color)GetValue(StrokeColorProperty);
            set => SetValue(StrokeColorProperty, value);
        }

        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        public double DividerVerticalPadding
        {
            get => (double)GetValue(DividerVerticalPaddingProperty);
            set => SetValue(DividerVerticalPaddingProperty, value);
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
            ((SegmentedSelector)bindable).OnItemsSourceChanged(oldValue as IEnumerable, newValue as IEnumerable);
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            UpdateViews();
        }

        protected virtual void OnItemTapped(object sender, EventArgs args)
        {
            var tappedIndex = _labels.IndexOf((Label)sender);
            var tappedItem = _items[tappedIndex];

            ItemTappedCommand?.Execute(tappedItem);
            ItemTapped?.Invoke(this, tappedItem);

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
                case nameof(Height):
                case nameof(Width):
                case nameof(StrokeThickness):
                    UpdateContainerViewsAppearance();
                    UpdateDividerViewsAppearance(itemsLazy.Value);
                    UpdateLabelsAppearance(itemsLazy.Value);
                    UpdateSelectedItemViewsAppearance(itemsLazy.Value);
                    break;
                case nameof(SelectedItem):
                case nameof(SelectedColor):
                    UpdateLabelsAppearance(itemsLazy.Value);
                    UpdateSelectedItemViewsAppearance(itemsLazy.Value);
                    break;
                case nameof(CornerRadius):
                case nameof(FillColor):
                    UpdateContainerViewsAppearance();
                    UpdateSelectedItemViewsAppearance(itemsLazy.Value);
                    break;
                case nameof(StrokeColor):
                    UpdateContainerViewsAppearance();
                    UpdateDividerViewsAppearance(itemsLazy.Value);
                    break;
                case nameof(SelectedFontAttributes):
                case nameof(SelectedFontFamily):
                case nameof(SelectedFontSize):
                    UpdateLabelsAppearance(itemsLazy.Value);
                    UpdateSelectedItemViewsAppearance(itemsLazy.Value);
                    break;
                case nameof(TextColor):
                case nameof(FontAttributes):
                case nameof(FontFamily):
                case nameof(FontSize):
                    UpdateLabelsAppearance(itemsLazy.Value);
                    break;
                case nameof(DividerVerticalPadding):
                    UpdateDividerViewsAppearance(itemsLazy.Value);
                    break;

            }
        }

        protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= OnItemsSourceCollectionChanged;
            }

            UpdateViews();

            if (newValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += OnItemsSourceCollectionChanged;
            }
        }

        protected virtual void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            UpdateViews();
        }

        protected virtual void UpdateViews()
        {
            _items.Clear();
            _items.AddRange(ItemsSource?.Cast<object>().ToList() ?? new List<object>());

            var items = _items.ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var label in _labels)
                {
                    label.GestureRecognizers.OfType<TapGestureRecognizer>().First().Tapped -= OnItemTapped;
                    label.GestureRecognizers.Clear();
                }

                _dividerViews.Clear();
                _dividersContainer.Children.Clear();
                _labels.Clear();
                _labelsContainer.Children.Clear();

                if (Parent is null)
                {
                    return;
                }

                UpdateContainerViewsAppearance();

                for (var i = 1; i < items.Count; ++i)
                {
                    var dividerView = new BoxView { Color = StrokeColor };
                    _dividerViews.Add(dividerView);
                    _dividersContainer.Children.Add(dividerView);
                }

                UpdateDividerViewsAppearance(items);

                for (var i = 0; i < items.Count; ++i)
                {
                    var label = CreateTappableLabel(items[i]?.ToString());

                    _labels.Add(label);
                    _labelsContainer.Children.Add(label);
                }

                UpdateLabelsAppearance(items);
                UpdateSelectedItemViewsAppearance(items);
            });
        }

        private Label CreateTappableLabel(string text)
        {
            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += OnItemTapped;

            var label = new Label
            {
                Text = text,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                GestureRecognizers = { recognizer },
            };

            return label;
        }

        private void UpdateContainerViewsAppearance()
        {
            if (Parent is null)
            {
                return;
            }

            var width = Width;
            var height = Height;
            var cornerRadius = CornerRadius;
            var strokeThickness = StrokeThickness;
            var strokeColor = StrokeColor;
            var fillColor = FillColor;

            _containerOuterView.CornerRadius = cornerRadius + strokeThickness / 2;
            _containerOuterView.Color = strokeColor;

            _containerInnerView.CornerRadius = cornerRadius - strokeThickness / 2;
            _containerInnerView.Color = fillColor;
            _containerInnerView.Margin = strokeThickness;

            var bounds = new Rectangle(0, 0, width, height);

            SetLayoutBounds(_containerOuterView, bounds);
            SetLayoutBounds(_containerInnerView, bounds);
        }

        private void UpdateDividerViewsAppearance(List<object> items)
        {
            if (Parent is null)
            {
                return;
            }

            var width = Width;
            var height = Height;
            var strokeThickness = StrokeThickness;
            var dividerVerticalPadding = DividerVerticalPadding;
            var strokeColor = StrokeColor;

            for (var i = 1; i < items.Count; ++i)
            {
                var dividerView = _dividerViews[i - 1];

                var step = (width - strokeThickness * (items.Count + 1)) / items.Count;
                var dividerX = i * (strokeThickness + step);
                var dividerY = strokeThickness + dividerVerticalPadding;
                var dividerWidth = strokeThickness;
                var dividerHeight = height - (strokeThickness + dividerVerticalPadding) * 2;
                var dividerBounds = new Rectangle(dividerX, dividerY, dividerWidth, dividerHeight);

                SetLayoutBounds(dividerView, dividerBounds);
                dividerView.Color = strokeColor;
            }
        }

        private void UpdateLabelsAppearance(List<object> items)
        {
            if (Parent is null)
            {
                return;
            }

            var selectedItem = SelectedItem;
            var width = Width;
            var height = Height;
            var strokeThickness = StrokeThickness;
            var selectedColor = SelectedColor;
            var selectedFontAttributes = SelectedFontAttributes;
            var selectedFontFamily = SelectedFontFamily;
            var selectedFontSize = SelectedFontSize;
            var textColor = TextColor;
            var fontAttributes = FontAttributes;
            var fontFamily = FontFamily;
            var fontSize = FontSize;

            var selectedItemIndex = items.IndexOf(selectedItem);
            var step = (width - strokeThickness * (items.Count + 1)) / items.Count;
            var labelY = 0.0;
            var labelWidth = step + strokeThickness;
            var labelHeight = height;

            for (var i = 0; i < items.Count; ++i)
            {
                var label = _labels[i];

                var labelX = i * (strokeThickness + step) + strokeThickness / 2;
                var labelBounds = new Rectangle(labelX, labelY, labelWidth, labelHeight);

                SetLayoutBounds(label, labelBounds);

                var isCurrentItemSelected = i == selectedItemIndex;

                label.Text = items[i]?.ToString();
                label.TextColor = isCurrentItemSelected ? selectedColor : textColor;
                label.FontAttributes = isCurrentItemSelected ? selectedFontAttributes : fontAttributes;
                label.FontFamily = isCurrentItemSelected ? selectedFontFamily : fontFamily;
                label.FontSize = isCurrentItemSelected ? selectedFontSize : fontSize;
                label.HorizontalTextAlignment = TextAlignment.Center;
                label.VerticalTextAlignment = TextAlignment.Center;
                label.Padding = new Thickness(strokeThickness / 2, strokeThickness);
            }
        }

        private void UpdateSelectedItemViewsAppearance(List<object> items)
        {
            if (Parent is null)
            {
                return;
            }

            var selectedItem = SelectedItem;
            var width = Width;
            var height = Height;
            var strokeThickness = StrokeThickness;
            var cornerRadius = CornerRadius;
            var fillColor = FillColor;
            var selectedColor = SelectedColor;
            var selectedFontAttributes = SelectedFontAttributes;
            var selectedFontFamily = SelectedFontFamily;
            var selectedFontSize = SelectedFontSize;

            var selectedItemIndex = items.IndexOf(selectedItem);
            var isItemSelected = selectedItemIndex != -1;
            _selectedItemInnerView.IsVisible = isItemSelected;
            _selectedItemOuterView.IsVisible = isItemSelected;

            if (!isItemSelected)
            {
                return;
            }

            var step = (width - strokeThickness * (items.Count + 1)) / items.Count;
            var containerX = selectedItemIndex * (strokeThickness + step);
            var containerY = 0.0;
            var containerWidth = step + strokeThickness * 2;
            var containerHeight = height;
            var containerBounds = new Rectangle(containerX, containerY, containerWidth, containerHeight);

            _selectedItemOuterView.CornerRadius = cornerRadius + strokeThickness / 2;
            _selectedItemOuterView.Color = selectedColor;

            _selectedItemInnerView.CornerRadius = cornerRadius - strokeThickness / 2;
            _selectedItemInnerView.Color = fillColor;
            _selectedItemInnerView.Margin = strokeThickness;

            SetLayoutBounds(_selectedItemOuterView, containerBounds);
            SetLayoutBounds(_selectedItemInnerView, containerBounds);

            var selectedLabel = _labels[selectedItemIndex];

            selectedLabel.TextColor = selectedColor;
            selectedLabel.FontAttributes = selectedFontAttributes;
            selectedLabel.FontFamily = selectedFontFamily;
            selectedLabel.FontSize = selectedFontSize;
        }
    }
}
