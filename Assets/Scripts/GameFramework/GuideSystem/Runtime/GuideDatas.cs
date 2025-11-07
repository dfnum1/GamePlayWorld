#if USE_GUIDESYSTEM
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideData
作    者:	HappLI
描    述:	引导数据
*********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Framework.Plugin.Guide
{
    [CreateAssetMenu(menuName="GameData/GuideDatas"), Data.PublishRefresh("Refresh")]
    public class GuideDatas : ScriptableObject
    {
        static GuideDatas ms_Instacne = null;
        public TextAsset[] datas;
        Dictionary<int, GuideGroup> m_vDatas = new Dictionary<int, GuideGroup>();
        internal Dictionary<int, GuideGroup>  allDatas
        {
            get
            {
                return m_vDatas;
            }
        }
        //------------------------------------------------------
        public static Dictionary<int, GuideGroup> AllDatas
        {
            get
            {
                if (ms_Instacne != null) return ms_Instacne.allDatas;
                return null;
            }
        }
        //------------------------------------------------------
        public static void InitData(bool bForce = false)
        {
            if (ms_Instacne == null) return;
            ms_Instacne.Init(bForce);
        }
        //------------------------------------------------------
        public static GuideGroup GetGuideGroup(int id)
        {
            if (ms_Instacne == null) return null;
            return ms_Instacne.GetGuide(id);
        }
        //------------------------------------------------------
        public GuideGroup GetGuide(int id)
        {
            if (ms_Instacne == null) return null;
            GuideGroup group;
            if (ms_Instacne.m_vDatas.TryGetValue(id, out group))
                return group;
            return null;
        }
        //------------------------------------------------------
        void OnEnable()
        {
            ms_Instacne = this;
            Mapping();
        }
        //------------------------------------------------------
        public void Mapping()
        {
            if (datas == null) return;
            m_vDatas.Clear();
            try
            {
                for (int i = 0; i < datas.Length; ++i)
                {
                    if (datas[i] != null)
                    {
                        GuideGroup ai = UnityEngine.JsonUtility.FromJson<GuideGroup>(datas[i].text);
                        if (!m_vDatas.ContainsKey(ai.Guid))
                        {
#if UNITY_EDITOR
                            ai.strFile = UnityEditor.AssetDatabase.GetAssetPath(datas[i]);
#endif
                            m_vDatas.Add(ai.Guid, ai);
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(ex.ToString());
            }
        }
        //------------------------------------------------------
        public void Init(bool bForceInit=false)
        {
            if(m_vDatas.Count<=0)
            {
                Mapping();
            }
            foreach (var db in m_vDatas)
            {
                db.Value.Init(bForceInit);
            }
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public string GetSaveRoot()
        {
            string root = UnityEditor.AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(root))
            {
                Debug.LogError("找不到引导数据配置存储路径");
                return null;
            }
            root = Path.GetDirectoryName(root);
            return root.Replace("\\", "/");
        }
        //------------------------------------------------------
        public GuideGroup New()
        {
            string saveToPath = GetSaveRoot();
            if (saveToPath == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("提示", "找不到引导数据配置存储路径", "好的");
                return null;
            }
            if (m_vDatas == null) m_vDatas = new Dictionary<int, GuideGroup>();
            int guid = 0;
            foreach (var db in m_vDatas)
            {
                guid = Mathf.Max(db.Key, guid);
            }
            ++guid;
            string strFile = UnityEditor.EditorUtility.SaveFilePanel("Guide", saveToPath, guid.ToString(), "json");
            strFile = strFile.Replace("\\", "/").Replace(Application.dataPath + "/", "Assets/");
            if (!strFile.Contains(saveToPath))
            {
                UnityEditor.EditorUtility.DisplayDialog("提示", "必须保存在\r\n" + saveToPath + "\r\n 目录中", "好的");
                return null;
            }

            GuideGroup aiData = new GuideGroup();
            aiData.Guid = guid;
            aiData.strFile = strFile;
            aiData.Name = System.IO.Path.GetFileNameWithoutExtension(strFile);

            m_vDatas.Add(guid, aiData);

            return aiData;
        }
        //------------------------------------------------------
        public void Save(bool bSaveAll = false)
        {
            if (bSaveAll)
            {
                if (m_vDatas != null)
                {
                    foreach (var db in m_vDatas)
                    {
                        db.Value.Save();
                    }
                }
            }

            RefreshDatas(this);
        }
        //------------------------------------------------------
        void Refresh()
        {
            string saveToPath = GetSaveRoot();
            if (saveToPath == null)
            {
                return;
            }

            List<TextAsset> vAssets = new List<TextAsset>();
            string[] files = System.IO.Directory.GetFiles(saveToPath, "*.json", System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; ++i)
            {
                string path = files[i].Replace("\\", "/").Replace(Application.dataPath, "Assets");
                if (path.Contains(".meta")) continue;
                TextAsset asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if (asset == null) continue;
                vAssets.Add(asset);
            }
            this.datas = vAssets.ToArray();
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
            this.Mapping();
        }
        //------------------------------------------------------
        public static void RefreshDatas(GuideDatas aiObject = null)
        {
            if (aiObject == null)
            {
                string[] aiDatas = UnityEditor.AssetDatabase.FindAssets("t:GuideDatas");
                if (aiDatas != null && aiDatas.Length > 0)
                {
                    aiObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GuideDatas>(UnityEditor.AssetDatabase.GUIDToAssetPath(aiDatas[0]));
                }
            }

            if (aiObject == null) return;
            aiObject.Refresh();
        }
        //------------------------------------------------------
        public void CommitServer()
        {
            RefreshDatas(this);
            string saveToPath = GetSaveRoot();
            if (saveToPath == null)
                return;
            List<string> files = new List<string>();
            files.Add(saveToPath);
            files.Add(UnityEditor.AssetDatabase.GetAssetPath(this));
            UnitySVN.SVNCommit(files.ToArray());
        }
#endif
    }

#if UNITY_EDITOR
    [UnityEditor.CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(GuideDatas), true)]
    public class GuideDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GuideDatas controller = target as GuideDatas;
            Color color = GUI.color;
            for (int i = 0; i < controller.datas.Length; ++i)
            {
                if (controller.datas[i] == null)
                    GUI.color = Color.red;
                else
                    GUI.color = color;
                controller.datas[i] = UnityEditor.EditorGUILayout.ObjectField((i + 1).ToString(), controller.datas[i], typeof(TextAsset),false) as TextAsset;
                GUI.color = color;
            }
            OnInnerInspectorGUI();
            if (GUILayout.Button("刷新"))
            {
                GuideDatas.RefreshDatas(controller);
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
            }
            if (GUILayout.Button("提交"))
            {
                controller.CommitServer();
            }
            serializedObject.ApplyModifiedProperties();
        }
        //------------------------------------------------------
        protected virtual void OnInnerInspectorGUI() { }
    }
#endif
}
#endif