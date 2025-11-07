/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	AInstanceAble
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Collections.Generic;
using Framework.Base;
using UnityEngine;
namespace Framework.Core
{
    //------------------------------------------------------
    public enum EInstanceCallbackType
    {
        Enable,
        Disiable,
        Recyled,
        Destroy,
        ColliderEnter,
        ColliderStay,
        ColliderExit,
    }
    //------------------------------------------------------
    public interface IInstanceAbleCallback
    {
        void OnInstanceCallback(AInstanceAble pAble, EInstanceCallbackType eType);
    }
    //------------------------------------------------------
    public class MaterialMapper
    {
        static Dictionary<Material, Material> m_vMapping = new Dictionary<Material, Material>(16);
        public static void Mapping(Material srcMat, Material newMat)
        {
            m_vMapping[srcMat] = newMat;
        }
        //------------------------------------------------------
        public static Material GetMaterial(Material srcMat)
        {
            Material newMat;
            if (m_vMapping.TryGetValue(srcMat, out newMat))
                return newMat;
            return null;
        }
    }
    //------------------------------------------------------
    [HideInInspector]
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class AInstanceAble : MonoBehaviour , IUserData//LinkListBehaviour<AInstanceAble>
    {
        public static Action<string, GameObject, AInstanceAble> OnRealDestroyLinster;
        public static Action<string, GameObject, AInstanceAble> OnDestroyLinster;
        public static Action<string, GameObject, AInstanceAble> OnRecyleLinster;
        public static Action<string, GameObject, AInstanceAble> OnPoolStartLinster;
        protected Transform m_pTransform;
        GameObject m_pObject;
        Asset m_BindAsset = null;

        public bool allowChangeLayer = true;
        [HideInInspector]
        [System.NonSerialized]
        internal float lastUseTime = 0;

        GameObject m_pPrefab = null;
        bool m_bActive = true;
        bool m_bRecyled = true;

        private List<Renderer> m_pRenders;
        private List<LibRender> m_pLibRenders;

        private int m_nDefaultLayerFlag = 0;

        private List<IInstanceAbleCallback> m_vCallbacks;

        private Dictionary<int, Component> m_vComponents = null;

        private int m_nDelayDestroyRecyleMaxCount = 2;
        private float m_fDelayDestroy = 0;
        //------------------------------------------------------
        public void RegisterCallback(IInstanceAbleCallback callback)
        {
            if (m_vCallbacks == null) m_vCallbacks = new List<IInstanceAbleCallback>(2);
            if (m_vCallbacks.Contains(callback)) return;
            m_vCallbacks.Add(callback);
        }
        //------------------------------------------------------
        public void UnRegisterCallback(IInstanceAbleCallback callback)
        {
            if (m_vCallbacks == null) return;
            m_vCallbacks.Remove(callback);
        }
        //------------------------------------------------------
        public virtual bool CanRecyle()
        {
            return true;
        }
        //------------------------------------------------------
        protected virtual void Awake()
        {
            m_pTransform = transform;
            m_pObject = gameObject;
            m_bActive = true;
            m_bRecyled = false;
            ClearMaterialBlock();
            ResetDelayDestroyParam();
            m_nDefaultLayerFlag = gameObject.layer;
       //     base.OnAwake();
        }
        //------------------------------------------------------
        protected virtual void OnEnable()
        {
            if (m_vCallbacks == null) return;
            for (int i = 0; i < m_vCallbacks.Count; ++i)
                m_vCallbacks[i].OnInstanceCallback(this,EInstanceCallbackType.Enable);
        }
        //------------------------------------------------------
        protected virtual void OnDisable()
        {
            if (m_vCallbacks == null) return;
            for (int i = 0; i < m_vCallbacks.Count; ++i)
                m_vCallbacks[i].OnInstanceCallback(this, EInstanceCallbackType.Disiable);
        }
        //------------------------------------------------------
        protected virtual void OnDestroy()
        {
            if (OnRealDestroyLinster != null)
                OnRealDestroyLinster((m_BindAsset != null) ? m_BindAsset.Path : null, m_pPrefab, this);
            m_pTransform = null;
            m_pObject = null;
            if (m_pLibRenders != null)
            {
                for (int i = 0; i < m_pLibRenders.Count; ++i)
                {
                    m_pLibRenders[i].Restore(true);
                    MaterialLibrarys.RecycleLibray(m_pLibRenders[i]);
                }
            }
            m_pLibRenders = null;
            if (m_BindAsset != null)
            {
                m_BindAsset.Release();
                m_BindAsset = null;
            }
            m_pPrefab = null;
            lastUseTime = 0;
            m_vComponents = null;
            ResetDelayDestroyParam();
            if (m_vCallbacks != null)
            {
                for (int i = 0; i < m_vCallbacks.Count; ++i)
                {
                    m_vCallbacks[i].OnInstanceCallback(this, EInstanceCallbackType.Destroy);
                }
                m_vCallbacks.Clear();
            }
        }
        //------------------------------------------------------
        private void RefreshComponents()
        {
        }
        //------------------------------------------------------
        public virtual void OnRecyle()
        {
            if (OnRecyleLinster != null)
                OnRecyleLinster((m_BindAsset != null) ? m_BindAsset.Path : null, m_pPrefab, this);

            ResetDelayDestroyParam();
            lastUseTime = Time.time;
            m_bActive = true;
            m_bRecyled = true;
            SetUnActive();
            ClearMaterialBlock();
            ResetLayer();
            if (m_vCallbacks != null)
            {
                for(int i = 0; i < m_vCallbacks.Count; ++i)
                {
                    m_vCallbacks[i].OnInstanceCallback(this, EInstanceCallbackType.Recyled);
                }
                m_vCallbacks.Clear();
            }
            RefreshComponents();
        }
        //------------------------------------------------------
        public virtual void OnPoolReady()
        {
            ClearMaterialBlock();
            ResetDelayDestroyParam();
            m_bRecyled = false;
            ResetLayer();
        }
        //------------------------------------------------------
        public virtual void OnPoolStart()
        {
            ResetDelayDestroyParam();
            if (OnPoolStartLinster != null)
                OnPoolStartLinster((m_BindAsset != null) ? m_BindAsset.Path : null, m_pPrefab, this);
        }
        //------------------------------------------------------
        void OnPoolAwake()
        {
            ResetDelayDestroyParam();
            if (m_nDefaultLayerFlag == 0)
                m_nDefaultLayerFlag = gameObject.layer;
            ClearMaterialBlock();
            ResetLayer();
            RefreshComponents();
        }
        //------------------------------------------------------
        public Transform GetTransorm()
        {
            if (m_pTransform == null) m_pTransform = transform;
            return m_pTransform;
        }
        //------------------------------------------------------
        public GameObject GetObject()
        {
            return m_pObject;
        }
        //------------------------------------------------------
        public string AssetFile
        {
            get
            {
                if (m_BindAsset == null) return null;
                return m_BindAsset.Path;
            }
        }
        //------------------------------------------------------
        public GameObject Prefab
        {
            get
            {
                return m_pPrefab;
            }
        }
        //------------------------------------------------------
        public int InstanceID
        {
            get
            {
                if (m_pPrefab) return m_pPrefab.GetInstanceID();
                else if (m_BindAsset != null ) m_BindAsset.GetInstanceID();
                return 0;
            }
        }
        //------------------------------------------------------
        public virtual void RecyleDestroy(int recyleMax = 2)
        {
#if UNITY_EDITOR
            if(!Application.isPlaying || !AFramework.isStartup)
            {
                RealDestroy();
                return;
            }
#endif
            if(this.m_BindAsset ==null && m_pPrefab == null )
            {
                RealDestroy();
                return;
            }
            ResetDelayDestroyParam();
#if !USE_SERVER
            FileSystemUtil.DeSpawnInstance(this, recyleMax);
#endif
        }
        //------------------------------------------------------
        public void ResetDelayDestroyParam()
        {
            m_fDelayDestroy = 0;
            m_nDelayDestroyRecyleMaxCount = 2;
        }
        //------------------------------------------------------
        public virtual void DelayDestroy(float fDelay,int recyleMax = 2)
        {
            if (fDelay <= 0)
            {
                RecyleDestroy(recyleMax);
                return;
            }
            if (m_bRecyled) return;
//#if !USE_SERVER
//            SetParent(ARootsHandler.ActorsRoot);
//#endif
            m_fDelayDestroy = Time.time + fDelay;
        }
        //------------------------------------------------------
        internal void RealDestroy()
        {
            if (OnDestroyLinster != null)
                OnDestroyLinster((m_BindAsset != null) ? m_BindAsset.Path : null, m_pPrefab, this);

            //    if (m_BindAsset != null) m_BindAsset.Release();
            //    m_BindAsset = null;
            //    m_pPrefab = null;
            ClearMaterialBlock();
            ResetDelayDestroyParam();

            if (gameObject)
            {
                //if (m_BindAsset != null)
                //{
                //    m_BindAsset.Release();
                //    m_BindAsset = null;
                //}
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    GameObject.DestroyImmediate(gameObject);
                else
                    GameObject.Destroy(gameObject);
#else
                GameObject.Destroy(gameObject);
#endif
            }
        }
        //------------------------------------------------------
        public virtual void Destroy()
        {
            RecyleDestroy();
        }
        //------------------------------------------------------
        public void SetBindAsset(Asset pBindAsset)
        {
            if (m_BindAsset !=null && m_BindAsset == pBindAsset)
            {
                if (m_BindAsset.RefCnt <= 0)
                    m_BindAsset.Grab();
                OnPoolAwake();
                return;
            }

            if (m_BindAsset != null)m_BindAsset.Release();

            if (pBindAsset != null) pBindAsset.Grab();

            m_BindAsset = pBindAsset;
            m_pPrefab = null;
            OnPoolAwake();
        }
        //------------------------------------------------------
        public void SetBindPrefab(GameObject pPrefab)
        {
            if (m_pPrefab == pPrefab)
            {
                OnPoolAwake();
                return;
            }
            if (m_BindAsset != null) m_BindAsset.Release();

            m_pPrefab = pPrefab;
            OnPoolAwake();
        }
        //------------------------------------------------------
        public bool IsRecyled()
        {
            return m_bRecyled;
        }
        //------------------------------------------------------
        public T GetComponent<T>(Type type) where T : MonoBehaviour
        {
            if (m_pObject == null) return null;
            return m_pObject.GetComponent(type) as T;
        }
        //------------------------------------------------------
        public List<Renderer> GetRenders()
        {
            if(m_pRenders == null)
            {
                Renderer[] renders = GetComponentsInChildren<Renderer>();
                m_pRenders = new List<Renderer>(1);
                m_pLibRenders = new List<LibRender>(1);
                if (renders != null)
                {
                    for (int i = 0; i < renders.Length; ++i)
                    {
                        if (renders[i] is SkinnedMeshRenderer || renders[i] is MeshRenderer)
                        {
                            m_pRenders.Add(renders[i]);
                            LibRender lib = MaterialLibrarys.NewLibray();
                            lib.Backup(renders[i]);
                            m_pLibRenders.Add(lib);
                        }
                    }
                }
            }
            return m_pRenders;
        }
        //------------------------------------------------------
        public Bounds GetRenderBounds()
        {
            GetRenders();
            Bounds bound = new Bounds();
            if (m_pRenders!=null && m_pRenders.Count>0)
            {
                bound.min = Vector3.one * 999;
                bound.max = -Vector3.one * 999;

                for (int i = 0; i < m_pRenders.Count; ++i)
                {
                    bound.min = Vector3.Min(m_pRenders[i].bounds.min, bound.min);
                    bound.max = Vector3.Max(m_pRenders[i].bounds.max, bound.max);
                }
                return bound;
            }
            return bound;
        }
        //------------------------------------------------------
        void ModifyRenders()
        {
            GetRenders();
            if (m_pLibRenders == null) return;
            LibRender lib;
            for (int i = 0; i < m_pLibRenders.Count; ++i)
            {
                lib = m_pLibRenders[i];
                lib.CheckRuntime();
            }
        }
        //------------------------------------------------------
        public void SetPosition(Vector3 postion, bool bLocal = false)
        {
            if(bLocal)
                GetTransorm().localPosition = postion;
            else
                GetTransorm().position = postion;
        }
        //------------------------------------------------------
        public Vector3 GetPosition(bool bLocal = false)
        {
            if (bLocal)
                return GetTransorm().localPosition;
            else
                return GetTransorm().position;
        }
        //------------------------------------------------------
        public void SetRotation(Quaternion rot, bool bLocal = false)
        {
            if (bLocal)
                GetTransorm().rotation = rot;
            else
                GetTransorm().localRotation = rot;
        }
        //------------------------------------------------------
        public Quaternion GetRotation(bool bLocal = false)
        {
            if (bLocal)
                return GetTransorm().localRotation;
            else
                return GetTransorm().rotation;
        }
        //------------------------------------------------------
        public void SetForward(Vector3 forward)
        {
            GetTransorm().forward = forward;
        }
        //------------------------------------------------------
        public Vector3 GetForward()
        {
            return GetTransorm().forward;
        }
        //------------------------------------------------------
        public void SetUp(Vector3 up)
        {
            GetTransorm().up = up;
        }
        //------------------------------------------------------
        public Vector3 GetUp()
        {
            return GetTransorm().up;
        }
        //------------------------------------------------------
        public void SetEulerAngle(Vector3 eulerAngles, bool bLocal = false)
        {
            if (bLocal)
                GetTransorm().localEulerAngles = eulerAngles;
            else
                GetTransorm().eulerAngles = eulerAngles;
        }
        //------------------------------------------------------
        public Vector3 GetEulerAngle(bool bLocal = false)
        {
            if (bLocal)
                return GetTransorm().localEulerAngles;
            else
                return GetTransorm().eulerAngles;
        }
        //------------------------------------------------------
        public void SetScale(Vector3 scale)
        {
            GetTransorm().localScale = scale;
        }
        //------------------------------------------------------
        public Vector3 GetScale(bool bLocal = true)
        {
            if (bLocal) return GetTransorm().localScale;
            return GetTransorm().lossyScale;
        }
        //------------------------------------------------------
        public void Reset(bool bScale=false)
        {
            GetTransorm().position = Vector3.zero;
            if (bScale) GetTransorm().localScale = Vector3.one;
            GetTransorm().eulerAngles = Vector3.zero;
        }
        //------------------------------------------------------
        public virtual void SetActive()
        {
            if (m_bActive) return;
            GetTransorm().position = Vector3.zero;
            m_bActive = true;
        }
        //------------------------------------------------------
        public void SetUnActive()
        {
             if (!m_bActive) return;
            GetTransorm().position = ConstDef.INVAILD_POS;
            m_bActive = false;
        }
        //------------------------------------------------------
        public bool IsActive()
        {
            return m_bActive;
        }
        //------------------------------------------------------
        public void SetParent(Transform pParent, bool bFollowParentLayer = false)
        {
            if (GetTransorm() == null)
                return;
            if(GetTransorm().parent == pParent)
            {
                if (bFollowParentLayer)
                {
                    if (pParent)
                        SetRenderLayer(pParent.gameObject.layer);
                }
                return;
            }
            GetTransorm().SetParent(pParent, true);
            if (bFollowParentLayer)
            {
                if (pParent)
                    SetRenderLayer(pParent.gameObject.layer);
            }
        }
        //------------------------------------------------------
        public Transform GetParent()
        {
            return GetTransorm().parent;
        }
        //------------------------------------------------------
        public int GetDefaultLayer()
        {
            return m_nDefaultLayerFlag;
        }
        //------------------------------------------------------
        public int GetLayer()
        {
            return gameObject.layer;
        }
        //------------------------------------------------------
        public virtual void ResetLayer()
        {
            if (!allowChangeLayer) return;
#if !USE_SERVER
            if (gameObject.layer != m_nDefaultLayerFlag)
            {
                LayerUtil.SetRenderLayer(gameObject, m_nDefaultLayerFlag);
            }
#endif
        }
        //------------------------------------------------------
        public void SetRenderLayer(int layer)
        {
#if !USE_SERVER
            if (!allowChangeLayer) return;
            if (layer == -1) return;
            if (gameObject.layer == LayerUtil.PhysicLayer)
                return;
            if (gameObject.layer != layer)
            {
                if (m_nDefaultLayerFlag == 0) m_nDefaultLayerFlag = gameObject.layer;
                LayerUtil.SetRenderLayer(gameObject, layer);
            }
#endif
        }
        //------------------------------------------------------
        public void ClearMaterialBlock()
        {
            if (m_pLibRenders == null) return;
            for (int i = 0; i < m_pLibRenders.Count; ++i)
            {
                m_pLibRenders[i].Restore(false);
            }
        }
        //------------------------------------------------------
        public void SetColor(string propName, Color color, bool bBlock = true, bool bShare = true, int index = -1)
        {
            bShare = true;
            bBlock = false;
            ModifyRenders();
            MaterialLibrarys.SetRenderColor(m_pLibRenders, propName, color, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public Color GetColor(string propName, int index = 0)
        {
            if (string.IsNullOrEmpty(propName)) return Color.white;
            GetRenders();
            if (m_pRenders == null || index < 0 || index >= m_pRenders.Count || m_pRenders[index].sharedMaterial == null) return Color.white;
            return m_pRenders[index].sharedMaterial.GetColor(propName);
        }
        //------------------------------------------------------
        public void SetVector(string propName, Vector4 vec4, bool bBlock = true, bool bShare = true, int index = -1)
        {
            bShare = true;
            bBlock = false;
            ModifyRenders();
            MaterialLibrarys.SetRenderVector(m_pLibRenders, propName, vec4, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public void SetVector(string propName, Vector3 vec3, bool bBlock = true, bool bShare = true, int index = -1)
        {
            bShare = true;
            bBlock = false;
            ModifyRenders();
            MaterialLibrarys.SetRenderVector(m_pLibRenders, propName, vec3, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public Vector4 GetVector(string propName, int index = 0)
        {
            if (string.IsNullOrEmpty(propName)) return Vector4.zero;
            GetRenders();
            if (m_pRenders == null || index < 0 || index >= m_pRenders.Count || m_pRenders[index].sharedMaterial == null) return Vector4.zero;
            return m_pRenders[index].sharedMaterial.GetVector(propName);
        }
        //------------------------------------------------------
        public void SetFloat(string propName, float fValue, bool bBlock = true, bool bShare = true, int index= -1)
        {
            bShare = true;
            bBlock = false;
            ModifyRenders();
            MaterialLibrarys.SetRenderFloat(m_pLibRenders, propName, fValue, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public float GetFloat(string propName, int index = 0)
        {
            if (string.IsNullOrEmpty(propName)) return 0;
            GetRenders();
            if (m_pRenders == null || index < 0 || index >= m_pRenders.Count || m_pRenders[index].sharedMaterial == null) return 0;
            return m_pRenders[index].sharedMaterial.GetFloat(propName);
        }
        //------------------------------------------------------
        public void SetInt(string propName, int nValue, bool bBlock = true, bool bShare = true, int index = -1)
        {
            bShare = true;
            bBlock = false;
            ModifyRenders();
            MaterialLibrarys.SetRenderInt(m_pLibRenders, propName, nValue, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public int GetInt(string propName, int index = 0)
        {
            if (string.IsNullOrEmpty(propName)) return 0;
            GetRenders();
            if (m_pRenders == null || index < 0 || index >= m_pRenders.Count || m_pRenders[index].sharedMaterial == null) return 0;
            return m_pRenders[index].sharedMaterial.GetInt(propName);
        }
        //------------------------------------------------------
        public void SetTexture(string propName, Texture texture, bool bBlock = true, bool bShare = true, int index = -1)
        {
            bShare = true;
            bBlock = false;
            ModifyRenders();
            MaterialLibrarys.SetRenderTexture(m_pLibRenders, propName, texture, bBlock, bShare, index);
        }
        //------------------------------------------------------
        public void SetMaterial(Material material, int index=1, bool bAutoDestroy = true)
        {
            ModifyRenders();
            MaterialLibrarys.SetMaterial(m_pLibRenders, material, index, bAutoDestroy);
        }
        //------------------------------------------------------
        public void LerpToMaterial(Material material, float fLerpTime, int index = -1, float fKeepTime =0, string propertyName=null, bool bAutoDestroy = true)
        {
            ModifyRenders();
            MaterialLibrarys.LerpToMaterial(m_pLibRenders, material, fLerpTime, index, fKeepTime, propertyName, bAutoDestroy);
        }
        //------------------------------------------------------
        public void FadeoutMaterial()
        {
            MaterialLibrarys.FadeoutLerpMaterial(m_pLibRenders);
        }
        //------------------------------------------------------
        public void RemoveMaterialByIndex(int index)
        {
           // MaterialLibrarys.RemoveMaterial(this, index);
        }
        //------------------------------------------------------
        public void RemoveMaterial(Material pMaterial)
        {
         //   MaterialLibrarys.RemoveMaterial(this, pMaterial);
        }
        //------------------------------------------------------
        public void ReplaceShader(string name, int materialIndex=-1)
        {
            ModifyRenders();
            MaterialLibrarys.ReplaceShader(m_pLibRenders, name, materialIndex);
        }
        //------------------------------------------------------
        public void EnableKeyWorld(string keyWorld, bool bEnable, int materialIndex = -1)
        {
            ModifyRenders();
            MaterialLibrarys.EnableKeyWorld(m_pLibRenders, keyWorld, bEnable, materialIndex);
        }
        //------------------------------------------------------
        protected virtual void LateUpdate()
        {
            if(m_pLibRenders!=null)
            {
                for (int i = 0; i < m_pLibRenders.Count; ++i)
                {
                    m_pLibRenders[i].Update();
                }
            }
            if(m_fDelayDestroy>0)
            {
                if(Time.time >= m_fDelayDestroy)
                {
                    RecyleDestroy(m_nDelayDestroyRecyleMaxCount);
                    ResetDelayDestroyParam();
                }
            }
        }
        //------------------------------------------------------
        public void EnableBehaviour<T>(bool bEnabled) where T : Component
        {
            if (m_vComponents == null) return;
            int hashCode = typeof(T).GetHashCode();
            Component retCom;
            if (m_vComponents.TryGetValue(hashCode, out retCom))
            {
                if (retCom is Behaviour) ((Behaviour)retCom).enabled = bEnabled;
                else if (retCom is Collider) ((Collider)retCom).enabled = bEnabled;
            }
        }
        //------------------------------------------------------
        public T GetBehaviour<T>(bool bFindChild = false) where T : Component
        {
            int hashCode = typeof(T).GetHashCode();
            if(m_vComponents == null)
            {
                m_vComponents = new Dictionary<int, Component>(4);
            }
            Component retCom;
            if (m_vComponents.TryGetValue(hashCode, out retCom))
                return retCom as T;
               
            retCom = GetComponent<T>();
            if(retCom == null && bFindChild)
            {
                retCom = GetComponentInChildren<T>();
            }
            m_vComponents[hashCode] = retCom;
            return retCom as T;
        }
        //------------------------------------------------------
        public T AddBehaviour<T>(bool bNullNew = true,System.Type type = null) where T : Component
        {
            if (m_pObject == null) return null;
            if(type == null) type = typeof(T);
            int hashCode = type.GetHashCode();
            if (m_vComponents !=null)
            {
                Component outCom;
                if(m_vComponents.TryGetValue(hashCode, out outCom))
                {
                    if(!bNullNew)
                    {
                        return outCom as T;
                    }
                }
            }
            T newComp = m_pObject.AddComponent(type) as T;
            if (newComp == null) return null;
            if (m_vComponents == null) m_vComponents = new Dictionary<int, Component>(2);
            m_vComponents[hashCode]= newComp;
            return newComp;
        }
        //------------------------------------------------------
        public virtual void OnFreezed(bool bFreezed)
        {

        }
        //------------------------------------------------------
        void OnTriggerEnter(Collider collider)
        {
            Debug.Log("OnTriggerEnter:" + collider.transform.name);
            if (m_vCallbacks != null)
            {
                for (int i = 0; i < m_vCallbacks.Count; ++i)
                {
                    m_vCallbacks[i].OnInstanceCallback(this, EInstanceCallbackType.ColliderEnter);
                }
            }
        }
        //------------------------------------------------------
        void OnTriggerStay(Collider collider)
        {
            Debug.Log("OnTriggerStay:" + collider.transform.name);
            if (m_vCallbacks != null)
            {
                for (int i = 0; i < m_vCallbacks.Count; ++i)
                {
                    m_vCallbacks[i].OnInstanceCallback(this, EInstanceCallbackType.ColliderStay);
                }
            }
        }
        //------------------------------------------------------
        void OnTriggerExit(Collider collider)
        {
            Debug.Log("OnTriggerExit:" + collider.transform.name);
            if (m_vCallbacks != null)
            {
                for (int i = 0; i < m_vCallbacks.Count; ++i)
                {
                    m_vCallbacks[i].OnInstanceCallback(this, EInstanceCallbackType.ColliderExit);
                }
            }
        }
    }

}