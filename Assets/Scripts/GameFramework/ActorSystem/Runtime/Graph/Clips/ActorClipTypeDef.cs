#if USE_CUTSCENE && USE_ACTORSYSTEM
#if UNITY_EDITOR
using Framework.Cutscene.Editor;
using Framework.ED;
#endif
using UnityEngine;
using Framework.Base;

namespace Framework.Core
{
    public enum EActorCutsceneClipType
    {
        eMoveToLockTarget = 500, //移动到锁定目标位置
        eHitFrame = 501, //判定帧
    }
}
#endif