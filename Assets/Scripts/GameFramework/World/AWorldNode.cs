/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	WorldNode
作    者:	HappLI
描    述:	世界节点
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using Framework.Data;
using ExternEngine;
using Framework.Cutscene.Runtime;


#if USE_FIXEDMATH
using ExternEngine;
#else
using FVector3 = UnityEngine.Vector3;
#endif

namespace Framework.Core
{
    public struct TransformDebug
    {
        public int frameCount;
        public Vector3 position;
        public Vector3 rotate;
        public Vector3 scale;
    }

    [Plugin.AT.ATExportMono("World/WorldNode")]
    public abstract partial class AWorldNode
#if USE_CUTSCENE
        : Framework.Cutscene.Runtime.ICutsceneObject
#else
        : IUserData
#endif
    {
        private AWorldNode m_pNext = null;
        private AWorldNode m_pPrev = null;
        protected IUserData m_pContextData = null;
        private string m_strModelFile = null;

        private InstanceOperiaon m_pInstanceOperiaon = null;
        protected Transform m_pParentTransform = null;
        protected AInstanceAble m_pObjectAble = null;
        protected bool m_bLoadCompleted = false;
#if USE_CUTSCENE
        private bool m_bCutsceneHold = false;
#endif

        protected int m_nInstanceID = 0;
        protected byte m_nAttackGroup = 0;

        private int m_nRenderLayer = -1;

        private EActorType m_NodeType = EActorType.Actor;
        private World m_pWorld = null;

        protected FFloat m_fDestoryDelta = 0;

        private bool m_bDirtyTerrainY = false;
        private FVector3 m_vTerrain = FVector3.zero;
        private FVector3 m_vTerrainUp = FVector3.up;
        protected AFramework m_pGame = null;

        protected AServerSync m_pServerSync = null;
        internal PartObjects m_PartObjects = null;

        protected List<TransformDebug>  m_vTransformDebugs = null;
#if UNITY_EDITOR
        public int nSpatialCellIndex = -1;
        private Base.ProfilerTicker m_ProfileTicker = new Base.ProfilerTicker(10);
#endif
        //------------------------------------------------------
        public static void OnConstruction(AWorldNode pNode, World world, EActorType type)
        {
            if (pNode != null)
            {
                pNode.m_bLoadCompleted = false;
                pNode.m_fDestoryDelta = 0;
                pNode.m_NodeType = type;
                pNode.m_pWorld = world;
#if UNITY_EDITOR
                pNode.nSpatialCellIndex = -1;
#endif
                pNode.Construction();
            }
        }
        //------------------------------------------------------
        protected AWorldNode(AFramework pGame)
        {
            m_pGame = pGame;
            m_nFlags = (ushort)EWorldNodeFlag.Default;
        }
        //------------------------------------------------------
        ~AWorldNode()
        {
            m_pGame = null;
        }
        //------------------------------------------------------
        protected virtual void Construction()
        {
#if UNITY_EDITOR
            nSpatialCellIndex = -1;
            m_ProfileTicker.Start("WorldNode");
#endif
            m_nRenderLayer = -1;
            m_bLoadCompleted = false;
            m_fDestoryDelta = 0;
            m_nAttackGroup = 0;
            m_bDirtyTerrainY = false;
            m_vTerrain = FVector3.zero;
            m_vTerrainUp = FVector3.up;
#if USE_CUTSCENE
            m_bCutsceneHold = false;
#endif
            ResetFreeze();
            m_nFlags = (ushort)EWorldNodeFlag.Default;
            m_BoundBox.Clear();
            if (m_pServerSync != null) m_pServerSync.Destroy();
        }
        //------------------------------------------------------
        public void OnCreated()
        {
            m_fDestoryDelta = 0;
            if (m_pServerSync != null) m_pServerSync.Destroy();
            ResetFreeze();
            InnerCreated();
        }
        //------------------------------------------------------
        [Plugin.AT.ATMethod]
        public EActorType GetActorType()
        {
            return m_NodeType;
        }
        //------------------------------------------------------
        public virtual int GetExternType()
        {
            return -1;
        }
        //------------------------------------------------------
        public World GetWorld()
        {
            return m_pWorld;
        }
        //------------------------------------------------------
        public ShareFrameParams GetShareFrameParams()
        {
            if (m_pGame == null) return null;
            return m_pGame.shareParams;
        }
        //------------------------------------------------------
        public AWorldNode GetNext()
        {
            return m_pNext;
        }
        //------------------------------------------------------
        public void SetNext(AWorldNode pNode)
        {
            m_pNext = pNode;
        }
        //------------------------------------------------------
        public AWorldNode GetPrev()
        {
            return m_pPrev;
        }
        //------------------------------------------------------
        public void SetPrev(AWorldNode pNode)
        {
            m_pPrev = pNode;
        }
        //------------------------------------------------------
        public void SetContextData(IUserData pContextData)
        {
            if (m_pContextData == pContextData) return;
            m_pContextData = pContextData;
            OnContextDataDirty();
        }
        //------------------------------------------------------
        public virtual IUserData GetContextData()
        {
            return m_pContextData;
        }
        //--------------------------------------------------------
        public T GetCfgData<T>() where T : BaseData
        {
            if (m_pContextData == null || !(m_pContextData is T)) return null;
            return m_pContextData as T;
        }
        //------------------------------------------------------
        [Plugin.AT.ATMethod]
        public byte GetAttackGroup()
        {
            return m_nAttackGroup;
        }
        //------------------------------------------------------
        public void SetAttackGroup(byte attackGroup)
        {
            m_nAttackGroup = attackGroup;
        }
        //------------------------------------------------------
        public virtual bool CanAttackGroup(byte attackGroup)
        {
            return m_nAttackGroup != attackGroup;
        }
        //--------------------------------------------------------
        public T GetComponent<T>(bool bFindChild = false) where T : Component
        {
            if (m_pObjectAble == null) return null;
            return m_pObjectAble.GetBehaviour<T>(bFindChild);
        }
        //--------------------------------------------------------
        public T AddComponent<T>(bool bNullNew = true) where T : Component
        {
            if (m_pObjectAble == null) return null;
            return m_pObjectAble.AddBehaviour<T>(bNullNew);
        }
        //------------------------------------------------------
        public void SetObjectAble(AInstanceAble instanceAble)
        {
            if (m_pObjectAble == instanceAble)
            {
                m_bDirtyTerrainY = true;
                m_Transform.bDirtyPos = true;
                m_Transform.bDirtyEuler = true;
                m_Transform.bDirtyScale = true;
                return;
            }
            if (m_pObjectAble != null)
            {
#if !USE_SERVER
                m_pObjectAble.EnableBehaviour<CharacterController>(false);
#endif
                m_pObjectAble.RecyleDestroy();
            }
            if (IsDestroy())
            {
                if (instanceAble != null) instanceAble.RecyleDestroy();
                return;
            }
            m_pObjectAble = instanceAble;
            OnInnerSpawnObject(instanceAble);
            if (m_pObjectAble!=null)
            {
                m_Transform.bDirtyPos = true;
                m_Transform.bDirtyEuler = true;
                m_Transform.bDirtyScale = true;
                m_bDirtyTerrainY = true;
                if(m_nRenderLayer != -1)
                {
                    m_pObjectAble.SetRenderLayer(m_nRenderLayer);
                }
#if UNITY_EDITOR
                System.Type debuger = AWorldNodeDebuger.GetDebugerType();
                if(debuger!=null)
                {
                    AWorldNodeDebuger pDebug = m_pObjectAble.GetBehaviour<AWorldNodeDebuger>();
                    if (pDebug == null) pDebug = m_pObjectAble.AddBehaviour<AWorldNodeDebuger>(true, debuger);
                    if(pDebug!=null) pDebug.pNode = this;
                }
#endif
                m_pWorld.OnWorldNodeCallback(this, EWorldNodeStatus.Loaded);
            }
            UpdateTransform();
#if UNITY_EDITOR
            m_ProfileTicker.Stop(instanceAble!=null ? instanceAble.name:"");
#endif
            if(m_pParentTransform!=null)
            {
                m_pObjectAble.SetParent(m_pParentTransform);
                m_pParentTransform = null;
            }
        }
        //------------------------------------------------------
        [Plugin.AT.ATMethod,Plugin.AT.ATExportGUID(1974416911)]
        public AInstanceAble GetObjectAble()
        {
            return m_pObjectAble;
        }
        //------------------------------------------------------
        public void SetInstanceID(int id)
        {
            m_nInstanceID = id;
        }
        //------------------------------------------------------
        [Plugin.AT.ATMethod]
        public virtual int GetInstanceID()
        {
            return m_nInstanceID;
        }
        //------------------------------------------------------
        public virtual long GetSvrGuid()
        {
            return 0;
        }
        //------------------------------------------------------
        public void SetParentTransform(Transform parentRoot)
        {
            m_pParentTransform = parentRoot;
            if (m_pObjectAble == null) return;
            m_pObjectAble.SetParent(parentRoot);
            m_Transform.bDirtyPos = true;
            m_Transform.bDirtyEuler = true;
            m_Transform.bDirtyScale = true;
        }
        //------------------------------------------------------
        public void SetRenderLayer(int nLayer)
        {
            if (m_nRenderLayer == nLayer) return;
            m_nRenderLayer = nLayer;
            if (m_pObjectAble != null)
            {
                m_pObjectAble.SetRenderLayer(nLayer);
            }
        }
        //------------------------------------------------------
        public int GetRenderLayer()
        {
            return m_nRenderLayer;
        }
        //------------------------------------------------------
        public void Update(FFloat fFrame)
        {
            if (IsFlag(EWorldNodeFlag.Destroy))
                return;
            UpdateFreeze(fFrame);
            fFrame = fFrame * GetTimeSpeed();
            InnerUpdate(fFrame);
            UpdateTransform();
            if (m_PartObjects != null) m_PartObjects.Update(fFrame);
            if (m_pServerSync != null) m_pServerSync.Update(fFrame);
            if (m_fDestoryDelta > 0)
            {
                m_fDestoryDelta -= fFrame;
                if (m_fDestoryDelta <= 0)
                {
                    SetDestroy();
                }
            }
        }
        //--------------------------------------------------------
        public void LoadModel(string strModelFile, Transform parentRoot, bool bAsync)
        {
            if (m_strModelFile != null)
            {
                if (m_strModelFile.CompareTo(strModelFile) == 0)
                    return;
            }
            if (m_pObjectAble != null)
            {
                m_pObjectAble.Destroy();
            }
            if (m_pInstanceOperiaon != null)
                m_pInstanceOperiaon.Earse();
            m_strModelFile = strModelFile;

            if (parentRoot == null)
                parentRoot = RootsHandler.ActorsRoot;

            m_pInstanceOperiaon = m_pGame.FileSystem.SpawnInstance(strModelFile, bAsync);
            if (m_pInstanceOperiaon != null)
            {
                m_pInstanceOperiaon.pByParent = parentRoot;
                m_pInstanceOperiaon.OnSign = OnSpawnSign;
                m_pInstanceOperiaon.OnCallback = OnSpawnCallback;
            }
        }
        //-------------------------------------------------
        public void SetInstanceOperiaon(InstanceOperiaon pCallback)
        {
            if (m_pInstanceOperiaon != pCallback)
            {
                if (m_pInstanceOperiaon != null) m_pInstanceOperiaon.Earse();
            }
            m_pInstanceOperiaon = pCallback;
        }
        //-------------------------------------------------
        public void OnSpawnCallback(InstanceOperiaon pCallback)
        {
            m_pInstanceOperiaon = null;
            m_bLoadCompleted = true;
            if (pCallback == null)
            {
                SetObjectAble(null);
                return;
            }
            SetObjectAble(pCallback.GetAble());
        }
        //-------------------------------------------------
        public void OnCreateBakerSkin(Plugin.CGpuSkinMeshAgent pAgent)
        {
            if (pAgent == null)
                return;
            OnInnerSpawnObject(pAgent);
        }
        //-------------------------------------------------
        public void OnSpawnSign(InstanceOperiaon pCallback)
        {
            if (m_nInstanceID == 0)
            {
                pCallback.SetUsed(false);
                return;
            }
            if (IsFlag(EWorldNodeFlag.Destroy))
            {
                pCallback.SetUsed(false);
                return;
            }
            if (IsFlag(EWorldNodeFlag.Killed))
            {
                pCallback.SetUsed(false);
                return;
            }
        }
        //------------------------------------------------------
        public virtual FVector3 GetTerrain()
        {
            return m_vTerrain;
        }
        //------------------------------------------------------
        public FVector3 GetTerrainUp()
        {
            return m_vTerrainUp;
        }
        //------------------------------------------------------
        public void SetTerrain(FVector3 terrain, FVector3 up)
        {
            m_vTerrainUp = up;
            if (m_vTerrain.y != terrain.y) m_bDirtyTerrainY = true;
            m_vTerrain = terrain;
        }
        //------------------------------------------------------
        public void SetTerrain(FVector3 terrain)
        {
            if (m_vTerrain.y != terrain.y) m_bDirtyTerrainY = true;
            m_vTerrain = terrain;
        }
        //------------------------------------------------------
        public FVector3 LimitTerrain(FVector3 position)
        {
            if (IsTerrainBelow())
                position = GetTerrain();
            return position;
        }
        //------------------------------------------------------
        public bool IsTerrainBelow()
        {
            return IsTerrainBelow(GetPosition());
        }
        //------------------------------------------------------
        public bool IsTerrainBelow(FVector3 curPos, float factor= 0.0f)
        {
            FVector3 diffDir = curPos - GetTerrain();
            return FVector3.Dot(diffDir, GetTerrainUp()) < factor;
        }
        //------------------------------------------------------
        public AServerSync GetServerSync()
        {
            return m_pServerSync;
        }
        //------------------------------------------------------
        public void SetServerSync(AServerSync serverSync)
        {
            if (m_pServerSync == serverSync) return;
            if(m_pServerSync!=null) m_pServerSync.Destroy();
            m_pServerSync = serverSync;
            if (m_pServerSync != null) m_pServerSync.Awake(this);
        }
        //------------------------------------------------------
        public void ClearParts()
        { 
            if (m_PartObjects != null)
            {
                m_PartObjects.Clear();
            }
        }
        //------------------------------------------------------
        public int AddPart(string strPart, Vector3 offset, Vector3 eulerAngleOffset, float fScale = 1, string strSlot = null, ESlotBindBit bit = ESlotBindBit.All, float fLifeTime = -1, float fParticleSpeed = -1, bool bVisibleSyncOwner = true)
        {
            if(m_PartObjects == null)
            {
                m_PartObjects = TypeInstancePool.Malloc<PartObjects>();
                m_PartObjects.SetOwner(this);
            }
            return m_PartObjects.Create(strPart, offset, eulerAngleOffset, fScale, strSlot, bit, fLifeTime, fParticleSpeed, bVisibleSyncOwner);
        }
        //------------------------------------------------------
        public int AddPart(string strPart, Vector3 offset, Vector3 eulerAngleOffset, Transform pTrans, float fScale = 1, ESlotBindBit bit = ESlotBindBit.All, float fLifeTime = -1, float fParticleSpeed = -1, bool bVisibleSyncOwner = true)
        {
            if (m_PartObjects == null)
            {
                m_PartObjects = TypeInstancePool.Malloc<PartObjects>();
                m_PartObjects.SetOwner(this);
            }
            return m_PartObjects.Create(strPart, offset, eulerAngleOffset, pTrans, fScale, bit, fLifeTime, fParticleSpeed, bVisibleSyncOwner);
        }
        //------------------------------------------------------
        public List<TransformDebug> GetDebug()
        {
            return m_vTransformDebugs;
        }
#if USE_CUTSCENE
        //------------------------------------------------------
        public UnityEngine.Object GetUniyObject()
        {
            return m_pObjectAble;
        }
        //------------------------------------------------------
        public UnityEngine.Transform GetUniyTransform()
        {
            if (m_pObjectAble == null) return null;
            return m_pObjectAble.transform;
        }
        //------------------------------------------------------
        public UnityEngine.Animator GetAnimator()
        {
            return GetComponent<UnityEngine.Animator>(true);
        }
        //------------------------------------------------------
        public virtual UnityEngine.Camera GetCamera()
        {
            return GetComponent<UnityEngine.Camera>(true);
        }
        //------------------------------------------------------
        public virtual bool SetParameter(EParamType type, CutsceneParam paramData)
        {
            switch (type)
            {
                case EParamType.ePosition:
                    {
                        SetPosition(paramData.ToVector3());
                        return true;
                    }
                case EParamType.eEulerAngle:
                    {
                        SetEulerAngle(paramData.ToVector3());
                        return true;
                    }
                case EParamType.eQuraternion:
                    {
                        SetEulerAngle(paramData.ToQuaternion().eulerAngles);
                        return true;
                    }
                    case EParamType.eScale:
                    {
                        SetScale(paramData.ToVector3());
                        return true;
                    }
                    case EParamType.eHold:
                    {
                        m_bCutsceneHold = paramData.GetBool();
                        return true;
                    }
            }
            return false;
        }
        //------------------------------------------------------
        public virtual bool GetParameter(EParamType type, ref CutsceneParam paramData)
        {
            switch (type)
            {
                case EParamType.ePosition:
                    {
                        paramData.SetVector3(GetPosition());
                        return true;
                    }
                case EParamType.eEulerAngle:
                    {
                        paramData.SetVector3(GetEulerAngle());
                        return true;
                    }
                case EParamType.eQuraternion:
                    {
                        paramData.SetQuaternion(Quaternion.Euler(GetEulerAngle()));
                        return true;
                    }
                case EParamType.eScale:
                    {
                        paramData.SetVector3(GetScale());
                        return true;
                    }
                case EParamType.eHold:
                    {
                        paramData.SetBool(m_bCutsceneHold);
                        return true;
                    }
                case EParamType.eBindSlotMatrix:
                    {
                        var matrix = GetEventBindSlot(paramData.ToString(), -1);
                        paramData.SetMatrix(matrix);
                        return true;
                    }
            }
            return false;
        }
#endif
        //------------------------------------------------------
        public bool IsCutscneHolded()
        {
#if USE_CUTSCENE
            return m_bCutsceneHold;
#else
            return false;
#endif
        }
        //------------------------------------------------------
        internal void FreeDestroy()
        {
            if (m_pPrev != null)
            {
                m_pPrev.SetNext(m_pNext);
            }
            if (m_pNext != null)
            {
                m_pNext.SetPrev(m_pPrev);
            }
            m_strModelFile = null;
            m_pNext = null;
            m_pPrev = null;
            m_pParentTransform = null;
            ResetFreeze();
            m_nInstanceID = 0;
            m_vTerrain = FVector3.zero;
            m_vTerrainUp = FVector3.zero;
            m_bDirtyTerrainY = false;
            m_fDestoryDelta = 0;
            m_Transform.Clear();
            //     m_nFlags = 0;
            if (m_pServerSync != null) m_pServerSync.Destroy();
            if (m_PartObjects != null)
            {
                m_PartObjects.Free();
                m_PartObjects = null;
            }
            if (m_pObjectAble != null)
                m_pObjectAble.RecyleDestroy();
            m_pObjectAble = null;
            if (m_pInstanceOperiaon != null)
                m_pInstanceOperiaon.Earse();
            m_bLoadCompleted = false;
            if (m_vTransformDebugs != null) m_vTransformDebugs.Clear();
#if UNITY_EDITOR
            nSpatialCellIndex = -1;
#endif
#if USE_CUTSCENE
            m_bCutsceneHold = false;
#endif
            m_nRenderLayer = -1;
            OnDestroy();
            if(m_pWorld!=null)
            {
                if (!m_pWorld.Recyle(this))
                {
                    OnReleaseGC();
                }
                m_pWorld = null;
            }
        }
    }
}
