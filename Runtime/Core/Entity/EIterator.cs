namespace JECS.Core
{
    public struct EIterator
    {
        private JLinkNode<Archetype> __archetypeNode;
        private int __entitysIndex;
        private JEntity __current;

        public EIterator(JWorld w, UInt256 archetype)
        {
            __archetypeNode = w.ArchetypeMgr.LoadCache(archetype).First;
            __entitysIndex = -1;
            __current = null;
        }

        // public JEntity Current => __archetypeNode.Value.EntityRefs[__entitysIndex];
        public JEntity Current => __current;

        public bool MoveNext()
        {
            while (__archetypeNode != null)
            {
                JArray<JEntity> entityRefs = __archetypeNode.Value.EntityRefs;
                while (++__entitysIndex < entityRefs.Count)
                {
                    __current = entityRefs[__entitysIndex];
                    if (__current != null)
                        return true;
                }

                __archetypeNode = __archetypeNode.Next;
                __entitysIndex = -1;
            }

            return false;
        }
    }
}