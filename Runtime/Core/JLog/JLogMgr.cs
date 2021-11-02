namespace JECS.Core
{
    /// <summary>
    /// ECS内部log，用于记录每ECS帧内的实体与组件变化
    /// </summary>
    public class JLogMgr
    {
        /// <summary>
        /// 由_returnList指向每帧内返回的log信息队列
        /// </summary>
        private JLogList _returnList = new JLogList();

        /// <summary>
        /// 由_noteList指向每帧内正在做记录的log信息队列
        /// </summary>
        private JLogList _noteList = new JLogList();

        internal void Clear()
        {
            _returnList.Clear();
            _noteList.Clear();
        }

        /// <summary>
        /// 获取当前帧
        /// </summary>
        /// <returns></returns>
        public JLogList LogInfoList()
        {
            return _returnList;
        }

        /// <summary>
        /// 每帧调用，切换队列
        /// </summary>
        internal void Switch()
        {
            JLogList tmp = _noteList;
            _noteList = _returnList;
            _returnList = tmp;
            _noteList.Clear();
        }

        internal void AddEntity(int UID)
        {
            _noteList.AddEntity(UID);
        }

        internal void DelEntity(int UID)
        {
            _noteList.DelEntity(UID);
        }

        internal void AddComponent(int UID, int compId)
        {
            _noteList.AddComponent(UID, compId);
        }

        internal void DelComponent(int UID, int compId)
        {
            _noteList.DelComponent(UID, compId);
        }
    }
}