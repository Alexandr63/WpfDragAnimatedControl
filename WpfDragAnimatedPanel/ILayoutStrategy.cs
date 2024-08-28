using System.Collections.Generic;
using System.Windows;

namespace WpfDragAnimatedPanel
{
    /// <summary>
    /// Стратегия отображения дочерних элементов на панели.
    /// </summary>
    public interface ILayoutStrategy
    {
        /// <summary>
        /// Размер панели. 
        /// </summary>
        Size ResultSize { get; }

        /// <summary>
        /// Вычислить размеры панели и расположение дочерних элементов.
        /// </summary>
        /// <param name="availableSize">Доступный размер панели.</param>
        /// <param name="sizes">Размеры дочерних элементов.</param>
        /// <param name="isDragging">Признак, что выполняется перенос элементов.</param>
        void Calculate(Size availableSize, List<Size> sizes, bool isDragging);

        /// <summary>
        /// Возвращает расположение и размер дочернего элемента по его индексу.
        /// </summary>
        /// <param name="index">Индекс дочернего элемента.</param>
        Rect GetPosition(int index);

        /// <summary>
        /// Возвращает индекс дочернего элемента по указанным координатам.
        /// </summary>
        /// <param name="position">Координаты, по которым расположен дочерний элемент.</param>
        int GetIndex(Point position);

        /// <summary>
        /// Возвращает ширину колонки, в которой находится элемент с заданным индексом.
        /// </summary>
        /// <param name="index">Индекс элемента.</param>
        double GetColumnWidthByElementIndex(int index);

        /// <summary>
        /// Возвращает высоту строки, в которой находится элемент с заданным индексом.
        /// </summary>
        /// <param name="index">Индекс элемента.</param>
        double GetRowHeightByElementIndex(int index);

        /// <summary>
        /// Возвращает информацию о расположении элемента по его индексу.
        /// </summary>
        /// <param name="index">Индекс элемента.</param>
        DragItemLayoutInfo GetLayoutInfo(int index);
    }
}