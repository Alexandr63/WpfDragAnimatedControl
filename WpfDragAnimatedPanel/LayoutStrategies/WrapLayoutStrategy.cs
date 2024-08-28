using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace WpfDragAnimatedPanel.LayoutStrategies
{
    /// <summary>
    /// Стратегия отображения дочерних элементов с построчным заполнением.
    /// </summary>
    public class WrapLayoutStrategy : ILayoutStrategy
    {
        #region Private Fields

        private readonly List<List<Size>> _rows = new List<List<Size>>();
        private readonly List<double> _rowHeights = new List<double>();

        private readonly List<DragItemLayoutInfo> _itemsLayoutInfos = new List<DragItemLayoutInfo>();

        #endregion

        #region ILayoutStrategy Implementation

        public Size ResultSize => _rowHeights.Any() ? new Size(_rows.Select(row => row.Sum(item => item.Width)).Max(), _rowHeights.Sum()) : new Size(0, 0);

        public void Calculate(Size availableSize, List<Size> measures, bool isDragging)
        {
            System.Diagnostics.Debug.WriteLine($">>> WrapLayoutStrategy Calculate availableSize:{availableSize}");

            if (!isDragging)
            {
                _rowHeights.Clear();
            }

            _rows.Clear();

            if (!measures.Any())
            {
                return;
            }

            double rowWidth = 0;
            double rowHeight = 0;
            List<Size> row = null;

            int itemIndex = 0;
            bool isNewRow = true;

int rowIndex = 0;
int columnIndex = 0;
_itemsLayoutInfos.Clear();

            // Заполняем строки
            while (true)
            {
                // Прошлись по всем элементам
                if (measures.Count <= itemIndex)
                {
                    break;
                }

                // Размер текущего элемента
                Size itemSize = measures[itemIndex];

                // Если это первый элемент в строке - добавляем его
                if (isNewRow)
                {
                    isNewRow = false;

                    row = new List<Size> { itemSize };
                    _rows.Add(row);
                    rowWidth = itemSize.Width;
                    rowHeight = itemSize.Height;

                    itemIndex++;

_itemsLayoutInfos.Add(new DragItemLayoutInfo(rowIndex, columnIndex, itemSize));
columnIndex++;
                }
                // Если это как минимум второй элемент в строке - проверяем
                else
                {
                    // Новый элемент помещается в строку
                    if (rowWidth + itemSize.Width < availableSize.Width)
                    {
                        // Добавляем элемент в строку
                        row.Add(itemSize);
                        rowWidth += itemSize.Width;
                        if (rowHeight < itemSize.Height)
                        {
                            rowHeight = itemSize.Height;
                        }

                        itemIndex++;

_itemsLayoutInfos.Add(new DragItemLayoutInfo(rowIndex, columnIndex, itemSize));
columnIndex++;
                    }
                    // Новый элемент не помещается в строку
                    else
                    {
                        // Он будет добавлен на следующую строку, завершаем текущую строку
                        isNewRow = true;

                        if (!isDragging)
                        {
                            _rowHeights.Add(rowHeight);
                        }

columnIndex = 0;
rowIndex++;
                    }
                }
            }

            if (!isDragging)
            {
                _rowHeights.Add(rowHeight);
            }

System.Diagnostics.Debug.WriteLine($">>> WrapLayoutStrategy Calculate _rowHeights:{string.Join(',', _rowHeights)} ; ResultSize:{ResultSize}");
foreach (DragItemLayoutInfo layoutInfo in _itemsLayoutInfos)
{
    System.Diagnostics.Debug.WriteLine($"\t{layoutInfo.RowIndex};{layoutInfo.ColumnIndex} {layoutInfo.Size}");
}
        }

        public Rect GetPosition(int index)
        {
            int counter = 0;
            int rowIndex = 0;
            int elementIndex = 0;

            foreach (List<Size> row in _rows)
            {
                // Проверяем, что элемент находится в этой строке
                if (counter + row.Count > index)
                {
                    elementIndex = index - counter;
                    break;
                }

                rowIndex++;
                counter += row.Count;
            }

            double x = 0d;
            if (elementIndex > 0)
            {
                for (int i = 0; i < elementIndex; i++)
                {
                    x += _rows[rowIndex][i].Width;
                }
            }

            double y = 0d;
            for (int i = 0; i < rowIndex; i++)
            {
                y += _rowHeights[i];
            }

            try
            {
                return new Rect(new Point(x, y), new Size(_rows[rowIndex][elementIndex].Width, _rowHeights[rowIndex]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Rect();
            }
        }

        public int GetIndex(Point position)
        {
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

            double x = 0d;
            int elementIndex = 0;
            while (true)
            {
                if (elementIndex < _rows[rowIndex].Count)
                {
                    x += _rows[rowIndex][elementIndex].Width;

                    if (position.X < x)
                    {
                        break;
                    }
                }
                else
                {
                    elementIndex--;
                    break;
                }

                elementIndex++;
            }

            int index = 0;
            for (int i = 0; i < rowIndex; i++)
            {
                index += _rows[i].Count;
            }
            index += elementIndex;

            return index;
        }

        public double GetColumnWidthByElementIndex(int index)
        {
            int i = 0;
            foreach (List<Size> row in _rows)
            {
                foreach (Size item in row)
                {
                    if (i == index)
                    {
                        return item.Width;
                    }

                    i++;
                }
            }

            throw new IndexOutOfRangeException($"Element with index {index} not found.");
        }

        public double GetRowHeightByElementIndex(int index)
        {
            int i = 0;
            int rowIndex = 0;
            foreach (List<Size> row in _rows)
            {
                if (i + row.Count > index)
                {
                    System.Diagnostics.Debug.WriteLine($">>> WrapLayoutStrategy GetRowHeightByElementIndex index:{index} ; Height:{_rowHeights[rowIndex]}");

                    return _rowHeights[rowIndex];
                }

                i += row.Count;
                rowIndex++;
            }

            throw new IndexOutOfRangeException($"Element with index {index} not found.");
        }

        public DragItemLayoutInfo GetLayoutInfo(int index)
        {
            return _itemsLayoutInfos[index];
        }

        #endregion
    }
}
