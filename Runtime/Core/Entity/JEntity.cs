namespace JECS.Core
{
    public class JEntity
    {
        internal int OwnerArchetypeUid = -1;

        /// <summary>
        /// 实体唯一id
        /// </summary>
        public int UID;

        /// <summary>
        /// 实体当前原型
        /// </summary>
        public UInt256 Archetype;

        /// <summary>
        /// 实体内部组件数组
        /// </summary>
        private readonly JComp[] _comps;

        public JEntity(int compNum)
        {
            _comps = new JComp[compNum];
        }

        /// <summary>
        /// 由当前实体获取组件
        /// </summary>
        public JComp GetC(int compId)
        {
            return _comps[compId];
        }

        /// <summary>
        /// 当前实体添加组件
        /// </summary>
        public JComp AddC(JWorld w, int compId)
        {
            // 实际添加组件时，原型管理更新
            if (__RealAddComp(w, compId)) w.ArchetypeMgr.OnEChange(this);
            return _comps[compId];
        }

        /// <summary>
        /// 实际添加组件函数，执行具体的组件生成与绑定，并返回添加成功标识符
        /// </summary>
        private bool __RealAddComp(JWorld w, int comp)
        {
            if (!Archetype.Add(comp)) return false;

            JComp newC = w.SpawnC(comp, UID);
            _comps[comp] = newC;
            w.LogMgr.AddComponent(UID, comp);
            return true;
        }

        #region 快捷方法 - 批量添加组件

        public void AddCs(JWorld w, int comp1)
        {
            bool added = false;

            added |= __RealAddComp(w, comp1);

            if (added) w.ArchetypeMgr.OnEChange(this);
        }

        public void AddCs(JWorld w, int comp1, int comp2)
        {
            bool added = false;

            added |= __RealAddComp(w, comp1);
            added |= __RealAddComp(w, comp2);

            if (added) w.ArchetypeMgr.OnEChange(this);
        }

        public void AddCs(JWorld w, int comp1, int comp2, int comp3)
        {
            bool added = false;

            added |= __RealAddComp(w, comp1);
            added |= __RealAddComp(w, comp2);
            added |= __RealAddComp(w, comp3);

            if (added) w.ArchetypeMgr.OnEChange(this);
        }

        public void AddCs(JWorld w, int comp1, int comp2, int comp3, int comp4)
        {
            bool added = false;

            added |= __RealAddComp(w, comp1);
            added |= __RealAddComp(w, comp2);
            added |= __RealAddComp(w, comp3);
            added |= __RealAddComp(w, comp4);

            if (added) w.ArchetypeMgr.OnEChange(this);
        }

        public void AddCs(JWorld w, int comp1, int comp2, int comp3, int comp4, int comp5)
        {
            bool added = false;

            added |= __RealAddComp(w, comp1);
            added |= __RealAddComp(w, comp2);
            added |= __RealAddComp(w, comp3);
            added |= __RealAddComp(w, comp4);
            added |= __RealAddComp(w, comp5);

            if (added) w.ArchetypeMgr.OnEChange(this);
        }

        public void AddCs(JWorld w, int comp1, int comp2, int comp3, int comp4, int comp5, int comp6)
        {
            bool added = false;

            added |= __RealAddComp(w, comp1);
            added |= __RealAddComp(w, comp2);
            added |= __RealAddComp(w, comp3);
            added |= __RealAddComp(w, comp4);
            added |= __RealAddComp(w, comp5);
            added |= __RealAddComp(w, comp6);

            if (added) w.ArchetypeMgr.OnEChange(this);
        }

        public void AddCs(JWorld w, int comp1, int comp2, int comp3, int comp4, int comp5, int comp6, int comp7)
        {
            bool added = false;

            added |= __RealAddComp(w, comp1);
            added |= __RealAddComp(w, comp2);
            added |= __RealAddComp(w, comp3);
            added |= __RealAddComp(w, comp4);
            added |= __RealAddComp(w, comp5);
            added |= __RealAddComp(w, comp6);
            added |= __RealAddComp(w, comp7);

            if (added) w.ArchetypeMgr.OnEChange(this);
        }

        public void AddCs(JWorld w, params int[] comps)
        {
            bool added = false;

            for (int i = 0, imax = comps.Length; i < imax; i++)
            {
                added |= __RealAddComp(w, comps[i]);
            }

            if (added) w.ArchetypeMgr.OnEChange(this);
        }

        #endregion

        /// <summary>
        /// 实体与指定类型组件脱钩，若实体不包含该组件则不发生任何行为
        /// </summary>
        public void UnlinkC(JWorld w, int compId)
        {
            if (!Archetype.Del(compId)) return;
            w.ArchetypeMgr.OnEChange(this);
        }

        /// <summary>
        /// 实体与指定类型组件重新挂钩，若实体内部不包含该组件则添加
        /// </summary>
        public void RelinkC(JWorld w, int compId)
        {
            if (!Archetype.Add(compId)) return;
            if (_comps[compId] == null) __RealAddComp(w, compId);
            w.ArchetypeMgr.OnEChange(this);
        }

        /// <summary>
        /// 删除实体指定组件，并返回成功删除标识
        /// </summary>
        public bool DelC(JWorld w, int compId)
        {
            if (!Archetype.Del(compId)) return false;

            w.ReleaseC(_comps[compId]);
            _comps[compId] = null;
            w.ArchetypeMgr.OnEChange(this);
            w.LogMgr.DelComponent(UID, compId);
            return true;
        }

        /// <summary>
        /// 实体删除时调用
        /// </summary>
        internal void OnDel(JWorld w)
        {
            // 原型内删除该实体引用
            w.ArchetypeMgr.OnEDel(this);

            // 释放组件资源
            for (int i = 0, imax = _comps.Length; i < imax; i++)
            {
                var comp = _comps[i];
                if (comp == null) continue;

                w.LogMgr.DelComponent(UID, comp.CompId);
                w.ReleaseC(comp);
                _comps[i] = null;
            }

            // 通知ECS内部log系统删除实体，并清空实体内容
            w.LogMgr.DelEntity(UID);
            OwnerArchetypeUid = -1;
            UID = -1;
            Archetype.Clear();
        }

        /// <summary>
        /// 实体内部资源释放与清空
        /// </summary>
        internal void OnClear(JWorld w)
        {
            // 清空内部id信息
            OwnerArchetypeUid = -1;
            UID = -1;

            // 清空原型
            Archetype.Clear();

            // 释放组件
            for (int i = 0, imax = _comps.Length; i < imax; i++)
            {
                var comp = _comps[i];
                if (comp == null) continue;
                
                w.ReleaseC(comp);
                _comps[i] = null;
            }
        }

#if UNITY_EDITOR
        public override string ToString()
        {
            return string.Format("[{0}: {1}]", UID, UInt256Iterator.UInt256ToString(Archetype));
        }
#endif
    }
}