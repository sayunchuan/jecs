using System.Collections.Generic;

namespace JECS.Core
{
    public class JLogList
    {
        private List<JLogMsg> __listCore;
        private Queue<JLogMsg> __msgQueue;

        public JLogList()
        {
            __listCore = new List<JLogMsg>();
            __msgQueue = new Queue<JLogMsg>();
        }

        public JLogMsg this[int index]
        {
            get { return __listCore[index]; }
        }

        public int Count
        {
            get { return __listCore.Count; }
        }

        public void Clear()
        {
            for (int i = 0, imax = __listCore.Count; i < imax; i++)
            {
                __listCore[i].Clean();
                __msgQueue.Enqueue(__listCore[i]);
            }

            __listCore.Clear();
        }

        private JLogMsg __NewMsg()
        {
            if (__msgQueue.Count > 0)
            {
                return __msgQueue.Dequeue();
            }

            return new JLogMsg();
        }

        public void AddEntity(int UID)
        {
            JLogMsg n = __NewMsg();
            n.AddEntity(UID);
            __listCore.Add(n);
        }

        public void DelEntity(int UID)
        {
            JLogMsg n = __NewMsg();
            n.DelEntity(UID);
            __listCore.Add(n);
        }

        public void AddComponent(int UID, int compId)
        {
            JLogMsg n = __NewMsg();
            n.AddComponent(UID, compId);
            __listCore.Add(n);
        }

        public void DelComponent(int UID, int compId)
        {
            JLogMsg n = __NewMsg();
            n.DelComponent(UID, compId);
            __listCore.Add(n);
        }
    }
}