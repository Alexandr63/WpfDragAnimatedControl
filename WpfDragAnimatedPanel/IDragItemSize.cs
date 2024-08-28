using System.Windows;

namespace WpfDragAnimatedPanel
{
    public interface IDragItemSize 
    {
        double Width { get; set; }

        double Height { get; set; }

        Size GetSize();
    }
}
