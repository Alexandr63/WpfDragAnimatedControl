using System.Collections.Generic;
using System.Linq;
using WpfDragAnimatedControl.Example.Helpers;
using WpfDragAnimatedControl.Example.Models;
using WpfDragAnimatedControl.Tools;

namespace WpfDragAnimatedControl.Example.ViewModels
{
    public class MainWindowViewModel : NotifyPropertyChangedObject
    {
        #region Private Fields

        private ExtendedObservableCollection<ImageModel> _imageSource;
        private double _itemSizeMultiplier = 1d;

        // NOTE поставить этот параметр в true, если нужно отображать все элементы одинакового размера
        private const bool IS_SAME_SIZE = false;

        // NOTE если нужно отображать все элементы одинакового размера - установить в этом параметре ширину элемента
        private const double SAME_SIZE_WIDTH = 248d;

        // NOTE если нужно отображать все элементы одинакового размера - установить в этом параметре высоту элемента
        private const double SAME_SIZE_HEIGHT = 350d;

        #endregion

        #region Ctor

        public MainWindowViewModel()
        {
            Images = new ExtendedObservableCollection<ImageModel>()
            {
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/0.jpg"), Tag = "0" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/1.jpg"), Tag = "1" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/2.jpg"), Tag = "2" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/3-4.jpg"), Tag = "3-4" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/5-6.jpg"), Tag = "5-6" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/7a.jpg"), Tag = "7a" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/8a.jpg"), Tag = "8a" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/9.jpg"), Tag = "9" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/10-11a.jpg"), Tag = "10-11a" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/12-13a.jpg"), Tag = "12-13a" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/14.jpg"), Tag = "14" }
            };

            if (IS_SAME_SIZE)
            {
                foreach (ImageModel imageModel in Images)
                {
                    imageModel.Width = SAME_SIZE_WIDTH;
                    imageModel.Height = SAME_SIZE_HEIGHT;
                }    
            }
            else
            {
                const double START_HEIGHT = 200d;
                ItemSizeMultiplier = START_HEIGHT / Images.Max(x => x.Height);

                Resize(Images);
            }
        }

        #endregion

        #region Public Properties

        public ExtendedObservableCollection<ImageModel> Images
        {
            get => _imageSource;
            set => SetField(ref _imageSource, value);
        }

        public double ItemSizeMultiplier
        {
            get => _itemSizeMultiplier;
            set => SetField(ref _itemSizeMultiplier, value);
        }

        #endregion

        #region Public Methods

        public void AddImages()
        {
            List<ImageModel> images = new List<ImageModel>()
            {
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/0.jpg"), Tag = "0" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/1.jpg"), Tag = "1" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/2.jpg"), Tag = "2" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/3-4.jpg"), Tag = "3-4" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/5-6.jpg"), Tag = "5-6" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/7a.jpg"), Tag = "7a" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/8a.jpg"), Tag = "8a" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/9.jpg"), Tag = "9" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/10-11a.jpg"), Tag = "10-11a" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/12-13a.jpg"), Tag = "12-13a" },
                new ImageModel { ImageSource = BitmapImageHelper.FileToBitmapImage("Images/14.jpg"), Tag = "14" }
            };

            if (IS_SAME_SIZE)
            {
                foreach (ImageModel imageModel in images)
                {
                    imageModel.Width = SAME_SIZE_WIDTH;
                    imageModel.Height = SAME_SIZE_HEIGHT;
                }
            }

            Resize(images);
            
            Images.AddRange(images);
        }

        #endregion

        #region Private Methods

        private void Resize(IEnumerable<ImageModel> images)
        {
            foreach (ImageModel imageModel in images)
            {
                imageModel.Height *= ItemSizeMultiplier;
                imageModel.Width *= ItemSizeMultiplier;
            }
        }

        #endregion
    }
}
