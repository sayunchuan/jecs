using System;

namespace JECS.Core
{
    public abstract class JWorld
    {
        /// <summary>
        /// 实体实例池
        /// </summary>
        private readonly JQueue<JEntity> _entityPool = new JQueue<JEntity>();

        /// <summary>
        /// 实体列表
        /// </summary>
        private readonly JBlockArray<JEntity> _entitys = new JBlockArray<JEntity>();

        /// <summary>
        /// 实体删除锁定集合，用于锁定实体的删除操作仅执行一次
        /// </summary>
        private readonly JHashSet_Int32 _entityDeleteLock = new JHashSet_Int32();

        /// <summary>
        /// 组件实例池
        /// </summary>
        private JCompPool[] _compCachePools;

        /// <summary>
        /// 系统队列，依序执行
        /// </summary>
        private readonly JList<JSystem> _systems = new JList<JSystem>();

        /// <summary>
        /// 该系统下的组件数量，用于生成指定数量的组件数组与组件池数组
        /// </summary>
        public abstract int CompNum { get; }

        /// <summary>
        /// 原型管理
        /// </summary>
        public readonly ArchetypeMgr ArchetypeMgr = new ArchetypeMgr();

        /// <summary>
        /// 日志管理
        /// </summary>
        public readonly JLogMgr LogMgr = new JLogMgr();

        /// <summary>
        /// 世界时间
        /// </summary>
        public readonly Time Time = new Time();

        public JWorld()
        {
            _compCachePools = new JCompPool[CompNum];
        }

        /// <summary>
        /// 基础初始化，ECS系统运行前执行
        /// </summary>
        public virtual void Init()
        {
            // 初始化所有系统，供系统调用新数据进行运行配置
            for (int i = 0, imax = _systems.Count; i < imax; i++)
            {
                var s = _systems[i];
                s.OnInit();
            }
        }

        /// <summary>
        /// 释放资源，将世界恢复到初始的样子，但是对各缓存实例则保留
        /// </summary>
        public virtual void Clear()
        {
            // recover entity and component
            for (int i = 0, imax = _entitys.Count; i < imax; i++)
            {
                var e = _entitys[i];
                if (e == null) continue;
                ReleaseE(e);
            }

            _entitys.Clear();
            _entityDeleteLock.Clear();

            // recover system
            for (int i = 0, imax = _systems.Count; i < imax; i++)
            {
                var s = _systems[i];
                s.OnClear();
            }

            // reset archetype manager
            ArchetypeMgr.Clear();

            // reset JLogMgr
            LogMgr.Clear();

            // 重置时间
            Time.Clear();
        }

        #region protected Methods

        /// <summary>
        /// 添加组件池，应于世界实例生成时调用，将所有类型的组件池加入世界中
        /// </summary>
        protected void _AddCompPool(int compId, JCompPool pool)
        {
            pool.CompId = compId;
            if (compId >= _compCachePools.Length)
            {
                int wantLen = _compCachePools.Length;
                while (wantLen <= compId)
                {
                    wantLen <<= 1;
                }

                JCompPool[] tmp = new JCompPool[wantLen];
                Array.Copy(_compCachePools, 0, tmp, 0, _compCachePools.Length);
                _compCachePools = tmp;
            }

            _compCachePools[compId] = pool;
        }

        /// <summary>
        /// 添加系统，应于世界实例生成时调用，将所有系统加入世界中
        /// </summary>
        protected void _AddSystem(JSystem sys)
        {
            _systems.Add(sys);
            sys.SetWorld(this);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// 生成组件
        /// </summary>
        internal JComp SpawnC(int compId, int uid)
        {
            return _compCachePools[compId].Spawn(this, uid);
        }

        /// <summary>
        /// 释放组件资源，按照组件类型入池，并清理组件内容
        /// </summary>
        internal void ReleaseC(JComp comp)
        {
            _compCachePools[comp.CompId].Release(this, comp);
        }

        /// <summary>
        /// 释放实体资源，并将实体实例入池，内部无需处理log系统
        /// </summary>
        internal void ReleaseE(JEntity entity)
        {
            int uid = entity.UID;
            entity.OnClear(this);
            _entitys.Release(uid);
            _entityPool.Enqueue(entity);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 获取新实体
        /// </summary>
        public JEntity NewE()
        {
            JEntity res = _entityPool.Count > 0 ? _entityPool.Dequeue() : new JEntity(CompNum);
            res.UID = _entitys.Count;
            _entitys.Add(res);
            LogMgr.AddEntity(res.UID);
            return res;
        }

        /// <summary>
        /// 加载指定uid实体
        /// </summary>
        public JEntity GetE(int uid)
        {
            return uid >= 0 && uid < _entitys.Count ? _entitys[uid] : null;
        }

        /// <summary>
        /// 删除指定实体
        /// </summary>
        public virtual void DelE(int uid)
        {
            JEntity e = _entitys[uid];
            if (e == null) return;

            // 删除实体已被锁定，则跳过删除操作
            if (_entityDeleteLock.Contains(uid)) return;

            // 锁定实体
            _entityDeleteLock.Add(uid);

            // 删除实体核心操作
            e.OnDel(this);
            _entitys.Release(uid);
            _entityPool.Enqueue(e);

            // 解锁实体
            _entityDeleteLock.Remove(uid);
        }

        /// <summary>
        /// 删除指定组件
        /// </summary>
        public virtual bool DelC(JEntity entity, int compId)
        {
            return entity.DelC(this, compId);
        }

        /// <summary>
        /// 获取指定原型的实体迭代器
        /// </summary>
        public EIterator EntityIte(UInt256 archetype)
        {
            return new EIterator(this, archetype);
        }

        /// <summary>
        /// 解绑组件，并返回解绑成功标识
        /// </summary>
        public bool UnlinkComp(JEntity e, int compId)
        {
            return e.UnlinkC(this, compId);
        }

        /// <summary>
        /// 重绑组件，并返回重绑成功标识
        /// </summary>
        public bool RelinkComp(JEntity e, int compId)
        {
            return e.RelinkC(this, compId);
        }

        /// <summary>
        /// ECS系统心跳
        /// </summary>
        public virtual void Tick(TickParam p)
        {
            // 更新ECS帧数与世界时间
            Time.Tick(p.DeltaMilliseconds);

            // 系统优先运行
            for (int i = 0, imax = _systems.Count; i < imax; i++)
            {
                _systems[i].Tick(p);
            }

            // 原型管理更新
            ArchetypeMgr.ExeDirty();

            // 清除通知中心记录
            LogMgr.Switch();
        }

        #endregion
    }
}