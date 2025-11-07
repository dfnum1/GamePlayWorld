/********************************************************************
生成日期:	3:14:2022  16:08
类    名: 	StateParam
作    者:	HappLI
描    述:	状态参数
*********************************************************************/
using Framework.Base;
using UnityEngine;

namespace Framework.Core
{
    public interface StateParam : IUserData
    {
        uint GetDamageID();
        System.Collections.Generic.List<Core.AWorldNode> GetLockTargets(bool isEmptyReLock=true);
        void AddLockTarget(Core.AWorldNode pActor, bool bClear = false);
        void ClearLockTargets();
    }

    public interface HitDamageParam : StateParam
    {
        AWorldNode GetAttacter();
        AWorldNode GetTarget();
        AWorldNode GetParentAttacker();

        Vector3 GetHitPostion();
        StateParam GetAttackStateParam();
    }

    //------------------------------------------------------
    [System.Serializable]
    public class AttackFrameParameter : Data.BaseData
    {
        [DisplayNameGUI("受击硬直")]
        public float stuck_time_hit;
        [DisplayNameGUI("受击计算朝向")]
        public bool target_direction_postion;


        //! on hit
        [DisplayNameGUI("受击者受击动作类型")]
        public uint target_action_hit;
        [Display("击退力度")]
        public Vector3 hit_back_speed;
        [Display("击退时位移摩檫力","<0时,不起作用")]
        public float hit_back_fraction =-1;
        [Display("击退时位移重力", "<0时,不起作用")]
        public float hit_back_gravity = -1f;


        [Display("受击持续时长", "如果为0且有动作，则取动作的时长")] public float target_duration_hit;
        [Display("受击特效缩放")]public float target_effect_hit_scale = 1.0f;
        [Display("受击特效"), StringViewGUI(typeof(GameObject))]public string target_effect_hit;
        [Display("受击特效偏移")] public Vector3 target_effect_hit_offset;
        [DisplayNameGUI("受击特效挂点"),BindSlotGUI] public string effect_hit_slot;

        [DisplayNameGUI("声音(路径)"), StringViewGUI("FMODUnity.EventReference", -1), StringViewGUI(typeof(AudioClip))]
        public string sound_hit;

        [DisplayNameGUI("声音(id)")]
        [Data.FormViewBinder("Data.CsvData_Audio", "id")]
        public uint sound_hit_id = 0;

        [DisplayNameGUI("伤害ID")]
        public uint damage;

        public void Reset()
        {
            target_action_hit = 0;
            hit_back_speed = Vector3.zero;
            hit_back_fraction = -1;
            hit_back_gravity = -1;
            stuck_time_hit = 0.0f;
            target_direction_postion = false;
            target_duration_hit = 0.0f;
            target_effect_hit_offset = Vector3.zero;

            target_effect_hit = "";
            sound_hit = "";
            damage = 0;
        }
#if UNITY_EDITOR
        //-------------------------------------------
        public virtual void SaveBinaryTo(ref Framework.Data.BinaryUtil serializer)
        {
            serializer.WriteByte(0);//version
            serializer.WriteFloat(stuck_time_hit);
            serializer.WriteBool(target_direction_postion);
            serializer.WriteUint32(target_action_hit);
            serializer.WriteVector3(hit_back_speed);
            serializer.WriteFloat(hit_back_fraction);
            serializer.WriteFloat(hit_back_gravity);
            serializer.WriteFloat(target_duration_hit);
            serializer.WriteFloat(target_effect_hit_scale);
            serializer.WriteString(target_effect_hit);
            serializer.WriteVector3(target_effect_hit_offset);
            serializer.WriteString(sound_hit);
            serializer.WriteUint32(sound_hit_id);
            serializer.WriteUint32(damage);
        }
#endif
    }
}
