
/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	ActorStateInfo
作    者:	HappLI
描    述:	角色状态信息对象，比如技能，buff等
*********************************************************************/
#if USE_ACTORSYSTEM
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FVector2 = UnityEngine.Vector2;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
using System.Collections.Generic;
using UnityEngine;
using Framework.Plugin.AT;
#if USE_CUTSCENE
using Framework.Cutscene.Runtime;
using Framework.CutsceneAT.Runtime;
#endif
namespace Framework.Core
{
	public enum EActorStateStatus
    {
        eBegin,
        eTicking,
        eEnd,
    }
    public abstract class AActorStateInfo : TypeObject, StateParam
    {
        public virtual uint GetDamageID(){ return 0; }
        public abstract void AddLockTarget(AWorldNode pNode, bool bClear = false);
        public abstract void ClearLockTargets();
        public abstract List<AWorldNode> GetLockTargets( bool isEmptyReLock = true);
    }
}
#endif
