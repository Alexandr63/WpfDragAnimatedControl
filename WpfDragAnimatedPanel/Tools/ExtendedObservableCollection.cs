using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace WpfDragAnimatedPanel.Tools
{
    public sealed class ExtendedObservableCollection<T> : ObservableCollection<T>
        where T : class, INotifyPropertyChanged
    {
        #region Ctor.

        public ExtendedObservableCollection() : base()
        {
            CollectionChanged += ObservableCollectionChangedEventHandler;
        }

        #endregion

        #region Public Methods

        public void Cleanup()
        {
            foreach (T item in this)
            {
                item.PropertyChanged -= ItemPropertyChangedEventHandler;
            }

            Clear();
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            using (IEnumerator<T> en = collection.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    Add(en.Current);
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void RemoveItem(int index)
        {
            if (index >= Count)
            {
                throw new IndexOutOfRangeException($"Collection has {Count} items. Index of item for removing is {index}.");
            }

            this[index].PropertyChanged -= ItemPropertyChangedEventHandler;
            base.RemoveItem(index);
        }

        #endregion

        #region Private Methods

        private void ObservableCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChangedEventHandler;
                }
            }

            if (e.OldItems != null)
            {
                foreach (object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChangedEventHandler;
                }
            }
        }

        private void ItemPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            T senderItem = sender as T;
            if (senderItem == null)
            {
                return;
            }

            if (Contains(senderItem))
            {
                NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, senderItem, senderItem, IndexOf(senderItem));
                OnCollectionChanged(args);
            }
        }

        #endregion
    }
}
