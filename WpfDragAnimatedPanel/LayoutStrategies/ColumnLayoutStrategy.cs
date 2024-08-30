using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfDragAnimatedPanel.LayoutStrategies
{
    /// <summary>
    /// Стратегия отображения дочерних элементов на в столбик.
    /// </summary>
    public class ColumnLayoutStrategy : ILayoutStrategy
    {
        #region Private Fields

        private readonly List<Size> _column = new List<Size>();
        private double _width = 0d;

        #endregion

        #region ILayoutStrategy Implementation

        public Size ResultSize => _column.Any() ? new Size(_width, _column.Sum(item => item.Height)) : new Size(0, 0);

        public void MeasureLayout(Size availableSize, List<Size> measures, bool isDragging)
        {
            _width = 0d;
            _column.Clear();

            if (!measures.Any())
            {
                return;
            }

            foreach (Size measure in measures)
            {
                if (measure.Width > _width)
                {
                    _width = measure.Width;
                }

                _column.Add(measure);
            }
        }

        public Rect GetPosition(int index)
        {
            double y = 0d;
            for (int i = 0; i < index; i++)
            {
                y += _column[i].Height;
            }

            return new Rect(new Point(0d, y), new Size(_width, _column[index].Height));
        }

        public int GetIndex(Point position)
        {
            double y = 0d;
            int index = 0;
            while (true)
            {
                if (index < _column.Count)
                {
                    y += _column[index].Height;

                    if (position.Y < y)
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
                ColumnIndex = 0,
                RowIndex = index,
                ColumnWidth = _width,
                RowHeight = _column[index].Height
            };
        }

        #endregion
    }
}
