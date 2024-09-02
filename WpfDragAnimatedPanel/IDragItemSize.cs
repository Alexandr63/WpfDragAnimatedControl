using System.Windows;

namespace WpfDragAnimatedControl
{
    public interface IDragItemSize 
    {
        double Width { get; set; }

        double Height { get; set; }

        Size GetSize();
    }
}
