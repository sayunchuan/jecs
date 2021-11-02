using System;
using System.Collections.Generic;

namespace JECS.Core
{
    public abstract class JWorld
    {
        /// <summary>
        /// 实体实例池
        /// </summary>
        private Queue<JEntity> __entityPool = new Queue<JEntity>();

        /// <summary>
        /// 实体列表
        /// </summary>
        private JBlockArray<JEntity> __entitys = new JBlockArray<JEntity>();

        /// <summary>
        /// 实体删除锁定集合，用于锁定实体的删除操作仅执行一次
        /// </summary>
        private JHashSet_Int32 __entityDeleteLock = new JHashSet_Int32();

        /// <summary>
        /// 组件实例池
        /// </summary>
        private JCompPool[] __compCachePools = new JCompPool[8];

        /// <summary>
        /// 系统队列，依序执行
        /// </summary>
        private List<JSystem> __systems = new List<JSystem>();

        /// <summary>
        /// 该系统下的组件数量，用于生成指定数量的组件数组与组件池数组
        /// </summary>
        public abstract int CompNum { get; }

        /// <summary>
        /// 原型管理
        /// </summary>
        public ArchetypeMgr ArchetypeMgr = new ArchetypeMgr();

        /// <summary>
        /// 日志管理
        /// </summary>
        public JLogMgr logMgr = new JLogMgr();

        /// <summary>
        /// 世界时间
        /// </summary>
        public Time Time = new Time();

        public JWorld()
        {
            __compCachePools = new JCompPool[CompNum];
        }

        /// <summary>
        /// 基础初始化，运行前执行
        /// </summary>
        public virtual void InitBase()
        {
            // 初始化所有系统，供系统调用新数据进行运行配置
            for (int i = 0, imax = __systems.Count; i < imax; i++)
            {
                var s = __systems[i];
                s.OnInit();
            }
        }

        /// <summary>
        /// 释放资源，将世界恢复到初始的样子，但是对各缓存实例则保留
        /// </summary>
        public virtual void Release()
        {
            // recover entity and component
            for (int i = 0, imax = __entitys.Count; i < imax; i++)
            {
                var e = __entitys[i];
                if (e == null) continue;
                ReleaseE(e);
            }

            __entitys.Clear();
            __entityDeleteLock.Clear();

            // recover system
            for (int i = 0, imax = __systems.Count; i < imax; i++)
            {
                var s = __systems[i];
                s.OnRelease();
            }

            // reset archetype manager
            ArchetypeMgr.Release();

            // reset JLogMgr
            logMgr.Release();

            // 重置时间
            Time.Release();
        }

        #region protected Methods

        protected void _AddCompPool(int compId, JCompPool pool)
        {
            pool.CompId = compId;
            if (compId >= __compCachePools.Length)
            {
                int wantLen = __compCachePools.Length;
                while (wantLen <= compId)
                {
                    wantLen <<= 1;
                }

                JCompPool[] tmp = new JCompPool[wantLen];
                Array.Copy(__compCachePools, 0, tmp, 0, __compCachePools.Length);
                __compCachePools = tmp;
            }

            __compCachePools[compId] = pool;
        }

        protected void _AddSystem(JSystem sys)
        {
            __systems.Add(sys);
            sys.SetWorld(this);
        }

        #endregion

        #region Internal Methods

        internal JComp SpawnC(int compId, int uid)
        {
            return __compCachePools[compId].Spawn(this, uid);
        }

        /// <summary>
        /// 释放组件资源，按照组件类型入池
        /// </summary>
        internal void ReleaseC(JComp comp)
        {
            __compCachePools[comp.CompId].Release(this, comp);
        }

        /// <summary>
        /// 释放实体资源，并将实体实例入池
        /// </summary>
        internal void ReleaseE(JEntity entity)
        {
            int uid = entity.UID;
            entity.OnRelease(this);
            __entitys.Release(uid);
            __entityPool.Enqueue(entity);
        }

        #endregion

        #region Public Methods

        public JEntity NewE()
        {
            JEntity res = __entityPool.Count > 0 ? __entityPool.Dequeue() : new JEntity(CompNum);
            res.UID = __entitys.Count;
            __entitys.Add(res);
            logMgr.AddEntity(res.UID);
            return res;
        }

        public JEntity GetE(int uid)
        {
            return uid >= 0 && uid < __entitys.Count ? __entitys[uid] : null;
        }

        public virtual void DelE(int uid)
        {
            JEntity e = __entitys[uid];
            if (e == null) return;

            // 删除实体已被锁定，则跳过删除操作
            if (__entityDeleteLock.Contains(uid)) return;

            // 锁定实体
            __entityDeleteLock.Add(uid);

            // 删除实体核心操作
            e.OnDel(this);
            __entitys.Release(uid);
            __entityPool.Enqueue(e);

            // 解锁实体
            __entityDeleteLock.Remove(uid);
        }

        public virtual bool DelC(JEntity entity, int compId)
        {
            return entity.DelC(this, compId);
        }

        public EIterator EntityIte(UInt256 archetype)
        {
            return new EIterator(this, archetype);
        }

        public void RelinkComp(JEntity e, int compId)
        {
            e.RelinkC(this, compId);
        }

        public void UnlinkComp(JEntity e, int compId)
        {
            e.UnlinkC(this, compId);
        }

        public virtual void Tick(TickParam p)
        {
            // 更新ECS帧数与世界时间
            Time.Tick(p.DeltaMilliseconds);

            // 系统优先运行
            for (int i = 0, imax = __systems.Count; i < imax; i++)
            {
                __systems[i].Tick(p);
            }

            // 原型管理更新
            ArchetypeMgr.ExeDirty();

            // 清除通知中心记录
            logMgr.Switch();
        }

        #endregion
    }
}