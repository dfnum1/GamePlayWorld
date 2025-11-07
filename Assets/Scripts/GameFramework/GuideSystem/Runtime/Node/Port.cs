#if USE_GUIDESYSTEM
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	Port
作    者:	HappLI
描    述:	变量端口
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.Guide
{
    public interface IPortEditor
    {

    }
    public abstract class IPort
    {
        public abstract int GetGuid();
        public virtual void SetFlag(EArgvFalg flag) { }
#if UNITY_EDITOR
        [System.NonSerialized]
        Dictionary<int, IPortEditor> m_vEditors = new Dictionary<int, IPortEditor>();
        public T GetEditor<T>(int guid) where T : IPortEditor, new()
        {
            if(!m_vEditors.ContainsKey(guid))
                m_vEditors.Add(guid, new T());
            return (T)m_vEditors[guid];
        }
#endif
    }

    public enum EArgvFalg : byte
    {
        Get = 1<<0,
        Set = 1<<1,

        GetAndPort = 1 << 2,
        SetAndPort = 1 << 3,

        None = 0,
        All = EArgvFalg.Get| EArgvFalg.Set,
        PortAll = EArgvFalg.GetAndPort | EArgvFalg.SetAndPort,
    }

    [System.Serializable]
    public class ArgvPort : IPort
    {
        public int guid = 0;
        [SerializeField]
        protected int value  =0;
        public string strValue = null;

        [System.NonSerialized]
        private int m_fillVaue = 0;
        public int fillValue
        {
            get { return m_fillVaue; }
            set { m_fillVaue = value; }
        }

        public EArgvFalg Flag = EArgvFalg.None;
        public bool bGet { get { return (Flag == EArgvFalg.All || Flag == EArgvFalg.Get); } }
        public bool bSet { get { return (Flag == EArgvFalg.All || Flag == EArgvFalg.Set); } }

        public override int GetGuid() { return guid; }

        public override void SetFlag(EArgvFalg flag) { Flag = flag; }

#if UNITY_EDITOR
        [System.NonSerialized]
        public System.Type bindType = null;
        [System.NonSerialized]
        public int enumDisplayType = 0;
#endif
        public void Init()
        {
            m_fillVaue = value;
        }
    }

    [System.Serializable]
    public struct DummyPort
    {
        public int nodeGuid;
        public int argvGuid;
    }

    [System.Serializable]
    public class VarPort : IPort
    {
        public EOpType type = EOpType.None;
        public int value = 0;
        public DummyPort[] dummys = null;

        public override int GetGuid() { return 0; }

        [System.NonSerialized]
        Dictionary<int, ArgvPort> m_vDummyPort = null;
        public void Init(GuideGroup pGroup)
        {
            if (dummys == null || dummys.Length <=0) return;
            m_vDummyPort = new Dictionary<int, ArgvPort>(dummys.Length);
            for(int i=0; i < dummys.Length; ++i)
            {
                ArgvPort port = pGroup.GetPort(dummys[i].argvGuid);
                if(port == null) continue;
                BaseNode pNode = pGroup.GetNode<BaseNode>(dummys[i].nodeGuid);
                if (pNode == null) continue;
                m_vDummyPort[dummys[i].nodeGuid] = port;
            }
        }

        public ArgvPort GetDummyArgv(List<BaseNode> vTrackNode)
        {
            if (m_vDummyPort == null) return null;
            if(m_vDummyPort.Count == 1)
            {
                foreach(var db in m_vDummyPort)
                {
                    return db.Value;
                }
            }
            for(int i = vTrackNode.Count-1;i >=0; --i)
            {
                ArgvPort port;
                if (m_vDummyPort.TryGetValue(vTrackNode[i].Guid, out port))
                    return port;
            }
            return null;
        }
#if UNITY_EDITOR
        public Dictionary<int, ArgvPort> dummyMaps
        {
            get { return m_vDummyPort; }
        }
        public void AddDummyArgv(int nodeGuide, ArgvPort port)
        {
            if (m_vDummyPort == null) m_vDummyPort = new Dictionary<int, ArgvPort>();
            m_vDummyPort[nodeGuide] = port;
        }
        public void DelDummyArgv(int nodeGuide)
        {
            if (m_vDummyPort == null) return;
            m_vDummyPort.Remove(nodeGuide);
        }
        public bool ContainsDummyArgv(int nodeGuide)
        {
            if (m_vDummyPort == null) return false;
            return m_vDummyPort.ContainsKey(nodeGuide);
        }
#endif

        public void Save()
        {
#if UNITY_EDITOR
            if (m_vDummyPort != null && m_vDummyPort.Count>0)
            {
                List<DummyPort> vPort = new List<DummyPort>();
                foreach (var db in m_vDummyPort)
                {
                    vPort.Add(new DummyPort() { argvGuid = db.Value.guid, nodeGuid = db.Key });
                }
                dummys = vPort.ToArray();
            }
            else
            {
                dummys = null;
            }
#endif
        }
    }
}
#endif