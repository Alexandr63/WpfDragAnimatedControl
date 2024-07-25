using System.Windows.Media;

namespace WpfDragAnimatedPanel.Example.Models
{
    public class ImageModel : NotifyPropertyChangedObject
    {
        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetField(ref _imageSource, value);
        }
    }
}
