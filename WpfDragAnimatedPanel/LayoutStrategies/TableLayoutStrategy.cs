using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfDragAnimatedPanel.LayoutStrategies
{
    /// <summary>
    /// Стратегия отображения дочерних элементов в виде таблицы с построчным заполнением с различной шириной строк.
    /// </summary>
    public class TableLayoutStrategy : ILayoutStrategy
    {
        #region Private Fields

        private readonly List<double> _rowHeights = new List<double>();
        private readonly List<double> _columnWidths = new List<double>();
        
        #endregion

        #region ILayoutStrategy Implementation

        public Size ResultSize => _columnWidths.Any() && _rowHeights.Any() ? new Size(_columnWidths.Sum(), _rowHeights.Sum()) : new Size(0, 0);

        public void MeasureLayout(Size availableSize, List<Size> measures, bool isDragging)
        {
            if (!isDragging)
            {
                _rowHeights.Clear();
            }
            int columnsCount = _columnWidths.Count;

            _columnWidths.Clear();

            if (!measures.Any())
            {
                return;
            }

            if (!isDragging)
            {
                columnsCount = GetColumnCount(availableSize, measures);
            }

            for (int i = 0; i < columnsCount; i++)
            {
                _columnWidths.Add(0d);
            }

            if (!isDragging)
            {
                int rowsCount = Convert.ToInt32(Math.Ceiling(measures.Count / (double) columnsCount));
                for (int i = 0; i < rowsCount; i++)
                {
                    _rowHeights.Add(0d);
                }
            }

            for (int index = 0; index < measures.Count; index++)
            {
                int columnIndex = index % _columnWidths.Count;
                if (_columnWidths[columnIndex] < measures[index].Width)
                {
                    _columnWidths[columnIndex] = measures[index].Width;
                }

                if (!isDragging)
                {
                    int rowIndex = index / _columnWidths.Count;
                    if (_rowHeights[rowIndex] < measures[index].Height)
                    {
                        _rowHeights[rowIndex] = measures[index].Height;
                    }
                }
            }
        }

        public Rect GetPosition(int index)
        {
            int columnIndex = index % _columnWidths.Count;
            int rowIndex = index / _columnWidths.Count;

            double x = 0d;
            for (int i = 0; i < columnIndex; i++)
            {
                x += _columnWidths[i];
            }

            double y = 0d;
            for (int i = 0; i < rowIndex; i++)
            {
                y += _rowHeights[i];
            }

            return new Rect(new Point(x, y), new Size(_columnWidths[columnIndex], _rowHeights[rowIndex]));
        }

        public int GetIndex(Point position)
        {
            double x = 0d;
            int columnIndex = 0;
            while (true)
            {
                if (columnIndex < _columnWidths.Count)
                {
                    x += _columnWidths[columnIndex];

                    if (position.X < x)
                    {
                        break;
                    }
                }
                else
                {
                    columnIndex--;
                    break;
                }

                columnIndex++;
            }

            double y = 0d;
            int rowIndex = 0;
            while (true)
            {
                if (rowIndex < _rowHeights.Count)
                {
                    y += _rowHeights[rowIndex];

                    if (position.Y < y)
                    {
                        break;
                    }
                }
                else
                {
                    rowIndex--;
                    break;
                }

                rowIndex++;
            }
            
            int index = (rowIndex * _columnWidths.Count) + columnIndex;
            
            return index;
        }

        public DragItemLayoutInfo GetLayoutInfo(int index)
        {
            int columnIndex = index % _columnWidths.Count; 
            int rowIndex = index / _columnWidths.Count;

            return new DragItemLayoutInfo()
            {
                ColumnIndex = columnIndex,
                RowIndex = rowIndex,
                ColumnWidth = _columnWidths[columnIndex],
                RowHeight = _rowHeights[rowIndex]
            };
        }

        #endregion

        #region Private Methods

        private int GetColumnCount(Size availableSize, List<Size> measures)
        {
            int columnsCount;
            // Цикл по возможному количеству колонок. От максимального числа к минимальному.
            for (columnsCount = measures.Count; columnsCount > 0; columnsCount--)
            {
                // Если columnCount == 1 - значит будет одна колонка в таблице и, скорее всего, она выйдет за пределы контрола.
                if (columnsCount == 1)
                {
                    break;
                }

                // Считаем длину строки
                double rowWidth = 0d;
                for (int columnIndex = 0; columnIndex < columnsCount; columnIndex++)
                {
                    // Получаем максимальную ширину колонки
                    rowWidth += GetMaxColumnSize(measures, columnsCount, columnIndex);
                }

                // Если длина строки меньше заданной величины - мы нашли максимальное число колонок в таблице
                if (rowWidth < availableSize.Width)
                {
                    break;
                }
            }

            return columnsCount;
        }

        private double GetMaxColumnSize(List<Size> measures, int columnsCount, int columnIndex)
        {
            int rowsCount = Convert.ToInt32(Math.Ceiling(measures.Count / (double)columnsCount));

            double width = 0d;
            for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                int index = rowIndex * columnsCount + columnIndex;

                if (index < measures.Count)
                {
                    if (width < measures[index].Width)
                    {
                        width = measures[index].Width;
                    }
                }
            }

            return width;
        }

        #endregion
    }
}