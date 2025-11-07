/********************************************************************
生成日期:	12:28:2020 13:16
类    名: 	UICliper
作    者:	Happli
描    述:	ui裁剪，用于canvas 并行裁剪
*********************************************************************/

using UnityEngine.UI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.UI
{
    [UIWidgetExport]
    [ExecuteInEditMode]
    public class UICliper : MonoBehaviour
    {
        static int _CustomClipRectID = 0;
        static Vector3[] wc = new Vector3[4];
        public ScrollRect scrollRect;
        public ListView listView;
        public RectMask2D mask;
        public Material clipMat;

        public Graphic[] graphics;
        //------------------------------------------------------
        private void Awake()
        {
            if(scrollRect)
            {
                scrollRect.onValueChanged.AddListener((e) => { setClip(); });
                setClip();
            }
            else if (listView)
            {
                listView.onValueChanged.AddListener((e) => { setClip(); });
                setClip();
            }
        }
        //------------------------------------------------------
        void setClip()
        {
            if (mask && clipMat)
            {
                if (_CustomClipRectID == 0)
                    _CustomClipRectID = Shader.PropertyToID("_CustomClipRect");
                mask.rectTransform.GetWorldCorners(wc);        // 计算world space中的点坐标
                var clipRec1t = new Vector4(wc[0].x, wc[0].y, wc[2].x, wc[2].y);// 选取左下角和右上角
                clipMat.SetVector(_CustomClipRectID, clipRec1t);
                for (int i = 0; i < graphics.Length; ++i)
                {
                    if (!graphics[i])
                    {
                        continue;
                    }
                    graphics[i].material = clipMat;
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UICliper), true)]
    [CanEditMultipleObjects]
    public class UICliperEditor : Editor 
    {
        //------------------------------------------------------
        protected void OnEnable()
        {
            UICliper cliper = target as UICliper;
            if (cliper.clipMat == null)
                cliper.clipMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/DatasRef/UI/Materials/UICliperMaterial.mat");
            if (cliper.graphics == null)
                cliper.graphics = cliper.GetComponentsInChildren<Graphic>(true);
        }
        //------------------------------------------------------
        protected void OnDisable()
        {
            UICliper cliper = target as UICliper;
            if (cliper.clipMat == null)
                cliper.clipMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/DatasRef/UI/Materials/UICliperMaterial.mat");
            if (cliper.graphics == null)
                cliper.graphics = cliper.GetComponentsInChildren<Graphic>(true);
        }
    }
#endif
}