using System;
using System.Collections.Generic;
using System.Linq;
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

        public void MeasureLayout(Size availableSize, List<Size> measures, bool isDragging)
        {
            System.Diagnostics.Debug.WriteLine($">>> WrapLayoutStrategy MeasureLayout availableSize:{availableSize}");

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

                    _itemsLayoutInfos.Add(new DragItemLayoutInfo()
                    {
                        RowIndex = rowIndex,
                        ColumnIndex = columnIndex,
                        ColumnWidth = itemSize.Width
                    });

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

                        _itemsLayoutInfos.Add(new DragItemLayoutInfo()
                        {
                            RowIndex = rowIndex,
                            ColumnIndex = columnIndex,
                            ColumnWidth = itemSize.Width
                        });

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

            UpdateRowHeightsInLayoutInfos();

            
            System.Diagnostics.Debug.WriteLine($">>> WrapLayoutStrategy {string.Join(',', _itemsLayoutInfos.Select(x=>x.ColumnWidth))}");
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
                    // NOTE деление на 2 надо для более корректного отображения в ситуации, когда мы перетаскиваем элемент с индексом i из строки j, находящийся в конце строки на позицию i+1 в строке, находящийся в строке j+1.
                    // При этом элемент, находящийся на позиции i+1, слишком широкий, и не может быть перенесен на предыдущую строку. В этом случаи возникает дергание элементов при перемещении мыши.
                    x += _rows[rowIndex][elementIndex].Width / 2d;

                    if (position.X < x)
                    {
                        break;
                    }

                    x += _rows[rowIndex][elementIndex].Width / 2d;

                    if (position.X < x)
                    {
                        if (elementIndex + 1 < _rows[rowIndex].Count)
                        {
                            elementIndex++;
                        }

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
            /*
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
            */
        }

        public DragItemLayoutInfo GetLayoutInfo(int index)
        {
            return _itemsLayoutInfos[index];
        }

        #endregion

        #region Private Methods

        private void UpdateRowHeightsInLayoutInfos()
        {
            for (int rowIndex = 0; rowIndex < _itemsLayoutInfos.Max(x => x.RowIndex); rowIndex++)
            {
                foreach (DragItemLayoutInfo layoutInfo in _itemsLayoutInfos.Where(x => x.RowIndex == rowIndex))
                {
                    layoutInfo.RowHeight = _rowHeights[rowIndex];
                }
            }
        }

        #endregion
    }
}
