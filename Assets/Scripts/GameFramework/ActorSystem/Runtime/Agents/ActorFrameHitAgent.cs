#if USE_ACTORSYSTEM
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	FrameHitData
作    者:	HappLI
描    述:	判定帧
*********************************************************************/
namespace Framework.Core
{
    public struct FrameHitData : IUserData
    {
        public AWorldNode pTarget;
    //    public AFrameClip pFrameClip;
        public void Destroy()
        {
        }
    }
    public class ActorFrameHitAgent : AActorAgent
    {
	}
}
#endif