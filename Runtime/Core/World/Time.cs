namespace JECS.Core
{
    public class Time
    {
        /// <summary>
        /// 当前时间，标识从世界开始心跳起的持续时间，单位毫秒
        /// </summary>
        public int Now { get; private set; }

        /// <summary>
        /// 当前帧号
        /// </summary>
        public int Frame { get; private set; }

        public Time()
        {
            Clear();
        }

        public void Clear()
        {
            Now = 0;
            Frame = 0;
        }

        public void Tick(int delta_ms)
        {
            Now += delta_ms;
            Frame++;
        }

        public override string ToString()
        {
            return $"{{\"Now\":{Now}, \"Frame\":{Frame}}}";
        }
    }
}