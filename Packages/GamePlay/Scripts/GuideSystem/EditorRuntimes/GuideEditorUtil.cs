#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TagLib.Asf;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Guide.Editor
{
    //------------------------------------------------------
    public class GuideEditorUtil
    {
        public static void RecodeWidget(GuideSystemEditor pEditor, GameObject widget)
        {
            if (widget == null)
                return;

            var uiGraphic = widget.GetComponent<Graphic>();
            if (uiGraphic == null)
                return;
            GuideGuid guide = null;
            int listIndex = -1;
            string tagPath = "";
            bool bDynamicCreate = false;
            if (pEditor.OnCustomRecodeMethod!=null)
            {
                var objData = pEditor.OnCustomRecodeMethod.Invoke(null, new object[] { uiGraphic });
                if(objData!=null)
                {
                    var dynamicCreateField = objData.GetType().GetField("dynamicCreate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (dynamicCreateField != null && dynamicCreateField.FieldType == typeof(bool)) 
                        bDynamicCreate=(bool)dynamicCreateField.GetValue(objData);

                    var tagPathField = objData.GetType().GetField("pathTag", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (tagPathField != null && tagPathField.FieldType == typeof(string))
                        tagPath = (string)tagPathField.GetValue(objData);

                    var listField = objData.GetType().GetField("listIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (listField != null && listField.FieldType == typeof(int))
                        listIndex = (int)listField.GetValue(objData);

                    var listViewField = objData.GetType().GetField("guideGuid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (listViewField != null)
                        guide = (GuideGuid)listViewField.GetValue(objData);
                }
            }

            if(guide == null)
            {
                guide = widget.GetComponent<GuideGuid>();
                if (guide == null) guide = widget.AddComponent<GuideGuid>();
                if (guide.guid == 0)
                {
                    guide.guid = GuideGuidUtl.GeneratorGUID(guide);
                    GuideGuidUtl.SetDirtyPrefab(guide);
                }
            }
            GuideGuidUtl.OnAdd(guide, false);



            //   EventTriggerListener eventTrigger = widget.GetComponent<EventTriggerListener>();
            //   if (eventTrigger == null) eventTrigger = widget.AddComponent<EventTriggerListener>();
            //   eventTrigger.SetGuideGuid(guide);

            // var guidTag = GuideTag.GetTag(guide);
            string guidTag = "";
            if(bDynamicCreate)
            {
                guidTag = tagPath;
            }

            pEditor.AddRecodeClickStep(guide, guidTag, listIndex);
        }
        //------------------------------------------------------
        internal static void SetNodeDefault(BaseNode pNode)
        {
            Framework.Guide.Editor.GuideSystemEditor.NodeAttr nodeAttr = null;
            if(pNode is StepNode)
            {
                if (!Framework.Guide.Editor.GuideSystemEditor.StepTypes.TryGetValue(pNode.GetEnumType(), out nodeAttr))
                    return;
            }
            if (pNode is ExcudeNode)
            {
                if (!Framework.Guide.Editor.GuideSystemEditor.ExcudeTypes.TryGetValue(pNode.GetEnumType(), out nodeAttr))
                    return;
            }
            if (pNode is TriggerNode)
            {
                if (!Framework.Guide.Editor.GuideSystemEditor.TriggerTypes.TryGetValue(pNode.GetEnumType(), out nodeAttr))
                    return;
            }

            pNode.CheckPorts();
            var ports = pNode.GetArgvPorts();
            if (ports == null)
                return;


            for (int i =0; i < ports.Count && i < nodeAttr.argvs.Count; ++i)
            {
                var port = ports[i];
                if (port == null)
                    continue;
                object defValue = nodeAttr.argvs[i].attr.defaultValue;
                if (defValue == null) continue;
                if(nodeAttr.argvs[i].attr.displayType == typeof(UnityEngine.Vector2))
                {
                    var splits = defValue.ToString().Split(new char[] { ',', '|' });
                    if(splits.Length>1)
                    {
                        float x = 0f;
                        float y = 0f;
                        float.TryParse(splits[0], out x);
                        float.TryParse(splits[1], out y);
                        var valueField = port.GetType().GetField("value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if(valueField!=null)
                        {
                            valueField.SetValue(port, (int)(((uint)(x * 100f) << 16) | (uint)(y * 100f)));
                        }
                    }
                }
                else if (nodeAttr.argvs[i].attr.displayType == typeof(UnityEngine.Color))
                {
                    var splits = defValue.ToString().Split(new char[] { ',', '|' });
                    if (splits.Length > 1)
                    {
                        float.TryParse(splits[0], out var r);
                        float.TryParse(splits[1], out var g);
                        float.TryParse(splits[2], out var b);
                        float.TryParse(splits[3], out var a);
                        var valueField = port.GetType().GetField("value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (valueField != null)
                        {
                            int color = ((int)(r * 255f) << 24) | ((int)(g * 255f) << 16) | ((int)(b * 255f) << 8) | (int)(a * 255f);
                            valueField.SetValue(port, color);
                        }
                    }
                }
                else if(nodeAttr.argvs[i].attr.displayType.IsEnum)
                {
                    var valueField = port.GetType().GetField("value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (valueField != null)
                    {
                        try
                        {
                            int enumValue = Convert.ToInt32(defValue);
                            valueField.SetValue(port, enumValue);
                        }
                        catch
                        {

                        }
                    }
                }
                else if (nodeAttr.argvs[i].attr.displayType == typeof(float))
                {
                    var valueField = port.GetType().GetField("value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (valueField != null && float.TryParse(defValue.ToString(), out var fValue))
                    {
                        valueField.SetValue(port, (int)(fValue * 1000));
                    }
                }
                else if (nodeAttr.argvs[i].attr.displayType == typeof(string))
                {
                    var valueField = port.GetType().GetField("strValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (valueField != null)
                    {
                        valueField.SetValue(port, defValue.ToString());
                    }
                }
                else
                {
                    var valueField = port.GetType().GetField("value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (valueField != null && int.TryParse(defValue.ToString(), out var nValue))
                    {
                        valueField.SetValue(port, nValue);
                    }
                }
            }
        }
        //------------------------------------------------------
        public static void CommitGit(string guideRoot)
        {
            guideRoot = guideRoot.Replace("\\", "/");
            if (guideRoot.StartsWith("Assets/")) guideRoot = guideRoot.Substring("Assets/".Length);

            bool useCommitFileList = false;
            string cacheFile = Path.Combine(Application.dataPath, "../Library/GuideTemps/CommitGitCache.txt");
            if (useCommitFileList)
            {
                if (!System.IO.File.Exists(cacheFile))
                {
                    EditorUtility.DisplayDialog("提交引导修改", "没有可提提交的修改记录", "确定");
                    return;
                }
            }


            string tortoiseGitExe = "TortoiseGitProc.exe";
            string[] possiblePaths = {
    @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe",
    @"C:\Program Files (x86)\TortoiseGit\bin\TortoiseGitProc.exe"
};
            foreach (var path in possiblePaths)
            {
                if (System.IO.File.Exists(path))
                {
                    tortoiseGitExe = path;
                    break;
                }
            }

            //   if(!System.IO.File.Exists(tortoiseGitExe))
            //   {
            //       UnityEngine.Debug.LogError("TortoiseGitProc.exe 未找到，请确保已安装 TortoiseGit 并正确配置路径。");
            //       return;
            //   }
            if (useCommitFileList)
            {
                string commandStr = $"/command:commit /pathfile:\"{cacheFile}\"";
                try
                {
                    Process p = new Process();
                    p.StartInfo.FileName = tortoiseGitExe;
                    p.StartInfo.Arguments = commandStr;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.WorkingDirectory = Application.dataPath;
                    p.Start();
                    p.BeginOutputReadLine();
                    p.WaitForExit();
                    int exitCode = p.ExitCode;
                    p.Close();
                    p.Dispose();
                    if (exitCode == 0)
                    {
                        //    if(File.Exists(cacheFile))
                        //       File.Delete(cacheFile);
                        //    EditorUtility.DisplayDialog("提交引导修改", "提交成功。", "确定");
                    }
                    else if (exitCode != 0)
                    {
                        EditorUtility.DisplayDialog("提交引导修改", "提交失败。", "确定");
                    }
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogError($"无法启动 TortoiseGit 提交界面: {ex.Message}");
                }
                return;
            }
            try
            {
                int commitCode = 0;
                string commitPath = "";
                List<string> commitDir = new List<string>();
                commitDir.Add(guideRoot);
                if (!string.IsNullOrEmpty(GuidePreferences.GetSettings().commitRoots))
                {
                    var splitPaths = GuidePreferences.GetSettings().commitRoots.Split(new char[] { ';', '|', '*' });
                    for(int i =0; i < splitPaths.Length; ++i)
                    {
                        var temp = splitPaths[i].Replace("\\", "/");
                        if (temp.StartsWith("Assets/")) temp = temp.Substring("Assets/".Length);
                        commitDir.Add(temp);
                    }
                }
                for (int i = 0; i < commitDir.Count; ++i)
                {
                    commitPath += Path.Combine(Application.dataPath, commitDir[i]).Replace("\\", "/");
                    if (i < commitDir.Count - 1) commitPath += "*";
                }
                string commandStr = $"/command:commit /path:\"{commitPath}\"";
                Process p = new Process();
                p.StartInfo.FileName = tortoiseGitExe;
                p.StartInfo.Arguments = commandStr;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.WorkingDirectory = Application.dataPath;
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd();
                p.WaitForExit();
                int exitCode = p.ExitCode;
                p.Close();
                p.Dispose();
                commitCode += exitCode;
                if (commitCode == 0)
                {
                    if (!string.IsNullOrEmpty(output))
                        EditorUtility.DisplayDialog("提交引导修改", output, "确定");
                }
                else if (commitCode != 0)
                {
                    if (error == null) error = "";
                    EditorUtility.DisplayDialog("提交引导修改", "提交失败。" + "\r\n" + error, "确定");
                }
            }
            catch
            {
                EditorUtility.DisplayDialog("提交引导修改", "提交失败。", "确定");
            }
        }
    }
}
#endif