#if USE_GUIDESYSTEM
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ExcudeNode
作    者:	HappLI
描    述:	执行器，获取一些想要的参数
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.Guide
{
    [System.Serializable]
    public class ExcudeNode : SeqNode
    {
        public int type = -1;
        public int[] argvGuids = null;

        public string[] beginEvents = null;
        [System.NonSerialized]
        public List<IUserData> vBeginEvents = null;


        public string[] endEvents = null;
        [System.NonSerialized]
        public List<IUserData> vEndEvents = null;

        [System.NonSerialized]
        public List<ArgvPort> _Ports = new List<ArgvPort>();
        public override List<ArgvPort> GetArgvPorts()
        {
            return _Ports;
        }

        public override int GetEnumType()
        {
            return type;
        }

        public override List<IUserData> GetBeginEvents()
        {
#if UNITY_EDITOR
            if (vBeginEvents == null) vBeginEvents = new List<IUserData>();
#endif
            return vBeginEvents;
        }

        public override List<IUserData> GetEndEvents()
        {
#if UNITY_EDITOR
            if (vEndEvents == null) vEndEvents = new List<IUserData>();
#endif
            return vEndEvents;
        }

        public override void Init(GuideGroup pGroup)
        {
            base.Init(pGroup);
            _Ports.Clear();
            if(argvGuids != null)
            {
                for(int i = 0; i < argvGuids.Length; ++i)
                {
                    ArgvPort port = pGroup.GetPort(argvGuids[i]);
                    if (port == null) continue;
                    _Ports.Add(port);
                }
            }

            vBeginEvents = null;
            if (beginEvents != null && beginEvents.Length > 0)
            {
                vBeginEvents = new List<IUserData>(beginEvents.Length);
                for (int i = 0; i < beginEvents.Length; ++i)
                {
                    IUserData pEvt = GuideSystem.getInstance().BuildEvent(beginEvents[i]);
                    if (pEvt == null) continue;
                    vBeginEvents.Add(pEvt);
                }
            }
            vEndEvents = null;
            if (endEvents != null && endEvents.Length > 0)
            {
                vEndEvents = new List<IUserData>(endEvents.Length);
                for (int i = 0; i < endEvents.Length; ++i)
                {
                    IUserData pEvt = GuideSystem.getInstance().BuildEvent(endEvents[i]);
                    if (pEvt == null) continue;
                    vEndEvents.Add(pEvt);
                }
            }
        }
#if UNITY_EDITOR
        public override void SetArgvPorts(List<ArgvPort> vPorts)
        {
            _Ports = vPorts;
            if (_Ports != null && _Ports.Count > 0)
            {
                argvGuids = new int[_Ports.Count];
                for (int i = 0; i < _Ports.Count; ++i)
                {
                    argvGuids[i] = _Ports[i].guid;
                }
            }
            else
                argvGuids = null;
        }
        //------------------------------------------------------
        public override void Save()
        {
            base.Save();
            if (_Ports != null && _Ports.Count>0)
            {
                argvGuids = new int[_Ports.Count];
                for (int i = 0; i < _Ports.Count; ++i)
                {
                    argvGuids[i] = _Ports[i].guid;
                }
            }
            else
                argvGuids = null;

            if (vBeginEvents!=null && vBeginEvents.Count > 0)
            {
                List<string> vCmd = new List<string>();
                for (int i = 0; i < vBeginEvents.Count; ++i)
                {
                    if (vBeginEvents[i] == null) continue;
                    vCmd.Add(vBeginEvents[i].ToString());
                }
                beginEvents = vCmd.ToArray();
            }
            else
                beginEvents = null;

            if (vEndEvents != null && vEndEvents.Count > 0)
            {
                List<string> vCmd = new List<string>();
                for (int i = 0; i < vEndEvents.Count; ++i)
                {
                    if (vEndEvents[i] == null) continue;
                    vCmd.Add(vEndEvents[i].ToString());
                }
                endEvents = vCmd.ToArray();
            }
            else
                endEvents = null;
        }
        //------------------------------------------------------
        internal override void CheckPorts()
        {
            base.CheckPorts();
            GuideEditor.NodeAttr nodeAttr;
            if (!GuideEditor.Instance.ExcudeTypes.TryGetValue(type, out nodeAttr))
                return;

            if (_Ports == null) _Ports = new List<ArgvPort>();
            if (nodeAttr.argvs != null && nodeAttr.argvs.Count != _Ports.Count)
            {
                if (nodeAttr.argvs.Count < _Ports.Count)
                    _Ports.RemoveRange(nodeAttr.argvs.Count, _Ports.Count - nodeAttr.argvs.Count);
                else
                {
                    for (int i = _Ports.Count; i < nodeAttr.argvs.Count; ++i)
                    {
                        _Ports.Add(new ArgvPort() { guid = GuideEditorLogic.BuildPortGUID() });
                    }
                }
            }
        }
#endif
    }
}
#endif