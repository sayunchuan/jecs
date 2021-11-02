using System;
using System.Collections.Generic;
using System.Text;
using JECS.Core;

namespace JECS.Editor.AutoTools
{
    public abstract class ASystemAutoGenerate : AJECSWindowItem
    {
        public override string Title => "自动生成 - JECS系统注册代码";

        public override string Tooltip =>
            "自动生成JECS的系统注册代码，通过调用\'JWorld\'的\'_AddSystem\'方法将所有系统注册到ECS内";

        public override void OnGUI()
        {
            if (Button("Generate"))
            {
                Generate();
            }
        }

        public virtual void Generate()
        {
            // TODO: 自定义实现生成按钮的反应，可于此处将代码写入目标文件中
        }

        /// <summary>
        /// 反射出所有系统类，并按照名称排序
        /// </summary>
        public List<Type> AllSystemType()
        {
            List<Type> items = new List<Type>();
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblys)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (!type.IsClass) continue;
                    if (type.IsAbstract) continue;
                    if (!type.IsSubclassOf(typeof(JSystem))) continue;

                    items.Add(type);
                }
            }

            items.Sort((l, r) => { return String.Compare(l.FullName, r.FullName, StringComparison.Ordinal); });

            return items;
        }

        public virtual string AddAllSystemCode()
        {
            var allType = AllSystemType();
            StringBuilder sb = new StringBuilder();
            foreach (var type in allType)
            {
                sb.Append("_AddSystem(new ").Append(type.Name).AppendLine("());");
            }

            return sb.ToString();
        }
    }
}