/********************************************************************
生成日期:	2020-6-11
类    名: 	RotationTweenEffecter
作    者:	
描    述:	旋转过渡表现
*********************************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Framework.RtgTween
{
    [ExecuteInEditMode]
    public class RotationTweenEffecter : TweenEffecter
    {
        [Header("开始旋转角度")]
        public Vector3 StartRotation;
        [Header("结束旋转角度")]
        public Vector3 EndRotation;
        
        
        /// <summary>
        /// 编辑器预览使用
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="bEditor"></param>
        public override void Play(Vector3 startPos, bool bEditor = false,float fromTime = 0)
        {
            base.Play(startPos, bEditor, fromTime);
        }
        //------------------------------------------------------
        /// <summary>
        /// 实际项目运行效果使用
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="bEditor"></param>
        public override void Play(Vector3 startPos, Vector3 endPos, bool bEditor = false,float fromTime = 0)
        {
            base.Play(startPos, endPos, bEditor, fromTime);
        }
        //------------------------------------------------------
        public override void ForceUpdate(float fFrameTime)
        {
            //base.ForceUpdate(fFrameTime);//重写父类,去掉贝塞尔曲线位移
            if (!m_bPlaying) return;
            if (m_fTime < 0 || fDuration <= 0 || m_pTransform == null) return;
            UpdateSpeed(fFrameTime);
            m_fTime += fFrameTime*m_fSpeed;

            m_pTransform.rotation = Quaternion.Lerp(Quaternion.Euler(StartRotation), Quaternion.Euler(EndRotation), m_fTime / fDuration);

            if (m_fTime >= fDuration)
            {
                Stop();
            }
        }
        //------------------------------------------------------
        public override void OnPoolStart()
        {
            base.OnPoolStart();
            m_bPlaying = false;
            if (m_pTransform)
            {
                m_pTransform.rotation = Quaternion.identity;
            }
        }
        //------------------------------------------------------
        //资源回收
        public override void OnRecyle()
        {
            base.OnRecyle();
            m_bPlaying = false;
            if (m_pTransform)
            {
                m_pTransform.rotation = Quaternion.identity;
            }
            
        }
    }


}
