/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	TextPopEffecter
作    者:	HappLI
描    述:	晃动表现
*********************************************************************/
#if USE_TMPRO
using TMPro;
#endif
using UnityEngine;

namespace Framework.RtgTween
{
    [ExecuteInEditMode]
    public class TextPopEffecter : TweenEffecter
    {
        public bool bRandomEnd = true;
        public bool bRandomStart = true;
        public int nRandomRange = 130;
        public int nStartXRandomRange = 20;
        public int nStartYRandomRange = 20;
#if USE_TMPRO
        public TMP_Text pTex;
#else
        public UnityEngine.UI.Text pTex;
#endif
        public UnityEngine.UI.Image pImg;
        //------------------------------------------------------
        public override void Play(Vector3 startPos, bool bEditor = false,float fromTime = 0)
        {
            base.Play(startPos, bEditor, fromTime);
            if (fDuration <= 0) return;
            if (pTex != null)
            {
                Color color = pTex.color;
                color.a = alpha.Evaluate(m_fTime);
                pTex.color = color;
            }

            if (pImg != null)
            {
                Color color = pImg.color;
                color.a = alpha.Evaluate(m_fTime);
                pImg.color = color;
            }
            m_pTransform.localScale = Vector3.zero;
        }
        //------------------------------------------------------
        public void SetText(string text)
        {
            if (pTex == null) return;
            pTex.text = text;
        }
        //------------------------------------------------------
        public void SetImg(string path)
        {
            if (pImg == null) return;
            pImg.gameObject.SetActive(!string.IsNullOrEmpty(path));
            (pImg as UI.ImageEx)?.SetAssetByPath(path, null);
        }
        //------------------------------------------------------
        public void SetColor(Color color)
        {
            if (pTex == null) return;
            pTex.color = color;
        }
        //------------------------------------------------------
        public override void ForceUpdate(float fFrameTime)
        {
            if (!m_bPlaying) return;
            base.ForceUpdate(fFrameTime);
            {
                float duration = fDuration;
                if (duration <= 0) duration = 1;
                if (pTex != null)
                {
                    Color color = pTex.color;
                    color.a = alpha.Evaluate(m_fTime/ fDuration);
                    pTex.color = color;                   
                }

                if (pImg != null)
                {
                    Color color = pImg.color;
                    color.a = alpha.Evaluate(m_fTime / fDuration);
                    pImg.color = color;
                }
            }
        }
    }
}
