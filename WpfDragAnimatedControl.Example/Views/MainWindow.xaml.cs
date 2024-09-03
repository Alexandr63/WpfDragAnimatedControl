using System.Windows;
using WpfDragAnimatedControl.Example.Models;
using WpfDragAnimatedControl.Example.ViewModels;

namespace WpfDragAnimatedControl.Example.Views
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

        #region Event Handlers

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

        #endregion
    }
}