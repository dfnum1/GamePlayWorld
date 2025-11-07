/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	ActionStateProperty
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.Base;
using UnityEngine;

namespace Framework.Core
{
    [System.Serializable]
    public struct PhysicPropertyData
    {
        public enum EFlag : byte
        {
            [PluginDisplay("曲线位移重置")] FrameReset = 1<<0,
            [PluginDisplay("忽略作用朝向")] IngoreDir = 1<<1,
            [PluginDisplay("攻击-受击相对方向")] AttackTargetDir = 1 << 2,
            [PluginDisplay("地形区域校正")] TerrainAdjust = 1 << 3,
        }
        [System.Serializable]
        public struct PhysicData
        {
            public bool bUseHorSpeed;
            public float fHorSpeed;
            public float fToHorSpeed;

            public bool bUseVerSpeed;
            public float fVerSpeed;
            public float fToVerSpeed;

            public bool bUseDeepSpeed;
            public float fDeepSpeed;
            public float fToDeepSpeed;

            public bool bUseGravity;
            public float fGravity;
            public float fToGravity;

            public bool bUseFraction;
            public float fFraction;
            public float fToFraction;
        }
        public bool bUseFrame;
        public bool bUseEnd;
        public SplineData.KeyFrame[] Frames;

        public PhysicData physic;
        [DisplayEnumBitGUI(typeof(EFlag), false), PluginDisplay("标志")]
        public byte propertyFlags;

        public float fTriggerTime;
        public string strName;

#if UNITY_EDITOR
        [System.NonSerialized]
        public bool bExpand;
        [System.NonSerialized]
        public bool bDeling;
#endif

        public void Reset()
        {
            Frames = null;
            fTriggerTime = 0;
        }

        public bool IsValid
        {
            get
            {
                if(bUseFrame)
                {
                    return Frames != null && Frames.Length > 0;
                }
                return physic.bUseFraction || physic.bUseGravity || physic.bUseHorSpeed || physic.bUseVerSpeed;
            }
        }

        public float FrameDuration
        {
            get
            {
                if(bUseFrame && Frames != null )
                {
                    float fTime = 0;
                    for(int i = 0; i < Frames.Length; ++i)
                    {
                        fTime = Mathf.Max(fTime, Frames[i].time);
                    }
                    return fTime;
                }
                return 0;
            }
        }

        public bool IsFlag(EFlag flag)
        {
            return (propertyFlags & (int)flag) != 0;
        }

        public void Copy(PhysicPropertyData pData )
        {
#if UNITY_EDITOR
            strName = pData.strName;
#endif
            bUseFrame = pData.bUseFrame;
            bUseEnd = pData.bUseEnd;
            propertyFlags = pData.propertyFlags;
            if (pData.Frames != null && pData.Frames.Length>0)
            {
                Frames = new SplineData.KeyFrame[pData.Frames.Length];
                for(int i = 0; i < pData.Frames.Length; ++i)
                {
                    Frames[i] = new SplineData.KeyFrame();
                    Frames[i].time = pData.Frames[i].time;
                    Frames[i].position = pData.Frames[i].position;
                    Frames[i].eulerAngle = pData.Frames[i].eulerAngle;
                    Frames[i].inTan = pData.Frames[i].inTan;
                    Frames[i].outTan = pData.Frames[i].outTan;
                }
            }
            physic = new PhysicData();
            physic.bUseHorSpeed = pData.physic.bUseHorSpeed;
            physic.fHorSpeed = pData.physic.fHorSpeed;
            physic.bUseVerSpeed = pData.physic.bUseVerSpeed;
            physic.fVerSpeed = pData.physic.fVerSpeed;
            physic.bUseGravity = pData.physic.bUseGravity;
            physic.fGravity = pData.physic.fGravity;
            physic.bUseFraction = pData.physic.bUseFraction;
            physic.fFraction = pData.physic.fFraction;
        }
#if UNITY_EDITOR
        [System.NonSerialized]
        System.Collections.Generic.HashSet<string> m_vIngores;
        public void OnInspector(System.Object param = null)
        {
            if (m_vIngores == null) m_vIngores = new System.Collections.Generic.HashSet<string>();
            m_vIngores.Clear();
            m_vIngores.Add("fTriggerTime");

            ED.DrawPropertyCore.Draw(ref this, "property", false, m_vIngores);
        }
#endif
#if UNITY_EDITOR
        public void Write(ref Framework.Data.BinaryUtil seralizer)
        {
            seralizer.WriteFloat(fTriggerTime);

            seralizer.WriteBool(physic.bUseHorSpeed);
            seralizer.WriteFloat(physic.fHorSpeed);
            seralizer.WriteFloat(physic.fToHorSpeed);

            seralizer.WriteBool(physic.bUseVerSpeed);
            seralizer.WriteFloat(physic.fVerSpeed);
            seralizer.WriteFloat(physic.fToVerSpeed);

            seralizer.WriteBool(physic.bUseDeepSpeed);
            seralizer.WriteFloat(physic.fDeepSpeed);
            seralizer.WriteFloat(physic.fToDeepSpeed);

            seralizer.WriteBool(physic.bUseGravity);
            seralizer.WriteFloat(physic.fGravity);
            seralizer.WriteFloat(physic.fToGravity);
 
            seralizer.WriteBool(physic.bUseFraction);
            seralizer.WriteFloat(physic.fFraction);
            seralizer.WriteFloat(physic.fToFraction);

            seralizer.WriteBool(bUseFrame);
            seralizer.WriteBool(bUseEnd);
            seralizer.WriteByte(propertyFlags);

            if (bUseFrame)
            {
                if(Frames!=null) seralizer.WriteUshort((ushort)Frames.Length);
                else seralizer.WriteUshort(0);
                if (Frames.Length > 0)
                {
                    for(int i =0; i < Frames.Length; ++i)
                    {
                        seralizer.WriteFloat(Frames[i].time);
                        seralizer.WriteVector3(Frames[i].position);
                        seralizer.WriteVector3(Frames[i].eulerAngle);
                        seralizer.WriteVector3(Frames[i].inTan);
                        seralizer.WriteVector3(Frames[i].outTan);
                    }
                }
            }
        }
#endif
        public void Read(ref Framework.Data.BinaryUtil seralizer)
        {
            fTriggerTime = seralizer.ToFloat();

            physic.bUseHorSpeed = seralizer.ToBool();
            physic.fHorSpeed = seralizer.ToFloat();
            seralizer.ToFloat(physic.fToHorSpeed);

            physic.bUseVerSpeed = seralizer.ToBool();
            physic.fVerSpeed = seralizer.ToFloat();
            physic.fToVerSpeed = seralizer.ToFloat();

            physic.bUseDeepSpeed = seralizer.ToBool();
            physic.fDeepSpeed = seralizer.ToFloat();
            physic.fToDeepSpeed = seralizer.ToFloat();

            physic.bUseGravity = seralizer.ToBool();
            physic.fGravity = seralizer.ToFloat();
            physic.fToGravity =  seralizer.ToFloat();

            physic.bUseFraction = seralizer.ToBool();
            physic.fFraction = seralizer.ToFloat();
            physic.fToFraction = seralizer.ToFloat();

            bUseFrame = seralizer.ToBool();
            bUseEnd = seralizer.ToBool();
            propertyFlags = seralizer.ToByte();

            if (bUseFrame)
            {
                int cnt = seralizer.ToUshort();
                if (cnt > 0)
                {
                    Frames = new SplineData.KeyFrame[cnt];
                    for (int i = 0; i < Frames.Length; ++i)
                    {
                        SplineData.KeyFrame key = new SplineData.KeyFrame();
                        key.time = seralizer.ToFloat();
                        key.position =  seralizer.ToVec3();
                        key.eulerAngle = seralizer.ToVec3();
                        key.inTan = seralizer.ToVec3();
                        key.outTan = seralizer.ToVec3();
                        Frames[i] = key;
                    }
                }
            }
        }
    }
    //------------------------------------------------------
    public class PhysicProperty : IQuickSort<PhysicProperty>
    {
        public PhysicPropertyData propertyData;
        public bool bTriggered = false;
        public void Reset(PhysicPropertyData propData)
        {
            propertyData = propData;
            bTriggered = false;
        }
        //-----------------------------------------------------
        public int CompareTo(int userType, PhysicProperty other)
        {
            if (propertyData.fTriggerTime < other.propertyData.fTriggerTime) return -1;
            if (propertyData.fTriggerTime > other.propertyData.fTriggerTime) return 1;
            return 0;
        }
        //-----------------------------------------------------
        public void Recyle()
        {
            bTriggered = false;
        }
        //-----------------------------------------------------
        public void Destroy()
        {
        }
    }
}

