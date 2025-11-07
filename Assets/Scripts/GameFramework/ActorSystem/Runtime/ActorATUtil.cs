#if USE_ACTORSYSTEM
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	HitFrameActor
作    者:	HappLI
描    述:	
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
using Framework.Plugin.AT;

namespace Framework.Core
{
    [ATAPI("onGround", typeof(Actor))]
    internal struct OnGourndATData : IUserData
    {
        public EActorGroundType lastType;
        public EActorGroundType curType;

        public void Destroy()
        {
        }
    }
}
#endif