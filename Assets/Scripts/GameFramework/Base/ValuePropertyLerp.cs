/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	ValuePropertyLerp
作    者:	HappLI
描    述:	lerp value
*********************************************************************/
using UnityEngine;
namespace Framework.Core
{
    public interface ValueLerp
    {
        void Update(float fTime);
        void Reset();
        void Blance();
    }
    //------------------------------------------------------
    public struct FloatLerp : ValueLerp
    {
        public float value;
        public float toValue;
        public float fFactor;

        private float m_pingpongLock;
        private bool m_bPingpong;
        public void Reset()
        {
            m_pingpongLock = value;
            m_bPingpong = false;
            value = 0;
            toValue = 0;
            fFactor = 1;
        }
        public void Update(float fTime)
        {
            value = Mathf.Lerp(value, toValue, fTime * fFactor);
            if (m_bPingpong)
            {
                float t = (value - toValue);
                if (t <= 0.1f)
                {
                    value = toValue;
                    toValue = m_pingpongLock;
                    m_bPingpong = false;
                }
            }
        }

        public void SetPingpong(bool pingpong)
        {
            if (m_bPingpong) return;
            m_bPingpong = pingpong;
            if (pingpong)
            {
                m_pingpongLock = value;
            }
        }

        public void Blance()
        {
            m_bPingpong = false;
            m_pingpongLock = toValue;
            value = toValue;
        }

        public bool IsArrived(float factor = 0.1f)
        {
            return Mathf.Abs(value - toValue) <= factor;
        }
    }
    //------------------------------------------------------
    public struct Vector2Lerp : ValueLerp
    {
        public Vector2 value;
        public Vector2 toValue;
        public float fFactor;
        public void Reset()
        {
            value = Vector2.zero;
            toValue = Vector2.zero;
            fFactor = 1;
        }
        public void Update(float fTime)
        {
            value = Vector2.Lerp(value, toValue, fTime * fFactor);
        }
        public void Blance()
        {
            value = toValue;
        }
    }
    //------------------------------------------------------
    public struct Vector3Lerp : ValueLerp
    {
        public Vector3 value;
        public Vector3 toValue;
        public float fFactor;

        private Vector3 m_pingpongLock;
        private bool m_bPingpong;
        public void Reset()
        {
            fFactor = 1;
            m_bPingpong = false;
            m_pingpongLock = Vector3.zero;
            value = Vector3.zero;
            toValue = Vector3.zero;
        }
        public void Update(float fTime)
        {
            value = Vector3.Lerp(value, toValue, fTime * fFactor);
            if (m_bPingpong)
            {
                float t = (value - toValue).sqrMagnitude;
                if(t <= 0.1f)
                {
                    value = toValue;
                    toValue = m_pingpongLock;
                    m_bPingpong = false;
                }
            }
        }

        public void SetPingpong(bool pingpong)
        {
            if (m_bPingpong) return;
            m_bPingpong = pingpong;
            if (pingpong)
            {
                m_pingpongLock = value;
            }
        }
        public void Blance()
        {
            m_bPingpong = false;
            m_pingpongLock = toValue;
            value = toValue;
        }

        public bool IsArrived(int axis=-1, float factor = 0.1f)
        {
            if(axis<0 || axis> 3)  return BaseUtil.Equal(value, toValue, factor);
            return Mathf.Abs(value[axis] - toValue[axis]) <= factor;
        }
    }
    //------------------------------------------------------
    public struct Vector4Lerp : ValueLerp
    {
        public Vector4 value;
        public Vector4 toValue;
        public float fFactor;
        public void Reset()
        {
            fFactor = 1;
            value = Vector4.zero;
            toValue = Vector4.zero;
        }
        public void Update(float fTime)
        {
            value = Vector4.Lerp(value, toValue, fTime * fFactor);
        }
        public void Blance()
        {
            value = toValue;
        }
    }
    //------------------------------------------------------
    public struct ColorLerp : ValueLerp
    {
        public Color value;
        public Color toValue;
        public float fFactor;
        public void Reset()
        {
            fFactor = 1;
            value = Color.black;
            toValue = Color.black;
        }
        public void Update(float fTime)
        {
            value = Color.Lerp(value, toValue, fTime * fFactor);
        }
        public void Blance()
        {
            value = toValue;
        }
    }
    //------------------------------------------------------
    public struct QuaternionLerp : ValueLerp
    {
        public Quaternion value;
        public Quaternion toValue;
        public float fFactor;
        public void Reset()
        {
            fFactor = 1;
            value = Quaternion.identity;
            toValue = Quaternion.identity;
        }
        public void Update(float fTime)
        {
            value = Quaternion.Lerp(value, toValue, fTime * fFactor);
        }
        public void Blance()
        {
            value = toValue;
        }
    }
}

