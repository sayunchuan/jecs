using UnityEditor;
using UnityEngine;

namespace JECS.Editor
{
    /// <summary>
    /// 窗口项抽象类，用于辅助窗口内容的生成
    /// </summary>
    public abstract class AJECSWindowItem : IJECSWindowItem
    {
        protected JECSConsoleWindow window;
        public Rect Position => window.position;

        public abstract string Title { get; }
        public abstract string Tooltip { get; }

        public virtual void OnEnable(JECSConsoleWindow consoleWindow)
        {
            this.window = consoleWindow;
        }

        public abstract void OnGUI();

        /// <summary>
        /// 设置颜色
        /// </summary>
        public void SetColor(Color c)
        {
            GUI.color = c;
        }

        /// <summary>
        /// 重置颜色
        /// </summary>
        public void ResetColor()
        {
            GUI.color = Color.white;
        }

        /// <summary>
        /// 显示标题
        /// </summary>
        /// <param name="text"></param>
        public void LTitle(string text)
        {
            GUIStyle s = new GUIStyle(GUI.skin.label);
            s.fontStyle = FontStyle.Bold;
            GUILayout.Label(text, s);
        }

        public void Label(string text, params GUILayoutOption[] options) => GUILayout.Label(text, options);

        public void Label(string text, string tooltips, params GUILayoutOption[] options) =>
            GUILayout.Label(new GUIContent(text, tooltips), options);

        public bool Button(string text, params GUILayoutOption[] options) => GUILayout.Button(text, options);

        public bool Button(string text, string tooltip, params GUILayoutOption[] options) =>
            GUILayout.Button(new GUIContent(text, tooltip), options);

        public void Space(float pixels) => GUILayout.Space(pixels);

        public void HelpBox(string message, MessageType type, bool wide) =>
            EditorGUILayout.HelpBox(message, type, wide);

        public bool DisplayDialog(string title, string message, string ok) =>
            EditorUtility.DisplayDialog(title, message, ok);

        public bool DisplayDialog(string title, string message, string ok, string cancel) =>
            EditorUtility.DisplayDialog(title, message, ok, cancel);

        public Vector2 BeginScrollView(Vector2 scrollPosition, params GUILayoutOption[] options) =>
            GUILayout.BeginScrollView(scrollPosition, options);

        public void EndScrollView() => GUILayout.EndScrollView();

        public void BeginHorizontal(params GUILayoutOption[] options) => GUILayout.BeginHorizontal(options);

        public void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options) =>
            GUILayout.BeginHorizontal(style, options);

        public void EndHorizontal() => GUILayout.EndHorizontal();

        public void BeginVertical(params GUILayoutOption[] options) => GUILayout.BeginVertical(options);

        public void BeginVertical(GUIStyle style, params GUILayoutOption[] options) =>
            GUILayout.BeginVertical(style, options);

        public void EndVertical() => GUILayout.EndVertical();

        public Color GetColor(Color baseColor, params Color[] backColor)
        {
            Color tmp = baseColor;
            for (int i = 0, imax = backColor.Length; i < imax; i++)
            {
                tmp -= backColor[i];
            }

            tmp.r = tmp.r > 0 ? tmp.r : 0;
            tmp.g = tmp.g > 0 ? tmp.g : 0;
            tmp.b = tmp.b > 0 ? tmp.b : 0;
            tmp.a = 1;
            return tmp;
        }
    }
}