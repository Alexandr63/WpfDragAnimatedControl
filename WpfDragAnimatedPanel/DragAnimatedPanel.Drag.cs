using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfDragAnimatedPanel.Tools;

namespace WpfDragAnimatedPanel
{
    public partial class DragAnimatedPanel
    {
        #region Constants

        private DateTime _mouseDownTime;

        #endregion

        #region Private Fields

        private int _draggedIndex;
        private bool _firstScrollRequest = true;
        private ScrollViewer _scrollContainer;
        private double _lastMousePosX;
        private double _lastMousePosY;
        private UIElement _lastMousePositionElement = null;
        private int _lastMouseMoveTime;
        private double _x;
        private double _y;

        #endregion

        #region Properties

        public UIElement DraggedElement { get; set; }

        private ScrollViewer ScrollViewer
        {
            get
            {
                if (_firstScrollRequest && _scrollContainer == null)
                {
                    _firstScrollRequest = false;
                    _scrollContainer = (ScrollViewer)ControlsHelper.GetParent(this, (ve) => ve is ScrollViewer);
                }
                return _scrollContainer;
            }
        }

        #endregion

        #region Private Methods

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && DraggedElement == null && !IsMouseCaptured)
            {
                const double MIN_CLICK_TIME_MILLISECONDS = 40d;
                const int MIN_SHIFT_VALUE = 10;

                TimeSpan clickDuration = DateTime.Now - _mouseDownTime;
                Point mousePos = Mouse.GetPosition(this);
                double difX = Math.Abs(mousePos.X - _lastMousePosX);
                double difY = Math.Abs(mousePos.Y - _lastMousePosY);
                // Защита от случайного перехода в режим перетаскивания при быстром прокликивании элементов. Смотрит, как долго была нажата кнопка мыши и насколько переместилась мышь.
                if (clickDuration.Milliseconds > MIN_CLICK_TIME_MILLISECONDS && (difX > MIN_SHIFT_VALUE || difY > MIN_SHIFT_VALUE))
                {
                    StartDrag(e);
                }
            }
            else if (DraggedElement != null)
            {
                OnDragOver(e);
            }
        }

        private void OnDragOver(MouseEventArgs e)
        {
            const int MOUSE_TIME_DIF = 25;
            const double MOUSE_DIF = 10d;

            Point mousePos = Mouse.GetPosition(this);
            double difX = mousePos.X - _lastMousePosX;
            double difY = mousePos.Y - _lastMousePosY;
            int timeDif = e.Timestamp - _lastMouseMoveTime;
            if ((Math.Abs(difX) > MOUSE_DIF || Math.Abs(difY) > MOUSE_DIF) && timeDif > MOUSE_TIME_DIF)
            {
                DoScroll();
                
                int index = _layoutStrategy.GetIndex(mousePos);
                _x += difX;
                _y += difY;

                _lastMousePosX = mousePos.X;
                _lastMousePosY = mousePos.Y;
                _lastMouseMoveTime = e.Timestamp;
                SwapElement(index);
                AnimateTo(DraggedElement, _x, _y, 0);
            }
        }

        private void StartDrag(MouseEventArgs e)
        {
            DraggedElement = _lastMousePositionElement;
            _lastMousePositionElement = null;
            if (DraggedElement == null)
            {
                return;
            }

            _draggedIndex = Children.IndexOf(DraggedElement);
            Point p = GetItemVisualPoint(DraggedElement);
            _x = p.X;
            _y = p.Y;

            SetZIndex(DraggedElement, 1000);

            _lastMouseMoveTime = e.Timestamp;

            InvalidateArrange();

            e.Handled = true;

            CaptureMouse();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Point mousePos = Mouse.GetPosition(this);
            _lastMousePosX = mousePos.X;
            _lastMousePosY = mousePos.Y;
            _mouseDownTime = DateTime.Now;
            _lastMousePositionElement = GetChildThatHasMouseOver();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
        }

        private void SwapElement(int index)
        {
            if (index == _draggedIndex || index < 0)
            {
                return;
            }
            
            if (index >= Children.Count)
            {
                index = Children.Count - 1;
            }

            int[] parameter = new int[] { _draggedIndex, index };
            if (SwapCommand != null && SwapCommand.CanExecute(parameter))
            {
                SwapCommand.Execute(parameter);
                DraggedElement = Children[index]; // this is because after changing the collection the element is other			
                FillNewDraggedChild(DraggedElement);
                _draggedIndex = index;
            }

            InvalidateArrange();
        }

        private void FillNewDraggedChild(UIElement child)
        {
            if (!(child.RenderTransform is TransformGroup))
            {
                child.RenderTransformOrigin = new Point(0.5, 0.5);
                TransformGroup group = new TransformGroup();
                child.RenderTransform = group;
                group.Children.Add(new TranslateTransform());
            }
            
            SetZIndex(child, 1000);
            AnimateTo(child, _x, _y, 0);
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            FinishDrag();
        }

        private void FinishDrag()
        {
            if (DraggedElement != null)
            {
                DraggedElement = null;

                System.Diagnostics.Debug.WriteLine(">>> Finish Drag");
                InvalidateMeasure();
            }
        }

        private void DoScroll()
        {
            if (ScrollViewer != null)
            {
                Point position = Mouse.GetPosition(ScrollViewer);
                double scrollMargin = Math.Min(ScrollViewer.FontSize * 2, ScrollViewer.ActualHeight / 2);

                if (position.X >= ScrollViewer.ActualWidth - scrollMargin && ScrollViewer.HorizontalOffset < ScrollViewer.ExtentWidth - ScrollViewer.ViewportWidth)
                {
                    ScrollViewer.LineRight();
                }
                else if (position.X < scrollMargin && ScrollViewer.HorizontalOffset > 0)
                {
                    ScrollViewer.LineLeft();
                }
                else if (position.Y >= ScrollViewer.ActualHeight - scrollMargin && ScrollViewer.VerticalOffset < ScrollViewer.ExtentHeight - ScrollViewer.ViewportHeight)
                {
                    ScrollViewer.LineDown();
                }
                else if (position.Y < scrollMargin && ScrollViewer.VerticalOffset > 0)
                {
                    ScrollViewer.LineUp();
                }
            }
        }

        #endregion
    }
}
