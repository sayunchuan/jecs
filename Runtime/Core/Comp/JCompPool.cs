using System;

namespace JECS.Core
{
    public class JCompPool
    {
        /// <summary>
        /// 组件类型id
        /// </summary>
        public int CompId;

        /// <summary>
        /// 当前类型组件实例池
        /// </summary>
        private readonly JQueue<JComp> _pools = new JQueue<JComp>();

        /// <summary>
        /// 组件生成方法委托
        /// </summary>
        private readonly Func<JComp> _spawnFunc;

        public JCompPool(Func<JComp> spawnFunc)
        {
            _spawnFunc = spawnFunc;
        }

        /// <summary>
        /// 生成组件资源
        /// </summary>
        internal JComp Spawn(JWorld w, int uid)
        {
            JComp res = _pools.Count > 0 ? _pools.Dequeue() : _spawnFunc();
            res.InternalOnLoad(w, uid);
            return res;
        }

        /// <summary>
        /// 释放组件实例，并将其入池
        /// </summary>
        internal void Release(JWorld w, JComp item)
        {
            item.InternalOnClear(w);
            _pools.Enqueue(item);
        }

        public override string ToString()
        {
            return CompId.ToString();
        }
    }
}