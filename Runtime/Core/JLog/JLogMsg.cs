namespace JECS.Core
{
    /// <summary>
    /// ECS内部消息
    /// </summary>
    public class JLogMsg
    {
        public int UID { get; private set; }
        public JLogMsgType MsgType { get; private set; }
        public int CompId { get; private set; }

        public void Clean()
        {
            UID = -1;
        }

        public void AddEntity(int UID)
        {
            this.UID = UID;
            MsgType = JLogMsgType.EntityAdd;
        }

        public void DelEntity(int UID)
        {
            this.UID = UID;
            MsgType = JLogMsgType.EntityDel;
        }

        public void AddComponent(int UID, int compId)
        {
            this.UID = UID;
            MsgType = JLogMsgType.ComponentAdd;
            CompId = compId;
        }

        public void DelComponent(int UID, int compId)
        {
            this.UID = UID;
            MsgType = JLogMsgType.ComponentDel;
            CompId = compId;
        }
    }
}