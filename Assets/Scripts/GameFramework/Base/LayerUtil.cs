using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Core;
using UnityEditor;
using UnityEngine;

namespace Framework.Base
{
    public enum EMaskLayer
    {
      //  [PluginDisplay("背景层")] BackLayer = 8,
        [PluginDisplay("前置层")] ForeLayer = 10,
      //  [PluginDisplay("特效层")] EffectLayer = 11,
        [PluginDisplay("UI3D层")] UI_3D = 12,
        [PluginDisplay("UI3D背景层")] UIBG_3D = 13,
   //     [PluginDisplay("UI3D前置层")] UIBGAfter_3D = 14,
        [PluginDisplay("物理碰撞层")] PhysicLayer = 15,
    };
    public class LayerUtil
    {
   //     public const string ms_backgroundLayerName = "BackLayer";
        public const string ms_foregroundLayerName = "ForceLayer";
        public const string ms_physicLayerName = "PhysicLayer";
  //      public const string ms_effectLayerName = "EffectLayer";
        public const string ms_ui3DLayerName = "UI_3D";
        public const string ms_ui3DBgLayerName = "UIBG_3D";
   //     public const string ms_ui3DBgAfterLayerName = "UIBGAfter_3D";
        private static int ms_nForeLayer = 0;
      //  private static int ms_nEffectLayer = 0;
        private static int ms_nPhysicLayer = 0;
        //private static int ms_nBackLayer = 0;

        private static int ms_ui3DLayer = 0;
        private static int ms_ui3DBgLayer = 0;
      //  private static int ms_ui3DBgAfterLayer = 0;
        public static int ForceLayer
        {
            get { if (ms_nForeLayer == 0) ms_nForeLayer = LayerMask.NameToLayer(ms_foregroundLayerName); return ms_nForeLayer; }
        }
        //public static int EffectLayer
        //{
        //    get { if (ms_nEffectLayer == 0) ms_nEffectLayer = LayerMask.NameToLayer(ms_effectLayerName); return ms_nEffectLayer; }
        //}
        //public static int BackLayer
        //{
        //    get { if (ms_nBackLayer == 0) ms_nBackLayer = LayerMask.NameToLayer(ms_backgroundLayerName); return ms_nBackLayer; }
        //}
        public static int PhysicLayer
        {
            get { if (ms_nPhysicLayer == 0) ms_nPhysicLayer = LayerMask.NameToLayer(ms_physicLayerName); return ms_nPhysicLayer; }
        }
        static public int RenderUIModelLayer
        {
            get { if (ms_ui3DLayer == 0) ms_ui3DLayer = LayerMask.NameToLayer(LayerUtil.ms_ui3DLayerName); return ms_ui3DLayer; }
        }
        static public int RenderUIModelBGLayer 
        {
            get { if (ms_ui3DBgLayer == 0) ms_ui3DBgLayer = LayerMask.NameToLayer(LayerUtil.ms_ui3DBgLayerName); return ms_ui3DBgLayer; }
        }
        //static public int RenderUIModelBGAfterLayer
        //{
        //    get { if (ms_ui3DBgAfterLayer == 0) ms_ui3DBgAfterLayer = LayerMask.NameToLayer(LayerUtil.ms_ui3DBgAfterLayerName); return ms_ui3DBgAfterLayer; }
        //}
        //-----------------------------------------------------
        public static void SetRenderLayer(GameObject pGo, int layer, bool bChild = true)
        {
#if !USE_SERVER
            if (pGo != null)
            {
                if (pGo.layer != layer)
                {
                    pGo.layer = layer;
                    if (bChild)
                    {
                        Transform pTrans = pGo.transform;
                        for (int i = 0; i < pTrans.childCount; ++i)
                        {
                            SetRenderLayer(pTrans.GetChild(i).gameObject, layer, bChild);
                        }
                    }
                }
            }
#endif
        }

        //-----------------------------------------------------
        public static void SetModelUI3DLayer(GameObject model)
        {
            SetLayer(model.transform, RenderUIModelLayer);
        }
        //-----------------------------------------------------
        public static void SetModelUIBG3DLayer(GameObject model)
        {
            SetLayer(model.transform, RenderUIModelBGLayer);
        }
        //-----------------------------------------------------
        //public static void SetModelUIBGAfter3DLayer(GameObject model)
        //{
        //    SetLayer(model.transform, RenderUIModelBGAfterLayer);
        //}
        //-----------------------------------------------------
        public static void SetModelForeLayer(GameObject model)
        {
            if (model == null)
            {
                return;
            }
            SetLayer(model.transform, ForceLayer);
        }
        //-----------------------------------------------------
        public static void SetLayer(Transform pTrans, int layer)
        {
            if (pTrans != null)
            {
                pTrans.gameObject.layer = layer;
                for (int i = 0; i < pTrans.childCount; ++i)
                {
                    pTrans.GetChild(i).gameObject.layer = layer;
                    SetLayer(pTrans.GetChild(i), layer);
                }
            }
        }
#if UNITY_EDITOR
        //-----------------------------------------------------
        public static string[] RENDER_LAYERS_POP = new string[] { "ForeLayer", "UI", "UI_3D", "UIBG_3D" };
        public static int[] RENDER_LAYERS_VALUE = new int[]
        {
            LayerMask.NameToLayer(RENDER_LAYERS_POP[0]),
            LayerMask.NameToLayer(RENDER_LAYERS_POP[1]),
            LayerMask.NameToLayer(RENDER_LAYERS_POP[2]),
            LayerMask.NameToLayer(RENDER_LAYERS_POP[3]),
        };
        //------------------------------------------------------
        public static void CheckTagAndLayer()
        {
            ClearLayerAndTag();
            // AddLayer(ms_backgroundLayerName, (int)EMaskLayer.BackLayer);
            AddLayer(ms_foregroundLayerName, (int)EMaskLayer.ForeLayer);
            //  AddLayer(ms_effectLayerName, (int)EMaskLayer.EffectLayer);
            AddLayer(ms_ui3DLayerName, (int)EMaskLayer.UI_3D);
            AddLayer(ms_ui3DBgLayerName, (int)EMaskLayer.UIBG_3D);
            // AddLayer(ms_ui3DBgAfterLayerName, (int)EMaskLayer.UIBGAfter_3D);

            AddLayer("PhysicLayer", (int)EMaskLayer.PhysicLayer);
            AddLayer("Editor", 31);

            AddTag("Terrain");
            AddTag("Timeline");
        }
        //------------------------------------------------------
        static void ClearLayerAndTag()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            for (int i = 0; i < layers.arraySize; i++)
            {
                SerializedProperty data = layers.GetArrayElementAtIndex(i);
                data.stringValue = "";
            }

            SerializedProperty tags = tagManager.FindProperty("tags");
            for (int i = 0; i < tags.arraySize; i++)
            {
                SerializedProperty data = tags.GetArrayElementAtIndex(i);
                data.stringValue = "";
            }
            tagManager.ApplyModifiedProperties();
        }
        //------------------------------------------------------
        static void AddLayer(string layer, int idLayer)
        {
            var targetName = "Element " + idLayer;
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            for (int i = 0; i < layers.arraySize; i++)
            {
                SerializedProperty data = layers.GetArrayElementAtIndex(i);
                if (data.displayName == targetName)
                {
                    data.stringValue = layer;
                    tagManager.ApplyModifiedProperties();
                    return;
                }
            }
        }
        //------------------------------------------------------
        static void AddTag(string tag)
        {
            if (!isHasTag(tag))
            {
                UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
                if ((asset != null) && (asset.Length > 0))
                {
                    SerializedObject so = new SerializedObject(asset[0]);
                    SerializedProperty tags = so.FindProperty("tags");

                    for (int i = 0; i < tags.arraySize; ++i)
                    {
                        if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                        {
                            return;     // Tag already present, nothing to do.
                        }
                    }
                    int size = tags.arraySize;
                    tags.InsertArrayElementAtIndex(size);
                    tags.GetArrayElementAtIndex(size).stringValue = tag;
                    so.ApplyModifiedProperties();
                    so.Update();
                }
            }
        }
        //------------------------------------------------------
        static bool isHasTag(string tag)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tag))
                    return true;
            }
            return false;
        }
        //-----------------------------------------------------
        public static int PopRenderLayer(string strDisplayName, int curVar, GUILayoutOption[] layOps = null)
        {
            return PopRenderLayer(new GUIContent(strDisplayName), curVar, layOps);
        }
        //-----------------------------------------------------
        public static int PopRenderLayer(GUIContent strDisplayName, int curVar, GUILayoutOption[] layOps = null)
        {
            int index = -1;
            for (int i = 0; i < RENDER_LAYERS_VALUE.Length; ++i)
            {
                if (RENDER_LAYERS_VALUE[i] == curVar)
                {
                    index = i;
                }
            }
            if (string.IsNullOrEmpty(strDisplayName.text))
                index = EditorGUILayout.Popup(index, RENDER_LAYERS_POP, layOps);
            else
                index = EditorGUILayout.Popup(strDisplayName, index, RENDER_LAYERS_POP, layOps);
            if (index >= 0 && index < RENDER_LAYERS_VALUE.Length)
            {
                curVar = RENDER_LAYERS_VALUE[index];
            }
            return curVar;
        }
        //-----------------------------------------------------
        public static int PopRenderLayerMask(string strDisplayName, int curVar, GUILayoutOption[] layOps = null)
        {
            return PopRenderLayerMask(new GUIContent(strDisplayName), curVar, layOps);
        }
        //------------------------------------------------------
        public static int PopRenderLayerMask(GUIContent strDisplayName, int curVar, GUILayoutOption[] layOps = null)
        {
            if (!string.IsNullOrEmpty(strDisplayName.text))
                EditorGUILayout.LabelField(strDisplayName);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < RENDER_LAYERS_POP.Length; ++i)
            {
                int mask = LayerMask.NameToLayer(RENDER_LAYERS_POP[i]);
                bool toggle = EditorGUILayout.Toggle(RENDER_LAYERS_POP[i], (curVar & (1 << mask)) != 0);
                if (toggle) curVar |= 1 << mask;
                else curVar &= ~(1 << mask);
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            return curVar;
        }
#endif
    }
}
