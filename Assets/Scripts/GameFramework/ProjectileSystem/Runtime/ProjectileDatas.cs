/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Projectile
作    者:	HappLI
描    述:	飞行道具
*********************************************************************/

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif
using Framework.Base;
using System.Collections.Generic;
using UnityEngine;
#if USE_SERVER
using AudioClip = ExternEngine.AudioClip;
using Transform = ExternEngine.Transform;
#endif

#if UNITY_EDITOR
using System.Linq;
#endif

namespace Framework.Core
{
    public enum EProjectileParabolicType
    {
        [PluginDisplay("起点终点曲线")] StartEnd = 0,
        [PluginDisplay("路径点曲线")] TrackPath = 1,
        [PluginDisplay("路径点曲线连线终点")] TrackPathLinkEnd = 2,
    }
    public struct ProjectileKeyframe
    {
        public FFloat time;
        public FFloat speed;
        public FVector3 point;
        public FVector3 inTan;
        public FVector3 outTan;
        public ProjectileKeyframe(FFloat time, FFloat fSpeed, FVector3 point, FVector3 inTan, FVector3 outTan)
        {
            this.speed = fSpeed;
            this.time = time;
            this.point = point;
            this.inTan = inTan;
            this.outTan = outTan;
        }
    }
    public enum ELaunchFlag : uint
    {
        [DisplayNameGUI("使用X方向")] DirX = 1 << 0,
        [DisplayNameGUI("使用Y方向")] DirY = 1 << 1,
        [DisplayNameGUI("使用Z方向")] DirZ = 1 << 2,
        [DisplayNameGUI("朝向发射")] DirectionLuanch = 1 << 3,
        [DisplayNameGUI("死亡保持")] DieKeep = 1 << 4,
        [DisplayNameGUI("根据目标更新曲线终点")] RefreshEndPoint = 1 << 5,
        [DisplayNameGUI("只打目标")] TrackIngoreOtherCollision = 1 << 6,
        [DisableGUI] [DisplayNameGUI("使用方向")] AllDir = DirX | DirY | DirZ,
    }

    public enum EBoundFlag : byte
    {
        [DisplayNameGUI("弹射目标排除已弹射")] BoundDiscardBounded = 1 << 0,
        [DisplayNameGUI("弹射反转")] BoundInversion = 1 << 1,
        [DisplayNameGUI("弹射伤害+已弹次数")] BoundDamageAdd = 1 << 2,
        [DisplayNameGUI("物理反弹")] PhysicReflectBound = 1 << 3,
    }

    public enum EProjectileType : byte
    {
        [DisplayNameGUI("飞行器")] Projectile = 0,
        [DisplayNameGUI("跟踪飞行器")] Track = 1,
        [DisplayNameGUI("陷阱")] Trap = 2,
        [DisplayNameGUI("追踪点位-xy")] TrackPoint = 3,
        [DisplayNameGUI("飞行轨迹")] TrackPath = 4,
        [DisplayNameGUI("弹跳弹")] Bounce = 5,
    }

    public enum EProjecitleBornType : byte
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
        [DisplayNameGUI("跟随发射时目标位置")]
        FollowThenTargetPos,
    }

    public enum EProjectileCollisionType : byte
    {
        BOX = 0,
        CAPSULE,
        NONE
    };

    [System.Serializable]
    public class ProjectileData : AttackFrameParameter
    {
        public uint id;
        [DisplayNameGUI("类型")]
        public EProjectileType type = EProjectileType.Projectile;
        public EProjecitleBornType bornType = EProjecitleBornType.None;
        public float effectSpeed = -1;
        public string effect;
        //      public string effect_trail_wide;
        [DisplayNameGUI("发射音效(路径)"), StringViewGUI("FMODUnity.EventReference", -1), StringViewGUI(typeof(AudioClip))]
        public string sound_launch;
        [DisplayNameGUI("发射音效(id)")]
        [Data.FormViewBinder("Data.CsvData_Audio", "id")]
        public uint sound_launch_id;
        public Vector3[] speedLows;
        public Vector3[] speedUppers;
        public Vector3[] speedMaxs;
        public Vector3[] accelerations;
        public Vector2 speedLerp;
        public Vector3 minRotate;
        public Vector3 maxRotate;
        public EProjectileCollisionType collisionType;
        public Vector3 aabb_min;
        public Vector3 aabb_max;
        //   public float hit_rate_base;
        public float life_time = 5;
        public byte max_oneframe_hit = 1;
        public byte hit_count = 1;
        public float hit_step;
        public bool penetrable = false;
        public bool counteract;

        [DisplayNameGUI("更新标志")]
        [DisplayEnumBitGUI(typeof(ELaunchFlag))]
        public uint launch_flag = (int)ELaunchFlag.AllDir;

        public float explode_range;
        [Data.FormViewBinder("Data.CsvData_SkillDamage", "groupID")]
        public uint explode_damage_id;

        public bool externLogicSpeed = true;
        public float launch_delay = 0;

        [DisplayNameGUI("弹射标志")]
        [DisplayEnumBitGUI(typeof(EBoundFlag))]
        public ushort bound_flag = 0;
        public int bound_count = 0;
        public float bound_range = 0;
        public Vector3 bound_speed = Vector3.one;
        [DisplayNameGUI("弹射伤害")]
        [Data.FormViewBinder("Data.CsvData_SkillDamage", "groupID")]
        public int bound_damage_id = 0;
        [Data.FormViewBinder("Data.CsvData_Buff", "groupID")]
        [DisplayNameGUI("弹射Buff")]
        public int[] bound_buffs = null;
        [DisplayNameGUI("弹射锁定个数")]
        public byte bound_lock_num = 0;
        [DisplayNameGUI("弹射锁定参数1")]
        [NoListHeaderGUI]
        public int[] bound_lock_param1 = null;
        [DisplayNameGUI("弹射锁定参数2")]
        [NoListHeaderGUI]
        public int[] bound_lock_param2 = null;
        [DisplayNameGUI("弹射锁定参数3")]
        [NoListHeaderGUI]
        public int[] bound_lock_param3 = null;
        [DisplayNameGUI("弹射锁定赛道")]
        [NoListHeaderGUI]
        public short[] bound_lock_rode = null;
        [DisplayNameGUI("弹射锁定高度")]
        public bool bound_lockHeight = false;
        [DisplayNameGUI("弹射锁定最小高度")]
        public float bound_minLockHeight = 0;
        [DisplayNameGUI("弹射锁定最大高度")]
        public float bound_maxLockHeight = 0;

        public string bound_effect = null;
        public float bound_effectSpeed = -1;
        [StringViewGUI("FMODUnity.EventReference", -1), StringViewGUI(typeof(AudioClip))]
        public string bound_sound_launch = null;
        [Data.FormViewBinder("Data.CsvData_Audio", "id")]
        public uint bound_sound_launch_id = 0;
        public string bound_hit_effect = null;
        [StringViewGUI("FMODUnity.EventReference", -1), StringViewGUI(typeof(AudioClip))]
        public string bound_hit_sound = null;
        [Data.FormViewBinder("Data.CsvData_Audio", "id")]
        public uint bound_hit_sound_id = 0;

        public string waring_effect = "";
        public float waring_duration = 0;

        [DisplayNameGUI("追踪目标绑点")]
        public string[] track_target_slot;
        public Vector3 track_target_offset;

        public string explode_effect;
        public Vector3 explode_effect_offset;

        [DisplayNameGUI("缩放")]
        public float scale = 1;
        public string desc;

        [DisplayNameGUI("打中事件ID-对受击者")]
        [Data.FormViewBinder("Data.CsvData_EventDatas", "nID")]
        public int HitEventID;

        [DisplayNameGUI("打中事件ID-对攻击者")]
        [Data.FormViewBinder("Data.CsvData_EventDatas", "nID")]
        public int AttackEventID;

        [DisplayNameGUI("结束触发事件ID")]
        [Data.FormViewBinder("Data.CsvData_EventDatas", "nID")]
        public int OverEventID;

        [DisplayNameGUI("间隔事件ID")]
        [Data.FormViewBinder("Data.CsvData_EventDatas", "nID")]
        public int StepEventID;
        [DisplayNameGUI("间隔时间")]
        public float fEventStepGap = 0;
        [DisplayNameGUI("立即触发")]
        public bool bImmedate = false;

        [DisplayNameGUI("忽略场景地表检测")]
        public bool unSceneTest = false;

        [DisplayNameGUI("分类ID")]
        public byte classify = 0;
        public static bool IsTrack(EProjectileType type)
        {
            return type == EProjectileType.Track || type == EProjectileType.TrackPoint || type == EProjectileType.TrackPath;
        }
        //------------------------------------------------------
        public bool IsValidTrackPath()
        {
            if (speedLows == null || speedMaxs == null || speedUppers == null || accelerations == null) return false;
            if (speedLows.Length != speedUppers.Length || speedUppers.Length != speedMaxs.Length || speedMaxs.Length != accelerations.Length) return false;
            if (speedLows.Length <= 0) return false;
            return true;
        }
        //------------------------------------------------------
        public void AddTrackPoint(FVector3 point, FVector3 inTan, FVector3 outTan)
        {
            List<Vector3> points = speedMaxs != null ? new List<Vector3>(speedMaxs) : new List<Vector3>();
            List<Vector3> inTans = speedLows != null ? new List<Vector3>(speedLows) : new List<Vector3>();
            List<Vector3> outTans = speedUppers != null ? new List<Vector3>(speedUppers) : new List<Vector3>();
            List<Vector3> accSpeeds = accelerations != null ? new List<Vector3>(accelerations) : new List<Vector3>();
            points.Add(point);
            inTans.Add(inTan);
            outTans.Add(outTan);
            accSpeeds.Add(Vector3.right);
            speedMaxs = points.ToArray();
            speedUppers = outTans.ToArray();
            speedLows = inTans.ToArray();
            accelerations = accSpeeds.ToArray();
        }
        //------------------------------------------------------
        public void InsertTrackPoint(int index, FVector3 point, FVector3 inTan, FVector3 outTan)
        {
            List<Vector3> points = speedMaxs != null ? new List<Vector3>(speedMaxs) : new List<Vector3>();
            List<Vector3> inTans = speedLows != null ? new List<Vector3>(speedLows) : new List<Vector3>();
            List<Vector3> outTans = speedUppers != null ? new List<Vector3>(speedUppers) : new List<Vector3>();
            List<Vector3> accSpeeds = accelerations != null ? new List<Vector3>(accelerations) : new List<Vector3>();
            if (index < 0) index = 0;
            if (index >= points.Count) index = points.Count;
            points.Insert(index, point);
            inTans.Insert(index, inTan);
            outTans.Insert(index, outTan);
            accSpeeds.Insert(index, Vector3.right);
            speedMaxs = points.ToArray();
            speedUppers = outTans.ToArray();
            speedLows = inTans.ToArray();
            accelerations = accSpeeds.ToArray();
        }
        //------------------------------------------------------
        public void RemoveTrackPoint(int index)
        {
            List<Vector3> points = speedMaxs != null ? new List<Vector3>(speedMaxs) : new List<Vector3>();
            List<Vector3> inTans = speedLows != null ? new List<Vector3>(speedLows) : new List<Vector3>();
            List<Vector3> outTans = speedUppers != null ? new List<Vector3>(speedUppers) : new List<Vector3>();
            List<Vector3> accSpeeds = accelerations != null ? new List<Vector3>(accelerations) : new List<Vector3>();
            points.RemoveAt(index);
            inTans.RemoveAt(index);
            outTans.RemoveAt(index);
            accSpeeds.RemoveAt(index);
            speedMaxs = points.ToArray();
            speedUppers = outTans.ToArray();
            speedLows = inTans.ToArray();
            accelerations = accSpeeds.ToArray();
        }
        //------------------------------------------------------
        public bool IsLaunchFlaged(ELaunchFlag flag)
        {
            return (launch_flag & (int)flag) != 0;
        }
    }
    //------------------------------------------------------
    [CreateAssetMenu(menuName = "GameData/ProjectileDatas"), Data.PublishRefresh("Refresh")]
    public class ProjectileDatas : ScriptableObject
    {
        public ProjectileData[] projectiles;
        //-----------------------------------------------------
        public Dictionary<uint, ProjectileData> GetDatas(Dictionary<uint, ProjectileData> projectileDatas = null)
        {
            if(projectileDatas == null) projectileDatas = new Dictionary<uint, ProjectileData>();
            if (projectiles == null)
                return projectileDatas;

            for(int i =0; i < projectiles.Length; ++i)
            {
                projectileDatas[projectiles[i].id] = projectiles[i];
            }
            return projectileDatas;
        }
        //-----------------------------------------------------
#if UNITY_EDITOR
        void Refresh()
        {
            string projectileFileRoot = UnityEditor.AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(projectileFileRoot))
                return;
            projectileFileRoot = projectileFileRoot.Replace("\\", "/");
            var files = System.IO.Directory.GetFiles(projectileFileRoot, "*.json", System.IO.SearchOption.AllDirectories);
            List<ProjectileData> vDatas = new List<ProjectileData>();
            for (int i = 0; i < files.Length; ++i)
            {
                try
                {
                    ProjectileData projeileData = JsonUtility.FromJson<ProjectileData>(System.IO.File.ReadAllText(files[i]));
                    if (projeileData != null)
                        vDatas.Add(projeileData);
                }
                catch (System.Exception expec)
                {
                    string file = files[i].Replace("\\", "/").Replace(projectileFileRoot, "");
                    Debug.LogError($"file {file} data convert ProjectileData error: {expec.Message}");
                }
            }
            projectiles = vDatas.ToArray();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        //-----------------------------------------------------
        public static void RefreshDatas()
        {
            string[] projectileDataGuids = UnityEditor.AssetDatabase.FindAssets("t:ProjectileDatas");
            if (projectileDataGuids == null || projectileDataGuids.Length<=0)
                return;
            var projectiles = UnityEditor.AssetDatabase.LoadAssetAtPath<ProjectileDatas>(UnityEditor.AssetDatabase.GUIDToAssetPath(projectileDataGuids[0]));
            if (projectiles == null)
                return;

            projectiles.Refresh();


            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
        }
#endif
    }
}

