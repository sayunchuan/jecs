using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

namespace JECS.Editor.Tools
{
    public class OpenFolder
    {
        private static string shellPath;

        public static void Open(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            if (!Directory.Exists(path))
            {
                UnityEngine.Debug.LogError("No Directory: " + path);
                return;
            }

            var f = Directory.GetFiles(Application.dataPath + "/..", "opendir.sh", SearchOption.AllDirectories);
            shellPath = f[0];
            UnityEngine.Debug.Log(shellPath);

            // 新开线程防止锁死
            Thread newThread = new Thread(new ParameterizedThreadStart(CmdOpenDirectory));
            newThread.Start(path);
        }

        private static void CmdOpenDirectory(object obj)
        {
            Process p = new Process();
#if UNITY_EDITOR_WIN
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c start " + obj.ToString();
#elif UNITY_EDITOR_OSX
            p.StartInfo.FileName = "bash";
            p.StartInfo.Arguments = shellPath + " " + obj.ToString();
#endif
            //UnityEngine.Debug.Log(p.StartInfo.Arguments);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.WaitForExit();
            p.Close();
        }
    }
}