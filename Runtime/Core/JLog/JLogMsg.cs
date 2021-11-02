namespace JECS.Core
{
    public class JLogMsg
    {
        public int UID { get; private set; }
        public JLogMsgType msgType { get; private set; }
        public int compId { get; private set; }

        public void Clean()
        {
            UID = -1;
        }

        public void AddEntity(int UID)
        {
            this.UID = UID;
            msgType = JLogMsgType.EntityAdd;
        }

        public void DelEntity(int UID)
        {
            this.UID = UID;
            msgType = JLogMsgType.EntityDel;
        }

        public void AddComponent(int UID, int compId)
        {
            this.UID = UID;
            msgType = JLogMsgType.ComponentAdd;
            this.compId = compId;
        }

        public void DelComponent(int UID, int compId)
        {
            this.UID = UID;
            msgType = JLogMsgType.ComponentDel;
            this.compId = compId;
        }
    }
}