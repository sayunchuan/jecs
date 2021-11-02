namespace JECS.Core
{
    public class ArchetypeMgr
    {
        // -------------------------- 原型实例与原型链实例的缓存池 -----------------------------

        private readonly JQueue<Archetype> _archetypeInsPool = new JQueue<Archetype>();
        private readonly JQueue<JLink<Archetype>> _linkInsPool = new JQueue<JLink<Archetype>>();

        // -------------------------- 原型数据 -----------------------------

        /// <summary>
        /// 原型核心数据
        /// </summary>
        internal readonly JArray<Archetype> Archetypes = new JArray<Archetype>();

        /// <summary>
        /// 原型核心数据的索引表
        /// </summary>
        private readonly JDictionary<UInt256, int> _archetypesTable =
            new JDictionary<UInt256, int>(UInt256Compare.Default);

        // -------------------------- 原型链数据 -----------------------------

        /// <summary>
        /// 原型链核心数据
        /// 使用过的原型，会将所有包含指定原型的原始原型数据链接并存储
        /// </summary>
        private readonly JArray<JLink<Archetype>> _cacheArchetypeLinks = new JArray<JLink<Archetype>>();

        /// <summary>
        /// 原型链核心数据对应的原型类型
        /// </summary>
        private readonly JArray<UInt256> _cacheArchetypeNote = new JArray<UInt256>();

        /// <summary>
        /// 原型链数据索引
        /// </summary>
        private readonly JDictionary<UInt256, int> _cacheArchetypeTable =
            new JDictionary<UInt256, int>(UInt256Compare.Default);

        /// <summary>
        /// 实体变化时调用，更新实体的原型归属
        /// </summary>
        internal void OnEChange(JEntity e)
        {
            // 将实体从原属原型内移除
            Archetype at = _loadArchetype(e.OwnerArchetypeUid);
            at?.RemoveEntity(e.UID);

            // 添加实体至新属原型内
            at = _loadArchetype(e.Archetype);
            at.AddEntity(e);
        }

        /// <summary>
        /// 使用原型唯一ID加载原型实例
        /// </summary>
        private Archetype _loadArchetype(int archetypeUID)
        {
            return archetypeUID < Archetypes.Count && archetypeUID >= 0 ? Archetypes[archetypeUID] : null;
        }

        /// <summary>
        /// 使用原型类型加载原型实例
        /// </summary>
        private Archetype _loadArchetype(UInt256 archetype)
        {
            int index;
            if (_archetypesTable.TryGetValue(archetype, out index))
            {
                return Archetypes[index];
            }

            return _NewArchetype(archetype);
        }

        /// <summary>
        /// 创建指定原型的实例
        /// </summary>
        private Archetype _NewArchetype(UInt256 archetype)
        {
            // 新建实例
            Archetype res = _archetypeInsPool.Count > 0 ? _archetypeInsPool.Dequeue() : new Archetype();

            // 将实例的uid与实例数组下标关联
            res.ArchetypeUid = Archetypes.Count;
            res.Type = archetype;
            Archetypes.Add(res);

            // 更新原型索引表
            _archetypesTable[archetype] = res.ArchetypeUid;

            // 刷新原型链
            for (int i = 0, imax = _cacheArchetypeNote.Count; i < imax; i++)
            {
                if (archetype.Contain(_cacheArchetypeNote[i]))
                {
                    _cacheArchetypeLinks[i].AddLast(res);
                }
            }

            // 返回新原型实例
            return res;
        }

        /// <summary>
        /// 加载指定原型类型的原型链
        /// </summary>
        internal JLink<Archetype> LoadCache(UInt256 archetype)
        {
            // 由已缓存原型链中搜索目标
            int index;
            if (_cacheArchetypeTable.TryGetValue(archetype, out index))
            {
                return _cacheArchetypeLinks[index];
            }

            // 已有数据中无目标，则新建原型链并缓存
            _cacheArchetypeTable[archetype] = _cacheArchetypeNote.Count;
            _cacheArchetypeNote.Add(archetype);
            JLink<Archetype> link = _linkInsPool.Count > 0 ? _linkInsPool.Dequeue() : new JLink<Archetype>();
            _cacheArchetypeLinks.Add(link);

            for (int i = 0, imax = Archetypes.Count; i < imax; i++)
            {
                if (Archetypes[i].Type.Contain(archetype))
                {
                    link.AddLast(Archetypes[i]);
                }
            }

            return link;
        }

        /// <summary>
        /// 实体被删除时调用，删除实体的原型归属
        /// </summary>
        internal void OnEDel(JEntity e)
        {
            Archetype at = _loadArchetype(e.OwnerArchetypeUid);
            at?.RemoveEntity(e.UID);
        }

        /// <summary>
        /// 逐原型调用脏数据处理
        /// </summary>
        internal void ExeDirty()
        {
            for (int i = 0, imax = Archetypes.Count; i < imax; i++)
            {
                Archetypes[i].ExeDirty();
            }
        }

        /// <summary>
        /// 原型管理器内部资源释放
        /// </summary>
        internal void Clear()
        {
            // 原型核心数据重置并使实例入池
            for (int i = 0, imax = Archetypes.Count; i < imax; i++)
            {
                Archetype a = Archetypes[i];
                if (a == null) continue;

                a.Clear();
                _archetypeInsPool.Enqueue(a);
            }

            // 清空原型数据引用与查询表
            Archetypes.Clear();
            _archetypesTable.Clear();

            // 原型链实例入池
            for (int i = 0, imax = _cacheArchetypeLinks.Count; i < imax; i++)
            {
                JLink<Archetype> link = _cacheArchetypeLinks[i];
                link.Clear();
                _linkInsPool.Enqueue(link);
            }

            // 清空原型链、原型链对应数据、原型链查询表
            _cacheArchetypeLinks.Clear();
            _cacheArchetypeNote.Clear();
            _cacheArchetypeTable.Clear();
        }
    }
}