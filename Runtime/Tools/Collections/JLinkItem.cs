namespace JECS
{
    /// <summary>
    /// 容器使用链接数据项
    /// </summary>
    public struct JLinkItem<TKey, TValue>
    {
        public int hash;
        public int next;

        public TKey key;
        public TValue value;

        public JLinkItem(int hash, int next, TKey key, TValue value)
        {
            this.hash = hash;
            this.next = next;
            this.key = key;
            this.value = value;
        }

#if UNITY_EDITOR
        public override string ToString()
        {
            string k = key != null ? key.ToString() : string.Empty;
            string v = value != null ? value.ToString() : string.Empty;
            return $"[CLink][hash:{hash}, next:{next}][{k},{v}]";
        }
#endif
    }
}