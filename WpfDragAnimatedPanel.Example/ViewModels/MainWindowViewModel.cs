using System.Collections.Generic;
using System.Linq;
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
        private bool _autoSizeMode = false;
        private double _multiplier = 1d;

        private bool IS_SAME_SIZE = true;
        private double SAME_SIZE_WIDTH = 248d;
        private double SAME_SIZE_HEIGHT = 350d;

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
                Multiplier *= START_HEIGHT / Images.Max(x => x.Height);
            }

            Resize(Images);
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

        public double Multiplier
        {
            get => _multiplier;
            set => SetField(ref _multiplier, value);
        }

        #endregion

        #region Public Methods

        public void AddImages()
        {
            List<ImageModel> images = new List<ImageModel>()
            {
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/0.jpg"), Tag = "0"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/1.jpg"), Tag = "1"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/2.jpg"), Tag = "2"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/3-4.jpg"), Tag = "3-4"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/5-6.jpg"), Tag = "5-6"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/7a.jpg"), Tag = "7a"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/8a.jpg"), Tag = "8a"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images2/9.jpg"), Tag = "9"},

                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images/0.jpg"), Tag = "00"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images/1.jpg"), Tag = "01"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images/2.jpg"), Tag = "02"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images/3.jpg"), Tag = "03"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images/4.jpg"), Tag = "04"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images/5.jpg"), Tag = "05"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images/6.jpg"), Tag = "06"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images/7.jpg"), Tag = "07"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images/8.jpg"), Tag = "08"},
                new ImageModel {ImageSource = BitmapImageHelper.FileToBitmapImage("Images/9.jpg"), Tag = "09"}
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
                imageModel.Height *= Multiplier;
                imageModel.Width *= Multiplier;
            }
        }

        #endregion
    }
}
