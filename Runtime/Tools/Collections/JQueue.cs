using System;
using System.Collections.Generic;

namespace JECS
{
    public class JQueue<T>
    {
        private int _size;
        private int _head;
        private int _tail;
        private T[] _array;

        public int Count
        {
            get { return _size; }
        }

        public JQueue()
        {
            _size = 0;
            _array = new T[_size];
            _head = 0;
            _tail = 0;
        }

        public JQueue(IEnumerable<T> collection) : this()
        {
            using (IEnumerator<T> en = collection.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    Enqueue(en.Current);
                }
            }
        }

        public void Clear()
        {
            Array.Clear(_array, 0, _array.Length);
            _size = 0;
            _head = 0;
            _tail = 0;
        }

        public T Dequeue()
        {
            if (_size <= 0)
            {
                return default;
            }

            T removed = _array[_head];
            _array[_head] = default;
            _head = (_head + 1) % _array.Length;
            _size--;
            return removed;
        }

        public void Enqueue(T item)
        {
            if (_size == _array.Length)
            {
                __AllocateMore();
            }

            _array[_tail] = item;
            _tail = (_tail + 1) % _array.Length;
            _size++;
        }

        public T Peek()
        {
            if (_size <= 0)
            {
                return default;
            }

            return _array[_head];
        }

        private void __AllocateMore()
        {
            T[] newarray = new T[Math.Max((_array.Length << 1) + 1, 3)];
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, newarray, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, newarray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, newarray, _array.Length - _head, _tail);
                }
            }

            _array = newarray;
            _head = 0;
            _tail = _size % _array.Length;
        }
    }
}