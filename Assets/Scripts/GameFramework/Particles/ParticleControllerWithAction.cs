/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ParticleControllerWithAction
作    者:	HappLI
描    述:	特效控制器，结束时收到动作影响
*********************************************************************/
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;
namespace Framework.Core
{
    public class ParticleControllerWithAction : ParticleController
    {
        public bool clipPlay = false;
        public AnimationClip startActionClip = null;
        public AnimationClip endActionClip = null;
        public bool startActionClipInv = false;
        public bool endActionClipInv = false;

        public int startAction = 0;
        public int endAction = 0;
        public float fDelayTime = 0;

        private int m_nEndAction = 0;
        private float m_fClipDuration = 0;
        private bool m_bInvPlayer = false;
        private AnimationClip m_PlayingClip = null;
        //-------------------------------------------------
        public override void OnPoolStart()
        {
            base.OnPoolStart();
            m_nEndAction = 0;
            m_fClipDuration = 0;
            m_bInvPlayer = false;
            if (clipPlay)
            {
                if (startActionClip!=null)
                {
                    m_PlayingClip = startActionClip;
                    m_bInvPlayer = startActionClipInv;
                    m_nEndAction = (endActionClip != null) ? 1:0;
                }
            }
            else
            {
                if (startAction != 0)
                {
                    Animator animator = GetBehaviour<Animator>();
                    if (animator) animator.Play(startAction);
                }
                m_nEndAction = endAction;
            }
        }
        //-------------------------------------------------
        public override void RecyleDestroy(int recyleMax = 2)
        {
            if(m_nEndAction !=0)
            {
                int endHash = m_nEndAction;
                m_nEndAction = 0;
                m_fClipDuration = 0;
                if (clipPlay)
                {
                    if (endHash != 0)
                    {
                        m_PlayingClip = endActionClip;
                        m_bInvPlayer = endActionClipInv;
                        DelayDestroy(fDelayTime);
                        return;
                    }
                }
                else
                {
                    Animator animator = GetBehaviour<Animator>();
                    if (animator)
                    {
                        animator.Play(endHash);
                        DelayDestroy(fDelayTime);
                        return;
                    }
                }
            }
            base.RecyleDestroy(recyleMax);
        }
        //-------------------------------------------------
        protected override void LateUpdate()
        {
            base.LateUpdate();
            m_fClipDuration += Time.deltaTime;
            if (clipPlay)
            {
                if (m_PlayingClip)
                {
                    if(m_bInvPlayer)
                        m_PlayingClip.SampleAnimation(GetObject(), Mathf.Clamp(m_PlayingClip.length - m_fClipDuration,0, m_PlayingClip.length));
                    else
                        m_PlayingClip.SampleAnimation(GetObject(), Mathf.Clamp(m_fClipDuration, 0, m_PlayingClip.length));
                }
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ParticleControllerWithAction))]
    public class ParticleControllerWithActionEditor : Editor
    {
        System.Collections.Generic.List<AnimatorState> m_vPop = new System.Collections.Generic.List<AnimatorState>();
        System.Collections.Generic.List<string> m_vPopName = new System.Collections.Generic.List<string>();
        System.Collections.Generic.List<int> m_vPopNameHash = new System.Collections.Generic.List<int>();
        void Check()
        {
            m_vPop.Add(null);
            m_vPopName.Add("None");
            m_vPopNameHash.Add(0);
            ParticleControllerWithAction withAction = target as ParticleControllerWithAction;
            Animator animator = withAction.GetComponent<Animator>();
            if (animator != null)
            {
                AnimatorController controller = (AnimatorController)animator.runtimeAnimatorController;
                if (controller != null && controller.layers!=null && controller.layers.Length >0)
                {
                    AnimatorControllerLayer layer = controller.layers[0];
                    for(int i = 0; i < layer.stateMachine.states.Length; ++i)
                    {
                        if (layer.stateMachine.states[i].state == null || layer.stateMachine.states[i].state.motion == null) continue;
                        m_vPop.Add(layer.stateMachine.states[i].state);
                        m_vPopName.Add(layer.stateMachine.states[i].state.name);
                        m_vPopNameHash.Add(layer.stateMachine.states[i].state.nameHash);
                    }
                }
            }
        }
        //-------------------------------------------------
        private void OnEnable()
        {
            Check();
        }
        //-------------------------------------------------
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ParticleControllerWithAction parWithAction = target as ParticleControllerWithAction;
            parWithAction.clipPlay = EditorGUILayout.Toggle("使用AnimClip", parWithAction.clipPlay);
            parWithAction.fDelayTime = EditorGUILayout.FloatField("延迟删除", parWithAction.fDelayTime);
            if (parWithAction.clipPlay)
            {
                parWithAction.startActionClip = EditorGUILayout.ObjectField("开始动画", parWithAction.startActionClip, typeof(AnimationClip), false) as AnimationClip;
                parWithAction.startActionClipInv = EditorGUILayout.Toggle("开始动画反向播", parWithAction.startActionClipInv);
                parWithAction.endActionClip = EditorGUILayout.ObjectField("结束动画", parWithAction.endActionClip, typeof(AnimationClip), false) as AnimationClip;
                parWithAction.endActionClipInv = EditorGUILayout.Toggle("结束动画反向播", parWithAction.endActionClipInv);
                if (parWithAction.endActionClip != null)
                    parWithAction.fDelayTime = parWithAction.endActionClip.length;
            }
            else
            {
                //start
                {
                    int index = m_vPopNameHash.IndexOf(parWithAction.startAction);
                    index = EditorGUILayout.Popup("开始动作", index, m_vPopName.ToArray());
                    if (index >= 0 && index < m_vPop.Count)
                        parWithAction.startAction = m_vPopNameHash[index];
                }
                //end
                {
                    int index = m_vPopNameHash.IndexOf(parWithAction.endAction);
                    index = EditorGUILayout.Popup("结束动作", index, m_vPopName.ToArray());
                    if (index >= 0 && index < m_vPop.Count)
                    {
                        parWithAction.endAction = m_vPopNameHash[index];
                        if (m_vPop[index] != null && m_vPop[index].motion)
                            parWithAction.fDelayTime = m_vPop[index].motion.averageDuration;
                    }
                }
            }

            if (GUILayout.Button("刷新"))
            {
                UnityEditor.EditorUtility.SetDirty(target);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
            }
            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}
