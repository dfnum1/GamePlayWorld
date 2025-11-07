/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	RuntimeEvent
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.Data;
using System.Collections.Generic;
using System.Xml;
using Framework.Base;
using UnityEngine;

namespace Framework.Core
{
    public class BuildEventUtl
    {
        private static List<string> ms_vCatchs = null;
        internal static List<string> GetCatchs()
        {
            if (ms_vCatchs == null) ms_vCatchs = new List<string>(32);
            return ms_vCatchs;
        }
        public static BaseEvent BuildEvent(AFramework framework, string strCmd)
        {
            return BaseEvent.NewEvent(framework,strCmd);
        }
        public static BaseEvent BuildEventByType(AFramework framework, ushort eventType)
        {
            BaseEvent param = null;
            if (framework == null || framework.eventSystem == null) param = ActionEventCore.NewEvent(eventType);
            else param = framework.eventSystem.MallocEvent((int)eventType);

            if(param == null)
            {
                Debug.LogError("evenType:" + eventType + ": malloc error");
#if UNITY_EDITOR
                ED.EventPopDatas.NewEvent(eventType);
#endif
            }
            if (param != null) param.Create(framework);
            return param;
        }
    }

    public enum EActionWithBit : byte
    {
        [PluginDisplay("结束停止")]
        StopData = 1<<0,
        [PluginDisplay("结束擦除数据")]
        ClearData = 1<<1,
        [PluginDisplay("结束延时清理")]
        DelayStop = 1<<2,
        [PluginDisplay("结束保持")]
        StopKeep = 1<<3,
        [PluginDisplay("死亡保持")]
        DieKeep = 1 << 4,
        [PluginDisplay("结束触发")]
        StopTrigger = 1 << 5,
    }

    public enum EEventBit:int
    {
        [PluginDisplay("作用第0层")]
        EventLayer0= 1 << 12,
        [PluginDisplay("作用第1层")]
        EventLayer1 = 1 << 13,
        [PluginDisplay("作用第2层")]
        EventLayer2 = 1 << 14,
        [PluginDisplay("作用第3层")]
        EventLayer3 = 1 << 15,
    }
    //------------------------------------------------------
    public interface IEventCreate
    {
        IEventParameter OnCreateEvent(int eventType);
    }
    //------------------------------------------------------
    public interface IEventParameter : IUserData
    {
        ushort GetEventType();
    }
    //------------------------------------------------------
    [System.Serializable]
    public abstract class BaseEvent : IEventParameter, IQuickSort<BaseEvent>
    {
        [PluginDisplay("触发时间")] public float triggetTime = 0;
        [PluginDisplay("上限次数")] public short totalTriggertCount = -1;
        [PluginDisplay("概率")] public float triggerRate = 1;

        [PluginDisplay("逻辑标志")]
        [DisplayEnumBitGUI(typeof(EEventBit))]
        public ushort triggerBit = (ushort)((int)EEventBit.EventLayer0| (int)EEventBit.EventLayer1| (int)EEventBit.EventLayer2| (int)EEventBit.EventLayer3);

        [PluginDisplay("动作结束处理标志")]
        [DisplayEnumBitGUI(typeof(EActionWithBit))]
        public byte actionWithBit = (byte)EActionWithBit.ClearData;

        [PluginDisplay("死亡后可触发")] public bool canTriggerAfterKilled = true;

        AFramework m_pFramework = null;

        protected List<string> vParams = null;
        //-------------------------------------------
        internal void Create(AFramework pFramework)
        {
            m_pFramework = pFramework;
        }
        //-------------------------------------------
        internal AFramework GetFramework()
        {
            return m_pFramework;
        }
        //-------------------------------------------
        public virtual void BeginEvent()
        {
        }
        //-------------------------------------------
        public virtual void EndEvent()
        {
        }
        //-------------------------------------------

#if UNITY_EDITOR
        [System.NonSerialized]
        public bool bDeling = false;
        public bool bExpand { get; set; }
        public virtual string GetTips()
        {
            return null;
        }
#endif
        [System.NonSerialized]
        protected byte m_nVersion = 0;
        //-------------------------------------------
        public virtual ushort GetEventType()
        {
            return (ushort)EEventType.Base;
        }
        //------------------------------------------------------
        public byte GetVersion()
        {
            return m_nVersion;
        }
        //------------------------------------------------------
        public virtual void FillParams() { }
        //------------------------------------------------------
        public bool IsEventBit(EEventBit bit)
        {
            return (triggerBit & (int)bit) != 0;
        }
        //------------------------------------------------------
        public bool IsEventBit(int bit)
        {
            return (triggerBit & (int)bit) != 0;
        }
        //------------------------------------------------------
        public void SetEventBit(EEventBit bit, bool bSet)
        {
            if (bSet) triggerBit |= (ushort)(bit);
            else triggerBit &= (ushort)(~(int)bit);
        }
        //------------------------------------------------------
        public bool IsEventLayer(int layer)
        {
            if (layer + 12 >= 16) return false;
            return (triggerBit & (1 << (12 + layer))) != 0;
        }
        //------------------------------------------------------
        public void SetEventLayer(int layer, bool bSet)
        {
            if (layer + 12 >= 16) return;
            if (bSet) triggerBit |= (ushort)(1 << (12 + layer));
            else triggerBit &= (ushort)(~(1 << (12 + layer)));
        }
        //------------------------------------------------------
        public bool clearWithAction
        {
            get
            {
                return (actionWithBit & (byte)EActionWithBit.ClearData) != 0;
            }
#if UNITY_EDITOR
            set
            {
                actionWithBit |= (byte)EActionWithBit.ClearData;
            }
#endif
        }
        //------------------------------------------------------
        public bool delayStopWithAction
        {
            get
            {
                return (actionWithBit & (byte)EActionWithBit.DelayStop) != 0;
            }
#if UNITY_EDITOR
            set
            {
                actionWithBit |= (byte)EActionWithBit.DelayStop;
            }
#endif
        }
        //------------------------------------------------------
        public bool stopWithAction
        {
            get
            {
                return (actionWithBit & (byte)EActionWithBit.StopData) != 0;
            }
#if UNITY_EDITOR
            set
            {
                actionWithBit |= (byte)EActionWithBit.StopData;
            }
#endif
        }
        //------------------------------------------------------
        public bool stopKeeped
        {
            get
            {
                return (actionWithBit & (byte)EActionWithBit.StopKeep) != 0;
            }
#if UNITY_EDITOR
            set
            {
                actionWithBit |= (byte)EActionWithBit.StopKeep;
            }
#endif
        }
        //------------------------------------------------------
        public bool dieKeep
        {
            get
            {
                return (actionWithBit & (byte)EActionWithBit.DieKeep) != 0;
            }
#if UNITY_EDITOR
            set
            {
                actionWithBit |= (byte)EActionWithBit.DieKeep;
            }
#endif
        }
        //------------------------------------------------------
        public bool stopTrigger
        {
            get
            {
                return (actionWithBit & (byte)EActionWithBit.StopTrigger) != 0;
            }
#if UNITY_EDITOR
            set
            {
                actionWithBit |= (byte)EActionWithBit.StopTrigger;
            }
#endif
        }
        //-------------------------------------------
        public static BaseEvent NewEvent(AFramework framework, string strCmd)
        {
            if (string.IsNullOrEmpty(strCmd)) return null;
            string head = "";
            strCmd = strCmd.Trim();
            for (int i = 0; i < strCmd.Length; ++i)
            {
                if(strCmd[i] == ':')
                {
                    i++;
                    strCmd = strCmd.Substring(i);
                    break;
                }
                if(strCmd[i] == ' ' || strCmd[i] == '\t' || strCmd[i] == '\r' || strCmd[i] == '\n') continue;
                head += strCmd[i];
            }
            if (string.IsNullOrEmpty(strCmd)) return null;
            ushort headInt = 0;
            if (!ushort.TryParse(head, out headInt) || headInt <= 0) return null;
            BaseEvent param = BuildEventUtl.BuildEventByType(framework, headInt);
            if (param != null)
            {
                if(param.ReadCmd(strCmd)) return param;
                else
                {
                    ActionEventCore.ReleaseEvent(param);
                }
            }

            return null;
        }
        //-----------------------------------------------------
        public static BaseEvent NewEvent(AFramework framework, System.Xml.XmlElement xml)
        {
            if (xml == null || !xml.HasAttribute("type")) return null;
            string head = xml.GetAttribute("type");
            ushort headInt = 0;
            if (!ushort.TryParse(head, out headInt) || headInt <= 0) return null;
            BaseEvent param = BuildEventUtl.BuildEventByType(framework, headInt);
            if (param == null ) return null;
            param.Read(ref xml);
            return param;
        }
        //-----------------------------------------------------
        public static BaseEvent NewEvent(AFramework framework, ref BinaryUtil binray)
        {
            ushort eventType = binray.ToUshort();
            BaseEvent param = BuildEventUtl.BuildEventByType(framework, eventType);
            if (param == null) return null;
            param.Read(ref binray);
            return param;
        }
        //-------------------------------------------
        public static BaseEvent Instance(BaseEvent evtParam, AFramework framework =null)
        {
            if (framework == null) framework = evtParam.GetFramework();
            BaseEvent newParam = BuildEventUtl.BuildEventByType(framework, evtParam.GetEventType());
            if (newParam == null) return null;
            if (newParam.Copy(evtParam))
            {
                return newParam;
            }
            return null;
        }
        //-------------------------------------------
        public virtual void CollectPreload(AFramework pFramework, List<string> vFiles, HashSet<string> vAssets)
        {
            if (pFramework != null)
                pFramework.eventSystem.CollectPreload(this, vFiles, vAssets);
        }
        //-------------------------------------------
        public virtual bool Copy(BaseEvent evtParam)
        {
            if (evtParam == null || evtParam.GetEventType() != GetEventType()) return false;
            this.totalTriggertCount = evtParam.totalTriggertCount;
            this.triggerRate = evtParam.triggerRate;
            this.triggerBit = evtParam.triggerBit;
            this.actionWithBit = evtParam.actionWithBit;
            this.canTriggerAfterKilled = evtParam.canTriggerAfterKilled;
            this.triggetTime = evtParam.triggetTime;
            return true;
        }
#if UNITY_EDITOR
        //-------------------------------------------
        public override string ToString()
        {
            return WriteCmd();
        }
        //-------------------------------------------
        public string WriteCmd()
        {
            string strCMD = ((int)GetEventType()).ToString() + ":"
                + triggetTime.ToString() + ","
                + totalTriggertCount.ToString() + ","
                + triggerRate.ToString() + ","
                + triggerBit.ToString() + "," +
                actionWithBit.ToString() + "," +
                (canTriggerAfterKilled ? "1" : "0");
            string temp = InnerWrite();
            if (!string.IsNullOrEmpty(temp)) strCMD += "," + temp;
            return strCMD;
        }
        //-------------------------------------------
        protected virtual string InnerWrite() { return ""; }
#endif
        //-------------------------------------------
        public bool ReadCmd(string strParam)
        {
            List<string> vParams = CheckParameter(strParam);
            if (vParams.Count <= 0 || !float.TryParse(vParams[0], out triggetTime)) return false;
            vParams.RemoveAt(0);

            if (vParams.Count <= 0|| !short.TryParse(vParams[0], out totalTriggertCount)) return false;
            vParams.RemoveAt(0);

            if (vParams.Count <= 0 || !float.TryParse(vParams[0], out triggerRate)) return false;
            vParams.RemoveAt(0);

            if (vParams.Count <= 0 || !ushort.TryParse(vParams[0], out triggerBit)) return false;
            vParams.RemoveAt(0);

            byte flag = 0;
            if (vParams.Count <= 0 || !byte.TryParse(vParams[0], out flag)) return false;
            actionWithBit = flag;
            vParams.RemoveAt(0);

            flag = 0;
            if (vParams.Count <= 0 || !byte.TryParse(vParams[0], out flag)) return false;
            canTriggerAfterKilled = flag != 0;
            vParams.RemoveAt(0);

            bool bOk = InnerRead(vParams);
            if (!bOk) return false;

            OnReadSerialized();
            return true;
        }
        //-------------------------------------------
        protected virtual bool InnerRead(List<string> vData) { return true; }
#if UNITY_EDITOR
        //-------------------------------------------
        public XmlElement Write(XmlDocument xmlDoc, ref XmlElement parent, string nodeName = "Event")
        {
            if (parent == null) return null;
            XmlElement evt = xmlDoc.CreateElement(nodeName);
            evt.SetAttribute("type", ((int)GetEventType()).ToString());
            evt.SetAttribute("triggetTime", triggetTime.ToString());
            evt.SetAttribute("totalTriCount", totalTriggertCount.ToString());
            evt.SetAttribute("triggerRate", triggerRate.ToString());
            evt.SetAttribute("triggerBit", triggerBit.ToString());
            evt.SetAttribute("actionWithBit", actionWithBit.ToString());
            evt.SetAttribute("canTriggerAfterKilled", canTriggerAfterKilled ? "1" : "0");

            InnerWrite(xmlDoc, ref evt);
            parent.AppendChild(evt);
            return evt;
        }
        //-------------------------------------------
        public virtual void InnerWrite(XmlDocument pDoc, ref XmlElement ele) { }
#endif
        public virtual void InnerRead(ref XmlElement ele) { }
        //-------------------------------------------
        protected virtual void OnReadSerialized() { }
        //-------------------------------------------
        public void Read(ref System.Xml.XmlElement parent)
        {
            if (parent.HasAttribute("triggetTime")) float.TryParse(parent.GetAttribute("triggetTime"), out triggetTime);
            if (parent.HasAttribute("totalTriCount")) short.TryParse(parent.GetAttribute("triggerCnt"), out totalTriggertCount);
            if (parent.HasAttribute("triggerRate")) float.TryParse(parent.GetAttribute("triggerRate"), out triggerRate);
            if (parent.HasAttribute("triggerBit")) ushort.TryParse(parent.GetAttribute("triggerBit"), out triggerBit);
            if (parent.HasAttribute("actionWithBit")) byte.TryParse(parent.GetAttribute("actionWithBit"), out actionWithBit);
            if (parent.HasAttribute("canTriggerAfterKilled")) canTriggerAfterKilled = parent.GetAttribute("canTriggerAfterKilled").CompareTo("0") != 0;
            InnerRead(ref parent);
            OnReadSerialized();
        }
        //-------------------------------------------
        public virtual bool Read(ref Data.BinaryUtil reader)
        {
            reader.ToShort();
            m_nVersion = reader.ToByte();
            triggetTime = reader.ToFloat();
            totalTriggertCount = reader.ToShort();
            triggerRate = reader.ToFloat();
            triggerBit = reader.ToUshort();
            actionWithBit = reader.ToByte();
            canTriggerAfterKilled = reader.ToBool();
            return true;
        }
#if UNITY_EDITOR
        //-------------------------------------------
        public virtual void Write(ref Framework.Data.BinaryUtil writer)
        {
            byte version = 0;
            writer.WriteShort((short)GetEventType());
            writer.WriteByte(version);
            writer.WriteFloat(triggetTime);
            writer.WriteShort(totalTriggertCount);
            writer.WriteFloat(triggerRate);
            writer.WriteUshort(triggerBit);
            writer.WriteByte(actionWithBit);
            writer.WriteBool(canTriggerAfterKilled);
        }
#endif
        //-------------------------------------------
        public List<string> CheckParameter(string strParam)
        {
            List<string> vParams = null;
            if (m_pFramework != null)
                vParams = m_pFramework.shareParams.stringCatchList;
            else vParams = BuildEventUtl.GetCatchs();
            vParams.Clear();
            string[] argvs = strParam.Split(',');
            for (int i = 0; i < argvs.Length; ++i)
                vParams.Add(argvs[i]);
            return vParams;
        }
        //-------------------------------------------
        public virtual byte GetBindBit()
        {
            return (byte)ESlotBindBit.All;
        }
        //-------------------------------------------
        public virtual Vector3 GetOffset()
        {
            return Vector3.zero;
        }
        //-------------------------------------------
        public virtual string GetParentSlot()
        {
            return null;
        }
        //------------------------------------------------------
        public abstract void OnExecute(EventSystem pEventSystem);
        //------------------------------------------------------
        public virtual void Destroy() { }
#if UNITY_EDITOR
        //------------------------------------------------------
        public virtual void OnSceneGUI(Rect rc, Transform target) { }
        public virtual bool OnEdit(bool bCheck = false) { return false; }
        public virtual bool OnPreview(bool bCheck = false, System.Object target = null) { return false; }
        public virtual bool OnInspector(System.Object param = null) { return false; }
        public virtual void OnPreviewUpdate(float fTime, Transform target) { }
        public virtual void PlayPreview(bool play) { }
        public virtual bool OnInspectorGUI(System.Action<string> OnFiledDraw = null) { return false; }
#endif     
        //------------------------------------------------------
        public int CompareTo(int type, BaseEvent other)
        {
            if (triggetTime < other.triggetTime) return -1;
            else if (triggetTime > other.triggetTime) return 1;
            return 0;
        }
    }
}

