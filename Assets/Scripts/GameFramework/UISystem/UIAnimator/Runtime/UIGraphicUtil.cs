/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	UIAnimatorGroup
作    者:	HappLI
描    述:	UI 动画
*********************************************************************/
using System.Collections.Generic;
using Framework.RtgTween;
using UnityEngine;

namespace Framework.UI
{
    //------------------------------------------------------
    public class UIGraphicUtil
    {
        public static Dictionary<Behaviour, RtgTween.RtgTweenerProperty> BackupGraphic<T>(Dictionary<Behaviour, RtgTween.RtgTweenerProperty> vGraphic, Transform pTrans) where T : Behaviour
        {
            if (vGraphic != null) vGraphic.Clear();
            if (pTrans == null) return vGraphic;
            T[] grahpics = pTrans.GetComponentsInChildren<T>(true);
            if (grahpics != null && grahpics.Length>0)
            {
                if (vGraphic == null)
                    vGraphic = new Dictionary<Behaviour, RtgTweenerProperty>(grahpics.Length);
                for (int i = 0; i < grahpics.Length; ++i)
                {
                    if (grahpics[i] is CanvasGroup)
                    {
                        CanvasGroup graphic = grahpics[i] as CanvasGroup;
                        RtgTweenerProperty prop = new RtgTweenerProperty();
                        prop.setColor(new Color(1,1,1,graphic.alpha));
                        vGraphic[graphic] = prop;
                    }
                    else if (grahpics[i] is UnityEngine.UI.Graphic)
                    {
                        UnityEngine.UI.Graphic graphic = grahpics[i] as UnityEngine.UI.Graphic;
                        RtgTweenerProperty prop = new RtgTweenerProperty();
                        prop.setColor(graphic.color);
                        vGraphic[graphic] = prop;
                    }
                    else if (grahpics[i] is UnityEngine.UI.Shadow)
                    {
                        UnityEngine.UI.Shadow graphic = (grahpics[i] as UnityEngine.UI.Shadow);
                        RtgTweenerProperty prop = new RtgTweenerProperty();
                        prop.setVector4(graphic.effectColor);
                        vGraphic[graphic] = prop;
                    }
                }
            }
            return vGraphic;
        }
        public static void RecoverGraphic(Dictionary<UnityEngine.Behaviour, RtgTween.RtgTweenerProperty> vGraphic)
        {
            if (vGraphic != null)
            {
                foreach (var db in vGraphic)
                {
                    if (db.Key == null) continue;
                    if (db.Key is UnityEngine.CanvasGroup)
                    {
                        UnityEngine.CanvasGroup grphic = db.Key as UnityEngine.CanvasGroup;
                        grphic.alpha = db.Value.toAlpha();
                    }
                    else if (db.Key is UnityEngine.UI.Graphic)
                    {
                        UnityEngine.UI.Graphic grphic = db.Key as UnityEngine.UI.Graphic;
                        grphic.color = db.Value.toColor();
                    }
                    else if (db.Key is UnityEngine.UI.Shadow)
                    {
                        UnityEngine.UI.Shadow grphic = db.Key as UnityEngine.UI.Shadow;
                        grphic.effectColor = db.Value.toColor();
                    }
                }
            }
        }

        public static void UpdateGraphicColor(Dictionary<UnityEngine.Behaviour, RtgTween.RtgTweenerProperty> vGraphic, Color color)
        {
            if (vGraphic == null) return;
            foreach (var db in vGraphic)
            {
                if (db.Key == null) continue;
                if (db.Key is UnityEngine.UI.Graphic)
                {
                    UnityEngine.UI.Graphic grpahic = db.Key as UnityEngine.UI.Graphic;
                    grpahic.color = color;
#if UNITY_EDITOR
                    SetDirtyForceDraw(grpahic.transform);
#endif
                }
                else if (db.Key is UnityEngine.UI.Shadow)
                {
                    UnityEngine.UI.Shadow grpahic = db.Key as UnityEngine.UI.Shadow;
                    grpahic.effectColor = color;
#if UNITY_EDITOR
                    SetDirtyForceDraw(grpahic.transform);
#endif
                }
            }
        }
#if UNITY_EDITOR
        public static void SetDirtyForceDraw(Transform rect)
        {
            if(!Application.isPlaying)
            {
                Vector3 pos = rect.position;
                rect.position += new Vector3(0.1f, 0.1f, 0.1f);
                rect.position = pos;
            }
        }
#endif
        public static void UpdateGraphicAlpha(Dictionary<Behaviour, RtgTween.RtgTweenerProperty> vGraphic, float factor)
        {
            if (vGraphic == null) return;
            foreach (var db in vGraphic)
            {
                if (db.Key == null) continue;
                if (!db.Key.isActiveAndEnabled) continue;
                if (db.Key is UnityEngine.CanvasGroup)
                {
                    UnityEngine.CanvasGroup grpahic = db.Key as UnityEngine.CanvasGroup;
                    grpahic.alpha = factor * db.Value.toAlpha();
#if UNITY_EDITOR
                    SetDirtyForceDraw(grpahic.transform);
#endif
                }
                else if (db.Key is UnityEngine.UI.Graphic)
                {
                    UnityEngine.UI.Graphic grpahic = db.Key as UnityEngine.UI.Graphic;
                    Color color = grpahic.color;
                    color.a = factor * db.Value.toAlpha();
                    grpahic.color = color;
#if UNITY_EDITOR
                    SetDirtyForceDraw(grpahic.transform);
#endif
                }
                else if (db.Key is UnityEngine.UI.Shadow)
                {
                    UnityEngine.UI.Shadow grpahic = db.Key as UnityEngine.UI.Shadow;
                    Color color = grpahic.effectColor;
                    color.a = factor * db.Value.toAlpha();
                    grpahic.effectColor = color;
#if UNITY_EDITOR
                    SetDirtyForceDraw(grpahic.transform);
#endif
                }
            }
        }
    }
}

