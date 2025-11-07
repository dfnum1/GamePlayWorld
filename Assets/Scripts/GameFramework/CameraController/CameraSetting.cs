/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	CameraSetting
作    者:	HappLI
描    述:	相机设置
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if USE_URP
using UnityEngine.Rendering.Universal;
#endif
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Core
{
    public class CameraSetting : MonoBehaviour
    {
#if USE_URP
        public Volume postProcessVolume;
#endif
        public Camera mainCamera = null;
    }
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraSetting), true)]
    public class CameraSettingEditor : Editor
    {
        protected float m_fTestKeyTime = 0;
        protected float m_fMaxTime = 10;
        protected float m_fCurTime = 0;
        protected float m_fRealMaxTime = 0;

     //   bool m_bLockLookat = false;
        Transform m_pLookAtTransform = null;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            CameraSetting cameraSetting = target as CameraSetting;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mainCamera"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("postProcessVolume"), true);
            CameraController controller = CameraController.getInstance();
            if (controller == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            CameraMode pMode = controller.GetCurrentMode();

            if (controller.IsEditorMode())
            {
                controller.EditorController.DrawSetting();
            }
            if (pMode != null)
            {
                if (controller.IsEditorMode())
                {
                    pMode.Update(0);
                    pMode.Blance();
                    pMode.ResetLockOffsets();
                    pMode.SetLookFocusScatter(Vector3.zero, 0, 0);
                }

                Vector3 finalPos = Vector3.zero;
                Vector3 finalEulerAngle = Vector3.zero;
                Vector3 lookatPos = Vector3.zero;
                Vector3 position_offset = Vector3.zero;
                if (pMode is BattleCameraMode)
                {
                    m_pLookAtTransform = EditorGUILayout.ObjectField("锁定目标", m_pLookAtTransform, typeof(Transform), true) as Transform;
                    if (m_pLookAtTransform != null)
                    {
                        lookatPos = BaseUtil.RayHitPos(controller.GetPosition(), controller.GetDir(), m_pLookAtTransform.position.y);
                        EditorGUILayout.BeginHorizontal();
                        position_offset = lookatPos - m_pLookAtTransform.position;
                        EditorGUILayout.Vector3Field("相机位置", controller.GetPosition());
                        if (GUILayout.Button("复制"))
                        {
                            TextEditor t = new TextEditor();
                            t.text = string.Format("{0:F2}|{1:F2}|{2:F2}", position_offset.x, position_offset.y, position_offset.z);
                            t.OnFocus();
                            t.Copy();
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        Vector3 eulerAngle = EditorGUILayout.Vector3Field("当前角度", controller.GetEulerAngle());
                        if (GUILayout.Button("复制"))
                        {
                            TextEditor t = new TextEditor();
                            t.text = string.Format("{0:F2}|{1:F2}|{2:F2}", eulerAngle.x, eulerAngle.y, eulerAngle.z);
                            t.OnFocus();
                            t.Copy();
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Vector3Field("当前视点偏移", position_offset);
                        if (GUILayout.Button("复制"))
                        {
                            TextEditor t = new TextEditor();
                            t.text = string.Format("{0:F2}|{1:F2}|{2:F2}", eulerAngle.x, eulerAngle.y, eulerAngle.z);
                            t.OnFocus();
                            t.Copy();
                        }
                        EditorGUILayout.EndHorizontal();

                        finalPos = m_pLookAtTransform.position + position_offset;
                        finalEulerAngle = eulerAngle;
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Vector3Field("相机位置", controller.GetPosition());
                        if (GUILayout.Button("复制"))
                        {
                            TextEditor t = new TextEditor();
                            t.text = string.Format("{0:F2}|{1:F2}|{2:F2}", position_offset.x, position_offset.y, position_offset.z);
                            t.OnFocus();
                            t.Copy();
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        Vector3 eulerAngle = EditorGUILayout.Vector3Field("当前角度", controller.GetEulerAngle());
                        if (GUILayout.Button("复制"))
                        {
                            TextEditor t = new TextEditor();
                            t.text = string.Format("{0:F2}|{1:F2}|{2:F2}", eulerAngle.x, eulerAngle.y, eulerAngle.z);
                            t.OnFocus();
                            t.Copy();
                        }
                        EditorGUILayout.EndHorizontal();

                        lookatPos = BaseUtil.RayHitPos(controller.GetPosition(), controller.GetDir(), 0);
                        position_offset = lookatPos - Vector3.zero;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Vector3Field("当前视点偏移", position_offset);
                        if (GUILayout.Button("复制"))
                        {
                            TextEditor t = new TextEditor();
                            t.text = string.Format("{0:F2}|{1:F2}|{2:F2}", eulerAngle.x, eulerAngle.y, eulerAngle.z);
                            t.OnFocus();
                            t.Copy();
                        }
                        EditorGUILayout.EndHorizontal();

                        finalPos = controller.GetPosition();
                        finalEulerAngle = controller.GetEulerAngle();
                    }
                    EditorGUILayout.BeginHorizontal();
                    float curDistance = EditorGUILayout.FloatField("当前视距", (lookatPos - controller.GetPosition()).magnitude);
                    if (GUILayout.Button("复制"))
                    {
                        TextEditor t = new TextEditor();
                        t.text = string.Format("{0:F2}", curDistance);
                        t.OnFocus();
                        t.Copy();
                    }
                    EditorGUILayout.EndHorizontal();

                    if (GUILayout.Button(new GUIContent("复制Excel格式", "对应场景表中的'位置\t角度\t当前视点偏移\t当前视距'列")))
                    {
                        TextEditor t = new TextEditor();
                        t.text = string.Format("{0:F2}|{1:F2}|{2:F2}\t{3:F2}|{4:F2}|{5:F2}\t{6:F2}|{7:F2}|{8:F2}\t{9:F2}", controller.GetPosition().x, controller.GetPosition().y, controller.GetPosition().z
                            , controller.GetEulerAngle().x, controller.GetEulerAngle().y, controller.GetEulerAngle().z
                            , position_offset.x, position_offset.y, position_offset.z, curDistance);
                        t.OnFocus();
                        t.Copy();
                    }
                }
                else 
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Vector3Field("相机位置", controller.GetPosition());
                    if (GUILayout.Button("复制"))
                    {
                        TextEditor t = new TextEditor();
                        t.text = string.Format("{0:F2}|{1:F2}|{2:F2}", position_offset.x, position_offset.y, position_offset.z);
                        t.OnFocus();
                        t.Copy();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    Vector3 eulerAngle = EditorGUILayout.Vector3Field("当前角度", controller.GetEulerAngle());
                    if (GUILayout.Button("复制"))
                    {
                        TextEditor t = new TextEditor();
                        t.text = string.Format("{0:F2}|{1:F2}|{2:F2}", eulerAngle.x, eulerAngle.y, eulerAngle.z);
                        t.OnFocus();
                        t.Copy();
                    }
                    EditorGUILayout.EndHorizontal();

                    lookatPos = BaseUtil.RayHitPos(controller.GetPosition(), controller.GetDir(), 0);
                    finalPos = controller.GetPosition();
                    finalEulerAngle = controller.GetEulerAngle();
                }
                if (GUILayout.Button("作用到当前相机"))
                {
                    if (pMode is BattleCameraMode battleMode)
                    {
                        Vector3 pos = finalPos;
                        Vector3 euler = finalEulerAngle;
                        ApplayBattleCamera(pos, m_pLookAtTransform.position, euler, Vector3.up, battleMode);
                    }
                    else
                    {
                        Vector3 pos = finalPos;
                        Vector3 euler = finalEulerAngle;
                        pMode.SetCurrentTrans(pos);
                        pMode.SetCurrentEulerAngle(finalEulerAngle);
                    }
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                Vector3 position_offset = EditorGUILayout.Vector3Field("相机位置", controller.GetPosition());
                if (GUILayout.Button("复制"))
                {
                    TextEditor t = new TextEditor();
                    t.text = string.Format("{0:F2}|{1:F2}|{2:F2}", position_offset.x, position_offset.y, position_offset.z);
                    t.OnFocus();
                    t.Copy();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                Vector3 eulerAngle = EditorGUILayout.Vector3Field("当前角度", controller.GetEulerAngle());
                if (GUILayout.Button("复制"))
                {
                    TextEditor t = new TextEditor();
                    t.text = string.Format("{0:F2}|{1:F2}|{2:F2}", eulerAngle.x, eulerAngle.y, eulerAngle.z);
                    t.OnFocus();
                    t.Copy();
                }
                EditorGUILayout.EndHorizontal();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        //------------------------------------------------------
        static Vector3 GetCurrentLookAt(Vector3 pos, Vector3 dir, float floorY)
        {
            Vector3 curLookAt = BaseUtil.RayHitPos(pos, dir, floorY);
            if (Vector3.Dot(dir, curLookAt - pos) < 0)
            {
                float dist = Vector3.Distance(curLookAt, pos);
                curLookAt = pos + (pos - curLookAt).normalized * dist;
            }
            return curLookAt;
        }
        //---------------------------------------
        public static void ApplayBattleCamera(Vector3 pos, Vector3 targetPos, Vector3 eulerAngle, Vector3 up, BattleCameraMode mode)
        {
            if (mode == null) return;

            Vector3 lookat = GetCurrentLookAt(pos, BaseUtil.EulersAngleToDirection(eulerAngle), targetPos.y);

            mode.ResetLockOffsets();
            mode.SetCurrentLookAtOffset(lookat - mode.GetFollowLookAtPosition());
            mode.SetCurrentTransOffset(Vector3.zero);
            mode.SetFollowDistance(Vector3.Distance(lookat, pos), true);
            mode.SetCurrentEulerAngle(eulerAngle);
            mode.Start();
        }
        //---------------------------------------
        void OnSceneGUI()
        {
            CameraSetting camreaSetting = target as CameraSetting;
            CameraController controller = CameraController.getInstance() as CameraController;
            if (controller == null) return;
            CameraMode pMode = controller.GetCurrentMode();
            if (pMode == null) return;

            Color color = Handles.color;
            Handles.color = Color.red;
            Vector3 lookat = GetCurrentLookAt(controller.GetPosition(), controller.GetDir(), pMode.GetFollowLookAtPosition().y);
            Handles.SphereHandleCap(0, lookat, Quaternion.identity, Mathf.Min(0.5f, HandleUtility.GetHandleSize(lookat)), EventType.Repaint);
            Handles.DrawLine(controller.GetPosition(), lookat);
            Handles.color = color;
        }
    }
#endif
}