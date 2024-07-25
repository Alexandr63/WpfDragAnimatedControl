using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void UpdateDragAnimatedPanelView(DragAnimatedPanel dragAnimatedPanel, Size panelNewSize, bool? wheelUp)
        {
            if (!IsLoaded || !IsInitialized)
            {
                return;
            }

            const int SCROLL_SIZE = 25;
            const double MIN_SIZE = 50d;
            const double MAX_SIZE = 1000d;

            double oldHeight = dragAnimatedPanel.ItemHeight;
            double oldWidth = dragAnimatedPanel.ItemWidth;

            double height;
            double width;

            if (!_model.AutoSizeMode || dragAnimatedPanel.FillType == FillType.Wrap)
            {
                if (!wheelUp.HasValue ||
                    (wheelUp.Value && (dragAnimatedPanel.ItemHeight > MAX_SIZE || dragAnimatedPanel.ItemWidth > MAX_SIZE)) ||
                    (!wheelUp.Value && (dragAnimatedPanel.ItemHeight < MIN_SIZE || dragAnimatedPanel.ItemWidth < MIN_SIZE))
                   )
                {
                    // protection against too large/ small item size
                    return;
                }

                if (wheelUp.Value)
                {
                    const double MULTIPLIER_UP = 1.1;
                    height = dragAnimatedPanel.ItemHeight * MULTIPLIER_UP;
                    width = dragAnimatedPanel.ItemWidth * MULTIPLIER_UP;
                }
                else
                {
                    const double MULTIPLIER_DOWN = 0.9;
                    height = dragAnimatedPanel.ItemHeight * MULTIPLIER_DOWN;
                    width = dragAnimatedPanel.ItemWidth * MULTIPLIER_DOWN;
                }

                dragAnimatedPanel.ItemWidth = width;
                dragAnimatedPanel.ItemHeight = height;
            }
            else if (_model.AutoSizeMode)
            {
                switch (dragAnimatedPanel.FillType)
                {
                    case FillType.Horizontal:
                        height = panelNewSize.Height - SCROLL_SIZE; // consider the scroll size
                        width = oldWidth * height / oldHeight;

                        break;
                    case FillType.Vertical:
                        width = panelNewSize.Width - SCROLL_SIZE; // consider the scroll size
                        height = oldHeight * width / oldWidth;

                        break;
                    default:
                        return;
                }

                dragAnimatedPanel.ItemWidth = width;
                dragAnimatedPanel.ItemHeight = height;
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

            ScrollViewer scrollViewer = ControlsHelper.GetVisualChild<ScrollViewer>((ListBox)sender);
            double w = scrollViewer.ViewportWidth;
            double h = scrollViewer.ViewportHeight;

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
    }
}