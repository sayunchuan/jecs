namespace JECS.Core
{
    /// <summary>
    /// 实体迭代器
    /// </summary>
    public struct EIterator
    {
        /// <summary>
        /// 实体迭代器所用原型链数据
        /// </summary>
        private JLinkNode<Archetype> _archetypeLinkNode;

        /// <summary>
        /// 迭代数据下标id
        /// </summary>
        private int _entitysIndex;

        /// <summary>
        /// 迭代器当前指向实体
        /// </summary>
        private JEntity _current;

        public EIterator(JWorld w, UInt256 archetype)
        {
            _archetypeLinkNode = w.ArchetypeMgr.LoadCache(archetype).First;
            _entitysIndex = -1;
            _current = null;
        }

        public JEntity Current => _current;

        public bool MoveNext()
        {
            while (_archetypeLinkNode != null)
            {
                JArray<JEntity> entityRefs = _archetypeLinkNode.Value.EntityRefs;
                while (++_entitysIndex < entityRefs.Count)
                {
                    _current = entityRefs[_entitysIndex];
                    if (_current != null)
                        return true;
                }

                _archetypeLinkNode = _archetypeLinkNode.Next;
                _entitysIndex = -1;
            }

            return false;
        }
    }
}