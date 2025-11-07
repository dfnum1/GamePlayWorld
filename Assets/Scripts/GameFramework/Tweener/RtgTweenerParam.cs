/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	UIAnimatorGroup
作    者:	HappLI
描    述:	UI 动画
*********************************************************************/
using System.Collections.Generic;
using Framework.Base;
using UnityEngine;

namespace Framework.RtgTween
{
    public enum  EEaseType
    {
        [PluginDisplay("线性")] RTG_LINEAR,
        [PluginDisplay("正炫")] RTG_SINE,
        [PluginDisplay("Quint")] RTG_QUINT,
        [PluginDisplay("Quart")] RTG_QUART,
        [PluginDisplay("Quad")] RTG_QUAD,
        [PluginDisplay("爆炸")] RTG_EXPO,
        [PluginDisplay("橡皮圈")] RTG_ELASTIC,
        [PluginDisplay("立方")] RTG_CUBIC,
        [PluginDisplay("圆形")] RTG_CIRC,
        [PluginDisplay("弹跳")] RTG_BOUNCE,
        [PluginDisplay("回弹")] RTG_BACK,
        [PluginDisplay("无")] RTG_NUM
    }
    //------------------------------------------------------
    public enum EQuationType
    {
       [PluginDisplay("缓和进入")] RTG_EASE_IN,
       [PluginDisplay("缓和退出")] RTG_EASE_OUT,
       [PluginDisplay("缓和进出")] RTG_EASE_IN_OUT
    };
    //------------------------------------------------------
    public enum ETweenStatus
    {
        Start,
        Steping,
        Complete,
    }
    public delegate void OnTweenerDelegate(RtgTweenerParam param, ETweenStatus status);
    //------------------------------------------------------
    [System.Serializable]
    public struct RtgTweenerProperty
    {
        public float property1;
        public float property2;
        public float property3;
        public float property4;

        public void setFloat(float va)
        {
            property1 = va;
        }
        public void setVector2(Vector2 vec2)
        {
            property1 = vec2.x;
            property2 = vec2.y;
        }
        public void setVector3(Vector3 vec3)
        {
            property1 = vec3.x;
            property2 = vec3.y;
            property3 = vec3.z;
        }
        public void setVector4(Vector4 vec3)
        {
            property1 = vec3.x;
            property2 = vec3.y;
            property3 = vec3.z;
            property4 = vec3.w;
        }
        public void setColor(Color col)
        {
            property1 = col.r;
            property2 = col.g;
            property3 = col.b;
            property4 = col.a;
        }

        public void setAlpha(float a)
        {
            property4 = a;
        }

        public Vector2 toScreen()
        {
            return new Vector2(property1*Screen.width, property2*Screen.height);
        }
        public Vector2 toVector2()
        {
            return new Vector2(property1, property2);
        }
        public Vector3 toVector3()
        {
            return new Vector3(property1, property2, property3);
        }
        public Vector4 toVector4()
        {
            return new Vector4(property1, property2, property3, property4);
        }
        public Color toColor()
        {
            return new Color(property1, property2, property3, property4);
        }
        public float toAlpha()
        {
            return property4;
        }

        public void Reset()
        {
            property1 = property2 = property3 = property4 = 0;
        }
    }
    //------------------------------------------------------
    public enum ETweenPropertyType
    {
        UserDef = 0,
        Position,
        Scale,
        Rotate,
        Color,
        Pivot,
        Alpha,
    }

    //public class TweenerParamJsonData
    //{
    //    public int property;
    //    public RtgTweenerProperty from;
    //    public RtgTweenerProperty to;

    //    public bool bLocal;
    //    public bool useBackup;
    //    public bool backupReover;

    //    public float time;
    //    public int transition;
    //    public int equation;
    //    public bool pingpong;

    //    public float delay;
    //    public int loop;
    //    public int delay_times;

    //    public float finalValue;
    //    public float initialValue;

    //    public UnityEngine.Object pBy;

    //    [System.NonSerialized]
    //    public UnityEngine.Object pController;

    //    [System.NonSerialized]
    //    public int tweenerID;
    //    [System.NonSerialized]
    //    public int runtimeLoop;
    //    [System.NonSerialized]
    //    public int runtime_delay_times;
    //    [System.NonSerialized]
    //    public long runtime;
    //    [System.NonSerialized]
    //    public bool started;

    //    [System.NonSerialized]
    //    public OnTweenerDelegate listerner;

    //    [System.NonSerialized]
    //    public RtgTweenerProperty tweeningValue;

    //    public float runtime_finalValue;
    //    public float runtime_initialValue;

    //    [System.NonSerialized]
    //    private RtgTweenerProperty m_Backup;
    //    [System.NonSerialized]
    //    private bool m_bBackuped;

    //    [System.NonSerialized]
    //    private Vector3 m_PivotOffset;
    //}
    //------------------------------------------------------
    [System.Serializable]
    public struct RtgTweenerParam
    {
        public ETweenPropertyType property;
        public RtgTweenerProperty from;
        public RtgTweenerProperty to;

        public bool bLocal;
        public bool useBackup;
        public bool backupReover;

        public float time;
        public EEaseType transition;
        public EQuationType equation;
        public bool pingpong;

        public float delay;
        public int loop;
        public int delay_times;

        public float finalValue;
        public float initialValue;
        public AnimationCurve lerpCurve;

        public UnityEngine.Object pBy;

        [System.NonSerialized]
        public UnityEngine.Object pController;

        [System.NonSerialized]
        public int tweenerID;
        [System.NonSerialized]
        public int runtimeLoop;
        [System.NonSerialized]
        public int runtime_delay_times;
        [System.NonSerialized]
        public long runtime;
        [System.NonSerialized]
        public bool started;

        [System.NonSerialized]
        public OnTweenerDelegate listerner;

        [System.NonSerialized]
        public RtgTweenerProperty tweeningValue;

        public float runtime_finalValue;
        public float runtime_initialValue;

        [System.NonSerialized]
        private RtgTweenerProperty m_Backup;
        [System.NonSerialized]
        private bool m_bBackuped;

        [System.NonSerialized]
        private Vector3 m_PivotOffset ;

        [System.NonSerialized]
        public IUserData pUserData;

        [System.NonSerialized]
        private Dictionary<UnityEngine.Behaviour, RtgTweenerProperty> m_vGraphics;
        //------------------------------------------------------
        public RtgTweenerParam(UnityEngine.Object pController, float ptime, RtgTweenerProperty from, RtgTweenerProperty to, EEaseType ptransition = EEaseType.RTG_EXPO, EQuationType pequation = EQuationType.RTG_EASE_OUT, float delay = 0)
        {
            pUserData = null;
            this.property = ETweenPropertyType.UserDef;
            this.tweenerID = -1;
            this.pController = pController;
            this.time = ptime;
            this.transition = ptransition;
            this.equation = pequation;
            this.pingpong = false;
            this.from = from;
            this.to = to;
            this.delay = delay;
            this.loop = 1;
            this.listerner = null;
            this.bLocal = true;
            this.pBy = null;
            this.useBackup = false;
            this.backupReover = false;
            this.delay_times = 0;

            this.runtime_delay_times = 0;
            this.runtimeLoop = 0;
            this.runtime = 0;
            this.started = false;
            this.tweeningValue = from;
            this.m_Backup = new RtgTweenerProperty();

            this.finalValue = 1;
            this.initialValue = 0;
            this.lerpCurve = null;

            this.m_bBackuped = false;

            this.m_PivotOffset = Vector3.zero;

            this.runtime_finalValue = 1;
            this.runtime_initialValue = 0;

            m_vGraphics = null;
        }
        //------------------------------------------------------
        public void Reset()
        {
            this.runtimeLoop = 0;
            this.runtime = 0;
            this.started = false;
            this.tweeningValue = from;
         //   this.m_bBackuped = false;

            this.runtime_finalValue = this.finalValue;
            this.runtime_initialValue = this.initialValue;

            this.tweenerID = 0;
         //   this.pController = null;
        }
        //------------------------------------------------------
        public void SetPivotOffset(Vector3 offset)
        {
            m_PivotOffset = offset;
        }
        //------------------------------------------------------
        public Vector3 GetPivotOffset()
        {
            return m_PivotOffset;
        }
        //------------------------------------------------------
        public UnityEngine.Object GetController()
        {
            if (pBy != null) return pBy;
            return pController;
        }
        //------------------------------------------------------
        public RtgTweenerProperty getBackup()
        {
            return m_Backup;
        }
        //------------------------------------------------------
        public RtgTweenerProperty BackUp()
        {
            if (m_bBackuped) return m_Backup;
            if (pController == null) return m_Backup;
            m_bBackuped = true;
            if (m_vGraphics != null) m_vGraphics.Clear();
            this.runtime_finalValue = this.finalValue;
            this.runtime_initialValue = this.initialValue;
            m_Backup = new RtgTweenerProperty();
            if (GetController() == null) return m_Backup;
            if (property == ETweenPropertyType.Position)
            {
                Transform transform = GetTransform();
                if (transform) m_Backup.setVector3(bLocal ? transform.localPosition : transform.position);
            }
            else if (property == ETweenPropertyType.Rotate)
            {
                Transform transform = GetTransform();
                if (transform) m_Backup.setVector3(bLocal ? transform.localEulerAngles : transform.eulerAngles);
            }
            else if (property == ETweenPropertyType.Scale)
            {
                Transform transform = GetTransform();
                if (transform) m_Backup.setVector3(transform.localScale);
            }
            else if (property == ETweenPropertyType.Pivot)
            {
                RectTransform transform = GetRectTransform();
                if (transform)
                {
                    m_Backup.setVector2(transform.pivot);
                    Vector2 backup_pivot = transform.pivot;
                    transform.pivot = to.toVector2();
                    Vector2 offset = (to.toVector2() - backup_pivot);
                    offset.x *= transform.sizeDelta.x;
                    offset.y *= transform.sizeDelta.y;
                    transform.offsetMin += offset;
                    transform.offsetMax += offset;
                }
            }
            else if (property == ETweenPropertyType.Color)
            {
                m_vGraphics = UI.UIGraphicUtil.BackupGraphic<UnityEngine.UI.Graphic>(m_vGraphics, GetTransform());
            }
            else if (property == ETweenPropertyType.Alpha)
            {
                CanvasGroup canvasGroup = GetCanvasGroup();
                if (canvasGroup == null)
                    m_vGraphics = UI.UIGraphicUtil.BackupGraphic<UnityEngine.CanvasGroup>(m_vGraphics, GetTransform());
                else
                    m_Backup.property4 = canvasGroup.alpha;
            }
            return m_Backup;
        }
        //------------------------------------------------------
        public void Recover()
        {
            if (!m_bBackuped) return;
            if (GetController() == null) return;
            if (property == ETweenPropertyType.Position)
            {
                Transform transform = GetTransform();
                if (transform)
                {
                    if(bLocal)
                        transform.localPosition = m_Backup.toVector3();
                    else
                        transform.position = m_Backup.toVector3();
                }
            }
            else if (property == ETweenPropertyType.Rotate)
            {
                Transform transform = GetTransform();
                if (transform)
                {
                    if (bLocal)
                        transform.localEulerAngles = m_Backup.toVector3();
                    else
                        transform.eulerAngles = m_Backup.toVector3();
                }
            }
            else if (property == ETweenPropertyType.Scale)
            {
                Transform transform = GetTransform();
                if (transform) transform.localScale = m_Backup.toVector3();
            }
            else if (property == ETweenPropertyType.Pivot)
            {
                RectTransform transform = GetRectTransform();
                if (transform)
                {
                    transform.pivot = m_Backup.toVector2();
                    Vector2 offset = (to.toVector2() - m_Backup.toVector2());
                    offset.x *= transform.sizeDelta.x;
                    offset.y *= transform.sizeDelta.y;
                    transform.offsetMin -= offset;
                    transform.offsetMax -= offset;
                }
            }
            else if (property == ETweenPropertyType.Color)
            {
                UI.UIGraphicUtil.RecoverGraphic(m_vGraphics);
            }
            else if(property == ETweenPropertyType.Alpha)
            {
                CanvasGroup canvasGroup = GetCanvasGroup();
                if (canvasGroup == null)
                    UI.UIGraphicUtil.RecoverGraphic(m_vGraphics);
                else
                {
                    canvasGroup.alpha = m_Backup.toAlpha();
                    if (m_vGraphics != null) m_vGraphics.Clear();
                }
            }
            //    if (m_vGraphics != null) m_vGraphics.Clear();
        }
        //------------------------------------------------------
        public Transform GetTransform()
        {
            if (GetController() == null) return null;
            Transform transform = GetController() as Transform;
            if (transform) return transform;
            GameObject go = GetController() as GameObject;
            if (go) return go.transform;

            Behaviour behavior = GetController() as Behaviour;
            if (behavior) return behavior.transform;
            return null;
        }
        //------------------------------------------------------
        public RectTransform GetRectTransform()
        {
            return GetTransform() as RectTransform;
        }
        //------------------------------------------------------
        public CanvasGroup GetCanvasGroup()
        {
            return GetController() as CanvasGroup;
        }
        //------------------------------------------------------
        public Dictionary<UnityEngine.Behaviour, RtgTweenerProperty> GetUIGraphics()
        {
            return m_vGraphics;
        }
    }
}

