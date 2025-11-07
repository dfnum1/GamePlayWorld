#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework.Base;

namespace Framework.UI.ED
{
    class UISplineEditor
    {
        UITargetBindTrackParameter m_Target = null;
        //------------------------------------------------------
        public void SetTarget(UITargetBindTrackParameter target)
        {
            m_Target = target;
        }
        //------------------------------------------------------
        public UITargetBindTrackParameter GetTarget()
        {
            return m_Target;
        }

        Vector2 m_Scroll = Vector2.zero;
        List<float> m_vValues = new List<float>();
        //-----------------------------------------------------
        void DrawDebug()
        {
            if (!Core.DeclareKit.HasDeclare(Core.eDeclareType.Pos, m_Target.dataDeclare) ||
                m_Target.datas.Count<=0) return;
            float fmax = 0;
            for(int i = 0; i < m_Target.datas.Count; ++i)
            {
                fmax = Mathf.Max(fmax, m_Target.datas[i].time);
            }
            float time = 0;
            Vector3 prePos = Vector3.zero;
            Color bak = UnityEditor.Handles.color;
            Core.DeclareKit.GetVector3(Core.eDeclareType.Pos, m_Target.dataDeclare, m_Target.datas[0].propertys, ref prePos);
            UnityEditor.Handles.color = Color.cyan;

            while (time < fmax)
            {
                Vector3 pos = Vector3.zero;
                Evaluate(time, m_vValues);

                Core.DeclareKit.GetVector3(Core.eDeclareType.Pos, m_Target.dataDeclare, m_vValues, ref pos);

                UnityEditor.Handles.DrawLine(prePos, pos);

                prePos = pos;
                time += 0.01f;
            }
            UnityEditor.Handles.color = bak;
        }
        //-----------------------------------------------------
        public void OnSceneGUI(SceneView sceneView)
        {
            if (m_Target == null || m_Target.datas == null) return;
            DrawDebug();
            Color color = Handles.color;

            UnityEngine.Event currentEvt = UnityEngine.Event.current;
            for (int i = 0; i < m_Target.datas.Count; ++i)
            {
                UITargetBindTrackParameter.SplineData frame = m_Target.datas[i];

                Vector3 position = Vector3.zero;
                bool bPos = m_Target.GetVector3(Core.eDeclareType.Pos, i, ref position);
                    if (currentEvt != null && currentEvt.shift && currentEvt.control)
                {
                    if(bPos)
                    {
                        Vector3 inTan = Vector3.zero;
                        if (m_Target.GetVector3(Core.eDeclareType.Euler, i, ref inTan))
                        {
                            inTan = Handles.DoPositionHandle(position + inTan, Quaternion.identity) - position;
                            m_Target.SetVector3(Core.eDeclareType.Euler, i, inTan);

                            Handles.color = Color.red;
                            Handles.SphereHandleCap(0, position + inTan, Quaternion.identity, 0.1f, EventType.Repaint);
                            Handles.color = color;
                        }
                        Vector3 outTan = Vector3.zero;
                        if (m_Target.GetVector3(Core.eDeclareType.Euler, i, ref outTan))
                        {
                            outTan = Handles.DoPositionHandle(position + outTan, Quaternion.identity) - position;
                            m_Target.SetVector3(Core.eDeclareType.Euler,i, outTan);
                            Handles.color = Color.red;
                            Handles.SphereHandleCap(0, position + outTan, Quaternion.identity, 0.1f, EventType.Repaint);
                            Handles.color = color;
                        }

                        Handles.SphereHandleCap(0, position, Quaternion.identity, 0.2f, EventType.Repaint);
                    }
                }
                else if (currentEvt != null && currentEvt.shift)
                {
                    if (Tools.current == Tool.Move)
                    {
                        if (bPos)
                        {
                            position = Handles.DoPositionHandle(position, Quaternion.identity);
                            m_Target.SetVector3(Core.eDeclareType.Pos, i, position);
                        }
                    }
                }

                Handles.Label(position, "P[" + i.ToString() + "]");
            }

            if (currentEvt != null && currentEvt.shift)
            {
                for (int i = 0; i < m_Target.datas.Count; ++i)
                {
                    Vector3 position = Vector3.zero;
                    Core.DeclareKit.GetVector3(Core.eDeclareType.Pos, m_Target.dataDeclare, m_Target.datas[i].propertys, ref position);
                    Vector3 prevPos = Vector3.zero;
                    Vector3 nextPos = Vector3.zero;
                    Core.DeclareKit.GetVector3(Core.eDeclareType.Pos, m_Target.dataDeclare, m_Target.datas[i].propertys, ref prevPos);
                    Core.DeclareKit.GetVector3(Core.eDeclareType.Pos, m_Target.dataDeclare, m_Target.datas[i].propertys, ref nextPos);
                    Vector3 retPos = Vector3.zero;
                    if (i - 1 >= 0 && Evaluate((m_Target.datas[i].time+ m_Target.datas[i-1].time)/2, m_vValues))
                    {
                        Core.DeclareKit.GetVector3(Core.eDeclareType.Pos, m_Target.dataDeclare, m_vValues, ref prevPos);
                    }
                    if (i + 1 < m_Target.datas.Count && Evaluate((m_Target.datas[i].time + m_Target.datas[i + 1].time) / 2, m_vValues))
                    {
                        Core.DeclareKit.GetVector3(Core.eDeclareType.Pos, m_Target.dataDeclare, m_vValues, ref nextPos);
                    }

                    Vector2 curuiPos = HandleUtility.WorldToGUIPoint(position);
                    GUILayout.BeginArea(new Rect(curuiPos.x, curuiPos.y, 40, 25));
                    if (GUILayout.Button("删除"))
                    {
                        if (EditorUtility.DisplayDialog("提示", "删除这一帧", "删除", "取消"))
                        {
                            m_Target.datas.RemoveAt(i);
                            break;
                        }
                    }
                    GUILayout.EndArea();

                    if (i - 1 >= 0)
                    {
                        Vector2 position2 = HandleUtility.WorldToGUIPoint(prevPos);
                        GUILayout.BeginArea(new Rect(position2.x, position2.y, 40, 25));
                        if (GUILayout.Button("插入"))
                        {
                            float insert = (m_Target.datas[i].time + m_Target.datas[i-1].time) / 2;
                            if (Evaluate(insert, m_vValues))
                            {
                                UITargetBindTrackParameter.SplineData newframe = new UITargetBindTrackParameter.SplineData();
                                newframe.time = insert;
                                newframe.propertys = new List<float>(m_vValues.ToArray());
                                break;
                            }
                        }
                        GUILayout.EndArea();
                    }
                    if (i + 1 < m_Target.datas.Count)
                    {
                        float time = (m_Target.datas[i].time + m_Target.datas[i +1].time) / 2;
                        Vector2 position2 = HandleUtility.WorldToGUIPoint(nextPos);
                        GUILayout.BeginArea(new Rect(position2.x, position2.y, 40, 25));
                        if (GUILayout.Button("插入"))
                        {
                            float insert = (m_Target.datas[i].time + m_Target.datas[i + 1].time) / 2;
                            if (Evaluate(insert, m_vValues))
                            {
                                UITargetBindTrackParameter.SplineData newframe = new UITargetBindTrackParameter.SplineData();
                                newframe.time = insert;
                                newframe.propertys = new List<float>(m_vValues.ToArray());
                                break;
                            }
                        }
                        GUILayout.EndArea();
                    }

                    if (currentEvt != null && currentEvt.control)
                    {
                        Vector2 position2 = HandleUtility.WorldToGUIPoint(position);
                        GUILayout.BeginArea(new Rect(position2.x, position2.y + 25, 100, 50));
                        if (GUILayout.Button("目标同步帧"))
                        {
                            SetObjectToProperty(m_Target.dataDeclare, m_Target.datas[i].propertys);
                        }
                        if (GUILayout.Button("帧同步目标"))
                        {
                            SetPropertyToObject(m_Target.dataDeclare, m_Target.datas[i].propertys);
                        }
                        GUILayout.EndArea();
                    }
                }
            }
        }
        //------------------------------------------------------
        public void SetPropertyToObject(int declare, List<float> propertys)
        {

            if (m_Target == null || m_Target.pEditController == null) return;
            Transform transform = m_Target.GetTransform();
            if (transform)
            {
                Vector3 vec3 = Vector3.zero;
                if (Core.DeclareKit.GetVector3(Core.eDeclareType.Pos, declare, propertys, ref vec3))
                    transform.position = vec3;
                if (Core.DeclareKit.GetVector3(Core.eDeclareType.Euler, declare, propertys, ref vec3))
                    transform.eulerAngles = vec3;
                if (Core.DeclareKit.GetVector3(Core.eDeclareType.Scale, declare, propertys, ref vec3))
                    transform.localScale = vec3;

                if (Core.DeclareKit.GetVector3(Core.eDeclareType.LookAt, declare, propertys, ref vec3))
                    transform.LookAt(vec3);
            }
            Camera camera = m_Target.GetCamera();
            if (camera)
            {
                float fvalue = 0;
                if (Core.DeclareKit.GetFloat(Core.eDeclareType.Fov, declare, propertys, ref fvalue))
                    camera.fieldOfView = fvalue;
            }
        }
        //------------------------------------------------------
        public void SetObjectToProperty(int declare, List<float> propertys)
        {
            if (m_Target.pEditController == null) return;
            Transform transform = m_Target.GetTransform();
            if (transform)
            {
                Core.DeclareKit.SetVector3(Core.eDeclareType.Pos, declare, propertys, transform.position);
                Core.DeclareKit.SetVector3(Core.eDeclareType.Euler, declare, propertys, transform.eulerAngles);
                Core.DeclareKit.SetVector3(Core.eDeclareType.Scale, declare, propertys, transform.localScale);
            }
            Camera camera = m_Target.GetCamera();
            if (camera)
            {
                Core.DeclareKit.SetFloat(Core.eDeclareType.Fov, declare, propertys, camera.fieldOfView);
            }
        }
        //------------------------------------------------------
        bool Evaluate(float fTime, List<float> retPropertys)
        {
            retPropertys.Clear();
            if (m_Target.datas == null) return false;
            int size = Core.DeclareKit.CalcTotalSize(m_Target.dataDeclare);
            if (size <= 0) return false;
            for (int i = 0; i < size; ++i)
                retPropertys.Add(0);

            if (fTime <= m_Target.datas[0].time)
            {
                retPropertys = m_Target.datas[0].propertys;
                return true;
            }
            if (fTime >= m_Target.datas[m_Target.datas.Count - 1].time)
            {
                retPropertys = m_Target.datas[0].propertys;
                return true;
            }

            int __len = m_Target.datas.Count;
            int __half;
            int __middle;
            int __first = 0;
            while (__len > 0)
            {
                __half = __len >> 1;
                __middle = __first + __half;

                if (fTime < m_Target.datas[__middle].time)
                    __len = __half;
                else
                {
                    __first = __middle;
                    ++__first;
                    __len = __len - __half - 1;
                }
            }

            int lhs = __first - 1;
            int rhs = Mathf.Min(m_Target.datas.Count - 1, __first);

            if (lhs < 0 || lhs >= m_Target.datas.Count || rhs < 0 || rhs >= m_Target.datas.Count)
                return false;

            UITargetBindTrackParameter.SplineData lhsKey = m_Target.datas[lhs];
            UITargetBindTrackParameter.SplineData rhsKey = m_Target.datas[rhs];

            float dx = rhsKey.time - lhsKey.time;
            float t;
            if (dx != 0f)
            {
                t = (fTime - lhsKey.time) / dx;
            }
            else
                t = 0;

            int index = Core.DeclareKit.CalcBoundIndex(Core.eDeclareType.Pos, m_Target.dataDeclare);
            if(index>=0 && index+2< lhsKey.propertys.Count)
            {
                Vector3 lpos = new Vector3(lhsKey.propertys[index], lhsKey.propertys[index+1], lhsKey.propertys[index+2]);
                Vector3 rpos = new Vector3(rhsKey.propertys[index], rhsKey.propertys[index + 1], rhsKey.propertys[index + 2]);

                Vector3 inTan = Vector3.zero;
                Vector3 outTan = Vector3.zero;
                int inTanIndex = Core.DeclareKit.CalcBoundIndex(Core.eDeclareType.InTag, m_Target.dataDeclare);
                if(inTanIndex >= 0 && inTanIndex + 2< rhsKey.propertys.Count)
                {
                    inTan = new Vector3(rhsKey.propertys[inTanIndex], rhsKey.propertys[inTanIndex + 1], rhsKey.propertys[inTanIndex + 2]);
                    retPropertys[inTanIndex] = inTan.x;
                    retPropertys[inTanIndex + 1] = inTan.y;
                    retPropertys[inTanIndex + 2] = inTan.z;
                }
                int outTanIndex = Core.DeclareKit.CalcBoundIndex(Core.eDeclareType.OutTag, m_Target.dataDeclare);
                if (outTanIndex >= 0 && outTanIndex + 2 < rhsKey.propertys.Count)
                {
                    outTan = new Vector3(lhsKey.propertys[outTanIndex], lhsKey.propertys[outTanIndex + 1], lhsKey.propertys[outTanIndex + 2]);
                    retPropertys[outTanIndex] = outTan.x;
                    retPropertys[outTanIndex+1] = outTan.y;
                    retPropertys[outTanIndex+2] = outTan.z;
                }
                Vector3 retPos = BezierUtility.Bezier4(t, lpos, lpos+ outTan, rpos+ inTan, rpos);
                retPropertys[index] = retPos.x;
                retPropertys[index + 1] = retPos.y;
                retPropertys[index + 2] = retPos.z;
            }
            index = Core.DeclareKit.CalcBoundIndex(Core.eDeclareType.Euler, m_Target.dataDeclare);
            if (index >= 0 && index + 2 < lhsKey.propertys.Count)
            {
                Vector3 leuler = new Vector3(lhsKey.propertys[index], lhsKey.propertys[index + 1], lhsKey.propertys[index + 2]);
                Vector3 reuler = new Vector3(rhsKey.propertys[index], rhsKey.propertys[index + 1], rhsKey.propertys[index + 2]);
                Vector3 retEuler =Vector3.Lerp(leuler, reuler, t);
                retPropertys[index] = retEuler.x;
                retPropertys[index + 1] = retEuler.y;
                retPropertys[index + 2] = retEuler.z;
            }
            index = Core.DeclareKit.CalcBoundIndex(Core.eDeclareType.LookAt, m_Target.dataDeclare);
            if (index >= 0 && index + 2 < lhsKey.propertys.Count)
            {
                Vector3 leuler = new Vector3(lhsKey.propertys[index], lhsKey.propertys[index + 1], lhsKey.propertys[index + 2]);
                Vector3 reuler = new Vector3(rhsKey.propertys[index], rhsKey.propertys[index + 1], rhsKey.propertys[index + 2]);
                Vector3 retEuler = Vector3.Lerp(leuler, reuler, t);
                retPropertys[index] = retEuler.x;
                retPropertys[index + 1] = retEuler.y;
                retPropertys[index + 2] = retEuler.z;
            }
            index = Core.DeclareKit.CalcBoundIndex(Core.eDeclareType.Scale, m_Target.dataDeclare);
            if (index >= 0 && index + 2 < lhsKey.propertys.Count)
            {
                Vector3 leuler = new Vector3(lhsKey.propertys[index], lhsKey.propertys[index + 1], lhsKey.propertys[index + 2]);
                Vector3 reuler = new Vector3(rhsKey.propertys[index], rhsKey.propertys[index + 1], rhsKey.propertys[index + 2]);
                Vector3 retEuler = Vector3.Lerp(leuler, reuler, t);
                retPropertys[index] = retEuler.x;
                retPropertys[index + 1] = retEuler.y;
                retPropertys[index + 2] = retEuler.z;
            }
            index = Core.DeclareKit.CalcBoundIndex(Core.eDeclareType.Fov, m_Target.dataDeclare);
            if (index >= 0 && index < lhsKey.propertys.Count)
            {
                retPropertys[index] = Mathf.Lerp(lhsKey.propertys[index], rhsKey.propertys[index], t);
            }
            return true;
        }
        //------------------------------------------------------
        public void Update(float fTime)
        {
            if (m_Target == null) return;

            if (Evaluate(fTime, m_vValues))
            {
                SetPropertyToObject(m_Target.dataDeclare, m_vValues);
            }
        }
    }
}
#endif
