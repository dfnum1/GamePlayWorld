
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Core
{
    internal class PartObjects : TypeObject
    {
        AWorldNode m_pOwner = null;
        private int m_nGUID = 0;
        protected LinkedList<PartObject> m_vParts = null;
        public void SetOwner(AWorldNode pNode)
        {
            m_pOwner = pNode;
        }
        //------------------------------------------------------
        public int Create(string strPart, Vector3 offset, Vector3 eulerAngleOffset, float fScale = 1, string strSlot = null, ESlotBindBit bit = ESlotBindBit.All, float fLifeTime = -1, float fParticleSpeed = -1, bool bVisibleSyncOwner = true)
        {
            if (m_pOwner == null) return 0;
            if (fScale <= 0) fScale = 1;
            InstanceOperiaon pOp = FileSystemUtil.SpawnInstance(strPart, true);
            if (pOp != null)
            {
                Vector3 euler = BaseUtil.DirectionToEulersAngle(m_pOwner.GetDirection(), m_pOwner.GetUp());
                Vector3 up = m_pOwner.GetUp();
                Vector3 slotOffset = Vector3.zero;
                //    Transform pTrans = m_pOwner.GetEventSlot(strSlot, ref slotOffset);
                PartObject partObj = TypeInstancePool.Malloc<PartObject>();
                partObj.SetGuid(++m_nGUID);
                Transform pTrans = m_pOwner.FindBindSlot(strSlot);
                if (pTrans != null)
                {
                    partObj.SetBindBit((byte)bit);
                    partObj.SetVisibleSyncOwner(bVisibleSyncOwner);
                    partObj.SetSlot(strSlot);
                    partObj.SetSlotTransform(pTrans);
                    partObj.SetRotateOffset(eulerAngleOffset);
                    partObj.SetOffset(offset + slotOffset);
                    partObj.SetLifeTime(fLifeTime);
                    partObj.SetScaleFactor(fScale);
                    partObj.SetParticleSpeed(fParticleSpeed);
                    partObj.SetRenderLayer(m_pOwner.GetRenderLayer());
                }
                else
                {
                    Transform pSlot = DyncmicTransformCollects.FindTransformByName(strSlot);
                    if (pSlot == null)
                    {
                        offset += m_pOwner.GetPosition();
                        euler += eulerAngleOffset;
                    }
                    else
                    {
                        euler = eulerAngleOffset;
                    }
                    partObj.SetOffset(offset);
                    partObj.SetBindBit((byte)bit);
                    partObj.SetSlotTransform(pSlot);
                    partObj.SetParticleSpeed(fParticleSpeed);
                    partObj.SetScaleFactor(fScale);
                }
                pOp.SetByParent(RootsHandler.ParticlesRoot);
                pOp.OnSign = partObj.OnSign;
                pOp.OnCallback = partObj.OnSpawnCallback;
                pOp.SetUserData(0, m_pOwner);
                if (m_vParts == null) m_vParts = new LinkedList<PartObject>();
                m_vParts.AddLast(partObj);
                return partObj.GetGuid();
            }
            return 0;
        }
        //------------------------------------------------------
        public int Create(string strPart, Vector3 offset, Vector3 eulerAngleOffset, Transform pTrans, float fScale = 1, ESlotBindBit bit = ESlotBindBit.All, float fLifeTime = -1, float fParticleSpeed = -1, bool bVisibleSyncOwner = true)
        {
            if (m_pOwner == null) return 0;
            //    if (pTrans == null) return 0;
            if (fScale <= 0) fScale = 1;
            InstanceOperiaon pOp = FileSystemUtil.SpawnInstance(strPart, true);
            if (pOp != null)
            {
                Vector3 euler = BaseUtil.DirectionToEulersAngle(m_pOwner.GetDirection(), m_pOwner.GetUp());
                Vector3 up = m_pOwner.GetUp();

                PartObject partObj = TypeInstancePool.Malloc<PartObject>();
                partObj.SetVisibleSyncOwner(bVisibleSyncOwner);
                partObj.SetGuid(++m_nGUID);
                partObj.SetBindBit((byte)bit);
                if (pTrans) partObj.SetSlot(pTrans.name);
                else partObj.SetSlot(null);
                partObj.SetSlotTransform(pTrans);
                partObj.SetRotateOffset(eulerAngleOffset);
                partObj.SetOffset(offset);
                partObj.SetScaleFactor(fScale);
                partObj.SetLifeTime(fLifeTime);
                partObj.SetParticleSpeed(fParticleSpeed);
                partObj.SetRenderLayer(m_pOwner.GetRenderLayer());

                pOp.SetByParent(RootsHandler.ParticlesRoot);
                pOp.OnSign = partObj.OnSign;
                pOp.OnCallback = partObj.OnSpawnCallback;
                pOp.SetUserData(0, m_pOwner);

                if (m_vParts == null) m_vParts = new LinkedList<PartObject>();
                m_vParts.AddLast(partObj);
                return partObj.GetGuid();
            }
            return 0;
        }
        //------------------------------------------------------
        public void Update(float fFrame)
        {
            if (m_vParts != null)
            {
                var node = m_vParts.First;
                while(node !=null)
                {
                    var next = node.Next;
                    var part = node.Value;
                    if (!part.Update(m_pOwner, fFrame))
                    {
                        part.Free();
                        m_vParts.Remove(node);
                    }
                    node = next;
                }
            }
        }
        //------------------------------------------------------
        public void Clear()
        {
            if (m_vParts != null)
            {
                for (var node = m_vParts.First; node != null; node = node.Next)
                {
                    node.Value.Free();
                }
                m_vParts.Clear();
            }
        }
        //------------------------------------------------------
        public override void Destroy()
        {
            Clear();
            m_pOwner = null;
        }
    }
}
