/********************************************************************
Éú³ÉÈÕÆÚ:	1:11:2020 13:16
Àà    Ãû: 	Visualizer
×÷    Õß:	HappLI
Ãè    Êö:	Plus ²ã´úÀíäÖÈ¾µ÷ÊÔ½Ó¿Ú
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Framework.Core
{
    public class Visualizer
    {
        const int VISUALIZER_NUM_LINES = (1024 * 2);
        const int VISUALIZER_NUM_TRIANGLES = (1024 * 3);
        const int NUM_SEGMENTS = 32;
        public delegate void VISUALIZER_renderPoint2D(Vector3 v, float size, Vector4 color);
        public delegate void VISUALIZER_renderPoint3D(Vector3 v, float size, Vector4 color);
        public delegate void VISUALIZER_renderLine2D(Vector3 v0, Vector3 v1, Vector4 color);
        public delegate void VISUALIZER_renderLine3D(Vector3 v0, Vector3 v1, Vector4 color);
        public delegate void VISUALIZER_renderTriangle2D(Vector3 v0, Vector3 v1, Vector3 v2, Vector4 color);
        public delegate void VISUALIZER_renderTriangle3D(Vector3 v0, Vector3 v1, Vector3 v2, Vector4 color);
        public delegate void VISUALIZER_renderQuad2D(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector4 color);
        public delegate void VISUALIZER_renderQuad3D(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector4 color);
        public delegate void VISUALIZER_renderVector(Vector3 position, Vector3 vector, Vector4 color);
        public delegate void VISUALIZER_renderDirection(Vector3 position, Vector3 direction, Vector4 color);
        public delegate void VISUALIZER_renderBox(Vector3 size, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderFrustum(Matrix4x4 projection, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderCircle(float radius, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderSector(float radius, float angle, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderCone(float radius, float angle, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderSphere(float radius, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderCapsule(float radius, float height, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderCylinder(float radius, float height, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderEllipse(Vector3 radius, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderSolidBox(Vector3 size, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderSolidSphere(float radius, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderSolidCapsule(float radius, float height, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderSolidCylinder(float radius, float height, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderSolidEllipse(Vector3 radius, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderScissor(float x, float y, float width, float height, Vector4 color);
        public delegate void VISUALIZER_renderBoundBox(Vector3 bound_min, Vector3 bound_max, Matrix4x4 transform, Vector4 color);
        public delegate void VISUALIZER_renderBoundSphere(Vector3 center, float radius, Matrix4x4 transform, Vector4 color);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct sInvInterface
        {
            public VISUALIZER_renderPoint2D renderPoint2D;
            public VISUALIZER_renderPoint3D renderPoint3D;
            public VISUALIZER_renderLine2D renderLine2D;
            public VISUALIZER_renderLine3D renderLine3D;
            public VISUALIZER_renderTriangle2D renderTriangle2D;
            public VISUALIZER_renderTriangle3D renderTriangle3D;
            public VISUALIZER_renderQuad2D renderQuad2D;
            public VISUALIZER_renderQuad3D renderQuad3D;
            public VISUALIZER_renderVector renderVector;
            public VISUALIZER_renderDirection renderDirection;
            public VISUALIZER_renderBox renderBox;
            public VISUALIZER_renderFrustum renderFrustum;
            public VISUALIZER_renderCircle renderCircle;
            public VISUALIZER_renderSector renderSector;
            public VISUALIZER_renderCone renderCone;
            public VISUALIZER_renderSphere renderSphere;
            public VISUALIZER_renderCapsule renderCapsule;
            public VISUALIZER_renderCylinder renderCylinder;
            public VISUALIZER_renderEllipse renderEllipse;
            public VISUALIZER_renderSolidBox renderSolidBox;
            public VISUALIZER_renderSolidSphere renderSolidSphere;
            public VISUALIZER_renderSolidCapsule renderSolidCapsule;
            public VISUALIZER_renderSolidCylinder renderSolidCylinder;
            public VISUALIZER_renderSolidEllipse renderSolidEllipse;
            public VISUALIZER_renderScissor renderScissor;
            public VISUALIZER_renderBoundBox renderBoundBox;
            public VISUALIZER_renderBoundSphere renderBoundSphere;
        };
        sInvInterface m_pInvInterface;
        Material m_WireMaterial;
        Material m_WireMaterial2D;

        class Line
        {
            public Vector3 xyz0;
            public Vector3 xyz1;
            public Color color;
        }

        struct Triangle : Base.IQuickSort<Triangle>
        {
            public Vector3 xyz0;
            public Vector3 xyz1;
            public Vector3 xyz2;
            public Color color;
            public float distance;

            public int CompareTo(int type, Triangle other)
            {
                return (int)(distance - other.distance);
            }
            //-----------------------------------------------------
            public void Destroy()
            {
            }
        }

        struct Vertex
        {
            public Vector3 xyz;               // coordinate
            public Vector4 texcoord;          // texture coordinates
            public Color color;
        }

        List<Line> m_vLines2d = new List<Line>();                  // 2d lines
        List<Line> m_vLines3d = new List<Line>();                  // 3d lines
        List<Triangle> m_vTriangles2d = new List<Triangle>();          // 2d triangles
        List<Triangle> m_vTriangles3d = new List<Triangle>();          // 3d triangles

        Matrix4x4 m_ModelView = Matrix4x4.identity;

        Matrix4x4 m_Projection = Matrix4x4.identity;

        int     m_nSize;								// handle size
        Vector3 m_CameraPosition = Vector3.zero;
        Vector3     m_CameraDirection = Vector3.forward;

        Vector2[] m_sincos = new Vector2[NUM_SEGMENTS + 1];
        public Visualizer()
        {
            // m_WireMaterial = (Material)UnityEditor.EditorGUIUtility.LoadRequired("SceneView/HandleLines.mat");
            m_WireMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            m_WireMaterial.hideFlags = HideFlags.HideAndDontSave;
            m_WireMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m_WireMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m_WireMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            m_WireMaterial.SetInt("_ZWrite", 0);

            m_WireMaterial2D = new Material(Shader.Find("Hidden/Internal-Colored"));
            //    m_WireMaterial2D = (Material)UnityEditor.EditorGUIUtility.LoadRequired("SceneView/2DHandleLines.mat");
            //             m_WireMaterial2D = new Material("Shader \"Lines/Colored Blended\" {" +
            //              "SubShader { Pass { " +
            //              "    Blend SrcAlpha OneMinusSrcAlpha " +
            //              "    ZWrite Off Cull Off Fog { Mode Off } " +
            //              "    BindChannels {" +
            //              "      Bind \"vertex\", vertex Bind \"color\", color }" +
            //              "} } }");

            m_pInvInterface = new sInvInterface();

            for (int i = 0; i <= NUM_SEGMENTS; i++)
            {
                float angle = Mathf.PI*2 * i / NUM_SEGMENTS;
                m_sincos[i] = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
            }
        }
        //-------------------------------------------------
        public void Init(ref sInvBridgeInterface invInterface )
        {
        #if UNITY_EDITOR
            m_pInvInterface.renderPoint2D   = renderPoint2D;
            m_pInvInterface.renderPoint3D   = renderPoint3D;
            m_pInvInterface.renderLine2D    = renderLine2D;
            m_pInvInterface.renderLine3D    = renderLine3D;
            m_pInvInterface.renderTriangle2D= renderTriangle2D;
            m_pInvInterface.renderTriangle3D= renderTriangle3D;
            m_pInvInterface.renderQuad2D    = renderQuad2D;
            m_pInvInterface.renderQuad3D    = renderQuad3D;
            m_pInvInterface.renderVector    = renderVector;
            m_pInvInterface.renderDirection = renderDirection;
            m_pInvInterface.renderBox       = renderBox;
            m_pInvInterface.renderFrustum   = renderFrustum;
            m_pInvInterface.renderCircle    = renderCircle;
            m_pInvInterface.renderSector    = renderSector;
            m_pInvInterface.renderCone      = renderCone;
            m_pInvInterface.renderSphere    = renderSphere;
            m_pInvInterface.renderCapsule   = renderCapsule;
            m_pInvInterface.renderCylinder  = renderCylinder;
            m_pInvInterface.renderEllipse   = renderEllipse;
            m_pInvInterface.renderSolidBox  = renderSolidBox;
            m_pInvInterface.renderSolidSphere= renderSolidSphere;
            m_pInvInterface.renderSolidCapsule= renderSolidCapsule;
            m_pInvInterface.renderSolidCylinder= renderSolidCylinder;
            m_pInvInterface.renderScissor= renderScissor;
            m_pInvInterface.renderBoundBox= renderBoundBox;
            m_pInvInterface.renderBoundSphere= renderBoundSphere;

            invInterface.visualizer = m_pInvInterface;
       #endif       
        }
        //-------------------------------------------------
        void Clear()
        {
            m_vLines2d.Clear();
            m_vLines3d.Clear();
            m_vTriangles2d.Clear();
            m_vTriangles3d.Clear();
        }
        //-------------------------------------------------
        public void Render(Camera camera)
        {
            m_Projection = Matrix4x4.identity;// camera.projectionMatrix;
            m_ModelView = Matrix4x4.identity;// camera.cameraToWorldMatrix;
            m_CameraPosition = camera.transform.position;
            m_CameraDirection = camera.transform.forward;

            // 3d primitives
            GL.PushMatrix();
            if (m_vTriangles3d.Count > 0 || m_vLines3d.Count > 0)
            {
                m_WireMaterial.SetPass(0);
                if (m_vTriangles3d.Count > 0)
                {
                    GL.Begin(GL.TRIANGLES);
                    render_triangles(m_vTriangles3d);
                    GL.End();
                }
                if (m_vLines3d.Count > 0)
                {
                    GL.Begin(GL.LINES);
                    render_lines(m_vLines3d);
                    GL.End();
                }
            }

            // 2d primitives
            if ((m_vLines2d.Count > 0 || m_vTriangles2d.Count > 0) && m_WireMaterial2D !=null )
            {
                m_WireMaterial2D.SetPass(0);
                GL.LoadOrtho();
                if (m_vTriangles2d.Count > 0)
                {
                    GL.Begin(GL.TRIANGLES);
                    render_triangles(m_vTriangles2d);
                    GL.End();
                }
                if (m_vLines2d.Count > 0)
                {
                    GL.Begin(GL.LINES);
                    render_lines(m_vLines2d);
                    GL.End();
                }
            }
            GL.PopMatrix();

            Clear();
        }
        //-------------------------------------------------
        float getScale(Vector3 position)
        {
            Vector3 row_3 = m_ModelView.GetRow(1);
            // project position
            Vector4 p0 = m_Projection * m_ModelView * new Vector4(position.x, position.y, position.z, 1.0f);
            Vector4 p1 = m_Projection * m_ModelView * new Vector4(position.x+ row_3.x, position.y+ row_3.y, position.z+ row_3.z, 1.0f);
            if (p0.w < Mathf.Epsilon || p1.w < Mathf.Epsilon) return 0.0f;

            // screen coordinates
            p0.x = (0.5f + p0.x / p0.w * 0.5f) * Screen.width;
            p0.y = (0.5f - p0.y / p0.w * 0.5f) * Screen.height;
            p1.x = (0.5f + p1.x / p1.w * 0.5f) * Screen.width;
            p1.y = (0.5f - p1.y / p1.w * 0.5f) * Screen.height;

            return m_nSize / (new Vector2(p0.x, p0.y) - new Vector2(p1.x, p1.y)).magnitude;
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderPoint2D))]
        public void renderPoint2D(Vector3 v, float size, Vector4 color)
        {
            Vector3 v0 = v + new Vector3(-size, -size, 0.0f);
            Vector3 v1 = v + new Vector3(size, -size, 0.0f);
            Vector3 v2 = v + new Vector3(size, size, 0.0f);
            Vector3 v3 = v + new Vector3(-size, size, 0.0f);

            Triangle t = new Triangle();
            t.color = new Color(color.x, color.y, color.z, color.w);
            t.distance = v.z;

            t.xyz0 = v0;
            t.xyz1 = v1;
            t.xyz2 = v2;
            m_vTriangles2d.Add(t);

            t.xyz0 = v2;
            t.xyz1 = v3;
            t.xyz2 = v0;
            m_vTriangles2d.Add(t);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderPoint3D))]
        public void renderPoint3D(Vector3 v, float size, Vector4 color)
        {
            Vector3 x = m_ModelView.GetRow(0) * size;
            Vector3 y = m_ModelView.GetRow(1) * size;
            Vector3 v0 = v - x - y;
            Vector3 v1 = v + x - y;
            Vector3 v2 = v + x + y;
            Vector3 v3 = v - x + y;

            Triangle t = new Triangle();
            t.color = new Color(color.x, color.y, color.z, color.w);

            t.distance = Vector3.Dot( m_CameraPosition - v, m_CameraDirection);

            t.xyz0 = m_ModelView.MultiplyPoint(v0);
            t.xyz1 = m_ModelView.MultiplyPoint(v1);
            t.xyz2 = m_ModelView.MultiplyPoint(v2);
            m_vTriangles3d.Add(t);

            t.xyz0 = m_ModelView.MultiplyPoint(v2);
            t.xyz1 = m_ModelView.MultiplyPoint(v3);
            t.xyz2 = m_ModelView.MultiplyPoint(v0);
            m_vTriangles3d.Add(t);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderLine2D))]
        public void renderLine2D(Vector3 v0, Vector3 v1, Vector4 color)
        {
            Line l = new Line();
            l.color = new Color(color.x, color.y, color.z, color.w);
            l.xyz0 = v0;
            l.xyz1 = v1;
            m_vLines2d.Add(l);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderLine3D))]
        public void renderLine3D(Vector3 v0, Vector3 v1, Vector4 color)
        {
            Line l = new Line();
            l.color = new Color(color.x, color.y, color.z, color.w);
            l.xyz0 = m_ModelView.MultiplyPoint(v0);
            l.xyz1 = m_ModelView.MultiplyPoint(v1);
            m_vLines3d.Add(l);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderTriangle2D))]
        public void renderTriangle2D(Vector3 v0, Vector3 v1, Vector3 v2, Vector4 color)
        {
            Triangle t = new Triangle();
            t.xyz0 = v0;
            t.xyz1 = v1;
            t.xyz2 = v2;
            t.color = new Color(color.x, color.y, color.z, color.w);
            t.distance = (v0.z + v1.z + v2.z) / 3.0f;
            m_vTriangles2d.Add(t);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderTriangle3D))]
        public void renderTriangle3D(Vector3 v0, Vector3 v1, Vector3 v2, Vector4 color)
        {
            Triangle t = new Triangle();
            t.xyz0 = m_ModelView.MultiplyPoint(v0);
            t.xyz1 = m_ModelView.MultiplyPoint(v1);
            t.xyz2 = m_ModelView.MultiplyPoint(v2);
            t.color = new Color(color.x, color.y, color.z, color.w);
            t.distance = Vector3.Dot(m_CameraPosition - (v0 + v1 + v2) / 3.0f, m_CameraDirection);
            m_vTriangles3d.Add(t);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderQuad2D))]
        public void renderQuad2D(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector4 color)
        {
            Triangle t = new Triangle();
            t.color = new Color(color.x, color.y, color.z, color.w);
            t.distance = (v0.z + v1.z + v2.z + v3.z) / 4.0f;

            t.xyz0 = v0;
            t.xyz1 = v1;
            t.xyz2 = v2;
            m_vTriangles2d.Add(t);

            t.xyz0 = v2;
            t.xyz1 = v3;
            t.xyz2 = v0;
            m_vTriangles2d.Add(t);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderQuad3D))]
        public void renderQuad3D(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector4 color)
        {
            Triangle t = new Triangle();
            t.color = new Color(color.x, color.y, color.z, color.w);
            t.distance = Vector3.Dot(m_CameraPosition - (v0 + v1 + v2 + v3) / 4.0f, m_CameraDirection);

            t.xyz0 = m_ModelView.MultiplyPoint(v0);
            t.xyz1 = m_ModelView.MultiplyPoint(v1);
            t.xyz2 = m_ModelView.MultiplyPoint(v2);
            m_vTriangles3d.Add(t);

            t.xyz0 =m_ModelView.MultiplyPoint(v2);
            t.xyz1 =m_ModelView.MultiplyPoint(v3);
            t.xyz2 = m_ModelView.MultiplyPoint(v0);
            m_vTriangles3d.Add(t);
        }
        //-------------------------------------------------
        void orthoBasis(Vector3 v, ref Vector3 tangent, ref Vector3 binormal)
        {
            if (Mathf.Abs(v.z) > 0.7f)
            {
                float length2 = v.y * v.y + v.z * v.z;
                float ilength = 1f/Mathf.Sqrt(length2);
                tangent.x = 0.0f;
                tangent.y = -v.z * ilength;
                tangent.z = v.y * ilength;
                binormal.x = length2 * ilength;
                binormal.y = -v.x * tangent.z;
                binormal.z = v.x * tangent.y;
            }
            else
            {
                float length2 = v.x * v.x + v.y * v.y;
                float ilength = 1f / Mathf.Sqrt(length2);
                tangent.x = -v.y * ilength;
                tangent.y = v.x * ilength;
                tangent.z = 0.0f;
                binormal.x = -v.z * tangent.y;
                binormal.y = v.z * tangent.x;
                binormal.z = length2 * ilength;
            }
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderVector))]
        public void renderVector(Vector3 position, Vector3 vector, Vector4 color)
        {
            // length
            Vector3 direction = vector - position;
            float length = direction.magnitude;
            if (length < Mathf.Epsilon) return;

            // vector line
            renderLine3D(position, vector, color);

            // arrow scale
            float scale = getScale(vector) / 4.0f;

            // vector arrow
            if (scale > Mathf.Epsilon && scale < length / 2.0f)
            {

                direction /= length;

                Vector3 x = Vector3.zero, y = Vector3.zero;
                orthoBasis(direction, ref x,ref y);
                x *= scale * 0.2f;
                y *= scale * 0.2f;

                Vector3 offset = vector - direction * scale;

                for (int i = 0; i < 16; i++)
                {
                    float angle = Mathf.PI * 2 * (i + 0) / 16.0f;
                    float s0, c0, s1, c1;
                    s1 = Mathf.Sin(angle);
                    c1  = Mathf.Cos(angle);

                    angle = Mathf.PI * 2 * (i + 0) / 16.0f;
                    s0 = Mathf.Sin(angle);
                    c0 = Mathf.Cos(angle);

                    Vector3 p0 = offset + x * s0 + y * c0;
                    Vector3 p1 = offset + x * s1 + y * c1;
                    renderTriangle3D(p0, p1, vector, color);
                    renderTriangle3D(p1, p0, vector, color);
                }
            }
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderDirection))]
        public void renderDirection(Vector3 position, Vector3 direction, Vector4 color)
        {
            float scale = getScale(position);

            if (scale > Mathf.Epsilon) renderVector(position, position + direction * scale, color);

        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderBox))]
        public void renderBox(Vector3 size, Matrix4x4  transform, Vector4 color)
        {
            Vector3[] points = {
        new Vector3(-1.0f,-1.0f,-1.0f),  new Vector3(1.0f,-1.0f,-1.0f),  new Vector3(-1.0f,1.0f,-1.0f),  new Vector3(1.0f,1.0f,-1.0f),
         new Vector3(-1.0f,-1.0f, 1.0f),  new Vector3(1.0f,-1.0f, 1.0f),  new Vector3(-1.0f,1.0f, 1.0f),  new Vector3(1.0f,1.0f, 1.0f),
    };
            Matrix4x4 m = transform * Matrix4x4.Scale(size * 0.5f);
            for (int i = 0; i < 8; i++)
            {
                points[i] = m.MultiplyPoint(points[i]);
            }

            renderLine3D(points[0], points[1], color);
            renderLine3D(points[1], points[3], color);
            renderLine3D(points[3], points[2], color);
            renderLine3D(points[2], points[0], color);
            renderLine3D(points[4], points[5], color);
            renderLine3D(points[5], points[7], color);
            renderLine3D(points[7], points[6], color);
            renderLine3D(points[6], points[4], color);
            renderLine3D(points[0], points[4], color);
            renderLine3D(points[1], points[5], color);
            renderLine3D(points[3], points[7], color);
            renderLine3D(points[2], points[6], color);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderFrustum))]
        public void renderFrustum(Matrix4x4  projection, Matrix4x4 transform, Vector4 color)
        {
            Vector3[] points = {
        new Vector3(-1.0f,-1.0f,-1.0f), new Vector3(1.0f,-1.0f,-1.0f), new Vector3(-1.0f,1.0f,-1.0f), new Vector3(1.0f,1.0f,-1.0f),
        new Vector3(-1.0f,-1.0f, 1.0f), new Vector3(1.0f,-1.0f, 1.0f), new Vector3(-1.0f,1.0f, 1.0f), new Vector3(1.0f,1.0f, 1.0f),
    };

            Matrix4x4 m = Matrix4x4.Inverse(m_Projection);
            for (int i = 0; i < 8; i++)
            {
                Vector4 point = m * new Vector4(points[i].x, points[i].y, points[i].z, 1);
                points[i] = transform.MultiplyPoint(new Vector3(point.x, point.y, point.z) / point.w);
            }

            renderLine3D(points[0], points[1], color);
            renderLine3D(points[1], points[3], color);
            renderLine3D(points[3], points[2], color);
            renderLine3D(points[2], points[0], color);
            renderLine3D(points[4], points[5], color);
            renderLine3D(points[5], points[7], color);
            renderLine3D(points[7], points[6], color);
            renderLine3D(points[6], points[4], color);
            renderLine3D(points[0], points[4], color);
            renderLine3D(points[1], points[5], color);
            renderLine3D(points[3], points[7], color);
            renderLine3D(points[2], points[6], color);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderCircle))]
        public void renderCircle(float radius, Matrix4x4  transform, Vector4 color)
        {
            for (int i = 0; i < NUM_SEGMENTS; i++)
            {
                float s0 = m_sincos[i].x * radius;
                float c0 = m_sincos[i].y * radius;
                float s1 = m_sincos[i + 1].x * radius;
                float c1 = m_sincos[i + 1].y * radius;
                renderLine3D(transform.MultiplyPoint(new Vector3(s0, c0, 0.0f)), transform.MultiplyPoint(new Vector3(s1, c1, 0.0f)), color);
            }
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderSector))]
        public void renderSector(float radius, float angle, Matrix4x4  transform, Vector4 color)
        {
            if (Mathf.Abs(angle- 360.0f)<=0.01f)
            {
                for (int i = 0; i < NUM_SEGMENTS; i++)
                {
                    float s0 = m_sincos[i].x * radius;
                    float c0 = m_sincos[i].y * radius;
                    float s1 = m_sincos[i + 1].x * radius;
                    float c1 = m_sincos[i + 1].y * radius;
                    renderLine3D(transform.MultiplyPoint(new Vector3(s0, c0, 0.0f)), transform.MultiplyPoint(new Vector3(s1, c1, 0.0f)), color);
                }
            }
            else
            {
                float scale = angle * Mathf.Deg2Rad / NUM_SEGMENTS;
                float offset = Mathf.PI + (360.0f - angle) * Mathf.Deg2Rad / 2.0f;
                for (int i = 0; i < NUM_SEGMENTS; i++)
                {
                    float a0 = (i + 0) * scale + offset;
                    float a1 = (i + 1) * scale + offset;
                    float s0, c0, s1, c1;
                    s0 = Mathf.Sin(a0);
                    c0 = Mathf.Cos(a0);
                    s1 = Mathf.Sin(a1);
                    c1 = Mathf.Cos(a1);
                    s0 = s0 * radius;
                    c0 = c0 * radius;
                    s1 = s1 * radius;
                    c1 = c1 * radius;
                    renderLine3D(transform.MultiplyPoint(new Vector3(s0, c0, 0.0f)), transform.MultiplyPoint(new Vector3(s1, c1, 0.0f)), color);
                }
                float s2, c2;
                s2 = Mathf.Sin(offset);
                c2 = Mathf.Cos(offset);
                s2 = s2 * radius;
                c2 = c2 * radius;
                renderLine3D(transform.MultiplyPoint(new Vector3(s2, c2, 0.0f)), transform.MultiplyPoint(Vector3.zero), color);
                renderLine3D(transform.MultiplyPoint(new Vector3(-s2, c2, 0.0f)), transform.MultiplyPoint(Vector3.zero), color);
            }
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderCone))]
        public void renderCone(float radius, float angle, Matrix4x4  transform, Vector4 color)
        {
            if (angle == 180.0f)
            {
                for (int i = 0; i < NUM_SEGMENTS; i++)
                {
                    float s0 = m_sincos[i].x * radius;
                    float c0 = m_sincos[i].y * radius;
                    float s1 = m_sincos[i + 1].x * radius;
                    float c1 = m_sincos[i + 1].y * radius;
                    renderLine3D(transform.MultiplyPoint(new Vector3(0.0f, s0, c0)), transform.MultiplyPoint( new Vector3(0.0f, s1, c1)), color);
                    renderLine3D(transform.MultiplyPoint(new Vector3(s0, 0.0f, c0)), transform.MultiplyPoint( new Vector3(s1, 0.0f, c1)), color);
                    renderLine3D(transform.MultiplyPoint(new Vector3(s0, c0, 0.0f)), transform.MultiplyPoint(new Vector3(s1, c1, 0.0f)), color);
                }
            }
            else
            {
                float scale = angle * Mathf.Deg2Rad / NUM_SEGMENTS * 2.0f;
                float offset = Mathf.PI + (180.0f - angle) * Mathf.Deg2Rad;
                for (int i = 0; i < NUM_SEGMENTS; i++)
                {
                    float a0 = (i + 0) * scale + offset;
                    float a1 = (i + 1) * scale + offset;
                    float s0, c0, s1, c1;
                    s0 = Mathf.Sin(a0);
                    c0 = Mathf.Cos(a0);
                    s1 = Mathf.Sin(a1);
                    c1 = Mathf.Cos(a1);
                    s0 = s0 * radius;
                    c0 = c0 * radius;
                    s1 = s1 * radius;
                    c1 = c1 * radius;
                    renderLine3D(transform.MultiplyPoint(new Vector3(0.0f, s0, c0)), transform.MultiplyPoint( new Vector3(0.0f, s1, c1)), color);
                    renderLine3D(transform.MultiplyPoint(new Vector3(s0, 0.0f, c0)), transform.MultiplyPoint(new Vector3(s1, 0.0f, c1)), color);
                }
                float s2, c2;
                s2 = Mathf.Sin(offset);
                c2 = Mathf.Cos(offset);
                s2 = s2 * radius;
                c2 = c2 * radius;
                for (int i = 0; i < NUM_SEGMENTS; i++)
                {
                    float s0 = m_sincos[i].x * s2;
                    float c0 = m_sincos[i].y * s2;
                    float s1 = m_sincos[i + 1].x * s2;
                    float c1 = m_sincos[i + 1].y * s2;
                    renderLine3D(transform.MultiplyPoint(new Vector3(s0, c0, c2)), transform.MultiplyPoint(new Vector3(s1, c1, c2)), color);
                }
                renderLine3D(transform.MultiplyPoint( new Vector3(s2, 0.0f, c2)), transform.MultiplyPoint(Vector3.zero), color);
                renderLine3D(transform.MultiplyPoint( new Vector3(-s2, 0.0f, c2)), transform.MultiplyPoint(Vector3.zero), color);
                renderLine3D(transform.MultiplyPoint( new Vector3(0.0f, s2, c2)), transform.MultiplyPoint(Vector3.zero), color);
                renderLine3D(transform.MultiplyPoint(new Vector3(0.0f, -s2, c2)), transform.MultiplyPoint(Vector3.zero), color);
                if (angle > 90.0f)
                {
                    for (int i = 0; i < NUM_SEGMENTS; i++)
                    {
                        float s0 = m_sincos[i].x * radius;
                        float c0 = m_sincos[i].y * radius;
                        float s1 = m_sincos[i + 1].x * radius;
                        float c1 = m_sincos[i + 1].y * radius;
                        renderLine3D(transform.MultiplyPoint(new Vector3(s0, c0, 0.0f)), transform.MultiplyPoint(new Vector3(s1, c1, 0.0f)), color);
                    }
                }
            }
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderSphere))]
        public void renderSphere(float radius, Matrix4x4  transform, Vector4 color)
        {
            for (int i = 0; i < NUM_SEGMENTS; i++)
            {
                float s0 = m_sincos[i].x * radius;
                float c0 = m_sincos[i].y * radius;
                float s1 = m_sincos[i + 1].x * radius;
                float c1 = m_sincos[i + 1].y * radius;
                renderLine3D(transform.MultiplyPoint( new Vector3(0.0f, s0, c0)), transform.MultiplyPoint(new Vector3(0.0f, s1, c1)), color);
                renderLine3D(transform.MultiplyPoint(new Vector3(s0, 0.0f, c0)), transform.MultiplyPoint(new Vector3(s1, 0.0f, c1)), color);
                renderLine3D(transform.MultiplyPoint(new Vector3(s0, c0, 0.0f)), transform.MultiplyPoint(new Vector3(s1, c1, 0.0f)), color);
            }
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderCapsule))]
        public void renderCapsule(float radius, float height, Matrix4x4  transform, Vector4 color)
        {
            float h = height / 2.0f;
            for (int i = 0; i < NUM_SEGMENTS; i++)
            {
                float s0 = m_sincos[i].x * radius;
                float c0 = m_sincos[i].y * radius;
                float s1 = m_sincos[i + 1].x * radius;
                float c1 = m_sincos[i + 1].y * radius;
                renderLine3D(transform.MultiplyPoint( new Vector3(s0, c0, h)), transform.MultiplyPoint( new Vector3(s1, c1, h)), color);
                renderLine3D(transform.MultiplyPoint(new Vector3(s0, c0, -h)), transform.MultiplyPoint(new Vector3(s1, c1, -h)), color);
            }

            for (int i = 0; i < NUM_SEGMENTS / 2; i++)
            {
                float s0 = m_sincos[i].x * radius;
                float c0 = m_sincos[i].y * radius;
                float s1 = m_sincos[i + 1].x * radius;
                float c1 = m_sincos[i + 1].y * radius;
                renderLine3D(transform.MultiplyPoint(new Vector3(c0, 0.0f, s0 + h)), transform * new Vector3(c1, 0.0f, s1 + h), color);
                renderLine3D(transform.MultiplyPoint(new Vector3(0.0f, c0, s0 + h)), transform * new Vector3(0.0f, c1, s1 + h), color);
                renderLine3D(transform.MultiplyPoint(new Vector3(c0, 0.0f, -s0 - h)), transform * new Vector3(c1, 0.0f, -s1 - h), color);
                renderLine3D(transform.MultiplyPoint(new Vector3(0.0f, c0, -s0 - h)), transform * new Vector3(0.0f, c1, -s1 - h), color);
            }

            renderLine3D(transform * new Vector3(radius, 0.0f, -h), transform.MultiplyPoint(new Vector3(radius, 0.0f, h)), color);
            renderLine3D(transform * new Vector3(-radius, 0.0f, -h),transform.MultiplyPoint( new Vector3(-radius, 0.0f, h)), color);
            renderLine3D(transform * new Vector3(0.0f, radius, -h), transform.MultiplyPoint(new Vector3(0.0f, radius, h)), color);
            renderLine3D(transform * new Vector3(0.0f, -radius, -h), transform.MultiplyPoint(new Vector3(0.0f, -radius, h)), color);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderCylinder))]
        public void renderCylinder(float radius, float height, Matrix4x4  transform, Vector4 color)
        {
            float h = height / 2.0f;
            for (int i = 0; i < NUM_SEGMENTS; i++)
            {
                float s0 = m_sincos[i].x * radius;
                float c0 = m_sincos[i].y * radius;
                float s1 = m_sincos[i + 1].x * radius;
                float c1 = m_sincos[i + 1].y * radius;
                renderLine3D(transform.MultiplyPoint(new Vector3(s0, c0, h)), transform.MultiplyPoint(new Vector3(s1, c1, h)), color);
                renderLine3D(transform.MultiplyPoint(new Vector3(s0, c0, -h)),transform.MultiplyPoint( new Vector3(s1, c1, -h)), color);
            }

            renderLine3D(transform.MultiplyPoint(new Vector3(-radius, 0.0f, -h)),transform.MultiplyPoint( new Vector3(radius, 0.0f, -h)), color);
            renderLine3D(transform.MultiplyPoint(new Vector3(0.0f, -radius, -h)),transform.MultiplyPoint( new Vector3(0.0f, radius, -h)), color);
            renderLine3D(transform.MultiplyPoint(new Vector3(-radius, 0.0f, h)), transform.MultiplyPoint(new Vector3(radius, 0.0f, h)), color);
            renderLine3D(transform.MultiplyPoint(new Vector3(0.0f, -radius, h)), transform.MultiplyPoint(new Vector3(0.0f, radius, h)), color);
            renderLine3D(transform.MultiplyPoint(new Vector3(radius, 0.0f, -h)), transform.MultiplyPoint(new Vector3(radius, 0.0f, h)), color);
            renderLine3D(transform.MultiplyPoint(new Vector3(-radius, 0.0f, -h)),transform.MultiplyPoint( new Vector3(-radius, 0.0f, h)), color);
            renderLine3D(transform.MultiplyPoint(new Vector3(0.0f, radius, -h)), transform.MultiplyPoint(new Vector3(0.0f, radius, h)), color);
            renderLine3D(transform.MultiplyPoint(new Vector3(0.0f, -radius, -h)), transform.MultiplyPoint(new Vector3(0.0f, -radius, h)), color);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderEllipse))]
        public void renderEllipse(Vector3 radius, Matrix4x4  transform, Vector4 color)
        {
            for (int i = 0; i < NUM_SEGMENTS; i++)
            {
                float s0 = m_sincos[i].x;
                float c0 = m_sincos[i].y;
                float s1 = m_sincos[i + 1].x;
                float c1 = m_sincos[i + 1].y;
                renderLine3D(transform.MultiplyPoint(new Vector3(0.0f, radius.y * s0, radius.z * c0)), transform.MultiplyPoint(new Vector3(0.0f, radius.y * s1, radius.z * c1)), color);
                renderLine3D(transform.MultiplyPoint(new Vector3(radius.x * s0, 0.0f, radius.z * c0)), transform.MultiplyPoint(new Vector3(radius.x * s1, 0.0f, radius.z * c1)), color);
                renderLine3D(transform.MultiplyPoint(new Vector3(radius.x * s0, radius.y * c0, 0.0f)), transform.MultiplyPoint(new Vector3(radius.x * s1, radius.y * c1, 0.0f)), color);
            }
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderSolidBox))]
        public void renderSolidBox(Vector3 size, Matrix4x4  transform, Vector4 color)
        {
            Vector3[] points = {
        new Vector3(-1.0f,-1.0f,-1.0f), new Vector3(1.0f,-1.0f,-1.0f), new Vector3(-1.0f,1.0f,-1.0f), new Vector3(1.0f,1.0f,-1.0f),
        new Vector3(-1.0f,-1.0f, 1.0f), new Vector3(1.0f,-1.0f, 1.0f), new Vector3(-1.0f,1.0f, 1.0f), new Vector3(1.0f,1.0f, 1.0f),
    };

            Matrix4x4 m = transform * Matrix4x4.Scale(size * 0.5f);
            for (int i = 0; i < 8; i++)
            {
                points[i] = m * points[i];
            }

            renderLine3D(points[0], points[1], color);
            renderLine3D(points[1], points[3], color);
            renderLine3D(points[3], points[2], color);
            renderLine3D(points[2], points[0], color);
            renderLine3D(points[4], points[5], color);
            renderLine3D(points[5], points[7], color);
            renderLine3D(points[7], points[6], color);
            renderLine3D(points[6], points[4], color);
            renderLine3D(points[0], points[4], color);
            renderLine3D(points[1], points[5], color);
            renderLine3D(points[3], points[7], color);
            renderLine3D(points[2], points[6], color);

            renderQuad3D(points[3], points[1], points[5], points[7], color);
            renderQuad3D(points[0], points[2], points[6], points[4], color);
            renderQuad3D(points[2], points[3], points[7], points[6], color);
            renderQuad3D(points[1], points[0], points[4], points[5], color);
            renderQuad3D(points[4], points[6], points[7], points[5], color);
            renderQuad3D(points[2], points[0], points[1], points[3], color);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderSolidSphere))]
        public void renderSolidSphere(float radius, Matrix4x4  transform, Vector4 color)
        {
            Vector3 v0 = Vector3.zero, v1 = Vector3.zero, v2 = Vector3.zero, v3 = Vector3.zero;
            for (int i = 0; i < NUM_SEGMENTS / 2; i++)
            {
                int phi0 = i + NUM_SEGMENTS / 4 + 0;
                int phi1 = i + NUM_SEGMENTS / 4 + 1;
                for (int j = 0; j <= NUM_SEGMENTS; j += 2)
                {
                    int theta = j % NUM_SEGMENTS;
                    v2 = transform.MultiplyPoint( (new Vector3(m_sincos[phi0].y * m_sincos[theta].y, -m_sincos[phi0].y * m_sincos[theta].x, m_sincos[phi0].x)) * radius);
                    v3 = transform.MultiplyPoint((new Vector3(m_sincos[phi1].y * m_sincos[theta].y, -m_sincos[phi1].y * m_sincos[theta].x, m_sincos[phi1].x)) * radius);
                    if (j != 0)
                    {
                        if (i == 0)
                        {
                            renderLine3D(v3, v2, color);
                            renderLine3D(v2, v1, color);
                            renderLine3D(v1, v3, color);
                            renderTriangle3D(v3, v2, v1, color);
                        }
                        else if (i == NUM_SEGMENTS / 2 - 1)
                        {
                            renderLine3D(v0, v1, color);
                            renderLine3D(v1, v2, color);
                            renderLine3D(v2, v0, color);
                            renderTriangle3D(v0, v1, v2, color);
                        }
                        else
                        {
                            renderLine3D(v0, v1, color);
                            renderLine3D(v1, v3, color);
                            renderLine3D(v3, v2, color);
                            renderLine3D(v2, v0, color);
                            renderQuad3D(v0, v1, v3, v2, color);
                        }
                    }
                    v0 = v2;
                    v1 = v3;
                }
            }
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderSolidCapsule))]
        public void renderSolidCapsule(float radius, float height, Matrix4x4  transform, Vector4 color)
        {
            Vector3 v0 = Vector3.zero, v1 = Vector3.zero, v2 = Vector3.zero, v3 = Vector3.zero;
            float h = height / 2.0f;
            for (int i = 0; i < NUM_SEGMENTS / 2; i++)
            {
                int phi0 = i + NUM_SEGMENTS / 4 + 0;
                int phi1 = i + NUM_SEGMENTS / 4 + 1;
                float z = (i < NUM_SEGMENTS / 4) ? h : -h;
                for (int j = 0; j <= NUM_SEGMENTS; j += 2)
                {
                    int theta = j % NUM_SEGMENTS;
                    v2 = transform.MultiplyPoint((new Vector3(m_sincos[phi0].y * m_sincos[theta].y, -m_sincos[phi0].y * m_sincos[theta].x, m_sincos[phi0].x) * radius + new Vector3(0.0f, 0.0f, z)));
                    v3 = transform.MultiplyPoint((new Vector3(m_sincos[phi1].y * m_sincos[theta].y, -m_sincos[phi1].y * m_sincos[theta].x, m_sincos[phi1].x) * radius + new Vector3(0.0f, 0.0f, z)));
                    if (j != 0)
                    {
                        if (i == 0)
                        {
                            renderLine3D(v3, v2, color);
                            renderLine3D(v2, v1, color);
                            renderLine3D(v1, v3, color);
                            renderTriangle3D(v3, v2, v1, color);
                        }
                        else if (i == NUM_SEGMENTS / 2 - 1)
                        {
                            renderLine3D(v0, v1, color);
                            renderLine3D(v1, v2, color);
                            renderLine3D(v2, v0, color);
                            renderTriangle3D(v0, v1, v2, color);
                        }
                        else
                        {
                            renderLine3D(v0, v1, color);
                            renderLine3D(v1, v3, color);
                            renderLine3D(v3, v2, color);
                            renderLine3D(v2, v0, color);
                            renderQuad3D(v0, v1, v3, v2, color);
                        }
                    }
                    v0 = v2;
                    v1 = v3;
                }
            }
            for (int i = 0; i <= NUM_SEGMENTS; i += 2)
            {
                int theta = i % NUM_SEGMENTS;
                v2 = transform.MultiplyPoint( new Vector3(m_sincos[theta].y * radius, m_sincos[theta].x * radius, h));
                v3 = transform.MultiplyPoint(new Vector3(m_sincos[theta].y * radius, m_sincos[theta].x * radius, -h));
                if (i != 0)
                {
                    renderLine3D(v0, v1, color);
                    renderLine3D(v1, v3, color);
                    renderLine3D(v3, v2, color);
                    renderLine3D(v2, v0, color);
                    renderQuad3D(v0, v1, v3, v2, color);
                }
                v0 = v2;
                v1 = v3;
            }
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderSolidCylinder))]
        public void renderSolidCylinder(float radius, float height, Matrix4x4  transform, Vector4 color)
        {
            float h = height / 2.0f;
            Vector3 v0 = transform.MultiplyPoint(new Vector3(0.0f, 0.0f, h));
            Vector3 v1 = transform.MultiplyPoint(new Vector3(0.0f, 0.0f, -h));
            for (int i = 0; i < NUM_SEGMENTS; i++)
            {
                float s0 = m_sincos[i].x * radius;
                float c0 = m_sincos[i].y * radius;
                float s1 = m_sincos[i + 1].x * radius;
                float c1 = m_sincos[i + 1].y * radius;
                Vector3 v2 = transform.MultiplyPoint(new Vector3(s0, c0, h));
                Vector3 v3 = transform.MultiplyPoint(new Vector3(s1, c1, h));
                Vector3 v4 = transform.MultiplyPoint(new Vector3(s0, c0, -h));
                Vector3 v5 = transform.MultiplyPoint(new Vector3(s1, c1, -h));
                renderLine3D(v2, v3, color);
                renderLine3D(v4, v5, color);
                renderTriangle3D(v0, v2, v3, color);
                renderTriangle3D(v1, v4, v5, color);
                renderQuad3D(v2, v3, v5, v4, color);
            }

            renderLine3D(transform.MultiplyPoint( new Vector3(-radius, 0.0f, -h)),transform.MultiplyPoint( new Vector3(radius, 0.0f, -h)), color);
            renderLine3D(transform.MultiplyPoint( new Vector3(0.0f, -radius, -h)),transform.MultiplyPoint( new Vector3(0.0f, radius, -h)), color);
            renderLine3D(transform.MultiplyPoint( new Vector3(-radius, 0.0f, h)), transform.MultiplyPoint(new Vector3(radius, 0.0f, h)), color);
            renderLine3D(transform.MultiplyPoint( new Vector3(0.0f, -radius, h)), transform.MultiplyPoint(new Vector3(0.0f, radius, h)), color);
            renderLine3D(transform.MultiplyPoint( new Vector3(radius, 0.0f, -h)), transform.MultiplyPoint(new Vector3(radius, 0.0f, h)), color);
            renderLine3D(transform.MultiplyPoint( new Vector3(-radius, 0.0f, -h)),transform.MultiplyPoint( new Vector3(-radius, 0.0f, h)), color);
            renderLine3D(transform.MultiplyPoint( new Vector3(0.0f, radius, -h)), transform.MultiplyPoint(new Vector3(0.0f, radius, h)), color);
            renderLine3D(transform.MultiplyPoint(new Vector3(0.0f, -radius, -h)), transform.MultiplyPoint(new Vector3(0.0f, -radius, h)), color);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderScissor))]
        public void renderScissor(float x, float y, float width, float height, Vector4 color)
        {
            float x0 = x;
            float x1 = x0 + width;
            float y0 = 1.0f - y;
            float y1 = y0 - height;
            renderLine2D(new Vector3(x0, y0, 0.0f),new Vector3(x1, y0, 0.0f), color);
            renderLine2D(new Vector3(x0, y1, 0.0f),new Vector3(x1, y1, 0.0f), color);
            renderLine2D(new Vector3(x0, y0, 0.0f),new Vector3(x0, y1, 0.0f), color);
            renderLine2D(new Vector3(x1, y0, 0.0f), new Vector3(x1, y1, 0.0f), color);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderBoundBox))]
        public void renderBoundBox( Vector3 bound_min, Vector3 bound_max, Matrix4x4  transform, Vector4 color)
        {
            Bounds bb = new Bounds();
            bb.SetMinMax(bound_min, bound_max);

            Vector3 center = (bound_min + bound_max) * 0.5f;
            Vector3 axis = bound_max - center;
            float x =Mathf.Abs(transform.m00) * axis.x +  Mathf.Abs(transform.m01) * axis.y + Mathf.Abs(transform.m02) * axis.z;
            float y = Mathf.Abs(transform.m10) * axis.x + Mathf.Abs(transform.m11) * axis.y + Mathf.Abs(transform.m12) * axis.z;
            float z = Mathf.Abs(transform.m20) * axis.x + Mathf.Abs(transform.m21) * axis.y + Mathf.Abs(transform.m22) * axis.z;
            center = transform.MultiplyPoint(center);
            bound_min = center - new Vector3(x, y, z);
            bound_max = center + new Vector3(x, y, z);

            renderBox( bound_max- bound_min, Matrix4x4.Translate((bound_max+ bound_min)/2), color);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(VISUALIZER_renderBoundSphere))]
        public void renderBoundSphere(Vector3 center, float radius, Matrix4x4  transform, Vector4 color)
        {
            if (radius <= 0) return;

            center = transform * center;
            float x = transform.m00 * transform.m00 + transform.m10 * transform.m10 + transform.m20 * transform.m20;
            float y = transform.m01 * transform.m01 + transform.m11 * transform.m11 + transform.m21 * transform.m21;
            float z = transform.m02 * transform.m02 + transform.m12 * transform.m12 + transform.m22 * transform.m22;
            float scale = Mathf.Sqrt(Mathf.Max(Mathf.Max(x, y), z));
            radius = radius * scale;

            renderSphere(radius, Matrix4x4.Translate(center), color);
        }
        //-------------------------------------------------
        void render_lines(List<Line> lines)
        {
            // copy vertices
            for (int i = 0; i < lines.Count; i++)
            {
                GL.Color(lines[i].color);
                GL.Vertex3(lines[i].xyz0.x, lines[i].xyz0.y, lines[i].xyz0.z);
                GL.Vertex3(lines[i].xyz1.x, lines[i].xyz1.y, lines[i].xyz1.z);
            }
        }
        //-------------------------------------------------
        void render_triangles(List<Triangle> triangles)
        {
            // copy vertices
            Base.SortUtility.QuickSortUp<Triangle>(ref triangles);
            for (int i = 0; i < triangles.Count; i++)
            {
                GL.Color(triangles[i].color);
                GL.Vertex3(triangles[i].xyz0.x, triangles[i].xyz0.y, triangles[i].xyz0.z);
                GL.Vertex3(triangles[i].xyz1.x, triangles[i].xyz1.y, triangles[i].xyz1.z);
                GL.Vertex3(triangles[i].xyz2.x, triangles[i].xyz2.y, triangles[i].xyz2.z);
            }
        }
    }
}