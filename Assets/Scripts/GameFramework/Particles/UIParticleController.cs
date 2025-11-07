/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UIParticleController
作    者:	HappLI
描    述:	UI特效控制器
*********************************************************************/
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.Core
{
    [UI.UIWidgetExport]
    public class UIParticleController : MonoBehaviour
    {
        [Header("播放结束时隐藏")]
        public bool bPlayEndHide = false;
        public TrailRenderer[] arrTrails = null;
        public ParticleSystem[] arrSystems = null;
        public List<DOTweenAnimation> arrDoTweens = null;
        bool m_bResetDirty = false;
        bool m_bActive = true;
        public bool Active { get { return m_bActive; } }
        //------------------------------------------------------
        void ResetDirty()
        {
            if (m_bResetDirty)
            {
                if (arrSystems != null)
                {
                    ParticleSystem par;
                    for (int i = 0; i < arrSystems.Length; ++i)
                    {
                        par = arrSystems[i];
                        if (par == null) continue;
                        par.Clear(true);
                        par.time = 0;
                        par.Play(true);
                    }
                }
                ResetTrails();
                if (arrDoTweens != null)
                {
                    for (int i = 0; i < arrDoTweens.Count; ++i)
                    {
                        if (arrDoTweens[i].tween == null)
                            arrDoTweens[i].CreateTween();
                        arrDoTweens[i].DORestart();
                    }
                }
                m_bResetDirty = false;
            }
        }
        //------------------------------------------------------
        bool IsStop()
        {
            bool bAllStop = true;
            ParticleSystem par;
            for (int i = 0; i < arrSystems.Length; ++i)
            {
                par = arrSystems[i];
                if (par == null) continue;
                if (par.isPlaying)
                {
                    bAllStop = false;
                    break;
                }
            }

            if (arrDoTweens != null)
            {
                for (int i = 0; i < arrDoTweens.Count; ++i)
                {
                    if (arrDoTweens[i].tween == null)
                        continue;
                    if (arrDoTweens[i].tween.IsPlaying())
                    {
                        bAllStop = false;
                        break;
                    }
                }
            }

            return bAllStop;
        }
        //------------------------------------------------------
        void ResetTrails()
        {
            if (arrTrails != null)
            {
                Vector3 scale = transform.localScale;
                for (int i = 0; i < arrTrails.Length; ++i)
                {
                    if (arrTrails[i]) arrTrails[i].Clear();
                }
            }
        }
        //------------------------------------------------------
        public void Play()
        {
            m_bResetDirty = true;
            SetActive(true);
        }
        //------------------------------------------------------
        private void LateUpdate()
        {
            if(m_bResetDirty)ResetDirty();

            if (bPlayEndHide && m_bActive && IsStop() )
            {
                SetActive(false);
            }
        }
        //------------------------------------------------------
        public float GetDuration()
        {
            float duration = 0;
            if(arrSystems!=null)
            { 
                for(int i = 0; i < arrSystems.Length; ++i)
                {
                    if(arrSystems[i] && !arrSystems[i].main.loop)
                    {
                        duration = Mathf.Max(duration, arrSystems[i].main.duration);
                    }
                }
            }
            if(arrDoTweens!=null)
            {
                for(int i = 0; i < arrDoTweens.Count; ++i)
                {
                    if(arrDoTweens[i].loops>0)
                    {
                        duration = Mathf.Max(duration, arrDoTweens[i].duration * arrDoTweens[i].loops);
                    }
                }
            }
            return duration;
        }
        //------------------------------------------------------
        public void SetActive(bool active)
        {
            m_bActive = active;
            gameObject.SetActive(active);
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UIParticleController))]
    public class UIParticleControllerEditor : Editor
    {
        //------------------------------------------------------
        private void OnEnable()
        {
            Refresh();
        }
        //------------------------------------------------------
        private void OnDisable()
        {
            Refresh();
        }
        //------------------------------------------------------
        void Refresh()
        {
            UIParticleController ctl = target as UIParticleController;
            ctl.arrTrails = ctl.gameObject.GetComponentsInChildren<TrailRenderer>();
            ctl.arrSystems = ctl.gameObject.GetComponentsInChildren<ParticleSystem>();

            if (ctl.arrDoTweens != null)
                ctl.arrDoTweens.Clear();

            DOTweenAnimation[] tweens = ctl.gameObject.GetComponentsInChildren<DOTweenAnimation>();
            if (tweens != null && tweens.Length > 0)
            {
                ctl.arrDoTweens = new List<DOTweenAnimation>(tweens);
            }
            DOTweenAnimation tween = ctl.gameObject.GetComponent<DOTweenAnimation>();
            if (tween)
            {
                if (ctl.arrDoTweens == null) ctl.arrDoTweens = new List<DOTweenAnimation>(1);
                if (!ctl.arrDoTweens.Contains(tween)) ctl.arrDoTweens.Add(tween);
            }
        }
        //------------------------------------------------------
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("刷新"))
            {
                Refresh();
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
    }
#endif
}
