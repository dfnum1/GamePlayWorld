/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	ADyanmicLoader
作    者:	HappLI
描    述:	动态加载，对象关系绑定操作
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    //------------------------------------------------------
    public class DynamicLoader : ADyanmicLoader
    {
        protected override bool LoadSignCheck()
        {
            return true;
        }
        protected override bool LoadInstanceSignCheck()
        {
            return true;
        }
    }
    //------------------------------------------------------
    public abstract class ADyanmicLoader : IInstanceAbleCallback
    {
        protected struct AssetData : IUserData
        {
            public string strPath;
            public UnityEngine.Object pObject;
            public Asset pAsset;
            public AssetOperiaon assetOperiaon;

            public void Destroy()
            {
                if (assetOperiaon != null) assetOperiaon.Clear();
                assetOperiaon = null;
                if (pAsset != null) pAsset.Grab();
                pAsset = null;
                strPath = null;
            }
        }

        struct InstanceData : IUserData
        {
            public string strFile;
            public GameObject pPrefab;
            public IUserData userPtr;

            public InstanceOperiaon pOperiaon;
            public AInstanceAble pAble;
            public Action<InstanceOperiaon> onCallback;
            public void Destroy()
            {
                if (pOperiaon != null) pOperiaon.Earse();
                pOperiaon = null;
                if (pAble != null) pAble.RecyleDestroy();
                pAble = null;
                onCallback = null;
                userPtr = null;
            }
        }

        Dictionary<UnityEngine.Object, AssetData> m_vDynamicLoad = new Dictionary<UnityEngine.Object, AssetData>(32);
        List<InstanceData> m_vInstance = null;
        bool m_bLockedInstance = false;
        //------------------------------------------------------
        protected abstract bool LoadSignCheck();
        protected abstract bool LoadInstanceSignCheck();
        //------------------------------------------------------
        public AssetOperiaon LoadObjectAsset(UnityEngine.Object pObj, string strPath, bool bPermanent = false, bool bAysnc = false,Sprite defaultSprite = null)
        {
            if (pObj == null) return null;
            if (pObj is Framework.UI.RawImageEx)
            {
                Framework.UI.RawImageEx image = (Framework.UI.RawImageEx)pObj;

                return image.SetAssetByPath(strPath, defaultSprite);
            }
            else if (pObj is Framework.UI.ImageEx)
            {
                Framework.UI.ImageEx image = (Framework.UI.ImageEx)pObj;

                return image.SetAssetByPath(strPath, defaultSprite);
            }

            if (string.IsNullOrEmpty(strPath))
            {
                if (pObj is Framework.UI.RawImageEx)
                {
                    Framework.UI.RawImageEx image = (Framework.UI.RawImageEx)pObj;

                    return image.SetAssetByPath(strPath, defaultSprite);
                }
                else if (pObj is Framework.UI.ImageEx)
                {
                    Framework.UI.ImageEx image = (Framework.UI.ImageEx)pObj;

                    return image.SetAssetByPath(strPath, defaultSprite);
                }
                AssetData pCheck;
                if (m_vDynamicLoad.TryGetValue(pObj, out pCheck))
                {
                    if (pCheck.pAsset != null) pCheck.pAsset.Release();
                    m_vDynamicLoad.Remove(pObj);
                }
                
                if (defaultSprite == null) defaultSprite = Base.ConstDef.TransparencySprite;
                if (defaultSprite!=null)
                {
                    if (pObj is UnityEngine.UI.RawImage)
                    {
                        UnityEngine.UI.RawImage image = (UnityEngine.UI.RawImage)pObj;
                        if (image != null)
                            image.texture = defaultSprite.texture;
                    }
                    else if (pObj is UnityEngine.UI.Image)
                    {
                        UnityEngine.UI.Image image = (UnityEngine.UI.Image)pObj;
                        if (image != null)
                            image.sprite = defaultSprite;
                    }
                    else if (pObj is UnityEngine.SpriteRenderer)
                    {
                        UnityEngine.SpriteRenderer spriteRender = (UnityEngine.SpriteRenderer)pObj;
                        if (spriteRender != null)
                            spriteRender.sprite = defaultSprite;
                    }
                }

                return null;
            }

            AssetData pData;
            if (!m_vDynamicLoad.TryGetValue(pObj, out pData))
            {
                pData = new AssetData();
                pData.strPath = strPath;
                pData.pAsset = null;
                pData.pObject = pObj;
                m_vDynamicLoad.Add(pObj, pData);
            }
            else
            {
                if (pData.assetOperiaon!=null && strPath.CompareTo(pData.assetOperiaon.strFile) == 0)
                {
                    return pData.assetOperiaon;
                }
            }
            if (pData.assetOperiaon != null)
                pData.assetOperiaon.Clear();
            pData.strPath = strPath;

            System.Type assingAssetType = null;
            //! set default
            if (pObj is UnityEngine.UI.RawImage)
            {
                UnityEngine.UI.RawImage image = (UnityEngine.UI.RawImage)pObj;
                image.texture = defaultSprite ? defaultSprite.texture : Base.ConstDef.TransparencyTexture;
            }
            else if (pObj is UnityEngine.UI.Image)
            {
                assingAssetType = typeof(Sprite);
                UnityEngine.UI.Image image = (UnityEngine.UI.Image)pObj;
                image.sprite = defaultSprite ? defaultSprite : Base.ConstDef.TransparencySprite;
            }
            else if (pObj is UnityEngine.SpriteRenderer)
            {
                assingAssetType = typeof(Sprite);
                UnityEngine.SpriteRenderer spriteRender = (UnityEngine.SpriteRenderer)pObj;
                spriteRender.sprite = defaultSprite ? defaultSprite : Base.ConstDef.TransparencySprite;
            }

            AssetOperiaon pOp = null;
            if (bAysnc)
                pOp =FileSystemUtil.AsyncReadFile(strPath, OnLoadAssetCallback);
            else
                pOp =FileSystemUtil.ReadFile(strPath, OnLoadAssetCallback);
            if (pOp != null)
            {
                pOp.bPermanent = bPermanent;
                pOp.userData = pData;
                pOp.assingType = assingAssetType;
            }
            pData.assetOperiaon = pOp;
            m_vDynamicLoad[pObj] = pData;
            return pOp;
        }
        //------------------------------------------------------
        private void OnLoadAssetCallback(AssetOperiaon pCB)
        {
            if (pCB == null) return;

            if(pCB.pAsset!=null)
            {
                //! grab asset 1
                pCB.pAsset.Grab();
            }
            if(pCB.userData == null || !(pCB.userData is AssetData))
            {
                if (pCB.pAsset != null)
                {
                    pCB.pAsset.Release();
                }
                return;
            }
            AssetData assetData = (AssetData)pCB.userData;
            assetData.assetOperiaon = null;
            UnityEngine.Object pObject = assetData.pObject;

            if (!LoadSignCheck())
            {
                if (pCB.pAsset != null)
                {
                    pCB.pAsset.Release();
                }
                m_vDynamicLoad.Remove(pObject);
                return;
            }

            AssetData dynamicMapAsset;
            if (pObject == null || !m_vDynamicLoad.TryGetValue(pObject, out dynamicMapAsset))
            {
                if (pCB.pAsset != null)
                {
                    pCB.pAsset.Release();
                }
                return;
            }
            if(pCB.pAsset != null && pCB.pAsset.Path.CompareTo(dynamicMapAsset.strPath) != 0)
            {
                pCB.pAsset.Release();
                m_vDynamicLoad[pObject] = assetData;
                return;
            }

            if (pObject is Framework.UI.RawImageEx)
            {
                Framework.UI.RawImageEx image = (Framework.UI.RawImageEx)pObject;

                image.SetAssetRef(pCB.pAsset);
                m_vDynamicLoad.Remove(pObject);
                return;
            }

            if (pObject is Framework.UI.ImageEx)
            {
                Framework.UI.ImageEx image = (Framework.UI.ImageEx)pObject;

                image.SetAssetRef((Asset)pCB.pAsset);
                m_vDynamicLoad.Remove(pObject);
                return;
            }

            if (!pCB.isValid())
            {
                m_vDynamicLoad.Remove(pObject);
                return;
            }

            if (OnLoadAsset(pObject, (Asset)pCB.pAsset) && pObject)
            {
                if (pCB.pAsset != assetData.pAsset)
                {
                    //! if pre asset, release ref 1
                    if (assetData.pAsset != null)
                    {
                        assetData.pAsset.Release();
                    }
                }
                else
                {
                    //! if common asset, release ref 1
                    if (pCB.pAsset.RefCnt>1)
                        pCB.pAsset.Release();
                }
                assetData.pAsset = (Asset)pCB.pAsset;
                m_vDynamicLoad[pObject] = assetData;
            }
            else
            {
                pCB.pAsset.Release();
                if (pObject) m_vDynamicLoad.Remove(pObject);
            }
        }
        //------------------------------------------------------
        protected virtual bool OnLoadAsset(UnityEngine.Object pObj, Asset pAssetRef)
        {
            if (pObj == null) return false;

            if (pObj is Framework.UI.RawImageEx)
            {
                Framework.UI.RawImageEx image = (Framework.UI.RawImageEx)pObj;

                image.SetAssetRef(pAssetRef);
                return true;
            }

            if (pObj is Framework.UI.ImageEx)
            {
                Framework.UI.ImageEx image = (Framework.UI.ImageEx)pObj;

                image.SetAssetRef(pAssetRef);
                return true;
            }

            if (pObj is UnityEngine.UI.RawImage)
            {
                UnityEngine.UI.RawImage image = (UnityEngine.UI.RawImage)pObj;

                Sprite spriteAsset = pAssetRef.GetOrigin<Sprite>();
                if (spriteAsset)
                {
                    UI.RawImageEx.SetSprite(image, spriteAsset);
                    return true;
                }
                else
                {
                    Texture pTexture = pAssetRef.GetOrigin() as Texture2D;
                    if (pTexture)
                    {
                        image.texture = pTexture;
                        image.uvRect = new Rect(0, 0, 1, 1);
                        image.SetMaterialDirty();
                        return true;
                    }
                }

                return false;
            }

            if (pObj is UnityEngine.UI.Image)
            {
                UnityEngine.UI.Image image = (UnityEngine.UI.Image)pObj;
                if (image == null)
                    return false;
                Sprite spr = pAssetRef.GetOrigin<Sprite>();
                if (spr)
                {
                    image.sprite = spr;
                    image.SetMaterialDirty();
                    return true;
                }
                else
                {
#if UNITY_EDITOR
                    if (FileSystemUtil.GetStreamType() <= EFileSystemType.AssetData)
                    {
                        image.sprite = Base.ConstDef.TransparencySprite;
                        image.SetMaterialDirty();
                    }
                    else
                    {
                        Texture2D texture2d = pAssetRef.GetOrigin() as Texture2D;
                        if (texture2d)
                        {
                            UnityEditor.TextureImporter import = UnityEditor.AssetImporter.GetAtPath(pAssetRef.Path) as UnityEditor.TextureImporter;
                            if (import && import.textureType != UnityEditor.TextureImporterType.Sprite)
                                Debug.LogError("请将" + pAssetRef.Path + "  图片转为Sprite");

                            Vector4 border = Vector4.zero;
                            if (image.sprite) border = image.sprite.border;
                            Sprite sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), Vector2.zero, 100, 0, SpriteMeshType.Tight, border);
                            if (sprite)
                            {
                                image.sprite = sprite;
                                image.SetMaterialDirty();
                                return true;
                            }
                        }
                    }

#else
                    image.sprite =Base.ConstDef.TransparencySprite;
                    image.SetMaterialDirty();
#endif
                }
            }

            if (pObj is UnityEngine.SpriteRenderer)
            {
                UnityEngine.SpriteRenderer spriteRender = (UnityEngine.SpriteRenderer)pObj;
                if (spriteRender == null)
                    return false;
                Sprite spr = pAssetRef.GetOrigin<Sprite>();
                if (spr)
                {
                    spriteRender.sprite = spr;
                    return true;
                }
                else
                {
#if UNITY_EDITOR
                    if (FileSystemUtil.GetStreamType() <= EFileSystemType.AssetData)
                    {
                        Texture2D texture2d = pAssetRef.GetOrigin() as Texture2D;
                        if (texture2d)
                        {
                            UnityEditor.TextureImporter import = UnityEditor.AssetImporter.GetAtPath(pAssetRef.Path) as UnityEditor.TextureImporter;
                            if (import && import.textureType != UnityEditor.TextureImporterType.Sprite)
                                Debug.LogError("请将" + pAssetRef.Path + "  图片转为Sprite");
                            Vector4 border = Vector4.zero;
                            if (spriteRender.sprite != null) border = spriteRender.sprite.border;
                            Sprite sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect);
                            if (sprite)
                            {
                                spriteRender.sprite = sprite;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        spriteRender.sprite = Base.ConstDef.TransparencySprite;
                    }

#else
                     spriteRender.sprite =Base.ConstDef.TransparencySprite;
#endif
                }
            }

            return false;
        }
        //------------------------------------------------------
        public bool IsInstanced(string strFile)
        {
            if (string.IsNullOrEmpty(strFile) || m_vInstance == null) return false;
            for(int i =0; i < m_vInstance.Count; )
            {
                if (m_vInstance[i].pAble != null && m_vInstance[i].pAble.IsRecyled())
                {
                    continue;
                }
                if (strFile.CompareTo(m_vInstance[i].strFile)==0)
                {
                    return true;
                }
                ++i;
            }
            return false;
        }
        //------------------------------------------------------
        public bool IsInstanced(GameObject pPrefab)
        {
            if (pPrefab  == null|| m_vInstance == null) return false;
            for (int i = 0; i < m_vInstance.Count; )
            {
                if (m_vInstance[i].pAble != null && m_vInstance[i].pAble.IsRecyled())
                {
                    continue;
                }
                if (pPrefab == m_vInstance[i].pPrefab)
                {
                    return true;
                }
                ++i;
            }
            return false;
        }
        //------------------------------------------------------
        public bool IsInstanced(IUserData pPtr)
        {
            if (pPtr == null || m_vInstance == null) return false;
            for (int i = 0; i < m_vInstance.Count;)
            {
                if (m_vInstance[i].pAble != null && m_vInstance[i].pAble.IsRecyled())
                {
                    continue;
                }
                if (pPtr == m_vInstance[i].userPtr)
                {
                    return true;
                }
                ++i;
            }
            return false;
        }
        //------------------------------------------------------
        public void RemoveInstanced(string strFile, int checkCnt = 1)
        {
            if (string.IsNullOrEmpty(strFile) || m_vInstance == null) return;
            int count = 0;
            InstanceData instData;
            for (int i = 0; i < m_vInstance.Count;)
            {
                instData = m_vInstance[i];
                if (instData.pAble != null && instData.pAble.IsRecyled())
                {
                    m_vInstance.RemoveAt(i);
                    continue;
                }
                if (strFile.CompareTo(instData.strFile) == 0)
                {
                    m_bLockedInstance = true;
                    instData.Destroy();
                    m_vInstance.RemoveAt(i);
                    m_bLockedInstance = false;
                    count++;
                    if (checkCnt > 0 && count >= checkCnt) break;
                    continue;
                }
                ++i;
            }
        }
        //------------------------------------------------------
        public void RemoveInstanced(GameObject pPrefab, int checkCnt = 1)
        {
            if (pPrefab  == null || m_vInstance == null) return;
            int count = 0;
            InstanceData instData;
            for (int i = 0; i < m_vInstance.Count; )
            {
                instData = m_vInstance[i];
                if (instData.pAble != null && instData.pAble.IsRecyled())
                {
                    m_vInstance.RemoveAt(i);
                    continue;
                }
                if (pPrefab == instData.pPrefab)
                {
                    m_bLockedInstance = true;
                    instData.Destroy();
                    m_vInstance.RemoveAt(i);
                    m_bLockedInstance = false;
                    count++;
                    if (checkCnt > 0 && count >= checkCnt) break;
                    continue;
                }
                ++i;
            }
        }
        //------------------------------------------------------
        public void RemoveInstanced(IUserData pPtr, int checkCnt = 1)
        {
            if (pPtr == null || m_vInstance == null) return;
            int count = 0;
            InstanceData instData;
            for (int i = 0; i < m_vInstance.Count;)
            {
                instData = m_vInstance[i];
                if (instData.pAble != null && instData.pAble.IsRecyled())
                {
                    m_vInstance.RemoveAt(i);
                    continue;
                }
                if (pPtr == instData.userPtr)
                {
                    m_bLockedInstance = true;
                    instData.Destroy();
                    m_vInstance.RemoveAt(i);
                    m_bLockedInstance = false;
                    count++;
                    if (checkCnt>0 && count >= checkCnt) break;
                    continue;
                }
                ++i;
            }
        }
        //------------------------------------------------------
        public InstanceOperiaon LoadInstance(uint key, Transform pParent, bool bAsync = true, Action<InstanceOperiaon> OnCallback = null, IUserData userPtr = null)
        {
            string strFile = Base.LocalizationUtil.Convert(key);
            return LoadInstance(strFile, pParent, bAsync, OnCallback, userPtr);
        }
        //------------------------------------------------------
        public InstanceOperiaon LoadInstance(string strFile, Transform pParent, bool bAsync=true, Action<InstanceOperiaon> OnCallback = null, IUserData userPtr = null)
        {
            if (string.IsNullOrEmpty(strFile) || pParent == null)
                return null;
            InstanceOperiaon pOp = FileSystemUtil.SpawnInstance(strFile, bAsync);
            if(pOp!=null)
            {
                pOp.pByParent = pParent;
                pOp.OnCallback = OnSpawnInstance;
                pOp.OnSign = OnSpawnInstanceSign;
                InstanceData data = new InstanceData();
                data.userPtr = userPtr;
                data.pOperiaon = pOp;
                data.strFile = strFile;
                data.pAble = null;
                data.onCallback = OnCallback;
                if (m_vInstance == null) m_vInstance = new List<InstanceData>(4);
                m_vInstance.Add(data);
                return pOp;
            }
            return null;
        }
        //------------------------------------------------------
        public InstanceOperiaon LoadInstance(GameObject pPrefab, Transform pParent, bool bAsync = true, Action<InstanceOperiaon> OnCallback = null, IUserData userPtr = null)
        {
            if (pPrefab == null || pParent == null)
                return null;
            InstanceOperiaon pOp =FileSystemUtil.SpawnInstance(pPrefab, bAsync);
            if (pOp != null)
            {
                pOp.pByParent = pParent;
                pOp.OnCallback = OnSpawnInstance;
                pOp.OnSign = OnSpawnInstanceSign;
                InstanceData data = new InstanceData();
                data.userPtr = userPtr;
                data.pOperiaon = pOp;
                data.pPrefab = pPrefab;
                data.pAble = null;
                data.onCallback = OnCallback;
                if (m_vInstance == null) m_vInstance = new List<InstanceData>(4);
                m_vInstance.Add(data);
                return pOp;
            }
            return null;
        }
        //------------------------------------------------------
        void OnSpawnInstance(InstanceOperiaon pCallback)
        {
            InstanceData instData;
            for(int i = 0; i< m_vInstance.Count; ++i)
            {
                instData = m_vInstance[i];
                if(instData.pOperiaon == pCallback)
                {
                    instData.pAble = pCallback.pPoolAble;
                    instData.pOperiaon = null;
                    m_vInstance[i] = instData;

                    if (instData.pAble!=null)
                    {
                        BaseUtil.ResetGameObject(instData.pAble.gameObject, EResetType.All);
                        if (instData.pAble.GetTransorm() is RectTransform)
                        {
                            RectTransform rect = instData.pAble.GetTransorm() as RectTransform;
                            rect.localScale = Vector3.one;
                            rect.anchoredPosition3D = Vector2.zero;
                            rect.offsetMax = Vector3.zero;
                            rect.offsetMin = Vector3.zero;
                            rect.anchorMin = Vector2.zero;
                            rect.anchorMax = Vector2.one;
                        }

                        instData.pAble.gameObject.SetActive(true);
                        instData.pAble.RegisterCallback(this);
                        OnSpawInstanced(instData.pAble);
                    }
                    instData.onCallback?.Invoke(pCallback);
                    return;
                }
            }
            if(pCallback.pPoolAble) pCallback.pPoolAble.RecyleDestroy();
        }
        //------------------------------------------------------
        void OnSpawnInstanceSign(InstanceOperiaon pCallback)
        {
            pCallback.bUsed = LoadInstanceSignCheck();
        }
        //------------------------------------------------------
        protected virtual void OnSpawInstanced(AInstanceAble pAble)
        {

        }
        //------------------------------------------------------
        public virtual void OnInstanceCallback(AInstanceAble pAble, EInstanceCallbackType eType)
        {
            if (m_bLockedInstance) return;
            if (eType == EInstanceCallbackType.Destroy || eType == EInstanceCallbackType.Recyled)
            {
                for (int i = 0; i < m_vInstance.Count; ++i)
                {
                    if (pAble == m_vInstance[i].pAble)
                    {
                        m_vInstance.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        //------------------------------------------------------
        public virtual void ClearLoaded()
        {
            foreach (var db in m_vDynamicLoad)
            {
                if (db.Key == null) continue;
                if (db.Value.pAsset != null)
                    db.Value.pAsset.Release();
                if (db.Value.assetOperiaon != null)
                    db.Value.assetOperiaon.Clear();

                if (db.Key is UnityEngine.UI.RawImage)
                {
                    UnityEngine.UI.RawImage image = (UnityEngine.UI.RawImage)db.Key;
                    image.texture = Base.ConstDef.TransparencyTexture;
                }
                else if (db.Key is UnityEngine.UI.Image)
                {
                    UnityEngine.UI.Image image = (UnityEngine.UI.Image)db.Key;
                    image.sprite = Base.ConstDef.TransparencySprite;
                }
                else if (db.Key is UnityEngine.SpriteRenderer)
                {
                    UnityEngine.SpriteRenderer spriteRender = (UnityEngine.SpriteRenderer)db.Key;
                    spriteRender.sprite = Base.ConstDef.TransparencySprite;
                }
            }
            m_vDynamicLoad.Clear();

            m_bLockedInstance = true;
            if (m_vInstance!=null)
            {
                for (int i = 0; i < m_vInstance.Count; ++i)
                {
                    m_vInstance[i].Destroy();
                }
                m_vInstance.Clear();
            }
            m_bLockedInstance = false;
        }
    }
}

