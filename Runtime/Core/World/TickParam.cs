namespace JECS.Core
{
    public class TickParam
    {
#if UNITY_EDITOR
        public bool lastFrame { get; private set; }
        public int DeltaMilliseconds { get; private set; }
        public int ServerFrameId { get; private set; }
        public int LogicFrameId { get; private set; }
#else
        public bool lastFrame;
        public int DeltaMilliseconds;
        public int ServerFrameId;
        public int LogicFrameId;
#endif

        public void SetLast(bool isLastFrame)
        {
            lastFrame = isLastFrame;
        }

        public void SetDelta(int delta_ms)
        {
            DeltaMilliseconds = delta_ms;
        }

        public void SetServerFrameId(int id)
        {
            ServerFrameId = id;
        }

        public void SetLogicFrameId(int id)
        {
            LogicFrameId = id;
        }
    }
}