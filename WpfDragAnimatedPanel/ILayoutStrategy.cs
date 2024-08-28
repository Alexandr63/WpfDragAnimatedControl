using System.Collections.Generic;
using System.Windows;

namespace WpfDragAnimatedPanel
{
    /// <summary>
    /// ��������� ����������� �������� ��������� �� ������.
    /// </summary>
    public interface ILayoutStrategy
    {
        /// <summary>
        /// ������ ������. 
        /// </summary>
        Size ResultSize { get; }

        /// <summary>
        /// ��������� ������� ������ � ������������ �������� ���������.
        /// </summary>
        /// <param name="availableSize">��������� ������ ������.</param>
        /// <param name="sizes">������� �������� ���������.</param>
        /// <param name="isDragging">�������, ��� ����������� ������� ���������.</param>
        void MeasureLayout(Size availableSize, List<Size> sizes, bool isDragging);

        /// <summary>
        /// ���������� ������������ � ������ ��������� �������� �� ��� �������.
        /// </summary>
        /// <param name="index">������ ��������� ��������.</param>
        Rect GetPosition(int index);

        /// <summary>
        /// ���������� ������ ��������� �������� �� ��������� �����������.
        /// </summary>
        /// <param name="position">����������, �� ������� ���������� �������� �������.</param>
        int GetIndex(Point position);

        /// <summary>
        /// ���������� ���������� � ������������ �������� �� ��� �������.
        /// </summary>
        /// <param name="index">������ ��������.</param>
        DragItemLayoutInfo GetLayoutInfo(int index);
    }
}