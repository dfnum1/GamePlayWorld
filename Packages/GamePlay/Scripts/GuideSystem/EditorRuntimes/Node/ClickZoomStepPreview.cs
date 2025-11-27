/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	StepNodeDraw
作    者:	
描    述:	步骤器绘制逻辑
*********************************************************************/
#if UNITY_EDITOR
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Guide.Editor
{
    [GuideEditorPreview((int)GuideStepType.ClickZoom,"OnSceneView")]
    public class ClickZoomScenePreview
    {
        [GuideNodeMenu("聚焦位置")]
        public static void OnMenuCall(BaseNode pNode)
        {
            var stepNode = pNode as StepNode;
            if (stepNode == null) return;
            if (stepNode.type != (int)GuideStepType.ClickZoom) return;
            Vector3 worldPos = Vector3.zero;
            worldPos.x = stepNode._Ports[3].fillValue * 0.001f;
            worldPos.y = stepNode._Ports[4].fillValue * 0.001f;
            worldPos.z = stepNode._Ports[5].fillValue * 0.001f;
            SceneView.lastActiveSceneView?.LookAt(worldPos);
        }
        public static void OnSceneView(BaseNode pNode, SceneView sceneView)
        {
            var stepNode = pNode as StepNode;
            if (stepNode == null) return;
            if (stepNode.type != (int)GuideStepType.ClickZoom) return;

            bool b3DPos = stepNode._Ports[2].fillValue!=0;
            Vector3 worldPos = Vector3.zero;
            worldPos.x = stepNode._Ports[3].fillValue * 0.001f;
            worldPos.y = stepNode._Ports[4].fillValue * 0.001f;
            worldPos.z = stepNode._Ports[5].fillValue * 0.001f;
            if(!b3DPos && GuideSystem.getInstance().GetGuidePanel()!=null)
            {
                var editorImg = GuideSystem.getInstance().GetGuidePanel().GetClickZoomEditorImage();
                if(editorImg!=null)
                {
                    editorImg.localPosition = new Vector3(worldPos.x, worldPos.y,0);
                }
            }
            FieldInfo field = stepNode._Ports[0].GetType().GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);

            EditorGUI.BeginChangeCheck();
            if (!Event.current.shift)
            {
                if (b3DPos)
                    worldPos = Handles.DoPositionHandle(worldPos, Quaternion.identity);
                else
                {
                    var editorImg = GuideSystem.getInstance().GetGuidePanel()?.GetClickZoomEditorImage();
                    if (editorImg != null)
                    {
                        editorImg.position = Handles.DoPositionHandle(editorImg.position, Quaternion.identity);
                        worldPos = editorImg.localPosition;
                    }
                    else
                        worldPos = Handles.DoPositionHandle(worldPos, Quaternion.identity);
                }
            }
            if (EditorGUI.EndChangeCheck() && !GuideSystem.getInstance().bDoing)
            {
                stepNode._Ports[3].fillValue = (int)(worldPos.x * 1000);
                stepNode._Ports[4].fillValue = (int)(worldPos.y * 1000);
                stepNode._Ports[5].fillValue = (int)(worldPos.z * 1000);
                if (field != null)
                {
                    field.SetValue(stepNode._Ports[3], stepNode._Ports[3].fillValue);
                    field.SetValue(stepNode._Ports[4], stepNode._Ports[4].fillValue);
                    field.SetValue(stepNode._Ports[5], stepNode._Ports[5].fillValue);
                }
            }
            if (!b3DPos && GuideSystem.getInstance().GetGuidePanel()!=null)
            {
                var editorImg = GuideSystem.getInstance().GetGuidePanel().GetClickZoomEditorImage();
                if (editorImg != null)
                {
                    worldPos = editorImg.position;
                }
            }
            Handles.Label(worldPos, "引导点位");
            if (Event.current.shift)
            {
                Handles.Label(worldPos + Vector3.right * (stepNode._Ports[6].fillValue * 0.001f), "响应半径");
                EditorGUI.BeginChangeCheck();
                Vector3 worldPos1 = Handles.DoPositionHandle(worldPos + Vector3.right*(stepNode._Ports[6].fillValue * 0.001f), Quaternion.identity);
                if (EditorGUI.EndChangeCheck() && !GuideSystem.getInstance().bDoing)
                {
                    worldPos1.y = worldPos.y;
                    stepNode._Ports[6].fillValue = (int)((worldPos1 - worldPos).magnitude*1000);
                    if (field != null)
                    {
                        field.SetValue(stepNode._Ports[6], stepNode._Ports[6].fillValue);
                    }
                }
            }
            Color color = Handles.color;
            Handles.color = Color.red;
            //获取当先是否是为2D视图
            if (sceneView.in2DMode)
            {
                float tempRadius = stepNode._Ports[6].fillValue * 0.001f;
                var panel = GuideSystem.getInstance().GetGuidePanel();
                if(panel!=null)
                {
                    tempRadius *= panel.GetUGUIScaler();
                    tempRadius = panel.UGUISizeToWorldSize(tempRadius)*0.5f;
                }
                Handles.CircleHandleCap(0, worldPos, Quaternion.Euler(0, 0, 0), tempRadius, EventType.Repaint);
            }
            else
                Handles.CircleHandleCap(0, worldPos, Quaternion.Euler(90, 0, 0), stepNode._Ports[6].fillValue * 0.001f, EventType.Repaint);
            Handles.color = color;
        }
    }
}
#endif