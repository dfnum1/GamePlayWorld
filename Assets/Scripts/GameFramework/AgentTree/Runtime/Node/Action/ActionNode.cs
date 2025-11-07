/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	BaseNode
作    者:	HappLI
描    述:	动作节点
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.AT
{
    [Serializable]
    public class ActionNode : ExcudeNode
    {
        public EActionType actionType;
        public long custumeValue = 0;
        public Condition condition = null;

        public override int GetExcudeHash() { return (int)actionType; }
        public override void SetCustomValue(long value) { custumeValue = value; }
        public override long GetCustomValue() { return custumeValue; }

        public override void Copy(BaseNode pOther, bool bIncludeGuid = true)
        {
            base.Copy(pOther,bIncludeGuid);
            ActionNode pAT = pOther as ActionNode;
            custumeValue = pAT.custumeValue;
            if (pAT.condition != null)
            {
                condition = new Condition();
                condition.Copy(pAT.condition);
            }
            else condition = null;
        }
        //------------------------------------------------------
        public override void Destroy()
        {
            if (condition != null) condition.Destroy();
            base.Destroy();
        }
        //------------------------------------------------------
        protected override void OnReset()
        {
            if (condition != null) condition.Reset();
        }
        //------------------------------------------------------
        protected override void OnInit(AgentTree pTree)
        {
            if (condition != null)
                condition.Init(pTree);
        }

#if UNITY_EDITOR
        public override void SetExcudeHash(int hash)
        {
            actionType = (EActionType)hash;
        }
        //------------------------------------------------------
        public override T GetEditAttrData<T>()
        {
            if (m_pEditAttrData == null || m_pEditAttrData.actionID != (int)actionType)
            {
                ATExportNodeAttrData outData;
                if (AgentTreeEditorUtils.AssemblyATData.ExportActions.TryGetValue((int)actionType, out outData))
                    m_pEditAttrData = outData;
            }
            return m_pEditAttrData as T;
        }
        //------------------------------------------------------
        protected override void OnGetAllVariable(List<Variable> varibales)
        {
            if (condition != null && condition.portals != null)
            {
                for (int i = 0; i < condition.portals.Count; ++i)
                {
                    if (condition.portals[i].argv != null && condition.portals[i].argv.GetVariable() != null)
                        varibales.Add(condition.portals[i].argv.GetVariable());
                    if (condition.portals[i].compare != null && condition.portals[i].compare.GetVariable() != null)
                        varibales.Add(condition.portals[i].compare.GetVariable());
                }
            }
        }
        //------------------------------------------------------
        protected override void OnSave()
        {
            if (condition != null) condition.Save();
        }
        //------------------------------------------------------
        public override string ToTitleTips()
        {
            ATExportNodeAttrData actionAttrNode = GetEditAttrData<ATExportNodeAttrData>();
            if (actionAttrNode == null) return "";
            string ToolTips = actionAttrNode.methodAttr.ToolTips;
            if (string.IsNullOrEmpty(ToolTips))
                ToolTips = actionAttrNode.methodAttr.DisplayName;

            ToolTips += "[" + GUID.ToString() + "]";

            if (inArgvs != null)
            {
                for (int i = 0; i < inArgvs.Length; ++i)
                {
                    if (inArgvs[i].variable == null)
                    {
                        ToolTips += "\nin-槽" + i + "丢失!";
                        continue;
                    }
                    string strName = inArgvs[i].variable.strName;
                    if (i < actionAttrNode.InArgvs.Count)
                    {
                        strName = actionAttrNode.InArgvs[i].ToolTips;
                        if (string.IsNullOrEmpty(strName))
                            strName = actionAttrNode.InArgvs[i].DisplayName;
                    }
                    string addtiveName = "";
                    StructData structData;
                    if (pEditor!=null && pEditor.GetVariableStructs().TryGetValue(inArgvs[i].variable.GUID, out structData))
                    {
                        addtiveName = "-(结构[" + structData.structName + "]成员)";
                    }

                    ToolTips += "\nin-" + strName + "[" + inArgvs[i].variable.GUID.ToString() + "]" + addtiveName;
                }
            }
            if (outArgvs != null)
            {
                for (int i = 0; i <outArgvs.Length; ++i)
                {
                    if (outArgvs[i].variable == null)
                    {
                        ToolTips += "\nout-槽" + i + "丢失!";
                        continue;
                    }
                    string strName = outArgvs[i].variable.strName;
                    if (i < actionAttrNode.OutArgvs.Count)
                    {
                        strName = actionAttrNode.OutArgvs[i].ToolTips;
                        if (string.IsNullOrEmpty(strName))
                            strName = actionAttrNode.OutArgvs[i].Name;
                    }
                    ToolTips += "\nout-" + strName + "[" + outArgvs[i].variable.GUID.ToString() + "]";
                }
            }
            return ToolTips;
        }
        //------------------------------------------------------
        public override string ToTips(ArgvPort argvPort)
        {
            string ToolTips = "";
            ATExportNodeAttrData actionAttrNode = GetEditAttrData<ATExportNodeAttrData>();
            if (actionAttrNode == null) return "";
            if (argvPort.IsOutput() && actionAttrNode.OutArgvs != null && argvPort.index < actionAttrNode.OutArgvs.Count)
            {
                ToolTips = actionAttrNode.OutArgvs[argvPort.index].ToolTips;
                if (string.IsNullOrEmpty(ToolTips))
                    ToolTips = actionAttrNode.OutArgvs[argvPort.index].Name;
            }
            else if (argvPort.IsInput() && actionAttrNode.InArgvs != null && argvPort.index < actionAttrNode.InArgvs.Count)
            {
                ToolTips = actionAttrNode.InArgvs[argvPort.index].ToolTips;
                if (string.IsNullOrEmpty(ToolTips))
                    ToolTips = actionAttrNode.InArgvs[argvPort.index].DisplayName;
            }
            ToolTips += "[" + argvPort.variable.GUID.ToString() + "]" + argvPort.variable.ToValueText();

            if (argvPort.port != null && argvPort.port.dummyMap != null)
            {
                foreach (var db in argvPort.port.dummyMap)
                {
                    string strDummyName = db.Value.strName;
                    GraphNode parentNode;
                    if (pEditor!=null && pEditor.GetGraphNodes().TryGetValue(db.Key, out parentNode) && parentNode.BindNode != null)
                    {
                        ATExportNodeAttrData actionAttrNode1 = parentNode.BindNode.GetEditAttrData<ATExportNodeAttrData>();
                        if (actionAttrNode1!=null)
                        {
                            string ToolTips1 = actionAttrNode1.methodAttr.ToolTips;
                            if (string.IsNullOrEmpty(ToolTips1))
                                ToolTips1 = actionAttrNode1.methodAttr.DisplayName;
                            ToolTips1 += "[" + db.Key + "]";

                            string strVar1 = "";
                            int slot = parentNode.BindNode.IndexofOutArgv(db.Value);
                            if (slot >= 0 && slot < actionAttrNode1.OutArgvs.Count)
                            {
                                strVar1 = actionAttrNode1.OutArgvs[slot].ToolTips;
                                if (string.IsNullOrEmpty(strVar1))
                                    strVar1 = actionAttrNode1.OutArgvs[slot].Name;
                            }

                            if (string.IsNullOrEmpty(db.Value.strName))
                                strDummyName = ToolTips1 + "-" + strVar1;
                            else
                                strDummyName = ToolTips1 + "-" + db.Value.strName;
                        }
                    }
                    ToolTips += "\r\nD-" + strDummyName + "[" + db.Value.GUID.ToString() + "]" + db.Value.ToValueText();
                }
            }
            return ToolTips;
        }
#endif
    }
}
