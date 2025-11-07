/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	AssetBundleInfo
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Core
{
    //------------------------------------------------------
    public class AssetBundleInfo
    {
        public string abName = null;
        public string abPath = null;
        public AssetBundle assetbundle = null;
        public int refCnt = 0;
#if UNITY_EDITOR
        public List<string> RefBy = new List<string>();
#endif
        private bool m_bPermanent;
        public bool bAsync = false;
        public AsyncOperation asyncRequest = null;

        public bool bReqing = false;

        internal int lastTime = 0;

        public Package package = null;

        public PackageStream packageStream = null;
        public string[] dependAbNames = null;
        public List<AssetBundleInfo> depends = null;
        bool m_binitDeps = false;

        AFileSystem m_pFileSystem =null;
        public bool initedDeps
        {
            get { return m_binitDeps; }
            set { m_binitDeps = value; }
        }
        //------------------------------------------------------
        public bool permanent
        {
            get { return m_bPermanent; }
        }
        //------------------------------------------------------
        public AssetBundleInfo(AFileSystem fileSystem)
        {
            m_pFileSystem = fileSystem;
        }
        //------------------------------------------------------
        public float GetProgress()
        {
            if (assetbundle != null) return 1;
            float progress = 0;
            if(bAsync)
            {
                if (asyncRequest == null) return 0;

                progress += asyncRequest.progress;
                if (depends != null)
                {
                    for (int i = 0; i < depends.Count; ++i)
                    {
                        progress += depends[i].GetProgress();
                    }
                }
                return progress / ((float)depends.Count+1f);
            }
            else
            {
                if (assetbundle != null) return 1;
                return 0;
            }
        }
        //------------------------------------------------------
        public void setPermanent(bool bPermanent)
        {
            if (m_bPermanent != bPermanent && m_bPermanent)
                return;
            m_bPermanent = bPermanent;
            if (depends == null) return;
            for (int i = 0; i < depends.Count; ++i)
            {
                depends[i].setPermanent(bPermanent);
            }
        }
        //------------------------------------------------------
        public void AsyncRequest(string usedBy, bool bImmediately = false)
        {
#if !USE_SERVER
            lastTime = 0;
            if (depends != null)
            {
                for (int i = 0; i < depends.Count; ++i)
                {
                    depends[i].AsyncRequest(this.abName, bImmediately);
                }
            }
            refCnt++;
#if UNITY_EDITOR
            if (usedBy != null) RefBy.Add(usedBy);
#endif
            bReqing = true;
            if(m_pFileSystem!=null) m_pFileSystem.ReqCreateBundle(this, bImmediately);
#endif
        }
        //------------------------------------------------------
        public bool CheckAsyncLoaded()
        {
            lastTime = 0;
            if (bReqing) return false;
            bool bLoaded = true;
            if (depends != null)
            {
                for (int i = 0; i < depends.Count; ++i)
                {
                    if (!depends[i].CheckAsyncLoaded())
                    {
                        bLoaded = false;
                        break;
                    }
                }
            }

            if (!bLoaded)
                return false;

            if (this.asyncRequest != null)
            {
                if (this.asyncRequest.isDone)
                {
                    if(this.asyncRequest is AssetBundleCreateRequest)
                    {
                        AssetBundleCreateRequest abReq = this.asyncRequest as AssetBundleCreateRequest;
                        this.assetbundle = abReq.assetBundle;
                    }
                    else if(this.asyncRequest is UnityWebRequestAsyncOperation)
                    {
                        UnityWebRequestAsyncOperation abReq = this.asyncRequest as UnityWebRequestAsyncOperation;
                        this.assetbundle = DownloadHandlerAssetBundle.GetContent(abReq.webRequest);
                    }
                    this.asyncRequest = null;
                }
                else
                    bLoaded = false;
            }

            return bLoaded;
        }
        //------------------------------------------------------
        public void LoadAndUsed(string useBy, bool bImmediately = false)
        {
#if !USE_SERVER
            lastTime = 0;
            if (depends != null)
            {
                for (int i = 0; i < depends.Count; ++i)
                {
                    depends[i].LoadAndUsed(this.abName, bImmediately);
                }
            }
            refCnt++;
#if UNITY_EDITOR
            if (useBy != null) RefBy.Add(useBy);
#endif
            bReqing = true;
            if(m_pFileSystem!=null) m_pFileSystem.ReqCreateBundle(this, bImmediately);
#endif
        }
        //------------------------------------------------------
        public bool CheckLoaded()
        {
            lastTime = 0;
            if (bReqing) return false;
            bool bHasLoaded = true;
            if (depends != null)
            {
                for (int i = 0; i < depends.Count; ++i)
                {
                    if (!depends[i].CheckLoaded())
                    {
                        bHasLoaded = false;
                        break;
                    }
                }
            }
            if (!bHasLoaded)
                return false;

            if (this.asyncRequest != null)
            {
                if (this.asyncRequest.isDone)
                {
                    if (this.asyncRequest is AssetBundleCreateRequest)
                    {
                        AssetBundleCreateRequest abReq = this.asyncRequest as AssetBundleCreateRequest;
                        this.assetbundle = abReq.assetBundle;
                    }
                    else if (this.asyncRequest is UnityWebRequestAsyncOperation)
                    {
                        UnityWebRequestAsyncOperation abReq = this.asyncRequest as UnityWebRequestAsyncOperation;
                        this.assetbundle = DownloadHandlerAssetBundle.GetContent(abReq.webRequest);
                    }
                    this.asyncRequest = null;
                }
                else
                    bHasLoaded = false;
            }
            return bHasLoaded;
        }
        //------------------------------------------------------
        public bool CheckDispose()
        {
            if (refCnt > 0)
                return true;

            if (this.m_bPermanent)
                return true;

            if(this.asyncRequest!=null)
            {
                if (this.asyncRequest.isDone)
                {
                    if (this.asyncRequest is AssetBundleCreateRequest)
                    {
                        AssetBundleCreateRequest abReq = this.asyncRequest as AssetBundleCreateRequest;
                        if (abReq.assetBundle) assetbundle.Unload(true);
                    }
                    else if (this.asyncRequest is UnityWebRequestAsyncOperation)
                    {
                        UnityWebRequestAsyncOperation abReq = this.asyncRequest as UnityWebRequestAsyncOperation;
                        var tempAssetbundle = DownloadHandlerAssetBundle.GetContent(abReq.webRequest);
                        if (tempAssetbundle) tempAssetbundle.Unload(true);
                    }
                }
                else
                    return false;
            }
            return true;
        }
        //------------------------------------------------------
        public void Unload(bool bUnload= true, string useBy =null)
        {
            --refCnt;

            if (depends != null)
            {
                for (int i = 0; i < depends.Count; ++i)
                {
                    depends[i].Unload(bUnload,this.abName);
                }
            }
#if UNITY_EDITOR
            if(!string.IsNullOrEmpty(useBy))
            RefBy.Remove(useBy);
#endif
            if (refCnt > 0) return;

#if UNITY_EDITOR
            RefBy.Clear();
#endif
            if(packageStream!=null)
            {
                packageStream.Seek(0, System.IO.SeekOrigin.Begin);
            }
            bReqing = true;
            refCnt = 0;
            lastTime = 0;
            if (!m_bPermanent)
            {
                if (assetbundle) assetbundle.Unload(bUnload);
                assetbundle = null;

                if(this.asyncRequest!=null)
                {
                    if (m_pFileSystem != null) m_pFileSystem.ReleaseAsyncRequest(this);
                }
            }
        }
    }
}

