namespace JECS.Core
{
    public class Archetype
    {
        /// <summary>
        /// 脏数据标记位，实体引用列表发生变化时设置
        /// </summary>
        private bool __dirty;

        /// <summary>
        /// 原型唯一ID
        /// </summary>
        internal int ArchetypeUID;

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
            Reset();
        }

        public void Reset()
        {
            __dirty = false;
            ArchetypeUID = -1;
            Type.Clear();
            EntityRefs.Clear();
        }

        /// <summary>
        /// 脏数据处理函数
        /// </summary>
        internal void ExeDirty()
        {
            if (!__dirty)
            {
                return;
            }

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

            __dirty = false;
        }

        /// <summary>
        /// 添加实体
        /// 频繁操作方法，实体组件发生变更时被调用
        /// </summary>
        internal void AddEntity(JEntity e)
        {
            EntityRefs.Add(e);
            e.OwnerArchetypeUID = ArchetypeUID;
            __dirty = true;
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
                __dirty = true;
                return;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, Count:{1}", UInt256Iterator.UInt256ToString(Type), EntityRefs.Count);
        }
    }
}