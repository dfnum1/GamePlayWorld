#if USE_CUTSCENE
/********************************************************************
生成日期:	06:30:2025
类    名: 	ProjecitleClip
作    者:	HappLI
描    述:	弹道剪辑
*********************************************************************/
using Framework.Base;
using UnityEngine;
using Framework.Core;
using System.Collections.Generic;

#if UNITY_EDITOR
using Framework.ED;
using System.Reflection;
using UnityEditor;
using Framework.Cutscene.Editor;
#endif
namespace Framework.Cutscene.Runtime
{
    [System.Serializable, CutsceneClip("弹道Clip")]
    public class ProjectileClip : IBaseClip
    {
        [PluginUnFilter]
        public enum EBornType : byte
        {
            [Display("无")]
            None = 0,
            [DisplayNameGUI("跟随发射者")]
            FollowTrigger,
            [DisplayNameGUI("跟随目标")]
            FollowTarget,
            [DisplayNameGUI("初始跟随目标")]
            StartTargetPos,
            [DisplayNameGUI("初始跟随触发者")]
            StartTriggerPos,
            [DisplayNameGUI("初始触发者Z向")]
            StartTriggerPosZ,
            [DisplayNameGUI("跟随发射时目标位置")]
            FollowThenTargetPos,
        }

        [Display("基本属性")] public BaseClipProp baseProp;

        [BindSlotGUI, DisplayNameGUI("触发绑点")]
        public string bindSlot = "";

        [Display("偏移")]
        public Vector3 offset = Vector3.zero;

        [DisplayNameGUI("自身旋转")]
        public Vector3 localRotate = Vector3.zero;

        public uint projectileLibrary = 0;
        public ProjectileData projecitleData = null;
        //-----------------------------------------------------
        public ACutsceneDriver CreateDriver()
        {
            return new ProjecitleDriver();
        }
        //-----------------------------------------------------
        public ushort GetIdType()
        {
            return (ushort)EClipType.eProjecitle;
        }
        //-----------------------------------------------------
        public float GetDuration()
        {
            return baseProp.duration;
        }
        //-----------------------------------------------------
        public EClipEdgeType GetEndEdgeType()
        {
            return baseProp.endEdgeType;
        }
        //-----------------------------------------------------
        public string GetName()
        {
            return baseProp.name;
        }
        //-----------------------------------------------------
        public ushort GetRepeatCount()
        {
            return baseProp.repeatCnt;
        }
        //-----------------------------------------------------
        public float GetTime()
        {
            return baseProp.time;
        }
        //-----------------------------------------------------
        public float GetBlend(bool bIn)
        {
            return baseProp.GetBlend(bIn);
        }
#if UNITY_EDITOR
        //-----------------------------------------------------
        public void OnSceneView(SceneView sceneView)
        {
            if (baseProp.ownerTrackObject == null)
                return;
        }
#endif		
    }
    //-----------------------------------------------------
    //相机移动驱动逻辑
    //-----------------------------------------------------
    public class ProjecitleDriver : ACutsceneDriver
    {
        //-----------------------------------------------------
        public override void OnDestroy()
        {
        }
        //-----------------------------------------------------
        public override bool OnClipEnter(CutsceneTrack pTrack, FrameData frameData)
        {
            ProjectileClip projectileClip = frameData.clip.Cast<ProjectileClip>();
            var pBinder = pTrack.GetBindLastCutsceneObject();
            if (pBinder == null)
                return true;
            if (!(pBinder is AWorldNode))
            {
                return true;
            }
            AWorldNode pOwner = pBinder as AWorldNode;
            AWorldNode pTarget = null;

            ProjectileData projectileData = null;
            if (projectileClip.projectileLibrary != 0)
            {
                projectileData = GetFramework().projectileManager.GetProjectileData(projectileClip.projectileLibrary);
            }
            else projectileData = projectileClip.projecitleData;
            if (projectileData == null)
                return true;

            Vector3 vEventPos = pOwner.GetPosition() + projectileClip.offset;
            if (!string.IsNullOrEmpty(projectileClip.bindSlot) && pOwner != null)
            {
                var bindSlot = pOwner.FindBindSlot(projectileClip.bindSlot);
                if (bindSlot != null)
                {
                    vEventPos = bindSlot.position;
                    if (projectileClip.offset.sqrMagnitude > 0)
                    {
                        vEventPos += bindSlot.forward * projectileClip.offset.z;
                        vEventPos += bindSlot.right * projectileClip.offset.x;
                        vEventPos += bindSlot.up * projectileClip.offset.y;
                    }
                }
            }

            StateParam stateParam = null;
            List<AWorldNode> vLockTargets = null;
            if (pOwner != null && pOwner is Actor)
            {
                Actor pActor = (Actor)pOwner;
                var skillSystem = pActor.GetSkillSystem();
                if (skillSystem != null)
                {
                    vLockTargets = skillSystem.GetLockTargets();
                }
                stateParam = pActor.GetStateParam();
            }

            StateParam targetStateParam = null;
            if (pTarget!=null)
            {
                Actor pActor = (Actor)pOwner;
                targetStateParam = pActor.GetStateParam();
            }

            float fDelayGap = 0;
            var cacheList = GetFramework().shareParams.catchUserDataList;
            if (vLockTargets == null)
                GetFramework().projectileManager.LaunchProjectile(projectileData, pOwner, stateParam, vEventPos, pOwner.GetDirection(), pTarget, 0, 0, null, cacheList);
            else
            {
                foreach(var db in vLockTargets)
                    GetFramework().projectileManager.LaunchProjectile(projectileData, pOwner, stateParam, vEventPos, pOwner.GetDirection(), db, 0, 0, null, cacheList);
            }
            for (int p = 0; p < cacheList.Count; ++p)
            {
                ProjectileNode pProj = (ProjectileNode)cacheList[p];
                pProj.SetLifeTime(projectileClip.GetDuration());
                pProj.SetRemainLifeTime(projectileClip.GetDuration());
                pProj.SetOffsetEulerAngle(projectileClip.localRotate);
                pProj.SetBindOwnerSlot(projectileClip.bindSlot);
                pProj.SetStartBindOffset(projectileClip.offset);
                if (fDelayGap > 0)
                {
                    pProj.SetVisible(false);
                }
            }
            return true;
        }
    }
}
#endif