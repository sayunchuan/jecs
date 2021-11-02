namespace JECS.Core
{
    public abstract class JSystem
    {
        /// <summary>
        /// 世界实例引用
        /// </summary>
#if UNITY_EDITOR
        public JWorld w { get; private set; }
#else
        public JWorld w;
#endif

        internal void SetWorld(JWorld w)
        {
            this.w = w;
            OnBindWord();
        }

        protected abstract void OnBindWord();

        public abstract void OnInit();

        /// <summary>
        /// 获取指定组件掩码对应实体的迭代器
        /// </summary>
        protected EIterator _CacheUIDIterator(UInt256 needArchetypes)
        {
            return w.EntityIte(needArchetypes);
        }

        public abstract void Tick(TickParam param);

        public abstract void OnClear();
    }
}