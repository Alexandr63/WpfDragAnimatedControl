using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfDragAnimatedPanel.Example.Models;
using WpfDragAnimatedPanel.Example.ViewModels;
using WpfDragAnimatedPanel.Tools;

namespace WpfDragAnimatedPanel.Example.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private readonly MainWindowViewModel _model = new MainWindowViewModel();

        #endregion

        #region Ctor

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _model;
        }

        #endregion

        #region Private Methods

        private void UpdateDragAnimatedPanelView(DragAnimatedPanel dragPanel, Size panelNewSize, bool? wheelUp)
        {
            if (!IsLoaded || !IsInitialized || TestListBox.ItemsSource == null)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine($">>> UpdateDragAnimatedPanelView panelNewSize:{panelNewSize}");

            const double MIN_SIZE = 50d;
            const double MAX_SIZE = 1000d;

            List<IDragItemSize> items = new List<IDragItemSize>();
            foreach (object item in TestListBox.ItemsSource)
            {
                items.Add((IDragItemSize)item);
            }

            if (_model.AutoSizeMode)
            {
                const int SCROLL_SIZE = 60;

                double multiplier;
                switch (dragPanel.FillType)
                {
                    case FillType.Row:
                        multiplier = (TestListBox.ActualHeight - SCROLL_SIZE) / items.Max(x => x.Height);
                        foreach (IDragItemSize item in items)
                        {
                            item.Height *= multiplier;
                            item.Width *= multiplier;
                        }
                        return;
                    case FillType.Column:
                        multiplier = (TestListBox.ActualWidth - SCROLL_SIZE) / items.Max(x => x.Width);
                        foreach (IDragItemSize item in items)
                        {
                            item.Height *= multiplier;
                            item.Width *= multiplier;
                        }
                        return;
                }
            }

            if (wheelUp.HasValue)
            {
                if ((wheelUp.Value && (items.Max(x => x.Width) > MAX_SIZE || items.Max(x => x.Height) > MAX_SIZE)) ||
                    (!wheelUp.Value && (items.Min(x => x.Width) < MIN_SIZE || items.Min(x => x.Height) < MIN_SIZE)))
                {
                    // Защита от слишком большого и маленького размеров элементов
                    return;
                }

                foreach (IDragItemSize item in items)
                {
                    if (wheelUp.Value)
                    {
                        const double MULTIPLIER_UP = 1.1d;
                        item.Height *= MULTIPLIER_UP;
                        item.Width *= MULTIPLIER_UP;
                    }
                    else
                    {
                        const double MULTIPLIER_DOWN = 0.9d;
                        item.Height *= MULTIPLIER_DOWN;
                        item.Width *= MULTIPLIER_DOWN;
                    }
                }
            }

            dragPanel.Measure(panelNewSize);

            // dragPanel.InvalidateMeasure();
        }

        //private void UpdateDragAnimatedPanelView(DragAnimatedPanel dragAnimatedPanel, Size panelNewSize, bool? wheelUp)
        //{
        //    if (!IsLoaded || !IsInitialized)
        //    {
        //        return;
        //    }

        //    const int SCROLL_SIZE = 25;
        //    const double MIN_SIZE = 50d;
        //    const double MAX_SIZE = 1000d;

        //    double oldHeight = dragAnimatedPanel.ItemHeight;
        //    double oldWidth = dragAnimatedPanel.ItemWidth;

        //    double height;
        //    double width;

        //    if (!_model.AutoSizeMode || dragAnimatedPanel.FillType == FillType.Wrap)
        //    {
        //        if (!wheelUp.HasValue ||
        //            (wheelUp.Value && (dragAnimatedPanel.ItemHeight > MAX_SIZE || dragAnimatedPanel.ItemWidth > MAX_SIZE)) ||
        //            (!wheelUp.Value && (dragAnimatedPanel.ItemHeight < MIN_SIZE || dragAnimatedPanel.ItemWidth < MIN_SIZE))
        //           )
        //        {
        //            // protection against too large/ small item size
        //            return;
        //        }

        //        if (wheelUp.Value)
        //        {
        //            const double MULTIPLIER_UP = 1.1;
        //            height = dragAnimatedPanel.ItemHeight * MULTIPLIER_UP;
        //            width = dragAnimatedPanel.ItemWidth * MULTIPLIER_UP;
        //        }
        //        else
        //        {
        //            const double MULTIPLIER_DOWN = 0.9;
        //            height = dragAnimatedPanel.ItemHeight * MULTIPLIER_DOWN;
        //            width = dragAnimatedPanel.ItemWidth * MULTIPLIER_DOWN;
        //        }

        //        dragAnimatedPanel.ItemWidth = width;
        //        dragAnimatedPanel.ItemHeight = height;
        //    }
        //    else if (_model.AutoSizeMode)
        //    {
        //        switch (dragAnimatedPanel.FillType)
        //        {
        //            case FillType.Row:
        //                height = panelNewSize.Height - SCROLL_SIZE; // consider the scroll size
        //                width = oldWidth * height / oldHeight;

        //                break;
        //            case FillType.Column:
        //                width = panelNewSize.Width - SCROLL_SIZE; // consider the scroll size
        //                height = oldHeight * width / oldWidth;

        //                break;
        //            default:
        //                return;
        //        }

        //        dragAnimatedPanel.ItemWidth = width;
        //        dragAnimatedPanel.ItemHeight = height;
        //    }
        //}

        #endregion

        #region Event Handlers
        
        private void TestListBoxPreviewMouseWheelEventHandler(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
            {
                return;
            }

            if (_model.FillType == FillType.Wrap || !_model.AutoSizeMode)
            {
                DragAnimatedPanel dragAnimatedPanel = ControlsHelper.GetDragAnimatedPanel((ListBox)sender);
                UpdateDragAnimatedPanelView(dragAnimatedPanel, dragAnimatedPanel.RenderSize, e.Delta > 0);

                e.Handled = true;
            }
        }

        private void TestListBoxSizeChangedEventHandler(object sender, SizeChangedEventArgs e)
        {
            DragAnimatedPanel dragAnimatedPanel = ControlsHelper.GetDragAnimatedPanel((ListBox)sender);
            UpdateDragAnimatedPanelView(dragAnimatedPanel, e.NewSize, null);
        }

        private void AutoSizeModeChangedEventHandler(object sender, RoutedEventArgs e)
        {
            if (_model.AutoSizeMode)
            {
                DragAnimatedPanel dragAnimatedPanel = ControlsHelper.GetDragAnimatedPanel(TestListBox);
                ScrollViewer scrollViewer = ControlsHelper.GetVisualChild<ScrollViewer>(TestListBox);
                UpdateDragAnimatedPanelView(dragAnimatedPanel, new Size(scrollViewer.ViewportWidth, scrollViewer.ViewportHeight), null);
            }
        }

        private void FillTypesChangedEventHandler(object sender, SelectionChangedEventArgs e)
        {
            DragAnimatedPanel dragAnimatedPanel = ControlsHelper.GetDragAnimatedPanel(TestListBox);
            ScrollViewer scrollViewer = ControlsHelper.GetVisualChild<ScrollViewer>(TestListBox);
            UpdateDragAnimatedPanelView(dragAnimatedPanel, new Size(scrollViewer.ViewportWidth, scrollViewer.ViewportHeight), null);
        }
        
        #endregion

        private void TestButtonClickEventHandler(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($">>> Test list:");
            foreach (ImageModel imageModel in _model.Images)
            {
                System.Diagnostics.Debug.WriteLine($"\t{imageModel.Tag}");
            }
        }
    }
}