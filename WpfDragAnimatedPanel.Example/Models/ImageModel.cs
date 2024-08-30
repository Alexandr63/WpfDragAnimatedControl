using System.Windows;
using System.Windows.Media;

namespace WpfDragAnimatedPanel.Example.Models
{
    public class ImageModel : NotifyPropertyChangedObject, IDragItemSize
    {
        private ImageSource _imageSource;
        private double _width = 0d;
        private double _height = 0d;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                if (value != _imageSource)
                {
                    SetField(ref _imageSource, value);
                }

                if (value == null)
                {
                    Width = 0d;
                    Height = 0d;
                }
                else
                {
                    Width = ImageSource.Width;
                    Height = ImageSource.Height;
                }
            }
        }

        public double Width
        {
            get => _width;
            set => SetField(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => SetField(ref _height, value);
        }

        public string Tag { get; init; }

        public Size GetSize()
        {
            return new Size(Width, Height);
        }

        public override string ToString()
        {
            return $"{Tag}\tSize: {GetSize()}";
        }
    }
}
