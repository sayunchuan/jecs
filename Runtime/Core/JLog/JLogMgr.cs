namespace JECS.Core
{
    public class JLogMgr
    {
        private JLogList __returnList;
        private JLogList __noteList;

        public JLogMgr()
        {
            __returnList = new JLogList();
            __noteList = new JLogList();
        }

        public void Release()
        {
            __returnList.Clear();
            __noteList.Clear();
        }

        public JLogList LogInfoList()
        {
            return __returnList;
        }

        public void Switch()
        {
            JLogList tmp = __noteList;
            __noteList = __returnList;
            __returnList = tmp;
            __noteList.Clear();
        }

        public void AddEntity(int UID)
        {
            __noteList.AddEntity(UID);
        }

        public void DelEntity(int UID)
        {
            __noteList.DelEntity(UID);
        }

        public void AddComponent(int UID, int compId)
        {
            __noteList.AddComponent(UID, compId);
        }

        public void DelComponent(int UID, int compId)
        {
            __noteList.DelComponent(UID, compId);
        }
    }
}