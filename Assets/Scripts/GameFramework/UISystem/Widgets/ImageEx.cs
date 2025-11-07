/********************************************************************
生成日期:	12:28:2020 13:16
类    名: 	ImageEx
作    者:	Happli
描    述:	Image 扩展
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
    [ExecuteAlways]
    public class ImageEx : Image
    {
		public bool bAysncLoad = false;
        public string texturePath;

        private Sprite m_DefaultSprite = null;
        private string m_strPath = null;
        Asset m_pAssetRef;
        private bool m_bDirtyTexture = false;
        protected override void Awake()
        {
            if (!string.IsNullOrEmpty(m_strPath))
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
            base.OnDestroy();
        }
        //------------------------------------------------------
        public AssetOperiaon SetAssetByPath(string path,Sprite defaultSprite)
        {
            if (string.IsNullOrEmpty(path))
            {
                m_DefaultSprite = defaultSprite;
                SetAssetRef(null);
                m_strPath = null;
                return null;
            }
            if (path.CompareTo(m_strPath) == 0)
            {
                if(m_pAssetRef!=null && m_pAssetRef.IsValid())
                {
                    if (m_pAssetRef.RefCnt <= 0) m_pAssetRef.Grab();
                     Sprite oriSprite = m_pAssetRef.GetOrigin<Sprite>();
                    if (oriSprite != null) SetSprite(oriSprite);
                    return null;
                }
            }
            m_strPath = path;

            m_DefaultSprite = defaultSprite;
            SetSprite(m_DefaultSprite ? m_DefaultSprite : Base.ConstDef.TransparencySprite);
            AssetOperiaon pCallback;
			if(bAysncLoad)
				pCallback = FileSystemUtil.AsyncReadFile(m_strPath, OnAssetLoadCallback);
			else pCallback = FileSystemUtil.ReadFile(m_strPath, OnAssetLoadCallback);
            if (pCallback != null)
            {
                pCallback.assingType = typeof(Sprite);
                pCallback.userData = new VariableObj() { pGO = this };
            }
            return pCallback;
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
            if (assetRef != null && !string.IsNullOrEmpty(assetRef.Path) && assetRef.Path.CompareTo(m_strPath) != 0)
                return;

            if (m_pAssetRef != null)
            {
                m_pAssetRef.Release();
                m_pAssetRef = null;
            }
            if (assetRef != null)
            {
                Sprite oriSprite = assetRef.GetOrigin<Sprite>();
                assetRef.Grab();
                m_pAssetRef = assetRef;
                SetSprite(oriSprite);
                if (oriSprite == null)
                {
#if UNITY_EDITOR
                    if(FileSystemUtil.GetStreamType() <= EFileSystemType.AssetData)
                    {
                        Debug.Log("object asset is not sprite:" + assetRef.Path);
                        UnityEditor.TextureImporter import = UnityEditor.AssetImporter.GetAtPath(assetRef.Path) as UnityEditor.TextureImporter;
                        if (import && import.textureType != UnityEditor.TextureImporterType.Sprite)
                            Debug.LogError("请将" + assetRef.Path + "  图片转为Sprite");
                        Texture2D texture2d = assetRef.GetOrigin() as Texture2D;
                        if (texture2d != null)
                        {
                            SetSprite(Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), Vector2.zero, 100));
                        }
                        else
                            SetSprite(m_DefaultSprite ? m_DefaultSprite : Base.ConstDef.TransparencySprite);
                    }
                    else SetSprite(m_DefaultSprite ? m_DefaultSprite : Base.ConstDef.TransparencySprite);

#else
                    SetSprite(m_DefaultSprite? m_DefaultSprite : Base.ConstDef.TransparencySprite);
#endif
                }
                m_bDirtyTexture = true;
            }
            else
            {
                SetSprite(m_DefaultSprite? m_DefaultSprite : Base.ConstDef.TransparencySprite);
                m_bDirtyTexture = true;
                m_strPath = null;
            }
        }
        //------------------------------------------------------
        public string GetSpritePath()
        {
            if (string.IsNullOrEmpty(m_strPath)) return texturePath;
            return m_strPath;
        }
        //------------------------------------------------------
        void SetSprite(Sprite oriSprite)
        {
            this.sprite = oriSprite;
#if UNITY_EDITOR
            if(!Application.isPlaying)
                this.SetMaterialDirty();
#endif            
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

            if (pObj is ImageEx)
            {
                pCallback.pAsset.Release();
                ImageEx rawImage = pObj as ImageEx;
                rawImage.SetAssetRef(pCallback.pAsset);
                return;
            }
            else
            {
                pCallback.pAsset.Release();
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
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ImageEx), true)]
    [CanEditMultipleObjects]
    public class ImageExEditor : UnityEditor.UI.ImageEditor
    {
        string m_strPath = null;
        System.Reflection.FieldInfo m_pStrPathField = null;
        //------------------------------------------------------
        protected override void OnEnable()
        {
            base.OnEnable();
            ImageEx image = target as ImageEx;

            m_pStrPathField = image.GetType().GetField("m_strPath", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
            
            m_strPath = image.GetSpritePath();
            if (!string.IsNullOrEmpty(m_strPath))
                image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(m_strPath);
            else image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/DatasRef/UI/Textures/default/ui_empty.png");
        }
        //------------------------------------------------------
        protected override void OnDisable()
        {
            base.OnDisable();
            ImageEx image = target as ImageEx;
            if(string.IsNullOrEmpty(m_strPath))
                image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/DatasRef/UI/Textures/default/ui_empty.png");
        }
        //------------------------------------------------------
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ImageEx image = target as ImageEx;
            image.bAysncLoad = EditorGUILayout.Toggle("异步", image.bAysncLoad);
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if(EditorGUI.EndChangeCheck())
                Framework.ED.EditorUtil.RepaintPlayModeView();
 
            image.texturePath = image.sprite ? AssetDatabase.GetAssetPath(image.sprite) : null;
            m_strPath = image.texturePath;
            if(m_pStrPathField!=null)
                m_pStrPathField.SetValue(image, m_strPath);
        }
    }
#endif
}