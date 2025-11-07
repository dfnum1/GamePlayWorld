#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Framework.UI.ED
{
    public class UIAnimatorElementDrawer
    {
        UIAnimatorEditor m_pEditor;
        public void SetEditor(UIAnimatorEditor editor)
        {
            m_pEditor = editor;
        }

        //------------------------------------------------------
        public void DrawPositionTrackValue(UIAnimatorParameter pParameter)
        {
            Vector3 from = pParameter.from.toVector3();
            Vector3 to = pParameter.to.toVector3();
            from = EditorGUILayout.Vector3Field("from pos", from);
            to = EditorGUILayout.Vector3Field("to pos", to);
            pParameter.from.setVector3(from);
            pParameter.to.setVector3(to);
        }
        //------------------------------------------------------
        public void DrawRotateTrackValue(UIAnimatorParameter pParameter)
        {
            Vector3 from = pParameter.from.toVector3();
            Vector3 to = pParameter.to.toVector3();
            from = EditorGUILayout.Vector3Field("from rotate", from);
            to = EditorGUILayout.Vector3Field("to rotate", to);
            pParameter.from.setVector3(from);
            pParameter.to.setVector3(to);
        }
        //------------------------------------------------------
        public void DrawScaleTrackValue(UIAnimatorParameter pParameter)
        {
            Vector3 from = pParameter.from.toVector3();
            Vector3 to = pParameter.to.toVector3();
            from = EditorGUILayout.Vector3Field("from scale", from);
            to = EditorGUILayout.Vector3Field("to scale", to);
            pParameter.from.setVector3(from);
            pParameter.to.setVector3(to);
        }
        //------------------------------------------------------
        public void DrawPivotTrackValue(UIAnimatorParameter pParameter)
        {
            Vector2 to = pParameter.to.toVector2();
            to = EditorGUILayout.Vector2Field("锚点", to);
            pParameter.to.setVector2(to);
        }
        //------------------------------------------------------
        public void DrawAlphaTrackValue(UIAnimatorParameter pParameter)
        {
            float from = pParameter.from.property1;
            float to = pParameter.to.property1;
            from = EditorGUILayout.Slider("from alpha", from,0,1);
            to = EditorGUILayout.Slider("to alpha", to,0,1);
            pParameter.from.property1 =from;
            pParameter.to.property1 = to;
        }
        //------------------------------------------------------
        public void DrawColorTrackValue(UIAnimatorParameter pParameter)
        {
            Color from = pParameter.from.toColor();
            Color to = pParameter.to.toColor();
            from = EditorGUILayout.ColorField("from color", from);
            to = EditorGUILayout.ColorField("to color", to);
            pParameter.from.setColor(from);
            pParameter.to.setColor(to);
        }
    }
}
#endif
