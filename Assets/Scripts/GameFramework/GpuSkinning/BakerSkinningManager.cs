#if !USE_SERVER
using UnityEngine;
using System.Collections.Generic;
using Framework.Core;
#if USE_URP
using UnityEngine.Rendering.Universal;
#endif
namespace Framework.Plugin
{
    public class BakerSkinningManager : AModule, IJobUpdate
    {
        struct ReqCallback : IUserData
        {
            public System.Action<CGpuSkinMeshAgent> onCallback;
            public void Destroy()
            {
            }
        }
        private List<MaterialRef> m_MaterialFactory = new List<MaterialRef>(32);
        private Dictionary<string, SkinFrameData> m_vData = new Dictionary<string, SkinFrameData>(32);
        private LinkedList<CGpuSkinMeshAgent> m_vAgents = new LinkedList<CGpuSkinMeshAgent>();
        private Stack<IUserData> m_vRemoveAgents = new Stack<IUserData>(16);
        List<BakeSkinDrawCmd> m_vRuntimeDrawCmds = new List<BakeSkinDrawCmd>(16);
        private Matrix4x4 m_CullingMatrix = Matrix4x4.identity;
        Vector3 m_LastCameraPos = Vector3.zero;
        Vector3 m_LastCaemraEulerAngle = Vector3.zero;
        int m_nDirtyDraw = 0;
#if USE_URP
        BakerSkinningDrawer m_UrpDraw = null;
#endif
        //-----------------------------------------------------------------------------
        public void RemoveSkin(IUserData owner)
        {
            m_vRemoveAgents.Push(owner);
            if (m_nDirtyDraw <= 0) m_nDirtyDraw = 2;
        }
        //-----------------------------------------------------------------------------
        public bool LoadSkinFrameData(string strFile, ESkinType skinType, IUserData owner, System.Action<CGpuSkinMeshAgent> OnResult = null)
        {
            if (string.IsNullOrEmpty(strFile)) return false;
            for (var node = m_vAgents.First; node != null; node = node.Next)
            {
                if (node.Value.GetOwner() == owner)
                    return true;
            }
            
            SkinFrameData skinData = null;
            if (!m_vData.TryGetValue(strFile, out skinData))
            {
                AssetOperiaon pAssetOp = GetFramework().FileSystem.AsyncReadFile(strFile);
                pAssetOp.OnCallback = OnLoadSkinData;
                pAssetOp.userData = owner;
                if(OnResult!=null)
                    pAssetOp.userData1 = new ReqCallback() { onCallback = OnResult };
            }
            else
            {
                if (skinData is SkinFrameBoneData)
                    skinType = ESkinType.GpuArray;
                else if (skinData is SkinFrameMeshData)
                    skinType = ESkinType.CpuData;

                if (skinData == null)
                {
                    if (OnResult != null) OnResult(null);
                    return false;
                }
                CGpuSkinMeshAgent agent = new CGpuSkinMeshAgent(owner);
                agent.SetSkinFrameData(skinData, skinType);
                m_vAgents.AddLast(agent);
                if(m_nDirtyDraw<=0) m_nDirtyDraw = 2;
                if (OnResult != null) OnResult(agent);
            }
            return true;
        }
        //-----------------------------------------------------------------------------
        void OnLoadSkinData(AssetOperiaon pAssetOp)
        {
            if(m_vRemoveAgents.Contains(pAssetOp.userData))
            {
                pAssetOp.pAsset.Release();
                return;
            }
            ESkinType skinType = ESkinType.None;
            SkinFrameData skinData = null;
            ScriptableObject asset = pAssetOp.GetOrigin<ScriptableObject>();
            if (asset != null)
            {
                if (asset is ASkeletonGpuData)
                {
                    SkinFrameBoneData boneData = new SkinFrameBoneData();
                    boneData.boneAnimations = asset as ASkeletonGpuData;
                    boneData.getAniTex();
                    skinData = boneData;
                    skinData.assetFile = pAssetOp.strFile;
                    skinType = ESkinType.GpuArray;
                }
                else if (asset is ASkeletonCpuData)
                {
                    SkinFrameMeshData boneData = new SkinFrameMeshData();
                    boneData.animations = asset as ASkeletonCpuData;
                    skinData = boneData;
                    skinData.assetFile = pAssetOp.strFile;
                    skinType = ESkinType.CpuData;
                }
                if (skinData != null)
                {
                    skinData.getShareMat();
                    skinData.getShareMesh();
                    skinData.SetDelegate(RemoveSkinData, CreateMateral, ReleaseMaterial);

                    CGpuSkinMeshAgent agent = new CGpuSkinMeshAgent(pAssetOp.userData);
                    agent.SetSkinFrameData(skinData, skinType);
                    m_vAgents.AddLast(agent);
                    if(m_nDirtyDraw<=0) m_nDirtyDraw = 2;
                    if (pAssetOp.userData1 != null)
                    {
                        ReqCallback reqCallback = (ReqCallback)pAssetOp.userData1;
                        reqCallback.onCallback(agent);
                    }
                }
                else
                {
                    if (pAssetOp.userData1 != null)
                    {
                        ReqCallback reqCallback = (ReqCallback)pAssetOp.userData1;
                        reqCallback.onCallback(null);
                    }
                }
                m_vData.Add(skinData.assetFile, skinData);
            }
            else
            {
                pAssetOp.pAsset.Release();
                if (pAssetOp.userData1 != null)
                {
                    ReqCallback reqCallback = (ReqCallback)pAssetOp.userData1;
                    reqCallback.onCallback(null);
                }
            }
        }
        //-----------------------------------------------------------------------------
        void RemoveSkinData(SkinFrameData data)
        {
            if (m_vData.ContainsKey(data.assetFile))
            {
                m_vData.Remove(data.assetFile);
            }
        }
        //-----------------------------------------------------------------------------
        protected override void OnDestroy()
        {
            if(m_vData!=null)
            {
                foreach (var db in m_vData)
                {
                    db.Value.SetDelegate(null, null, null);
                    db.Value.clear();
                }
                m_vData.Clear();
            }

            m_MaterialFactory.Clear();
            m_vRuntimeDrawCmds.Clear();
            m_nDirtyDraw = 0;
            m_LastCameraPos = Vector3.zero;
            m_LastCaemraEulerAngle = Vector3.zero;
            m_vRemoveAgents.Clear();
            m_vAgents.Clear();
        }
        //-----------------------------------------------------------------------------
        void ReleaseMaterial(MaterialRef mat)
        {
            for (int i = 0; i < m_MaterialFactory.Count; ++i)
            {
                if (m_MaterialFactory[i] == mat)
                {
                    m_MaterialFactory[i].refIndex--;
                    if (m_MaterialFactory[i].refIndex <= 0)
                    {
                        m_MaterialFactory.RemoveAt(i);
                    }
                    break;
                }
            }
        }
        //-----------------------------------------------------------------------------
        MaterialRef CreateMateral(Texture mainTexture)
        {
            MaterialRef mat = null;
            for (int i = 0; i < m_MaterialFactory.Count; ++i)
            {
                if (m_MaterialFactory[i].texture == mainTexture)
                    return m_MaterialFactory[i];
            }

            mat = new MaterialRef();
            mat.texture = mainTexture;
            m_MaterialFactory.Add(mat);
            return mat;
        }
        //-----------------------------------------------------------------------------
        protected override void OnUpdate(float fFrame)
        {
            for (var node = m_vAgents.First; node != null; node = node.Next)
            {
                var skinAgent = node.Value;
                skinAgent.ForceUpdate(fFrame);
            }

            var mainCamera = CameraUtil.mainCamera;
            if (mainCamera == null)
                return;

            m_CullingMatrix = mainCamera.cullingMatrix;
            Vector3 pos = CameraUtil.mainCameraPosition;
            Vector3 euler = CameraUtil.mainCameraEulerAngle;
            if (!BaseUtil.Equal(pos, m_LastCameraPos, 0.1f) || !BaseUtil.Equal(euler, m_LastCaemraEulerAngle, 0.1f))
            {
                m_nDirtyDraw = 2;
                m_LastCameraPos = pos;
                m_LastCaemraEulerAngle = euler;
            }
#if USE_URP
            if(m_UrpDraw == null)
                m_UrpDraw = URP.URPPostWorker.CastPostPass<BakerSkinningDrawer>(URP.EPostPassType.ForceCustomOpaque);
            if (m_UrpDraw == null)
                return;

            m_UrpDraw.UpdateSkins(m_vRuntimeDrawCmds, SystemInfo.supportsInstancing);
#endif
        }
        //------------------------------------------------------
        BakeSkinDrawCmd GetDrawCmdStack(SkinFrameData skinData, out int index)
        {
            index = -1;
            for (int i = 0; i < m_vRuntimeDrawCmds.Count; ++i)
            {
                if (m_vRuntimeDrawCmds[i].stackCount < 1023)
                {
                    index = i;
                    return m_vRuntimeDrawCmds[i];
                }
            }

            BakeSkinDrawCmd stack = new BakeSkinDrawCmd(skinData);
            m_vRuntimeDrawCmds.Add(stack);
            index = m_vRuntimeDrawCmds.Count - 1;
            return stack;
        }
        //-----------------------------------------------------------------------------
        public bool OnJobUpdate(float fFrame, IUserData userData = null)
        {
            if (m_nDirtyDraw == 0)
                return true;

            for(int i =0; i < m_vRuntimeDrawCmds.Count; ++i)
            {
                var cmd = m_vRuntimeDrawCmds[i];
                cmd.stackCount = 0;
                m_vRuntimeDrawCmds[i] = cmd;
            }

            CGpuSkinMeshAgent skinAgent;
            int curIndex = 0;
            for (var node = m_vAgents.First; node != null; node = node.Next)
            {
                skinAgent = node.Value;
                if (!skinAgent.IsCanDraw())
                    continue;
                var mesh = skinAgent.GetShareMesh();
                if (mesh == null)
                    continue;
                if (skinAgent.GetShareMaterial() == null)
                    continue;

                if (!Base.IntersetionUtil.PositionInView(m_CullingMatrix, skinAgent.GetPosition(), 1.5f))
                    continue;

                BakeSkinDrawCmd cmdStack = GetDrawCmdStack(skinAgent.frameData, out curIndex);
                if (curIndex < 0 || !cmdStack.IsValid)
                    continue;
                skinAgent.FillInstanceParams(cmdStack.vParams, cmdStack.stackCount);
                cmdStack.vMatrixs[cmdStack.stackCount] = Matrix4x4.TRS(skinAgent.GetPosition(), Quaternion.Euler(skinAgent.GetEulerAngle()), skinAgent.GetScale());
                cmdStack.stackCount++;
                m_vRuntimeDrawCmds[curIndex] = cmdStack;
            }
            m_nDirtyDraw = 0;
            for (var node = m_vAgents.First; node != null;)
            {
                var next = node.Next;
                var pCur = node.Value;
                if (m_nDirtyDraw != 0) break;
                if (m_vRemoveAgents.Contains(pCur.GetOwner()))
                {
                    pCur.Destroy();
                    m_vAgents.Remove(node);
                }
                node = next;
            }
            if(m_nDirtyDraw == 0)
                m_vRemoveAgents.Clear();

            return true;
        }
        //-----------------------------------------------------------------------------
        public int GetJob()
        {
            return 1;
        }
        //-----------------------------------------------------------------------------
        public void OnJobComplete(IUserData userData = null)
        {
        }
    }
}
#endif