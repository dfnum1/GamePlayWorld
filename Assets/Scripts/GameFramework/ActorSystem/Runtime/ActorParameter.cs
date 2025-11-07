/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	ActorParameter
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FVector2 = UnityEngine.Vector2;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
namespace Framework.Core
{
    public interface IActorAttrDirtyCallback
    {
        void OnActorAttrDirty(Actor pActor, byte type, float oldValue, float newValue);
    }
#if USE_ACTORSYSTEM
    public class ActorParameter : TypeObject
    {
        IUserData m_pConfigData;
        Dictionary<byte, FFloat> m_vAttributes = null;
        Actor m_pActor;

        byte m_HpAttrType = 1;
        private List<IActorAttrDirtyCallback> m_vCallbacks = null;

        public ActorParameter(Actor pActor)
        {
            m_pActor = pActor;
            m_vAttributes = new Dictionary<byte, FFloat>(2);
        }
        //--------------------------------------------------------
        internal void AddCallback(IActorAttrDirtyCallback callback)
        {
            if (m_vCallbacks == null) m_vCallbacks = new List<IActorAttrDirtyCallback>(2);
            if (m_vCallbacks.Contains(callback)) return;
            m_vCallbacks.Add(callback);
        }
        //--------------------------------------------------------
        internal void RemoveCallback(IActorAttrDirtyCallback callback)
        {
            if (m_vCallbacks == null) return;
            m_vCallbacks.Remove(callback);
        }
        //--------------------------------------------------------
        public void SetCfgData(IUserData cfgData)
        {
            m_pConfigData = cfgData;
        }
        //--------------------------------------------------------
        public IUserData GetCfgData()
        {
            return m_pConfigData;
        }
        //--------------------------------------------------------
        internal uint GetConfigID()
        {
            return 0;
        }
        //--------------------------------------------------------
        public ExternEngine.FFloat GetModelHeight()
        {
            return 1.0f;
        }
        //--------------------------------------------------------
        public void SetHpAttrType(byte type)
        {
            m_HpAttrType = type;
        }
        //--------------------------------------------------------
        internal void SetAttrs(byte[] attiTypes, FFloat[] values)
        {
            if (attiTypes == null || values == null)
                return;
            if (attiTypes.Length != values.Length)
                return;

            for(int i =0; i < attiTypes.Length; ++i)
            {
                SetAttr(attiTypes[i], values[i]);
            }
        }
        //--------------------------------------------------------
        internal void SetAttrs(byte[] attiTypes, int[] values)
        {
            if (attiTypes == null || values == null)
                return;
            if (attiTypes.Length != values.Length)
                return;

            for (int i = 0; i < attiTypes.Length; ++i)
            {
                SetAttr(attiTypes[i], values[i]);
            }
        }
        //--------------------------------------------------------
        internal void SetAttr(byte type, FFloat value)
        {
            FFloat oldValue = 0;
            if(!m_vAttributes.TryGetValue(type, out oldValue))
                oldValue = -1;
            m_vAttributes[type] = value;
            DoAttrDirtyCall(type, oldValue, m_vAttributes[type]);
        }
        //--------------------------------------------------------
        internal FFloat GetAttr(byte type, float defVal = 0)
        {
            if (m_vAttributes.TryGetValue(type, out var val))
                return val;
            return defVal;
        }
        //--------------------------------------------------------
        internal void RemoveAttr(byte type)
        {
            m_vAttributes.Remove(type);
        }
        //--------------------------------------------------------
        internal void AppendAttrs(byte[] attiTypes, FFloat[] values)
        {
            if (attiTypes == null || values == null)
                return;
            if (attiTypes.Length != values.Length)
                return;
            for (int i = 0; i < attiTypes.Length; ++i)
            {
                AppendAttr(attiTypes[i], values[i]);
            }
        }
        //--------------------------------------------------------
        internal void AppendAttr(byte type, FFloat value)
        {
            FFloat oldValue = 0;
            if (m_vAttributes.TryGetValue(type, out oldValue))
            {
                m_vAttributes[type] = oldValue + value;
            }
            else
            {
                oldValue = -1;
                m_vAttributes[type] = value;
            }
            DoAttrDirtyCall(type, oldValue, m_vAttributes[type]);
            m_pActor.GetGameModule().OnActorAttrDirty(m_pActor, type, value, oldValue);
        }
        //--------------------------------------------------------
        internal void SubAttrs(byte[] attiTypes, FFloat[] values)
        {
            if (attiTypes == null || values == null)
                return;
            if (attiTypes.Length != values.Length)
                return;
            for (int i = 0; i < attiTypes.Length; ++i)
            {
                SubAttr(attiTypes[i], values[i]);
            }
        }
        //--------------------------------------------------------
        internal void SubAttr(byte type, FFloat value, bool bLowerZero = false)
        {
            if (m_vAttributes.TryGetValue(type, out var val))
            {
                FFloat oldValue = val;
                if(bLowerZero)
                {
                    if(val < value)
                        m_vAttributes[type] = 0;
                    else
                        m_vAttributes[type] = val - value;
                }
                else
                    m_vAttributes[type] = val - value;
                DoAttrDirtyCall(type, oldValue, m_vAttributes[type]);
            }
        }
        //--------------------------------------------------------
        internal void ClearAttrs()
        {
            if(m_vAttributes!=null)
                m_vAttributes.Clear();
        }
        //--------------------------------------------------------
        void DoAttrDirtyCall(byte type, FFloat oldValue, FFloat newValue)
        {
            if (oldValue == newValue)
                return;

            m_pActor.GetGameModule().OnActorAttrDirty(m_pActor, type, oldValue, newValue);

            if (m_vCallbacks == null)
                return;
            foreach(var db in m_vCallbacks)
            {
                db.OnActorAttrDirty(m_pActor, type, oldValue, newValue);
            }
        }
        //--------------------------------------------------------
        public void Update(float fDeltaTime)
        {
            if(m_HpAttrType >0)
            {
                if(!m_pActor.IsKilled() && m_vAttributes.TryGetValue(m_HpAttrType, out var attrValue) && attrValue <= 0)
                {
                    m_pActor.SetKilled(true);
                }
            }
        }
        //--------------------------------------------------------
        public void Clear()
        {
            ClearAttrs();
            m_HpAttrType = 1;
            m_pConfigData = null;
            if (m_vCallbacks != null) m_vCallbacks.Clear();
        }
        //--------------------------------------------------------
        public override void Destroy()
        {
            Clear();
        }
    }
#endif
}
