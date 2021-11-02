namespace JECS.Editor
{
    public interface IJECSWindowItem
    {
        /// <summary>
        /// 窗口项标题
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 窗口项标题说明
        /// </summary>
        string Tooltip { get; }

        void OnEnable(JECSConsoleWindow consoleWindow);
        void OnGUI();
    }
}