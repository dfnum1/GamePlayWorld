/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Terrain
作    者:	HappLI
描    述:	
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FQuaternion = UnityEngine.Quaternion;
#endif
using Framework.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public enum EPhyTerrain
    {
        None = 0,
        Hit,
        UnHitBelow,
    }
    public enum ELimitZoomFlag
    {
        LimitX = 1<<0,
        LimitZ = 1<<1,
    }
    public enum ETerrainHeightType
    {
        [PluginDisplay("扯TM地表，放飞自我吧")]
        None  =0,
        [PluginDisplay("贴地")]
        TerrainY = 1,
        [PluginDisplay("贴地高度")]
        TerrainH = 2,
    }
    public class TerrainLayers
    {
        protected struct Polygon
        {
            public List<Vector3> points;

            public bool IsContain(UnityEngine.Bounds bounds)
            {
                if (points == null || points.Count <= 0) return false;
                for(int i =0; i < points.Count; ++i)
                {
                    if (bounds.Contains(points[i])) return true;
                }
                return false;
            }
            public FFloat TestPolygonInsection(Vector3 point, bool bCheckContain, out FVector3 vInsection)
            {
                vInsection = point;
                if (points == null || points.Count < 3) return FFloat.MaxValue;
                if(bCheckContain)
                {
                    if (PolygonUtil.ContainsConvexPolygonPoint(points, point))
                    {
                        return 0;
                    }
                }
  

                FVector3 center = FVector3.zero;
                for (int i = 0; i < points.Count; ++i) center += points[i];
                center /= points.Count;
                center.y = point.y;

                FVector3 insection = FVector3.zero;
                for(int i =0; i < points.Count; ++i)
                {
                    int inext = (i + 1) % points.Count;
#if USE_FIXEDMATH
                    if (IntersetionUtil.LineLineIntersectionF(out insection, center, point, points[i], points[inext]))
#else
                    if (IntersetionUtil.LineLineIntersection(out insection, center, point, points[i], points[inext]))
#endif
                    {
                        vInsection = insection;
                        return (insection - point).sqrMagnitude;
                    }
                }
                return FFloat.MaxValue;
            }
        }
        protected struct Zoom
        {
            public int guid;
            public Vector3 start;
            public Vector3 end;
            public List<Polygon> polygons;
        }
        protected FVector3 m_TerrainUp = FVector3.up;
        protected FVector3 m_TerrainPos = FVector3.zero;
        protected List<Vector3> m_vPaths = null;
#if !USE_SERVER
        protected RaycastHit[] m_MutiPhysicHit;
        protected RaycastHit m_PhysicHit;
        protected RaycastHit2D[] m_MutiPhysicHit2D;
        protected RaycastHit2D m_PhysicHit2D;
        protected Collider[] m_PhysicColliders;
        protected int m_nTerrainLayer = 1 << LayerMask.NameToLayer(LayerUtil.ms_physicLayerName);
#endif
        protected List<Zoom> m_LayerZooms = new List<Zoom>(6);

        public static Vector2Int TerrainGridSize = Vector2Int.one * 3;
        protected List<TerrainGridMap> m_vGridMaps = null;
        protected Dictionary<long, TerrainGridLinkOffline> m_vTerrainGridOfflines = null;
        Dictionary<int, List<TerrainGridMap>> m_vBridgeTerrainGridMaps = null;
        List<IPathSearcher> m_vPathSearchers = new List<IPathSearcher>(10);
        List<IPathSearcher> m_vPathSearcherCompletes =  new List<IPathSearcher>(10);
        Dictionary<long, List<Vector3Int>> m_vCatchPaths =new Dictionary<long, List<Vector3Int>>(50);

        protected FFloat m_fTerrainLowerHeight = 0.0f;
        protected bool m_bEnableLowerHeightLimit = false;
        //------------------------------------------------------
        public virtual EPhyTerrain GetHeight(FVector3 curPos, FVector3 curUp, ref FVector3 terrain, ref FVector3 normal, float maxDistance = 1f, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            terrain = curPos;
            normal = curUp;
            if (m_vGridMaps != null && m_vGridMaps.Count > 0)
            {
             //   bool bHasHitMap = false;
                Vector3Int worldGrid = TerrainLayers.WorldPosToWorldGrid3D(curPos);
                int maxHeight = 0;
                for (int i = 0; i < m_vGridMaps.Count; ++i)
                {
                    int tempHeight;
                    if (m_vGridMaps[i].SampleHeight(worldGrid, out tempHeight))
                    {
                        if (tempHeight >= maxHeight)
                            maxHeight = tempHeight;
               //         bHasHitMap = true;
                    }
                }
                terrain.y = (FFloat)TerrainLayers.GridPosYToWorldPosY(maxHeight);
                AdjustTerrainLower(ref terrain);
                return EPhyTerrain.Hit;
            }
            if (pIngore != null)
            {
                if (m_MutiPhysicHit == null) m_MutiPhysicHit = new RaycastHit[2];
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                int hitCnt = Physics.RaycastNonAlloc(curPos + curUp * stepHeight, -curUp, m_MutiPhysicHit, maxDistance - stepHeight, m_nTerrainLayer);
                if (hitCnt > 0)
                {
                    bool bValidHit = false;
                    float nearlyDist = float.MaxValue;
                    RaycastHit curtHit = m_MutiPhysicHit[0];
                    if (pIngore != null)
                    {
                        for (int i = 0; i < hitCnt; ++i)
                        {
                            if (!BaseUtil.IsSubTransform(m_MutiPhysicHit[i].transform, pIngore.GetTransorm()))
                            {
                                float dist =  (m_MutiPhysicHit[i].point - curPos).sqrMagnitude;
                                if(dist <= nearlyDist)
                                {
                                    nearlyDist = dist;
                                    curtHit = m_MutiPhysicHit[i];
                                    bValidHit = true;
                                }
                            }
                        }
                    }
                    else bValidHit = true;

                    if (bValidHit)
                    {
                        normal = curtHit.normal;
                        terrain = curtHit.point;
                        AdjustTerrainLower(ref terrain);
                        if(FVector3.Dot(curUp, normal) >=0)
                            return EPhyTerrain.Hit;
                        else 
                            return EPhyTerrain.UnHitBelow;
                    }
                }
            }
            else
            {
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                if (Physics.Raycast(curPos + curUp * stepHeight, -curUp, out m_PhysicHit, maxDistance - stepHeight, m_nTerrainLayer))
                {
                    normal = m_PhysicHit.normal;
                    terrain = m_PhysicHit.point;
                    AdjustTerrainLower(ref terrain);
                    if (FVector3.Dot(curUp, normal) >= 0)
                        return EPhyTerrain.Hit;
                    else
                        return EPhyTerrain.UnHitBelow;
                }
            }
            return EPhyTerrain.None;
        }
        //------------------------------------------------------
        public virtual EPhyTerrain GetHeight(FVector3 curPos, FVector3 curUp, ref FVector3 terrain, ref FVector3 normal, ref int mask, float maxDistance = 1f, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            terrain = curPos;
            normal = curUp;
            if (m_vGridMaps != null && m_vGridMaps.Count > 0)
            {
                bool bHasHitMap = false;
                Vector3Int worldGrid = TerrainLayers.WorldPosToWorldGrid3D(curPos);
                int maxHeight = 0;
                for (int i = 0; i < m_vGridMaps.Count; ++i)
                {
                    int tempHeight;
                    if (m_vGridMaps[i].SampleHeight(worldGrid, out tempHeight))
                    {
                        if (tempHeight >= maxHeight)
                            maxHeight = tempHeight;
                        bHasHitMap = true;
                    }
                }
                terrain.y = (FFloat)TerrainLayers.GridPosYToWorldPosY(maxHeight);
                AdjustTerrainLower(ref terrain);
                return EPhyTerrain.Hit;
            }
            if (pIngore != null)
            {
                if (m_MutiPhysicHit == null) m_MutiPhysicHit = new RaycastHit[2];
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                int hitCnt = Physics.RaycastNonAlloc(curPos + curUp * stepHeight, -curUp, m_MutiPhysicHit, maxDistance - stepHeight, m_nTerrainLayer);
                if (hitCnt > 0)
                {
                    bool bValidHit = false;
                    float nearlyDist = float.MaxValue;
                    RaycastHit curtHit = m_MutiPhysicHit[0];
                    if (pIngore != null)
                    {
                        for (int i = 0; i < hitCnt; ++i)
                        {
                            if (!BaseUtil.IsSubTransform(m_MutiPhysicHit[i].transform, pIngore.GetTransorm()))
                            {
                                float dist = (m_MutiPhysicHit[i].point - curPos).sqrMagnitude;
                                if (dist <= nearlyDist)
                                {
                                    nearlyDist = dist;
                                    curtHit = m_MutiPhysicHit[i];
                                    bValidHit = true;
                                }
                            }
                        }
                    }
                    else bValidHit = true;

                    if (bValidHit)
                    {
                        normal = curtHit.normal;
                        terrain = curtHit.point;
                        AdjustTerrainLower(ref terrain);
                        if (FVector3.Dot(curUp, normal) >= 0)
                            return EPhyTerrain.Hit;
                        else
                            return EPhyTerrain.UnHitBelow;
                    }
                }
            }
            else
            {
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                if (Physics.Raycast(curPos + curUp * stepHeight, -curUp, out m_PhysicHit, maxDistance - stepHeight, m_nTerrainLayer))
                {
                    mask = m_nTerrainLayer;
                    normal = m_PhysicHit.normal;
                    terrain = m_PhysicHit.point;
                    AdjustTerrainLower(ref terrain);
                    if (FVector3.Dot(curUp, normal) >= 0)
                        return EPhyTerrain.Hit;
                    else
                        return EPhyTerrain.UnHitBelow;
                }
            }
            return EPhyTerrain.None;
        }
        //------------------------------------------------------
        public bool Raycast(FVector3 curPos, FVector3 vDir, FFloat distance, out RaycastHit hit)
        {
            if (Physics.Raycast(curPos, vDir, out hit, distance,m_nTerrainLayer))
            {
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public virtual EPhyTerrain GetHeight2D(FVector3 curPos, FVector3 curUp, ref FVector3 terrain, ref FVector3 normal, float maxDistance = 1f, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            terrain = curPos;
            normal = curUp;
            EPhyTerrain result = GetHeight(curPos, curUp, ref terrain, ref normal, maxDistance, stepHeight, pIngore);
            if (result == EPhyTerrain.Hit)
                return result;

            if (pIngore != null)
            {
                if (m_MutiPhysicHit == null) m_MutiPhysicHit = new RaycastHit[2];
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                FVector3 oriPos = curPos + curUp * stepHeight;
#if USE_FIXEDMATH
                int hitCnt = Physics2D.RaycastNonAlloc(oriPos.ToVec2(), -curUp.ToVec2(), m_MutiPhysicHit2D, maxDistance - stepHeight, m_nTerrainLayer);
#else
                int hitCnt = Physics2D.RaycastNonAlloc(oriPos, -curUp, m_MutiPhysicHit2D, maxDistance - stepHeight, m_nTerrainLayer);
#endif
                if (hitCnt > 0)
                {
                    bool bValidHit = false;
                    float nearlyDist = float.MaxValue;
                    RaycastHit2D curtHit = m_MutiPhysicHit2D[0];
                    if (pIngore != null)
                    {
#if !USE_FIXEDMATH
                        Vector2 pos2D = new Vector2(curPos.x, curPos.y);
#endif
                        for (int i = 0; i < hitCnt; ++i)
                        {
                            if (!BaseUtil.IsSubTransform(m_MutiPhysicHit2D[i].transform, pIngore.GetTransorm()))
                            {
#if USE_FIXEDMATH
                                float dist = (m_MutiPhysicHit2D[i].point - curPos).sqrMagnitude;
#else
                                float dist = (m_MutiPhysicHit2D[i].point - pos2D).sqrMagnitude;
#endif
                                if (dist <= nearlyDist)
                                {
                                    nearlyDist = dist;
                                    curtHit = m_MutiPhysicHit2D[i];
                                    bValidHit = true;
                                }
                            }
                        }
                    }
                    else bValidHit = true;

                    if (bValidHit)
                    {
#if USE_FIXEDMATH
                        normal = curtHit.normal.ToVector3();
                        terrain = curtHit.point.ToVector3();
#else
                        normal = curtHit.normal;
                        terrain = curtHit.point;
#endif
                        AdjustTerrainLower(ref terrain);
                        if (FVector3.Dot(curUp, normal) >= 0)
                            return EPhyTerrain.Hit;
                        else
                            return EPhyTerrain.UnHitBelow;
                    }
                }
            }
            else
            {
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                FVector3 oriPos = curPos + curUp * stepHeight;
#if USE_FIXEDMATH
                m_PhysicHit2D = Physics2D.Raycast(oriPos.ToVec2(), -curUp.ToVec2(), maxDistance - stepHeight, m_nTerrainLayer);
#else
                m_PhysicHit2D = Physics2D.Raycast(oriPos, -curUp, maxDistance - stepHeight, m_nTerrainLayer);
#endif
                if (m_PhysicHit2D.collider!=null)
                {
#if USE_FIXEDMATH
                    normal = m_PhysicHit2D.normal.ToVector3();
                    terrain = m_PhysicHit2D.point.ToVector3();
#else
                    normal = m_PhysicHit2D.normal;
                    terrain = m_PhysicHit2D.point;
#endif
                    AdjustTerrainLower(ref terrain);
                    if (FVector3.Dot(curUp, normal) >= 0)
                        return EPhyTerrain.Hit;
                    else
                        return EPhyTerrain.UnHitBelow;
                }
            }
            return EPhyTerrain.None;
        }
        //------------------------------------------------------
        public virtual EPhyTerrain GetHeight2D(FVector3 curPos, FVector3 curUp, ref FVector3 terrain, ref FVector3 normal, ref int mask, float maxDistance = 1f, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            terrain = curPos;
            normal = curUp;

            EPhyTerrain result = GetHeight(curPos, curUp, ref terrain, ref normal, ref mask, maxDistance, stepHeight, pIngore);
            if (result == EPhyTerrain.Hit)
                return result;
            if (pIngore != null)
            {
                if (m_MutiPhysicHit == null) m_MutiPhysicHit = new RaycastHit[2];
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                FVector3 oriPos = curPos + curUp * stepHeight;
#if USE_FIXEDMATH
                int hitCnt = Physics2D.RaycastNonAlloc(oriPos.ToVec2(), -curUp.ToVec2(), m_MutiPhysicHit2D, maxDistance - stepHeight, m_nTerrainLayer);
#else
                int hitCnt = Physics2D.RaycastNonAlloc(oriPos, -curUp, m_MutiPhysicHit2D, maxDistance - stepHeight, m_nTerrainLayer);
#endif
                if (hitCnt > 0)
                {
                    bool bValidHit = false;
                    float nearlyDist = float.MaxValue;
                    RaycastHit2D curtHit = m_MutiPhysicHit2D[0];
                    if (pIngore != null)
                    {
#if !USE_FIXEDMATH
                        Vector2 pos2D = new Vector2(curPos.x, curPos.y);
#endif
                        for (int i = 0; i < hitCnt; ++i)
                        {
                            if (!BaseUtil.IsSubTransform(m_MutiPhysicHit2D[i].transform, pIngore.GetTransorm()))
                            {
#if USE_FIXEDMATH
                                float dist = (m_MutiPhysicHit2D[i].point - curPos).sqrMagnitude;
#else
                                float dist = (m_MutiPhysicHit2D[i].point - pos2D).sqrMagnitude;
#endif
                                if (dist <= nearlyDist)
                                {
                                    nearlyDist = dist;
                                    curtHit = m_MutiPhysicHit2D[i];
                                    bValidHit = true;
                                }
                            }
                        }
                    }
                    else bValidHit = true;

                    if (bValidHit)
                    {
#if USE_FIXEDMATH
                        normal = curtHit.normal.ToVector3();
                        terrain = curtHit.point.ToVector3();
#else
                        normal = curtHit.normal;
                        terrain = curtHit.point;
#endif
                        AdjustTerrainLower(ref terrain);
                        if (FVector3.Dot(curUp, normal) >= 0)
                            return EPhyTerrain.Hit;
                        else
                            return EPhyTerrain.UnHitBelow;
                    }
                }
            }
            else
            {
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                FVector3 oriPos = curPos + curUp * stepHeight;
#if USE_FIXEDMATH
                m_PhysicHit2D = Physics2D.Raycast(oriPos.ToVec2(), -curUp.ToVec2(), maxDistance - stepHeight, m_nTerrainLayer);
#else
                m_PhysicHit2D = Physics2D.Raycast(oriPos, -curUp, maxDistance - stepHeight, m_nTerrainLayer);
#endif
                if (m_PhysicHit2D.collider!=null)
                {
                    mask = m_nTerrainLayer;
#if USE_FIXEDMATH
                    normal = m_PhysicHit2D.normal.ToVector3();
                    terrain = m_PhysicHit2D.point.ToVector3();
#else
                    normal = m_PhysicHit2D.normal;
                    terrain = m_PhysicHit2D.point;
#endif
                    AdjustTerrainLower(ref terrain);
                    if (FVector3.Dot(curUp, normal) >= 0)
                        return EPhyTerrain.Hit;
                    else
                        return EPhyTerrain.UnHitBelow;
                }
            }
            return EPhyTerrain.None;
        }
        //------------------------------------------------------
        public bool Raycast2D(FVector3 curPos, FVector3 vDir, FFloat distance, out RaycastHit2D hit)
        {
#if USE_FIXEDMATH
            hit = Physics2D.Raycast(curPos.ToVec2(), vDir.ToVec2(), distance, m_nTerrainLayer);
#else
            hit = Physics2D.Raycast(curPos, vDir, distance, m_nTerrainLayer);
#endif
            if (hit.collider!=null)
            {
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public void AdjustTerrainLower(ref FVector3 curPos)
        {
            if (!m_bEnableLowerHeightLimit) return;
#if USE_FIXEDMATH
            curPos.y = FMath.Max(m_fTerrainLowerHeight, curPos.y);
#else
            curPos.y = System.Math.Max(m_fTerrainLowerHeight, curPos.y);
#endif
        }
        //------------------------------------------------------
        public void EnableTerrainLowerLimit( bool bEnable)
        {
            m_bEnableLowerHeightLimit = bEnable;
        }
        //------------------------------------------------------
        public void SetTerrainLowerLimit(FFloat fLowerHeightLimit)
        {
            m_fTerrainLowerHeight = fLowerHeightLimit;
        }
        //------------------------------------------------------
        public virtual void SetTerrainPosition(FVector3 pos, FVector3 terraionUp)
        {
            m_TerrainUp = terraionUp;
            m_TerrainPos = pos;
        }
        //------------------------------------------------------
        public virtual FVector3 GetTerrainPosition()
        {
            return m_TerrainPos;
        }
        //------------------------------------------------------
        public virtual FVector3 GetTerrainUp()
        {
            return m_TerrainUp;
        }
        //------------------------------------------------------
        public virtual void Clear()
        {
            m_fTerrainLowerHeight = 0.0f;
            m_bEnableLowerHeightLimit = false;
            m_TerrainUp = FVector3.up;
            m_TerrainPos = FVector3.zero;
            m_LayerZooms.Clear();
            if (m_vGridMaps != null) m_vGridMaps.Clear();
            if (m_vTerrainGridOfflines != null) m_vTerrainGridOfflines.Clear();
            if (m_vPathSearchers != null) m_vPathSearchers.Clear();
            if (m_vPathSearcherCompletes != null) m_vPathSearcherCompletes.Clear();
            if (m_vCatchPaths != null) m_vCatchPaths.Clear();
        }
        //------------------------------------------------------
        public static bool FindPath(AFramework framework, IPathSearcher pathSearcher)
        {
            if (pathSearcher == null) return false;
            TerrainLayers terrains = TerrainManager.GetTerrainLayers(framework);
            if (terrains == null) return false;
           // Vector3Int startGrid = WorldPosToWorldGrid3D(pathSearcher.GetStart());
           // Vector3Int endGrid = WorldPosToWorldGrid3D(pathSearcher.GetEnd());

            if (terrains.m_vPathSearchers == null) terrains.m_vPathSearchers = new List<IPathSearcher>();
            pathSearcher.SetFramework(framework);
            terrains.m_vPathSearchers.Add(pathSearcher);
            return true;
        }
        //------------------------------------------------------
        protected virtual void OnAddTerrainZoom(int guid, List<Vector3> vPolygon, Vector3 centerPos, bool bClear) { }
        protected virtual void OnRemoveTerrainZoom(int guid) { }
        //------------------------------------------------------
        public static void AddTerrainZoom(AFramework framework, int guid, List<Vector3> vPolygon, Vector3 centerPos, bool bClear)
        {
            TerrainLayers terrains = TerrainManager.GetTerrainLayers(framework);
            if (terrains == null) return;
            terrains.OnAddTerrainZoom(guid, vPolygon, centerPos, bClear);
        }
        //------------------------------------------------------
        public static void RemoveTerrainZoom(AFramework pFramework, int guid)
        {
            TerrainLayers terrains = TerrainManager.GetTerrainLayers(pFramework);
            if (terrains == null) return;
            terrains.OnRemoveTerrainZoom(guid);
        }
        //------------------------------------------------------
        public static int CheckInZooms(AWorldNode pNode, bool bOnlyAxisX = false)
        {
            if (pNode == null) return 0;
            return CheckInZooms(pNode.GetGameModule(), pNode.GetPosition(), bOnlyAxisX);
        }
        //------------------------------------------------------
        public static void LimitInWorldZooms(AFramework pFramework, ref FVector3 vPosition)
        {
            if (pFramework == null) return;
            TerrainLayers terrains = TerrainManager.GetTerrainLayers(pFramework);
            if (terrains == null) return;
#if USE_FIXEDMATH
            FVector3 vMin = FVector3.min, vMax = FVector3.max;
#else
            FVector3 vMin = Vector3.one*float.MinValue, vMax = Vector3.one*float.MaxValue;
#endif
            if (pFramework.gameWorld.GetLimitZoom(ref vMin, ref vMax))
            {
#if USE_FIXEDMATH
                if ((vMax.x - vMin.x) > 0.5f)
                {
                    vPosition.x = ExternEngine.FMath.Clamp(vPosition.x, vMin.x, vMax.x);
                }
                if ((vMax.z - vMin.z) > 0.5f)
                {
                    vPosition.z = ExternEngine.FMath.Clamp(vPosition.z, vMin.z, vMax.z);
                }
#else
                if ((vMax.x - vMin.x) > 0.5f)
                {
                    vPosition.x = System.Math.Clamp(vPosition.x, vMin.x, vMax.x);
                }
                if ((vMax.z - vMin.z) > 0.5f)
                {
                    vPosition.z = System.Math.Clamp(vPosition.z, vMin.z, vMax.z);
                }
#endif
            }
        }
        //------------------------------------------------------
        public static void LimitInZooms(AFramework pFramework, ref FVector3 vPosition)
        {
            TerrainLayers terrains = TerrainManager.GetTerrainLayers(pFramework);
            if (terrains == null) return ;
            LimitInWorldZooms(pFramework, ref vPosition);

            if (terrains.m_LayerZooms.Count > 0)
            {
                Zoom zoom;
                for (int z = 0; z < terrains.m_LayerZooms.Count; ++z)
                {
                    zoom = terrains.m_LayerZooms[z];
                    if (vPosition.z >= zoom.start.z && vPosition.z <= zoom.end.z)
                    {
                        bool bHasContain = false;
                        for (int i = 0; i < zoom.polygons.Count; ++i)
                        {
                            if (PolygonUtil.ContainsConvexPolygonPoint(zoom.polygons[i].points, vPosition))
                            {
                                bHasContain = true;
                                break;
                            }
                        }
                        if(!bHasContain)
                        {
                            FFloat nearlyDist = FFloat.MaxValue;
                            FVector3 adjstPos = vPosition;
                            for (int i = 0; i < zoom.polygons.Count; ++i)
                            {
                                FFloat cur = zoom.polygons[i].TestPolygonInsection(vPosition,false, out adjstPos);
                                if(cur <= nearlyDist)
                                {
                                    vPosition = adjstPos;
                                    nearlyDist = cur;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
        //------------------------------------------------------
        public static int CheckInZooms(AFramework pFramework, FVector3 vPosition, bool bOnlyAxisX = false)
        {
            TerrainLayers terrains = TerrainManager.GetTerrainLayers(pFramework);
            if (terrains == null) return 0;
            if (pFramework == null) return 0;
            int flag = 0;

#if USE_FIXEDMATH
            FVector3 vMin = FVector3.min, vMax = FVector3.max;
#else
            FVector3 vMin = Vector3.one * float.MinValue, vMax = Vector3.one * float.MaxValue;
#endif
            if (pFramework.gameWorld.GetLimitZoom(ref vMin, ref vMax))
            {
                if (bOnlyAxisX)
                {
                    if ((vMax.x - vMin.x) > 0.5f)
                    {
                        if (vPosition.x < vMin.x || vPosition.x > vMax.x) return (int)ELimitZoomFlag.LimitX;
                    }
                }
                else
                {
                    if ((vMax.x - vMin.x) > 0.5f)
                    {
                        if (vPosition.x < vMin.x || vPosition.x > vMax.x) flag |= (int)ELimitZoomFlag.LimitX;
                    }
                    if ((vMax.z - vMin.z) > 0.5f)
                    {
                        if (vPosition.z < vMin.z || vPosition.z > vMax.z) flag |= (int)ELimitZoomFlag.LimitZ;
                    }
                }
            }

            if(terrains.m_vGridMaps!=null && terrains.m_vGridMaps.Count>0)
            {
                Vector3Int gridWorld = WorldPosToWorldGrid3D(vPosition);
                TerrainGridMap gridMap = GetGridMap(pFramework, gridWorld);
                if(gridMap == null)
                {
                    flag |= (int)ELimitZoomFlag.LimitX;
                    flag |= (int)ELimitZoomFlag.LimitZ;
                    return flag;
                }
                GridNode gridNode = gridMap.GetGridByWorld(gridWorld);
                if(!gridNode.IsValid || gridNode.IsState(EGridState.Obstacle))
                {
                    flag |= (int)ELimitZoomFlag.LimitX;
                    flag |= (int)ELimitZoomFlag.LimitZ;
                    return flag;
                }
            }

            if (terrains.m_LayerZooms.Count > 0)
            {
                bool bHasLimitZ = true;
                Zoom zoom;
                for (int z = 0; z < terrains.m_LayerZooms.Count; ++z)
                {
                    zoom = terrains.m_LayerZooms[z];
                    if (vPosition.z >= zoom.start.z && vPosition.z <= zoom.end.z)
                    {
                        for (int i = 0; i < zoom.polygons.Count; ++i)
                        {
                            if (PolygonUtil.ContainsConvexPolygonPoint(zoom.polygons[i].points, vPosition))
                            {
                                return flag;
                            }
                        }
                        bHasLimitZ = false;
                        flag |= (int)ELimitZoomFlag.LimitX;
                        break;
                    }
                }
                if (bHasLimitZ)
                    flag |= (int)ELimitZoomFlag.LimitZ;
            }

            return flag;
        }
        //------------------------------------------------------
        public static void AdjustTerrainHeight(AFramework framework, ref FVector3 curPos, FVector3 curUp, ETerrainHeightType heightType, float distance = 1000)
        {
            TerrainLayers layers = TerrainManager.GetTerrainLayers(framework);
            if (layers == null) return;
            if (heightType != ETerrainHeightType.None)
            {
                FVector3 fHeight = FVector3.zero;
                FVector3 vNormal = curUp;
                if (layers.GetHeight(curPos, curUp, ref fHeight, ref vNormal, 1000) == EPhyTerrain.Hit)
                {
                    curUp = vNormal;
                    if (heightType == ETerrainHeightType.TerrainY) curPos.y = fHeight.y;
                    else curPos.y += fHeight.y;
                }
                else
                {
                    if (heightType == ETerrainHeightType.TerrainY) curPos.y = 0;
                }
            }
        }
        //------------------------------------------------------
        public static bool SampleTerrainHeight(AFramework framework, FVector3 curPos, FVector3 curUp, ref FFloat terrainHeight, float distance = 1000, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            TerrainLayers layers = TerrainManager.GetTerrainLayers(framework);
            if (layers == null) return false;
            FVector3 terrain = FVector3.zero;
            terrainHeight = 0.0f;
            FVector3 vNormal = curUp;
            if (layers.GetHeight(curPos, curUp, ref terrain, ref vNormal, distance, stepHeight, pIngore) == EPhyTerrain.Hit)
            {
                terrainHeight = terrain.y;
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public static bool IsPhysicHit(AFramework framework, FVector3 curPos, FVector3 curUp, float distance = 1000, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            TerrainLayers layers = TerrainManager.GetTerrainLayers(framework);
            if (layers == null) return false;

            if(pIngore!=null)
            {
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                if (layers.m_MutiPhysicHit == null) layers.m_MutiPhysicHit = new RaycastHit[2];
                int hitCnt = Physics.RaycastNonAlloc(curPos + curUp * stepHeight, -curUp, layers.m_MutiPhysicHit, distance - stepHeight, layers.m_nTerrainLayer);
                if (hitCnt > 0)
                {
                    bool bValidHit = false;
                    RaycastHit curtHit = layers.m_MutiPhysicHit[0];
                    if (pIngore != null)
                    {
                        for (int i = 0; i < hitCnt; ++i)
                        {
                            if (!BaseUtil.IsSubTransform(layers.m_MutiPhysicHit[i].transform, pIngore.GetTransorm()))
                            {
                                curtHit = layers.m_MutiPhysicHit[i];
                                bValidHit = true;
                                break;
                            }
                        }
                    }
                    else bValidHit = true;
                    return bValidHit;
                }
            }
            else
            {
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                if (Physics.Raycast(curPos + curUp * stepHeight, -curUp, out layers.m_PhysicHit, distance - stepHeight, layers.m_nTerrainLayer))
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        public static bool IsPhysicHit(AFramework framework, FVector3 curPos, FVector3 curUp, out FVector3 hitPos, out FVector3 hitUp, float distance = 1000, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            hitPos = curPos;
            hitUp = curUp;
            TerrainLayers layers = TerrainManager.GetTerrainLayers(framework);
            if (layers == null) return false;
            if(pIngore!=null)
            {
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                if (layers.m_MutiPhysicHit == null) layers.m_MutiPhysicHit = new RaycastHit[2];
                int hitCnt = Physics.RaycastNonAlloc(curPos + curUp * stepHeight, -curUp, layers.m_MutiPhysicHit, distance - stepHeight, layers.m_nTerrainLayer);
                if (hitCnt > 0)
                {
                    bool bValidHit = false;
                    float nearlyDist = float.MaxValue;
                    RaycastHit curtHit = layers.m_MutiPhysicHit[0];
                    if (pIngore != null)
                    {
                        for (int i = 0; i < hitCnt; ++i)
                        {
                            if (!BaseUtil.IsSubTransform(layers.m_MutiPhysicHit[i].transform, pIngore.GetTransorm()))
                            {
                                float dist = (layers.m_MutiPhysicHit[i].point - curPos).sqrMagnitude;
                                if (dist <= nearlyDist)
                                {
                                    nearlyDist = dist;
                                    curtHit = layers.m_MutiPhysicHit[i];
                                    bValidHit = true;
                                }
                            }
                        }
                    }
                    else bValidHit = true;
                    if (bValidHit)
                    {
                        hitPos = curtHit.point;
                        hitUp = curtHit.normal;
                    }
                    return bValidHit;
                }
            }
            else 
            {
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                if (Physics.Raycast(curPos + curUp * stepHeight, -curUp, out layers.m_PhysicHit, distance - stepHeight, layers.m_nTerrainLayer))
                {
                    hitPos = layers.m_PhysicHit.point;
                    hitUp = layers.m_PhysicHit.normal;
                    return true;
                }
            }
            return false;
        }

        //------------------------------------------------------
        public static bool IsPhysicHit(AFramework framework, FVector3 curPos, FVector3 curUp, out FVector3 hitPos, out FVector3 hitUp, out Collider collider, float distance = 1000, float stepHeight = -1, AInstanceAble pIngore = null)
        {
            hitPos = curPos;
            hitUp = curUp;
            collider = null;
            TerrainLayers layers = TerrainManager.GetTerrainLayers(framework);
            if (layers == null) return false;
            if (pIngore != null)
            {
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                if (layers.m_MutiPhysicHit == null) layers.m_MutiPhysicHit = new RaycastHit[2];
                int hitCnt = Physics.RaycastNonAlloc(curPos + curUp * stepHeight, -curUp, layers.m_MutiPhysicHit, distance - stepHeight, layers.m_nTerrainLayer);
                if (hitCnt > 0)
                {
                    bool bValidHit = false;
                    float nearlyDist = float.MaxValue;
                    RaycastHit curtHit = layers.m_MutiPhysicHit[0];
                    if (pIngore != null)
                    {
                        for (int i = 0; i < hitCnt; ++i)
                        {
                            if (!BaseUtil.IsSubTransform(layers.m_MutiPhysicHit[i].transform, pIngore.GetTransorm()))
                            {
                                float dist = (layers.m_MutiPhysicHit[i].point - curPos).sqrMagnitude;
                                if (dist <= nearlyDist)
                                {
                                    nearlyDist = dist;
                                    curtHit = layers.m_MutiPhysicHit[i];
                                    bValidHit = true;
                                }
                            }
                        }
                    }
                    else bValidHit = true;
                    if (bValidHit)
                    {
                        collider = curtHit.collider;
                        hitPos = curtHit.point;
                        hitUp = curtHit.normal;
                    }
                    return bValidHit;
                }
            }
            else
            {
                if (stepHeight < 0) stepHeight = ConstDef.STEP_HEIGHT_LOWER;
                if (Physics.Raycast(curPos + curUp * stepHeight, -curUp, out layers.m_PhysicHit, distance - stepHeight, layers.m_nTerrainLayer))
                {
                    hitPos = layers.m_PhysicHit.point;
                    hitUp = layers.m_PhysicHit.normal;
                    collider = layers.m_PhysicHit.collider;
                    return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public static bool IsPhysicCapsuleHit(AFramework framework, FVector3 startPos, FVector3 endPos, FFloat radius, AInstanceAble pIngore = null)
        {
            TerrainLayers layers = TerrainManager.GetTerrainLayers(framework);
            if (layers == null) return  false;

            return Physics.CheckCapsule(startPos, endPos, radius, layers.m_nTerrainLayer);
        }
        //------------------------------------------------------
        public static bool IsPhysicBoxHit(AFramework framework, FVector3 center, FVector3 halfExtents, AInstanceAble pIngore = null)
        {
            TerrainLayers layers = TerrainManager.GetTerrainLayers(framework);
            if (layers == null) return false;
                return Physics.CheckBox(center, halfExtents, Quaternion.identity, layers.m_nTerrainLayer);

        }
        //------------------------------------------------------
        public static bool IsPhysicBoxHit(AFramework framework, FVector3 center, FVector3 halfExtents, FQuaternion quaternion, AInstanceAble pIngore = null)
        {
            TerrainLayers layers = TerrainManager.GetTerrainLayers(framework);
            if (layers == null) return false;
   //         if (pIngore == null)
                return Physics.CheckBox(center, halfExtents, quaternion, layers.m_nTerrainLayer);
        }
        //------------------------------------------------------
        public static int CalcPhysicOverlap(AFramework framework, FVector3 curPos, FFloat radius, out Collider[] colliders)
        {
            colliders = null;
            TerrainLayers layers = TerrainManager.GetTerrainLayers(framework);
            if (layers == null) return 0;
            if (layers.m_PhysicColliders == null) layers.m_PhysicColliders = new Collider[2];
            int hitCnt = Physics.OverlapSphereNonAlloc(curPos, radius, layers.m_PhysicColliders, layers.m_nTerrainLayer);
            if (hitCnt > 0) colliders = layers.m_PhysicColliders;
            return hitCnt;
        }
        //------------------------------------------------------
        public static bool CheckPhysicSphape(AFramework framework, FVector3 curPos, FFloat radius, AInstanceAble pIngore = null)
        {
            TerrainLayers layers = TerrainManager.GetTerrainLayers(framework);
            if (layers == null) return false;
            if(pIngore!=null)
            {
                if (layers.m_PhysicColliders == null) layers.m_PhysicColliders = new Collider[2];
                int hitCnt = Physics.OverlapSphereNonAlloc(curPos, radius, layers.m_PhysicColliders, layers.m_nTerrainLayer);
                bool bValidHit = false;
                for(int i =0; i < hitCnt; ++i)
                {
                    if (!BaseUtil.IsSubTransform(layers.m_PhysicColliders[i].transform, pIngore.GetTransorm()))
                    {
                        bValidHit = true;
                        break;
                    }
                }
                return bValidHit;
            }
            else
                return Physics.CheckSphere(curPos, radius, layers.m_nTerrainLayer);
        }
        //------------------------------------------------------
        public static bool HasGridMaps(AFramework framework)
        {
            TerrainLayers terrains = TerrainManager.GetTerrainLayers(framework);
            if (terrains == null) return false;
            if (terrains.m_vGridMaps != null && terrains.m_vGridMaps.Count > 0 && TerrainGridSize.sqrMagnitude > 0)
                return true;
            return false;
        }
        //------------------------------------------------------
        public static bool SimpleMove(AFramework framework, FVector3 worldPos, FVector3 vSpeed, float fRadius = 0)
        {
            if (vSpeed.sqrMagnitude<=0)
            {
                return !IsTerrainBlock(framework, worldPos);
            }
            if (Mathf.Abs(fRadius) >= 0.01)
                vSpeed += vSpeed.normalized * fRadius;
            if (IsTerrainBlock(framework,worldPos + vSpeed )) return false;
            return true;
        }
        //------------------------------------------------------
        public static bool IsTerrainBlock(AFramework framework, FVector3 worldPos)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return true;
            if(terrainLayer.m_vGridMaps == null || terrainLayer.m_vGridMaps.Count<=0) return false;
            Vector3Int gridWorld = WorldPosToWorldGrid3D(worldPos);
            TerrainGridMap gridMap = TerrainLayers.GetGridMap(framework, gridWorld, true);
            if (gridMap == null) return true;
            GridNode gridNode = gridMap.GetGridByWorld(gridWorld);
            if (gridNode.IsValid && gridNode.IsState(EGridState.Obstacle)) return true;
            return false;
        }
        //------------------------------------------------------
        public static Vector2Int WorldPosToWorldGrid(Vector3 worldPos)
        {
            int halfX = TerrainGridSize.x * 50;
            int halfY = TerrainGridSize.y * 50;
            float tX = Mathf.Floor(worldPos.x * 100);
            float tY = Mathf.Floor(worldPos.z * 100);
            int gX = Mathf.RoundToInt(tX / halfX) * 100;
            int gY = Mathf.RoundToInt(tY / halfY) * 100;
            return new Vector2Int(gX, gY);
        }
        //------------------------------------------------------
        public static Vector3Int WorldPosToWorldGrid3D(Vector3 worldPos)
        {
            int halfX = TerrainGridSize.x * 50;
            int halfY = TerrainGridSize.y * 50;
            float tX = Mathf.Floor(worldPos.x * 100);
            float tY = Mathf.Floor(worldPos.z * 100);
            int gX = Mathf.RoundToInt(tX / halfX) * 100;
            int gY = Mathf.RoundToInt(tY / halfY) * 100;
            return new Vector3Int(gX, WorldPosYToGridPosY(worldPos.y), gY);
        }
        //-----------------------------------------------------
        public static Vector3 WorldGridToWorldPos(Vector2Int worldGrid, int posY = 0)
        {
            float halfX = TerrainGridSize.x * 0.5f;
            float halfY = TerrainGridSize.y * 0.5f;
            float py = GridPosYToWorldPosY(posY);
            return new Vector3(worldGrid.x * 0.01f * halfX, py, worldGrid.y * 0.01f * halfY);
        }
        //-----------------------------------------------------
        public static Vector3 WorldGridToWorldPos(Vector3Int worldGrid)
        {
            float halfX = TerrainGridSize.x * 0.5f;
            float halfY = TerrainGridSize.y * 0.5f;
            float py = GridPosYToWorldPosY(worldGrid.y);
            return new Vector3(worldGrid.x * 0.01f * halfX, py, worldGrid.z * 0.01f * halfY);
        }
        //------------------------------------------------------
        public static Vector3 PathWorldGridToWorldPos(Vector3Int pathPoint, float offsetY = 0.01f)
        {
            return WorldGridToWorldPos(pathPoint) + new Vector3(TerrainGridSize.x * 0.25f, offsetY, TerrainGridSize.y * 0.25f);
        }
        //------------------------------------------------------
        public static float GridPosYToWorldPosY(int posY)
        {
            return posY * TerrainGridSize.y * 0.5f;
        }
        //------------------------------------------------------
        public static int WorldPosYToGridPosY(float posY)
        {
            int tempY = (int)(posY * 100);
            return tempY / (TerrainGridSize.y * 50);
        }
        //------------------------------------------------------
        public static void SetGridStateByWorld(AFramework framework, Vector3Int worldGrid, EGridState state)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return;
            if (terrainLayer.m_vGridMaps == null || terrainLayer.m_vGridMaps.Count <= 0) return;
            for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
            {
                terrainLayer.m_vGridMaps[i].SetGridStateByWorld(worldGrid, state);
            }
        }
        //------------------------------------------------------
        public static void SetRegionGridState(AFramework framework, Vector3Int mapGrid, Vector2Int gridSize, EGridState state)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return;
            if (terrainLayer.m_vGridMaps == null || terrainLayer.m_vGridMaps.Count <= 0) return;
            for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
            {
                terrainLayer.m_vGridMaps[i].SetRegionGridState(mapGrid, gridSize, state);
            }
        }
        //------------------------------------------------------
        public static GridNode GetGridByWorld(AFramework framework, Vector3Int worldGrid)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return GridNode.DEF;
            if (terrainLayer.m_vGridMaps == null || terrainLayer.m_vGridMaps.Count <= 0) return GridNode.DEF;
            for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
            {
                if (terrainLayer.m_vGridMaps[i].IsContain(worldGrid))
                {
                    return terrainLayer.m_vGridMaps[i].GetGridByWorld(worldGrid);
                }
            }
            return GridNode.DEF;
        }
        //------------------------------------------------------
        public static bool CheckGridStateByWorld(AFramework framework, Vector3Int worldGrid, uint flags)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return false;
            if (terrainLayer.m_vGridMaps == null || terrainLayer.m_vGridMaps.Count <= 0) return false;
            for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
            {
                if (terrainLayer.m_vGridMaps[i].CheckGridByWorld(worldGrid, flags))
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        public static bool CheckRegionGrid(AFramework framework, Vector3Int worldGridMin, Vector3Int worldGridMax, uint flag, HashSet<Vector3Int> vIngoreGrids = null)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return false;
            if (terrainLayer.m_vGridMaps == null || terrainLayer.m_vGridMaps.Count <= 0) return false;
            for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
            {
                if (terrainLayer.m_vGridMaps[i].CheckRegionGrid(worldGridMin, worldGridMax, flag, vIngoreGrids))
                    return true;
            }
            return false;
        }
        //------------------------------------------------------
        internal static TerrainGridMap GetGridMapByID(AFramework framework, int id)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return null;
            if (terrainLayer.m_vGridMaps == null || terrainLayer.m_vGridMaps.Count <= 0) return null;
            for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
            {
                if (terrainLayer.m_vGridMaps[i].GetID() == id)
                    return terrainLayer.m_vGridMaps[i];
            }
            return null;
        }
        //------------------------------------------------------
        public static TerrainGridMap GetGridMap(AFramework framework, Vector3Int worldGrid, bool bProjectionClosely = false)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return null;
            if (terrainLayer.m_vGridMaps == null || terrainLayer.m_vGridMaps.Count <= 0) return null;
            if(bProjectionClosely)
            {
                TerrainGridMap findMap = null;
                int dist = int.MaxValue;
                for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
                {
                    int projectDist;
                    if (terrainLayer.m_vGridMaps[i].GetProjectionClosely(worldGrid, out projectDist))
                    {
                        if(projectDist <= dist)
                        {
                            dist = projectDist;
                            findMap = terrainLayer.m_vGridMaps[i];
                        }
                    }
                }
                return findMap;
            }
            else
            {
                for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
                {
                    if (terrainLayer.m_vGridMaps[i].IsContain(worldGrid))
                        return terrainLayer.m_vGridMaps[i];
                }
            }

            return null;
        }
        //------------------------------------------------------
        public static List<TerrainGridMap> GetGridMaps(AFramework framework)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return null;
            return terrainLayer.m_vGridMaps;
        }
        //------------------------------------------------------
        public static TerrainGridMap ExternGridMap(AFramework framework, Vector3Int rectMin, Vector3Int rectMax, bool bRebuild = false, bool bKeepOldStates = false)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return null;
            if (terrainLayer.m_vGridMaps == null || terrainLayer.m_vGridMaps.Count <= 0) return null;
            TerrainGridMap gridMap = null;
            for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
            {
                gridMap = terrainLayer.m_vGridMaps[i];
                if (gridMap.Extern(rectMin, rectMax, bRebuild, bKeepOldStates))
                {
                    return gridMap;
                }
            }
            return null;
        }
        //------------------------------------------------------
        public static bool AddTerrainGridLinkOffline(AFramework framework, int startMap, int endMap, TerrainGridLinkOffline linkOffline)
        {
            if (!linkOffline.IsValid() || startMap == endMap)
                return false;
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return false;
            if (terrainLayer.m_vTerrainGridOfflines == null)
            {
                terrainLayer.m_vTerrainGridOfflines = new Dictionary<long, TerrainGridLinkOffline>();
            }

            Vector3 dir = linkOffline.lineEnd - linkOffline.lineStart;
            if (dir.sqrMagnitude <= 0) return false;

            Variable2 key = new Variable2() { intVal0 = startMap, intVal1 = endMap };
            linkOffline.expandDir = Quaternion.LookRotation(dir.normalized, Vector3.up) * Vector3.right;
            terrainLayer.m_vTerrainGridOfflines[key.longValue] = linkOffline;

            key = new Variable2() { intVal0 = endMap, intVal1 = startMap };
            TerrainGridLinkOffline revertOffline = linkOffline;
            revertOffline.lineStart = linkOffline.lineEnd;
            revertOffline.lineStartTan = linkOffline.lineEndTan;
            revertOffline.startAction = linkOffline.endAction;
            revertOffline.lineEnd = linkOffline.lineStart;
            revertOffline.lineEndTan = linkOffline.lineStartTan;
            revertOffline.endAction = linkOffline.startAction;
            linkOffline.expandDir = Quaternion.LookRotation(dir.normalized, Vector3.up) * Vector3.left;
            terrainLayer.m_vTerrainGridOfflines[key.longValue] = revertOffline;

            TerrainGridMap startGridMap = GetGridMapByID(framework, startMap);
            TerrainGridMap endGridMap = GetGridMapByID(framework, endMap);
            if (startGridMap == null || endGridMap == null)
                return true;

            //! build bridge
            if (terrainLayer.m_vBridgeTerrainGridMaps == null)
                terrainLayer.m_vBridgeTerrainGridMaps = new Dictionary<int, List<TerrainGridMap>>();
            //! start
            {
                List<TerrainGridMap> gridMaps;
                if (!terrainLayer.m_vBridgeTerrainGridMaps.TryGetValue(startMap, out gridMaps))
                {
                    gridMaps = new List<TerrainGridMap>();
                    terrainLayer.m_vBridgeTerrainGridMaps[startMap] = gridMaps;
                }
                if (!gridMaps.Contains(endGridMap)) gridMaps.Add(endGridMap);
            }
            //! end
            {
                List<TerrainGridMap> gridMaps;
                if (!terrainLayer.m_vBridgeTerrainGridMaps.TryGetValue(endMap, out gridMaps))
                {
                    gridMaps = new List<TerrainGridMap>();
                    terrainLayer.m_vBridgeTerrainGridMaps[endMap] = gridMaps;
                }
                if (!gridMaps.Contains(startGridMap)) gridMaps.Add(startGridMap);
            }

            return true;
        }
        //------------------------------------------------------
        public static void RemoveTerrainGridLinkOffline(AFramework framework, int startMap, int endMap)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null || terrainLayer.m_vTerrainGridOfflines == null) return;
            Variable2 key = new Variable2() { intVal0 = startMap, intVal1 = endMap };
            terrainLayer.m_vTerrainGridOfflines.Remove(key.longValue);
            key = new Variable2() { intVal0 = endMap, intVal1 = startMap };
            terrainLayer.m_vTerrainGridOfflines.Remove(key.longValue);

            if (terrainLayer.m_vBridgeTerrainGridMaps != null)
            {
                List<TerrainGridMap> gridMaps;
                if (terrainLayer.m_vBridgeTerrainGridMaps.TryGetValue(startMap, out gridMaps))
                {
                    for (int i = 0; i < gridMaps.Count; ++i)
                    {
                        if (gridMaps[i].GetID() == endMap)
                        {
                            gridMaps.RemoveAt(i);
                            break;
                        }
                    }
                }
                gridMaps = null;
                if (terrainLayer.m_vBridgeTerrainGridMaps.TryGetValue(endMap, out gridMaps))
                {
                    for (int i = 0; i < gridMaps.Count; ++i)
                    {
                        if (gridMaps[i].GetID() == startMap)
                        {
                            gridMaps.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public static TerrainGridLinkOffline GetTerrainGridLinkOffline(AFramework framework, int startMap, int endMap)
        {
            TerrainGridLinkOffline linkOffline = TerrainGridLinkOffline.DEF;
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null || terrainLayer.m_vTerrainGridOfflines == null) return linkOffline;
            Variable2 key = new Variable2() { intVal0 = startMap, intVal1 = endMap };
            if (terrainLayer.m_vTerrainGridOfflines.TryGetValue(key.longValue, out linkOffline))
                return linkOffline;
            linkOffline = TerrainGridLinkOffline.DEF;
            return linkOffline;
        }
        //------------------------------------------------------
        public static bool AddGridMap(AFramework framework, TerrainGridMap gridMap)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return false;
            if (terrainLayer.m_vGridMaps == null)
                terrainLayer.m_vGridMaps = new List<TerrainGridMap>();
            else
            {
                for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
                {
                    if (terrainLayer.m_vGridMaps[i] == gridMap || terrainLayer.m_vGridMaps[i].GetID() == gridMap.GetID())
                        return true;
                }
            }
            terrainLayer.m_vGridMaps.Add(gridMap);
            return true;
        }
        //------------------------------------------------------
        public static bool RemoveGridMap(AFramework framework, int id)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return false;
            if (terrainLayer.m_vGridMaps == null) return false;
            bool bRemoved = false;
            TerrainGridMap gridMap = null;
            for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
            {
                gridMap = terrainLayer.m_vGridMaps[i];
                if (gridMap.GetID() == id)
                {
                    terrainLayer.m_vGridMaps.RemoveAt(i);
                    bRemoved = true;
                    break;
                }
            }
            if (terrainLayer.m_vBridgeTerrainGridMaps != null)
            {
                terrainLayer.m_vBridgeTerrainGridMaps.Remove(id);
                List<TerrainGridMap> gridMaps;
                foreach (var db in terrainLayer.m_vBridgeTerrainGridMaps)
                {
                    gridMaps = db.Value;
                    for (int i = 0; i < gridMaps.Count; ++i)
                    {
                        if (gridMaps[i].GetID() == id)
                        {
                            gridMaps.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            return bRemoved;
        }
        //------------------------------------------------------
        internal static List<TerrainGridMap> GetBridgeTerrainGridMaps(AFramework framework, int mapID)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return null;
            if (terrainLayer.m_vBridgeTerrainGridMaps == null) return null;
            List<TerrainGridMap> gridMaps;
            if (terrainLayer.m_vBridgeTerrainGridMaps.TryGetValue(mapID, out gridMaps))
                return gridMaps;
            return null;
        }
        //------------------------------------------------------
        public static void BuildGridMap(AFramework framework, bool bRebuild = false)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return;
            if (terrainLayer.m_vGridMaps == null) return;
            for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
            {
                terrainLayer.m_vGridMaps[i].Build(bRebuild);
            }
        }
        //------------------------------------------------------
        public static void ClearGridMaps(AFramework framework)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return;
            if (terrainLayer.m_vGridMaps != null)
            {
                for (int i = 0; i < terrainLayer.m_vGridMaps.Count; ++i)
                {
                    terrainLayer.m_vGridMaps[i].Destroy();
                }
                terrainLayer.m_vGridMaps.Clear();
            }
            if (terrainLayer.m_vBridgeTerrainGridMaps != null)
                terrainLayer.m_vBridgeTerrainGridMaps.Clear();
            if (terrainLayer.m_vTerrainGridOfflines != null)
                terrainLayer.m_vTerrainGridOfflines.Clear();
        }
        //------------------------------------------------------
        public void Update(float fFrameTime)
        {
            if(m_vPathSearcherCompletes!=null)
            {
                for(int i =0; i < m_vPathSearcherCompletes.Count; ++i)
                {
                    m_vPathSearcherCompletes[i].DoCallback();
                }
                m_vPathSearcherCompletes.Clear();
            }
//             if (m_vPathSearchers != null)
//             {
//                 IPathSearcher search;
//                 for (int i = 0; i < m_vPathSearchers.Count; ++i)
//                 {
//                     search = m_vPathSearchers[i];
//                     if (search.IsValid())
//                     {
//                         search.StartSearch();
//                         search.DoCallback();
//                     }
//                 }
//                 m_vPathSearchers.Clear();
//             }
            OnInnerUpdate(fFrameTime);
        }
        //------------------------------------------------------
        public bool OnJobUpdate(float fFrame)
        {
            if (m_vPathSearchers != null)
            {
                IPathSearcher search;
                while(m_vPathSearchers.Count>0)
                {
                    search = m_vPathSearchers[0];
                    m_vPathSearchers.RemoveAt(0);
                    if (search.IsValid())
                    {
                        search.StartSearch();
                        m_vPathSearcherCompletes.Add(search);
                    }
                }
            }
            return true;
        }
        //------------------------------------------------------
        protected virtual void OnInnerUpdate(float fFrameTime) { }
        //------------------------------------------------------
#if UNITY_EDITOR
        public static void DebugGridMap(AFramework framework)
        {
            TerrainLayers terrainLayer = TerrainManager.GetTerrainLayers(framework);
            if (terrainLayer == null) return;

            Color color = Gizmos.color;
            if (terrainLayer.m_vGridMaps != null)
            {
                for (int m = 0; m < terrainLayer.m_vGridMaps.Count; ++m)
                {
                    TerrainGridMap gridMap = terrainLayer.m_vGridMaps[m];

                    UnityEditor.Handles.Label(WorldGridToWorldPos(gridMap.GetWorldCenter()), "地图[" + gridMap.GetID() + "]");
                    var grids = gridMap.GetGrids();
                    if (grids != null)
                    {
                        Vector3 half = new Vector3(TerrainGridSize.x * 0.25f, 0, TerrainGridSize.y * 0.25f);
                        Vector2Int mapGrid = Vector2Int.zero;
                        for (int i = 0; i < grids.Length; ++i)
                        {
                            mapGrid = gridMap.IndexToMapGrid(i);
                            if (grids[i].gridState != 0)
                            {
                                Vector3Int worldGrid = gridMap.MapGridToWorldGrid(mapGrid);
                                Gizmos.color = color;
                                if (grids[i].IsState(EGridState.Road))
                                {
                                    Gizmos.color = Color.yellow;
                                    Gizmos.DrawSphere(WorldGridToWorldPos(worldGrid) + half, 0.5f);
                                }
                                else if (grids[i].IsState(EGridState.Obstacle))
                                {
                                    Gizmos.color = Color.red;
                                    Gizmos.DrawSphere(WorldGridToWorldPos(worldGrid) + half, 0.5f);
                                }
                                else if (grids[i].IsState(EGridState.Wakeable))
                                {
                                    Gizmos.color = Color.white;
                                    Gizmos.DrawSphere(WorldGridToWorldPos(worldGrid) + half, 0.5f);
                                }
                                Gizmos.color = color;
                            }
                        }
                    }
                }
            }
            if (terrainLayer.m_vTerrainGridOfflines != null)
            {
                Gizmos.color = Color.cyan;
                Vector3 hallCell = new Vector3(TerrainGridSize.x, 0, TerrainGridSize.y) * 0.25f;
                foreach (var db in terrainLayer.m_vTerrainGridOfflines)
                {
                    TerrainGridLinkOffline linkeOffline = db.Value;
                    if (linkeOffline.expandDir.sqrMagnitude <= 0) continue;

                    Vector3 start = WorldGridToWorldPos(linkeOffline.lineStart) + hallCell;
                    Gizmos.DrawWireCube(start, Vector3.one*0.5f);

                    Vector3 end = WorldGridToWorldPos(linkeOffline.lineEnd) + hallCell;
                    Gizmos.DrawWireCube(end, Vector3.one * 0.5f);

                    Vector3 lastPt = linkeOffline.Sample(0);
                    float line = GridPosYToWorldPosY(linkeOffline.lineWidth);
                    float fTime = 0;
                    while (fTime <= 1)
                    {
                        Vector3 pt = linkeOffline.Sample(fTime);
                        Gizmos.DrawLine(lastPt + hallCell, pt + hallCell);
                        if (linkeOffline.lineWidth < 20)
                        {
                            for (int j = 1; j <= linkeOffline.lineWidth; ++j)
                                Gizmos.DrawLine(lastPt + hallCell + linkeOffline.expandDir * GridPosYToWorldPosY(j), pt + hallCell + linkeOffline.expandDir * GridPosYToWorldPosY(j));
                        }
                        lastPt = pt;
                        fTime += 0.1f;
                    }
                    Gizmos.DrawLine(start, start + linkeOffline.expandDir * line);
                    Gizmos.DrawLine(end, end + linkeOffline.expandDir * line);
                }
                Gizmos.color = color;
            }
        }
#endif
    }
}