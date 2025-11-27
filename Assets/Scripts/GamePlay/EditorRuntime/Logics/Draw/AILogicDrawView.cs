/********************************************************************
生成日期:	06:30:2025
类    名: 	TimelineDrawLogic
作    者:	HappLI
描    述:	时间轴控制逻辑
*********************************************************************/
#if UNITY_EDITOR
using Framework.ED;
using PlasticGui.Configuration.CloudEdition.Welcome;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Framework.War.Editor
{
    public class AILogicDrawView : GraphView
    {
        VisualElement m_pRoot;
        AEditorLogic m_pOwnerEditorLogic;
        //--------------------------------------------------------
        public AILogicDrawView(AEditorLogic pOwner)
        {
            m_pOwnerEditorLogic = pOwner;
            // 允许对Graph进行Zoom in/out
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            // 允许拖拽Content
            this.AddManipulator(new ContentDragger());
            // 允许Selection里的内容
            this.AddManipulator(new SelectionDragger());
            // GraphView允许进行框选
            this.AddManipulator(new RectangleSelector());

            // 添加右键菜单
            this.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                evt.menu.InsertAction(0, "执行", (a) =>
                {
                    

                });
                evt.menu.InsertAction(1, "停止", (a) =>
                {
                });
            }));

            //var menuWindowProvider = (AgentTreeSearcher)ScriptableObject.CreateInstance<AgentTreeSearcher>();
            //menuWindowProvider.ownerGraphView = this;
            //menuWindowProvider.OnSelectEntryHandler = OnMenuSelectEntry;
            //nodeCreationRequest += context =>
            //{
            //    SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider);
            //};
            //this.graphViewChanged += OnGraphViewChanged;
        }
        //-----------------------------------------------------
        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            return change;
        }
        //--------------------------------------------------------
        protected void OnDisable()
        {
        }
        //--------------------------------------------------------
        public void OnGUI(Rect rect)
        {
        }
    }
}
#endif