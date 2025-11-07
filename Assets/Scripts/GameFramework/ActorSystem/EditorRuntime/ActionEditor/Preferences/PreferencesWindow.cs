#if UNITY_EDITOR && USE_ACTORSYSTEM
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ActorSystem.ED
{
    class PreferencesWindow : PopupWindowContent
    {
        //private static Rect _myRect;
        //private bool firstPass = true;
        private static Vector2 win_size = new Vector2(400, 160);
        public static void Show(Rect rect)
        {
            rect.x = rect.x - win_size.x + rect.width;
            //_myRect = rect;
            PopupWindow.Show(rect, new PreferencesWindow());
        }

        public override Vector2 GetWindowSize() => win_size;

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("设置", new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize=22
            });
            GUILayout.Space(2);

            EditorPreferences.PreferencesGUI();
        }


    }
}
#endif