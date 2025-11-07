#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class UnityGit
{
    private const string GIT_ADD = "add";
    private const string GIT_COMMIT = "commit";
    private const string GIT_PULL = "pull";
    private const string GIT_LOG = "log";
    private const string GIT_REVERT = "revert";

    private const string GIT_COMMIT_MENU = "Assets/Git/Commit &g";
    private const string GIT_COMMIT_ALL_MENU = "Assets/Git/CommitAll &#g";
    private const string GIT_ADD_MENU = "Assets/Git/Add &a";
    private const string GIT_ADD_ALL_MENU = "Assets/Git/AddAll &#a";
    private const string GIT_PULL_MENU = "Assets/Git/Pull &p";
    private const string GIT_PULL_ALL_MENU = "Assets/Git/PullAll &#p";
    private const string GIT_LOG_MENU = "Assets/Git/ShowLog &l";
    private const string GIT_LOG_ALL_MENU = "Assets/Git/ShowLogAll &#l";
    private const string GIT_REVERT_MENU = "Assets/Git/Revert &r";

    /// <summary>
    /// 创建一个Git的cmd命令（通过TortoiseGit）
    /// </summary>
    /// <param name="command">命令</param>
    /// <param name="path">命令激活路径</param>
    public static void GitCommand(string command, string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        string c = "/c TortoiseGitProc.exe /command:{0} /path:\"{1}\" /closeonend 2";
        c = string.Format(c, command, path);
        ProcessStartInfo info = new ProcessStartInfo("cmd.exe", c);
        info.WindowStyle = ProcessWindowStyle.Hidden;
        Process.Start(info);
    }

    /// <summary>
    /// 直接调用git.exe命令（可选）
    /// </summary>
    public static void GitExeCommand(string args, string workingDir)
    {
        ProcessStartInfo info = new ProcessStartInfo("git", args);
        info.WorkingDirectory = workingDir;
        info.WindowStyle = ProcessWindowStyle.Hidden;
        info.CreateNoWindow = true;
        info.UseShellExecute = false;
        Process.Start(info);
    }

    public static void GitAdd(string[] paths)
    {
        if (paths == null || paths.Length <= 0) return;
        string path = string.Empty;
        for (int i = 0; i < paths.Length; i++)
        {
            string temp = paths[i].Replace("\\", "/");
            if (temp.Contains(":/"))
            {
                path += temp + "*";
                if (System.IO.File.Exists(temp + ".meta"))
                {
                    path += temp + ".meta*";
                }
            }
            else if (temp.StartsWith("Assets/"))
            {
                temp = AssetsPathToFilePath(temp);
                path += temp + "*";
                if (System.IO.File.Exists(temp + ".meta"))
                {
                    path += temp + ".meta*";
                }
            }
        }
        if (string.IsNullOrEmpty(path)) return;
        if (path[path.Length - 1] == '*')
            path = path.Substring(0, path.Length - 1);
        GitCommand(GIT_ADD, path);
    }

    public static void GitCommit(params string[] paths)
    {
        if (paths == null || paths.Length <= 0) return;
        string path = string.Empty;
        for (int i = 0; i < paths.Length; i++)
        {
            string temp = paths[i].Replace("\\", "/");
            if (temp.Contains(":/"))
            {
                path += temp + "*";
                if (System.IO.File.Exists(temp + ".meta"))
                {
                    path += temp + ".meta*";
                }
            }
            else if (temp.StartsWith("Assets/"))
            {
                temp = AssetsPathToFilePath(temp);
                path += temp + "*";
                if (System.IO.File.Exists(temp + ".meta"))
                {
                    path += temp + ".meta*";
                }
            }
        }
        if (string.IsNullOrEmpty(path)) return;
        if (path[path.Length - 1] == '*')
            path = path.Substring(0, path.Length - 1);
        GitCommand(GIT_COMMIT, path);
    }

    [MenuItem(GIT_ADD_MENU)]
    public static void GitAddMenu()
    {
        GitCommand(GIT_ADD, GetSelectedObjectPath());
    }

    [MenuItem(GIT_ADD_ALL_MENU)]
    public static void GitAddAllMenu()
    {
        GitCommand(GIT_ADD, Application.dataPath);
    }

    [MenuItem(GIT_COMMIT_MENU)]
    public static void GitCommitMenu()
    {
        GitCommand(GIT_COMMIT, GetSelectedObjectPath());
    }

    [MenuItem(GIT_COMMIT_ALL_MENU)]
    public static void GitCommitAllMenu()
    {
        GitCommand(GIT_COMMIT, Application.dataPath);
    }

    [MenuItem(GIT_PULL_MENU)]
    public static void GitPullMenu()
    {
        GitCommand(GIT_PULL, GetSelectedObjectPath());
    }

    [MenuItem(GIT_PULL_ALL_MENU)]
    public static void GitPullAllMenu()
    {
        GitCommand(GIT_PULL, Application.dataPath);
    }

    [MenuItem(GIT_LOG_MENU)]
    public static void GitLogMenu()
    {
        GitCommand(GIT_LOG, GetSelectedObjectPath());
    }

    [MenuItem(GIT_LOG_ALL_MENU)]
    public static void GitLogAllMenu()
    {
        GitCommand(GIT_LOG, Application.dataPath);
    }

    [MenuItem(GIT_REVERT_MENU)]
    public static void GitRevertMenu()
    {
        GitCommand(GIT_REVERT, GetSelectedObjectPath());
    }

    /// <summary>
    /// 获取全部选中物体的路径，包括meta文件
    /// </summary>
    private static string GetSelectedObjectPath()
    {
        string path = string.Empty;
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            path += AssetsPathToFilePath(AssetDatabase.GetAssetPath(Selection.objects[i]));
            path += "*";
            path += AssetsPathToFilePath(AssetDatabase.GetAssetPath(Selection.objects[i])) + ".meta";
            path += "*";
        }
        return path;
    }

    /// <summary>
    /// 将Assets路径转换为File路径
    /// </summary>
    public static string AssetsPathToFilePath(string path)
    {
        string m_path = Application.dataPath;
        m_path = m_path.Substring(0, m_path.Length - 6);
        m_path += path;
        return m_path;
    }
}
#endif