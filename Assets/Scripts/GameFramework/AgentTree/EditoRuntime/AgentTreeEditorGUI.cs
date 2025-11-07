#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Plugin.AT
{
    public partial class AgentTreeEditorLogic
    {
        private void OnDraw()
        {
            DrawGrid(m_pEditor.position, zoom, panOffset);
            DrawConnections();
            DrawDraggedConnection();
            DrawNodes();
            DrawSelectionBox();
            DrawTooltip();
        }
        //------------------------------------------------------
        void DrawConnections()
        {
            Vector2 mousePos = Event.current.mousePosition;
            List<ReroutePort> selection = m_preBoxSelectionReroute != null ? new List<ReroutePort>(m_preBoxSelectionReroute) : new List<ReroutePort>();
            hoveredReroute = new ReroutePort();

            m_vConnectionsSets.Clear();
            Color col = GUI.color;
            foreach (var db in m_vActioNodes)
            {
                DrawConnection(db.Value);
            }
            GUI.color = col;
            if (Event.current.type != EventType.Layout && currentActivity == EActivityType.DragGrid) selectedReroutes = selection;
        }
        //------------------------------------------------------
        void BuildTransferDots(int from, int to, List<Vector2> vPoints)
        {
            if (vPoints.Count <= 0)
                return;
            long transferKey = AgentTreeEditorUtils.BuildTransferKey(from, to);
            BuildTransferDots(transferKey, vPoints);
        }
        //------------------------------------------------------
        void BuildTransferDots(long transferKey, List<Vector2> vPoints)
        {
            if (vPoints.Count <= 0)
                return;
            if (m_vTransferDots.TryGetValue(transferKey, out var transferDot))
            {
                foreach (var db in transferDot.offsetDots)
                {
                    vPoints.Add(vPoints[0] + db);
                }
            }
        }
        //------------------------------------------------------
        void DrawConnection(GraphNode node)
        {
            int guid = node.GetGUID();
            for (int i = 0; i < node.Inputs.Count; ++i)
            {
                if (node.Inputs[i].variable == null) continue;

                Rect fromRect;
                if (!m_portConnectionPoints.TryGetValue(node.Inputs[i].GetGUID(), out fromRect)) continue;

                Color connectionColor = node.Inputs[i].GetColor();


                //! draw ref port
                if (node.Inputs[i].port != null && node.Inputs[i].port.pRefPort != null && node.Inputs[i].port.pRefPort.IsValid())
                {
                    RefPortNode refNode;
                    if (m_vRefPorts.TryGetValue(node.Inputs[i].port.pRefPort.id, out refNode))
                    {
                        List<Vector2> gridPoints = new List<Vector2>();
                        gridPoints.Add(refNode.GetLinkPosition());
                        BuildTransferDots(node.Inputs[i].port.pRefPort.id,node.Inputs[i].GetGUID(), gridPoints);
                        gridPoints.Add(fromRect.center);
                        DrawNoodle(connectionColor, gridPoints);
                    }
                    //                                     else
                    //                                     {
                    //                                         node.Inputs[i].port.pRefPort = null;
                    //                                     }
                }

                foreach (var indb in m_vActioNodes)
                {
                    if (indb.Value.BindNode == null || indb.Key == guid) continue;

                    if(indb.Value.Outputs.Count>0)
                    {
                        for (int j = 0; j < indb.Value.Outputs.Count; ++j)
                        {
                            if (indb.Value.Outputs[j].variable == null) continue;

                            bool bHasConnect = false;
                            if (indb.Value.Outputs[j].variable == node.Inputs[i].variable)
                                bHasConnect = true;
                            if (!bHasConnect)
                            {
                                bHasConnect = node.Inputs[i].port.HasDummy(indb.Key, indb.Value.Outputs[j].variable.GUID);
                            }
                            if (!bHasConnect) continue;

                            Rect toRect;
                            if (!m_portConnectionPoints.TryGetValue(indb.Value.Outputs[j].GetGUID(), out toRect)) continue;

                            System.Int64 key = Mathf.Min(indb.Value.Outputs[j].GetLinkGUID(), node.Inputs[i].GetLinkGUID()) << 32 | Mathf.Max(indb.Value.Outputs[j].GetLinkGUID(), node.Inputs[i].GetLinkGUID());
                            if (m_vConnectionsSets.Contains(key)) continue;

                            List<Vector2> gridPoints = new List<Vector2>();
                            gridPoints.Add(toRect.center);
                            BuildTransferDots(indb.Value.Outputs[j].GetGUID(), node.Inputs[i].GetGUID(), gridPoints);
                            gridPoints.Add(fromRect.center);
                            DrawNoodle(connectionColor, gridPoints);

                            m_vConnectionsSets.Add(key);
                        }
                    }
                    if(indb.Value.Inputs.Count>0)
                    {
                        for(int j =0;j < indb.Value.Inputs.Count; ++j)
                        {
                            if(indb.Value.Inputs[j].inputToOutput)
                            {
                                if (indb.Value.Inputs[j].variable == null) continue;

                                bool bHasConnect = false;
                                if (indb.Value.Inputs[j].variable == node.Inputs[i].variable)
                                    bHasConnect = true;
                                if (!bHasConnect)
                                {
                                    bHasConnect = node.Inputs[i].port.HasDummy(indb.Key, indb.Value.Inputs[j].variable.GUID);
                                }
                                if (!bHasConnect) continue;

                                Rect toRect;
                                if (!m_portConnectionPoints.TryGetValue(indb.Value.Inputs[j].GetGUID(), out toRect)) continue;

                                System.Int64 key = Mathf.Min(indb.Value.Inputs[j].GetLinkGUID(), node.Inputs[i].GetLinkGUID()) << 32 | Mathf.Max(indb.Value.Inputs[j].GetLinkGUID(), node.Inputs[i].GetLinkGUID());
                                if (m_vConnectionsSets.Contains(key)) continue;

                                List<Vector2> gridPoints = new List<Vector2>();
                                gridPoints.Add(toRect.center);
                                BuildTransferDots(indb.Value.Inputs[j].GetGUID(), node.Inputs[i].GetGUID(), gridPoints);
                                gridPoints.Add(fromRect.center);
                                DrawNoodle(connectionColor, gridPoints);
                                m_vConnectionsSets.Add(key);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < node.Outputs.Count; ++i)
            {
                if (node.Outputs[i].variable == null) continue;
                Rect fromRect;
                if (!m_portConnectionPoints.TryGetValue(node.Outputs[i].GetGUID(), out fromRect)) continue;

                Color connectionColor = node.Outputs[i].GetColor();
                foreach (var db in m_vActioNodes)
                {
                    if (db.Value.BindNode == null || db.Key == guid) continue;
                    if (db.Value.Inputs.Count <= 0) continue;
                    for (int j = 0; j < db.Value.Inputs.Count; ++j)
                    {
                        if (db.Value.Inputs[j].variable == null) continue;

                        bool bConnected = false;
                        if (db.Value.Inputs[j].variable == node.Outputs[i].variable) bConnected = true;
                        if(!bConnected)
                        {
                            bConnected = node.Outputs[i].port.HasDummy(db.Key, db.Value.Inputs[j].variable.GUID);
                        }
                        if (!bConnected) continue;
                        Rect toRect;
                        if (!m_portConnectionPoints.TryGetValue(db.Value.Inputs[j].GetGUID(), out toRect)) continue;

                        System.Int64 key = Mathf.Min(db.Value.Inputs[j].GetLinkGUID(), node.Outputs[i].GetLinkGUID()) << 32 | Mathf.Max(db.Value.Inputs[j].GetLinkGUID(), node.Outputs[i].GetLinkGUID());
                        if (m_vConnectionsSets.Contains(key)) continue;

                        List<Vector2> gridPoints = new List<Vector2>();
                        gridPoints.Add(fromRect.center);
                        BuildTransferDots(node.Outputs[i].GetGUID(), db.Value.Inputs[j].GetGUID(), gridPoints);
                        gridPoints.Add(toRect.center);
                        DrawNoodle(connectionColor, gridPoints);

                        m_vConnectionsSets.Add(key);
                    }
                }
            }

            //node link
            if (node.bLink)
            {
                Rect fromRect;
                if (!m_portConnectionPoints.TryGetValue(node.OutLink.GetGUID(), out fromRect)) return;
                Color connectionColor = AgentTreePreferences.GetSettings().linkLineColor;
                float wdith = AgentTreePreferences.GetSettings().linkLineWidth;
                for (int i = 0; i < node.NextLinks.Count; ++i)
                {
                    GraphNode grapNode = node.NextLinks[i];

                    if (grapNode.InLink.GetGUID() == node.OutLink.GetGUID()) continue;
                    Rect toRect;
                    if (!m_portConnectionPoints.TryGetValue(grapNode.InLink.GetGUID(), out toRect)) continue;

                    List<Vector2> gridPoints = new List<Vector2>();
                    gridPoints.Add(fromRect.center);
                    BuildTransferDots(node.OutLink.GetGUID(), grapNode.InLink.GetGUID(), gridPoints);
                    gridPoints.Add(toRect.center);
                    DrawNoodle(connectionColor, gridPoints, AgentTreePreferences.NoodleType.Count, wdith);
                }
            }

            if (node.OutConditionLinks != null)
            {
                for (int i = 0; i < node.OutConditionLinks.Count; ++i)
                {
                    if (node.OutConditionLinks[i].linkNodes.Count<=0) continue;

                    Rect fromRect;
                    if (!m_portConnectionPoints.TryGetValue(node.OutConditionLinks[i].GetGUID(), out fromRect)) return;
                    Color connectionColor = AgentTreePreferences.GetSettings().linkLineColor;
                    float wdith = AgentTreePreferences.GetSettings().linkLineWidth;
                    for (int j = 0; j < node.OutConditionLinks[i].linkNodes.Count; ++j)
                    {
                        if (node.OutConditionLinks[i].linkNodes[j] == node)
                        {
                            node.OutConditionLinks[i].linkNodes[j] = null;
                            continue;
                        }

                        Rect toRect;
                        if (!m_portConnectionPoints.TryGetValue(node.OutConditionLinks[i].linkNodes[j].InLink.GetGUID(), out toRect)) continue;

                        List<Vector2> gridPoints = new List<Vector2>();
                        gridPoints.Add(fromRect.center);
                        BuildTransferDots(node.OutConditionLinks[i].GetGUID(), node.OutConditionLinks[i].linkNodes[j].InLink.GetGUID(), gridPoints);
                        gridPoints.Add(toRect.center);
                        DrawNoodle(connectionColor, gridPoints, AgentTreePreferences.NoodleType.Count, wdith);
                    }
                    
                }
            }
            if (node.OutDelegateLinks != null)
            {
                for (int i = 0; i < node.OutDelegateLinks.Count; ++i)
                {
                    if (node.OutDelegateLinks[i].linkNodes.Count <= 0) continue;

                    Rect fromRect;
                    if (!m_portConnectionPoints.TryGetValue(node.OutDelegateLinks[i].GetGUID(), out fromRect)) return;
                    Color connectionColor = AgentTreePreferences.GetSettings().delegateLinkLineColor;
                    float wdith = AgentTreePreferences.GetSettings().linkLineWidth;
                    for (int j = 0; j < node.OutDelegateLinks[i].linkNodes.Count; ++j)
                    {
                        if (node.OutDelegateLinks[i].linkNodes[j] == node)
                        {
                            node.OutDelegateLinks[i].linkNodes[j] = null;
                            continue;
                        }

                        Rect toRect;
                        if (!m_portConnectionPoints.TryGetValue(node.OutDelegateLinks[i].linkNodes[j].InLink.GetGUID(), out toRect)) continue;

                        List<Vector2> gridPoints = new List<Vector2>();
                        gridPoints.Add(fromRect.center);
                        BuildTransferDots(node.OutDelegateLinks[i].GetGUID(), node.OutDelegateLinks[i].linkNodes[j].InLink.GetGUID(), gridPoints);
                        gridPoints.Add(toRect.center);
                        DrawNoodle(connectionColor, gridPoints, AgentTreePreferences.NoodleType.Count, wdith);
                    }

                }
            }
        }
        //------------------------------------------------------
        void DrawDraggedConnection()
        {
            IPortNode draggedOutput = PortUtil.GetPort(draggedOutputGuid);
            if (draggedOutput!=null)
            {
                Color col = draggedOutput.GetColor();
                col.a = draggedOutputTargetGuid >=0 ? 1.0f : 0.6f;

                Rect fromRect;
                if (!m_portConnectionPoints.TryGetValue(draggedOutputGuid, out fromRect)) return;
                List<Vector2> gridPoints = new List<Vector2>();
                gridPoints.Add(fromRect.center);
                for (int i = 0; i < draggedOutputReroutes.Count; i++)
                {
                    gridPoints.Add(draggedOutputReroutes[i]);
                }
                if (draggedOutputTargetGuid >= 0) gridPoints.Add(portConnectionPoints[draggedOutputTargetGuid].center);
                else gridPoints.Add(WindowToGridPosition(Event.current.mousePosition));

                DrawNoodle(col, gridPoints);

                Color bgcol = Color.black;
                Color frcol = col;
                bgcol.a = 0.6f;
                frcol.a = 0.6f;

                // Loop through reroute points again and draw the points
                for (int i = 0; i < draggedOutputReroutes.Count; i++)
                {
                    // Draw reroute point at position
                    Rect rect = new Rect(draggedOutputReroutes[i], new Vector2(16, 16));
                    rect.position = new Vector2(rect.position.x - 8, rect.position.y - 8);
                    rect = GridToWindowRect(rect);

                    GraphNode.DrawPortHandle(rect, bgcol, frcol);
                }
            }
            else
            {
                IPortNode draggedInput = PortUtil.GetPort(draggedInputGuid);
                if(draggedInput!=null)
                {
                    Color col = draggedInput.GetColor();
                    Rect fromRect;
                    if (!m_portConnectionPoints.TryGetValue(draggedInputGuid, out fromRect)) return;
                    List<Vector2> gridPoints = new List<Vector2>();
                    gridPoints.Add(fromRect.center);
                    gridPoints.Add(WindowToGridPosition(Event.current.mousePosition));

                    DrawNoodle(col, gridPoints);
                }
            }
        }
        //------------------------------------------------------
        void DrawNodes()
        {
            Event e = Event.current;
            if (e.type == EventType.Layout)
            {
                if(m_SelectionCache == null)
                    m_SelectionCache = new List<IGraphNode>();
            }

            BeginZoomed();
            Vector2 mousePos = Event.current.mousePosition;
            if (e.type != EventType.Layout)
            {
                hoveredNode = null;
                hoveredPortGuid = -1;
            }
            List<IGraphNode> preSelection = m_preBoxSelection != null ? new List<IGraphNode>(m_preBoxSelection) : new List<IGraphNode>();

            PortUtil.Clear();

            Vector2 boxStartPos = GridToWindowPositionNoClipped(dragBoxStart);
            Vector2 boxSize = mousePos - boxStartPos;
            if (boxSize.x < 0) { boxStartPos.x += boxSize.x; boxSize.x = Mathf.Abs(boxSize.x); }
            if (boxSize.y < 0) { boxStartPos.y += boxSize.y; boxSize.y = Mathf.Abs(boxSize.y); }
            Rect selectionBox = new Rect(boxStartPos, boxSize);

            Color guiColor = GUI.color;

            if(e.type == EventType.Repaint)
            {
                lastportConnectionPoints.Clear();
                foreach (var kvp in portConnectionPoints)
                {
                    lastportConnectionPoints.Add(kvp.Key);
                }
            }

            if (e.type == EventType.Layout)
            {
                if (m_culledNodes == null)
                    m_culledNodes = new List<GraphNode>();
                else
                    m_culledNodes.Clear();
            }
            if (Event.current.type != EventType.ScrollWheel)
            {
                foreach (var db in m_vActioNodes)
                {
                    //  Culling
                    //                  if (e.type == EventType.Layout)
                    //                 {
                    //                     // Cull unselected nodes outside view
                    //                     if (!m_SelectionCache.Contains(db.Value) && ShouldBeCulled(db.Value.GetPosition()))
                    //                     {
                    //                         m_culledNodes.Add(db.Value);
                    //                         continue;
                    // 
                    //                     }
                    //                 }
                    //                 else if (m_culledNodes.Contains(db.Value)) continue;

                    if (e.type == EventType.Layout)
                    {
                        float width = db.Value.GetWidth() / zoom;
                        float height = db.Value.GetHeight() / zoom;
                        Vector2 curSize = Vector2.zero;
                        if (nodeSizes.TryGetValue(db.Value, out curSize))
                        {
                            width = curSize.x / zoom;
                            height = curSize.y / zoom;
                        }
                        if (ShouldBeCulled(db.Value.GetPosition(), new Vector2(width, height)))
                        {
                            m_culledNodes.Add(db.Value);
                        }
                    }
                    Vector2 nodePos = GridToWindowPositionNoClipped(db.Value.GetPosition());

                    AgentTreeUtl.BeginArea(new Rect(nodePos, new Vector2(db.Value.GetWidth(), 4000)));
                    try
                    {
                        bool bCulled = m_culledNodes.Contains(db.Value);
                        if(!bCulled)
                        {
                            bCulled = !db.Value.IsExpand();
                        }

                        bool selected = m_SelectionCache.Contains(db.Value);
                        if (selected)
                        {
                            GUIStyle style = new GUIStyle(m_pEditor.GetBodyStyle());
                            GUIStyle highlightStyle = new GUIStyle(AgentTreeEditorResources.styles.nodeHighlight);
                            highlightStyle.padding = style.padding;
                            style.padding = new RectOffset();
                            GUI.color = db.Value.GetTint();
                            AgentTreeUtl.BeginVertical(style);
                            GUI.color = AgentTreePreferences.GetSettings().highlightColor;
                            AgentTreeUtl.BeginVertical(new GUIStyle(highlightStyle));
                        }
                        else
                        {
                            GUIStyle style = new GUIStyle(m_pEditor.GetBodyStyle());
                            if (IsExcudingNode(db.Value.GetGUID()))
                                GUI.color = AgentTreePreferences.GetSettings().excudeColor;
                            else
                                GUI.color = db.Value.GetTint();
                            if (m_pFinderInspector.IsSearching(db.Value))
                            {
                                GUI.color = AgentTreePreferences.GetSettings().nodeSearchColor;
                            }
                            AgentTreeUtl.BeginVertical(style);
                        }

                        GUI.color = guiColor;
                        EditorGUI.BeginChangeCheck();

                        if (!bCulled)
                        {
                            Vector2 mp = GridToWindowPosition(db.Value.GetPosition());
                            bool bInSecondView = m_pSearcher.IsMouseIn(mp) || m_pFinderInspector.IsMouseIn(mp);
                            EditorGUI.BeginDisabledGroup(bInSecondView);

                            db.Value.OnHeaderGUI();
                            if (zoom <= 1.5f)
                                db.Value.OnBodyGUI();
                            EditorGUI.EndDisabledGroup();
                        }

                        if (EditorGUI.EndChangeCheck())
                        {
                            m_pEditor.OnUpdateNode(db.Value);
                        }
                        AgentTreeUtl.EndVertical();

                        if (e.type == EventType.Repaint)
                        {
                            Vector2 size = GUILayoutUtility.GetLastRect().size;
                            if (!bCulled)
                            {
                                if (nodeSizes.ContainsKey(db.Value)) nodeSizes[db.Value] = size;
                                else nodeSizes.Add(db.Value, size);
                            }

                            foreach (ArgvPort input in db.Value.Inputs)
                            {
                                Vector2 portHandlePos = input.GetRect().center;
                                portHandlePos += db.Value.GetPosition();
                                Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                                portConnectionPoints[input.GetGUID()] = rect;
                                if (lastportConnectionPoints.Contains(input.GetGUID())) lastportConnectionPoints.Remove(input.GetGUID());

                            }

                            foreach (ArgvPort output in db.Value.Outputs)
                            {
                                Vector2 portHandlePos = output.GetRect().center;
                                portHandlePos += db.Value.GetPosition();
                                Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                                portConnectionPoints[output.GetGUID()] = rect;
                                if (lastportConnectionPoints.Contains(output.GetGUID())) lastportConnectionPoints.Remove(output.GetGUID());
                            }

                            if (db.Value.bLink)
                            {
                                {
                                    Vector2 portHandlePos = db.Value.InLink.GetRect().center;
                                    portHandlePos += db.Value.GetPosition();
                                    Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                                    portConnectionPoints[db.Value.InLink.GetGUID()] = rect;
                                    if (lastportConnectionPoints.Contains(db.Value.InLink.GetGUID())) lastportConnectionPoints.Remove(db.Value.InLink.GetGUID());
                                }

                                {
                                    Vector2 portHandlePos = db.Value.OutLink.GetRect().center;
                                    portHandlePos += db.Value.GetPosition();
                                    Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                                    portConnectionPoints[db.Value.OutLink.GetGUID()] = rect;
                                    if (lastportConnectionPoints.Contains(db.Value.OutLink.GetGUID())) lastportConnectionPoints.Remove(db.Value.OutLink.GetGUID());
                                }
                            }
                            if (db.Value.OutConditionLinks != null)
                            {
                                for (int i = 0; i < db.Value.OutConditionLinks.Count; ++i)
                                {
                                    Vector2 portHandlePos = db.Value.OutConditionLinks[i].GetRect().center;
                                    portHandlePos += db.Value.GetPosition();
                                    Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                                    portConnectionPoints[db.Value.OutConditionLinks[i].GetGUID()] = rect;
                                    if (lastportConnectionPoints.Contains(db.Value.OutConditionLinks[i].GetGUID())) lastportConnectionPoints.Remove(db.Value.OutConditionLinks[i].GetGUID());
                                }
                            }
                            if (db.Value.OutDelegateLinks != null)
                            {
                                for (int i = 0; i < db.Value.OutDelegateLinks.Count; ++i)
                                {
                                    Vector2 portHandlePos = db.Value.OutDelegateLinks[i].GetRect().center;
                                    portHandlePos += db.Value.GetPosition();
                                    Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                                    portConnectionPoints[db.Value.OutDelegateLinks[i].GetGUID()] = rect;
                                    if (lastportConnectionPoints.Contains(db.Value.OutDelegateLinks[i].GetGUID())) lastportConnectionPoints.Remove(db.Value.OutDelegateLinks[i].GetGUID());
                                }
                            }
                        }

                        if (selected) AgentTreeUtl.EndVertical();

                        if (e.type != EventType.Layout)
                        {
                            Vector2 nodeSize = GUILayoutUtility.GetLastRect().size;
                            Rect windowRect = new Rect(nodePos, nodeSize);
                            if (windowRect.Contains(mousePos)) hoveredNode = db.Value;

                            if (currentActivity == EActivityType.DragGrid)
                            {
                                if (windowRect.Overlaps(selectionBox))
                                    preSelection.Add(db.Value);
                            }

                            foreach (ArgvPort input in db.Value.Inputs)
                            {
                                //Check if port rect is available
                                if (!portConnectionPoints.ContainsKey(input.GetGUID())) continue;
                                Rect r = GridToWindowRectNoClipped(portConnectionPoints[input.GetGUID()]);
                                if (r.Contains(mousePos)) hoveredPortGuid = input.GetGUID();
                            }

                            foreach (ArgvPort output in db.Value.Outputs)
                            {
                                //Check if port rect is available
                                if (!portConnectionPoints.ContainsKey(output.GetGUID())) continue;
                                Rect r = GridToWindowRectNoClipped(portConnectionPoints[output.GetGUID()]);
                                if (r.Contains(mousePos)) hoveredPortGuid = output.GetGUID();
                            }

                            if (db.Value.bLink)
                            {
                                {
                                    if (!portConnectionPoints.ContainsKey(db.Value.InLink.GetGUID())) continue;
                                    Rect r = GridToWindowRectNoClipped(portConnectionPoints[db.Value.InLink.GetGUID()]);
                                    if (r.Contains(mousePos)) hoveredPortGuid = db.Value.InLink.GetGUID();
                                }
                                {
                                    if (!portConnectionPoints.ContainsKey(db.Value.OutLink.GetGUID())) continue;
                                    Rect r = GridToWindowRectNoClipped(portConnectionPoints[db.Value.OutLink.GetGUID()]);
                                    if (r.Contains(mousePos)) hoveredPortGuid = db.Value.OutLink.GetGUID();
                                }
                            }
                            if (db.Value.OutConditionLinks != null)
                            {
                                for (int i = 0; i < db.Value.OutConditionLinks.Count; ++i)
                                {
                                    if (!portConnectionPoints.ContainsKey(db.Value.OutConditionLinks[i].GetGUID())) continue;
                                    Rect r = GridToWindowRectNoClipped(portConnectionPoints[db.Value.OutConditionLinks[i].GetGUID()]);
                                    if (r.Contains(mousePos)) hoveredPortGuid = db.Value.OutConditionLinks[i].GetGUID();
                                }
                            }
                            if (db.Value.OutDelegateLinks != null)
                            {
                                for (int i = 0; i < db.Value.OutDelegateLinks.Count; ++i)
                                {
                                    if (!portConnectionPoints.ContainsKey(db.Value.OutDelegateLinks[i].GetGUID())) continue;
                                    Rect r = GridToWindowRectNoClipped(portConnectionPoints[db.Value.OutDelegateLinks[i].GetGUID()]);
                                    if (r.Contains(mousePos)) hoveredPortGuid = db.Value.OutDelegateLinks[i].GetGUID();
                                }
                            }
                        }
                    }
                    catch/* (System.Exception ex)*/
                    {
                       // Debug.Log(ex.ToString());
                    }

                    AgentTreeUtl.EndArea();
                }

                foreach(var db in m_vRefPorts)
                {
                    bool selected = m_SelectionCache.Contains(db.Value);
                    Vector2 nodePos = GridToWindowPositionNoClipped(db.Value.GetPosition());
                    try
                    {
                        AgentTreeUtl.BeginArea(new Rect(nodePos, new Vector2(db.Value.GetWidth(), 4000)));
                        if (selected)
                        {
                            GUIStyle style = new GUIStyle(m_pEditor.GetBodyStyle());
                            GUIStyle highlightStyle = new GUIStyle(AgentTreeEditorResources.styles.nodeHighlight);
                            highlightStyle.padding = style.padding;
                            style.padding = new RectOffset();
                            GUI.color = db.Value.GetTint();
                            AgentTreeUtl.BeginVertical(style);
                            GUI.color = AgentTreePreferences.GetSettings().highlightColor;
                            AgentTreeUtl.BeginVertical(new GUIStyle(highlightStyle));
                        }
                        else
                        {
                            GUIStyle style = new GUIStyle(m_pEditor.GetBodyStyle());
                            GUI.color = db.Value.GetTint();
                            AgentTreeUtl.BeginVertical(style);
                        }

                        GUI.color = guiColor;

                        Vector2 mp = GridToWindowPosition(db.Value.GetPosition());
                        bool bInSecondView = m_pSearcher.IsMouseIn(mp) || m_pFinderInspector.IsMouseIn(mp);
                        EditorGUI.BeginDisabledGroup(bInSecondView);
                        db.Value.OnBodyGUI();
                        EditorGUI.EndDisabledGroup();
                        AgentTreeUtl.EndVertical();
                        if (selected) AgentTreeUtl.EndVertical();

                        if (e.type != EventType.Layout)
                        {
                            Rect windowRect = new Rect(nodePos, new Vector2(db.Value.GetWidth(), db.Value.GetHeight()));
                            if (windowRect.Contains(mousePos)) hoveredNode = db.Value;

                            if (currentActivity == EActivityType.DragGrid)
                            {
                                if (windowRect.Overlaps(selectionBox))
                                    preSelection.Add(db.Value);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.Log(ex.ToString());
                    }
                    
                    AgentTreeUtl.EndArea();
                }
            }
            

            if (e.type == EventType.Repaint)
            {
                foreach (var ke in lastportConnectionPoints)
                {
                    portConnectionPoints.Remove(ke);
                }
                lastportConnectionPoints.Clear();
            }

            if (e.type != EventType.Layout && currentActivity == EActivityType.DragGrid)
            {
                m_SelectionCache = preSelection;
            }
            EndZoomed();
        }
        //------------------------------------------------------
        private bool ShouldBeCulled(Vector2 pos, Vector2 size)
        {
            Vector2 nodePos = GridToWindowPosition(pos);
            if ( (nodePos.x + size.x) >= 0 && (nodePos.x) <= m_pEditor.position.width &&
                (nodePos.y + size.y) >= 0 && (nodePos.y ) <= m_pEditor.position.height)
                return false;
            return true;
        }
        //------------------------------------------------------
        void DrawSelectionBox()
        {
            if (currentActivity == EActivityType.DragGrid)
            {
                Vector2 curPos = WindowToGridPosition(Event.current.mousePosition);
                Vector2 size = curPos - dragBoxStart;
                Rect r = new Rect(dragBoxStart, size);
                r.position = GridToWindowPosition(r.position);
                r.size /= zoom;
                Handles.DrawSolidRectangleWithOutline(r, new Color(0, 0, 0, 0.1f), new Color(1, 1, 1, 0.6f));
            }
        }
        //------------------------------------------------------
        void DrawTooltip()
        {
            if (!AgentTreePreferences.GetSettings().portTooltips) return;
            string ToolTips = "";
            IPortNode hoveredPort = PortUtil.GetPort(hoveredPortGuid);
            if (hoveredPort != null && hoveredPort.GetNode() != null)
            {
                if (hoveredPort is ArgvPort)
                {
                    ArgvPort argvPort = (ArgvPort)hoveredPort;
                    if (argvPort.baseNode == null || argvPort.baseNode.BindNode == null || argvPort.variable == null || argvPort.index < 0) return;
                    ToolTips = argvPort.baseNode.BindNode.ToTips(argvPort);
                }
            }
            else if(hoveredNode!=null && IsHoveringTitle(hoveredNode,false))
            {
                if(!string.IsNullOrEmpty(hoveredNode.ToTitleTips()))
                    ToolTips = hoveredNode.ToTitleTips();
            }
            if (!string.IsNullOrEmpty(ToolTips))
            {
                GUIContent content = new GUIContent();
                content.text = ToolTips;
                Vector2 size = AgentTreeEditorResources.styles.tooltip.CalcSize(content);
                Rect rect = new Rect(Event.current.mousePosition - (size), size);
                EditorGUI.LabelField(rect, content, AgentTreeEditorResources.styles.tooltip);
                Repaint();
            }
        }
        //------------------------------------------------------
        void BeginZoomed()
        {
            AgentTreeUtl.EndGroup();

            Rect position = new Rect(m_pEditor.position);
            position.x = 0;
            position.y = topPadding;

            Vector2 topLeft = new Vector2(position.xMin, position.yMin - topPadding);
            Rect clippedArea = ScaleSizeBy(position, zoom, topLeft);
            AgentTreeUtl.BeginGroup(clippedArea);

            prevGuiMatrix = GUI.matrix;
            Matrix4x4 translation = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(1.0f / zoom, 1.0f / zoom, 1.0f));
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;
        }
        //------------------------------------------------------
        public void EndZoomed()
        {
            GUI.matrix = prevGuiMatrix;
            AgentTreeUtl.EndGroup();
            AgentTreeUtl.BeginGroup(new Rect(0.0f, topPadding - (topPadding * zoom), Screen.width, Screen.height));
        }
        //------------------------------------------------------
        public static Rect ScaleSizeBy(Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }
        //------------------------------------------------------
        public void DrawGrid(Rect rect, float zoom, Vector2 panOffset)
        {
            rect.position = Vector2.zero;

            Vector2 center = rect.size / 2f;
            Texture2D gridTex = m_pEditor.GetGridTexture();
            Texture2D crossTex = m_pEditor.GetSecondaryGridTexture();

            // Offset from origin in tile units
            float xOffset = -(center.x * zoom + panOffset.x) / gridTex.width;
            float yOffset = ((center.y - rect.size.y) * zoom + panOffset.y) / gridTex.height;

            Vector2 tileOffset = new Vector2(xOffset, yOffset);

            // Amount of tiles
            float tileAmountX = Mathf.Round(rect.size.x * zoom) / gridTex.width;
            float tileAmountY = Mathf.Round(rect.size.y * zoom) / gridTex.height;

            Vector2 tileAmount = new Vector2(tileAmountX, tileAmountY);

            // Draw tiled background
            GUI.DrawTextureWithTexCoords(rect, gridTex, new Rect(tileOffset, tileAmount));
            GUI.DrawTextureWithTexCoords(rect, crossTex, new Rect(tileOffset + new Vector2(0.5f, 0.5f), tileAmount));
        }
        //------------------------------------------------------
        public void DrawNoodle(Color col, List<Vector2> gridPoints, AgentTreePreferences.NoodleType linkType = AgentTreePreferences.NoodleType.Count, float Width = 5)
        {
//             bool bCulled = true;
//             for(int i = 0; i < gridPoints.Count; ++i)
//             {
//                 if (!ShouldBeCulled(new Vector2(gridPoints[i].x, gridPoints[i].y), Vector2.zero))
//                 {
//                     bCulled = false;
//                 }
//             }
//             if (bCulled) return;
            if (linkType == AgentTreePreferences.NoodleType.Count) linkType = AgentTreePreferences.GetSettings().noodleType;
            Vector2[] windowPoints = gridPoints.Select(x => GridToWindowPosition(x)).ToArray();
            Handles.color = col;
            int length = gridPoints.Count;
            switch (linkType)
            {
                case AgentTreePreferences.NoodleType.Curve:
                    Vector2 outputTangent = Vector2.right;
                    for (int i = 0; i < length - 1; i++)
                    {
                        Vector2 inputTangent = Vector2.left;

                        if (i == 0) outputTangent = Vector2.right * Vector2.Distance(windowPoints[i], windowPoints[i + 1]) * 0.01f * zoom;
                        if (i < length - 2)
                        {
                            Vector2 ab = (windowPoints[i + 1] - windowPoints[i]).normalized;
                            Vector2 cb = (windowPoints[i + 1] - windowPoints[i + 2]).normalized;
                            Vector2 ac = (windowPoints[i + 2] - windowPoints[i]).normalized;
                            Vector2 p = (ab + cb) * 0.5f;
                            float tangentLength = (Vector2.Distance(windowPoints[i], windowPoints[i + 1]) + Vector2.Distance(windowPoints[i + 1], windowPoints[i + 2])) * 0.005f * zoom;
                            float side = ((ac.x * (windowPoints[i + 1].y - windowPoints[i].y)) - (ac.y * (windowPoints[i + 1].x - windowPoints[i].x)));

                            p = new Vector2(-p.y, p.x) * Mathf.Sign(side) * tangentLength;
                            inputTangent = p;
                        }
                        else
                        {
                            inputTangent = Vector2.left * Vector2.Distance(windowPoints[i], windowPoints[i + 1]) * 0.01f * zoom;
                        }

                        Handles.DrawBezier(windowPoints[i], windowPoints[i + 1], windowPoints[i] + ((outputTangent * 50) / zoom), windowPoints[i + 1] + ((inputTangent * 50) / zoom), col, null, Width);
                        outputTangent = -inputTangent;
                    }
                    break;
                case AgentTreePreferences.NoodleType.Line:
                    for (int i = 0; i < length - 1; i++)
                    {
                        Handles.DrawAAPolyLine(Width, windowPoints[i], windowPoints[i + 1]);
                    }
                    break;
                case AgentTreePreferences.NoodleType.Angled:
                    for (int i = 0; i < length - 1; i++)
                    {
                        if (i == length - 1) continue; // Skip last index
                        if (windowPoints[i].x <= windowPoints[i + 1].x - (50 / zoom))
                        {
                            float midpoint = (windowPoints[i].x + windowPoints[i + 1].x) * 0.5f;
                            Vector2 start_1 = windowPoints[i];
                            Vector2 end_1 = windowPoints[i + 1];
                            start_1.x = midpoint;
                            end_1.x = midpoint;
                            Handles.DrawAAPolyLine(Width, windowPoints[i], start_1);
                            Handles.DrawAAPolyLine(Width, start_1, end_1);
                            Handles.DrawAAPolyLine(Width, end_1, windowPoints[i + 1]);
                        }
                        else
                        {
                            float midpoint = (windowPoints[i].y + windowPoints[i + 1].y) * 0.5f;
                            Vector2 start_1 = windowPoints[i];
                            Vector2 end_1 = windowPoints[i + 1];
                            start_1.x += 25 / zoom;
                            end_1.x -= 25 / zoom;
                            Vector2 start_2 = start_1;
                            Vector2 end_2 = end_1;
                            start_2.y = midpoint;
                            end_2.y = midpoint;
                            Handles.DrawAAPolyLine(Width, windowPoints[i], start_1);
                            Handles.DrawAAPolyLine(Width, start_1, start_2);
                            Handles.DrawAAPolyLine(Width, start_2, end_2);
                            Handles.DrawAAPolyLine(Width, end_2, end_1);
                            Handles.DrawAAPolyLine(Width, end_1, windowPoints[i + 1]);
                        }
                    }
                    break;
            }
        }
    }
}
#endif