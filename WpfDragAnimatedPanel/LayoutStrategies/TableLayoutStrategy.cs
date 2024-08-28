using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfDragAnimatedPanel.LayoutStrategies
{
    /// <summary>
    /// Стратегия отображения дочерних элементов в виде таблицы с построчным заполнением с различной шириной строк.
    /// TODO переделать, кривая логика в MeasureLayout и вложенных методах. Избавиться от _colWidths (заменить на лист), _columnCount и, возможно _elementCount
    /// </summary>
    public class TableLayoutStrategy : ILayoutStrategy
    {
        #region Private Fields

        private int _columnCount;
        private double[] _colWidths;
        private readonly List<double> _rowHeights = new List<double>();
        private int _elementCount;

        #endregion

        #region ILayoutStrategy Implementation

        public Size ResultSize => _colWidths != null && _rowHeights.Any() ? new Size(_colWidths.Sum(), _rowHeights.Sum()) : new Size(0, 0);

        public void MeasureLayout(Size availableSize, List<Size> measures, bool isDragging)
        {
            BaseCalculation(availableSize, measures, isDragging);
            AdjustEmptySpace(availableSize);
        }

        public Rect GetPosition(int index)
        {
            int columnIndex = index % _columnCount;
            int rowIndex = index / _columnCount;

            double x = 0d;
            for (int i = 0; i < columnIndex; i++)
            {
                x += _colWidths[i];
            }

            double y = 0d;
            for (int i = 0; i < rowIndex; i++)
            {
                y += _rowHeights[i];
            }

            return new Rect(new Point(x, y), new Size(_colWidths[columnIndex], _rowHeights[rowIndex]));
        }

        public int GetIndex(Point position)
        {
            int col = 0;
            double x = 0d;
            while (x < position.X && _columnCount > col)
            {
                x += _colWidths[col];
                col++;
            }

            col--;
            int row = 0;
            double y = 0d;
            while (y < position.Y && _rowHeights.Count > row)
            {
                y += _rowHeights[row];
                row++;
            }

            row--;
            if (row < 0)
            {
                row = 0;
            }

            if (col < 0)
            {
                col = 0;
            }

            if (col >= _columnCount)
            {
                col = _columnCount - 1;
            }

            int result = row * _columnCount + col;
            if (result > _elementCount)
            {
                result = _elementCount - 1;
            }

            return result;
        }

        public double GetColumnWidthByElementIndex(int index)
        {
            int i = 0;
            for (int rowIndex = 0; rowIndex < _rowHeights.Count; rowIndex++)
            {
                if (i + _colWidths.Length < index)
                {
                    i += _colWidths.Length;
                }
                else
                {
                    return _colWidths[i + _colWidths.Length - index - 1];
                }
            }
            
            throw new IndexOutOfRangeException($"Element with index {index} not found.");
        }

        public double GetRowHeightByElementIndex(int index)
        {
            int i = 0;
            foreach (double rowHeight in _rowHeights)
            {
                if (i + _colWidths.Length < index)
                {
                    i += _colWidths.Length;
                }
                else
                {
                    return rowHeight;
                }
            }

            throw new IndexOutOfRangeException($"Element with index {index} not found.");
        }

        public DragItemLayoutInfo GetLayoutInfo(int index)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private void BaseCalculation(Size availableSize, List<Size> measures, bool isDragging)
        {
            _elementCount = measures.Count;
            _columnCount = GetColumnCount(availableSize, measures);
            if (_colWidths == null || _colWidths.Length < _columnCount)
            {
                _colWidths = new double[_columnCount];
            }

            bool calculating = true;
            while (calculating)
            {
                calculating = false;
                ResetSizes(isDragging);

                for (int row = 0; row * _columnCount < measures.Count; row++)
                {
                    double rowHeight = 0.0;
                    for (int col = 0; col < _columnCount; col++)
                    {
                        int i = row * _columnCount + col;
                        if (i >= measures.Count)
                        {
                            break;
                        }

                        _colWidths[col] = Math.Max(_colWidths[col], measures[i].Width);
                        rowHeight = Math.Max(rowHeight, measures[i].Height);
                    }

                    if (_columnCount > 1 && _colWidths.Sum() > availableSize.Width)
                    {
                        _columnCount--;
                        calculating = true;
                        break;
                    }

                    if (!isDragging)
                    {
                        _rowHeights.Add(rowHeight);
                    }
                }
            }
        }

        private void AdjustEmptySpace(Size availableSize)
        {
            double width = _colWidths.Sum();
            if (!double.IsNaN(availableSize.Width) && availableSize.Width > width)
            {
                double dif = (availableSize.Width - width) / _columnCount;
                for (int i = 0; i < _columnCount; i++)
                {
                    _colWidths[i] += dif;
                }
            }
        }

        private void ResetSizes(bool isDragging)
        {
            if (!isDragging)
            {
                _rowHeights.Clear();
            }

            for (int j = 0; j < _colWidths.Length; j++)
            {
                _colWidths[j] = 0;
            }
        }

        private static int GetColumnCount(Size availableSize, List<Size> measures)
        {
            double width = 0;
            for (int colCnt = 0; colCnt < measures.Count; colCnt++)
            {
                double nWidth = width + measures[colCnt].Width;
                if (nWidth > availableSize.Width)
                {
                    return Math.Max(1, colCnt);
                }
                width = nWidth;
            }

            return measures.Count;
        }

        #endregion
    }
}