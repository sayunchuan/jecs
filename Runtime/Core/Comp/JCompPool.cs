using System;
using System.Collections.Generic;

namespace JECS.Core
{
    public class JCompPool
    {
        public int CompId;

        /// <summary>
        /// 组件实例池
        /// </summary>
        private Queue<JComp> __pools;

        /// <summary>
        /// 组件生成方法委托
        /// </summary>
        private Func<JComp> __spawnFunc;

        public JCompPool(Func<JComp> spawnFunc)
        {
            __pools = new Queue<JComp>();
            __spawnFunc = spawnFunc;
        }

        /// <summary>
        /// 生成组件资源
        /// </summary>
        internal JComp Spawn(JWorld w, int uid)
        {
            JComp res = __pools.Count > 0 ? __pools.Dequeue() : __spawnFunc();
            res.OnLoad(w, uid);
            return res;
        }

        /// <summary>
        /// 释放组件资源
        /// </summary>
        internal void Release(JWorld w, JComp item)
        {
            item.OnRelease(w);
            __pools.Enqueue(item);
        }

        public override string ToString()
        {
            return CompId.ToString();
        }
    }
}