namespace JECS.Core
{
    public class JLogList
    {
        private readonly JList<JLogMsg> _listCore = new JList<JLogMsg>();
        private readonly JQueue<JLogMsg> _msgQueue = new JQueue<JLogMsg>();

        public JLogMsg this[int index] => _listCore[index];

        public int Count => _listCore.Count;

        public void Clear()
        {
            for (int i = 0, imax = _listCore.Count; i < imax; i++)
            {
                _listCore[i].Clean();
                _msgQueue.Enqueue(_listCore[i]);
            }

            _listCore.Clear();
        }

        private JLogMsg _NewMsg()
        {
            if (_msgQueue.Count > 0)
            {
                return _msgQueue.Dequeue();
            }

            return new JLogMsg();
        }

        public void AddEntity(int UID)
        {
            JLogMsg n = _NewMsg();
            n.AddEntity(UID);
            _listCore.Add(n);
        }

        public void DelEntity(int UID)
        {
            JLogMsg n = _NewMsg();
            n.DelEntity(UID);
            _listCore.Add(n);
        }

        public void AddComponent(int UID, int compId)
        {
            JLogMsg n = _NewMsg();
            n.AddComponent(UID, compId);
            _listCore.Add(n);
        }

        public void DelComponent(int UID, int compId)
        {
            JLogMsg n = _NewMsg();
            n.DelComponent(UID, compId);
            _listCore.Add(n);
        }
    }
}