using System.IO;
using System.Windows.Media.Imaging;

namespace WpfDragAnimatedControl.Example.Helpers
{
    public static class BitmapImageHelper
    {
        public static BitmapImage FileToBitmapImage(string filePath)
        {
            using MemoryStream ms = new MemoryStream(File.ReadAllBytes(filePath));
            
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }
    }
}
