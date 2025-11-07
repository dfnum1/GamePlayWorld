#if !USE_SERVER
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ShadersTabs
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.Core
{
   // [CreateAssetMenu]
    public abstract class AInstancesLimit : ScriptableObject
    {
        [System.Serializable]
        public struct Limit
        {
            public int instnaceID;
            public string file;
            public short cnt;
        }

        [System.Serializable]
        public struct PermanentAB
        {
            public string abName;
        }

        static AInstancesLimit ms_Instance = null;

        public Limit[] limits;
        public PermanentAB[] permaentABs;
        Dictionary<string, short> m_vLimits;
        Dictionary<int, short> m_vInstanceLimits;
        HashSet<string> m_vPermanetABs = null;
        private void OnEnable()
        {
            ms_Instance = this;
            Refresh();
        }
        //------------------------------------------------------
        public void Refresh()
        {
            if (m_vInstanceLimits != null)
                m_vInstanceLimits.Clear();
            if (m_vLimits != null)
                m_vLimits.Clear();

            if (m_vPermanetABs != null) m_vPermanetABs.Clear();

            if (limits != null && limits.Length > 0)
            {
                if(m_vInstanceLimits == null)
                    m_vInstanceLimits = new Dictionary<int, short>(limits.Length);
                if(m_vLimits == null)
                    m_vLimits = new Dictionary<string, short>(limits.Length);
                for (int i = 0; i < limits.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(limits[i].file))
                        m_vLimits[limits[i].file] = limits[i].cnt;
                    if (limits[i].instnaceID != 0)
                        m_vInstanceLimits[limits[i].instnaceID] = limits[i].cnt;
                }
            }

            if(permaentABs!=null && permaentABs.Length>0)
            {
                m_vPermanetABs = new HashSet<string>();
                for (int i = 0; i < permaentABs.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(permaentABs[i].abName))
                        m_vPermanetABs.Add(permaentABs[i].abName);
                }
            }
        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            m_vLimits = null;
            m_vInstanceLimits = null;
            ms_Instance = null;
        }
        //------------------------------------------------------
        public static bool IsValid()
        {
            return ms_Instance != null;
        }
        //------------------------------------------------------
        public static bool IsPermanentAB(string strAB)
        {
            if (ms_Instance == null || ms_Instance.m_vPermanetABs == null || string.IsNullOrEmpty(strAB)) return false;
            return ms_Instance.m_vPermanetABs.Contains(strAB);
        }
        //------------------------------------------------------
        public static bool CanInstnace(string strFile)
        {
            if (ms_Instance == null || ms_Instance.m_vLimits == null) return true;
            if (string.IsNullOrEmpty(strFile)) return false;

            short limit;
            if(ms_Instance.m_vLimits.TryGetValue(strFile, out limit))
            {
                if(limit>0)
                    return limit > FileSystemUtil.StatsInstanceCount(strFile);
            }
            return true;
        }
        //------------------------------------------------------
        public static bool CanInstnace(int instanceID)
        {
            if (ms_Instance == null || ms_Instance.m_vInstanceLimits == null) return true;
            if (instanceID == 0) return false;

            short limit;
            if (ms_Instance.m_vInstanceLimits.TryGetValue(instanceID, out limit) )
            {
                if (limit > 0)
                    return limit > FileSystemUtil.StatsInstanceCount(instanceID);
            }
            return true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AInstancesLimit), true)]
    [CanEditMultipleObjects]
    public class InstancesLimitEditor : Editor
    {
    //    bool m_bEpxandAssets = false;
     //   bool m_bExpandPaths = false;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Color colro = GUI.color;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;
            AInstancesLimit assets = target as AInstancesLimit;
            if(assets.limits!=null)
            {
                for (int i = 0; i < assets.limits.Length; ++i)
                {
                    AInstancesLimit.Limit item = assets.limits[i];
                    GUILayout.BeginHorizontal();
                    GameObject pAsset= AssetDatabase.LoadAssetAtPath<GameObject>(item.file);
                    if (pAsset == null)
                    {
                        GUI.color = Color.red;
                    }
                    else
                        GUI.color = colro;
                    pAsset = EditorGUILayout.ObjectField("资源[" + i + "]", pAsset, typeof(UnityEngine.GameObject), false) as GameObject;
                    if (pAsset != null)
                    {
                        item.file = AssetDatabase.GetAssetPath(pAsset);
                        item.instnaceID = pAsset.GetInstanceID();
                    }
                    else
                    {
                        item.instnaceID = 0;
                        item.file = null;
                    }
                    item.cnt = (short)EditorGUILayout.IntField(item.cnt);
                    GUI.color = colro;
                    if(GUILayout.Button("删除", new GUILayoutOption[] { GUILayout.Width(50) }))
                    {
                        List<AInstancesLimit.Limit> temsps = new List<AInstancesLimit.Limit>(assets.limits);
                        temsps.RemoveAt(i);
                        assets.limits = temsps.ToArray();
                        break;
                    }
                    assets.limits[i] = item;
                    GUILayout.EndHorizontal();
                }
            }
            if (GUILayout.Button("添加"))
            {
                List<AInstancesLimit.Limit> temsps = assets.limits != null ? new List<AInstancesLimit.Limit>(assets.limits) : new List<AInstancesLimit.Limit>();
                temsps.Add(new AInstancesLimit.Limit() { });
                assets.limits = temsps.ToArray();
            }

            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("刷新保存"))
            {
                assets.Refresh();
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            EditorGUIUtility.labelWidth = labelWidth;

        }
    }
#endif
}
#endif