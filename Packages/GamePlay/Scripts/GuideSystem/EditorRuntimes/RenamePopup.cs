#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Framework.Guide.Editor
{
    public class RenamePopup : EditorWindow
    {
		public static RenamePopup current { get; private set; }
		public object target;
		public object input;

		public string fieldName;
		private bool firstFrame = true;

		/// <summary> Show a rename popup for an asset at mouse position. Will trigger reimport of the asset on apply.
		public static RenamePopup Show(object target, object strName, float width = 200, string fieldName = "Name") {
			RenamePopup window = EditorWindow.GetWindow<RenamePopup>(true, "Rename " + strName, true);
			if (current != null) current.Close();
			current = window;
			window.target = target;
			window.input = strName;
			window.fieldName = fieldName;
            window.minSize = new Vector2(100, 44);
			window.position = new Rect(0, 0, width, 44);
			GUI.FocusControl("ClearAllFocus");
			window.UpdatePositionToMouse();
			return window;
		}

		private void UpdatePositionToMouse() {
			if (Event.current == null) return;
			Vector3 mousePoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
			Rect pos = position;
			pos.x = mousePoint.x - position.width * 0.5f;
			pos.y = mousePoint.y - 10;
			position = pos;
		}

		private void OnLostFocus() {
			// Make the popup close on lose focus
			Close();
		}

		private void OnGUI()
        {
			if (firstFrame)
            {
				UpdatePositionToMouse();
				firstFrame = false;
			}
			if(input is string)
			{
                input = EditorGUILayout.TextField(input.ToString());
                Event e = Event.current;
                if (input != null && input.ToString().Trim().Length > 0)
                {
                    if (GUILayout.Button("Apply") || (e.isKey && e.keyCode == KeyCode.Return))
                    {
                        FieldInfo field = target.GetType().GetField(fieldName);
                        if (field != null) field.SetValue(target, input);
                        Close();
                    }
                }
            }
        }
    }
}
#endif