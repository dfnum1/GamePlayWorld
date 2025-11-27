/********************************************************************
生成日期:	06:30:2025
类    名: 	CustomAgentUtil
作    者:	HappLI
描    述:	引导自定义步骤、执行器、触发器
*********************************************************************/
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Guide.Editor
{
    public class CustomAgentUtil
    {
        private static Dictionary<uint, GuideCustomAgent.AgentUnit> ms_vAgents = null;
        private static List<GuideCustomAgent.AgentUnit> ms_vTriggerLists = null;
        private static List<GuideCustomAgent.AgentUnit> ms_vStepLists = null;
        private static List<GuideCustomAgent.AgentUnit> ms_vExecuteLists = null;
        private static GuideCustomAgent ms_Agent;
        public static void Init(bool bForce = false)
        {
            if(!bForce && ms_Agent!=null)
            {
                return;
            }
            ms_vAgents = new Dictionary<uint, GuideCustomAgent.AgentUnit>();
            ms_vTriggerLists = new List<GuideCustomAgent.AgentUnit>();
            ms_vStepLists = new List<GuideCustomAgent.AgentUnit>();
            ms_vExecuteLists = new List<GuideCustomAgent.AgentUnit>();
            string[] cutscenes = AssetDatabase.FindAssets("t:GuideCustomAgent");
            for (int i = 0; i < cutscenes.Length; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(cutscenes[i]);
                GuideCustomAgent agents = AssetDatabase.LoadAssetAtPath<GuideCustomAgent>(path);
                if (agents == null)
                    continue;
                if(agents.vTriggerAgents != null)
                {
                    foreach (var item in agents.vTriggerAgents)
                    {
                        if (!item.IsValid()) continue;
                        if (!ms_vAgents.ContainsKey(item.customType))
                        {
                            ms_vAgents.Add(item.customType, item);
                            ms_vTriggerLists.Add(item);
                        }
                        else
                        {
                            Debug.LogError($"Custom event type {item.customType} already exists trigger in {path}");
                        }
                    }
                }
                if(agents.vStepAgents != null)
                {
                    foreach (var item in agents.vStepAgents)
                    {
                        if (!item.IsValid()) continue;
                        if (!ms_vAgents.ContainsKey(item.customType))
                        {
                            ms_vAgents.Add(item.customType, item);
                            ms_vStepLists.Add(item);
                        }
                        else
                        {
                            Debug.LogError($"Custom clip type {item.customType} already exists step in {path}");
                        }
                    }
                }
                if (agents.vExecuteAgents != null)
                {
                    foreach (var item in agents.vExecuteAgents)
                    {
                        if (!item.IsValid()) continue;
                        if (!ms_vAgents.ContainsKey(item.customType))
                        {
                            ms_vAgents.Add(item.customType, item);
                            ms_vExecuteLists.Add(item);
                        }
                        else
                        {
                            Debug.LogError($"Custom clip type {item.customType} already exists execute in {path}");
                        }
                    }
                }
                ms_Agent = agents;
                break;//only load the first one
            }
        }
        //-----------------------------------------------------
        internal static void RefreshEditorData()
        {
            GuideSystemEditor.StepTypes.Clear();
            GuideSystemEditor.ExportTypesPop.Clear();
            GuideSystemEditor.ExportTypes.Clear();
            GuideSystemEditor.TriggerTypes.Clear();
            GuideSystemEditor.ExportTriggerTypesPop.Clear();
            GuideSystemEditor.ExportTriggerTypes.Clear();
            GuideSystemEditor.ExcudeTypes.Clear();
            GuideSystemEditor.ExportExcudeTypesPop.Clear();
            GuideSystemEditor.ExportExcudeTypes.Clear();
            GuideSystemEditor.NodeTypes.Clear();
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    Type enumType = types[i];
                    if (enumType.IsEnum && enumType.IsDefined(typeof(GuideExportAttribute), false))
                    {
                        foreach (var v in Enum.GetValues(enumType))
                        {
                            string strName = Enum.GetName(enumType, v);
                            FieldInfo fi = enumType.GetField(strName);
                            int flagValue = (int)v;

                            if (!fi.IsDefined(typeof(GuideStepAttribute)) && !fi.IsDefined(typeof(GuideTriggerAttribute)) && !fi.IsDefined(typeof(GuideExcudeAttribute)))
                                continue;

                            bool bPreviewEditor = false;
                            string strDeclName = "";
                            if (fi.IsDefined(typeof(GuideStepAttribute)))
                            {
                                bPreviewEditor = fi.GetCustomAttribute<GuideStepAttribute>().bEditorPreview;
                                strDeclName = fi.GetCustomAttribute<GuideStepAttribute>().DisplayName;
                            }
                            else if (fi.IsDefined(typeof(GuideExcudeAttribute)))
                            {
                                bPreviewEditor = fi.GetCustomAttribute<GuideExcudeAttribute>().bEditorPreview;
                                strDeclName = fi.GetCustomAttribute<GuideExcudeAttribute>().DisplayName;
                            }
                            else
                                strDeclName = fi.GetCustomAttribute<GuideTriggerAttribute>().DisplayName;

                            {
                                GuideSystemEditor.NodeAttr node = new GuideSystemEditor.NodeAttr();
                                node.type = flagValue;
                                node.previewEditor = bPreviewEditor;

                                node.strShortName = strDeclName;
                                if (string.IsNullOrEmpty(node.strShortName))
                                    node.strShortName = strName;


                                node.strExportName = enumType.GetCustomAttribute<GuideExportAttribute>().strDisplay;
                                if (string.IsNullOrEmpty(node.strExportName))
                                    node.strExportName = enumType.Name;

                                node.strName = node.strExportName + "/" + node.strShortName;
                                node.strQueueName = node.strName + strName;

                                //    node.bCanAny = faaiDeclar.CanAny;
                                GuideArgvAttribute[] argvs = (GuideArgvAttribute[])fi.GetCustomAttributes(typeof(GuideArgvAttribute));

                                node.argvs = new List<GuideSystemEditor.NodeAttr.ArgvAttr>();
                                if (argvs != null && argvs.Length > 0)
                                {
                                    for (int a = 0; a < argvs.Length; ++a)
                                    {
                                        GuideSystemEditor.NodeAttr.ArgvAttr attr = new GuideSystemEditor.NodeAttr.ArgvAttr();
                                        if(!string.IsNullOrEmpty(argvs[a].dispayTypeName))
                                        {
                                            if(GuideSystemEditor.DisplayTypes.TryGetValue(argvs[a].dispayTypeName, out var pluginDisp))
                                            {
                                                argvs[a].displayType = pluginDisp.displayType;
                                            }
                                        }
                                        attr.attr = argvs[a];
                                        attr.bBit = attr.attr.bBit;
                                        node.argvs.Add(attr);
                                    }
                                }

                                if (fi.IsDefined(typeof(GuideStepAttribute)))
                                {
                                    node.previewEditor = bPreviewEditor;
                                    GuideSystemEditor.StepTypes[flagValue] = node;

                                    GuideSystemEditor.ExportTypesPop.Add(node.strShortName);
                                    GuideSystemEditor.ExportTypes.Add(flagValue);
                                    GuideSystemEditor.NodeTypes[flagValue] = node;
                                }
                                else if (fi.IsDefined(typeof(GuideTriggerAttribute)))
                                {
                                    GuideSystemEditor.TriggerTypes[flagValue] = node;

                                    GuideSystemEditor.ExportTriggerTypesPop.Add(node.strShortName);
                                    GuideSystemEditor.ExportTriggerTypes.Add(flagValue);
                                }
                                else
                                {
                                    GuideSystemEditor.ExcudeTypes[flagValue] = node;

                                    GuideSystemEditor.ExportExcudeTypesPop.Add(node.strShortName);
                                    GuideSystemEditor.ExportExcudeTypes.Add(flagValue);
                                    GuideSystemEditor.NodeTypes[flagValue] = node;
                                }
                            }
                        }
                    }
                }
            }
            Init();
            foreach(var db in ms_vTriggerLists)
            {
                var node = BuildNodeAttr(db, "触发器");
                GuideSystemEditor.TriggerTypes[node.type] = node;

                GuideSystemEditor.ExportTriggerTypesPop.Add(node.strShortName);
                GuideSystemEditor.ExportTriggerTypes.Add(node.type);
            }
            foreach (var db in ms_vStepLists)
            {
                var node = BuildNodeAttr(db, "步骤器");
                node.previewEditor = false;
                GuideSystemEditor.StepTypes[node.type] = node;

                GuideSystemEditor.ExportTypesPop.Add(node.strShortName);
                GuideSystemEditor.ExportTypes.Add(node.type);
                GuideSystemEditor.NodeTypes[node.type] = node;
            }
            foreach (var db in ms_vExecuteLists)
            {
                var node = BuildNodeAttr(db, "执行器");
                GuideSystemEditor.ExcudeTypes[node.type] = node;

                GuideSystemEditor.ExportExcudeTypesPop.Add(node.strShortName);
                GuideSystemEditor.ExportExcudeTypes.Add(node.type);
                GuideSystemEditor.NodeTypes[node.type] = node;
            }
        }
        //-----------------------------------------------------
        static GuideSystemEditor.NodeAttr BuildNodeAttr(GuideCustomAgent.AgentUnit unit, string exprotName, bool bPreviewEditor = false)
        {
            GuideSystemEditor.NodeAttr node = new GuideSystemEditor.NodeAttr();
            node.type = (int)unit.customType;
            node.previewEditor = bPreviewEditor;

            node.strShortName = unit.name;
            node.strExportName = exprotName;

            node.strName = node.strExportName + "/" + node.strShortName;
            node.strQueueName = node.strName;

            node.argvs = new List<GuideSystemEditor.NodeAttr.ArgvAttr>();
            if (unit.inputs != null && unit.inputs.Length > 0)
            {
                for (int a = 0; a < unit.inputs.Length; ++a)
                {
                    GuideArgvAttribute newAttr = new GuideArgvAttribute();
                    if(GuideSystemEditor.DisplayTypes.TryGetValue(unit.inputs[a].displayType, out var attrType)) newAttr.displayType = attrType.displayType;
                    newAttr.bBit = unit.inputs[a].bBit;
                    newAttr.Flag = unit.inputs[a].Flag;
                    newAttr.argvName = unit.inputs[a].name;
                    newAttr.DisplayName = unit.inputs[a].name;

                    GuideSystemEditor.NodeAttr.ArgvAttr attr = new GuideSystemEditor.NodeAttr.ArgvAttr();
                    attr.attr = newAttr;
                    attr.bBit = attr.attr.bBit;
                    node.argvs.Add(attr);
                }
            }
            if (unit.outputs != null && unit.outputs.Length > 0)
            {
                for (int a = 0; a < unit.outputs.Length; ++a)
                {
                    GuideArgvAttribute newAttr = new GuideArgvAttribute();
                    if (GuideSystemEditor.DisplayTypes.TryGetValue(unit.outputs[a].displayType, out var attrType)) newAttr.displayType = attrType.displayType;
                    newAttr.bBit = unit.outputs[a].bBit;
                    newAttr.Flag = unit.outputs[a].Flag;
                    newAttr.argvName = unit.outputs[a].name;
                    newAttr.DisplayName = unit.outputs[a].name;

                    GuideSystemEditor.NodeAttr.ArgvAttr attr = new GuideSystemEditor.NodeAttr.ArgvAttr();
                    attr.attr = newAttr;
                    attr.bBit = attr.attr.bBit;
                    node.argvs.Add(attr);
                }
            }
            return node;
        }
        //-----------------------------------------------------
        internal static void RefreshData(List<GuideCustomAgent.AgentUnit> vTrigger, List<GuideCustomAgent.AgentUnit> vSteps, List<GuideCustomAgent.AgentUnit> vExecutes)
        {
            ms_vAgents = new Dictionary<uint, GuideCustomAgent.AgentUnit>();
            ms_vTriggerLists = new List<GuideCustomAgent.AgentUnit>();
            ms_vStepLists = new List<GuideCustomAgent.AgentUnit>();
            ms_vExecuteLists = new List<GuideCustomAgent.AgentUnit>();
            if (vTrigger != null)
            {
                foreach (var item in vTrigger)
                {
                    if (!item.IsValid()) continue;
                    if (!ms_vAgents.ContainsKey(item.customType))
                    {
                        ms_vAgents.Add(item.customType, item);
                        ms_vTriggerLists.Add(item);
                    }
                    else
                    {
                        Debug.LogError($"Custom event type {item.customType} already trigger exists");
                    }
                }
            }
            if (vSteps != null)
            {
                foreach (var item in vSteps)
                {
                    if (!item.IsValid()) continue;
                    if (!ms_vAgents.ContainsKey(item.customType))
                    {
                        ms_vAgents.Add(item.customType, item);
                        ms_vStepLists.Add(item);
                    }
                    else
                    {
                        Debug.LogError($"Custom clip type {item.customType} already step exists");
                    }
                }
            }
            if (vExecutes != null)
            {
                foreach (var item in vExecutes)
                {
                    if (!item.IsValid()) continue;
                    if (!ms_vAgents.ContainsKey(item.customType))
                    {
                        ms_vAgents.Add(item.customType, item);
                        ms_vExecuteLists.Add(item);
                    }
                    else
                    {
                        Debug.LogError($"Custom clip type {item.customType} already execute exists");
                    }
                }
            }
            if (ms_Agent == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:GuideCustomAgent");
                if(guids.Length>0)
                {
                    ms_Agent = AssetDatabase.LoadAssetAtPath<GuideCustomAgent>(AssetDatabase.GUIDToAssetPath(guids[0]));
                }
                if(ms_Agent==null)
                {
                    GuideCustomAgent customAgents = ScriptableObject.CreateInstance<GuideCustomAgent>();
                    string saveFile = EditorUtility.SaveFilePanel("保存自定义行为参数配置", Application.dataPath, "GuideCustomAgent", "asset");
                    saveFile = saveFile.Replace("\\", "/");
                    if (!string.IsNullOrEmpty(saveFile) && saveFile.StartsWith(Application.dataPath.Replace("\\", "/")))
                    {
                        saveFile = saveFile.Replace("\\", "/").Replace(Application.dataPath.Replace("\\", "/"), "Assets");
                        if (saveFile.StartsWith("/")) saveFile = saveFile.Substring(1);
                        AssetDatabase.CreateAsset(customAgents, saveFile);
                        AssetDatabase.SaveAssets();
                        ms_Agent = AssetDatabase.LoadAssetAtPath<GuideCustomAgent>(saveFile);
                    }
                }
            }
            if(ms_Agent!=null)
            {
                ms_Agent.vTriggerAgents = vTrigger.ToArray();
                ms_Agent.vStepAgents = vSteps.ToArray();
                ms_Agent.vExecuteAgents = vExecutes.ToArray();
                EditorUtility.SetDirty(ms_Agent);
                AssetDatabase.SaveAssetIfDirty(ms_Agent);
            }
            RefreshEditorData();
        }
        //-----------------------------------------------------
        public static void AddTrigger(GuideCustomAgent.AgentUnit unit)
        {
            Init();
            if (ms_vAgents.ContainsKey(unit.customType) || HasTrigger(unit.name))
            {
                Debug.LogError("Invalid or duplicate custom event unit.");
                return;
            }
            ms_vAgents.Add(unit.customType, unit);
            ms_vTriggerLists.Add(unit);
            if (ms_Agent != null)
            {
                ms_Agent.vTriggerAgents = ms_vTriggerLists.ToArray();
                EditorUtility.SetDirty(ms_Agent);
                AssetDatabase.SaveAssetIfDirty(ms_Agent);
            }
        }
        //-----------------------------------------------------
        public static void RemoveTrigger(uint customType)
        {
            Init();
            if (ms_vAgents.Remove(customType))
            {
                bool bFound = false;
                for (int i = 0; i < ms_vTriggerLists.Count; ++i)
                {
                    if (ms_vTriggerLists[i].customType == customType)
                    {
                        ms_vTriggerLists.RemoveAt(i);
                        bFound = true;
                        break;
                    }
                }
                if (bFound && ms_Agent != null)
                {
                    ms_Agent.vTriggerAgents = ms_vTriggerLists.ToArray();
                    EditorUtility.SetDirty(ms_Agent);
                    AssetDatabase.SaveAssetIfDirty(ms_Agent);
                }
            }
        }
        //-----------------------------------------------------
        public static void AddStep(GuideCustomAgent.AgentUnit unit)
        {
            Init();
            if (ms_vAgents.ContainsKey(unit.customType) || HasStep(unit.name))
            {
                Debug.LogError("Invalid or duplicate custom event unit.");
                return;
            }
            ms_vAgents.Add(unit.customType, unit);
            ms_vStepLists.Add(unit);
            if (ms_Agent != null)
            {
                ms_Agent.vStepAgents = ms_vStepLists.ToArray();
                EditorUtility.SetDirty(ms_Agent);
                AssetDatabase.SaveAssetIfDirty(ms_Agent);
            }
        }
        //-----------------------------------------------------
        public static void RemoveStep(uint customType)
        {
            Init();
            if (ms_vAgents.Remove(customType))
            {
                bool bFound = false;
                for (int i = 0; i < ms_vStepLists.Count; ++i)
                {
                    if (ms_vStepLists[i].customType == customType)
                    {
                        ms_vStepLists.RemoveAt(i);
                        bFound = true;
                        break;
                    }
                }
                if (bFound && ms_Agent != null)
                {
                    ms_Agent.vStepAgents = ms_vStepLists.ToArray();
                    EditorUtility.SetDirty(ms_Agent);
                    AssetDatabase.SaveAssetIfDirty(ms_Agent);
                }
            }
        }
        //-----------------------------------------------------
        public static void AddExecute(GuideCustomAgent.AgentUnit unit)
        {
            Init();
            if (ms_vAgents.ContainsKey(unit.customType) || HasExecute(unit.name))
            {
                Debug.LogError("Invalid or duplicate custom event unit.");
                return;
            }
            ms_vAgents.Add(unit.customType, unit);
            ms_vExecuteLists.Add(unit);
            if (ms_Agent != null)
            {
                ms_Agent.vExecuteAgents = ms_vExecuteLists.ToArray();
                EditorUtility.SetDirty(ms_Agent);
                AssetDatabase.SaveAssetIfDirty(ms_Agent);
            }
        }
        //-----------------------------------------------------
        public static void RemoveExecute(uint customType)
        {
            Init();
            if (ms_vAgents.Remove(customType))
            {
                bool bFound = false;
                for (int i = 0; i < ms_vExecuteLists.Count; ++i)
                {
                    if (ms_vExecuteLists[i].customType == customType)
                    {
                        ms_vExecuteLists.RemoveAt(i);
                        bFound = true;
                        break;
                    }
                }
                if (bFound && ms_Agent != null)
                {
                    ms_Agent.vExecuteAgents = ms_vExecuteLists.ToArray();
                    EditorUtility.SetDirty(ms_Agent);
                    AssetDatabase.SaveAssetIfDirty(ms_Agent);
                }
            }
        }
        //-----------------------------------------------------
        public static bool HasAgent(uint customType)
        {
            Init();
            return ms_vAgents.ContainsKey(customType);
        }
        //-----------------------------------------------------
        public static bool HasTrigger(string name)
        {
            Init();
            foreach (var item in ms_vTriggerLists)
            {
                if (item.name == name)
                    return true;
            }
            return false;
        }
        //-----------------------------------------------------
        public static bool HasStep(string name)
        {
            Init();
            foreach (var item in ms_vStepLists)
            {
                if (item.name == name)
                    return true;
            }
            return false;
        }
        //-----------------------------------------------------
        public static bool HasExecute(string name)
        {
            Init();
            foreach (var item in ms_vExecuteLists)
            {
                if (item.name == name)
                    return true;
            }
            return false;
        }
        //-----------------------------------------------------
        public static GuideCustomAgent.AgentUnit GetAgent(uint customType)
        {
            Init();
            if (ms_vAgents.TryGetValue(customType, out GuideCustomAgent.AgentUnit unit))
            {
                return unit;
            }
            return default;
        }
        //-----------------------------------------------------
        public static List<GuideCustomAgent.AgentUnit> GetTriggerList()
        {
            Init();
            return ms_vTriggerLists;
        }
        //-----------------------------------------------------
        public static List<GuideCustomAgent.AgentUnit> GetStepList()
        {
            Init();
            return ms_vStepLists;
        }
        //-----------------------------------------------------
        public static List<GuideCustomAgent.AgentUnit> GetExecuteList()
        {
            Init();
            return ms_vExecuteLists;
        }
    }
}

#endif