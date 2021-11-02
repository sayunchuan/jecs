using System;

namespace JECS
{
    public class JArray<T>
    {
        private T[] _array;
        private int _count;

        public int Count => _count;

        public T this[int index]
        {
            get { return _array[index]; }
            set { _array[index] = value; }
        }

        public JArray()
        {
            _array = new T[8];
            _count = 0;
        }

        private void _AllocMore(int wannaL)
        {
            if (wannaL < _array.Length)
                return;

            int newL = _array.Length;
            while (newL < wannaL)
            {
                newL <<= 1;
            }

            T[] tmp = new T[newL];
            Array.Copy(_array, 0, tmp, 0, _array.Length);
            _array = tmp;
        }

        public void Add(T item)
        {
            _AllocMore(_count + 1);
            _array[_count++] = item;
        }

        public void Clear()
        {
            _count = 0;
            for (int i = 0, imax = _array.Length; i < imax; i++)
            {
                _array[i] = default(T);
            }
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
            {
                return;
            }

            for (int i = index, imax = _count - 1; i < imax; i++)
            {
                _array[i] = _array[i + 1];
            }

            _array[_count--] = default(T);
        }

        /// <summary>
        /// 移除从begin到end的数据，end不包含在移除数据内
        /// </summary>
        public void Remove(int begin, int end)
        {
            if (begin < 0) begin = 0;
            if (end > _count) end = _count;
            int offset = end - begin;

            for (int i = end, imax = _count; i < imax; i++)
            {
                _array[i - offset] = _array[i];
            }

            for (int i = _count - offset, imax = _count; i < imax; i++)
            {
                _array[i] = default;
            }

            _count -= offset;
        }
    }
}