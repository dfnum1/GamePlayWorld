/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	TerrainManager
作    者:	HappLI
描    述:	地表管理器
*********************************************************************/
using ExternEngine;
using UnityEngine;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif
namespace Framework.Core
{
    public enum ETerrainMode
    {
        Mode3D,
        Mode2D,
        Mode25D
    }
    public class TerrainManager : AModule, IDrawGizmos, IJobUpdate
    {
        protected TerrainLayers m_Terrains = new TerrainLayers();
        protected TerrainNavMeshs m_NavMeshs = new TerrainNavMeshs();
        protected ETerrainMode m_eTerrainMode = ETerrainMode.Mode3D;
        protected WorldMaskManager m_pWorldMaskManager = null;
        //------------------------------------------------------
        public TerrainLayers GetTerrains()
        {
            return m_Terrains;
        }
        //------------------------------------------------------
        public TerrainNavMeshs GetNavMeshs()
        {
            return m_NavMeshs;
        }
        //------------------------------------------------------
        public static void SetTerrainMode(AFramework framework,ETerrainMode mode)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return;
            mgr.m_eTerrainMode = mode;
        }
        //------------------------------------------------------
        public static ETerrainMode GetTerrainMode(AFramework framework)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return ETerrainMode.Mode3D;
            return mgr.m_eTerrainMode;
        }
        //------------------------------------------------------
        public static void DirtyClear(AFramework framework)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return;
            mgr.OnDitryClear();
        }
        //------------------------------------------------------
        public static TerrainManager GetInstance(AFramework framework)
        {
            if (framework == null) return null;
            return framework.terrainManager;
        }
        //--------------------------------------------------------
        static public bool SampleNavPoition(FVector3 position, out FVector3 navPos, float maxDistance = 100)
        {
            return TerrainNavMeshs.SampleNavPoition(position, out navPos, maxDistance);
        }
        //------------------------------------------------------
        static public TerrainLayers GetTerrainLayers(AFramework framework)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return null;
            return mgr.m_Terrains;
        }
        //------------------------------------------------------
        static public TerrainNavMeshs GetTerrainNavMeshs(AFramework framework)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return null;
            return mgr.m_NavMeshs;
        }
        //------------------------------------------------------
        static public WorldMaskManager GetWorldMask(AFramework framework)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return null;
            if(mgr.m_pWorldMaskManager == null)
            {
                mgr.m_pWorldMaskManager = new WorldMaskManager();
              //  mgr.m_pWorldMaskManager.Initialize();
            }
            return mgr.m_pWorldMaskManager;
        }
        //------------------------------------------------------
        static public bool HasNavMesh(AFramework framework)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return false;
            return mgr.GetNavMeshs().HasNavMesh();
        }
        //------------------------------------------------------
        public static EPhyTerrain GetAutoModeHeight(AFramework framework, FVector3 curPos, FVector3 curUp, ref FVector3 terrain, ref FVector3 normal, float maxDistance = 1f, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            normal = curUp;
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return EPhyTerrain.None;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return EPhyTerrain.None;
            if (mgr.m_eTerrainMode == ETerrainMode.Mode2D)
            {
                return layers.GetHeight2D(curPos, curUp, ref terrain, ref normal, maxDistance, stepHeight, pIngore);
            }
            return layers.GetHeight(curPos, curUp, ref terrain, ref normal, maxDistance, stepHeight, pIngore);
        }
        //------------------------------------------------------
        public static EPhyTerrain GetAutoModeHeight(AFramework framework, FVector3 curPos, Vector3 curUp, ref FVector3 terrain, ref FVector3 normal, ref int mask, float maxDistance = 1f, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            normal = curUp;
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return EPhyTerrain.None;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return EPhyTerrain.None;
            if (mgr.m_eTerrainMode == ETerrainMode.Mode2D)
            {
                return layers.GetHeight2D(curPos, curUp, ref terrain, ref normal, ref mask, maxDistance, stepHeight, pIngore);
            }
            return layers.GetHeight(curPos, curUp, ref terrain, ref normal, ref mask, maxDistance, stepHeight, pIngore);
        }
        //------------------------------------------------------
        public static EPhyTerrain GetHeight(AFramework framework, FVector3 curPos, FVector3 curUp, ref FVector3 terrain, ref FVector3 normal, float maxDistance = 1f, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            normal = curUp;
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return EPhyTerrain.None;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return EPhyTerrain.None;
            return layers.GetHeight(curPos, curUp, ref terrain, ref normal, maxDistance, stepHeight, pIngore);
        }
        //------------------------------------------------------
        public static EPhyTerrain GetHeight(AFramework framework, FVector3 curPos, Vector3 curUp, ref FVector3 terrain, ref FVector3 normal, ref int mask, float maxDistance = 1f, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            normal = curUp;
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return EPhyTerrain.None;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return EPhyTerrain.None;
            return layers.GetHeight(curPos, curUp, ref terrain, ref normal, ref mask, maxDistance, stepHeight, pIngore);
        }
        //------------------------------------------------------
        public static EPhyTerrain GetHeight2D(AFramework framework, FVector3 curPos, FVector3 curUp, ref FVector3 terrain, ref FVector3 normal, float maxDistance = 1f, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            normal = curUp;
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return EPhyTerrain.None;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return EPhyTerrain.None;
            return layers.GetHeight2D(curPos, curUp, ref terrain, ref normal, maxDistance, stepHeight, pIngore);
        }
        //------------------------------------------------------
        public static EPhyTerrain GetHeight2D(AFramework framework, FVector3 curPos, Vector3 curUp, ref FVector3 terrain, ref FVector3 normal, ref int mask, float maxDistance = 1f, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            normal = curUp;
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return EPhyTerrain.None;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return EPhyTerrain.None;
            return layers.GetHeight2D(curPos, curUp, ref terrain, ref normal, ref mask, maxDistance, stepHeight, pIngore);
        }
        //------------------------------------------------------
        public static bool SampleTerrainHeight(AFramework framework, Vector3 curPos, Vector3 curUp, ref FFloat terrainHeight, float distance = 1000, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            return TerrainLayers.SampleTerrainHeight(framework,curPos, curUp, ref terrainHeight, distance, stepHeight, pIngore);
        }
        //------------------------------------------------------
        public static bool IsPhysicHit(AFramework framework, FVector3 curPos, FVector3 curUp, float distance =1000, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            return TerrainLayers.IsPhysicHit(framework, curPos, curUp, distance, stepHeight, pIngore);
        }
        //------------------------------------------------------
        public static bool IsPhysicCapsuleHit(AFramework framework, FVector3 startPos, FVector3 endPos, FFloat radius, AInstanceAble pIngore = null)
        {
            return TerrainLayers.IsPhysicCapsuleHit(framework, startPos, endPos, radius, pIngore);
        }
        //------------------------------------------------------
        public static bool IsPhysicHit(AFramework framework, FVector3 curPos, FVector3 curUp, out FVector3 hitPos, out FVector3 hitUp, float distance = 1000, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            return TerrainLayers.IsPhysicHit(framework, curPos, curUp, out hitPos, out hitUp, distance,stepHeight, pIngore);
        }
        //------------------------------------------------------
        public static bool IsPhysicHit(AFramework framework, FVector3 curPos, FVector3 curUp, out FVector3 hitPos, out FVector3 hitUp, out Collider collider, float distance = 1000, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            return TerrainLayers.IsPhysicHit(framework, curPos, curUp, out hitPos, out hitUp, out collider, distance, stepHeight, pIngore);
        }
        //------------------------------------------------------
        public static int CalcPhysicOverlap(AFramework framework, FVector3 curPos, FFloat radius, out Collider[] colliders)
        {
            return TerrainLayers.CalcPhysicOverlap(framework, curPos, radius, out colliders);
        }
        //------------------------------------------------------
        public static bool CheckPhysicSphape(AFramework framework, FVector3 curPos, FFloat radius)
        {
            return TerrainLayers.CheckPhysicSphape(framework, curPos, radius);
        }
        //------------------------------------------------------
        public static bool IsTerrainBelow(AFramework framework, FVector3 position)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return false;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return false;
            FVector3 diffDir = position - layers.GetTerrainPosition();
            return FVector3.Dot(diffDir, layers.GetTerrainUp()) < 0.0f;
        }
        //------------------------------------------------------
        public static bool IsTerrainBelowProj(AFramework framework, FVector3 position, out FVector3 adjustPos)
        {
            adjustPos = position;
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return false;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return false;

            FVector3 diffDir = position - layers.GetTerrainPosition();
            if (FVector3.Dot(diffDir, layers.GetTerrainUp()) >= 0.0f) return false;

            adjustPos = BaseUtil.ProjectPointOnPlane(layers.GetTerrainUp(), layers.GetTerrainPosition(), position);
            return true;
        }
        //------------------------------------------------------
        public static bool ProjTerrainHeight(AFramework framework, FVector3 position, ref FFloat fHeight)
        {
            fHeight = 0.0f;
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return false;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return false;
            fHeight = BaseUtil.PointDistancePlane(layers.GetTerrainUp(), layers.GetTerrainPosition(), position);
            return true;
        }
        //------------------------------------------------------
        public static FVector3 ProjTerrain(AFramework framework, FVector3 curPos)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return curPos;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return curPos;
            return BaseUtil.ProjectPointOnPlane(layers.GetTerrainUp(), layers.GetTerrainPosition(), curPos);
        }
        //------------------------------------------------------
        public static FVector3 GetTerrainPosition(AFramework framework)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return FVector3.zero;
            TerrainLayers terrainLayer = mgr.GetTerrains();
            if (terrainLayer == null) return FVector3.zero;
            return terrainLayer.GetTerrainPosition();
        }
        //------------------------------------------------------
        public static FVector3 GetTerrainUp(AFramework framework)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return FVector3.up;
            TerrainLayers terrainLayer = mgr.GetTerrains();
            if (terrainLayer == null) return FVector3.up;
            return terrainLayer.GetTerrainUp();
        }
        //------------------------------------------------------
        public static void SetTerrainPosition(AFramework framework,FVector3 terrain, FVector3 terraionUp)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return;
            TerrainLayers terrainLayer = mgr.GetTerrains();
            if (terrainLayer == null) return;
            terrainLayer.SetTerrainPosition(terrain, terraionUp);
        }
        //------------------------------------------------------
        public static Vector3 ProjTerrain(Vector3 terrainPos, Vector3 terrainUp, Vector3 curPos)
        {
            Vector3 diffDir = curPos - terrainPos;
            if (Vector3.Dot(diffDir, terrainUp) >= 0) return curPos;

            curPos = BaseUtil.ProjectPointOnPlane(terrainUp, terrainPos, curPos);
            return curPos;
        }
        //------------------------------------------------------
        public static float ProjTerrainHeight(Vector3 terrainPos, Vector3 terrainUp, Vector3 curPos)
        {
            Vector3 diffDir = curPos - terrainPos;
            return BaseUtil.PointDistancePlane(terrainUp, terrainPos, curPos);
        }
#if USE_FIXEDMATH
        //------------------------------------------------------
        public static FVector3 ProjTerrain(FVector3 terrainPos, FVector3 terrainUp, FVector3 curPos)
        {
            FVector3 diffDir = curPos - terrainPos;
            if (Vector3.Dot(diffDir, terrainUp) >= FFloat.zero) return curPos;

            curPos = BaseUtil.ProjectPointOnPlane(terrainUp, terrainPos, curPos);
            return curPos;
        }
        //------------------------------------------------------
        public static FFloat ProjTerrainHeight(FVector3 terrainPos, FVector3 terrainUp, FVector3 curPos)
        {
            FVector3 diffDir = curPos - terrainPos;
            return BaseUtil.PointDistancePlane(terrainUp, terrainPos, curPos);
        }
#endif
        //------------------------------------------------------
        public static bool IsTerrainBelow(FVector3 terrainPos, FVector3 terrainUp, FVector3 position)
        {
            FVector3 diffDir = position - terrainPos;
            return FVector3.Dot(diffDir, terrainUp) < 0.0f;
        }
        //------------------------------------------------------
        public static void AdjustTerrainLower(AFramework framework,ref FVector3 curPos)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return;
            layers.AdjustTerrainLower(ref curPos);
        }
        //------------------------------------------------------
        public static void EnableTerrainLowerLimit(AFramework framework, bool bEnable)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return;
            layers.EnableTerrainLowerLimit(bEnable);
        }
        //------------------------------------------------------
        public static void SetTerrainLowerLimit(AFramework framework, FFloat fLowerHeightLimit)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null) return;
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null) return;
            layers.SetTerrainLowerLimit(fLowerHeightLimit);
        }
        //------------------------------------------------------
        public static bool Raycast(AFramework framework, FVector3 curPos, FVector3 vDir, FFloat distance, out RaycastHit hit)
        {
            TerrainManager mgr = GetInstance(framework);
            if (mgr == null)
            {
                hit = default;
                return false;
            }
            TerrainLayers layers = mgr.m_Terrains;
            if (layers == null)
            {
                hit = default;
                return false;
            }
            return layers.Raycast(curPos, vDir, distance, out hit);
        }
        //------------------------------------------------------
        protected override void OnAwake()
        {
            m_NavMeshs.Awake(this);
        }
        //------------------------------------------------------
        protected override void OnStart()
        {
            m_NavMeshs.Start();
        }
        //------------------------------------------------------
        public override void OnClearWorld()
        {
            m_NavMeshs.Clear();
            if (m_Terrains != null) m_Terrains.Clear();
            if (m_pWorldMaskManager != null) m_pWorldMaskManager.Clear();
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            if (m_pWorldMaskManager != null) m_pWorldMaskManager.Shutdown();
        }
        //------------------------------------------------------
        protected virtual void OnDitryClear()
        {

        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
            m_NavMeshs.Update(fFrame);
            if (m_Terrains!=null) m_Terrains.Update(fFrame);
            if (m_pWorldMaskManager != null) m_pWorldMaskManager.Update(fFrame);
        }
        //------------------------------------------------------
        public bool OnJobUpdate(float fFrame, IUserData userData = null)
        {
            //   lock(m_BrushLock)
            {
                OnJobUpdate();
                m_NavMeshs.JobUpdate(fFrame);
                if (m_Terrains != null) m_Terrains.OnJobUpdate(fFrame);
            }
            return true;
        }
        //------------------------------------------------------
        public void OnJobComplete(IUserData uerData = null)
        {

        }
        //------------------------------------------------------
        public virtual int GetJob()
        {
            return 0;
        }
        //------------------------------------------------------
        protected virtual void OnJobUpdate() { }
        //------------------------------------------------------
        public virtual void DrawGizmos() { }
    }
}