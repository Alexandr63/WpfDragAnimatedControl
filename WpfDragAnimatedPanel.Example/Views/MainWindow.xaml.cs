using System.Collections.Generic;
using System.Linq;
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

        private void Resize(double multiplier)
        {
            _model.Multiplier *= multiplier;
            foreach (ImageModel imageModel in _model.Images)
            {
                imageModel.Height *= multiplier;
                imageModel.Width *= multiplier;
            }
        }

        private void UpdateDragAnimatedPanelView(DragAnimatedPanel dragPanel, Size panelNewSize, bool? wheelUp)
        {
            if (!IsLoaded || !IsInitialized || TestListBox.ItemsSource == null)
            {
                return;
            }

            const double MIN_SIZE = 100d;
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
                        Resize(multiplier);
                        dragPanel.Measure(panelNewSize);
                        return;
                    case FillType.Column:
                        multiplier = (TestListBox.ActualWidth - SCROLL_SIZE) / items.Max(x => x.Width);
                        Resize(multiplier);
                        dragPanel.Measure(panelNewSize);
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

                if (wheelUp.Value)
                {
                    const double MULTIPLIER_UP = 1.1d;
                    Resize(MULTIPLIER_UP);
                }
                else
                {
                    const double MULTIPLIER_DOWN = 0.9d;
                    Resize(MULTIPLIER_DOWN);
                }

                dragPanel.Measure(panelNewSize);
            }
        }

        #endregion

        #region Event Handlers
        
        private void TestListBoxPreviewMouseWheelEventHandler(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
            {
                return;
            }

            DragAnimatedPanel dragAnimatedPanel = ControlsHelper.GetDragAnimatedPanel((ListBox)sender);
            UpdateDragAnimatedPanelView(dragAnimatedPanel, dragAnimatedPanel.RenderSize, e.Delta > 0);

            e.Handled = true;
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

        private void DataToOutputButtonClickEventHandler(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($">>> Test list:");
            foreach (ImageModel imageModel in _model.Images)
            {
                System.Diagnostics.Debug.WriteLine($"\t{imageModel}");
            }
        }

        private void AddItemsButtonClickEventHandler(object sender, RoutedEventArgs e)
        {
            _model.AddImages();
        }
    }
}