#if USE_CUTSCENE && UNITY_EDITOR
/********************************************************************
生成日期:	06:30:2025
类    名: 	ICutsceneEditor
作    者:	HappLI
描    述:	过场动作编辑器
*********************************************************************/
using Framework.Core;
using Framework.Cutscene.Runtime;
using Framework.ED;
using UnityEditor;
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
        public virtual void OnSpawnInstance(AInstanceAble pInstance) { }
        public virtual void OnSetTime(float time) { }
    }
}
#endif