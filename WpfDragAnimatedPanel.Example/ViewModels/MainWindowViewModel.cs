using WpfDragAnimatedPanel.Example.Helpers;
using WpfDragAnimatedPanel.Example.Models;
using WpfDragAnimatedPanel.Tools;

namespace WpfDragAnimatedPanel.Example.ViewModels
{
    public class MainWindowViewModel : NotifyPropertyChangedObject
    {
        #region Private Fields

        private ExtendedObservableCollection<ImageModel> _imageSource;
        private FillType _fillType = FillType.Wrap;
        private bool _autoSizeMode = true;

        #endregion

        #region Ctor

        public MainWindowViewModel()
        {
            //Images = new ExtendedObservableCollection<ImageModel>()
            //{
            //    new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/0.jpg") },
            //    new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/1.jpg") },
            //    new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/2.jpg") },
            //    new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/3.jpg") },
            //    new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/4.jpg") },
            //    new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/5.jpg") },
            //    new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/6.jpg") },
            //    new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/7.jpg") },
            //    new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/8.jpg") },
            //    new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/9.jpg") }
            //};

            Images = new ExtendedObservableCollection<ImageModel>()
            {
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/0.jpg"), Tag = "0" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/1.jpg"), Tag = "1" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/2.jpg"), Tag = "2" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/3-4.jpg"), Tag = "3-4" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/5-6.jpg"), Tag = "5-6" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/7a.jpg"), Tag = "7a" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/8a.jpg"), Tag = "8a" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/9.jpg"), Tag = "9" }
            };
        }

        #endregion

        #region Public Properties

        public ExtendedObservableCollection<ImageModel> Images
        {
            get => _imageSource;
            set => SetField(ref _imageSource, value);
        }

        public FillType FillType
        {
            get => _fillType;
            set => SetField(ref _fillType, value);
        }

        public bool AutoSizeMode
        {
            get => _autoSizeMode;
            set => SetField(ref _autoSizeMode, value);
        }

        #endregion
    }
}
