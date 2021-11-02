using System.Collections.Generic;

namespace JECS.Core
{
    public class ArchetypeMgr
    {
        private Queue<Archetype> __archetypeInsPool = new Queue<Archetype>();
        private Queue<JLink<Archetype>> __linkInsPool = new Queue<JLink<Archetype>>();

        /// <summary>
        /// 原型核心数据
        /// </summary>
        internal JArray<Archetype> archetypes = new JArray<Archetype>();

        /// <summary>
        /// 原型核心数据的索引表
        /// </summary>
        private JDictionary<UInt256, int> __archetypesTable = new JDictionary<UInt256, int>(UInt256Compare.Default);

        /// <summary>
        /// 原型链核心数据
        /// 使用过的原型，会将所有包含指定原型的原始原型数据链接并存储
        /// </summary>
        private JArray<JLink<Archetype>> __cacheArchetypeLinks = new JArray<JLink<Archetype>>();

        /// <summary>
        /// 原型链核心数据对应的原型类型
        /// </summary>
        private JArray<UInt256> __cacheArchetypeNote = new JArray<UInt256>();

        /// <summary>
        /// 原型链数据索引
        /// </summary>
        private JDictionary<UInt256, int> __cacheArchetypeTable = new JDictionary<UInt256, int>(UInt256Compare.Default);

        internal void OnEChange(JEntity e)
        {
            Archetype at = __loadArchetype(e.OwnerArchetypeUID);
            if (at != null)
            {
                at.RemoveEntity(e.UID);
            }

            at = __loadArchetype(e.Archetype);
            at.AddEntity(e);
        }

        private Archetype __loadArchetype(int archetypeUID)
        {
            return archetypeUID < archetypes.Count && archetypeUID >= 0 ? archetypes[archetypeUID] : null;
        }

        private Archetype __loadArchetype(UInt256 archetype)
        {
            int index;
            if (__archetypesTable.TryGetValue(archetype, out index))
            {
                return archetypes[index];
            }

            return __NewArchetype(archetype);
        }

        private Archetype __NewArchetype(UInt256 archetype)
        {
            Archetype res = __archetypeInsPool.Count > 0 ? __archetypeInsPool.Dequeue() : new Archetype();
            res.ArchetypeUID = archetypes.Count;
            res.Type = archetype;
            archetypes.Add(res);
            __archetypesTable[archetype] = res.ArchetypeUID;

            for (int i = 0, imax = __cacheArchetypeNote.Count; i < imax; i++)
            {
                if (archetype.Contain(__cacheArchetypeNote[i]))
                {
                    __cacheArchetypeLinks[i].AddLast(res);
                }
            }

            return res;
        }

        internal JLink<Archetype> LoadCache(UInt256 archetype)
        {
            int index;
            if (__cacheArchetypeTable.TryGetValue(archetype, out index))
            {
                return __cacheArchetypeLinks[index];
            }

            __cacheArchetypeTable[archetype] = __cacheArchetypeNote.Count;
            __cacheArchetypeNote.Add(archetype);
            JLink<Archetype> link = __linkInsPool.Count > 0 ? __linkInsPool.Dequeue() : new JLink<Archetype>();
            __cacheArchetypeLinks.Add(link);

            for (int i = 0, imax = archetypes.Count; i < imax; i++)
            {
                if (archetypes[i].Type.Contain(archetype))
                {
                    link.AddLast(archetypes[i]);
                }
            }

            return link;
        }

        internal void OnEDel(JEntity e)
        {
            Archetype at = __loadArchetype(e.OwnerArchetypeUID);
            if (at != null)
            {
                at.RemoveEntity(e.UID);
            }
        }

        /// <summary>
        /// 逐原型调用脏数据处理
        /// </summary>
        internal void ExeDirty()
        {
            for (int i = 0, imax = archetypes.Count; i < imax; i++)
            {
                archetypes[i].ExeDirty();
            }
        }

        // 原型管理器内部资源释放
        public void Release()
        {
            // 原型核心数据重置并入池
            for (int i = 0, imax = archetypes.Count; i < imax; i++)
            {
                Archetype a = archetypes[i];
                if (a == null) continue;
                a.Reset();
                __archetypeInsPool.Enqueue(a);
            }

            // 清空原型数据引用与查询表
            archetypes.Clear();
            __archetypesTable.Clear();

            // 原型链实例入池
            for (int i = 0, imax = __cacheArchetypeLinks.Count; i < imax; i++)
            {
                JLink<Archetype> link = __cacheArchetypeLinks[i];
                link.Clear();
                __linkInsPool.Enqueue(link);
            }

            // 清空原型链、原型链对应数据、原型链查询表
            __cacheArchetypeLinks.Clear();
            __cacheArchetypeNote.Clear();
            __cacheArchetypeTable.Clear();
        }
    }
}