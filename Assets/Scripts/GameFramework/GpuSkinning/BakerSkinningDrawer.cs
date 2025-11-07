#if !USE_SERVER
using Framework.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if USE_URP
using Framework.URP;
using UnityEngine.Rendering.Universal;
#endif
namespace Framework.Plugin
{
    internal struct BakeSkinDrawCmd
    {
        public SkinFrameData skinData;
        public Matrix4x4[] vMatrixs;
        public Vector4[] vParams;
        public int stackCount;
        public BakeSkinDrawCmd(SkinFrameData skinFrame)
        {
            skinData = skinFrame;
            vMatrixs = new Matrix4x4[1023];
            vParams = new Vector4[1023];
            stackCount = 0;
        }
        public bool IsValid
        {
            get
            {
                if (skinData != null)
                    return true;
                return false;
            }
        }
    }
#if USE_URP
    [PostPass(EPostPassType.ForceCustomOpaque, 0)]
    internal class BakerSkinningDrawer : APostLogic
    {
        CommandBuffer m_CmdBuff = null;
        MaterialPropertyBlock m_ComdProperty = null;
        //------------------------------------------------------
        public override void Awake()
        {
            base.Awake();
            CGpuSharePropertyID.Init();
        }
        //------------------------------------------------------
        public override void Excude(APostRenderPass pass, CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_CmdBuff != null)
            {
                context.ExecuteCommandBuffer(m_CmdBuff);
            }
        }
        //------------------------------------------------------
        internal void UpdateSkins(List<BakeSkinDrawCmd> vDraws,  bool bInstacing)
        {
            if (m_CmdBuff == null)
            {
                m_CmdBuff = new CommandBuffer();
                m_CmdBuff.name = "GPUSkinningDraw";
            }
            m_CmdBuff.Clear();
            int drawCnt = vDraws.Count;
            if (bInstacing)
            {
                if (m_ComdProperty == null)
                    m_ComdProperty = new MaterialPropertyBlock();

                BakeSkinDrawCmd cmd;
                for(int i =0; i < drawCnt; ++i)
                {
                    if (i >= vDraws.Count)
                        break;

                    cmd = vDraws[i];
                    if (cmd.stackCount <= 0)
                        continue;
                    if (cmd.skinData == null)
                        continue;
                    var mesh = cmd.skinData.getShareMesh();
                    if (mesh == null)
                        continue;

                    m_ComdProperty.SetVectorArray(CGpuSharePropertyID.shaderPorpID_GPUSkinning_FrameIndex_PixelSegmentation, cmd.vParams);
                    m_CmdBuff.DrawMeshInstanced(mesh, 0, cmd.skinData.getShareMat(), 0, cmd.vMatrixs, cmd.stackCount, m_ComdProperty);
                }
            }
            else
            {
                BakeSkinDrawCmd cmd;
                for (int i = 0; i < drawCnt; ++i)
                {
                    if (i >= vDraws.Count)
                        break;

                    cmd = vDraws[i];
                    if (cmd.stackCount <= 0)
                        continue;
                    if (cmd.skinData == null)
                        continue;
                    var mesh = cmd.skinData.getShareMesh();
                    if (mesh == null)
                        continue;
                    for (int j = 0; j < cmd.stackCount; j++)
                        m_CmdBuff.DrawMesh(mesh, cmd.vMatrixs[j], cmd.skinData.getShareMat());
                }
            }
        }
    }
#endif
}
#endif