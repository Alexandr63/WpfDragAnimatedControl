using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;
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

        private ILayoutStrategy _layoutStrategy;

        #endregion

        #region Ctor

        static DragAnimatedPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragAnimatedPanel), new FrameworkPropertyMetadata(typeof(DragAnimatedPanel)));
        }

        public DragAnimatedPanel() : base()
        {
            SwapCommand = GetDefaultSwapCommand();
            UpdateLayoutStrategy();

            AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove), false);
            AddHandler(MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
            MouseLeftButtonUp += OnMouseUp;
            LostMouseCapture += OnLostMouseCapture;
        }

        #endregion

        #region Properties

        public FillType FillType
        {
            get => (FillType)GetValue(FillTypeProperty);
            set => SetValue(FillTypeProperty, value);
        }

        public static readonly DependencyProperty FillTypeProperty = DependencyProperty.Register(nameof(FillType), typeof(FillType), typeof(DragAnimatedPanel), new UIPropertyMetadata(FillType.Wrap, FillTypePropertyDependencyPropertyChanged));
        
        public int AnimationMilliseconds
        {
            get => (int)GetValue(AnimationMillisecondsProperty);
            set => SetValue(AnimationMillisecondsProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AnimationMilliseconds.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AnimationMillisecondsProperty = DependencyProperty.Register(nameof(AnimationMilliseconds), typeof(int), typeof(DragAnimatedPanel), new UIPropertyMetadata(75, AnimationMillisecondsDependencyPropertyChanged));

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
            foreach (UIElement child in Children)
            {
                child.Measure(GetDragItemSize(child).GetSize());
            }

            if (Children.Count == 0)
            {
                _calculatedSize = new Size();
            }
            else
            {
                List<Size> childSizes = new List<Size>(Children.Count);
                foreach (UIElement child in Children)
                {
                    childSizes.Add(child.DesiredSize);
                }
                
                _layoutStrategy.MeasureLayout(availableSize, childSizes, DraggedElement != null);

                _calculatedSize = _layoutStrategy.ResultSize;
            }

            return _calculatedSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
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
                child.Arrange(new Rect(new Point(0, 0), GetDragItemSize(child).GetSize())); //when use transformations change to childs.DesireSize
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

        private void UpdateLayoutStrategy()
        {
            switch (FillType)
            {
                case FillType.Column:
                    _layoutStrategy = new ColumnLayoutStrategy();
                    break;
                case FillType.Row:
                    _layoutStrategy = new RowLayoutStrategy();
                    break;
                case FillType.Wrap:
                    _layoutStrategy = new WrapLayoutStrategy();
                    break;
                default:
                    throw new ArgumentException($"Unknown FillType {FillType}");
                    _layoutStrategy = new TableLayoutStrategy(); // TODO сделать вариант с Table
            }

            InvalidateMeasure();
        }

        private IDragItemSize GetDragItemSize(UIElement element)
        {
            return (IDragItemSize)((FrameworkElement)element).DataContext;
        }
        
        private ICommand GetDefaultSwapCommand()
        {
            return new DelegateCommand<int[]>(
                indexes =>
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
                indexes => indexes.Length > 1
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

        private void AnimateAll()
        {
            if (Children.Count == 0)
            {
                return;
            }

            double horizontalPosition = 0;
            double verticalPosition = 0;

            System.Diagnostics.Debug.WriteLine($">>> AnimateAll() _calculatedSize:{_calculatedSize}");
            int rowIndex = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                UIElement child = Children[i];
                //drag will locate dragged element
                if (child != DraggedElement)
                {
                    AnimateTo(child, horizontalPosition, verticalPosition, _isNotFirstArrange ? AnimationMilliseconds : 0);
                    // System.Diagnostics.Debug.WriteLine($">>> AnimateAll() index:{i} {((FrameworkElement)child).DataContext.ToString()} to position {horizontalPosition};{verticalPosition}");
                }

                DragItemLayoutInfo currentLayoutInfo = _layoutStrategy.GetLayoutInfo(i);
                horizontalPosition += currentLayoutInfo.ColumnWidth;

                if (i + 1 < Children.Count)
                {
                    DragItemLayoutInfo nextLayoutInfo = _layoutStrategy.GetLayoutInfo(i + 1);
                    if (nextLayoutInfo.RowIndex > rowIndex)
                    {
                        rowIndex++;
                        horizontalPosition = 0;
                        verticalPosition += currentLayoutInfo.RowHeight;
                    }
                }
            }

            // Старая версия с фиксированным размером 
            //int elementIndex = 0;
            //foreach (UIElement child in Children)
            //{


            //    elementIndex++;
            //    if (elementIndex < Children.Count)
            //    {
            //        colPosition += _layoutStrategy.GetColumnWidthByElementIndex(elementIndex);
            //        if (colPosition + 1 > _calculatedSize.Width)
            //        {
            //            colPosition = 0;
            //            rowPosition += _layoutStrategy.GetRowHeightByElementIndex(elementIndex);
            //        }
            //    }
            //}
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
        
        private static void AnimationMillisecondsDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DragAnimatedPanel)d).InvalidateMeasure();
        }

        private static void FillTypePropertyDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DragAnimatedPanel)d).UpdateLayoutStrategy();
        }

        #endregion
    }
}
