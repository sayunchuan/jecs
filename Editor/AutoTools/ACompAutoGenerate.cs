using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace JECS.Editor.AutoTools
{
    /// <summary>
    /// 组件池注册代码自动生成
    /// 通过枚举定义组件，再遍历枚举自动生成对应组件池（组件名应与枚举名保持一致）
    /// </summary>
    public abstract class ACompAutoGenerate : AJECSWindowItem
    {
        public override string Title => "自动生成 - JECS组件池注册代码";

        public override string Tooltip =>
            "自动生成JECS的组件池注册代码，通过调用\'JWorld\'的\'_AddCompPool\'方法将所有组件类型的组件池注册到ECS内";

        /// <summary>
        /// 组件枚举类型
        /// </summary>
        public abstract Type CompEnumType { get; }

        /// <summary>
        /// 自动生成的组件工具方法所在路径
        /// </summary>
        public abstract string CompUtilPath { get; }

        public virtual List<string> AllCompName()
        {
            List<string> names = new List<string>();
            foreach (var name in Enum.GetNames(CompEnumType))
            {
                names.Add(name);
            }

            return names;
        }

        public virtual string AddAllCompPoolCode()
        {
            var names = AllCompName();
            StringBuilder sb = new StringBuilder();

            string context = "_AddCompPool((int) $ENUM_NAME$.$ENUM_ITEM$, new JCompPool(() => new $COMP_TYPE$()));";
            foreach (string name in names)
            {
                sb.AppendLine(context.Replace("$ENUM_NAME$", CompEnumType.Name)
                    .Replace("$ENUM_ITEM$", name)
                    .Replace("$COMP_TYPE$", EnumName2CompName(name)));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 枚举内名称转组件名称
        /// </summary>
        public virtual string EnumName2CompName(string enumName)
        {
            return "Comp" + enumName;
        }

        private static string CoreBaseUtil = @"using JECS.Core;

public static class JECSCompUtil
{
    #region JEntity Get Comp Methods
$ENTITY_GET_METHODS$
    #endregion

    #region JEntity Add Comp Methods
$ENTITY_ADD_METHODS$
    #endregion

    #region JEntity Get Comp Methods
$CORE_GET_METHODS$
    #endregion

    #region JEntity Add Comp Methods
$CORE_ADD_METHODS$
    #endregion
}
";

        /// <summary>
        /// 生成JECS的组件公用代码，通过生成静态类JECSCompUtil，使JEntity与JWorld添加、获取组件更为便利
        /// </summary>
        public virtual void GenCompUtil()
        {
            string eGetMethods = "";
            string eAddMethods = "";
            string getMethods = "";
            string addMethods = "";
            var names = AllCompName();
            foreach (var name in names)
            {
                eGetMethods += EntityGetMethods(name);
                eAddMethods += EntityAddMethods(name);
                getMethods += GetMethods(name);
                addMethods += AddMethods(name);
            }

            string fileInfo = CoreBaseUtil.Replace("$ENTITY_GET_METHODS$", eGetMethods)
                .Replace("$ENTITY_ADD_METHODS$", eAddMethods)
                .Replace("$CORE_GET_METHODS$", getMethods)
                .Replace("$CORE_ADD_METHODS$", addMethods);
            File.WriteAllText(CompUtilPath, fileInfo);

            AssetDatabase.Refresh();
        }

        public virtual string EntityGetMethods(string name)
        {
            string context = @"
    public static $COMP_TYPE$ Get$COMP_TYPE$ (this JEntity e)
    {
        return ($COMP_TYPE$) e.GetC((int) $ENUM_NAME$.$ENUM_ITEM$);
    }
";
            return context.Replace("$COMP_TYPE$", EnumName2CompName(name))
                .Replace("$ENUM_ITEM$", name)
                .Replace("$ENUM_NAME$", CompEnumType.Name);
        }

        public virtual string EntityAddMethods(string name)
        {
            string context = @"
    public static $COMP_TYPE$ Add$COMP_TYPE$ (this JEntity e, JWorld w)
    {
        return ($COMP_TYPE$) e.AddC(w, (int) $ENUM_NAME$.$ENUM_ITEM$);
    }
";
            return context.Replace("$COMP_TYPE$", EnumName2CompName(name))
                .Replace("$ENUM_ITEM$", name)
                .Replace("$ENUM_NAME$", CompEnumType.Name);
        }

        public virtual string GetMethods(string name)
        {
            string context = @"
    public static $COMP_TYPE$ Get$COMP_TYPE$ (this JWorld w, int uid)
    {
        return w.GetE(uid)?.Get$COMP_TYPE$();
    }
";
            return context.Replace("$COMP_TYPE$", EnumName2CompName(name));
        }

        public virtual string AddMethods(string name)
        {
            string context = @"
    public static $COMP_TYPE$ Add$COMP_TYPE$ (this JWorld w, int uid)
    {
        return w.GetE(uid)?.Add$COMP_TYPE$(w);
    }
";
            return context.Replace("$COMP_TYPE$", EnumName2CompName(name));
        }
    }
}