namespace JECS.Core
{
    public abstract class JSystem
    {
        /// <summary>
        /// 世界实例引用
        /// </summary>
#if UNITY_EDITOR
        public JWorld _W { get; private set; }
#else
        public JWorld _W;
#endif

        public UInt256 needArchetype;

        public virtual void SetWorld(JWorld w)
        {
            _W = w;
        }

        public abstract void OnInit();

        /// <summary>
        /// 获取系统默认所需组件类型所构实体UID迭代器
        /// </summary>
        /// <returns></returns>
        protected EIterator _CacheUIDIterator()
        {
            return _W.EntityIte(needArchetype);
        }

        /// <summary>
        /// 获取指定组件掩码对应实体UID迭代器
        /// </summary>
        /// <returns></returns>
        protected EIterator _CacheUIDIterator(UInt256 needArchetypes)
        {
            return _W.EntityIte(needArchetypes);
        }

        public abstract void Tick(TickParam param);

        public abstract void OnRelease();
    }
}