using System.Windows;

namespace WpfDragAnimatedPanel
{
    public class DragItemLayoutInfo
    {
        public DragItemLayoutInfo()
        {
        }

        public int RowIndex { get; set; }

        public int ColumnIndex { get; set; }

        public double ColumnWidth { get; set; }

        public double RowHeight { get; set; }
    }
}
