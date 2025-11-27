/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	StepNode
作    者:	HappLI
描    述:	引导步骤节点
*********************************************************************/
using System.Collections.Generic;
namespace Framework.Guide
{
    [System.Serializable]
	public class TriggerNode : SeqNode, IQuickSort<TriggerNode>
    {
        public int type = -1;
        public int priority = 0;
        public int[] argvGuids = null;

        public string[] Events = null;
        [System.NonSerialized]
        public List<IUserData> vEvents = null;

        [System.NonSerialized]
        public List<ArgvPort> _Ports = new List<ArgvPort>();
        public override List<ArgvPort> GetArgvPorts()
        {
            return _Ports;
        }
        //-----------------------------------------------------
        public override bool IsCustom()
        {
            return this.type >= (int)GuideTriggerDef.CustomBegin && this.type <= (int)GuideTriggerDef.CustomEnd;
        }
        //-----------------------------------------------------
        public override int GetEnumType()
        {
            return type;
        }
        //-----------------------------------------------------
        public void FillArgv(VariableList argvs)
        {
            if (argvs == null) return;
            for (int i = 0; i < _Ports.Count && i < argvs.GetVarCount(); ++i)
            {
                var varType = argvs.GetVarType(i);
                if(varType == EVariableType.eString)
                    _Ports[i].fillStrValue = argvs.GetString(i);
                else
                    _Ports[i].fillValue = argvs.GetInt(i);
            }
        }
        //-----------------------------------------------------
        public int CompareTo(int type, TriggerNode other)
        {
            if (priority < other.priority) return -1;
            if (priority > other.priority) return 1;
            return 0;
        }
        //-----------------------------------------------------
        public override void Init(GuideGroup pGroup)
        {
            base.Init(pGroup);
            _Ports.Clear();
            if(argvGuids != null)
            {
                for(int i = 0; i < argvGuids.Length; ++i)
                {
                    ArgvPort port = pGroup.GetPort(argvGuids[i]);
                    if(port == null) continue;
                    _Ports.Add(port);
                }
            }
            vEvents = null;
            if (Events != null && Events.Length > 0)
            {
                vEvents = new List<IUserData>(Events.Length);
                for (int i = 0; i < Events.Length; ++i)
                {
                    IUserData pEvt = GuideSystem.getInstance().BuildEvent(Events[i]);
                    if (pEvt == null) continue;
                    vEvents.Add(pEvt);
                }
            }
        }
        //-----------------------------------------------------
        public ArgvPort GetPortArgsByGuid(int guid)
        {
            foreach (var port in _Ports)
            {
                if (port.guid == guid)
                {
                    return port;
                }
            }
            return null;
        }
        //-----------------------------------------------------
        public void Destroy()
        {
        }
        //-----------------------------------------------------
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

            if (vEvents != null && vEvents.Count > 0)
            {
                List<string> vCmd = new List<string>();
                for (int i = 0; i < vEvents.Count; ++i)
                {
                    if (vEvents[i] == null) continue;
                    vCmd.Add(vEvents[i].ToString());
                }
                Events = vCmd.ToArray();
            }
            else
                Events = null;
        }
        //------------------------------------------------------
        internal override void CheckPorts()
        {
            base.CheckPorts();
            Framework.Guide.Editor.GuideSystemEditor.NodeAttr nodeAttr;
            if (!Framework.Guide.Editor.GuideSystemEditor.TriggerTypes.TryGetValue(type, out nodeAttr))
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
                        _Ports.Add(new ArgvPort() { guid = Framework.Guide.Editor.GuideEditorLogic.BuildPortGUID() });
                    }
                }
            }
        }
#endif
    }
}
