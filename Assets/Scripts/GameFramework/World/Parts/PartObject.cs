
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Framework.Core
{
    public class PartObject : TypeObject, IInstanceAbleCallback
    {
        private int m_nGuid = 0;
        private bool m_bDestroyed = false;
        private bool m_bAutoDestroy = false;
        private string m_strSlot = null;
        private bool m_bVisibleSyncOwner = true;
        private byte m_bindBit = (int)ESlotBindBit.All;
        private Vector3 m_offset = Vector3.zero;
        private Vector3 m_rotateOffset = Vector3.zero;
        private float m_scaleFactor = 1;
        private AInstanceAble m_Object;
        private bool m_bSpawnTransfrom = false;
        private float m_fLifeTime = -1;
        private float m_fParticleSpeed = -1;

        protected int m_nLayerFlag = -1;
        private byte m_bFreezed = 0;

        protected Transform m_pSlotTranform;
        protected Vector3 m_SlotOffset;
        //------------------------------------------------------
        public void SetGuid(int guid)
        {
            m_nGuid = guid;
        }
        //------------------------------------------------------
        public int GetGuid()
        {
            return m_nGuid;
        }
        //------------------------------------------------------
        public string GetSlot()
        {
            return m_strSlot;
        }
        //------------------------------------------------------
        public void SetSlot(string slot)
        {
            m_strSlot = slot;
        }
        //------------------------------------------------------
        public bool IsVisibleSyncOwner()
        {
            return m_bVisibleSyncOwner;
        }
        //------------------------------------------------------
        public void SetVisibleSyncOwner(bool bSync)
        {
            m_bVisibleSyncOwner = bSync;
        }
        //------------------------------------------------------
        public void SetBindBit(byte bit)
        {
            m_bindBit = bit;
        }
        //------------------------------------------------------
        public byte GetBindBit()
        {
            return m_bindBit;
        }
        //------------------------------------------------------
        public void SetOffset(Vector3 offset)
        {
            m_offset = offset;
        }
        //------------------------------------------------------
        public Vector3 GetOffset()
        {
            return m_offset;
        }
        //------------------------------------------------------
        public void SetRotateOffset(Vector3 offset)
        {
            m_rotateOffset = offset;
        }
        //------------------------------------------------------
        public Vector3 GetRotateOffset()
        {
            return m_rotateOffset;
        }
        //------------------------------------------------------
        public void SetSlotTransform(Transform pSlot)
        {
            m_pSlotTranform = pSlot;
        }
        //------------------------------------------------------
        public AInstanceAble GetObject()
        {
            return m_Object;
        }
        //------------------------------------------------------
        public void SetScaleFactor(float f)
        {
            m_scaleFactor = f;
        }
        //------------------------------------------------------
        public bool IsSpawnTransfrom()
        {
            return m_bSpawnTransfrom;
        }
        //------------------------------------------------------
        public float GetLifeTime()
        {
            return m_fLifeTime;
        }
        //------------------------------------------------------
        public void SetLifeTime(float fTime)
        {
            m_fLifeTime = fTime;
        }
        //------------------------------------------------------
        public float GetParticleSpeed()
        {
            return m_fParticleSpeed;
        }
        //------------------------------------------------------
        public void SetParticleSpeed(float fSpeed)
        {
            m_fParticleSpeed = fSpeed;
            if (m_Object != null && m_Object is ParticleController)
            {
                ParticleController ctl = m_Object as ParticleController;
                ctl.SetSpeed(fSpeed);
            }
        }
        //------------------------------------------------------
        public virtual void SetRenderLayer(int layerFlag)
        {
            if (m_nLayerFlag == layerFlag) return;
            m_nLayerFlag = layerFlag;
            if (m_Object != null)
            {
                if (layerFlag != -1) m_Object.SetRenderLayer((int)m_nLayerFlag);
                else m_Object.ResetLayer();
            }
        }
        //-------------------------------------------------
        public void OnFreezed(bool bFreezed)
        {
            byte flag = (byte)(bFreezed ? 1 : 0);
            if (m_bFreezed == flag) return;
            m_bFreezed = flag;
            if (m_Object != null)
            {
                m_Object.OnFreezed(bFreezed);
            }
        }
        //------------------------------------------------------
        internal bool Update(AWorldNode pOwner, float fFrame)
        {
            if (m_bAutoDestroy) return false;
            if (m_Object == null) return true;
            if (pOwner.IsVisible() || !m_bVisibleSyncOwner)
            {
                bool bUseActorPos = false;
                if (m_bVisibleSyncOwner && !pOwner.IsVisible())
                {
                    bUseActorPos = true;
                }
                else
                {
                    if (m_pSlotTranform == null)
                    {
                        // m_pSlotTranform = pOwner.GetEventBindSlot(m_strSlot, ref m_SlotOffset);
                        m_pSlotTranform = pOwner.FindBindSlot(m_strSlot);
                        if (m_pSlotTranform) m_Object.SetParent(m_pSlotTranform, false);
                    }
                }


                if (!bUseActorPos && m_pSlotTranform != null)
                {
                    if ((m_bindBit & (int)ESlotBindBit.Position) != 0 || m_bSpawnTransfrom)
                        m_Object.SetPosition(m_pSlotTranform.position + m_offset + m_SlotOffset);
                    if ((m_bindBit & (int)ESlotBindBit.Rotation) != 0)
                        m_Object.SetEulerAngle(m_pSlotTranform.eulerAngles + m_rotateOffset);
                    if ((m_bindBit & (int)ESlotBindBit.Scale) != 0)
                        m_Object.SetScale(m_pSlotTranform.lossyScale * GetScaleFactor());
                    else
                        m_Object.SetScale(Vector3.one * GetScaleFactor());
                }
                else
                {
                    Vector3 vPosition = pOwner.GetPosition();
                    if (!string.IsNullOrEmpty(m_strSlot) && m_strSlot.CompareTo("CenterRode") == 0)
                        vPosition.x = 0;

                    if ((m_bindBit & (int)ESlotBindBit.Position) != 0 || m_bSpawnTransfrom)
                        m_Object.SetPosition(vPosition + m_offset + m_SlotOffset);
                    if ((m_bindBit & (int)ESlotBindBit.Rotation) != 0)
                        m_Object.SetEulerAngle(Quaternion.LookRotation(pOwner.GetDirection(), m_Object.GetUp()).eulerAngles + m_rotateOffset);
                    if ((m_bindBit & (int)ESlotBindBit.Scale) != 0)
                        m_Object.SetScale(m_Object.GetScale() * GetScaleFactor());
                    else
                        m_Object.SetScale(Vector3.one * GetScaleFactor());
                }
             //   if ((m_bindBit & (byte)ESlotBindBit.TerrainY) != 0)
             //   {
             //       Vector3 pos = m_Object.ProjectileTerrain(m_Object.GetTransorm().position);
             //       m_Object.SetPosition(pos);
             //   }
                m_bSpawnTransfrom = false;
            }
            else
            {
                //! temp,after use PRT render it
                m_Object.SetPosition(Base.ConstDef.INVAILD_POS);
            }
            return true;
        }
        //------------------------------------------------------
        public virtual void OnSign(InstanceOperiaon pOp)
        {
            AWorldNode pTemp = pOp.GetUserData<AWorldNode>(0);
            pOp.SetUsed(!m_bDestroyed && (pTemp != null && !pTemp.IsFlag(EWorldNodeFlag.Killed) && !pTemp.IsDestroy()));
        }
        //------------------------------------------------------
        public virtual void OnSpawnCallback(InstanceOperiaon pOp)
        {
            m_bSpawnTransfrom = true;
            m_Object = pOp.GetAble();
            if (m_Object == null)
                Destroy();
            else
            {
                m_Object.RegisterCallback(this);
                if (m_nLayerFlag != -1)
                    m_Object.SetRenderLayer((int)m_nLayerFlag);
                if (m_Object is ParticleController)
                {
                    ParticleController ctl = m_Object as ParticleController;
                    if (m_fParticleSpeed > 0)
                        ctl.SetSpeed(m_fParticleSpeed);
                    ctl.SetDisableCheck(true);
                    if (m_bFreezed != 0)
                    {
                        if (m_bFreezed == 1) ctl.Pause();
                        else ctl.Play();
                    }
                }
            }
        }
        //------------------------------------------------------
        public override void Destroy()
        {
            m_nLayerFlag = -1;
            m_bSpawnTransfrom = false;
            m_nGuid = 0;
            m_pSlotTranform = null;
            m_SlotOffset = Vector3.zero;
            m_bDestroyed = true;
            if (m_Object != null)
                m_Object.RecyleDestroy(2);
            m_Object = null;
            m_fLifeTime = -1;
            m_fParticleSpeed = -1;
            m_scaleFactor = 1;
            m_bFreezed = 0;
            m_bVisibleSyncOwner = true;

            m_bAutoDestroy = false;
        }
        //------------------------------------------------------
        public virtual float GetScaleFactor() { return m_scaleFactor; }

        //------------------------------------------------------
        public void OnInstanceCallback(AInstanceAble pAble, EInstanceCallbackType eType)
        {
            if (eType == EInstanceCallbackType.Destroy || eType == EInstanceCallbackType.Recyled)
            {
                m_Object = null;
                m_bAutoDestroy = true;
            }
        }
    }
}
