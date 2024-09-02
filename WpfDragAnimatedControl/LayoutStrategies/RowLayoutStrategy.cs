using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfDragAnimatedControl.LayoutStrategies
{
    /// <summary>
    /// Стратегия отображения дочерних элементов на в строку.
    /// </summary>
    public class RowLayoutStrategy : ILayoutStrategy
    {
        #region Private Fields

        private readonly List<Size> _row = new List<Size>();
        private double _height = 0d;

        #endregion

        #region ILayoutStrategy Implementation

        public Size ResultSize => _row.Any() ? new Size(_row.Sum(item => item.Width), _height) : new Size(0, 0);

        public void MeasureLayout(Size availableSize, List<Size> measures, bool isDragging)
        {
            _height = 0d;
            _row.Clear();

            if (!measures.Any())
            {
                return;
            }

            foreach (Size measure in measures)
            {
                if (measure.Height > _height)
                {
                    _height = measure.Height;
                }

                _row.Add(measure);
            }
        }

        public Rect GetPosition(int index)
        {
            double x = 0d;
            for (int i = 0; i < index; i++)
            {
                x += _row[i].Width;
            }

            return new Rect(new Point(x, 0d), new Size(_row[index].Width, _height));
        }

        public int GetIndex(Point position)
        {
            double x = 0d;
            int index = 0;
            while (true)
            {
                if (index < _row.Count)
                {
                    x += _row[index].Width;

                    if (position.X < x)
                    {
                        break;
                    }
                }
                else
                {
                    index--;
                    break;
                }

                index++;
            }

            return index;
        }

        public DragItemLayoutInfo GetLayoutInfo(int index)
        {
            return new DragItemLayoutInfo()
            {
                ColumnIndex = index,
                RowIndex = 0,
                ColumnWidth = _row[index].Width,
                RowHeight = _height
            };
        }

        #endregion
    }
}
