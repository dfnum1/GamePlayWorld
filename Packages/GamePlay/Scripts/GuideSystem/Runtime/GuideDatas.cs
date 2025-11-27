/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideData
作    者:	HappLI
描    述:	引导数据
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Guide
{
    [CreateAssetMenu]
    public class GuideDatas : ScriptableObject
    {
        static GuideDatas ms_Instacne = null;
        public TextAsset[] datas;
        Dictionary<int, GuideGroup> m_vDatas = new Dictionary<int, GuideGroup>();
        public Dictionary<int, GuideGroup>  allDatas
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
        public GuideGroup New()
        {
            if (m_vDatas == null) m_vDatas = new Dictionary<int, GuideGroup>();
            int guid = 0;
            foreach (var db in m_vDatas)
            {
                guid = Mathf.Max(db.Key, guid);
            }
            ++guid;
            string saveToPath = Framework.Guide.Editor.GuidePreferences.GetSettings().dataSavePath;
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
            if(string.IsNullOrEmpty(aiData.Name))
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

            Refresh(this);
        }
        //------------------------------------------------------
        public static void Refresh(GuideDatas aiObject = null)
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
            List<TextAsset> vLastAssets = null;
            int lastCnt = 0;
            if (aiObject.datas != null) vLastAssets = new List<TextAsset>(aiObject.datas);
            lastCnt = vLastAssets != null ? vLastAssets.Count : 0;
            List<TextAsset> vAssets = new List<TextAsset>();
            string saveToPath = Framework.Guide.Editor.GuidePreferences.GetSettings().dataSavePath;
            string[] files = System.IO.Directory.GetFiles(saveToPath, "*.json", System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; ++i)
            {
                string path = files[i].Replace("\\", "/").Replace(Application.dataPath, "Assets");
                if (path.Contains(".meta")) continue;
                TextAsset asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if (asset == null) continue;
                vAssets.Add(asset);
                if(vLastAssets!=null) vLastAssets.Remove(asset);
            }
            aiObject.datas = vAssets.ToArray();
            if(vLastAssets == null || vLastAssets.Count>0 || lastCnt != vAssets.Count)
            {
                UnityEditor.EditorUtility.SetDirty(aiObject);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(aiObject);
            }
            aiObject.Mapping();
        }
        //------------------------------------------------------
        public void CommitServer()
        {
            Refresh(this);
            List<string> files = new List<string>();
            string saveToPath = Framework.Guide.Editor.GuidePreferences.GetSettings().dataSavePath;
            files.Add(saveToPath);
            files.Add(UnityEditor.AssetDatabase.GetAssetPath(this));
         //   UnitySVN.SVNCommit(files.ToArray());
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
            if(controller.datas != null)
            {
                for (int i = 0; i < controller.datas.Length; ++i)
                {
                    if (controller.datas[i] == null)
                        GUI.color = Color.red;
                    else
                        GUI.color = color;
                    controller.datas[i] = UnityEditor.EditorGUILayout.ObjectField((i + 1).ToString(), controller.datas[i], typeof(TextAsset), false) as TextAsset;
                    GUI.color = color;
                }
            }

            OnInnerInspectorGUI();
            if (GUILayout.Button("刷新"))
            {
                GuideDatas.Refresh(controller);
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
