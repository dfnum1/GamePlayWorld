#if UNITY_EDITOR && USE_ACTORSYSTEM
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	AnimationClipPlayableNode
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Text;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework.GraphVisualizer
{
    public class AnimationClipPlayableNode : PlayableNode
    {
        public AnimationClipPlayableNode(Playable content, float weight = 1.0f)
            : base(content, weight)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            var p = (Playable) content;
            if (p.IsValid())
            {
                var acp = (AnimationClipPlayable) p;
                var clip = acp.GetAnimationClip();
                sb.AppendLine(InfoString("Clip", clip ? clip.name : "(none)"));
                if (clip)
                {
                    sb.AppendLine(InfoString("ClipLength", clip.length));
                }
                sb.AppendLine(InfoString("FrameRate", clip? clip.frameRate.ToString():"0"));
                sb.AppendLine(InfoString("ApplyFootIK", acp.GetApplyFootIK()));
                sb.AppendLine(InfoString("ApplyPlayableIK", acp.GetApplyPlayableIK()));
                if(clip.isLooping) sb.AppendLine(InfoString("Wrap", UnityEngine.WrapMode.Loop));
                else sb.AppendLine(InfoString("Wrap", UnityEngine.WrapMode.Once));
            }

            return sb.ToString();
        }
    }
}
#endif