using System;
using System.Collections;
using System.Collections.Generic;

namespace JECS
{
    public class JHashSet_Int32 : IEnumerable<int>
    {
        private const double DEFAULT_LOAD_FACTOR = 0.75;

        /// <summary>
        /// 存储_link的id，为了性能，由1开始
        /// </summary>
        private int[] _buckets;

        private JLinkItem<int, int>[] _link;

        private int _num;
        private int _count;
        private int _freeLink;

        private int _operation;

        public int Count => _num;

        public JHashSet_Int32()
        {
            _Init();
        }

        private void _Init()
        {
            _num = 0;
            _count = 0;
            _freeLink = -1;

            int num = _Double(_count);
            _buckets = new int[num];
            _link = new JLinkItem<int, int>[num];

            _operation = 0;
        }

        private void _Resize()
        {
            int oldNum = _buckets.Length;
            int num = _Double(oldNum);
            int[] tmpBuckets = new int[num];
            JLinkItem<int, int> tmpLinkItem;
            for (int i = 0, imax = _buckets.Length; i < imax; i++)
            {
                int next;
                for (int j = _buckets[i] - 1; j >= 0; j = next)
                {
                    tmpLinkItem = _link[j];
                    next = tmpLinkItem.next;
                    int newBucketIndex = (tmpLinkItem.hash & 0x7fffffff) % num;
                    _link[j].next = tmpBuckets[newBucketIndex] - 1;
                    tmpBuckets[newBucketIndex] = j + 1;
                }
            }

            _buckets = tmpBuckets;

            JLinkItem<int, int>[] _link_new = new JLinkItem<int, int>[num];
            Array.Copy(_link, 0, _link_new, 0, oldNum);
            _link = _link_new;
        }

        public bool Contains(int item)
        {
            int hash = item & 0x7fffffff;
            int index = hash % _buckets.Length;
            JLinkItem<int, int> linkItem;
            for (int i = _buckets[index] - 1; i != -1; i = linkItem.next)
            {
                linkItem = _link[i];
                if (linkItem.hash == hash && linkItem.key == item)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Add(int item)
        {
            int hash = item & 0x7fffffff;
            int index = hash % _buckets.Length;
            JLinkItem<int, int> linkItem;
            for (int i = _buckets[index] - 1; i >= 0; i = linkItem.next)
            {
                linkItem = _link[i];
                if (linkItem.hash == hash && linkItem.key == item)
                {
                    return false;
                }
            }

            if (_num + 1 >= _buckets.Length * DEFAULT_LOAD_FACTOR)
            {
                _Resize();
                index = hash % _buckets.Length;
            }

            _num++;
            int dataIndex;
            if (_freeLink < 0)
            {
                dataIndex = _count++;
            }
            else
            {
                dataIndex = _freeLink;
                _freeLink = _link[_freeLink].next;
            }

            _link[dataIndex] = new JLinkItem<int, int>(hash, _buckets[index] - 1, item, 0);
            _buckets[index] = dataIndex + 1;
            _operation++;
            return true;
        }

        public bool Remove(int item)
        {
            int hash = item & 0x7fffffff;
            int index = hash % _buckets.Length;
            int wantId = _buckets[index] - 1;
            if (wantId < 0)
            {
                return false;
            }

            int beforeId = -1;
            do
            {
                JLinkItem<int, int> linkItem = _link[wantId];
                if (linkItem.hash == hash && linkItem.key == item)
                {
                    break;
                }

                beforeId = wantId;
                wantId = linkItem.next;
            } while (wantId >= 0);

            if (wantId < 0)
            {
                return false;
            }

            _num--;
            if (beforeId < 0)
            {
                _buckets[index] = _link[wantId].next + 1;
            }
            else
            {
                _link[beforeId].next = _link[wantId].next;
            }

            _link[wantId] = new JLinkItem<int, int>(0, _freeLink, 0, 0);
            _freeLink = wantId;
            _operation++;
            return true;
        }

        public void Clear()
        {
            _num = 0;
            _count = 0;
            _freeLink = -1;

            Array.Clear(_buckets, 0, _buckets.Length);
            Array.Clear(_link, 0, _link.Length);

            _operation++;
        }

        private int _Double(int wantNum)
        {
            return wantNum < 7 ? 7 : HashHelpers.ExpandPrime(wantNum);
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (int i = 0, stamp = _operation; i < _count; i++)
            {
                if (_operation != stamp)
                {
                    throw new InvalidOperationException("HashSet have been modified while it was iterated over");
                }

                if (_link[i].hash != 0)
                {
                    yield return _link[i].key;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0, stamp = _operation; i < _count; i++)
            {
                if (_operation != stamp)
                {
                    throw new InvalidOperationException("HashSet have been modified while it was iterated over");
                }

                if (_link[i].hash != 0)
                {
                    yield return _link[i].key;
                }
            }
        }

        public Iterator GetIterator()
        {
            return new Iterator(this);
        }

        public override string ToString()
        {
            return string.Format("[CHashSet: Count={0}]", Count);
        }

        public struct Iterator
        {
            private JHashSet_Int32 _hash;
            private int _index;

            public Iterator(JHashSet_Int32 v)
            {
                _hash = v;
                _index = -1;
            }

            public bool MoveNext()
            {
                do
                {
                    _index++;
                } while (_index < _hash._count && _hash._link[_index].hash == 0);

                return _index < _hash._count;
            }

            public int Current => _hash._link[_index].key;
        }
    }
}