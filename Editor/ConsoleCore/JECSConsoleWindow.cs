using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JECS.Editor
{
    public class JECSConsoleWindow : EditorWindow
    {
        [MenuItem("JECS/Open Console Window")]
        public static void Generate()
        {
            GetWindow(typeof(JECSConsoleWindow));
        }

        private readonly List<IJECSWindowItem> _items = new List<IJECSWindowItem>();
        private Vector2 _scroll = Vector2.zero;

        public JECSConsoleWindow()
        {
            titleContent = new GUIContent("JECS Console Window");
        }

        private void OnEnable()
        {
            _items.Clear();
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblys)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (!type.IsClass) continue;
                    if (type.IsAbstract) continue;
                    if (!typeof(IJECSWindowItem).IsAssignableFrom(type)) continue;

                    IJECSWindowItem tar = Activator.CreateInstance(type) as IJECSWindowItem;
                    if (tar != null) _items.Add(tar);
                }
            }

            _items.Sort((l, r) =>
            {
                return String.Compare(l.GetType().FullName, r.GetType().FullName, StringComparison.Ordinal);
            });

            for (int i = 0, imax = _items.Count; i < imax; i++)
            {
                _items[i].OnEnable(this);
            }
        }

        private void OnGUI()
        {
            GUIStyle s = new GUIStyle(GUI.skin.label);
            s.fontStyle = FontStyle.Bold;
            s.fontSize = (int)(s.fontSize * 1.2);

            _scroll = GUILayout.BeginScrollView(_scroll);
            for (int i = 0, imax = _items.Count; i < imax; i++)
            {
                var item = _items[i];
                if (i != 0) GUILayout.Space(5f);
                GUILayout.BeginVertical("box");
                if (string.IsNullOrEmpty(item.Tooltip))
                {
                    GUILayout.Label($"{item.Title}:", s);
                }
                else
                {
                    GUILayout.Label(new GUIContent($"{item.Title}:", item.Tooltip), s);
                }

                GUILayout.Space(5f);
                item.OnGUI();
                GUILayout.EndVertical();
            }

            GUILayout.EndScrollView();
        }
    }
}