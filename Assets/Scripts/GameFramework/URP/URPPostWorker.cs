#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	AURPPostWorker
作    者:	HappLI
描    述:	URP
*********************************************************************/
using System.Collections.Generic;
using Framework.Base;
using Framework.Core;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace Framework.URP
{
    public enum EPostPassType
    {
        ForceCustomOpaque,
        ForceCustomTransparent,
        GpuSkin,
        Grab,
        Screen,
        Vignette,
        OultineOpaque,
        OultineTransparent,
        PlaneShadow,
        DepthPass,
        VolumeProcessPostOutline,
        [DisableGUI]Count,
    }

    public interface IURPPassWorkerCallback
    {
        void OnURPBuildPassLogic(APostRenderPass pass, List<APostLogic> vLogics);
    }

    public interface IBasePostPassRender
    { 
        URP.EPostPassType GetPassType();
    }
    //------------------------------------------------------
    public partial class URPPostWorker : AModule
    {
        class RenderPost
        {
            public bool isActived = false;
            public List<APostLogic> posts = null;

            public List<IPostPassRender> renderPass = null;
            public void ClearReset()
            {
                if (posts == null) return;
                for (int j = 0; j < posts.Count; ++j)
                    posts[j].ClearReset();
            }
        }
        RenderPost[] m_vPosts = new RenderPost[(int)EPostPassType.Count];
        static URPPostWorker sm_pInstance = null;
        private HashSet<IURPPassWorkerCallback> m_vCallback = null;
        //------------------------------------------------------
        public static URPPostWorker getInstance()
        {
            if (sm_pInstance == null)
            {
                return sm_pInstance;
            }

            return sm_pInstance;
        }
        protected uint m_nEnablePass = 0xffffffff;
        //------------------------------------------------------
        protected override void OnAwake()
        {
            sm_pInstance = this;
            m_nEnablePass = 0xffffffff;
        }
        //------------------------------------------------------
        protected override void OnDestroy()
        {
            sm_pInstance = null;
            m_nEnablePass = 0xffffffff;
            for (int i = 0; i < m_vPosts.Length; ++i)
            {
                if (m_vPosts[i] == null) continue;
                if (m_vPosts[i].posts == null) continue;
                for (int j = 0; j < m_vPosts[i].posts.Count; ++j)
                    m_vPosts[i].posts[j].Destroy();
                m_vPosts[i].posts.Clear();
            }
        }
        //------------------------------------------------------
        public void RegisterCallback(IURPPassWorkerCallback callback)
        {
            if (callback == null)
                return;
            if (m_vCallback == null) m_vCallback = new HashSet<IURPPassWorkerCallback>(2);
            m_vCallback.Add(callback);
        }
        //------------------------------------------------------
        public void UnRegisterCallback(IURPPassWorkerCallback callback)
        {
            if (callback == null || m_vCallback == null)
                return;
            m_vCallback.Remove(callback);
        }
        //------------------------------------------------------
        public T GetPostPass<T>(EPostPassType type) where T : APostLogic
        {
            if (type >= EPostPassType.Count) return null;
            RenderPost renderPost = m_vPosts[(int)type];
            if (renderPost == null || renderPost.posts == null) return null;
            T logic;
            for (int i = 0; i < renderPost.posts.Count; ++i)
            {
                logic = renderPost.posts[i] as T;
                if (logic != null) return logic;
            }
            return null;
        }
        //------------------------------------------------------
        public static void SetPassFlags(uint passFlags)
        {
            if (sm_pInstance == null) return;
            if (sm_pInstance.m_nEnablePass == passFlags)
                return;
            uint prePass = sm_pInstance.m_nEnablePass;
            sm_pInstance.m_nEnablePass = passFlags;
            for (int i =0; i < (int)EPostPassType.Count; ++i)
            {
                bool bToggle = (passFlags & (1 << i)) != 0;
                if ( ((prePass & (1<<i)) !=0) == bToggle) continue;
                sm_pInstance.OnTogglePass((EPostPassType)i, bToggle);
            }
        }
        //------------------------------------------------------
        public static bool IsEnabledPass(EPostPassType passType)
        {
#if UNITY_EDITOR
            if (!AFramework.isStartup) return true;
#endif
            if (passType == EPostPassType.Count || sm_pInstance == null) return false;
            return (sm_pInstance.m_nEnablePass & (1 << (int)passType)) != 0;
        }
        //------------------------------------------------------
        public static void EnablePass(EPostPassType passType, bool enable)
        {
            if (sm_pInstance == null) return;
            if (enable)
            {
                if (enable) sm_pInstance.m_nEnablePass |= (uint)(1 << (int)passType);
                else sm_pInstance.m_nEnablePass &= ~(uint)(1 << (int)passType);
                sm_pInstance.OnTogglePass(passType, enable);
            }
        }
        //------------------------------------------------------
        protected void OnTogglePass(EPostPassType passType, bool enable)
        {
            if (passType == EPostPassType.Grab)
                GrabPassFeature.OnTogglePass(enable);
            else if (passType == EPostPassType.DepthPass)
                DepthPassFeature.OnTogglePass(enable);
        }
        //------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
            RenderPost renderPost;
            for (int i = 0; i < m_vPosts.Length; ++i)
            {
                renderPost = m_vPosts[i];
                if (renderPost == null || renderPost.posts == null) continue;
                for (int j = 0; j < renderPost.posts.Count; ++j)
                    renderPost.posts[j].Update(fFrame);
            }
        }
        //------------------------------------------------------
        protected void OnRegisterPostRenderPass(IBasePostPassRender postPass)
        {
            int index = (int)postPass.GetPassType();
            if (index < 0 || index >= m_vPosts.Length || !(postPass is IPostPassRender)) return;
            IPostPassRender post = postPass as IPostPassRender;
            if (m_vPosts[index] == null) m_vPosts[index] = new RenderPost();
            if (m_vPosts[index].renderPass == null)
                m_vPosts[index].renderPass = new List<IPostPassRender>(2);
            if (m_vPosts[index].renderPass.Contains(post)) return;
            m_vPosts[index].renderPass.Add(post);
        }
        //------------------------------------------------------
        protected void OnUnRegisterPostRenderPass(IBasePostPassRender postPass)
        {
            URPPostWorker urpWork = getInstance() as URPPostWorker;
            if (urpWork == null) return;
            int index = (int)postPass.GetPassType();
            if (index < 0 || index >= m_vPosts.Length || !(postPass is IPostPassRender)) return;
            if (m_vPosts[index] == null) return;
            if (urpWork.m_vPosts[index].renderPass == null) return;
            IPostPassRender post = postPass as IPostPassRender;
            urpWork.m_vPosts[index].renderPass.Remove(post);
        }
        //------------------------------------------------------
        public static T CastPostPass<T>(EPostPassType type) where T : APostLogic
        {
            URPPostWorker urpWork = getInstance() as URPPostWorker;
            if (urpWork == null) return null;

            return urpWork.GetPostPass<T>(type);
        }
        //------------------------------------------------------
        public static void OnCreateRenderPass(APostRenderPass pass)
        {
            if (pass == null) return;
            URPPostWorker urpWork = getInstance() as URPPostWorker;
            if (urpWork == null) return;
            RenderPost renderPost = urpWork.m_vPosts[(int)pass.GetPostPassType()];
            if(renderPost == null)
            {
                renderPost = new RenderPost();
                if (renderPost.posts == null) renderPost.posts = new List<APostLogic>(2);
                urpWork.m_vPosts[(int)pass.GetPostPassType()] = renderPost;
                if (urpWork.m_vCallback != null)
                {
                    foreach (var db in urpWork.m_vCallback)
                    {
                        db.OnURPBuildPassLogic(pass,renderPost.posts);
                    }
                }
                urpWork.RegisterLogics(pass);
            }
        }
        //------------------------------------------------------
        public static void RegisterPostRenderPass(IBasePostPassRender postPass)
        {
            if (sm_pInstance == null) return;
            sm_pInstance.OnRegisterPostRenderPass(postPass);
        }
        //------------------------------------------------------
        public static void UnRegisterPostRenderPass(IBasePostPassRender postPass)
        {
            if (sm_pInstance == null) return;
            sm_pInstance.OnUnRegisterPostRenderPass(postPass);
        }
        //------------------------------------------------------
        public static void OnRenderPassExecude(APostRenderPass pass, CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (pass == null) return;
            EPostPassType postType = pass.GetPostPassType();
            if (postType >= EPostPassType.Count) return;
            URPPostWorker urpWork = getInstance() as URPPostWorker;
            if (urpWork == null) return;
            RenderPost renderPost = urpWork.m_vPosts[(int)postType];
            if (renderPost == null) return;
            renderPost.isActived = pass.IsActived();

            if (renderPost.renderPass != null)
            {
                for (int i = 0; i < renderPost.renderPass.Count; ++i)
                {
                    renderPost.renderPass[i].OnRenderPassExecute(cmd, context, ref renderingData);
                }
            }

            if (renderPost.posts == null) return;
            for (int i = 0; i < renderPost.posts.Count; ++i)
            {
                renderPost.posts[i].Excude(pass, cmd, context, ref renderingData);
            }
        }
        //------------------------------------------------------
        public static void OnRenderPassSetup(APostRenderPass pass, RenderTargetIdentifier source)
        {
            if (pass == null) return;
            EPostPassType postType = pass.GetPostPassType();
            if (postType >= EPostPassType.Count) return;
            URPPostWorker urpWork = getInstance() as URPPostWorker;
            if (urpWork == null) return;

            RenderPost renderPost = urpWork.m_vPosts[(int)postType];
            if (renderPost == null) return;
            renderPost.isActived = pass.IsActived();

            if (renderPost.renderPass != null)
            {
                for (int i = 0; i < renderPost.renderPass.Count; ++i)
                {
                    renderPost.renderPass[i].OnSetup(source);
                }
            }

            if (renderPost.posts == null) return;
            for (int i = 0; i < renderPost.posts.Count; ++i)
            {
                renderPost.posts[i].Setup(pass, source);
            }
        }
        //------------------------------------------------------
        public static void OnRenderPassFrameCleanup(APostRenderPass pass, CommandBuffer cmd)
        {
            if (pass == null) return;
            EPostPassType postType = pass.GetPostPassType();
            if (postType >= EPostPassType.Count) return;
            URPPostWorker urpWork = getInstance() as URPPostWorker;
            if (urpWork == null) return;

            RenderPost renderPost = urpWork.m_vPosts[(int)postType];
            if (renderPost == null) return;
            renderPost.isActived = pass.IsActived();

            if (renderPost.renderPass != null)
            {
                for (int i = 0; i < renderPost.renderPass.Count; ++i)
                {
                    renderPost.renderPass[i].OnFrameCleanup(cmd);
                }
            }

            if (renderPost.posts == null) return;
            for (int i = 0; i < renderPost.posts.Count; ++i)
            {
                renderPost.posts[i].FrameCleanup(pass, cmd);
            }
        }
        //------------------------------------------------------
        public static void ClearReset()
        {
            URPPostWorker urpWork = getInstance() as URPPostWorker;
            if (urpWork == null) return;
            RenderPost renderPost;
            for (int i = 0; i < urpWork.m_vPosts.Length; ++i)
            {
                renderPost = urpWork.m_vPosts[i];
                if (renderPost == null) continue;
                renderPost.ClearReset();
            }
        }
    }
}
#endif