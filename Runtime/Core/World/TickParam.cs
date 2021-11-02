namespace JECS.Core
{
    /// <summary>
    /// 系统心跳参数，可依据项目需求扩展
    /// </summary>
    public class TickParam
    {
#if UNITY_EDITOR
        public int DeltaMilliseconds { get; private set; }
#else
        public int DeltaMilliseconds;
#endif

        public void SetDelta(int delta_ms)
        {
            DeltaMilliseconds = delta_ms;
        }
    }
}