#if UNITY_EDITOR && USE_GUIDESYSTEM
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using System;

namespace Framework.Plugin.Guide
{
    //------------------------------------------------------
    public partial class NodeSearcher : GuideSearcher
    {
        protected override void OnSearch(string query)
        {
            GuideEditor pEditor = GuideEditor.Instance;
            int id = 0;
            {
                GuideEditor.OpParam param = new GuideEditor.OpParam();
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
                root.name = "步骤";
                root.id = id++;
                m_assetTree.AddData(root);

                foreach (var item in pEditor.StepTypes)
                {
                    bool bQuerty = IsQuery(query, item.Value.strQueueName);
                    if (!bQuerty) continue;
                    GuideEditor.StepParam param = new GuideEditor.StepParam();
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
                root.name = "触发器";
                root.id = id++;
                m_assetTree.AddData(root);

                foreach (var item in pEditor.TriggerTypes)
                {
                    bool bQuerty = IsQuery(query, item.Value.strQueueName);
                    if (!bQuerty) continue;
                    GuideEditor.TriggerParam param = new GuideEditor.TriggerParam();
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
                root.name = "执行器";
                root.id = id++;
                m_assetTree.AddData(root);
                foreach (var item in pEditor.ExcudeTypes)
                {
                    bool bQuerty = IsQuery(query, item.Value.strQueueName);
                    if (!bQuerty) continue;

                    GuideEditor.ExcudeParam param = new GuideEditor.ExcudeParam();
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