using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfDragAnimatedPanel.Commands;
using WpfDragAnimatedPanel.LayoutStrategies;
using WpfDragAnimatedPanel.Tools;

namespace WpfDragAnimatedPanel
{
    public sealed partial class DragAnimatedPanel : Panel
    {
        #region Private Fields

        private Size _calculatedSize;
        private bool _isNotFirstArrange = false;

        private int _columns;
        private int _rows;

        #endregion

        #region Ctor

        static DragAnimatedPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragAnimatedPanel), new FrameworkPropertyMetadata(typeof(DragAnimatedPanel)));
        }

        public DragAnimatedPanel() : base()
        {
            SwapCommand = GetDefaultSwapCommand();

            AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove), false);
            AddHandler(MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
            MouseLeftButtonUp += OnMouseUp;
            LostMouseCapture += OnLostMouseCapture;
        }

        #endregion

        #region Properties

        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(DragAnimatedPanel), new UIPropertyMetadata(248d, MeasureControlByDependencyPropertyChanged));

        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(nameof(ItemHeight), typeof(double), typeof(DragAnimatedPanel), new UIPropertyMetadata(350d, MeasureControlByDependencyPropertyChanged));

        public FillType FillType
        {
            get => (FillType)GetValue(FillTypeProperty);
            set => SetValue(FillTypeProperty, value);
        }

        public static readonly DependencyProperty FillTypeProperty = DependencyProperty.Register(nameof(FillType), typeof(FillType), typeof(DragAnimatedPanel), new UIPropertyMetadata(FillType.Wrap, MeasureControlByDependencyPropertyChanged));

        public int AnimationMilliseconds
        {
            get => (int)GetValue(AnimationMillisecondsProperty);
            set => SetValue(AnimationMillisecondsProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AnimationMilliseconds.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AnimationMillisecondsProperty = DependencyProperty.Register(nameof(AnimationMilliseconds), typeof(int), typeof(DragAnimatedPanel), new UIPropertyMetadata(75, MeasureControlByDependencyPropertyChanged));

        public ICommand SwapCommand
        {
            get => (ICommand)GetValue(SwapCommandProperty);
            set => SetValue(SwapCommandProperty, value);
        }

        public static readonly DependencyProperty SwapCommandProperty = DependencyProperty.Register(nameof(SwapCommand), typeof(ICommand), typeof(DragAnimatedPanel), new UIPropertyMetadata(null));
        
        #endregion

        #region Override Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            Size itemContainerSize = new Size(ItemWidth, ItemHeight);

            foreach (UIElement child in Children)
            {
                child.Measure(itemContainerSize);
            }

            int count = Children.Count;
            if (count == 0)
            {
                _calculatedSize = new Size();
            }
            else if (FillType == FillType.Row)
            {
                _calculatedSize = new Size(count * ItemWidth, ItemHeight);
            }
            else if (FillType == FillType.Column)
            {
                _calculatedSize = new Size(ItemWidth, count * ItemHeight);
            }
            else
            {
                if (availableSize.Width < ItemWidth)
                {
                    _calculatedSize = new Size(ItemWidth, count * ItemHeight);
                }
                else
                {
                    _columns = (int)Math.Truncate(availableSize.Width / ItemWidth);
                    _rows = count / _columns;
                    if (count % _columns != 0)
                    {
                        _rows++;
                    }

                    _calculatedSize = new Size(_columns * ItemWidth, _rows * ItemHeight);
                }
            }

            return _calculatedSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size finalItemSize = new Size(ItemWidth, ItemHeight);
            // if is animated then arrange elements to 0,0, and then put them on its location using the transform
            foreach (UIElement child in InternalChildren)
            {
                // If this is the first time we've seen this child, add our transforms
                if (!(child.RenderTransform is TransformGroup))
                {
                    child.RenderTransformOrigin = new Point(0.5, 0.5);
                    TransformGroup group = new TransformGroup();
                    child.RenderTransform = group;
                    group.Children.Add(new TranslateTransform());
                }

                //locate all children in 0,0 point //TODO: use infinity and then scale each element to items size
                child.Arrange(new Rect(new Point(0, 0), finalItemSize)); //when use transformations change to childs.DesireSize
            }

            AnimateAll();

            if (!_isNotFirstArrange)
            {
                _isNotFirstArrange = true;
            }

            return _calculatedSize;
        }

        #endregion

        #region Private Methods

        private ICommand GetDefaultSwapCommand()
        {
            return new DelegateCommand<int[]>(
                (indexes) =>
                {
                    int from = indexes[0];
                    int to = indexes[1];

                    ItemsControl parentItemsControl = ControlsHelper.GetParent(this, (x) => x is ItemsControl) as ItemsControl;
                    
                    // NOTE: если в ItemsSource не IList - необходимо написать свою реализацию команды 
                    IList list = (IList)parentItemsControl.ItemsSource;

                    if (from < 0 || to < 0 || from >= list.Count || to >= list.Count)
                    {
                        return;
                    }

                    object dragged = list[from];
                    list.Remove(dragged);
                    list.Insert(to, dragged);
                },
                (indexes => indexes.Length > 1)
            );
        }

        private UIElement GetChildThatHasMouseOver()
        {
            return ControlsHelper.GetParent(Mouse.DirectlyOver as DependencyObject, (ve) => Children.Contains(ve as UIElement)) as UIElement;
        }

        private Point GetItemVisualPoint(UIElement element)
        {
            TransformGroup group = (TransformGroup)element.RenderTransform;
            TranslateTransform trans = (TranslateTransform)group.Children[0];

            return new Point(trans.X, trans.Y);
        }

        private int GetIndexFromPoint(double x, double y)
        {
            int columnIndex = (int)Math.Truncate(x / ItemWidth);
            int rowIndex = (int)Math.Truncate(y / ItemHeight);

            int columns;
            if (FillType == FillType.Row)
            {
                columns = Children.Count;
            }
            else if (FillType == FillType.Column)
            {
                columns = 1;
            }
            else
            {
                columns = _columns;
            }

            return columns * rowIndex + columnIndex;
        }

        private void AnimateAll()
        {
            //Apply exactly the same algorithm, but inside of Arrange a call AnimateTo method
            double colPosition = 0;
            double rowPosition = 0;
            foreach (UIElement child in Children)
            {
                if (child != DraggedElement)
                {
                    AnimateTo(child, colPosition, rowPosition, _isNotFirstArrange ? AnimationMilliseconds : 0);
                }

                //drag will locate dragged element
                colPosition += ItemWidth;
                if (colPosition + 1 > _calculatedSize.Width)
                {
                    colPosition = 0;
                    rowPosition += ItemHeight;
                }
            }
        }

        private void AnimateTo(UIElement child, double x, double y, int duration)
        {
            TransformGroup group = (TransformGroup)child.RenderTransform;
            TranslateTransform trans = (TranslateTransform)group.Children.First((groupElement) => groupElement is TranslateTransform);

            trans.BeginAnimation(TranslateTransform.XProperty, MakeAnimation(x, duration));
            trans.BeginAnimation(TranslateTransform.YProperty, MakeAnimation(y, duration));
        }

        private DoubleAnimation MakeAnimation(double to, int duration)
        {
            return new DoubleAnimation(to, TimeSpan.FromMilliseconds(duration))
            {
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.7
            };
        }

        private void MeasureControl()
        {
            if (Children.Count == 0)
            {
                return;
            }

            Size calculatedSize;
            if (FillType == FillType.Row)
            {
                calculatedSize = new Size(Children.Count * ItemWidth, ItemHeight);
            }
            else if (FillType == FillType.Column)
            {
                calculatedSize = new Size(ItemWidth, Children.Count * ItemHeight);
            }
            else
            {
                calculatedSize = new Size(_columns * ItemWidth, _columns * ItemHeight);
            }

            if (_calculatedSize != calculatedSize)
            {
                _calculatedSize = calculatedSize;
                Measure(_calculatedSize);
            }
        }

        private static void MeasureControlByDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DragAnimatedPanel)d).MeasureControl();
        }

        #endregion
    }
}
