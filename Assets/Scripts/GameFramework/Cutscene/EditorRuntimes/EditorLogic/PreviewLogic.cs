#if USE_CUTSCENE
#if UNITY_EDITOR
/********************************************************************
生成日期:	06:30:2025
类    名: 	PreviewLogic
作    者:	HappLI
描    述:	预览窗口
*********************************************************************/
#if UNITY_EDITOR
using Framework.ED;
using UnityEditor;
using UnityEngine;

namespace Framework.Cutscene.Editor
{
   // [EditorBinder(typeof(CutsceneEditor), "PreviewRect")]
    public class PreviewLogic : AEditorLogic
    {
        TargetPreview m_Preview;
        GUIStyle m_PreviewStyle;


        //test
        GameObject m_RoleTest = null;
        //--------------------------------------------------------
        protected override void OnEnable()
        {
            if (m_Preview == null) m_Preview = new TargetPreview(GetOwner());
            GameObject[] roots = new GameObject[1];
            roots[0] = new GameObject("EditorRoot");
            m_Preview.AddPreview(roots[0]);

            m_Preview.SetCamera(0.01f, 10000f, 60f);
            m_Preview.Initialize(roots);
            m_Preview.SetPreviewInstance(roots[0] as GameObject);
            m_Preview.OnDrawAfterCB = this.OnDraw;
            m_Preview.bLeftMouseForbidMove = true;

            //test
            m_Preview.AddPreview(m_RoleTest);
            m_Preview.OnDrawAfterCB += OnDraw;
        }
        //--------------------------------------------------------
        protected override void OnDisable()
        {
            if (m_RoleTest != null)
            {
                GameObject.DestroyImmediate(m_RoleTest);
                m_RoleTest = null;
            }
            if (m_Preview != null) m_Preview.Destroy();
            m_Preview = null;
        }
        //--------------------------------------------------------
        void OnDraw(int controllerId, Camera camera, Event evt)
        {
        }
        //--------------------------------------------------------
        protected override void OnGUI()
        {
        }
    }
}

#endif
#endif
#endif