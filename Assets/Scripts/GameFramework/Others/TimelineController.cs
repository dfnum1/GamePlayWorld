/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	TimelineController
作    者:	HappLI
描    述:	
*********************************************************************/
using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
#if USE_URP
using UnityEngine.Rendering.Universal;
#endif
using static UnityEngine.GraphicsBuffer;
using System.Reflection;
using UnityEditor;

namespace Framework.Core
{
    public class TimelineController : AInstanceAble
    {
#if USE_TIMELINE
        public bool autoDestroy = false;
        public Camera mainCamera;
        public bool syncGameCamera = true;
        public Transform follow;
        public Transform[] slots;

        public bool holdEndCheck = false;
        public bool overUseCamera = false;
        public float lerpToGameCamera = 0;
        public bool urpCameraTrack = true;
        public bool useRenderCurve = false;
        public PlayableDirector playableDirector;

     //   public Cinemachine.CinemachineBrain cinemachine;

        public bool gameGlobalVolumeActive = false;
        public UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset urpAsset;
        public UnityEngine.Rendering.VolumeProfile volumeProfile;

        private IUserData m_CallbackVariable = null;
        protected System.Action<bool, IUserData> m_Callback = null;
        private bool m_bDealCameraActive = true;
        private bool m_bSyncCameraStatus = false;
        private bool m_bHoldModePlayEnd = false;
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearGameCamera();
        }
        //------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();
            m_bSyncCameraStatus = false;
            if (playableDirector)
            {
                playableDirector.stopped += OnStopTimeline;
                playableDirector.played += OnPlayTimeline;
            }
        }
        //------------------------------------------------------
        protected override void OnEnable()
        {
         //   CameraUtil.OnCameraEnable += OnCameraEnable;
            if(playableDirector !=null && playableDirector.playOnAwake)
                SyncGameCamera();
            base.OnEnable();
        }
        //------------------------------------------------------
        protected override void OnDisable()
        {
        //    CameraUtil.OnCameraEnable -= OnCameraEnable;
            ClearGameCamera();
            base.OnDisable();
        }
        //------------------------------------------------------
        protected override void LateUpdate()
        {
            base.LateUpdate();
            if(!m_bHoldModePlayEnd)
            {
                if (holdEndCheck && playableDirector != null && playableDirector.extrapolationMode == DirectorWrapMode.Hold)
                {
                    if (playableDirector.time + 0.1f >= playableDirector.duration)
                    {
                        m_bHoldModePlayEnd = true;
                        ClearGameCamera();
                    }
                }
            }
        }
        //------------------------------------------------------
        void OnCameraEnable(bool bEnable)
        {
            if (bEnable) m_bDealCameraActive = false;
        }
        //------------------------------------------------------
        public override void OnPoolStart()
        {
            base.OnPoolStart();
            m_bHoldModePlayEnd = false;
            if (playableDirector && playableDirector.playOnAwake)
            {
                SyncGameCamera();
                playableDirector.Play();
            }
        }
        //------------------------------------------------------
        public override void OnRecyle()
        {
            m_bHoldModePlayEnd = false;
            RestoreTracks();
            ClearGameCamera();
            base.OnRecyle();
        }
        //------------------------------------------------------
        public bool Play(System.Action<bool,IUserData> OnCallback = null, IUserData useData = null)
        {
            if (playableDirector)
            {
                if (OnCallback != null)
                {
                    m_CallbackVariable = useData;
                    m_Callback += OnCallback;
                }
                playableDirector.Play();
                m_bHoldModePlayEnd = false;
                return true;
            }
            return false;
        }
        //------------------------------------------------------
        public void Stop()
        {
            if (playableDirector)
            {
                playableDirector.Stop();
                m_bHoldModePlayEnd = true;
                ClearGameCamera();
            }
            m_CallbackVariable = null;
            m_Callback = null;
        }
        //------------------------------------------------------
        void OnPlayTimeline(PlayableDirector playAble)
        {
            if (playableDirector == playAble)
            {
                if (m_Callback != null) m_Callback(true, m_CallbackVariable);
                SyncGameCamera();
                m_bHoldModePlayEnd = playableDirector.time+0.1f >= playableDirector.duration;
            }
        }
        //------------------------------------------------------
        void OnStopTimeline(PlayableDirector playAble)
        {
            if (playableDirector == playAble)
            {
#if UNITY_EDITOR
                if(autoDestroy)
                {
                    if(AFramework.isStartup)
                        RecyleDestroy(1);
                }
#else
                if(autoDestroy)
                {
                    RecyleDestroy(1);
                }
#endif
            }
            if (m_Callback != null)
                m_Callback(false, m_CallbackVariable);
            m_Callback = null;
            m_CallbackVariable = null;
        }
        //------------------------------------------------------
        void SyncGameCamera()
        {
            if (m_bSyncCameraStatus) return;
            m_bSyncCameraStatus = true;
            UseProfile();
            m_bDealCameraActive = true;
        }
        //------------------------------------------------------
        void ClearGameCamera()
        {
            if (!m_bSyncCameraStatus) return;
            m_bSyncCameraStatus = false;
            RestoreProfile();

            if (syncGameCamera)
            {
            }
            if (mainCamera)
            {
#if USE_URP
                UnityEngine.Rendering.Universal.UniversalAdditionalCameraData cameraDataStack = mainCamera.GetUniversalAdditionalCameraData();
                if (AFramework.mainFramework != null && cameraDataStack != null)
                {
                    if (cameraDataStack.renderType == CameraRenderType.Base)
                    {
                        //if(urpCameraTrack)
                        //{
                        //    List<Camera> vStacks = cameraDataStack.cameraStack;
                        //    UIBaseFramework pUI = UI.UIKits.GetUIFramework();
                        //    if (pUI != null && pUI.GetUICamera() != null)
                        //    {
                        //        vStacks.Remove(pUI.GetUICamera());
                        //    }
                        //}
 
                        if(m_bDealCameraActive)
                        {
                            CameraUtil.ActiveRoot(true);
                            CameraUtil.ActiveVolume(!gameGlobalVolumeActive);
                        }

                    }
                    else
                        CameraUtil.RemoveCameraStack(mainCamera);
                }
#endif
                if (overUseCamera)
                {
                    CameraMode curCameraMode = CameraUtil.GetCurrentMode();
                    if(curCameraMode!=null)
                    {
                        Transform cameraTran = mainCamera.transform;
                        Vector3 curLookAt = BaseUtil.RayHitPos(cameraTran.position, cameraTran.forward, 0);
                        if (Vector3.Dot(cameraTran.forward, curLookAt - cameraTran.position) < 0)
                        {
                            float dist = Vector3.Distance(curLookAt, cameraTran.position);
                            curLookAt = cameraTran.position + (cameraTran.position - curLookAt).normalized * dist;
                        }

                        curCameraMode.Reset();
                        curCameraMode.SetLockCameraLookAtOffset(Vector3.zero);
                        curCameraMode.SetCurrentTransOffset(Vector3.zero);
                        curCameraMode.SetCurrentEulerAngle(mainCamera.transform.eulerAngles, false);
                        curCameraMode.AppendFollowDistance(Vector3.Distance(curLookAt, cameraTran.position),true, false);
                        curCameraMode.SetCurrentFov(mainCamera.fieldOfView);
                        curCameraMode.Start();
                    }
                }
            }

            RestoreTracks();

            if (m_Callback != null)
                m_Callback(false, m_CallbackVariable);
            m_Callback = null;
            m_CallbackVariable = null;
        }
        //------------------------------------------------------
        public void UseProfile()
        {
#if USE_URP
            if (volumeProfile || urpAsset)
            {
                if (volumeProfile) CameraUtil.SetPostProcess(volumeProfile);
                if (urpAsset) CameraUtil.SetURPAsset(urpAsset);
            }
#endif
        }
        //------------------------------------------------------
        public void RestoreProfile()
        {
#if USE_URP
            //if (volumeProfile || urpAsset)
            //{
            //    ICameraController ctl = CameraKit.cameraController;
            //    if (ctl != null)
            //    {
            //        if(!gameGlobalVolumeActive)
            //            if (volumeProfile) ctl.SetPostProcess(Data.GameQuality.volumeProfile);
            //        if (urpAsset) ctl.SetURPAsset(Data.GameQuality.urpAsset);
            //    }
            //}
#endif
        }
        //------------------------------------------------------
        public Transform GetBindSlot(string slot)
        {
            if (string.IsNullOrEmpty(slot) || slot == null) return null;
            for(int i =0; i < slots.Length; ++i)
            {
                if (slots[i] && slots[i].name.CompareTo(slot) == 0) return slots[i];
            }
            return null;
        }
        //------------------------------------------------------
        void RestoreTracks()
        {
            //if (m_TackSlots != null && playableDirector)
            //{
            //    PlayableBindSlot bindSlot;
            //    for (int i = 0; i < m_TackSlots.Count; ++i)
            //    {
            //        bindSlot = m_TackSlots[i];
            //        bindSlot.pUserAT = null;
            //        bindSlot.pAble = null;
            //        m_TackSlots[i] = bindSlot;
            //        if (bindSlot.binding.sourceObject == null) continue;
            //        playableDirector.SetGenericBinding(bindSlot.binding.sourceObject, bindSlot.source);
            //    }
            //}
        }
        //------------------------------------------------------
        void InitTracks()
        {
            //if (playableDirector == null) return;
            //if (slots == null || slots.Length <= 0) return;
            //if (m_TackSlots == null)
            //{
            //    m_TackSlots = new List<PlayableBindSlot>(slots.Length);
            //    for (int i = 0; i < slots.Length; ++i)
            //    {
            //        if (slots[i] == null) continue;
            //        PlayableBindSlot bindSlot = new PlayableBindSlot();
            //        bindSlot.pSlot = slots[i];
            //        bindSlot.strName = slots[i].name;
            //        bindSlot.pAble = null;
            //        bindSlot.pUserAT = null;
            //        bindSlot.source = null;
            //        bindSlot.bGenericBinding = false;
            //        m_TackSlots.Add(bindSlot);
            //    }
            //    foreach (var bind in playableDirector.playableAsset.outputs)
            //    {
            //        if (bind.sourceObject == null) continue;
            //        UnityEngine.Object source = playableDirector.GetGenericBinding(bind.sourceObject);
            //        if (source == null) continue;
            //        for (int i = 0; i < m_TackSlots.Count; ++i)
            //        {
            //            if (m_TackSlots[i].strName.CompareTo(source.name) == 0)
            //            {
            //                PlayableBindSlot bindSlot = m_TackSlots[i];
            //                bindSlot.bGenericBinding = false;
            //                bindSlot.binding = bind;
            //                bindSlot.source = source;
            //                if (bind.sourceObject is Timeline.IUserTrackAsset)
            //                {
            //                    bindSlot.playableAsset = bind.sourceObject as Timeline.IUserTrackAsset;
            //                    bindSlot.playableAsset.Reset(playableDirector);
            //                }
            //                m_TackSlots[i] = bindSlot;
            //            }
            //        }
            //    }
            //}
        }
        //------------------------------------------------------
        public void BindTrackObject(string trackName, AInstanceAble pObject,IUserData pUserData, bool bGenericBinding = true)
        {
            //if (playableDirector == null) return;
            //InitTracks();
            //if (m_TackSlots == null || m_TackSlots.Count<=0) return;
            //for(int i =0; i < m_TackSlots.Count; ++i)
            //{
            //    if (m_TackSlots[i].strName.CompareTo(trackName) == 0)
            //    {
            //        PlayableBindSlot bindSlot = m_TackSlots[i];
            //        bindSlot.pUserAT = pUserData;
            //        bindSlot.pAble = pObject;
            //        bindSlot.bGenericBinding = bGenericBinding;
            //        if (bindSlot.playableAsset != null)
            //        {
            //            bindSlot.playableAsset.SetUserPointer(pObject);
            //        }
            //        if (bindSlot.binding.outputTargetType == typeof(Animator))
            //        {
            //            playableDirector.SetGenericBinding(bindSlot.binding.sourceObject, pObject.GetBehaviour<Animator>());
            //        }
            //        else if (bindSlot.binding.outputTargetType == typeof(GameObject))
            //        {
            //            playableDirector.SetGenericBinding(bindSlot.binding.sourceObject, pObject.gameObject);
            //        }
            //        else
            //        {
            //            playableDirector.SetGenericBinding(bindSlot.binding.sourceObject, pObject.gameObject);
            //        }
            //        if(bindSlot.pSlot)
            //        {
            //            pObject.SetPosition(bindSlot.pSlot.position);
            //            pObject.SetScale(bindSlot.pSlot.lossyScale);
            //            pObject.SetEulerAngle(bindSlot.pSlot.eulerAngles);
            //            if(pUserData is AWorldNode)
            //            {
            //                AWorldNode node = pUserData as AWorldNode;
            //                node.SetFinalPosition(bindSlot.pSlot.position);
            //                node.SetEulerAngle(bindSlot.pSlot.eulerAngles);
            //                node.SetScale(bindSlot.pSlot.lossyScale);
            //            }
            //        }
            //        m_TackSlots[i] = bindSlot;
            //        break;
            //    }
            //}
        }
#endif
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(TimelineController), true)]
    public class TimelineControllerEditor : Editor
    {
#if USE_TIMELINE
        EditorWindow m_pEditorWindow = null;
        private void OnEnable()
        {
            TimelineController assets = target as TimelineController;
            if (assets.playableDirector == null)
            {
                assets.playableDirector = assets.gameObject.GetComponent<UnityEngine.Playables.PlayableDirector>();
            }
            m_pEditorWindow = EditorWindow.GetWindow<EditorWindow>("AnimationWindow");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TimelineController assets = target as TimelineController;

            System.Reflection.FieldInfo[] fiels = assets.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            for (int i = 0; i < fiels.Length; ++i)
            {
                if (fiels[i].IsNotSerialized) continue;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(fiels[i].Name));
            }
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("刷新保存"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            {
                if (m_pEditorWindow)
                {
                    int currentFrame = 0;
                    float curFrameTime = 0;
                    AnimationClip clip = null;
                    FieldInfo filed = m_pEditorWindow.GetType().GetField("m_AnimEditor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (filed != null)
                    {
                        object animEditor = filed.GetValue(m_pEditorWindow);
                        if (animEditor != null)
                        {
                            PropertyInfo propField = animEditor.GetType().GetProperty("selection", BindingFlags.Public | BindingFlags.Instance);
                            if (propField != null)
                            {
                                object animationwindowSelectionItem = propField.GetValue(animEditor);
                                if (animationwindowSelectionItem != null)
                                {
                                    PropertyInfo clipField = animationwindowSelectionItem.GetType().GetProperty("animationClip", BindingFlags.Public | BindingFlags.Instance);
                                    if (clipField != null)
                                    {
                                        clip = clipField.GetValue(animationwindowSelectionItem) as AnimationClip;
                                    }
                                }
                            }
                            propField = animEditor.GetType().GetProperty("state", BindingFlags.Public | BindingFlags.Instance);
                            if (propField != null)
                            {
                                object animationwindowState = propField.GetValue(animEditor);
                                if (animationwindowState != null)
                                {
                                    PropertyInfo clipField = animationwindowState.GetType().GetProperty("currentTime", BindingFlags.Public | BindingFlags.Instance);
                                    if (clipField != null)
                                    {
                                        curFrameTime = (float)clipField.GetValue(animationwindowState);
                                    }
                                    clipField = animationwindowState.GetType().GetProperty("currentFrame", BindingFlags.Public | BindingFlags.Instance);
                                    if (clipField != null)
                                    {
                                        currentFrame = (int)clipField.GetValue(animationwindowState);
                                    }
                                }
                            }
                        }
                    }

                    if (clip != null)
                    {
                        if (assets.slots != null)
                        {
                            if (GUILayout.Button("Lerp绑点到第" + currentFrame.ToString() + "帧"))
                            {
                                EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
                                if (bindings != null)
                                {
                                    for (int j = 0; j < assets.slots.Length; ++j)
                                    {
                                        if (assets.slots[j] == null) continue;
                                        if (assets.follow == assets.slots[j]) continue;
                                        string path = BaseUtil.GetTransformToPath(assets.slots[j], assets.transform, false);
                                        for (int i = 0; i < bindings.Length; ++i)
                                        {
                                            if (bindings[i].path.CompareTo(path) != 0) continue;
                                            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, bindings[i]);
                                            if (UpdateCurve(bindings[i], assets.slots[j], curFrameTime, curve))
                                            {
                                                AnimationUtility.SetEditorCurve(clip, bindings[i], curve);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (assets.follow != null && CameraController.getInstance() != null && CameraController.getInstance().GetCamera() &&
                            CameraController.getInstance().GetTransform()
                            && Application.isPlaying && GUILayout.Button("Lerp相机到第" + currentFrame.ToString() + "帧"))
                        {
                            string path = BaseUtil.GetTransformToPath(assets.follow, assets.transform, false);
                            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
                            if (bindings != null)
                            {
                                Transform pCamera = CameraController.getInstance().GetTransform();
                                for (int i = 0; i < bindings.Length; ++i)
                                {
                                    if (bindings[i].path.CompareTo(path) != 0) continue;
                                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, bindings[i]);
                                    if (UpdateCurve(bindings[i], pCamera, curFrameTime, curve))
                                    {
                                        AnimationUtility.SetEditorCurve(clip, bindings[i], curve);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
        //------------------------------------------------------
        bool UpdateCurve(EditorCurveBinding binding, Transform pSlot, float curFrameTime, AnimationCurve curve)
        {
            bool bDirty = false;
            if (binding.propertyName.CompareTo("m_Position.x") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, pSlot.position.x, 0.001f);
                bDirty = true;
            }
            else if (binding.propertyName.CompareTo("m_Position.y") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, pSlot.position.y, 0.001f);
                bDirty = true;
            }
            else if (binding.propertyName.CompareTo("m_Position.z") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, pSlot.position.z, 0.001f);
                bDirty = true;
            }
            else if (binding.propertyName.CompareTo("m_LocalPosition.x") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, pSlot.localPosition.x, 0.001f);
                bDirty = true;
            }
            else if (binding.propertyName.CompareTo("m_LocalPosition.y") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, pSlot.localPosition.y, 0.001f);
                bDirty = true;
            }
            else if (binding.propertyName.CompareTo("m_LocalPosition.z") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, pSlot.localPosition.z, 0.001f);
                bDirty = true;
            }
            else if (binding.propertyName.CompareTo("eulerAnglesRaw.x") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, pSlot.eulerAngles.x, 0.001f);
                bDirty = true;
            }
            else if (binding.propertyName.CompareTo("eulerAnglesRaw.y") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, ClampAngle(pSlot.eulerAngles.y), 0.001f);
                bDirty = true;
            }
            else if (binding.propertyName.CompareTo("eulerAnglesRaw.z") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, pSlot.eulerAngles.z, 0.001f);
                bDirty = true;
            }
            else if (binding.propertyName.CompareTo("localEulerAnglesRaw.x") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, pSlot.localEulerAngles.x, 0.001f);
                bDirty = true;
            }
            else if (binding.propertyName.CompareTo("localEulerAnglesRaw.y") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, ClampAngle(pSlot.localEulerAngles.y), 0.001f);
                bDirty = true;
            }
            else if (binding.propertyName.CompareTo("localEulerAnglesRaw.z") == 0)
            {
                BaseUtil.AddCurveKey(curve, curFrameTime, pSlot.localEulerAngles.z, 0.001f);
                bDirty = true;
            }
            return bDirty;
        }
        //------------------------------------------------------
        private static float ClampAngle(float angle)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return angle;
        }
#endif
    }
#endif
}