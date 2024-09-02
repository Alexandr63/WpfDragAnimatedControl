using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDragAnimatedControl.Tools
{
    public static class ControlsHelper
    {
        public static DependencyObject GetParent(DependencyObject obj, Func<DependencyObject, bool> matchFunction)
        {
            DependencyObject parent = obj;

            do
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            while (parent != null && !matchFunction.Invoke(parent));

            return parent;
        }

        public static DragAnimatedPanel GetDragAnimatedPanel(DependencyObject itemsControl)
        {
            ItemsPresenter itemsPresenter = GetVisualChild<ItemsPresenter>(itemsControl);
            DragAnimatedPanel itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0) as DragAnimatedPanel;

            return itemsPanel;
        }

        public static T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
