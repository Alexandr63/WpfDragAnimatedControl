using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfDragAnimatedControl.Tools;

namespace WpfDragAnimatedControl
{
    /// <summary>
    /// Interaction logic for DragAnimatedListBox.xaml
    /// </summary>
    public partial class DragAnimatedListBox : UserControl
    {
        #region Ctor

        public DragAnimatedListBox()
        {
            InitializeComponent();

            Loaded += LoadedEventHandler;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Тип заполнения контрола.
        /// </summary>
        public FillType FillType
        {
            get => (FillType)GetValue(FillTypeProperty);
            set => SetValue(FillTypeProperty, value);
        }

        public static readonly DependencyProperty FillTypeProperty = DependencyProperty.Register(nameof(FillType), typeof(FillType), typeof(DragAnimatedListBox), new UIPropertyMetadata(FillType.Wrap, FillTypePropertyChangedCallback));
        
        /// <summary>
        /// Время анимации при переносе элементов в миллисекундах.
        /// </summary>
        public int AnimationMilliseconds
        {
            get => (int)GetValue(AnimationMillisecondsProperty);
            set => SetValue(AnimationMillisecondsProperty, value);
        }

        public static readonly DependencyProperty AnimationMillisecondsProperty = DependencyProperty.Register(nameof(AnimationMilliseconds), typeof(int), typeof(DragAnimatedListBox), new UIPropertyMetadata(75));

        /// <summary>
        /// Ражим заполнения дочерними элементами всего доступного места. Используется только в <see cref="LayoutStrategies.RowLayoutStrategy"/> и <see cref="LayoutStrategies.ColumnLayoutStrategy"/> 
        /// </summary>
        public bool AutoSizeMode
        {
            get => (bool)GetValue(AutoSizeModeProperty);
            set => SetValue(AutoSizeModeProperty, value);
        }

        public static readonly DependencyProperty AutoSizeModeProperty = DependencyProperty.Register(nameof(AutoSizeMode), typeof(bool), typeof(DragAnimatedListBox), new UIPropertyMetadata(false, AutoSizeModePropertyChangedCallback));

        /// <summary>
        /// Множитель размера элементов, используемый в данный момент для отображения 
        /// </summary>
        public double ItemSizeMultiplier
        {
            get => (double)GetValue(ItemSizeMultiplierProperty);
            set => SetValue(ItemSizeMultiplierProperty, value);
        }

        public static readonly DependencyProperty ItemSizeMultiplierProperty = DependencyProperty.Register(nameof(ItemSizeMultiplier), typeof(double), typeof(DragAnimatedListBox), new UIPropertyMetadata(1d));
        
        /// <summary>
        /// Лист с элементами для отображения. Так как у нас выполняется изменение порядка отображения объектов нужен <see cref="IList"/>.
        /// </summary>
        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set
            {
                if (value == null)
                {
                    ClearValue(ItemsSourceProperty);
                }
                else
                {
                    SetValue(ItemsSourceProperty, value);
                }
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IList), typeof(DragAnimatedListBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Шаблон для отображаемого элемента.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }
        
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(DragAnimatedListBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Минимальный размер отображаемого элемента.
        /// </summary>
        public double MinItemSize
        {
            get => (double)GetValue(MinItemSizeProperty);
            set => SetValue(MinItemSizeProperty, value);
        }

        public static readonly DependencyProperty MinItemSizeProperty = DependencyProperty.Register(nameof(MinItemSize), typeof(double), typeof(DragAnimatedListBox), new UIPropertyMetadata(100d));

        /// <summary>
        /// Максимальный размер отображаемого элемента.
        /// </summary>
        public double MaxItemSize
        {
            get => (double)GetValue(MaxItemSizeProperty);
            set => SetValue(MaxItemSizeProperty, value);
        }

        public static readonly DependencyProperty MaxItemSizeProperty = DependencyProperty.Register(nameof(MaxItemSize), typeof(double), typeof(DragAnimatedListBox), new UIPropertyMetadata(1500d));

        #endregion

        #region Public Methods

        /// <summary>
        /// Прокрутить скролл до заданного объекта.
        /// </summary>
        public void ScrollIntoView(object item)
        {
            InnerListBox.ScrollIntoView(item);
        }

        #endregion

        #region Private Methods

        private void Resize(List<IDragItemSize> items, double itemSizeMultiplier)
        {
            ItemSizeMultiplier *= itemSizeMultiplier;
            foreach (IDragItemSize item in items)
            {
                item.Height *= itemSizeMultiplier;
                item.Width *= itemSizeMultiplier;
            }
        }

        private void UpdateDragAnimatedPanelView(bool isAutoSizeModeChanged, bool? wheelUp)
        {
            if (!IsLoaded || !IsInitialized || InnerListBox.ItemsSource == null)
            {
                return;
            }

            DragAnimatedPanel dragPanel = ControlsHelper.GetDragAnimatedPanel(InnerListBox);

            List<IDragItemSize> items = new List<IDragItemSize>();
            foreach (object item in InnerListBox.ItemsSource)
            {
                items.Add((IDragItemSize)item);
            }

            double multiplier;

            // Перешли в режим автозаполнения, он актуален только для заполнения в столбец или в строку
            if (AutoSizeMode)
            {
                const int VERTICAL_SCROLL_OFFSET = 5;

                switch (FillType)
                {
                    case FillType.Row:
                        double height = InnerListBox.ActualHeight - SystemParameters.VerticalScrollBarWidth - VERTICAL_SCROLL_OFFSET;
                        multiplier = height / items.Max(x => x.Height);
                        Resize(items, multiplier);
                        dragPanel.InvalidateMeasure();
                        return;
                    case FillType.Column:
                        double width = InnerListBox.ActualWidth - SystemParameters.VerticalScrollBarWidth - VERTICAL_SCROLL_OFFSET;
                        multiplier = width / items.Max(x => x.Width);
                        Resize(items, multiplier);
                        dragPanel.InvalidateMeasure();
                        return;
                }
            }
            
            // Выполняется масштабирования с помощью колесика мыши
            if (wheelUp.HasValue)
            {
                if ((wheelUp.Value && IsTooLargeItems(items, out _)) ||
                    (!wheelUp.Value && IsTooSmallItems(items, out _)))
                {
                    // Защита от слишком большого и маленького размеров элементов
                    return;
                }

                if (wheelUp.Value)
                {
                    const double MULTIPLIER_UP = 1.1d;
                    Resize(items, MULTIPLIER_UP);

                }
                else
                {
                    const double MULTIPLIER_DOWN = 0.9d;
                    Resize(items, MULTIPLIER_DOWN);
                }

                dragPanel.InvalidateMeasure();
                return;
            }

            // Вышли из режима автозаполнения
            if (isAutoSizeModeChanged)
            {
                // Надо уменьшить размер элементов
                if (IsTooLargeItems(items, out double maxSize))
                {
                    multiplier = MaxItemSize / maxSize;
                    Resize(items, multiplier);
                    dragPanel.InvalidateMeasure();
                    return;
                }

                // Надо увеличить размер элементов
                if (IsTooSmallItems(items, out double minSize))
                {
                    multiplier = MinItemSize / minSize;
                    Resize(items, multiplier);
                    dragPanel.InvalidateMeasure();
                    return;
                }
            }
        }

        private bool IsTooSmallItems(List<IDragItemSize> items, out double minSize)
        {
            double minWidth = items.Min(x => x.Width);
            double minHeight = items.Min(x => x.Height);
            
            bool isTooSmall = minWidth < MinItemSize || minHeight < MinItemSize;

            if (isTooSmall)
            {
                minSize = minWidth < minHeight ? minWidth : minHeight;
            }
            else
            {
                minSize = -1;
            }

            return isTooSmall;
        }

        private bool IsTooLargeItems(List<IDragItemSize> items, out double maxSize)
        {
            double maxWidth = items.Max(x => x.Width);
            double maxHeight = items.Max(x => x.Height);
            
            bool isTooLarge = maxWidth > MaxItemSize || maxHeight > MaxItemSize;

            if (isTooLarge)
            {
                maxSize = maxWidth > maxHeight ? maxWidth : maxHeight;
            }
            else
            {
                maxSize = -1;
            }
            
            return isTooLarge;
        }

        private void LoadedEventHandler(object sender, RoutedEventArgs e)
        {
            UpdateDragAnimatedPanelView(false, null);
        }

        private void InnerListBoxPreviewMouseWheelEventHandler(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
            {
                return;
            }

            UpdateDragAnimatedPanelView(false, e.Delta > 0);

            e.Handled = true;
        }

        private void InnerListBoxSizeChangedEventHandler(object sender, SizeChangedEventArgs e)
        {
            UpdateDragAnimatedPanelView(false, null);
        }
        
        private static void AutoSizeModePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DragAnimatedListBox control = (DragAnimatedListBox)d;

            if (control.FillType == FillType.Column || control.FillType == FillType.Row)
            {
                control.UpdateDragAnimatedPanelView(true, null);
            }
        }

        private static void FillTypePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DragAnimatedListBox control = (DragAnimatedListBox) d;
            control.UpdateDragAnimatedPanelView(false, null);
        }

        #endregion
    }
}
