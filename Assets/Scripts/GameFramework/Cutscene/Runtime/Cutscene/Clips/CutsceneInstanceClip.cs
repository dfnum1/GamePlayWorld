#if USE_CUTSCENE
/********************************************************************
生成日期:	07:22:2025
类    名: 	CutsceneInstancePrefabClip
作    者:	HappLI
描    述:	实例化预制体剪辑
*********************************************************************/
using UnityEngine;
using Framework.Base;
using Framework.Core;
using Framework.CutsceneAT.Runtime;


#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.Cutscene.Runtime
{
    [System.Serializable, CutsceneClip("实例化预制体Clip")]
    public class CutsceneInstancePrefabClip : IBaseClip
    {
        [Display("基本属性")]public BaseClipProp baseProp;
        [Display("识别ID")] public int objId;
        [Display("预制体"), StringViewGUI(typeof(GameObject))] public string prefabName;
        [Display("位置")] public Vector3 position;
        [Display("角度")] public Vector3 eulerAngle;
        [Display("缩放")] public Vector3 scale = Vector3.one;
        [Display("异步加载")] public bool asyncLoad = true;
        //-----------------------------------------------------
        public ACutsceneDriver CreateDriver()
        {
            return new CutsceneInstanceDriver();
        }
        //-----------------------------------------------------
        public ushort GetIdType()
        {
            return (ushort)EClipType.eInstancePrefab;
        }
        //-----------------------------------------------------
        public float GetDuration()
        {
            return baseProp.duration;
        }
        //-----------------------------------------------------
        public EClipEdgeType GetEndEdgeType()
        {
            return baseProp.endEdgeType;
        }
        //-----------------------------------------------------
        public string GetName()
        {
            return baseProp.name;
        }
        //-----------------------------------------------------
        public ushort GetRepeatCount()
        {
            return baseProp.repeatCnt;
        }
        //-----------------------------------------------------
        public float GetTime()
        {
            return baseProp.time;
        }
        //-----------------------------------------------------
        public float GetBlend(bool bIn)
        {
            return baseProp.GetBlend(bIn);
        }
#if UNITY_EDITOR
        //-----------------------------------------------------
        [AddInspector]
        public void OnEditor()
        {
            if (baseProp.ownerTrackObject != null)
            {
                var binder = baseProp.ownerTrackObject.GetBindLastCutsceneObject();
                if (binder != null)
                {
                    var transform = binder.GetUniyTransform();
                    if(transform!=null)
                    {
                        if (GUILayout.Button("设置当前位置信息"))
                        {
                            position = transform.position;
                            eulerAngle = transform.eulerAngles;
                            scale = transform.localScale;
                        }
                    }
                    if(GUILayout.Button("定位选择"))
                    {
                        Selection.activeObject = binder.GetUniyObject();
                        if(SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.LookAt(transform.position);
                    }
                }
            }
        }
        //-----------------------------------------------------
        void SetDefault()
        {
            this.objId = this.GetHashCode();
            if (Camera.main == null)
                return;
            // 获取视野中心点与地表 y=0 的交点
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            float t = 0f;
            if (ray.direction.y != 0f)
            {
                t = -ray.origin.y / ray.direction.y;
            }
            Vector3 groundPos = ray.origin + ray.direction * t;

            position = groundPos;
        }
#endif
    }
    //-----------------------------------------------------
    internal class CutsceneInstanceDriver : ACutsceneDriver, ICutsceneObject
    {
        private AInstanceAble m_Instance;
        private Animator m_pAnimator;
        private Vector3 m_BindOffset;
        private Vector3 m_BindRotate;
        private Vector3 m_Scale;
        private bool m_bEntered = false;
        //-----------------------------------------------------
        public override void OnDestroy()
        {
            RemoveObject(this);
            DestroyInstance();
        }
        //-----------------------------------------------------
        public override void OnSpawnInstance(AInstanceAble pInstance)
        {
            if (pInstance == m_Instance)
                return;
            m_Instance = pInstance;
            m_Instance.SetPosition(m_BindOffset);
            m_Instance.SetScale(m_Scale);
            m_Instance.SetEulerAngle(m_BindRotate);
        }
        //-----------------------------------------------------
        void OnSign(InstanceOperiaon op)
        {
            op.bUsed = !IsDestroyed() && m_bEntered;
        }
        //-----------------------------------------------------
        public override bool OnClipEnter(CutsceneTrack pTrack, FrameData clip)
        {
            m_bEntered = false;
            CutsceneInstancePrefabClip parClip = clip.clip.Cast<CutsceneInstancePrefabClip>();
            if (parClip == null || string.IsNullOrEmpty(parClip.prefabName))
                return false;

            m_bEntered = true;
            m_BindRotate = parClip.eulerAngle;
            m_BindOffset = parClip.position;
            m_Scale = parClip.scale;
            SpawnInstance(parClip.prefabName);
            /*if(parClip.objId!=0) */pTrack.BindTrackData(new ObjId(parClip.objId), this);
            return true;
        }
        //-----------------------------------------------------
        public override bool OnClipLeave(CutsceneTrack pTrack, FrameData clip)
        {
            if (clip.CanRestore() || clip.IsLeaveIn())
            {
                pTrack.RemoveObject(this);
                DestroyInstance();
            }
            return true;
        }
        //-----------------------------------------------------
        private void DestroyInstance()
        {
            if (m_Instance != null)
            {
                DespawnInstance(m_Instance);
                m_Instance = null;
            }
            m_pAnimator = null;
        }
        //-----------------------------------------------------
        public Object GetUniyObject()
        {
            return m_Instance;
        }
        //-----------------------------------------------------
        public Transform GetUniyTransform()
        {
            if (m_Instance == null) return null;
            return m_Instance.transform;
        }
        //-----------------------------------------------------
        public Animator GetAnimator()
        {
            if(m_pAnimator == null && m_Instance!=null)
            {
                m_pAnimator = m_Instance.GetComponent<Animator>();
            }
            return m_pAnimator;
        }
        //-----------------------------------------------------
        public Camera GetCamera()
        {
            return null;
        }
        //-----------------------------------------------------
        public bool SetParameter(EParamType type, CutsceneParam paramData)
        {
            if (m_Instance == null)
                return false;
            switch (type)
            {
                case EParamType.ePosition: m_Instance.transform.position = paramData.ToVector3(); return true;
                case EParamType.eEulerAngle: m_Instance.transform.eulerAngles = paramData.ToVector3(); return true;
                case EParamType.eQuraternion: m_Instance.transform.rotation = paramData.ToQuaternion(); return true;
                case EParamType.eScale: m_Instance.transform.localScale = paramData.ToVector3(); return true;
            }
            return false;
        }
        //-----------------------------------------------------
        public bool GetParameter(EParamType type, ref CutsceneParam paramData)
        {
            if (m_Instance == null)
                return false;
            switch (type)
            {
                case EParamType.ePosition: paramData.SetVector3(m_Instance.transform.position); return true;
                case EParamType.eEulerAngle: paramData.SetVector3(m_Instance.transform.eulerAngles); return true;
                case EParamType.eQuraternion: paramData.SetQuaternion(m_Instance.transform.rotation); return true;
                case EParamType.eScale: paramData.SetVector3(m_Instance.transform.localScale); return true;
            }
            return false;
        }
        //-----------------------------------------------------
        public void Destroy()
        {
        }
    }
}
#endif