using System;
using System.Collections.Generic;

namespace JECS
{
    /// <summary>
    /// 块数组，将元素按块存储，每块是一个独立的数组，当块内数组元素均为空时则释放该块的内存
    /// 该结构仅用于数据递增的情况下，即数据的添加无法指定位置
    /// </summary>
    public class JBlockArray<T> where T : class
    {
        private const int BlockSizeMask = 0x1f;
        private const int BlockSizeMod = 5;
        private const int BlockSize = 32;

        private Queue<JBlock<T>> _blockPool = new Queue<JBlock<T>>();
        private JBlock<T>[] _array;
        private int _count;
        public int Count => _count;

        public T this[int index]
        {
            get
            {
                JBlock<T> block = _array[index >> BlockSizeMod];
                return block == null ? null : block.array[index & BlockSizeMask];
            }
        }

        public JBlockArray()
        {
            _array = new JBlock<T>[8];
            _count = 0;
        }

        private void _AllocMore()
        {
            int maxIndex = _count >> BlockSizeMod;
            if (maxIndex < _array.Length)
                return;

            int newL = _array.Length;
            while (newL <= maxIndex)
            {
                newL <<= 1;
            }

            JBlock<T>[] tmp = new JBlock<T>[newL];
            Array.Copy(_array, 0, tmp, 0, _array.Length);
            _array = tmp;
        }

        public void Add(T item)
        {
            _AllocMore();
            int blockIndex = _count >> BlockSizeMod;
            JBlock<T> block = _array[blockIndex];
            if (block == null)
            {
                block = _blockPool.Count > 0 ? _blockPool.Dequeue() : new JBlock<T>();
                _array[blockIndex] = block;
#if UNITY_EDITOR
                block.offset = blockIndex * BlockSize;
#endif
            }

            block.Add(_count & BlockSizeMask, item);
            _count++;
        }

        public void Clear()
        {
            _count = 0;
            for (int i = 0, imax = _array.Length; i < imax; i++)
            {
                var tmp = _array[i];
                if (tmp != null)
                {
                    _blockPool.Enqueue(tmp);
                    tmp.Clear();
                }

                _array[i] = null;
            }
        }

        public void Release(int index)
        {
            var block = _array[index >> BlockSizeMod];
            block.Del(index & BlockSizeMask);
            if (block.Empty())
            {
                _blockPool.Enqueue(block);
                _array[index >> BlockSizeMod] = null;
            }
        }

        public override string ToString()
        {
            return $"Count:{_count}";
        }

        /// <summary>
        /// 一个由指定大小数组组成的数据块，为便于查看而在Editor宏下开放该块的数据偏移
        /// </summary>
        private class JBlock<T> where T : class
        {
#if UNITY_EDITOR
            public int offset;
#endif
            private int _count;
            private bool _fulled;
            public T[] array = new T[BlockSize];

            public JBlock()
            {
                _count = 0;
                _fulled = false;
            }

            public void Add(int index, T item)
            {
                array[index] = item;
                _count++;
                _fulled = (index == (BlockSize - 1));
            }

            public void Del(int index)
            {
                array[index] = null;
                _count--;
            }

            public bool Empty()
            {
                // 当该数据块已经被填充满，且当前数据量为0，则表示该数据块已被清空
                return _fulled && _count <= 0;
            }

            public void Clear()
            {
                _count = 0;
                _fulled = false;
                for (int i = 0; i < BlockSize; i++) array[i] = null;
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                return $"[{offset}..{offset + BlockSize - 1}]";
            }
#endif
        }
    }
}