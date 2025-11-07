/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	BubbleText
作    者:	happli
描    述:	泡泡文本逻辑
*********************************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    [UIWidgetExport]
    public class BubbleText : MonoBehaviour
    {
        public Text label;
        public RtgTween.TweenerGroup tweenGroup;
        public short showTwen = 0;
        public short hideTween = 1;

        [Header("泡泡显示时长")]
        public float bubbleShowTime = 5;
        [Header("冒泡间隔")]
        public float queueDuration = 5;

        //! runtime
        bool m_bShowBubble = false;
        private string[] m_vQueueText = null;
        private uint[] m_TextDatas = null;
        private float m_fQueueGap = 0;
        private float m_fShowTime = 0;
        //------------------------------------------------------
        private void LateUpdate()
        {
            if(m_bShowBubble)
            {
                if (m_fShowTime > 0)
                {
                    m_fShowTime -= Time.deltaTime;
                    if (m_fShowTime <= 0)
                    {
                        if (hideTween >= 0 && tweenGroup != null) tweenGroup.ForcePlay(hideTween);
                        else transform.localScale = Vector3.zero;
                        m_bShowBubble = false;
                    }
                }
            }
            else
            {
                if( (m_TextDatas != null && m_TextDatas.Length > 0) || (m_vQueueText != null && m_vQueueText.Length > 0) )
                {
                    m_fQueueGap -= Time.deltaTime;
                    if (m_fQueueGap <= 0)
                    {
                        m_fQueueGap = queueDuration;
                        if (UnityEngine.Random.Range(0, 1)>0.5f)
                        {
                            if (m_TextDatas != null && m_TextDatas.Length > 0)
                            {
                               // SetText(Base.GlobalUtil.ToLocalization((int)m_TextDatas[UnityEngine.Random.Range(0, m_TextDatas.Length)]));
                            }
                            else if (m_vQueueText != null && m_vQueueText.Length > 0)
                                SetText(m_vQueueText[UnityEngine.Random.Range(0, m_vQueueText.Length)]);
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public bool IsShowing()
        {
            if (tweenGroup != null && tweenGroup.isPlaying()) return true;
            return m_bShowBubble;
        }
        //------------------------------------------------------
        public void SetText(string strText, float fHideTime=-1)
        {
            if (label == null) return;
            label.text = strText;
            if (fHideTime > 0) m_fShowTime = fHideTime;
            else m_fShowTime = bubbleShowTime;
            transform.localScale = Vector3.one;
            if (showTwen>=0 && tweenGroup != null) tweenGroup.ForcePlay(showTwen);
            else
                gameObject.SetActive(true);
            m_bShowBubble = true;
        }
        //------------------------------------------------------
        public void SetQueueText(string[] queueTexts)
        {
            m_fQueueGap = 0;
            m_bShowBubble = false;
            m_fShowTime = 0;
            m_vQueueText = null;
            m_TextDatas = null;
            if (queueTexts == null) return;
            m_vQueueText = queueTexts;
            m_fQueueGap = 0;
            gameObject.SetActive(true);
        }
        //------------------------------------------------------
        public void SetQueueText(uint[] queueTexts)
        {
            m_fQueueGap = 0;
            m_bShowBubble = false;
            m_fShowTime = 0;
            m_vQueueText = null;
            m_TextDatas = null;
            if (queueTexts == null) return;
            m_TextDatas = queueTexts;
            m_fQueueGap = 0;
            gameObject.SetActive(true);
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_vQueueText = null;
            m_TextDatas = null;
            m_fQueueGap = 0;
            m_bShowBubble = false;
            m_fShowTime = 0;
            transform.localScale = Vector3.zero;
        }
    }
}
