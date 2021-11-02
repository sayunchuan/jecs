using System;
using System.Collections.Generic;

namespace JECS
{
    public class JDictionary<TKey, TValue>
    {
        private const double DEFAULT_LOAD_FACTOR = 0.75;

        /// <summary>
        /// 数据桶，存储hash值相对数据桶大小的模运算值相同的链接表首节点位置。存储值为_link的id，为了性能，id由1开始
        /// </summary>
        private int[] _buckets = new int[7];

        /// <summary>
        /// hash连接列表，呈链接状态的节点的hash值相对数据桶的模运算值相同
        /// </summary>
        private JLinkItem<TKey, TValue>[] _link = new JLinkItem<TKey, TValue>[7];

        /// <summary>
        /// 数据的实际数量
        /// </summary>
        private int _num;

        /// <summary>
        /// 回收节点的坐标
        /// </summary>
        private int _freeLink;

        /// <summary>
        /// 数组有效位置的游标
        /// </summary>
        private int _count;

        private IEqualityComparer<TKey> _keyCompare;
        private IEqualityComparer<TValue> _valueCompare;

        public int Count => _num;

        public TValue this[TKey key]
        {
            get
            {
                int wantId = _FindEntry(key);
                if (wantId >= 0)
                {
                    return _link[wantId].value;
                }

                throw new KeyNotFoundException();
            }
            set
            {
                int hash = _GetHash(key);
                int index = (hash & 0x7fffffff) % _buckets.Length;
                int wantId = _buckets[index] - 1;
                int listLastPtr = -1;
                if (wantId >= 0)
                {
                    JLinkItem<TKey, TValue> linkItem = _link[wantId];
                    while (linkItem.hash != hash || !_keyCompare.Equals(linkItem.key, key))
                    {
                        listLastPtr = wantId;
                        wantId = linkItem.next;
                        if (wantId == -1)
                        {
                            break;
                        }

                        linkItem = _link[wantId];
                    }
                }

                if (wantId < 0)
                {
                    // wantId<0，说明未存储该key的数据，所以进行完整赋值，同时对其next节点进行赋值（将可能的碰撞数据进行链接）
                    if (_num + 1 >= _buckets.Length * DEFAULT_LOAD_FACTOR)
                    {
                        _Resize();
                        index = (hash & 0x7fffffff) % _buckets.Length;
                    }

                    _num++;
                    if (_freeLink < 0)
                    {
                        wantId = _count++;
                    }
                    else
                    {
                        wantId = _freeLink;
                        _freeLink = _link[_freeLink].next;
                    }

                    _link[wantId] = new JLinkItem<TKey, TValue>(hash, _buckets[index] - 1, key, value);
                    _buckets[index] = wantId + 1;
                }
                else if (listLastPtr != -1)
                {
                    // listLastPtr指向链表want数据前，将want数据前移至链表头
                    _link[listLastPtr].next = _link[wantId].next;
                    _link[wantId].next = _buckets[index] - 1;
                    _buckets[index] = wantId + 1;
                    _link[wantId].value = value;
                }
                else
                {
                    // wantId不小于0，说明已存储该key的数据，只进行值修改即可
                    _link[wantId].value = value;
                }
            }
        }

        public JDictionary()
        {
            _Init(null);
        }

        public JDictionary(IEqualityComparer<TKey> keyCompare)
        {
            _Init(keyCompare);
        }

        private void _Init(IEqualityComparer<TKey> keyCompare)
        {
            _keyCompare = keyCompare == null ? EqualityComparer<TKey>.Default : keyCompare;
            _valueCompare = EqualityComparer<TValue>.Default;

            Clear();
        }

        private int _GetHash(TKey key)
        {
            return _keyCompare.GetHashCode(key) | -2147483648;
        }

        private void _Resize()
        {
            int oldNum = _buckets.Length;
            int num = _Double(oldNum);
            int[] tmpBuckets = new int[num];
            JLinkItem<TKey, TValue> tmpLinkItem;
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

            JLinkItem<TKey, TValue>[] _link_new = new JLinkItem<TKey, TValue>[num];
            Array.Copy(_link, 0, _link_new, 0, oldNum);
            _link = _link_new;
        }

        #region Methods Add

        public void Add(TKey key, TValue value)
        {
            int hash = _GetHash(key);
            int index = (hash & 0x7fffffff) % _buckets.Length;
            JLinkItem<TKey, TValue> linkItem;
            for (int i = _buckets[index] - 1; i >= 0; i = linkItem.next)
            {
                linkItem = _link[i];
                if (linkItem.hash == hash && _keyCompare.Equals(linkItem.key, key))
                {
                    throw new ArgumentException("An element with the same key already exists in the dictionary.");
                }
            }

            if (_num + 1 >= _buckets.Length * DEFAULT_LOAD_FACTOR)
            {
                _Resize();
                index = (hash & 0x7fffffff) % _buckets.Length;
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

            _link[dataIndex] = new JLinkItem<TKey, TValue>(hash, _buckets[index] - 1, key, value);
            _buckets[index] = dataIndex + 1;
        }

        #endregion

        #region Methods ContainsKey

        public bool ContainsKey(TKey key)
        {
            return _FindEntry(key) >= 0;
        }

        private int _FindEntry(TKey key)
        {
            int hash = _GetHash(key);
            int index = (hash & 0x7fffffff) % _buckets.Length;
            int wantId = _buckets[index] - 1;
            while (wantId >= 0)
            {
                JLinkItem<TKey, TValue> linkItem = _link[wantId];
                if (linkItem.hash == hash && _keyCompare.Equals(linkItem.key, key))
                {
                    return wantId;
                }

                wantId = linkItem.next;
            }

            return -1;
        }

        public bool ContainsValue(TValue value)
        {
            JLinkItem<TKey, TValue> linkItem;
            for (int i = 0, imax = _buckets.Length; i < imax; i++)
            {
                for (int index = _buckets[i] - 1; index >= 0; index = linkItem.next)
                {
                    linkItem = _link[index];
                    if (_valueCompare.Equals(value, linkItem.value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Methods Remove

        public bool Remove(TKey key)
        {
            int hash = _GetHash(key);
            int index = (hash & 0x7fffffff) % _buckets.Length;
            int wantId = _buckets[index] - 1;
            if (wantId < 0)
            {
                return false;
            }

            int beforeId = -1;
            do
            {
                JLinkItem<TKey, TValue> linkItem = _link[wantId];
                if (linkItem.hash == hash && _keyCompare.Equals(linkItem.key, key))
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

            _link[wantId] = new JLinkItem<TKey, TValue>(0, _freeLink, default, default);
            _freeLink = wantId;
            return true;
        }

        #endregion

        public bool TryGetValue(TKey key, out TValue value)
        {
            int entry = _FindEntry(key);
            if (entry >= 0)
            {
                value = _link[entry].value;
                return true;
            }

            value = default;
            return false;
        }

        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(_buckets, 0, _buckets.Length);
                Array.Clear(_link, 0, _link.Length);
            }

            _num = 0;
            _count = 0;
            _freeLink = -1;
        }

        private int _Double(int wantNum)
        {
            if (wantNum < 7)
            {
                return 7;
            }

            return HashHelpers.ExpandPrime(wantNum);
        }

        public void CopyTo(JDictionary<TKey, TValue> other)
        {
            other.Clear();

            other._num = _num;
            other._count = _count;
            other._freeLink = _freeLink;

            other._keyCompare = _keyCompare;
            other._valueCompare = _valueCompare;

            int num = _buckets.Length;
            other._buckets = new int[num];
            Array.Copy(_buckets, 0, other._buckets, 0, num);
            other._link = new JLinkItem<TKey, TValue>[num];
            Array.Copy(_link, 0, other._link, 0, num);
        }
    }
}