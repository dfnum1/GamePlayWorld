#if UNITY_EDITOR && USE_GUIDESYSTEM
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Plugin.Guide
{
    public partial class GuideEditorLogic
    {
        float m_fGridRecodAlpha = 0.5f;
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

            m_vConnectionsSets.Clear();
            Color col = GUI.color;
            foreach (var db in m_vActioNodes)
            {
                DrawConnection(db.Value);
            }
            GUI.color = col;
        }
        //------------------------------------------------------
        void DrawConnection(GraphNode node)
        {
            int guid = node.GetGUID();
            for (int i = 0; i < node.Port.Count; ++i)
            {
                if (node.Port[i].GetNode() == null) continue;
                IPort port = node.Port[i].GetPort();
                if ((port is VarPort))
                {
                    VarPort varPort = (VarPort)port;

                    Rect fromRect = Rect.zero;
                    if (!m_portConnectionPoints.TryGetValue(node.Port[i].GetGUID(), out fromRect)) continue;

                    Color connectionColor = node.Port[i].GetColor();
                    foreach (var indb in m_vActioNodes)
                    {
                        if (indb.Value.bindNode == null || indb.Key == guid) continue;

                        if (indb.Value.Port.Count <= 0 || !varPort.ContainsDummyArgv(indb.Key)) continue;

                        ArgvPort argvPort = varPort.dummyMaps[indb.Key];
                        if (argvPort == null) continue;
                        IPortNode linkPort = indb.Value.ContainsPort(argvPort);
                        if (linkPort == null) continue;

                        Rect toRect;
                        if (!m_portConnectionPoints.TryGetValue(linkPort.GetGUID(), out toRect)) continue;

                        System.Int64 key = Mathf.Min(linkPort.GetGUID(), node.Port[i].GetGUID()) << 32 | Mathf.Max(linkPort.GetGUID(), node.Port[i].GetGUID());
                        if (m_vConnectionsSets.Contains(key)) continue;

                        List<Vector2> gridPoints = new List<Vector2>();
                        gridPoints.Add(toRect.center);
                        gridPoints.Add(fromRect.center);
                        DrawNoodle(connectionColor, gridPoints);

                        m_vConnectionsSets.Add(key);
                    }
                }
                else if( port is ArgvPort )
                {
                    ArgvPort argvPort = (ArgvPort)port;

                    Rect fromRect = Rect.zero;
                    if (!m_portConnectionPoints.TryGetValue(node.Port[i].GetGUID(), out fromRect)) continue;

                    Color connectionColor = node.Port[i].GetColor();
                    foreach (var indb in m_vActioNodes)
                    {
                        if (indb.Value.bindNode == null || indb.Key == guid) continue;
                        IPortNode linkPort = indb.Value.ContainsPort(argvPort);
                        if (linkPort == null) continue;

                        Rect toRect;
                        if (!m_portConnectionPoints.TryGetValue(linkPort.GetGUID(), out toRect)) continue;

                        System.Int64 key = Mathf.Min(linkPort.GetGUID(), node.Port[i].GetGUID()) << 32 | Mathf.Max(linkPort.GetGUID(), node.Port[i].GetGUID());
                        if (m_vConnectionsSets.Contains(key)) continue;

                        List<Vector2> gridPoints = new List<Vector2>();
                        gridPoints.Add(fromRect.center);
                        gridPoints.Add(toRect.center);
                        DrawNoodle(connectionColor, gridPoints);

                        m_vConnectionsSets.Add(key);
                    }
                }
            }
            //node link
            if (node.bLinkOut)
            {
                Rect fromRect;
                if (!m_portConnectionPoints.TryGetValue(node.linkOutPort.GetGUID(), out fromRect)) return;
                Color connectionColor = GuidePreferences.GetSettings().linkLineColor;
                float wdith = GuidePreferences.GetSettings().linkLineWidth;
                for (int i = 0; i < node.Links.Count; ++i)
                {
                    GraphNode grapNode = node.Links[i];
                    if (!grapNode.bLinkIn) continue;
                    if (grapNode.GetGUID() == node.GetGUID()) continue;
                    Rect toRect;
                    if (!m_portConnectionPoints.TryGetValue(grapNode.linkInPort.GetGUID(), out toRect)) continue;

                    List<Vector2> gridPoints = new List<Vector2>();
                    gridPoints.Add(fromRect.center);
                    gridPoints.Add(toRect.center);
                    DrawNoodle(connectionColor, gridPoints, GuidePreferences.NoodleType.Count, wdith);
                }
            }

           // if( (node.bindNode is SeqNode) && (node.bindNode as SeqNode).IsAutoNext())
            {
                for (int i = 0; i < node.vExternPorts.Count; ++i)
                {
                    if (node.vExternPorts[i].direction != EPortIO.LinkOut) continue;
                    Rect fromRect;
                    if (!m_portConnectionPoints.TryGetValue(node.vExternPorts[i].GetGUID(), out fromRect)) return;
                    Color connectionColor = node.vExternPorts[i].GetColor();
                    float wdith = GuidePreferences.GetSettings().linkLineWidth;
                    for (int j = 0; j < node.vExternPorts[i].vLinks.Count; ++j)
                    {
                        GraphNode grapNode = node.vExternPorts[i].vLinks[j];
                        if (!grapNode.bLinkIn) continue;
                        if (grapNode.GetGUID() == node.GetGUID()) continue;
                        Rect toRect;
                        if (!m_portConnectionPoints.TryGetValue(grapNode.linkInPort.GetGUID(), out toRect)) continue;

                        List<Vector2> gridPoints = new List<Vector2>();
                        gridPoints.Add(fromRect.center);
                        gridPoints.Add(toRect.center);
                        DrawNoodle(connectionColor, gridPoints, GuidePreferences.NoodleType.Count, wdith);
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
        }
        //------------------------------------------------------
        void DrawNodes()
        {
            Event e = Event.current;
            if (e.type == EventType.Layout)
            {
                if(m_SelectionCache == null)
                    m_SelectionCache = new List<GraphNode>();
            }

            BeginZoomed();
            Vector2 mousePos = Event.current.mousePosition;
            if (e.type != EventType.Layout)
            {
                hoveredNode = null;
                hoveredPortGuid = -1;
            }
            List<GraphNode> preSelection = m_preBoxSelection != null ? new List<GraphNode>(m_preBoxSelection) : new List<GraphNode>();

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
            foreach (var db in m_vActioNodes)
            {
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
                
                BeginArea(new Rect(nodePos, new Vector2(db.Value.GetWidth(), 4000)));
               try
               {
                    bool bCulled = m_culledNodes.Contains(db.Value);
                    bool selected = m_SelectionCache.Contains(db.Value);
                    if (selected)
                    {
                        GUIStyle style = new GUIStyle(m_pEditor.GetBodyStyle());
                        GUIStyle highlightStyle = new GUIStyle(GuideEditorResources.styles.nodeHighlight);
                        highlightStyle.padding = style.padding;
                        style.padding = new RectOffset();
                        if (IsExcudingNode(db.Value.GetGUID()))
                            GUI.color = (db.Value.GetTint() + GuidePreferences.GetSettings().excudeColor)/2;
                        else
                            GUI.color = db.Value.GetTint();
                        BeginVertical(style);
                        GUI.color = GuidePreferences.GetSettings().highlightColor;
                        BeginVertical(new GUIStyle(highlightStyle));
                    }
                    else
                    {
                        GUIStyle style = new GUIStyle(m_pEditor.GetBodyStyle());
                        if(IsExcudingNode(db.Value.GetGUID()))
                            GUI.color = GuidePreferences.GetSettings().excudeColor;
                        else 
                        GUI.color = db.Value.GetTint();
                        BeginVertical(style);
                    }

                    GUI.color = guiColor;
                    EditorGUI.BeginChangeCheck();

                    if (!bCulled)
                    {
                        db.Value.OnHeaderGUI();
                        if (zoom <= 1.5f)
                            db.Value.OnBodyGUI();
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        m_pEditor.OnUpdateNode(db.Value);
                    }
                    EndVertical();

                    if (e.type == EventType.Repaint)
                    {
                        Vector2 size = GUILayoutUtility.GetLastRect().size;
                        if (!bCulled)
                        {
                            if (nodeSizes.ContainsKey(db.Value)) nodeSizes[db.Value] = size;
                            else nodeSizes.Add(db.Value, size);
                        }

                        foreach (IPortNode input in db.Value.Port)
                        {
                            Vector2 portHandlePos = input.GetRect().center;
                            portHandlePos += db.Value.GetPosition();
                            Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                            portConnectionPoints[input.GetGUID()] = rect;
                            if (lastportConnectionPoints.Contains(input.GetGUID())) lastportConnectionPoints.Remove(input.GetGUID());
                        }

                        for(int p =0; p < db.Value.vExternPorts.Count; ++p)
                        {
                            Vector2 portHandlePos = db.Value.vExternPorts[p].GetRect().center;
                            portHandlePos += db.Value.GetPosition();
                            Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                            portConnectionPoints[db.Value.vExternPorts[p].GetGUID()] = rect;
                            if (lastportConnectionPoints.Contains(db.Value.vExternPorts[p].GetGUID())) lastportConnectionPoints.Remove(db.Value.vExternPorts[p].GetGUID());
                        }

                        if (db.Value.bLinkIn)
                        {
                            {
                                Vector2 portHandlePos = db.Value.linkInPort.GetRect().center;
                                portHandlePos += db.Value.GetPosition();
                                Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                                portConnectionPoints[db.Value.linkInPort.GetGUID()] = rect;
                                if (lastportConnectionPoints.Contains(db.Value.linkInPort.GetGUID())) lastportConnectionPoints.Remove(db.Value.linkInPort.GetGUID());
                            }
                        }
                        if (db.Value.bLinkOut)
                        {
                            {
                                Vector2 portHandlePos = db.Value.linkOutPort.GetRect().center;
                                portHandlePos += db.Value.GetPosition();
                                Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                                portConnectionPoints[db.Value.linkOutPort.GetGUID()] = rect;
                                if (lastportConnectionPoints.Contains(db.Value.linkOutPort.GetGUID())) lastportConnectionPoints.Remove(db.Value.linkOutPort.GetGUID());
                            }
                        }
                    }

                    if (selected) EndVertical();

                    if (e.type != EventType.Layout)
                    {
                        Vector2 nodeSize = GUILayoutUtility.GetLastRect().size;
                        Rect windowRect = new Rect(nodePos, nodeSize);
                        if (windowRect.Contains(mousePos)) hoveredNode = db.Value;

                        if (currentActivity == EActivityType.DragGrid)
                        {
                            if (windowRect.Overlaps(selectionBox)) preSelection.Add(db.Value);
                        }

                        foreach (IPortNode input in db.Value.Port)
                        {
                            //Check if port rect is available
                            if (!portConnectionPoints.ContainsKey(input.GetGUID())) continue;
                            Rect r = GridToWindowRectNoClipped(portConnectionPoints[input.GetGUID()]);
                            if (r.Contains(mousePos)) hoveredPortGuid = input.GetGUID();
                        }

                        for(int p =0; p < db.Value.vExternPorts.Count; ++p)
                        {
                            if (!portConnectionPoints.ContainsKey(db.Value.vExternPorts[p].GetGUID())) continue;
                            Rect r = GridToWindowRectNoClipped(portConnectionPoints[db.Value.vExternPorts[p].GetGUID()]);
                            if (r.Contains(mousePos)) hoveredPortGuid = db.Value.vExternPorts[p].GetGUID();
                        }

                        if (db.Value.bLinkIn)
                        {
                            {
                                if (!portConnectionPoints.ContainsKey(db.Value.linkInPort.GetGUID())) continue;
                                Rect r = GridToWindowRectNoClipped(portConnectionPoints[db.Value.linkInPort.GetGUID()]);
                                if (r.Contains(mousePos)) hoveredPortGuid = db.Value.linkInPort.GetGUID();
                            }
                        }
                        if (db.Value.bLinkOut)
                        {
                            {
                                if (!portConnectionPoints.ContainsKey(db.Value.linkOutPort.GetGUID())) continue;
                                Rect r = GridToWindowRectNoClipped(portConnectionPoints[db.Value.linkOutPort.GetGUID()]);
                                if (r.Contains(mousePos)) hoveredPortGuid = db.Value.linkOutPort.GetGUID();
                            }
                        }
                    }
                }
                catch (System.Exception ex)
               {
                    Debug.Log(ex.ToString());
               }
                
                EndArea();
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
        private bool ShouldBeCulled(GraphNode node)
        {
            Vector2 nodePos = GridToWindowPositionNoClipped(node.GetPosition());
            if (nodePos.x / _zoom > node.GetWidth()) return true; // Right
            else if (nodePos.y / _zoom > node.GetHeight()) return true; // Bottom
            else if (nodeSizes.ContainsKey(node))
            {
                Vector2 size = nodeSizes[node];
                if (nodePos.x + size.x < 0) return true; // Left
                else if (nodePos.y + size.y < 0) return true; // Top
            }
            return false;
        }
        //------------------------------------------------------
        private bool ShouldBeCulled(Vector2 pos, Vector2 size)
        {
            Vector2 nodePos = GridToWindowPosition(pos);
            if ((nodePos.x + size.x) >= 0 && (nodePos.x) <= m_pEditor.position.width &&
                (nodePos.y + size.y) >= 0 && (nodePos.y) <= m_pEditor.position.height)
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
            if (!GuidePreferences.GetSettings().portTooltips) return;
            string ToolTips = "";

            IPortNode hoveredPort = PortUtil.GetPort(hoveredPortGuid);
            if (hoveredPort != null && hoveredPort.GetNode() != null)
            {
                if(hoveredPort.GetPort() is ArgvPort)
                    ToolTips += "GUID:" + (hoveredPort.GetPort() as ArgvPort).guid + "\r\n";
                else if (hoveredPort.GetPort() is VarPort)
                    ToolTips += "GUID: 未变量端口，不存在GUID\r\n";

                ToolTips += hoveredPort.GetTips();
                if (hoveredPort.GetPort() is VarPort)
                {
                    VarPort varPort = hoveredPort.GetPort() as VarPort;
                    if (varPort.dummyMaps != null)
                    {
                        foreach (var db in varPort.dummyMaps)
                        {
                            GraphNode pDumNode = null;
                            if (m_vActioNodes.TryGetValue(db.Key, out pDumNode))
                            {
                                IPortNode dummyPort = pDumNode.ContainsPort(db.Value);
                                ToolTips += "D-[ guid:" + db.Key + "->" + pDumNode.bindNode.Name + "]" + db.Value.guid + "-" + (dummyPort != null ? dummyPort.GetTips() : "port 丢失") + "\r\n";
                            }
                            else
                            {
                                ToolTips += "D-节点 丢失\r\n";
                            }
                        }
                    }
                }
            }
            else if (hoveredNode != null && IsHoveringTitle(hoveredNode, false))
            {
                ToolTips += "GUID:" + hoveredNode.GetGUID() + "\r\n";
            }


            if (!string.IsNullOrEmpty(ToolTips))
            {
                GUIContent content = new GUIContent();
                content.text = ToolTips;
                Vector2 size = GuideEditorResources.styles.tooltip.CalcSize(content);
                Rect rect = new Rect(Event.current.mousePosition - (size), size);
                EditorGUI.LabelField(rect, content, GuideEditorResources.styles.tooltip);
                Repaint();
            }
        }
        //------------------------------------------------------
        void BeginZoomed()
        {
            EndGroup();

            Rect position = new Rect(m_pEditor.position);
            position.x = 0;
            position.y = topPadding;

            Vector2 topLeft = new Vector2(position.xMin, position.yMin - topPadding);
            Rect clippedArea = ScaleSizeBy(position, zoom, topLeft);
            BeginGroup(clippedArea);

            prevGuiMatrix = GUI.matrix;
            Matrix4x4 translation = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(1.0f / zoom, 1.0f / zoom, 1.0f));
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;
        }
        //------------------------------------------------------
        public void EndZoomed()
        {
            GUI.matrix = prevGuiMatrix;
            EndGroup();
            BeginGroup(new Rect(0.0f, topPadding - (topPadding * zoom), Screen.width, Screen.height));
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
            Color color = GUI.color;
            if (GuideEditor.IsRecodeMode)
            {
                GUI.color = Color.Lerp(Color.white, new Color(1,0,0,0.85f), Mathf.Sin(m_fGridRecodAlpha));
                m_fGridRecodAlpha += 0.01f;
            }
            else
                GUI.color = Color.white;
            GUI.DrawTextureWithTexCoords(rect, gridTex, new Rect(tileOffset, tileAmount));
            GUI.DrawTextureWithTexCoords(rect, crossTex, new Rect(tileOffset + new Vector2(0.5f, 0.5f), tileAmount));
            GUI.color = color;
        }
        //------------------------------------------------------
        public void DrawNoodle(Color col, List<Vector2> gridPoints, GuidePreferences.NoodleType linkType = GuidePreferences.NoodleType.Count, float Width = 5)
        {
            bool bCulled = true;
            for (int i = 0; i < gridPoints.Count; ++i)
            {
                if (!ShouldBeCulled(new Vector2(gridPoints[i].x, gridPoints[i].y), Vector2.zero))
                {
                    bCulled = false;
                }
            }
            if (bCulled) return;
            if (linkType == GuidePreferences.NoodleType.Count) linkType = GuidePreferences.GetSettings().noodleType;
            Vector2[] windowPoints = gridPoints.Select(x => GridToWindowPosition(x)).ToArray();
            Handles.color = col;
            int length = gridPoints.Count;
            switch (linkType)
            {
                case GuidePreferences.NoodleType.Curve:
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
                case GuidePreferences.NoodleType.Line:
                    for (int i = 0; i < length - 1; i++)
                    {
                        Handles.DrawAAPolyLine(Width, windowPoints[i], windowPoints[i + 1]);
                    }
                    break;
                case GuidePreferences.NoodleType.Angled:
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
        static int m_nAreaClip = 0;
        static int m_nGroupClip = 0;
        static int m_nVerticalClip = 0;
        static int m_nHorizontalClip = 0;
        //------------------------------------------------------
        public static void BeginClip()
        {
            m_nAreaClip = 0;
            m_nGroupClip = 0;
            m_nVerticalClip = 0;
            m_nHorizontalClip = 0;
        }
        //------------------------------------------------------
        public static void EndClip()
        {
            for (int i = 0; i < m_nAreaClip; ++i) EndArea();
            for (int i = 0; i < m_nGroupClip; ++i) EndGroup();
            for (int i = 0; i < m_nVerticalClip; ++i) EndVertical();
            for (int i = 0; i < m_nHorizontalClip; ++i) EndHorizontal();
            m_nAreaClip = 0;
            m_nGroupClip = 0;
            m_nVerticalClip = 0;
            m_nHorizontalClip = 0;
        }
        //------------------------------------------------------
        public static void BeginArea(Rect rect)
        {
            m_nAreaClip++;
            GUILayout.BeginArea(rect);
        }
        //------------------------------------------------------
        public static void BeginArea(Rect rect, GUIStyle style)
        {
            m_nAreaClip++;
            GUILayout.BeginArea(rect, style);
        }
        //------------------------------------------------------
        public static void BeginArea(Rect rect, Texture image)
        {
            m_nAreaClip++;
            if (image != null)
                GUILayout.BeginArea(rect, image);
            else GUILayout.BeginArea(rect);
        }
        //------------------------------------------------------
        public static void EndArea()
        {
            GUILayout.EndArea();
            m_nAreaClip--;
        }
        //------------------------------------------------------
        public static void BeginGroup(Rect rect)
        {
            m_nGroupClip++;
            GUI.BeginGroup(rect);
        }
        //------------------------------------------------------
        public static void BeginGroup(Rect rect, Texture image)
        {
            m_nGroupClip++;
            if (image) GUI.BeginGroup(rect, image);
            else GUI.BeginGroup(rect);
        }
        //------------------------------------------------------
        public static void EndGroup()
        {
            GUI.EndGroup();
            m_nGroupClip--;
        }
        //------------------------------------------------------
        public static void BeginVertical(GUIStyle style = null)
        {
            m_nVerticalClip++;
            if (style != null) GUILayout.BeginVertical(style);
            else GUILayout.BeginVertical();
        }
        //------------------------------------------------------
        public static void EndVertical()
        {
            GUILayout.EndVertical();
            m_nVerticalClip--;
        }
        //------------------------------------------------------
        public static void BeginHorizontal(GUIStyle style = null)
        {
            m_nHorizontalClip++;
            if (style != null) GUILayout.BeginHorizontal(style);
            else GUILayout.BeginHorizontal();
        }
        //------------------------------------------------------
        public static void EndHorizontal()
        {
            GUILayout.EndHorizontal();
            m_nHorizontalClip--;
        }
    }
}
#endif