namespace JECS.Core
{
    public class Archetype
    {
        /// <summary>
        /// 脏数据标记位，实体引用列表发生变化时设置
        /// </summary>
        private bool _dirty;

        /// <summary>
        /// 原型唯一ID
        /// </summary>
        internal int ArchetypeUid;

        /// <summary>
        /// 原型bit数据
        /// </summary>
        public UInt256 Type;

        /// <summary>
        /// 原型对应实体数组
        /// </summary>
        public JArray<JEntity> EntityRefs = new JArray<JEntity>();

        internal Archetype()
        {
            Clear();
        }

        /// <summary>
        /// 重置原型数据，清空内部的实体引用列表，并清除原型类型
        /// </summary>
        internal void Clear()
        {
            _dirty = false;
            ArchetypeUid = -1;
            Type.Clear();
            EntityRefs.Clear();
        }

        /// <summary>
        /// 脏数据处理函数
        /// </summary>
        internal void ExeDirty()
        {
            // 无脏则略过
            if (!_dirty) return;

            // 对实体引用列表倒序遍历，移除空节点，此处需注意Remove传入参数的正确性
            int end = -1;
            for (int i = EntityRefs.Count - 1; i >= 0; i--)
            {
                if (EntityRefs[i] != null)
                {
                    if (end < 0) continue;

                    EntityRefs.Remove(i + 1, end);
                    end = -1;
                }
                else
                {
                    if (end < 0)
                    {
                        end = i + 1;
                    }
                }
            }

            if (end >= 0)
            {
                EntityRefs.Remove(0, end);
            }

            _dirty = false;
        }

        /// <summary>
        /// 添加实体
        /// 频繁操作方法，实体组件发生变更时被调用
        /// </summary>
        internal void AddEntity(JEntity e)
        {
            EntityRefs.Add(e);
            e.OwnerArchetypeUid = ArchetypeUid;
            _dirty = true;
        }

        /// <summary>
        /// 移除指定uid实体
        /// 频繁操作方法，实体组件发生变更时被调用
        /// </summary>
        internal void RemoveEntity(int uid)
        {
            for (int i = 0, imax = EntityRefs.Count; i < imax; i++)
            {
                JEntity e = EntityRefs[i];
                if (e == null || e.UID != uid) continue;

                EntityRefs[i] = null;
                _dirty = true;
                return;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, Count:{1}", UInt256Iterator.UInt256ToString(Type), EntityRefs.Count);
        }
    }
}