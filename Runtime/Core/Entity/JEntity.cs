namespace JECS.Core
{
    public class JEntity
    {
        internal int OwnerArchetypeUID = -1;

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
        private JComp[] __comps;

        public JEntity(int compNum)
        {
            __comps = new JComp[compNum];
        }

        public JComp GetC(int compId)
        {
            return __comps[compId];
        }

        public JComp AddC(JWorld w, int compId)
        {
            // 实际添加组件时，原型管理更新
            if (__RealAddComp(w, compId)) w.ArchetypeMgr.OnEChange(this);
            return __comps[compId];
        }

        #region Add Components

        private bool __RealAddComp(JWorld w, int comp)
        {
            if (!Archetype.Add(comp)) return false;

            JComp newC = w.SpawnC(comp, UID);
            __comps[comp] = newC;
            w.logMgr.AddComponent(UID, comp);
            return true;
        }

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

        public void RelinkC(JWorld w, int compId)
        {
            if (!Archetype.Add(compId)) return;
            w.ArchetypeMgr.OnEChange(this);
        }

        public void UnlinkC(JWorld w, int compId)
        {
            if (!Archetype.Del(compId)) return;
            w.ArchetypeMgr.OnEChange(this);
        }

        public bool DelC(JWorld w, int compId)
        {
            if (!Archetype.Del(compId)) return false;

            w.ReleaseC(__comps[compId]);
            __comps[compId] = null;
            w.ArchetypeMgr.OnEChange(this);
            w.logMgr.DelComponent(UID, compId);
            return true;
        }

        public void OnDel(JWorld w)
        {
            w.ArchetypeMgr.OnEDel(this);

            for (int i = 0, imax = __comps.Length; i < imax; i++)
            {
                var comp = __comps[i];
                if (comp == null)
                    continue;

                w.logMgr.DelComponent(UID, comp.CompId);
                w.ReleaseC(comp);
                __comps[i] = null;
            }

            OwnerArchetypeUID = -1;
            UID = -1;
            Archetype.Clear();
        }

        /// <summary>
        /// 实体内部资源释放
        /// </summary>
        internal void OnRelease(JWorld w)
        {
            // 清空内部id信息
            OwnerArchetypeUID = -1;
            UID = -1;

            // 清空原型
            Archetype.Clear();

            // 释放组件
            for (int i = 0, imax = __comps.Length; i < imax; i++)
            {
                var comp = __comps[i];
                if (comp == null) continue;
                w.ReleaseC(comp);
                __comps[i] = null;
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