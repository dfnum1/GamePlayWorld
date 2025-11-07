/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	TweenEffecter
作    者:	HappLI
描    述:	晃动表现
*********************************************************************/
using Framework.Base;
using Framework.Core;
using UnityEngine;

namespace Framework.RtgTween
{
    [ExecuteInEditMode]
    public class WorldUITweenEffecter : TweenEffecter
    {
        public Vector2 ThreeDto2DEndPos;//3d to 2d
        public float RayDistance = 20;

        public override bool IsPlaying()
        {
            return m_bPlaying;
        }

        protected override void Awake()
        {
            base.Awake();
        }
        //------------------------------------------------------
        public override void OnRecyle()
        {
            base.OnRecyle();
        }
        //------------------------------------------------------
        public override void Play(Vector3 startPos, Vector3 endPos, bool bEditor = false,float fromTime = 0)
        {
            base.Play(startPos, endPos, bEditor, fromTime);
            DisableCurve();
        }
        //------------------------------------------------------
        public override void Play(Vector3 startPos, bool bEditor = false, float fromTime = 0)
        {
            base.Play(startPos, bEditor, fromTime);
            DisableCurve();
        }
        //------------------------------------------------------
        public override void Stop(bool bDestroyCheck = true)
        {
            base.Stop(bDestroyCheck);
            //DisableCurve();
            ClearMaterialBlock();

            ResetScale();
        }
        //------------------------------------------------------
        void ResetScale()
        {
            Vector3 scale = Vector3.zero;
            if (sacleX != null && sacleX.length > 0) scale.x += sacleX.Evaluate(0);
            else scale.x = 1;
            if (sacleY != null && sacleY.length > 0) scale.y += sacleY.Evaluate(0);
            else scale.y = 1;
            if (sacleZ != null && sacleZ.length > 0) scale.z += sacleZ.Evaluate(0);
            else scale.z = 1;
            m_pTransform.localScale = scale;
        }
        //------------------------------------------------------
        void DisableCurve()
        {
            SetFloat("_CurveFactor", 0);
        }
        //------------------------------------------------------
        public override void ForceUpdate(float fFrameTime)
        {
            if (!m_bPlaying) return;
            if (AFramework.mainFramework == null) return;
            var uiMgr = UI.AUIManager.getInstance();
            if (uiMgr == null)
                return;

            if (m_fTime < 0 || fDuration <= 0) return;
            UpdateSpeed(fFrameTime);
            m_fTime += fFrameTime*m_fSpeed;

            if (CameraUtil.mainCamera == null)
                return;

            Vector3 EndPos1 = uiMgr.UGUIPosToWorldPos(CameraUtil.mainCamera, m_RuntimeEndPos, RayDistance);
            Vector3 StartPos1 = bLocal? m_pTransform.localPosition: m_pTransform.position;
            Vector3 m1 = StartPos1 + StartTangle;
            Vector3 m2 = EndPos1 + EndTangle;

            if (bLocal)
                m_pTransform.localPosition = m_StartPos + BezierUtility.Bezier4(m_fTime / fDuration, StartPos1, m1, m2, EndPos1);
            else
                m_pTransform.position = BezierUtility.Bezier4(m_fTime / fDuration, StartPos1, m1, m2, EndPos1);

            Vector3 scale = Vector3.zero;
            if (sacleX != null && sacleX.length > 0) scale.x += sacleX.Evaluate(m_fTime);
            else scale.x = 1;
            if (sacleY != null && sacleY.length > 0) scale.y += sacleY.Evaluate(m_fTime);
            else scale.y = 1;
            if (sacleZ != null && sacleZ.length > 0) scale.z += sacleZ.Evaluate(m_fTime);
            else scale.z = 1;
            m_pTransform.localScale = scale;

            if (m_fTime >= fDuration)
            {
                Stop();
                Framework.Plugin.AT.AgentTreeManager.getInstance().ExecuteEvent(5000, "OnGoldElementFlyFinish");
            }
        }
    }
}
