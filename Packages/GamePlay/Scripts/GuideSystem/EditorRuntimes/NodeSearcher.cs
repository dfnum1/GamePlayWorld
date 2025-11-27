#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using System;

namespace Framework.Guide.Editor
{
    //------------------------------------------------------
    public partial class NodeSearcher : GuideSearcher
    {
        protected override void OnSearch(string query)
        {
            GuideSystemEditor pEditor = GuideSystemEditor.Instance;
            int id = 0;
            {
                GuideSystemEditor.OpParam param = new GuideSystemEditor.OpParam();
                param.strName = "操作器";
                param.mousePos = inspectorRect.position;
                param.gridPos = pEditor.logic.WindowToGridPosition(param.mousePos);
                ItemEvent root  = new ItemEvent() { param = param, callback = pEditor.CreateConditionNode };
                root.depth = 0;
                root.name = "操作器";
                root.id = id++;
                m_assetTree.AddData(root);
                id++;
            }
            {
                ItemEvent root = new ItemEvent() { param = null, callback = null };
                root.depth = 0;
                root.name = "触发器";
                root.id = id++;
                m_assetTree.AddData(root);

                foreach (var item in GuideSystemEditor.TriggerTypes)
                {
                    bool bQuerty = IsQuery(query, item.Value.strQueueName);
                    if (!bQuerty) continue;
                    GuideSystemEditor.TriggerParam param = new GuideSystemEditor.TriggerParam();
                    param.Data = item.Value;
                    param.mousePos = inspectorRect.position;
                    param.gridPos = pEditor.logic.WindowToGridPosition(param.mousePos);
                    ItemEvent child = new ItemEvent() { param = param, callback = pEditor.CreateTriggerNode };
                    child.id = id++;
                    child.name = item.Value.strName;
                    child.depth = 1;
                    m_assetTree.AddData(child);
                }
            }
            {
                ItemEvent root = new ItemEvent() { param = null, callback = null };
                root.depth = 0;
                root.name = "步骤";
                root.id = id++;
                m_assetTree.AddData(root);

                foreach (var item in GuideSystemEditor.StepTypes)
                {
                    bool bQuerty = IsQuery(query, item.Value.strQueueName);
                    if (!bQuerty) continue;
                    GuideSystemEditor.StepParam param = new GuideSystemEditor.StepParam();
                    param.Data = item.Value;
                    param.mousePos = inspectorRect.position;
                    param.gridPos = pEditor.logic.WindowToGridPosition(param.mousePos);
                    ItemEvent child = new ItemEvent() { param = param, callback = pEditor.CreateStepNode };
                    child.id = id++;
                    child.name = item.Value.strName;
                    child.depth = 1;
                    m_assetTree.AddData(child);
                }
            }
            {
                ItemEvent root = new ItemEvent() { param = null, callback = null };
                root.depth = 0;
                root.name = "执行器";
                root.id = id++;
                m_assetTree.AddData(root);
                foreach (var item in GuideSystemEditor.ExcudeTypes)
                {
                    bool bQuerty = IsQuery(query, item.Value.strQueueName);
                    if (!bQuerty) continue;

                    GuideSystemEditor.ExcudeParam param = new GuideSystemEditor.ExcudeParam();
                    param.Data = item.Value;
                    param.mousePos = inspectorRect.position;
                    param.gridPos = pEditor.logic.WindowToGridPosition(param.mousePos);

                    ItemEvent child = new ItemEvent() { param = param, callback = pEditor.CreateExcudeNode };
                    child.id = id++;
                    child.name = item.Value.strName;
                    child.depth = 1;
                    m_assetTree.AddData(child);
                }
            }
        }
    }
}
#endif