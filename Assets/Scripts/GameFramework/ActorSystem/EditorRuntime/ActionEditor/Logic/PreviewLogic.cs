#if UNITY_EDITOR && USE_ACTORSYSTEM
/********************************************************************
生成日期:	11:06:2023
类    名: 	SkillEditorPreviewLogic
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.Core;
using Framework.Cutscene.Editor;
using Framework.ED;
using System.Collections.Generic;
using TagLib.Riff;
using UnityEditor;
using UnityEngine;

namespace ActorSystem.ED
{
    [EditorBinder(typeof(ActionEditorWindow), "PreviewRect")]
    public class PreviewLogic : ActionEditorLogic
    {
        TargetPreview m_Preview;
        GUIStyle m_PreviewStyle;

        List<Actor> m_vActors = new List<Actor>();
        List<ACutsceneLogic> m_vCutsceneLogics = null;
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
            m_Preview.SetFloorTexture(AssetUtil.GetFloorTexture());
            m_Preview.bLeftMouseForbidMove = true;
            m_Preview.OnDrawAfterCB += OnDraw;

            m_vCutsceneLogics = GetLogics<ACutsceneLogic>();

            AInstanceAble.OnPoolStartLinster += OnSpawnInstance;
        }
        //--------------------------------------------------------
        public override void OnSelectActor(Actor pActor)
        {
            if (pActor == null)
                return;

            if (m_vActors.Contains(pActor))
                return;
            m_vActors.Add(pActor);
            if(pActor != null && pActor.GetObjectAble())
                m_Preview.AddPreview(pActor.GetObjectAble().gameObject);
        }
        //--------------------------------------------------------
        protected override void OnDisable()
        {
            AInstanceAble.OnPoolStartLinster -= OnSpawnInstance;
            if (m_Preview != null) m_Preview.Destroy();
            m_Preview = null;
        }
        //--------------------------------------------------------
        public void AddInstance(AInstanceAble pAble)
        {
            if (m_Preview != null && pAble)
                m_Preview.AddPreview(pAble.gameObject);
        }
        //--------------------------------------------------------
        void OnSpawnInstance(string path, GameObject pPrefab, AInstanceAble pAble)
        {
            AddInstance(pAble);
        }
        //--------------------------------------------------------
        void OnDraw(int controllerId, Camera camera, Event evt)
        {
            if(m_vCutsceneLogics!=null)
            {
                foreach (var logic in m_vCutsceneLogics)
                {
                    logic.OnPreviewDraw(controllerId, camera, evt);
                }
            }
            for(int i =0 ; i < m_vActors.Count;)
            {
                var actor = m_vActors[i];
                if (actor == null || actor.IsDestroy())
                {
                    m_vActors.RemoveAt(i);
                    continue;
                }

                Handles.Label(actor.GetFinalPosition() + Vector3.up * 0.5f, "攻击组[" + actor.GetAttackGroup() + "]");
                Handles.ArrowHandleCap(0, actor.GetFinalPosition(), Quaternion.Euler(actor.GetEulerAngle()), 1.0f, EventType.Repaint);
                if(Tools.current == Tool.Rotate)
                    actor.SetEulerAngleImmediately(Handles.DoRotationHandle(Quaternion.Euler(actor.GetEulerAngle()),actor.GetFinalPosition()).eulerAngles);
                else
                    actor.SetFinalPosition(Handles.DoPositionHandle(actor.GetFinalPosition(), Quaternion.identity));
                ++i;
            }
        }
        //--------------------------------------------------------
        protected override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
        }
        //--------------------------------------------------------
        protected override void OnGUI()
        {
            var window = GetOwner<ActionEditorWindow>();
            if (m_Preview != null && window.PreviewRect.width > 0 && window.PreviewRect.height > 0)
            {
                if(m_PreviewStyle == null)
                    m_PreviewStyle = new GUIStyle(EditorStyles.textField);
                m_Preview.OnPreviewGUI(window.PreviewRect, m_PreviewStyle);
            }
        }
    }
}

#endif