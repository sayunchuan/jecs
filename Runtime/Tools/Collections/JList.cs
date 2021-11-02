using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JECS
{
    public class JList<T>
    {
        private int size;
        private T[] buffer;

        public IEqualityComparer<T> _compare;

        public JList()
        {
            _Init(null);
        }

        public JList(IEnumerable<T> collection)
        {
            using (IEnumerator<T> en = collection.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    Add(en.Current);
                }
            }

            _Init(null);
        }

        public JList(IEqualityComparer<T> comp)
        {
            _Init(comp);
        }

        private void _Init(IEqualityComparer<T> comp)
        {
            _compare = comp == null ? EqualityComparer<T>.Default : comp;
        }

        #region Interface

        public int Count => size;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (buffer == null || size == buffer.Length)
            {
                AllocateMore();
            }

            buffer[size++] = item;
        }

        public void Clear()
        {
            if (buffer != null) Array.Clear(buffer, 0, size);
            size = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < size; i++)
            {
                array[arrayIndex + i] = buffer[i];
            }
        }

        public bool Remove(T item)
        {
            if (buffer != null)
            {
                for (int i = 0; i < size; ++i)
                {
                    if (_compare.Equals(buffer[i], item))
                    {
                        --size;
                        buffer[i] = default;
                        for (int b = i; b < size; ++b)
                        {
                            buffer[b] = buffer[b + 1];
                        }

                        buffer[size] = default;
                        return true;
                    }
                }
            }

            return false;
        }

        public T this[int i]
        {
            get { return buffer[i]; }
            set { buffer[i] = value; }
        }

        public int IndexOf(T item)
        {
            if (buffer == null)
            {
                return -1;
            }

            for (int i = 0; i < size; ++i)
            {
                if (_compare.Equals(buffer[i], item))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            if (buffer == null || size == buffer.Length)
            {
                AllocateMore();
            }

            if (index > -1 && index < size)
            {
                for (int i = size; i > index; --i)
                    buffer[i] = buffer[i - 1];
                buffer[index] = item;
                ++size;
            }
            else
            {
                Add(item);
            }
        }

        public void RemoveAt(int index)
        {
            if (buffer != null && index > -1 && index < size)
            {
                --size;
                buffer[index] = default;
                for (int b = index; b < size; ++b)
                {
                    buffer[b] = buffer[b + 1];
                }

                buffer[size] = default;
            }
        }

        #endregion

        #region Methods

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public void AddRange(JList<T> clist)
        {
            for (int i = 0, imax = clist.Count; i < imax; i++)
            {
                Add(clist[i]);
            }
        }

        public T[] ToArray()
        {
            if (size > 0)
            {
                if (size <= buffer.Length)
                {
                    T[] newList = new T[size];
                    Array.Copy(buffer, 0, newList, 0, size);
                    return newList;
                }
            }

            return null;
        }

        public void Release()
        {
            size = 0;
            buffer = null;
        }

        #endregion

        private void AllocateMore()
        {
            T[] newList = (buffer != null) ? new T[Math.Max((buffer.Length << 1) + 1, 3)] : new T[3];
            if (buffer != null && size > 0)
                buffer.CopyTo(newList, 0);
            buffer = newList;
        }

        #region ICloneable implementation

        public object Clone()
        {
            JList<T> res = new JList<T>();
            res.size = size;
            res.buffer = new T[buffer.Length];
            if (buffer != null && size > 0)
            {
                buffer.CopyTo(res.buffer, 0);
            }

            return res;
        }

        #endregion

        #region Sort About

        [DebuggerHidden]
        [DebuggerStepThrough]
        public void Sort(Comparison<T> comparer)
        {
            if (size <= 0)
            {
                return;
            }

            HeapSort(comparer);
        }

        /// <summary>
        /// 堆排序算法 
        /// </summary>
        /// <param name="comparer">Comparer.</param>
        private void HeapSort(Comparison<T> comparer)
        {
            //初始堆  
            BuildingHeap(comparer);
            //从最后一个元素开始对序列进行调整  
            for (int i = size - 1; i > 0; --i)
            {
                //交换堆顶元素H[0]和堆中最后一个元素  
                T temp = buffer[i];
                buffer[i] = buffer[0];
                buffer[0] = temp;
                //每次交换堆顶元素和堆中最后一个元素之后，都要对堆进行调整  
                HeapAdjust(0, i, comparer);
            }
        }

        /// <summary>
        /// 初始堆进行调整 
        /// 将H[0..length-1]建成堆 
        /// 调整完之后第一个元素是序列的最小的元素 
        /// </summary>
        /// <param name="comparer">Comparer.</param>
        private void BuildingHeap(Comparison<T> comparer)
        {
            //最后一个有孩子的节点的位置 i=  (length -1) / 2  
            for (int i = (size - 1) / 2; i >= 0; --i)
            {
                HeapAdjust(i, size, comparer);
            }
        }

        /// <summary>
        /// 已知H[s…m]除了H[s] 外均满足堆的定义
        /// 调整H[s],使其成为大顶堆.即将对第s个结点为根的子树筛选
        /// </summary>
        /// <param name="s">待调整的数组元素的位置.</param>
        /// <param name="length">数组的长度.</param>
        /// <param name="comparer">Comparer.</param>
        private void HeapAdjust(int s, int length, Comparison<T> comparer)
        {
            T tmp = buffer[s];
            int child = 2 * s + 1; //左孩子结点的位置。(i+1 为当前调整结点的右孩子结点的位置)  
            while (child < length)
            {
                if (child + 1 < length && comparer(buffer[child], buffer[child + 1]) < 0)
                {
                    // 如果右孩子大于左孩子(找到比当前待调整结点大的孩子结点)  
                    ++child;
                }

                if (comparer(buffer[s], buffer[child]) < 0)
                {
                    // 如果较大的子结点大于父结点  
                    buffer[s] = buffer[child]; // 那么把较大的子结点往上移动，替换它的父结点  
                    s = child; // 重新设置s ,即待调整的下一个结点的位置  
                    child = 2 * s + 1;
                }
                else
                {
                    // 如果当前待调整结点大于它的左右孩子，则不需要调整，直接退出  
                    break;
                }

                buffer[s] = tmp; // 当前待调整的结点放到比其大的孩子结点位置上  
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("[CList: Count={0}, IsReadOnly={1}]", Count, IsReadOnly);
        }

        public static bool IsNilOrEmpty(JList<T> list)
        {
            return list == null || list.Count <= 0;
        }
    }
}