/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ParticleDiffChecker
作    者:	HappLI
描    述:	拖尾位置变化检测器
*********************************************************************/
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace Framework.Core
{
    public class ParticleDiffChecker : MonoBehaviour
    {
        public float diffDistance = 100;
        public TrailRenderer[] arrTrails = null;
        public ParticleSystem[] arrSystems = null;
        bool m_bDirtry = false;
        Vector3 m_Position = Vector3.zero;
        Transform m_pTransfrom;
        //------------------------------------------------------
        protected void Awake()
        {
            m_pTransfrom = transform;
#if UNITY_EDITOR
            if (arrTrails == null)
                arrTrails = gameObject.GetComponentsInChildren<TrailRenderer>();
            if(arrSystems == null)
                arrSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
#endif
            m_Position = m_pTransfrom.position;
        }
        //------------------------------------------------------
        void ResetDirty()
        {
            if (!m_bDirtry) return;
            if (arrTrails != null)
            {
                for (int i = 0; i < arrTrails.Length; ++i)
                    arrTrails[i].Clear();
            }
            if (arrSystems != null)
            {
                for (int i = 0; i < arrSystems.Length; ++i)
                {
                    if (arrSystems[i] == null) continue;
                    arrSystems[i].time = 0;
                    arrSystems[i].Clear(true);
                }
            }
            m_bDirtry = false;
        }
        //------------------------------------------------------
        private void Update()
        {
            if (arrTrails == null || arrTrails.Length <= 0 || m_pTransfrom == null) return;
            ResetDirty();
            if(!BaseUtil.Equal(m_Position,m_pTransfrom.position, diffDistance) )
            {
                m_Position = m_pTransfrom.position;
                m_bDirtry = true;
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ParticleDiffChecker))]
    [CanEditMultipleObjects]
    public class ParticleTailCheckerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            ParticleDiffChecker checker = target as ParticleDiffChecker;
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("刷新保存"))
            {
                checker.arrTrails = checker.GetComponentsInChildren<TrailRenderer>();
                checker.arrSystems = checker.GetComponentsInChildren<ParticleSystem>();

                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
#endif
}
