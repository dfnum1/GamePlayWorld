/********************************************************************
生成日期:	12:28:2020 13:16
类    名: 	RawImageEx
作    者:	Happli
描    述:	RawImage 扩展
*********************************************************************/

using UnityEngine.UI;
using UnityEngine;
using Framework.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Framework.UI
{
    [UIWidgetExport]
    public class RawImageEx : RawImage
    {
        public bool bAysncLoad = false;
        public string texturePath;

        private Sprite m_DefaultSprite = null;
        private string m_strPath = null;
        Asset m_pAssetRef;

        private bool m_bDirtyTexture = false;
        protected override void Awake()
        {
            if(!string.IsNullOrEmpty(m_strPath))
            {
                SetAssetByPath(m_strPath, m_DefaultSprite);
                return;
            }
            if (!string.IsNullOrEmpty(texturePath) && texturePath.CompareTo("Assets/DatasRef/UI/Textures/default/ui_empty.png") != 0)
                SetAssetByPath(texturePath, m_DefaultSprite);
            base.Awake();
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            if (m_pAssetRef != null) m_pAssetRef.Release();
            m_pAssetRef = null;
            m_strPath = null;
            m_DefaultSprite = null;
            base.OnDestroy();
        }
        //------------------------------------------------------
        public AssetOperiaon SetAssetByPath(string path,Sprite defaultSprite =null)
        {
            if (string.IsNullOrEmpty(path))
            {
                m_DefaultSprite = defaultSprite;
                SetAssetRef(null);
                m_strPath = null;
                return null;
            }
//#if UNITY_EDITOR
//            if (!Framework.ED.EditorHelp.CanAssetBundlePath(path, true, this.transform, path.CompareTo(texturePath) !=0))
//                return null;
//#endif
            if (path.CompareTo(m_strPath) == 0)
            {
                if (m_pAssetRef != null && m_pAssetRef.IsValid())
                {
                    if (m_pAssetRef.RefCnt <= 0) m_pAssetRef.Grab();
                    Texture oriTexture = m_pAssetRef.GetOrigin<Texture>();
                    if(oriTexture!=null) this.texture = oriTexture;
                    else if(this.texture == null) this.texture = m_DefaultSprite ? m_DefaultSprite.texture : Base.ConstDef.TransparencyTexture;
                    return null;
                }
            }
            m_strPath = path;
            m_DefaultSprite = defaultSprite;
            texture = m_DefaultSprite ? m_DefaultSprite.texture : Base.ConstDef.TransparencyTexture;
            AssetOperiaon pCallback;
            if(bAysncLoad)
                pCallback = FileSystemUtil.AsyncReadFile(m_strPath, OnAssetLoadCallback);
            else
                pCallback = FileSystemUtil.ReadFile(m_strPath, OnAssetLoadCallback);
            if (pCallback != null)
                pCallback.userData = new VariableObj() { pGO = this };
            return pCallback;
        }
        //------------------------------------------------------
        public string GetTexturePath()
        {
            if (string.IsNullOrEmpty(m_strPath)) return texturePath;
            return m_strPath;
        }
        //------------------------------------------------------
        public void SetAssetRef(Asset assetRef)
        {
            UpdateTexture(assetRef);
        }
        //------------------------------------------------------
        void UpdateTexture(Asset assetRef)
        {
            if (m_pAssetRef == assetRef)
                return;
            //! if set asset is not current path ,do release
            if(assetRef !=null && !string.IsNullOrEmpty(assetRef.Path) && assetRef.Path.CompareTo(m_strPath) !=0)
                return;

            if (m_pAssetRef != null)
            {
                m_pAssetRef.Release();
                m_pAssetRef = null;
            }

            if (assetRef != null)
            {
                m_pAssetRef = assetRef;
                assetRef.Grab();
                Sprite spriteAsset = assetRef.GetOrigin<Sprite>();
                if (spriteAsset)
                {
                    SetSprite(this, spriteAsset);
                }
                else
                {
                    this.uvRect = new Rect(0, 0, 1, 1);
                    texture = assetRef.GetOrigin() as Texture2D;
                }
                m_bDirtyTexture = true;
            }
            else
            {
                texture = m_DefaultSprite ? m_DefaultSprite.texture : Base.ConstDef.TransparencyTexture;
                m_strPath = null;
                m_bDirtyTexture = true;
            }
        }
        //------------------------------------------------------
        private void LateUpdate()
        {
            if (m_bDirtyTexture)
            {
                m_bDirtyTexture = false;
                SetMaterialDirty();
            }
            if (m_pAssetRef != null && m_pAssetRef.RefCnt <= 0) m_pAssetRef.Grab();
        }
        //------------------------------------------------------
        public static void SetSprite(RawImage pRawImage, Sprite sprite, bool bSetNativeSize= false)
        {
            if (sprite == null) return;
            RectTransform rectTransform = pRawImage.rectTransform;

            Vector4 uv = UnityEngine.Sprites.DataUtility.GetInnerUV(sprite);

            pRawImage.texture = sprite.texture;
            pRawImage.uvRect = new Rect(uv.x, uv.y, uv.z- uv.x, uv.w- uv.y);
            if (bSetNativeSize) pRawImage.SetNativeSize();
            if (pRawImage is RawImageEx)
            {
                (pRawImage as RawImageEx).m_bDirtyTexture = true;
            }
            else pRawImage.SetMaterialDirty();
        }
        //------------------------------------------------------
        static protected void OnAssetLoadCallback(AssetOperiaon pCallback)
        {
            if (pCallback.pAsset == null) return;
            pCallback.pAsset.Grab();
            if (pCallback.userData == null)
            {
                pCallback.pAsset.Release();
                return;
            }

            UnityEngine.Object pObj = ((VariableObj)pCallback.userData).pGO;

            if (pObj == null)
            {
                pCallback.pAsset.Release();
                return;
            }

            if (pObj is RawImageEx)
            {
                RawImageEx rawImage = pObj as RawImageEx;
                pCallback.pAsset.Release();
                rawImage.SetAssetRef((Asset)pCallback.pAsset);
            }
            else
            {
                pCallback.pAsset.Release();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RawImageEx), true)]
    [CanEditMultipleObjects]
    public class RawImageExEditor : UnityEditor.UI.RawImageEditor
    {
        string m_strPath = null;
        System.Reflection.FieldInfo m_pStrPathField = null;
        //------------------------------------------------------
        protected override void OnEnable()
        {
            RawImageEx image = target as RawImageEx;

            m_pStrPathField = image.GetType().GetField("m_strPath", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
            
            m_strPath = image.GetTexturePath();
            if (!string.IsNullOrEmpty(m_strPath))
                image.texture = AssetDatabase.LoadAssetAtPath<UnityEngine.Texture2D>(m_strPath);
            else image.texture = AssetDatabase.LoadAssetAtPath<UnityEngine.Texture2D>("Assets/DatasRef/UI/Textures/default/ui_empty.png");

            base.OnEnable();
        }
        //------------------------------------------------------
        protected override void OnDisable()
        {
            base.OnDisable();
            RawImageEx image = target as RawImageEx;
            if(string.IsNullOrEmpty(m_strPath))
                image.texture = AssetDatabase.LoadAssetAtPath<UnityEngine.Texture2D>("Assets/DatasRef/UI/Textures/default/ui_empty.png");
        }
        //------------------------------------------------------
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            RawImageEx image = target as RawImageEx;
            image.bAysncLoad = EditorGUILayout.Toggle("异步", image.bAysncLoad);
            base.OnInspectorGUI();
 
            image.texturePath = image.texture ? AssetDatabase.GetAssetPath(image.texture) : null;
            m_strPath = image.texturePath;
            if(m_pStrPathField!=null)
                m_pStrPathField.SetValue(image, m_strPath);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}