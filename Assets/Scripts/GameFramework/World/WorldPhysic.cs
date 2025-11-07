/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	SpatialNode
作    者:	HappLI
描    述:	世界节点
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
using RVO;

#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
using System.Collections.Generic;
using UnityEngine;
using RVO;

namespace Framework.Core
{
    public class WorldPhysic
    {
        internal class ScanerObser
        {
            public AWorldNode pNode;
            public IList<KeyValuePair<FFloat, RVO.RVOObstacle>> obstacleNeighbors_ = new List<KeyValuePair<FFloat, RVO.RVOObstacle>>(4);
            public HashSet<int> obstacleInsideSets = new HashSet<int>(4);
            public void Clear()
            {
                pNode = null;
                obstacleNeighbors_.Clear();
                obstacleInsideSets.Clear();
            }
            public FVector3 GetPosition()
            {
                return pNode.GetPosition();
            }
            internal void insertObstacleNeighbor(RVO.RVOObstacle obstacle, FFloat rangeSq)
            {
                RVO.RVOObstacle nextObstacle = obstacle.next;

                FFloat distSq = RVO.RVOMath.distSqPointLineSegment(obstacle.point, nextObstacle.point, GetPosition());

                if (distSq < rangeSq)
                {
                    obstacleNeighbors_.Add(new KeyValuePair<FFloat, RVO.RVOObstacle>(distSq, obstacle));

                    int i = obstacleNeighbors_.Count - 1;

                    while (i != 0 && distSq < obstacleNeighbors_[i - 1].Key)
                    {
                        obstacleNeighbors_[i] = obstacleNeighbors_[i - 1];
                        --i;
                    }
                    obstacleNeighbors_[i] = new KeyValuePair<FFloat, RVO.RVOObstacle>(distSq, obstacle);
                }
            }
            internal void insertCircleObstacleNeighbor(RVO.RVOObstacle obstacle, FFloat distSq, FFloat rangeSq)
            {
                if (!obstacle.IsCircle)
                    return;

                obstacleNeighbors_.Add(new KeyValuePair<FFloat, RVO.RVOObstacle>(distSq, obstacle));

                int i = obstacleNeighbors_.Count - 1;

                while (i != 0 && distSq < obstacleNeighbors_[i - 1].Key)
                {
                    obstacleNeighbors_[i] = obstacleNeighbors_[i - 1];
                    --i;
                }
                obstacleNeighbors_[i] = new KeyValuePair<FFloat, RVO.RVOObstacle>(distSq, obstacle);
            }
            internal bool IsInsideObstacle(FVector3 position, FFloat radius, out FVector3 accSpeed)
            {
                accSpeed = FVector3.zero;
                if (obstacleInsideSets.Count <= 0)
                    return false;
                foreach (var kv in obstacleNeighbors_)
                {
                    var obs = kv.Value;
                    if (!obstacleInsideSets.Contains(kv.Value.group))
                        continue;

                    var nearestPoint = obs.GetNearestPointOnPolygon(position, radius);
                    accSpeed = nearestPoint - position;

                    return true;
                }
                return false;
            }
        }
        struct Line
        {
            public FVector3 direction;
            public FVector3 point;
        }
        private World m_pWorld;
        FFloat m_fTimeStep = 0.25f;
        List<Line> m_vOrcaLines = new List<Line>(4);
        List<Line> m_vOrcaObsLines = new List<Line>(4);

        int m_nStaticGenID = 1;
        private bool m_bDirtyObs = false;
        List<RVO.RVOObstacle> m_vStaticObs = new List<RVO.RVOObstacle>(16);
        List<RVO.RVOObstacle> m_vCicleStaticObs = new List<RVO.RVOObstacle>(16);
        List<AWorldNode> m_vDynamicNodes = new List<AWorldNode>(4);
        Stack<RVO.RVOObstacle> m_vObsPool = new Stack<RVO.RVOObstacle>(32);

        ScanerObser m_ObsScaner = new ScanerObser();
        List<FVector3> m_vTemp = new List<FVector3>(4);
        //------------------------------------------------------
        public WorldPhysic(World world)
        {
            m_pWorld = world;
        }
        //------------------------------------------------------
        public void Update(FFloat fFrame)
        {

        }
        //------------------------------------------------------
        public RVO.RVOObstacle MallocObstacle()
        {
            if (m_vObsPool.Count > 0) return m_vObsPool.Pop();
            return new RVO.RVOObstacle();
        }
        //------------------------------------------------------
        public void FreeObstacle(RVO.RVOObstacle pObs)
        {
            pObs.Destroy();
            if (m_vObsPool.Count >= 32) return;
            m_vObsPool.Push(pObs);
        }
        //------------------------------------------------------
        public long AddStaticObstacle(FVector3 position, List<Vector3> vVerteices)
        {
            if (vVerteices.Count < 2)
                return -1;
            int begin = m_nStaticGenID;
            int obstacleNo = m_vStaticObs.Count;
            for (int i = 0; i < vVerteices.Count; ++i)
            {
                RVO.RVOObstacle obs = MallocObstacle();
                obs.point = vVerteices[i] + position;
                obs.id = m_nStaticGenID++;
                obs.group = begin;
                obs.IsCircle = false;

                if (i != 0)
                {
                    obs.prev = m_vStaticObs[m_vStaticObs.Count - 1];
                    obs.prev.next = obs;
                }

                if (i == vVerteices.Count - 1)
                {
                    obs.next = m_vStaticObs[obstacleNo];
                    obs.next.prev = obs;
                }
                obs.unitDir = RVO.RVOMath.normalize(vVerteices[(i == (vVerteices.Count - 1) ? 0 : i + 1)] - vVerteices[i]);

                if (vVerteices.Count == 2)
                    obs.IsConvex = true;
                else
                    obs.IsConvex = (RVO.RVOMath.leftOf(vVerteices[(i == 0 ? vVerteices.Count - 1 : i - 1)] + position, vVerteices[i] + position, vVerteices[(i == vVerteices.Count - 1 ? 0 : i + 1)] + position) >= 0.0f);

                m_vStaticObs.Add(obs);
                m_bDirtyObs = true;
            }
            int end = m_nStaticGenID;
            long key =(long)begin | ((long)end << 32);
            return key;
        }
        //------------------------------------------------------
        public long AddStaticObstacle(FVector3 position, FFloat radius)
        {
            if (radius <= 0)
                return -1;
            int begin = m_nStaticGenID;
            RVO.RVOObstacle obs = MallocObstacle();
            obs.point = position;
            obs.IsCircle = true;
            obs.IsConvex = false;
            obs.group = begin;
            obs.id = m_nStaticGenID++;

            obs.unitDir = FVector3.one * radius;
            m_vCicleStaticObs.Add(obs);
            int end = m_nStaticGenID;
            long key = (long)begin | ((long)end << 32);
            return key;
        }
        //------------------------------------------------------
        public void RemoveStaticObstacle(long key)
        {
            int begin = (int)(key & 0xFFFFFFFF);
            int end = (int)(key >> 32);
            bool bRemoved = false;
            for (int i = 0; i < m_vStaticObs.Count;)
            {
                int userId = m_vStaticObs[i].id;
                if (userId >= begin && userId < end)
                {
                    FreeObstacle(m_vStaticObs[i]);
                    m_vStaticObs.RemoveAt(i);
                    m_bDirtyObs = true;
                    bRemoved = true;
                    continue;
                }
                ++i;
            }
            if (bRemoved) return;
            for (int i = 0; i < m_vCicleStaticObs.Count;)
            {
                int userId = m_vCicleStaticObs[i].id;
                if (userId >= begin && userId < end)
                {
                    FreeObstacle(m_vCicleStaticObs[i]);
                    m_vCicleStaticObs.RemoveAt(i);
                    bRemoved = true;
                    continue;
                }
                ++i;
            }
        }
        //------------------------------------------------------
        bool IsEqualObs(ref RVO.RVOObstacle o1, ref RVO.RVOObstacle o2)
        {
            return o1.id == o2.id;
        }
        //------------------------------------------------------
        void ComputeObstacleNeighbors(Framework.Core.KDTree.WorldKDTree pKDTree, ScanerObser scaner, FVector3 position, FFloat radius, FFloat rangeSq)
        {
            pKDTree.computeObstacleNeighbors(m_ObsScaner, rangeSq);
            for (int i = 0; i < m_vCicleStaticObs.Count; ++i)
            {
                RVO.RVOObstacle obstacle = m_vCicleStaticObs[i];
                FVector3 relativePosition = obstacle.point - position;
#if USE_FIXEDMATH
                float distSq = RVOMath.sqr(radius + obstacle.unitDir.x);
#else
                float distSq = (radius + obstacle.unitDir.x) * (radius + obstacle.unitDir.x);
#endif
                if (relativePosition.sqrMagnitude <= distSq)
                {
                    scaner.obstacleInsideSets.Add(obstacle.group);
                    scaner.insertCircleObstacleNeighbor(obstacle, distSq, rangeSq);
                }
            }
        }
        //------------------------------------------------------
        public FVector3 ComputerNewVelocity(Framework.Core.KDTree.WorldKDTree pKDTree, AWorldNode pNode, FVector3 prefNormalVelocity, FFloat maxSpeed, FFloat timeHorizon, FFloat timeHorizonObs, out bool isCollisioned)
        {
            isCollisioned = false;
            FVector3 backUpSpeed = prefNormalVelocity;
            if (prefNormalVelocity.sqrMagnitude > 1) prefNormalVelocity.Normalize();
            m_ObsScaner.Clear();
            m_vDynamicNodes.Clear();
            m_vTemp.Clear();

            m_vOrcaLines.Clear();
            FFloat invTimeHorizon = 1 / timeHorizon;
            FVector3 position = pNode.GetPosition();
            FVector3 velocity = pNode.GetRVOVelocity();
            velocity.y = 0;
            FFloat radius = pNode.GetPhysicRadius();

            bool isStoped = false;
#if USE_FIXEDMATH
            if (RVOMath.IsZeroXZ(prefNormalVelocity))
#else
            if (Mathf.Abs(prefNormalVelocity.x)<=0.0001f && Mathf.Abs(prefNormalVelocity.z)<=0.0001f)
#endif
            {
                // if the velocity is zero, we use the preferred normal velocity
                isStoped = true;
            }
            if (maxSpeed <= 0.0f) maxSpeed = 1.0f;

            m_ObsScaner.Clear();
            m_ObsScaner.pNode = pNode;
            FFloat rangeSq = RVO.RVOMath.sqr(timeHorizonObs * maxSpeed + radius);
            if (m_bDirtyObs)
            {
                m_bDirtyObs = false;
                pKDTree.BuildObstacleTree(this, m_vStaticObs);
            }
            ComputeObstacleNeighbors(pKDTree, m_ObsScaner, position, radius, rangeSq);
            if (radius > 0)
                m_pWorld.ComputeCollisionByRadius(pNode, ref m_vDynamicNodes, radius);
            isCollisioned = m_vDynamicNodes.Count > 0 || m_ObsScaner.obstacleInsideSets.Count > 0;

            if (isCollisioned && isStoped)
            {
                if (m_ObsScaner.IsInsideObstacle(position, radius, out var speed))
                {
                    maxSpeed *= 5;
                    backUpSpeed = speed * maxSpeed;
                    prefNormalVelocity = backUpSpeed;
                    if (prefNormalVelocity.sqrMagnitude > 1) prefNormalVelocity.Normalize();
                }
            }

            AWorldNode other;

#if USE_FIXEDMATH
            FFloat zeroFloat = FFloat.zero;
#else
            FFloat zeroFloat = 0.0f;
#endif
            FFloat invTimeHorizonObst = 1.0f / timeHorizonObs;
            //! Create obstacle ORCA lines. 
            for (int i = 0; i < m_ObsScaner.obstacleNeighbors_.Count; ++i)
            {
                Line line;
                RVO.RVOObstacle obstacle1 = m_ObsScaner.obstacleNeighbors_[i].Value;
                if (obstacle1.IsCircle)
                {
                    FVector3 obstacleCenter = obstacle1.point;
                    float obstacleRadius = obstacle1.unitDir.x; // unitDir.x 存储圆半径
                    FVector3 relativePosition = obstacleCenter - position;
                    relativePosition.y = 0.0f;
                    float distSq = relativePosition.sqrMagnitude;
                    float combinedRadius = radius + obstacleRadius;
                    float combinedRadiusSq = combinedRadius * combinedRadius;

                    FVector3 u;
                    if (distSq > combinedRadiusSq)
                    {
                        //calculate the line direction and point
                        FVector3 w = velocity - invTimeHorizonObst * relativePosition;
                        float wLengthSq = w.sqrMagnitude;
                        float dotProduct = Vector3.Dot(w, relativePosition);

                        if (dotProduct < 0.0f && dotProduct * dotProduct > combinedRadiusSq * wLengthSq)
                        {
                            // project to the circle
                            float wLength = Mathf.Sqrt(wLengthSq);
                            FVector3 unitW = w / wLength;
                            line.direction = new Vector3(unitW.z, 0, -unitW.x);
                            u = (combinedRadius * invTimeHorizonObst - wLength) * unitW;
                        }
                        else
                        {
                            // project to the line
                            float leg = Mathf.Sqrt(distSq - combinedRadiusSq);
                            if (FVector3.Cross(relativePosition, w).y > 0.0f)
                            {
                                // left
                                line.direction = new FVector3(
                                    relativePosition.x * leg - relativePosition.z * combinedRadius,
                                    0,
                                    relativePosition.x * combinedRadius + relativePosition.z * leg
                                ) / distSq;
                            }
                            else
                            {
                                // right
                                line.direction = -new FVector3(
                                    relativePosition.x * leg + relativePosition.z * combinedRadius,
                                    0,
                                    -relativePosition.x * combinedRadius + relativePosition.z * leg
                                ) / distSq;
                            }
                            float dotProduct2 = Vector3.Dot(velocity, line.direction);
                            u = dotProduct2 * line.direction - velocity;
                        }
                    }
                    else
                    {
                        // the velocity is inside the circle, project to the tangent line
                        FFloat invTimeStep = 1.0f / m_fTimeStep;
                        FVector3 w = velocity - invTimeStep * relativePosition;
                        FFloat wLength = Mathf.Max(0.00001f, w.magnitude);
                        FVector3 unitW = w / wLength;
                        line.direction = new FVector3(unitW.z, 0, -unitW.x);
                        u = (combinedRadius * invTimeStep - wLength) * unitW;
                    }

                    line.point = velocity + 0.5f * u;
                    m_vOrcaLines.Add(line);
                    continue;
                }
                RVO.RVOObstacle obstacle2 = obstacle1.next;

                FVector3 relativePosition1 = obstacle1.point - position;
                FVector3 relativePosition2 = obstacle2.point - position;

                /*
                 * Check if velocity obstacle of obstacle is already taken care of by
                 * previously constructed obstacle ORCA lines.
                 */
                bool alreadyCovered = false;
                for (int j = 0; j < m_vOrcaLines.Count; ++j)
                {
                    if (RVO.RVOMath.det(invTimeHorizonObst * relativePosition1 - m_vOrcaLines[j].point, m_vOrcaLines[j].direction) - invTimeHorizonObst * radius >= -RVO.RVOMath.RVOEPSILON &&
                        RVO.RVOMath.det(invTimeHorizonObst * relativePosition2 - m_vOrcaLines[j].point, m_vOrcaLines[j].direction) - invTimeHorizonObst * radius >= -RVO.RVOMath.RVOEPSILON)
                    {
                        alreadyCovered = true;
                        break;
                    }
                }
                if (alreadyCovered)
                {
                    continue;
                }

                // Not yet covered. Check for collisions.
                FFloat distSq1 = RVO.RVOMath.absSq(relativePosition1);
                FFloat distSq2 = RVO.RVOMath.absSq(relativePosition2);
                FFloat radiusSq = RVO.RVOMath.sqr(radius);

                FVector3 obstacleVector = obstacle2.point - obstacle1.point;
                FFloat s = (-RVO.RVOMath.mul(relativePosition1, obstacleVector)) / RVO.RVOMath.absSq(obstacleVector);
                FFloat distSqLine = RVO.RVOMath.absSq(-relativePosition1 - s * obstacleVector);

                if (s < 0.0f && distSq1 <= radiusSq)
                {
                    /* Collision with left vertex. Ignore if non-convex. */
                    if (obstacle1.IsConvex)
                    {
                        line.point = FVector3.zero;
                        line.direction = RVO.RVOMath.normalize(new FVector3(-relativePosition1.z, zeroFloat, relativePosition1.x));
                        m_vOrcaLines.Add(line);
                    }

                    continue;
                }
                else if (s > 1.0f && distSq2 <= radiusSq)
                {
                    /* Collision with right vertex. Ignore if non-convex
                     * or if it will be taken care of by neighoring obstace */
                    if (obstacle2.IsConvex && RVO.RVOMath.det(relativePosition2, obstacle2.unitDir) >= 0.0f)
                    {
                        line.point = FVector3.zero;
                        line.direction = RVO.RVOMath.normalize(new FVector3(-relativePosition2.z, zeroFloat, relativePosition2.x));
                        m_vOrcaLines.Add(line);
                    }

                    continue;
                }
                else if (s >= 0.0f && s <= 1.0f && distSqLine <= radiusSq)
                {
                    /* Collision with obstacle segment. */
                    line.point = FVector3.zero;
                    line.direction = -obstacle1.unitDir;
                    m_vOrcaLines.Add(line);
                    continue;
                }

                /*
                 * No collision.
                 * Compute legs. When obliquely viewed, both legs can come from a single
                 * vertex. Legs extend cut-off line when nonconvex vertex.
                 */

                FVector3 leftLegDirection, rightLegDirection;

                if (s < 0.0f && distSqLine <= radiusSq)
                {
                    /*
                     * RvoObstacle viewed obliquely so that left vertex
                     * defines velocity obstacle.
                     */
                    if (!obstacle1.IsConvex)
                    {
                        /* Ignore obstacle. */
                        continue;
                    }

                    obstacle2 = obstacle1;

                    FFloat leg1 = RVO.RVOMath.sqrt(distSq1 - radiusSq);
                    leftLegDirection = new FVector3(relativePosition1.x * leg1 - relativePosition1.z * radius, zeroFloat, relativePosition1.x * radius + relativePosition1.z * leg1) / distSq1;
                    rightLegDirection = new FVector3(relativePosition1.x * leg1 + relativePosition1.z * radius, zeroFloat, -relativePosition1.x * radius + relativePosition1.z * leg1) / distSq1;
                }
                else if (s > 1.0f && distSqLine <= radiusSq)
                {
                    /*
                     * RvoObstacle viewed obliquely so that
                     * right vertex defines velocity obstacle.
                     */
                    if (!obstacle2.IsConvex)
                    {
                        /* Ignore obstacle. */
                        continue;
                    }

                    obstacle1 = obstacle2;

                    FFloat leg2 = RVO.RVOMath.sqrt(distSq2 - radiusSq);
                    leftLegDirection = new FVector3(relativePosition2.x * leg2 - relativePosition2.z * radius, zeroFloat, relativePosition2.x * radius + relativePosition2.z * leg2) / distSq2;
                    rightLegDirection = new FVector3(relativePosition2.x * leg2 + relativePosition2.z * radius, zeroFloat, -relativePosition2.x * radius + relativePosition2.z * leg2) / distSq2;
                }
                else
                {
                    /* Usual situation. */
                    if (obstacle1.IsConvex)
                    {
                        FFloat leg1 = RVO.RVOMath.sqrt(distSq1 - radiusSq);
                        leftLegDirection = new FVector3(relativePosition1.x * leg1 - relativePosition1.z * radius, zeroFloat, relativePosition1.x * radius + relativePosition1.z * leg1) / distSq1;
                    }
                    else
                    {
                        /* Left vertex non-convex; left leg extends cut-off line. */
                        leftLegDirection = -obstacle1.unitDir;
                    }

                    if (obstacle2.IsConvex)
                    {
                        FFloat leg2 = RVO.RVOMath.sqrt(distSq2 - radiusSq);
                        rightLegDirection = new FVector3(relativePosition2.x * leg2 + relativePosition2.z * radius, zeroFloat, -relativePosition2.x * radius + relativePosition2.z * leg2) / distSq2;
                    }
                    else
                    {
                        /* Right vertex non-convex; right leg extends cut-off line. */
                        rightLegDirection = obstacle1.unitDir;
                    }
                }

                /*
                 * Legs can never point into neighboring edge when convex vertex,
                 * take cutoff-line of neighboring edge instead. If velocity projected on
                 * "foreign" leg, no constraint is added.
                 */

                RVO.RVOObstacle leftNeighbor = obstacle1.prev;
                // const RvoObstacle*const leftNeighbor = obstacle1.prevObstacle_;

                bool isLeftLegForeign = false;
                bool isRightLegForeign = false;

                if (obstacle1.IsConvex && RVO.RVOMath.det(leftLegDirection, -leftNeighbor.unitDir) >= 0.0f)
                {
                    /* Left leg points into obstacle. */
                    leftLegDirection = -leftNeighbor.unitDir;
                    isLeftLegForeign = true;
                }

                if (obstacle2.IsConvex && RVO.RVOMath.det(rightLegDirection, obstacle2.unitDir) <= 0.0f)
                {
                    /* Right leg points into obstacle. */
                    rightLegDirection = obstacle2.unitDir;
                    isRightLegForeign = true;
                }

                /* Compute cut-off centers. */
                FVector3 leftCutoff = invTimeHorizonObst * (obstacle1.point - position);
                FVector3 rightCutoff = invTimeHorizonObst * (obstacle2.point - position);
                FVector3 cutoffVec = rightCutoff - leftCutoff;

                /* Project current velocity on velocity obstacle. */

                /* Check if current velocity is projected on cutoff circles. */
#if USE_FIXEDMATH
                FFloat t = obstacle1 == obstacle2 ? FFloat.half : (RVO.RVOMath.mul(velocity - leftCutoff, cutoffVec) / RVO.RVOMath.absSq(cutoffVec));
#else
                FFloat t = obstacle1 == obstacle2 ? 0.5f : (RVO.RVOMath.mul(velocity - leftCutoff, cutoffVec) / RVO.RVOMath.absSq(cutoffVec));
#endif
                FFloat tLeft = RVO.RVOMath.mul(velocity - leftCutoff, leftLegDirection);
                FFloat tRight = RVO.RVOMath.mul(velocity - rightCutoff, rightLegDirection);

                if ((t < 0.0f && tLeft < 0.0f) || (obstacle1 == obstacle2 && tLeft < 0.0f && tRight < 0.0f))
                {
                    /* Project on left cut-off circle. */
                    FVector3 unitW = RVO.RVOMath.normalize(velocity - leftCutoff);

                    line.direction = new FVector3(unitW.z, zeroFloat, -unitW.x);
                    line.point = leftCutoff + radius * invTimeHorizonObst * unitW;
                    m_vOrcaLines.Add(line);
                    continue;
                }
                else if (t > 1.0f && tRight < 0.0f)
                {
                    /* Project on right cut-off circle. */
                    FVector3 unitW = RVO.RVOMath.normalize(velocity - rightCutoff);

                    line.direction = new FVector3(unitW.z, zeroFloat, -unitW.x);
                    line.point = rightCutoff + radius * invTimeHorizonObst * unitW;
                    m_vOrcaLines.Add(line);
                    continue;
                }

                /*
                 * Project on left leg, right leg, or cut-off line, whichever is closest
                 * to velocity.
                 */
                FFloat distSqCutoff = (t < 0.0f || t > 1.0f || obstacle1 == obstacle2) ? FFloat.MinValue : RVO.RVOMath.absSq(velocity - (leftCutoff + t * cutoffVec));
                FFloat distSqLeft = ((tLeft < 0.0f) ? RVO.RVOMath.RVOEPSILON : RVO.RVOMath.absSq(velocity - (leftCutoff + tLeft * leftLegDirection)));
                FFloat distSqRight = ((tRight < 0.0f) ? RVO.RVOMath.RVOEPSILON : RVO.RVOMath.absSq(velocity - (rightCutoff + tRight * rightLegDirection)));

                if (distSqCutoff <= distSqLeft && distSqCutoff <= distSqRight)
                {
                    /* Project on cut-off line. */
                    line.direction = -obstacle1.unitDir;
                    line.point = leftCutoff + radius * invTimeHorizonObst * new FVector3(-line.direction.z, zeroFloat, line.direction.x);
                    m_vOrcaLines.Add(line);
                    continue;
                }
                else if (distSqLeft <= distSqRight)
                {
                    /* Project on left leg. */
                    if (isLeftLegForeign)
                    {
                        continue;
                    }

                    line.direction = leftLegDirection;
                    line.point = leftCutoff + radius * invTimeHorizonObst * new FVector3(-line.direction.z, zeroFloat, line.direction.x);
                    m_vOrcaLines.Add(line);
                    continue;
                }
                else
                {
                    /* Project on right leg. */
                    if (isRightLegForeign)
                    {
                        continue;
                    }

                    line.direction = -rightLegDirection;
                    line.point = rightCutoff + radius * invTimeHorizonObst * new FVector3(-line.direction.z, zeroFloat, line.direction.x);
                    m_vOrcaLines.Add(line);
                    continue;
                }
            }

            int numObstLines = m_vOrcaLines.Count;
            for (int i = 0; i < m_vDynamicNodes.Count; ++i)
            {
                other = m_vDynamicNodes[i];
                if (other == pNode) continue;
                if (other.GetPhysicRadius() <= 0 || other.IsKilled() || other.IsDestroy()) continue;
                FVector3 otherSpeed = other.GetRVOVelocity();
                otherSpeed.y = 0;
                FVector3 relativePosition = other.GetPosition() - position;
                relativePosition.y = 0;
                FVector3 relativeVelocity = velocity - otherSpeed;
                FFloat distSq = RVO.RVOMath.absSq(relativePosition);
                FFloat combinedRadius = radius + other.GetPhysicRadius();
                FFloat combinedRadiusSq = RVO.RVOMath.sqr(combinedRadius);

                Line line;
                FVector3 u;

                if (distSq > combinedRadiusSq)
                {
                    /* No collision. */
                    FVector3 w = relativeVelocity - invTimeHorizon * relativePosition;
                    w.y = 0;

                    /* Vector from cutoff center to relative velocity. */
                    FFloat wLengthSq = RVO.RVOMath.absSq(w);
                    FFloat dotProduct1 = FVector3.Dot(w, relativePosition);

                    if (dotProduct1 < 0.0f && RVO.RVOMath.sqr(dotProduct1) > combinedRadiusSq * wLengthSq)
                    {
                        /* Project on cut-off circle. */
                        FFloat wLength = RVO.RVOMath.sqrt(wLengthSq);
                        FVector3 unitW = w / wLength;

                        line.direction = new FVector3(unitW.z, unitW.y, -unitW.x);
                        u = (combinedRadius * invTimeHorizon - wLength) * unitW;
                    }
                    else
                    {
                        /* Project on legs. */
                        FFloat leg = RVO.RVOMath.sqrt(distSq - combinedRadiusSq);

                        if (RVO.RVOMath.det(relativePosition, w) > 0.0f)
                        {
                            /* Project on left leg. */
                            line.direction = new FVector3(relativePosition.x * leg - relativePosition.z * combinedRadius, zeroFloat, relativePosition.x * combinedRadius + relativePosition.z * leg) / distSq;
                        }
                        else
                        {
                            /* Project on right leg. */
                            line.direction = -new FVector3(relativePosition.x * leg + relativePosition.z * combinedRadius, zeroFloat, -relativePosition.x * combinedRadius + relativePosition.z * leg) / distSq;
                        }

                        FFloat dotProduct2 = FVector3.Dot(relativeVelocity, line.direction);
                        u = dotProduct2 * line.direction - relativeVelocity;
                    }
                }
                else
                {
                    /* Collision. Project on cut-off circle of time timeStep. */
                    FFloat invTimeStep = 1.0f / m_fTimeStep;

                    /* Vector from cutoff center to relative velocity. */
                    FVector3 w = relativeVelocity - invTimeStep * relativePosition;

#if USE_FIXEDMATH
                    FFloat wLength = FMath.Max(0.00001f, RVO.RVOMath.abs(w));
#else
                    FFloat wLength = System.Math.Max(0.00001f, RVO.RVOMath.abs(w));
#endif
                    FVector3 unitW = w / wLength;

                    line.direction = new FVector3(unitW.z, unitW.y, -unitW.x);
                    u = (combinedRadius * invTimeStep - wLength) * unitW;
                }

                line.point = velocity + 0.5f * u;
                m_vOrcaLines.Add(line);
            }
            if (m_vOrcaLines.Count <= 0)
            {
                return backUpSpeed;
            }

            FVector3 newVeocity = FVector3.zero;
            int lineFail = linearProgram2(m_vOrcaLines, maxSpeed, prefNormalVelocity * maxSpeed, false, ref newVeocity);
            if (lineFail < m_vOrcaLines.Count)
            {
                linearProgram3(m_vOrcaLines, numObstLines, lineFail, maxSpeed, ref newVeocity);
            }
            m_vOrcaLines.Clear();
            m_vOrcaObsLines.Clear();
            newVeocity.y = 0;
            m_vTemp.Clear();
            pNode.SetRVOVelocity(newVeocity);
            return newVeocity;
        }
        //-------------------------------------------------
        private bool linearProgram1(IList<Line> lines, int lineNo, FFloat radius, FVector3 optVelocity, bool directionOpt, ref FVector3 result)
        {
            FFloat dotProduct = FVector3.Dot(lines[lineNo].point, lines[lineNo].direction);
            FFloat discriminant = RVO.RVOMath.sqr(dotProduct) + RVO.RVOMath.sqr(radius) - RVO.RVOMath.absSq(lines[lineNo].point);

            if (discriminant < 0.0f)
            {
                /* Max speed circle fully invalidates line lineNo. */
                return false;
            }

            FFloat sqrtDiscriminant = RVO.RVOMath.sqrt(discriminant);
            FFloat tLeft = -dotProduct - sqrtDiscriminant;
            FFloat tRight = -dotProduct + sqrtDiscriminant;

            for (int i = 0; i < lineNo; ++i)
            {
                FFloat denominator = RVO.RVOMath.det(lines[lineNo].direction, lines[i].direction);
                FFloat numerator = RVO.RVOMath.det(lines[i].direction, lines[lineNo].point - lines[i].point);

                if (RVO.RVOMath.fabs(denominator) <= RVO.RVOMath.RVOEPSILON)
                {
                    /* Lines lineNo and i are (almost) parallel. */
                    if (numerator < 0.0f)
                    {
                        return false;
                    }

                    continue;
                }

                FFloat t = numerator / denominator;

                if (denominator >= 0.0f)
                {
                    /* Line i bounds line lineNo on the right. */
#if USE_FIXEDMATH
                    tRight = FMath.Min(tRight, t);
#else
                    tRight = System.Math.Min(tRight, t);
#endif
                }
                else
                {
                    /* Line i bounds line lineNo on the left. */
#if USE_FIXEDMATH
                    tLeft = FMath.Max(tLeft, t);
#else
                    tLeft = System.Math.Max(tLeft, t);
#endif
                }

                if (tLeft > tRight)
                {
                    return false;
                }
            }

            if (directionOpt)
            {
                /* Optimize direction. */
                if (FVector3.Dot(optVelocity, lines[lineNo].direction) > 0.0f)
                {
                    /* Take right extreme. */
                    result = lines[lineNo].point + tRight * lines[lineNo].direction;
                }
                else
                {
                    /* Take left extreme. */
                    result = lines[lineNo].point + tLeft * lines[lineNo].direction;
                }
            }
            else
            {
                /* Optimize closest point. */
                FFloat t = FVector3.Dot(lines[lineNo].direction, (optVelocity - lines[lineNo].point));

                if (t < tLeft)
                {
                    result = lines[lineNo].point + tLeft * lines[lineNo].direction;
                }
                else if (t > tRight)
                {
                    result = lines[lineNo].point + tRight * lines[lineNo].direction;
                }
                else
                {
                    result = lines[lineNo].point + t * lines[lineNo].direction;
                }
            }

            return true;
        }
        //------------------------------------------------------
        private int linearProgram2(IList<Line> lines, FFloat radius, FVector3 optVelocity, bool directionOpt, ref FVector3 result)
        {
            if (directionOpt)
            {
                /*
                 * Optimize direction. Note that the optimization velocity is of
                 * unit length in this case.
                 */
                result = optVelocity * radius;
            }
            else if (RVO.RVOMath.absSq(optVelocity) > RVO.RVOMath.sqr(radius))
            {
                /* Optimize closest point and outside circle. */
                result = RVO.RVOMath.normalize(optVelocity) * radius;
            }
            else
            {
                /* Optimize closest point and inside circle. */
                result = optVelocity * radius;
            }

            for (int i = 0; i < lines.Count; ++i)
            {
                if (RVO.RVOMath.det(lines[i].direction, lines[i].point - result) > 0.0f)
                {
                    /* Result does not satisfy constraint i. Compute new optimal result. */
                    FVector3 tempResult = result;
                    if (!linearProgram1(lines, i, radius, optVelocity, directionOpt, ref result))
                    {
                        result = tempResult;

                        return i;
                    }
                }
            }

            return lines.Count;
        }
        //------------------------------------------------------
        private void linearProgram3(IList<Line> lines, int numObstLines, int beginLine, FFloat radius, ref FVector3 result)
        {
            FFloat distance = 0;

            for (int i = beginLine; i < lines.Count; ++i)
            {
                if (RVO.RVOMath.det(lines[i].direction, lines[i].point - result) > distance)
                {
                    /* Result does not satisfy constraint of line i. */
                    IList<Line> projLines = new List<Line>();
                    for (int ii = 0; ii < numObstLines; ++ii)
                    {
                        projLines.Add(lines[ii]);
                    }

                    for (int j = numObstLines; j < i; ++j)
                    {
                        Line line;

                        FFloat determinant = RVO.RVOMath.det(lines[i].direction, lines[j].direction);

                        if (RVO.RVOMath.fabs(determinant) <= RVO.RVOMath.RVOEPSILON)
                        {
                            /* Line i and line j are parallel. */
                            if (FVector3.Dot(lines[i].direction, lines[j].direction) > 0.0f)
                            {
                                /* Line i and line j point in the same direction. */
                                continue;
                            }
                            else
                            {
                                /* Line i and line j point in opposite direction. */
                                line.point = 0.5f * (lines[i].point + lines[j].point);
                            }
                        }
                        else
                        {
                            line.point = lines[i].point + (RVO.RVOMath.det(lines[j].direction, lines[i].point - lines[j].point) / determinant) * lines[i].direction;
                        }

                        line.direction = RVO.RVOMath.normalize(lines[j].direction - lines[i].direction);
                        projLines.Add(line);
                    }

                    FVector3 tempResult = result;
                    if (linearProgram2(projLines, radius, new FVector3(-lines[i].direction.z, lines[i].direction.y, lines[i].direction.x), true, ref result) < projLines.Count)
                    {
                        /*
                         * This should in principle not happen. The result is by
                         * definition already in the feasible region of this
                         * linear program. If it fails, it is due to small
                         * floating point error, and the current result is kept.
                         */
                        result = tempResult;
                    }

                    distance = RVO.RVOMath.det(lines[i].direction, lines[i].point - result);
                }
            }
        }
        //------------------------------------------------------
        public void Clear()
        {
        }
        //------------------------------------------------------
        public void DrawDebug()
        {
            for (int i = 0; i < m_vStaticObs.Count; ++i)
            {
                if (m_vStaticObs[i].next != null)
                {
                    UnityEngine.Gizmos.DrawLine(m_vStaticObs[i].point, m_vStaticObs[i].next.point);
                }
            }
        }
    }
}
