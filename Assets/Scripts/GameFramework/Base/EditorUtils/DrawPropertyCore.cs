#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Framework.Core;

namespace Framework.ED
{
	public class DrawPropertyCore
	{
        public enum EDrawType
        {
            None,
            Remove,
            Edit,
        }
        static SplineData m_pSplineTracker = new SplineData();
        static bool m_bCopyParameter = false;
        static PhysicPropertyData m_pCopyParameter;

        static bool m_bEditorPropertyData = false;
        static PhysicPropertyData m_pEditorPropertyData;
        //------------------------------------------------------
        public static EDrawType Draw(ref PhysicPropertyData property, string strLabel, bool bPopDel = false, HashSet<string> vIngores = null)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            property.bExpand = EditorGUILayout.Foldout(property.bExpand, strLabel);
            if (bPopDel && GUILayout.Button("Del", new GUILayoutOption[] { GUILayout.Width(30) }))
            {
                if (EditorUtility.DisplayDialog("提示", "确定删除 ? ", "确定", "取消"))
                {
                    GUILayout.EndVertical();
                    return EDrawType.Remove;
                }
            }
            EDrawType drawType = EDrawType.None;
            if (GUILayout.Button("Copy", new GUILayoutOption[] { GUILayout.Width(50) }))
            {
                m_pCopyParameter = property;
                m_bCopyParameter = true;
            }
            if (m_bCopyParameter && GUILayout.Button("Parse", new GUILayoutOption[] { GUILayout.Width(50) }))
            {
                property.Copy(m_pCopyParameter);
                m_bCopyParameter = false;
            }
            if (property.bUseFrame && GUILayout.Button("编辑", new GUILayoutOption[] { GUILayout.Width(50) }))
            {
                m_bEditorPropertyData = true;
                m_pEditorPropertyData = property;
                drawType = EDrawType.Edit;
            }
            GUILayout.EndHorizontal();
            if (property.bExpand)
            {
#if UNITY_EDITOR
                if(vIngores == null || !vIngores.Contains("strName"))
                    property.strName = EditorGUILayout.TextField("说明", property.strName);
#endif
                if (vIngores == null || !vIngores.Contains("fTriggerTime"))
                    property.fTriggerTime = EditorGUILayout.FloatField("触发时间", property.fTriggerTime);
                //        EditorGUI.indentLevel++;
                if (vIngores == null || !vIngores.Contains("bUseFrame"))
                    property.bUseFrame = EditorGUILayout.Toggle("启用曲线路径", property.bUseFrame);
                if (vIngores == null || !vIngores.Contains("bUseEnd"))
                    property.bUseEnd = EditorGUILayout.Toggle("使用结束位置信息", property.bUseEnd);
                if (vIngores == null || !vIngores.Contains("propertyFlags"))
                    property = (PhysicPropertyData)ED.InspectorDrawUtil.DrawPropertyByFieldName(property, "propertyFlags");
                if (property.bUseFrame)
                {
                    if (property.Frames != null)
                    {
                        EditorGUI.indentLevel++;
                        for (int f = 0; f < property.Frames.Length; ++f)
                        {
                            SplineData.KeyFrame frame = property.Frames[f];
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("帧[" + (f + 1).ToString() + "]");
                            if (GUILayout.Button("-", new GUILayoutOption[] { GUILayout.Width(20) }))
                            {
                                if (EditorUtility.DisplayDialog("提示", "确定删除 ? ", "确定", "取消"))
                                {
                                    List<SplineData.KeyFrame> vFrams = new List<SplineData.KeyFrame>(property.Frames);
                                    vFrams.RemoveAt(f);
                                    property.Frames = vFrams.ToArray();
                                    break;
                                }
                            }
                            GUILayout.EndHorizontal();
                            frame.time = EditorGUILayout.FloatField("Time", frame.time);
                            frame.position = EditorGUILayout.Vector3Field("pos", frame.position);
                            frame.eulerAngle = EditorGUILayout.Vector3Field("euler", frame.eulerAngle);
                            property.Frames[f] = frame;
                        }
                        EditorGUI.indentLevel--;
                    }
                    if (GUILayout.Button("新增"))
                    {
                        List<SplineData.KeyFrame> vFrams = (property.Frames != null) ? new List<SplineData.KeyFrame>(property.Frames) : new List<SplineData.KeyFrame>();
                        float fLastTime = 0;
                        Vector3 lastPos = Vector3.zero;
                        if (vFrams.Count > 0)
                        {
                            fLastTime = vFrams[vFrams.Count - 1].time + 1;
                            lastPos = vFrams[vFrams.Count - 1].position;
                        }

                        vFrams.Add(new SplineData.KeyFrame { time = fLastTime, position = lastPos + Vector3.forward * 5 });
                        property.Frames = vFrams.ToArray();
                        m_pEditorPropertyData = property;
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    property.physic.bUseHorSpeed = EditorGUILayout.Toggle("X向速度", property.physic.bUseHorSpeed);
                    if (property.physic.bUseHorSpeed)
                    {
                        GUILayout.BeginHorizontal();
                        property.physic.fHorSpeed = EditorGUILayout.FloatField(property.physic.fHorSpeed);
                        GUILayout.Label("-", new GUILayoutOption[] { GUILayout.Width(8) });
                        property.physic.fToHorSpeed = EditorGUILayout.FloatField(property.physic.fToHorSpeed);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    property.physic.bUseVerSpeed = EditorGUILayout.Toggle("Y向速度", property.physic.bUseVerSpeed);
                    if (property.physic.bUseVerSpeed)
                    {
                        GUILayout.BeginHorizontal();
                        property.physic.fVerSpeed = EditorGUILayout.FloatField(property.physic.fVerSpeed);
                        GUILayout.Label("-", new GUILayoutOption[] { GUILayout.Width(8) });
                        property.physic.fToVerSpeed = EditorGUILayout.FloatField(property.physic.fToVerSpeed);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    property.physic.bUseDeepSpeed = EditorGUILayout.Toggle("Z向速度", property.physic.bUseDeepSpeed);
                    if (property.physic.bUseDeepSpeed)
                    {
                        GUILayout.BeginHorizontal();
                        property.physic.fDeepSpeed = EditorGUILayout.FloatField(property.physic.fDeepSpeed);
                        GUILayout.Label("-", new GUILayoutOption[] { GUILayout.Width(8) });
                        property.physic.fToDeepSpeed = EditorGUILayout.FloatField(property.physic.fToDeepSpeed);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    property.physic.bUseGravity = EditorGUILayout.Toggle("重力", property.physic.bUseGravity);
                    if (property.physic.bUseGravity)
                    {
                        GUILayout.BeginHorizontal();
                        property.physic.fGravity = EditorGUILayout.FloatField(property.physic.fGravity);
                        GUILayout.Label("-", new GUILayoutOption[] { GUILayout.Width(8) });
                        property.physic.fToGravity = EditorGUILayout.FloatField(property.physic.fToGravity);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    property.physic.bUseFraction = EditorGUILayout.Toggle("摩擦力", property.physic.bUseFraction);
                    if (property.physic.bUseFraction)
                    {
                        GUILayout.BeginHorizontal();
                        property.physic.fFraction = EditorGUILayout.FloatField(property.physic.fFraction);
                        GUILayout.Label("-", new GUILayoutOption[] { GUILayout.Width(8) });
                        property.physic.fToFraction = EditorGUILayout.FloatField(property.physic.fToFraction);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndHorizontal();
                }
                //         EditorGUI.indentLevel--;
            }
            GUILayout.EndVertical();
            return drawType;
        }
        //------------------------------------------------------
        public static void OnSceneGUI(Actor pActor, int control, Camera camera)
        {
            if (!m_bEditorPropertyData) return;
            if (!m_pEditorPropertyData.bUseFrame) return;
            if (m_pEditorPropertyData.Frames == null) return;
            Color back = Handles.color;
            float fMaxTime = 0;
            for(int i = 0; i < m_pEditorPropertyData.Frames.Length; ++i)
            {
                Handles.color = back;
                SplineData.KeyFrame frame = m_pEditorPropertyData.Frames[i];

                if (Tools.current == Tool.Rotate)
                    frame.eulerAngle = Handles.RotationHandle(Quaternion.Euler(frame.eulerAngle), frame.position).eulerAngles;
                else
                {
                    if(Event.current.shift)
                    {
                        frame.inTan = Handles.PositionHandle(frame.position + frame.inTan, Quaternion.identity) - frame.position;
                        frame.outTan = Handles.PositionHandle(frame.position + frame.outTan, Quaternion.identity) - frame.position;
                    }
                    else
                        frame.position = Handles.PositionHandle(frame.position, Quaternion.identity);
                }

                Handles.ArrowHandleCap(control, frame.position, Quaternion.Euler(frame.eulerAngle), 1, EventType.Repaint);

                Handles.color = Color.cyan;
                Handles.SphereHandleCap(control, frame.position + frame.inTan, Quaternion.identity, 0.5f,EventType.Repaint);
                Handles.ArrowHandleCap(control, frame.position, Quaternion.Euler(BaseUtil.DirectionToEulersAngle(frame.inTan.normalized)) , frame.inTan.magnitude, EventType.Repaint);
                Handles.color = Color.grey;
                Handles.SphereHandleCap(control, frame.position + frame.outTan, Quaternion.identity, 0.5f, EventType.Repaint);
                Handles.ArrowHandleCap(control, frame.position, Quaternion.Euler(BaseUtil.DirectionToEulersAngle(frame.outTan.normalized)), frame.outTan.magnitude, EventType.Repaint);
                Handles.color = back;

                m_pEditorPropertyData.Frames[i] = frame;

                fMaxTime = Mathf.Max(fMaxTime, frame.time);
            }
            m_pSplineTracker.SetFrames(m_pEditorPropertyData.Frames);
            DrawTest(m_pEditorPropertyData.Frames[0].position, m_pEditorPropertyData.Frames[m_pEditorPropertyData.Frames.Length-1].position, fMaxTime);
        }
        //------------------------------------------------------
        static void DrawTest( Vector3 startPos, Vector3 endPos, float maxTime)
        {
            if (maxTime<=0) return;
            Color bak = Handles.color;
            Vector3 prePos = startPos;
            float time = 0f;
            while (time < maxTime)
            {
                Vector3 pos = Vector3.zero, rot = Vector3.zero;
                Vector3 euler = Vector3.zero;
                Vector3 scale = Vector3.one;
                m_pSplineTracker.Evaluate(time, ref pos, ref euler, ref scale);

                UnityEditor.Handles.color = Color.yellow;
                UnityEditor.Handles.DrawLine(prePos, pos);
                UnityEditor.Handles.color = bak;

                prePos = pos;
                time += 0.01f;
            }
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawLine(prePos, endPos);
            UnityEditor.Handles.color = bak;
        }
    }
}
#endif