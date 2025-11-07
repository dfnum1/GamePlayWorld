/********************************************************************
生成日期:	27:7:2019   14:35
类    名: 	IGraphNode
作    者:	HappLI
描    述:	
*********************************************************************/
using UnityEngine;

namespace Framework.Plugin.AT
{
    public interface IGraphNode
    {
        void SetExpand(bool bExpand);
        bool IsExpand();
#if UNITY_EDITOR
        void OnSceneGUI(UnityEditor.SceneView sceneView);
        Vector2 GetPosition();
        void SetPosition(Vector2 pos);
        float GetWidth();
        float GetHeight();
        string GetDesc();
        int GetGUID();
        string ToTitleTips();
#endif
    }
    public enum EPortIO
    {
        In = 1 << 0,
        Out = 1 << 1,
    }
    public interface IPortEditor
    {
        EPortIO getIO();
        string GetDefaultName();
        void SetDefaultName(string strValue);
        int GetGUID();
        Color GetColor();
        bool IsInput();
        bool IsOutput();
        
        Rect GetRect();
        void SetRect(Rect rc);

        Rect GetViewRect();
        void SetViewRect(Rect rc);

        void Clear();
    }
}