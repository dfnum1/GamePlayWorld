/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	UIAnimatorGroup
作    者:	HappLI
描    述:	UI 动画
*********************************************************************/
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

namespace Framework.UI
{
    public interface UIAnimatorCallback
    {
        void OnUIAnimatorPlayableBegin(IUserData userData);
        void OnUIAnimatorPlayableEnd(IUserData userData);
    }
    [System.Serializable]
    public class UIAnimatorPlayable
    {
        UIAnimatorGroupData m_pGroupData = null;

        float m_fOverrideSpeedScale = 1;
        float m_fOverrideSpeed = -1;
        float m_fCurrentDelta;


        UnityEngine.Object m_pParentWidget = null;

        List<UIAnimatorTask> m_lUIAnimatorTask = null;

        private IUserData m_pUserData = null;
        List<UIAnimatorCallback> m_vCallback = null;

        public UIAnimatorPlayable()
        {
            Clear();
        }
        //------------------------------------------------------
        ~UIAnimatorPlayable()
        {

        }
        //------------------------------------------------------
        public void Clear()
        {
            m_fOverrideSpeedScale = 1;
            m_fOverrideSpeed = -1;
            m_fCurrentDelta = 0;

            m_pParentWidget = null;

            m_pGroupData = null;
            if (m_vCallback != null) m_vCallback.Clear();

            m_pUserData = null;
        }
        //------------------------------------------------------
        public void SetGroupData(UIAnimatorGroupData parameter)
        {
            Clear();
            m_pGroupData = parameter;
            if (m_pGroupData == null) return;
            if (m_lUIAnimatorTask == null) m_lUIAnimatorTask = new List<UIAnimatorTask>(2);
            m_lUIAnimatorTask.Clear();

            if (m_pGroupData.Parameters!= null && m_pGroupData.Parameters.Length>0)
            {
                for (int i = 0; i < m_pGroupData.Parameters.Length; ++i)
                {
                    if (m_pGroupData.Parameters[i].GetTrackDuration()<=0)
                        continue;

                    UIAnimator task = new UIAnimator();
                    task.SetParameter(m_pGroupData.Parameters[i]);
                    m_lUIAnimatorTask.Add(task);
                }
            }
            if (m_pGroupData.BindTracks != null && m_pGroupData.BindTracks.Length > 0)
            {
                for (int i = 0; i < m_pGroupData.BindTracks.Length; ++i)
                {
                    if (m_pGroupData.BindTracks[i].GetTrackDuration() <= 0)
                        continue;

                    UITargetBindTrack task = new UITargetBindTrack();
                    task.SetParameter(m_pGroupData.BindTracks[i]);
                    m_lUIAnimatorTask.Add(task);
                }
            }
        }
        //------------------------------------------------------
        public void SetUserData(IUserData userdata)
        {
            m_pUserData = userdata;
        }
        //------------------------------------------------------
        public void AddCallback(UIAnimatorCallback callback)
        {
            if (m_vCallback == null) m_vCallback = new List<UIAnimatorCallback>(2);
            if (m_vCallback.Contains(callback)) return;
            m_vCallback.Add(callback);
        }
        //------------------------------------------------------
        public void RemoveCallback(UIAnimatorCallback callback)
        {
            if (m_vCallback == null) return;
            m_vCallback.Remove(callback);
        }
        //------------------------------------------------------
        public UIAnimatorGroupData GetGroupData()
        {
            return m_pGroupData;
        }
        //------------------------------------------------------
        public void Start(bool bRefresh = true)
        {
            m_fCurrentDelta = 0;
            if (bRefresh)
                RefreshControlledWidget();
            else
            {
                for (int i = 0; i < m_lUIAnimatorTask.Count; ++i)
                {
                    if(m_lUIAnimatorTask[i].IsValid())
                        m_lUIAnimatorTask[i].Start();
                }
            }

            if(m_vCallback!=null)
            {
                for(int i = 0; i < m_vCallback.Count; ++i)
                {
                    m_vCallback[i].OnUIAnimatorPlayableBegin(m_pUserData);
                }
            }
        }
        //------------------------------------------------------
        public void Stop(bool bCallback = true)
        {
            if (m_lUIAnimatorTask != null)
            {
                for (int i = 0; i < m_lUIAnimatorTask.Count; ++i)
                {
                    m_lUIAnimatorTask[i].Stop();
                }
            }
            if (bCallback && m_vCallback != null)
            {
                for (int i = 0; i < m_vCallback.Count; ++i)
                {
                    m_vCallback[i].OnUIAnimatorPlayableEnd(m_pUserData);
                }
            }
        }
        //------------------------------------------------------
        public void StopRecover(bool bCallback = true)
        {
            if (m_lUIAnimatorTask != null)
            {
                for (int i = 0; i < m_lUIAnimatorTask.Count; ++i)
                {
                    m_lUIAnimatorTask[i].StopRecover();
                }
            }
            if (bCallback && m_vCallback != null)
            {
                for (int i = 0; i < m_vCallback.Count; ++i)
                {
                    m_vCallback[i].OnUIAnimatorPlayableEnd(m_pUserData);
                }
            }
        }
        //------------------------------------------------------
        public bool Update(float fFrameTime)
        {
            bool bEnd = true;
            if (m_lUIAnimatorTask != null)
            {
                fFrameTime = Mathf.Min(fFrameTime, UIAnimatorFactory.getInstance().GetMaxFrameTimeStep());
                m_fCurrentDelta += fFrameTime * GetSpeed()*GetSpeedScale();

                UIAnimatorTask animator;
                for (int i = 0; i < m_lUIAnimatorTask.Count;++i)
                {
                    animator = m_lUIAnimatorTask[i];
                    animator.Update((int)(m_fCurrentDelta*1000), (int)(fFrameTime*1000), false );
                    if(!animator.IsEnd())
                    {
                        bEnd = false;
                    }
                    m_lUIAnimatorTask[i] = animator;
                }
                if (bEnd)
                {
                    Stop();
                }
            }
            return bEnd;
        }
        //------------------------------------------------------
        public bool IsEnd()
        {
            if (m_lUIAnimatorTask == null) return true;
            {
                UIAnimatorTask animator;
                for (int i = 0; i < m_lUIAnimatorTask.Count; ++i)
                {
                    animator = m_lUIAnimatorTask[i];
                    if (!animator.IsEnd()) return false;
                }
            }
            return true;
        }
        //------------------------------------------------------
        public void SetController(UnityEngine.Object pController)
        {
            m_pParentWidget = pController;
            RefreshControlledWidget();
        }
        //------------------------------------------------------
        public UnityEngine.Object GetController()
        {
            return m_pParentWidget;
        }
        //------------------------------------------------------
        public float GetCurrentDelta()
        {
            return m_fCurrentDelta;
        }
        //------------------------------------------------------
        public void SetCurrentDelta(float fDelta)
        {
            m_fCurrentDelta = fDelta;
            if(m_lUIAnimatorTask != null)
            {
                for (int i = 0; i < m_lUIAnimatorTask.Count; ++i)
                {
                    m_lUIAnimatorTask[i].SetCurrentDelta(fDelta);
                }
            }
        }
        //------------------------------------------------------
        public float GetTrackDuration()
        {
            if (m_pGroupData == null) return 0;
            return m_pGroupData.GetTrackLength();
        }
        //------------------------------------------------------
        public void SetReverse(bool bReverse)
        {
            if (m_lUIAnimatorTask == null) return;
            UIAnimatorTask animator;
            for (int i = 0; i < m_lUIAnimatorTask.Count; ++i)
            {
                animator = m_lUIAnimatorTask[i];
                animator.SetReverse(bReverse);
                m_lUIAnimatorTask[i] = animator;
            }
        }
        //------------------------------------------------------
        public void RefreshControlledWidget()
        {
            if (m_lUIAnimatorTask == null) return;
            UIAnimatorTask animator;
            for (int i = 0; i < m_lUIAnimatorTask.Count; ++i)
            {
                animator = m_lUIAnimatorTask[i];

                if (animator.GetControllerType() == ELogicController.Widget)
                {
                    if(animator.GetParameter().bFirstParent)
                        animator.SetController(m_pParentWidget);
                    else
                        animator.SetController(FindController(animator.GetControllerName(), animator.GetControllerTag()));
                }
                else if (animator.GetControllerType() == ELogicController.UICamera)
                {
                    Camera uiCamera = Camera.main;
                    if (uiCamera != null)
                    {
                        animator.SetController(uiCamera);
                    }
                }
                else if (animator.GetControllerType() == ELogicController.GameCamera)
                {
                    //if (Core.CameraKit.GetTransform() != null)
                    //{
                    //    animator.SetController(Core.CameraKit.GetTransform());
                    //}
                    //else
                    {
                        animator.SetController(Camera.main);
                    }
                }
                animator.Start();
                m_lUIAnimatorTask[i] = animator;
            }
        }
        //------------------------------------------------------
        public void SetSpeedScale(float fSpeed)
        {
            m_fOverrideSpeedScale = fSpeed;
        }
        //------------------------------------------------------
        public float GetSpeedScale()
        {
            return m_fOverrideSpeedScale;
        }
        //------------------------------------------------------
        public void SetSpeed(float fSpeed)
        {
            m_fOverrideSpeed = fSpeed;
        }
        //------------------------------------------------------
        public float GetSpeed()
        {
            if (m_pGroupData == null || m_fOverrideSpeed > 0) return m_fOverrideSpeed;
            return m_pGroupData.fSpeed;
        }
        //------------------------------------------------------
        public bool IsControlled(UnityEngine.Object pWidget)
        {
            if (m_pParentWidget == pWidget) return true;
            if (m_lUIAnimatorTask == null) return false;

            UIAnimatorTask animator;
            for (int i = 0; i < m_lUIAnimatorTask.Count; ++i)
            {
                animator = m_lUIAnimatorTask[i];
                if (animator.GetController() == pWidget) return true;
            }
            return false;
        }
        //------------------------------------------------------
        Transform FindController(string name, int guid)
        {
            if (string.IsNullOrEmpty(name) && guid == 0)
                return null;

            if (m_pParentWidget == null) return null;
            Transform root = m_pParentWidget as Transform;
            if (root == null) return null;

            Transform find = null;
            if(guid!=0)
                find = DyncmicTransformCollects.FindTransformByGUID(guid);
            if (find != null && BaseUtil.IsSubTransform(find, root)) return find;
            find = DyncmicTransformCollects.FindTransformByName(name);
            if (find && BaseUtil.IsSubTransform(find, root)) return find;
            if(!string.IsNullOrEmpty(name))
                find = BaseUtil.FindTransform(root, name,3);
            return find;
        }
    }
}

