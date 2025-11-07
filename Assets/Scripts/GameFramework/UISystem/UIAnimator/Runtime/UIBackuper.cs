#if UNITY_EDITOR
/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	UIAnimatorGroup
作    者:	Backuper
描    述:	UI 动画
*********************************************************************/
using System.Collections.Generic;
using Framework.RtgTween;
using UnityEngine;

namespace Framework.UI
{
    public class Backuper
    {
        bool m_bBackuped = false;
        public Dictionary<UnityEngine.Object, BackuperProperty> m_Propertys = new Dictionary<UnityEngine.Object, BackuperProperty>();
        UnityEngine.Object m_pControll = null;
        //------------------------------------------------------
        public UnityEngine.Object GetController()
        {
            return m_pControll;
        }
        //------------------------------------------------------
        public Transform GetTransform()
        {
            if (m_pControll == null) return null;
            Transform transform = m_pControll as Transform;
            if (transform) return transform;
            GameObject go = m_pControll as GameObject;
            if (go) return go.transform;

            Behaviour behavior = m_pControll as Behaviour;
            if (behavior) return behavior.transform;
            return null;
        }
        //------------------------------------------------------
        public void SetController(UnityEngine.Object pController)
        {
            if (m_pControll == pController) return;
            if (m_bBackuped)
                Recovert();
            m_pControll = pController;
            m_bBackuped = false;
            Backup();
        }
        //------------------------------------------------------
        public void Backup()
        {
            if (m_bBackuped) return;
            if (m_pControll == null) return;
            m_bBackuped = true;
            m_Propertys.Clear();
            BackupIteration(m_pControll);
        }
        //------------------------------------------------------
        void BackupIteration(UnityEngine.Object pObj)
        {
            BackuperProperty prop = new BackuperProperty();
            prop.SetController(pObj);
            m_Propertys[pObj] = prop;
            Transform pTran = null;
            {
                pTran = pObj as Transform;
                if (pTran == null)
                {
                    GameObject go = m_pControll as GameObject;
                    if (go) pTran = go.transform;
                }
                if (pTran == null)
                {
                    Behaviour behavior = pObj as Behaviour;
                    if (behavior) pTran = behavior.transform;
                }
            }
            if (pTran == null) return;
            for (int i = 0; i < pTran.childCount; ++i)
            {
                Transform child = pTran.GetChild(i);
                BackupIteration(child);
            }
        }
        //------------------------------------------------------
        public void Recovert()
        {
            if (!m_bBackuped) return;
            foreach (var db in m_Propertys)
            {
                db.Value.Recovert();
            }
        }
    }
    public class BackuperProperty
    {
        private Dictionary<UnityEngine.Behaviour, RtgTween.RtgTweenerProperty> m_pUIGraphics;
        private UnityEngine.Object m_pController;
        public Vector3 position;
        public Vector3 scale;
        public Vector3 eulerAngle;
        public Vector2 privot;
        public Color color;
        public float fov;

        bool m_bBackuped = false;
        public void SetController(UnityEngine.Object pController)
        {
            if (m_pController == pController) return;
            Recovert();
            if (m_pUIGraphics != null) m_pUIGraphics.Clear();
            m_bBackuped = false;
            m_pController = pController;
            Backup();
        }
        //------------------------------------------------------
        public UnityEngine.Object GetController()
        {
            return m_pController;
        }
        //------------------------------------------------------
        public Transform GetTransform()
        {
            if (m_pController == null) return null;
            Transform transform = m_pController as Transform;
            if (transform) return transform;
            GameObject go = m_pController as GameObject;
            if (go) return go.transform;

            Behaviour behavior = m_pController as Behaviour;
            if (behavior) return behavior.transform;
            return null;
        }
        //------------------------------------------------------
        public Camera GetCamera()
        {
            if (m_pController == null) return null;
            return m_pController as Camera;
        }
        //------------------------------------------------------
        public RectTransform GetRectTransform()
        {
            return GetTransform() as RectTransform;
        }
        //------------------------------------------------------
        public Dictionary<UnityEngine.Behaviour, RtgTweenerProperty> GetUIGraphics()
        {
            return m_pUIGraphics;
        }
        //------------------------------------------------------
        public void Recovert()
        {
            if (m_bBackuped)
            {
                Transform pTrans = GetTransform();
                if (pTrans)
                {
                    pTrans.position = position;
                    pTrans.eulerAngles = eulerAngle;
                    pTrans.localScale = scale;
                }
                RectTransform rectTrans = GetRectTransform();
                if (rectTrans)
                {
                    rectTrans.pivot = privot;
                }
                if (m_pUIGraphics != null)
                {
                    UIGraphicUtil.RecoverGraphic(m_pUIGraphics);
                }
                Camera camera = GetCamera();
                if (camera)
                {
                    camera.fieldOfView = fov;
                }
            }
        }
        //------------------------------------------------------
        public void Backup()
        {
            if (m_pController == null) return;
            if (m_bBackuped) return;
            m_bBackuped = true;
            Transform pTrans = GetTransform();
            if (pTrans)
            {
                position = pTrans.position;
                eulerAngle = pTrans.eulerAngles;
                scale = pTrans.localScale;
            }
            Camera camera = GetCamera();
            if (camera)
            {
                fov = camera.fieldOfView;
            }
            RectTransform rectTrans = GetRectTransform();
            if (rectTrans)
            {
                privot = rectTrans.pivot;
            }
            m_pUIGraphics = UIGraphicUtil.BackupGraphic<UnityEngine.Behaviour>(m_pUIGraphics, pTrans);
        }
    }
    //------------------------------------------------------
    public struct EditorData
    {
        public Rect rect;
        public Color color;
        public float start;
        public float length;
    }
}
#endif
