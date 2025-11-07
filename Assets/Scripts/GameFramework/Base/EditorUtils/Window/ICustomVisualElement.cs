#if UNITY_EDITOR
/********************************************************************
生成日期:	11:06:2023
类    名: 	AEditorGraphElementView
作    者:	HappLI
描    述:	
*********************************************************************/

namespace Framework.ED
{
    public interface ICustomVisualElement
    {
        void Init(EditorWindowBase editor);
        void Enable();
        void DoEvent(UnityEngine.Event evt);
        void Disable();
        void DrawGUI();
        void Update(float delta);
        EditorWindowBase GetOwner();
        void Destroy();
    }
}

#endif