/********************************************************************
生成日期:	24:3:2020   16:59
类    名: 	WebHandler
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Net
{
    public class WebHandler : MonoBehaviour
    {
        static WebHandler ms_pInstance = null;
        enum EReqState
        {
            None,
            Reqing,
            Succeed,
            Failed,
        }
        struct ReqData : System.IComparable<ReqData>
        {
            public short repeatTimes;
            public int timeout;
            public System.Action<string> onCallback;
            public string url;

            public int CompareTo(ReqData other)
            {
                return url.CompareTo(other.url);
            }
            public static ReqData DEF = new ReqData() { url = null, repeatTimes = -1, onCallback = null };
            public bool IsValid()
            {
                return repeatTimes > 0;
            }
        }
        struct PostData : System.IComparable<PostData>
        {
            public short repeatTimes;
            public int timeout;
            public string content;
            public System.Action<string> onCallback;
            public string url;

            public int CompareTo(PostData other)
            {
                return url.CompareTo(other.url);
            }
            public static PostData DEF = new PostData() { url = null, repeatTimes = -1, onCallback = null };
            public bool IsValid()
            {
                return repeatTimes > 0;
            }
        }
        Queue<ReqData> m_vGet = null;
        Queue<PostData> m_vPost = null;
        Queue<PostData> m_vPut = null;

        System.Action<string> m_pPosyCallback = null;
        //------------------------------------------------------
        void Awake()
        {
            Init(this);
        }
        //------------------------------------------------------
        private void OnDestroy()
        {
            ms_pInstance = null;
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_vGet.Clear();
            m_vPost.Clear();
            m_vPut.Clear();
        }
        //------------------------------------------------------
        public static void Init(WebHandler handle)
        {
            ms_pInstance = handle;
            if (handle != null)
            {
                if(handle.m_vGet == null)
                handle.m_vGet = new Queue<ReqData>();
                if (handle.m_vPost == null)
                    handle.m_vPost = new Queue<PostData>();
                if (handle.m_vPut == null)
                    handle.m_vPut = new Queue<PostData>();
            }
        }
        //------------------------------------------------------
        public static void SetPosCallback(System.Action<string> vCallback)
        {
            if (ms_pInstance == null) return;
            ms_pInstance.m_pPosyCallback = vCallback;
        }
        //------------------------------------------------------
        public static bool PosHttp(string url, string content, System.Action<string> onCallback = null, short repeat = 3, int timeout = 10)
        {
            if (ms_pInstance == null) return false;
            ms_pInstance.PostHttp(url, content, onCallback, repeat, timeout);
            return false;
        }
        //------------------------------------------------------
        public static bool PutHttp(string url, string content, System.Action<string> onCallback = null, short repeat = 3, int timeout = 10)
        {
            if (ms_pInstance == null) return false;
            ms_pInstance.InnerPutHttp(url, content, onCallback, repeat, timeout);
            return false;
        }
        //------------------------------------------------------
        public static bool ReqHttp(string url, System.Action<string> onCallback, byte reqRepeat = 3, int timeout = 5)
        {
            if (ms_pInstance == null) return false;
            ms_pInstance.ReadHttp(url, onCallback, reqRepeat, timeout,null);
            return true;
        }
        //------------------------------------------------------
        public static bool ReqHttp(string url, string content, System.Action<string> onCallback, byte reqRepeat = 3, int timeout = 5)
        {
            if (ms_pInstance == null) return false;
            ms_pInstance.ReadHttp(url, onCallback, reqRepeat, timeout, content);
            return true;
        }
        //------------------------------------------------------
        void PostHttp(string url, string content, System.Action<string> onCallback, short repeat = 3, int timeout = 10)
        {
            if (string.IsNullOrEmpty(url))
            {
                Framework.Base.Logger.Warning("post http url is null");
                if (onCallback != null)
                    onCallback("Faild");
                return;
            }
            PostData reqData = new PostData();
            reqData.url = url;
            if (m_vPost.Contains(reqData))
            {
                return;
            }
            reqData.content = content;
            reqData.onCallback = onCallback;
            reqData.repeatTimes = repeat;
            reqData.timeout = timeout;
            m_vPost.Enqueue(reqData);

            StartCoroutine(HttpPostCoroutine());
        }
        //------------------------------------------------------
        void InnerPutHttp(string url, string content, System.Action<string> onCallback, short repeat = 3, int timeout = 10)
        {
            if (string.IsNullOrEmpty(url))
            {
                Framework.Base.Logger.Warning("put http url is null");
                if (onCallback != null)
                    onCallback("Faild");
                return;
            }
            PostData reqData = new PostData();
            reqData.url = url;
            if (m_vPut.Contains(reqData))
            {
                return;
            }
            reqData.content = content;
            reqData.onCallback = onCallback;
            reqData.repeatTimes = repeat;
            reqData.timeout = timeout;
            m_vPut.Enqueue(reqData);

            StartCoroutine(HttpPutCoroutine());
        }
        //------------------------------------------------------
        void ReadHttp(string url, System.Action<string> onCallback, byte reqRepeat = 3, int timeout = 5, string content = null)
        {
            if(string.IsNullOrEmpty(url))
            {
                Framework.Base.Logger.Warning("request http url is null");
                if (onCallback!=null)
                    onCallback("Faild");
                return;
            }
            ReqData reqData = new ReqData();
            reqData.url = url;
            if (!string.IsNullOrEmpty(content))
                reqData.url = BaseUtil.stringBuilder.Append(reqData.url).Append("?").Append(content).ToString();
            if (m_vGet.Contains(reqData))
            {
                return;
            }

            reqData.onCallback = onCallback;
            reqData.repeatTimes = reqRepeat;
            reqData.timeout = timeout;
            m_vGet.Enqueue(reqData);

            StartCoroutine(HttpCoroutine());
        }
        //------------------------------------------------------
        IEnumerator HttpCoroutine()
        {
            if (m_vGet.Count > 0)
            {
                ReqData reqData = m_vGet.Dequeue();
                bool bSucceed = false;
                while (reqData.repeatTimes > 0)
                {
                    reqData.repeatTimes--;
                    UnityWebRequest req = UnityWebRequest.Get(reqData.url);
                    req.timeout = reqData.timeout;
                    var asyncOp = req.SendWebRequest();
                    while (!asyncOp.isDone)
                    {
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(req.error) || (req.downloadedBytes <= 0))
                    {
                        if (!string.IsNullOrEmpty(req.error))
                        {
                            Framework.Base.Logger.Warning("http request error:[" + reqData.url + "] error:" + req.error);
                            //         reqData.onCallback("Fail");
                        }
                        else
                        {
                            Framework.Base.Logger.Warning("can't not request http:" + reqData.url);
                            //        reqData.onCallback("Fail");
                        }
                        req.Dispose();

                        yield return new WaitForSeconds(1);
                    }
                    else if (req.downloadHandler.text.StartsWith("Fail:"))
                    {
                        reqData.onCallback("Fail");
                        bSucceed = true;
                    }
                    else
                    {
                        reqData.onCallback(req.downloadHandler.text);
                        bSucceed = true;
                    }

                    req.Dispose();

                    if (bSucceed) break;
                }
                if (!bSucceed)
                {
                    if (reqData.onCallback != null)
                        reqData.onCallback("Fail");
                }
            }
            if (m_vGet.Count > 0)
                yield return HttpCoroutine();
        }
        //------------------------------------------------------
        IEnumerator HttpPostCoroutine()
        {
            if (m_vPost.Count > 0)
            {
                PostData reqData = m_vPost.Dequeue();
                bool bSucceed = false;
                while (reqData.repeatTimes > 0)
                {
                    reqData.repeatTimes--;
                    UnityWebRequest req = UnityWebRequest.Put(reqData.url, System.Text.Encoding.UTF8.GetBytes(reqData.content));
                    req.timeout = reqData.timeout;
                    req.method = UnityWebRequest.kHttpVerbPOST;
                    //req.SetRequestHeader("Content-Type", "application/octet-stream;charset=utf-8");
                    req.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
                    var asyncOp = req.SendWebRequest();
                    while (!asyncOp.isDone)
                    {
                        yield return null;
                    }
                    if (!string.IsNullOrEmpty(req.downloadHandler.text))
                        Debug.Log("content:" + req.downloadHandler.text);
                    if (!string.IsNullOrEmpty(req.error) || (req.downloadedBytes <= 0))
                    {
                        if (!string.IsNullOrEmpty(req.error))
                        {
                            Debug.LogWarning("http request error:[" + reqData.url + "] error:" + req.error);
                            //         reqData.onCallback("Fail");
                        }
                        else
                        {
                            Debug.LogWarning("can't not request http:" + reqData.url);
                            //        reqData.onCallback("Fail");
                        }
                        req.Dispose();

                        yield return new WaitForSeconds(1);
                    }
                    else if (req.downloadHandler.text.StartsWith("Fail:"))
                    {
                        if (reqData.onCallback != null) reqData.onCallback("Fail");
                        bSucceed = true;
                    }
                    else if (req.downloadHandler.text.StartsWith("Error:"))
                    {
                        Debug.LogWarning(req.downloadHandler.text);
                        if (reqData.onCallback != null) reqData.onCallback("Fail");
                        bSucceed = true;
                    }
                    else
                    {
                        if (m_pPosyCallback != null) m_pPosyCallback(req.downloadHandler.text);
                        if (reqData.onCallback != null) reqData.onCallback(req.downloadHandler.text);
                        bSucceed = true;
                    }

                    req.Dispose();

                    if (bSucceed) break;
                }
                if (!bSucceed)
                {
                    if (reqData.onCallback != null)
                        reqData.onCallback("Fail");
                }
            }
            if (m_vPost.Count>0)
                yield return HttpPostCoroutine();
        }
        //------------------------------------------------------
        IEnumerator HttpPutCoroutine()
        {
            if (m_vPut.Count > 0)
            {
                PostData reqData = m_vPut.Dequeue();
                bool bSucceed = false;
                while (reqData.repeatTimes > 0)
                {
                    reqData.repeatTimes--;
                    UnityWebRequest req = UnityWebRequest.Put(reqData.url, System.Text.Encoding.UTF8.GetBytes(reqData.content));
                    req.timeout = reqData.timeout;
                    req.method = UnityWebRequest.kHttpVerbPUT;
                    //req.SetRequestHeader("Content-Type", "application/octet-stream;charset=utf-8");
                    req.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
                    var asyncOp = req.SendWebRequest();
                    while (!asyncOp.isDone)
                    {
                        yield return null;
                    }
                    if (!string.IsNullOrEmpty(req.error) || (req.downloadedBytes <= 0))
                    {
                        if (!string.IsNullOrEmpty(req.error))
                        {
                            Debug.LogWarning("http request error:[" + reqData.url + "] error:" + req.error);
                            //         reqData.onCallback("Fail");
                        }
                        else
                        {
                            Debug.LogWarning("can't not request http:" + reqData.url);
                            //        reqData.onCallback("Fail");
                        }
                        req.Dispose();

                        yield return new WaitForSeconds(1);
                    }
                    else if (req.downloadHandler.text.StartsWith("Fail:"))
                    {
                        if (reqData.onCallback != null) reqData.onCallback("Fail");
                        bSucceed = true;
                    }
                    else if (req.downloadHandler.text.StartsWith("Error:"))
                    {
                        Debug.LogWarning(req.downloadHandler.text);
                        if (reqData.onCallback != null) reqData.onCallback("Fail");
                        bSucceed = true;
                    }
                    else
                    {
                        if (m_pPosyCallback != null) m_pPosyCallback(req.downloadHandler.text);
                        if (reqData.onCallback != null) reqData.onCallback(req.downloadHandler.text);
                        bSucceed = true;
                    }

                    req.Dispose();

                    if (bSucceed) break;
                }
                if (!bSucceed)
                {
                    if (reqData.onCallback != null)
                        reqData.onCallback("Fail");
                }
            }
            if (m_vPut.Count > 0)
                yield return HttpPostCoroutine();
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public void EditorUpdate()
        {
            if (m_vGet != null && m_vGet.Count > 0) StartCoroutine(HttpCoroutine());
            if(m_vPost!=null && m_vPost.Count>0)StartCoroutine(HttpPostCoroutine());
            if (m_vPut != null && m_vPut.Count > 0) StartCoroutine(HttpPutCoroutine());
        }
#endif
    }
}

