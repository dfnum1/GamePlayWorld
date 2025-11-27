/********************************************************************
生成日期:	06:30:2025
类    名: 	ACutsceneEditor
作    者:	HappLI
描    述:	过场动作编辑器基础类
*********************************************************************/
#if UNITY_EDITOR
using Framework.ED;
using Framework.Cutscene.Runtime;
using UnityEngine;

namespace Framework.Cutscene.Editor
{
    public abstract class ACutsceneEditor : EditorWindowBase
    {
        public abstract bool IsRuntimeOpenPlayingCutscene();
        public abstract void OpenRuntimePlayingCutscene(CutsceneInstance pInstance);
        public abstract CutsceneInstance GetCutsceneInstance();
        public abstract void OpenAgentTreeEdit();
        public abstract void SaveAgentTreeData();
        public abstract AgentTreeWindow GetAgentTreeWindow();
        public virtual void OnSpawnInstance(GameObject pInstance) { }
        public virtual void OnSetTime(float time) { }
    }
}

#endif