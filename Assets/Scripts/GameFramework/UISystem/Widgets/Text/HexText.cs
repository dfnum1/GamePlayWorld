/********************************************************************
生成日期:	1:29:2021 10:06
类    名: 	HexText
作    者:	zdq
描    述:	数字文本显示组件
*********************************************************************/
using UnityEngine;
using UnityEngine.UI;
using Framework.Base;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.UI
{
    [DisallowMultipleComponent]
    public class HexText : Text
    {
        public enum ShowType
        {
            None,
            Small,
            Big
        }

        [SerializeField]
        public ShowType numberShowType = ShowType.Big;

        protected override void Awake()
        {
            base.Awake();
            raycastTarget = false;
            resizeTextForBestFit = true;
            horizontalOverflow = HorizontalWrapMode.Wrap;
            verticalOverflow = VerticalWrapMode.Truncate;
        }
        //------------------------------------------------------
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            switch (numberShowType)
            {
                case ShowType.None:
                    break;
                case ShowType.Small:
                    NumberSmall();
                    break;
                case ShowType.Big:
                    NumberBig();
                    break;
                default:
                    break;
            }

            base.OnPopulateMesh(toFill);
        }
        //------------------------------------------------------
        /// <summary>
        /// 小进制：
        /// 满万进K；起始10K，低于10K 取消K进制
        /// 满千万(1000K)进M；起始10M，低于10M、高于10K， 取消M、恢复K进制
        /// （不保留小数点，向下取整 ）
        /// </summary>
        void NumberSmall()
        {
            if (long.TryParse(text, out long num))
            {
                m_Text = BaseUtil.GetNumString(num, 10000, 1000000);
            }
        }
        //------------------------------------------------------
        void NumberBig()
        {
            if (long.TryParse(text, out long num))
            {
                m_Text = BaseUtil.GetNumString(num, 10000000, 100000000);
            }
        }
        //------------------------------------------------------
        public void SetCostNum(long leftNum, long rightNum)
        {
            m_Text = BaseUtil.SetNum(leftNum, rightNum);
            SetVerticesDirty();
        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HexText))]
    public class HexTextEditor : UnityEditor.UI.TextEditor
    {
        SerializedProperty m_ShowType;
        protected override void OnEnable()
        {
            base.OnEnable();
            m_ShowType = serializedObject.FindProperty("numberShowType");
        }
        //------------------------------------------------------
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(m_ShowType);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}