#if UNITY_EDITOR
/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	GuideNodePreviewEditor
作    者:	HappLI
描    述:	引导节点预览编辑
*********************************************************************/

using System.Collections.Generic;
using UnityEditor;

namespace Framework.Guide.Editor
{
    public class GuideNodePreviewEditor
    {
        static BaseNode ms_pEditNode = null;
        public static void OnEnable()
        {
            ms_pEditNode = null;
            if (!GuideSystem.getInstance().bDoing)
            {
                OnVisible(true);
            }
        }
        //------------------------------------------------------
        public static void OnDisable()
        {
            ms_pEditNode = null;
            if (!GuideSystem.getInstance().bDoing)
            {
                OnVisible(false);
            }
        }
        //------------------------------------------------------
        public static void OnVisible(bool bVisible)
        {
            if (!bVisible) ms_pEditNode = null;
            if (GuideSystem.getInstance().bDoing)
                return;

            GuidePanel panel = GetGuidPanel();
            if (panel == null) return;
            if(bVisible) panel.Show();
            else panel.Hide();
        }
        //------------------------------------------------------
        static GuidePanel GetGuidPanel()
        {
            return GuideSystem.getInstance().GetGuidePanel();
        }
        //------------------------------------------------------
        public static void OnEditorPreview(BaseNode pNode)
        {
            ms_pEditNode = pNode;
            if (GuideSystem.getInstance().bDoing)
                return;
            if (pNode == null) return;
            SeqNode stepNode = pNode as SeqNode;
            if (stepNode == null) return;
            GuidePanel panel = GetGuidPanel();
            if (panel == null) return;
            List<ArgvPort> vPorts = stepNode.GetArgvPorts();
            panel.bDoing = false;
            if (vPorts != null)
            {
                for (int i = 0; i < vPorts.Count; ++i)
                {
                    vPorts[i].Init();
                }
            }
            bool bVisible = panel.IsVisible();
            if(!bVisible)
            {
                panel.Show();
            }
            bool bNewVisible = panel.IsVisible();
            panel.IsEditorPreview = true;
            if (pNode is StepNode)
                GuideStepHandler.OnGuideExecuteNode(stepNode as StepNode);
            else if (pNode is ExcudeNode)
                GuideExecudeHandler.OnGuideExecuteNode(pNode as ExcudeNode);
            panel.IsEditorPreview = false;
        }
    }
}
#endif