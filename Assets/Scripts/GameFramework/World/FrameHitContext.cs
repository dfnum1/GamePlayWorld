///********************************************************************
//生成日期:	1:11:2020 13:16
//类    名: 	Actor
//作    者:	HappLI
//描    述:	
//*********************************************************************/
//using ExternEngine;
//using Framework.Base;
//using UnityEngine;
//namespace Framework.Core
//{
//    public enum EHitType
//    {
//        [PluginDisplay("未知")]
//        Unknown = 0,
//        [PluginDisplay("爆炸")]
//        Explode = 1,
//        [PluginDisplay("弹射")]
//        Bound = 2,
//        [PluginDisplay("贯穿")]
//        MutiHit = 3,
//    }
//    public struct HitFrameActor
//    {
//        public uint     attack_frame_id;
//        public ActionState attack_action_state;
//        public ActionState target_action_state;
//        public StateParam attack_state_param;
//        public StateParam state_param;
//        public uint     hit_frame_id;
//        public FVector3 hit_position;
//        public FVector3 hit_direction;
//        public AWorldNode    actor_ptr;
//        public uint     damage_id;
//        public ushort   damage_level;
//        public IUserData skill_damage_data;
//        public IUserData skill_data;
//        public byte     projectileClassify;
//        public FFloat    damage_power;
//        public uint     hit_body_part;

//        public uint     target_id;
//        public uint     target_config_id;
//        public uint     target_element_flags;

//        public int attacker_target_count;

//        public EHitType hitType;
//        public IUserData hit_type_user_data;

//        public EActorType target_type;
//        public HitFrameActor(uint damage_id, AWorldNode actor, FVector3 hit_position, FVector3 hit_direction, byte projectileClassify = 0, StateParam attack_state_param = null, StateParam state_param = null, uint attack_frame_id=0xffffffff, uint hit_frame_id=0xffffffff)
//        {
//            this.damage_id = damage_id;
//            this.projectileClassify = 0;
//            this.damage_level = 1;
//            this.hit_position = hit_position;
//            this.hit_direction = hit_direction;
//            this.attack_state_param = attack_state_param;
//            this.state_param = state_param;
//            this.damage_power = 1;
//            this.actor_ptr = actor;
//            this.hit_frame_id = hit_frame_id;
//            this.attack_frame_id = attack_frame_id;
//            this.hit_body_part = 0xffffffff;
//            this.skill_damage_data = null;
//            this.skill_data = null;
//            this.hitType = EHitType.Unknown;
//            this.hit_type_user_data = null;
           
//            if (attack_state_param!=null)
//            {
//                if(attack_state_param is Skill)
//                {
//                    Skill skill = attack_state_param as Skill;
//                    this.damage_level = skill.skillLevel;
//                    if (damage_id == 0)  this.damage_id = skill.GetDamageID();
//                    this.skill_damage_data = skill.skillDamage;
//                    this.skill_data = skill.skillData;
//                }
//                else if(attack_state_param is BufferState)
//                {
//                    BufferState buff = attack_state_param as BufferState;
//                    this.damage_level = buff.level;
//                }
//            }

//            if(actor!=null)
//            {
//                this.target_config_id = actor.GetConfigID();
//                this.target_type = actor.GetActorType();
//                this.target_id = (uint)actor.GetInstanceID();
//                this.target_element_flags = actor.GetElementFlags();
//            }
//            else
//            {
//                this.target_type = EActorType.Count;
//                this.target_id = 0xffffffff;
//                this.target_config_id = 0xffffffff;
//                this.target_element_flags = 0;
//            }
//            attack_action_state = null;
//            target_action_state = null;
//            if(actor!=null && actor is Actor)
//            {
//                target_action_state = (actor as Actor).GetCurrentActionState();
//            }
//            attacker_target_count = 0;
//        }
//    }
//    public struct ActorAttackData
//    {
//        public ActionState attack_action_state;
//        public ActionState target_action_state;
//        public Data.StateParam attack_state_param;
//        public Data.StateParam state_param;
//        public uint attack_frame_id;
//        public uint hit_frame_id;
//        public bool hard_frame;
//        public uint hit_part_body;
//        public uint action_id_on_hit;
//        public EActorGroundType ground_type;
//        public FFloat action_speed;
//        public FFloat action_speed_on_hit;
//        public FFloat state_delta;
//        public FFloat last_state_delta;
//        public int stuck_limit;
//        public bool facing_fowraed;
//        public FFloat final_hp;
//        public FFloat hp_incremnet;

//        public FVector3 hit_position;
//        public FVector3 hit_direction;

//        public IContextData skill_damage_data;
//        public IContextData skill_data;
//        public uint damage_id;
//        public ushort damage_level;
//        public byte projectileClassify;
//        public FFloat damage_power;

//        public bool critical_hit;

//        public int mul_hit_cnt;

//        public bool bPopText;
//        public AWorldNode attacker_parent_ptr;
//        public AWorldNode attacker_ptr;
//        public AWorldNode target_ptr;

//        public int attacker_id;
//        public int target_id;

//        public int attacker_config_id;
//        public int target_config_id;

//        public uint attacker_element_flags;
//        public uint target_element_flags;

//        public EActorType attacker_type;
//        public EActorType target_type;

//        public EHitType hitType;
//        public VariablePoolAble hit_type_user_data;

//        public int attacker_target_count;
//        public int frame_attack_index;

//        public IContextData elementEffect;

//        public BufferState buffDamageCall;

//        public static ActorAttackData DEFAULT = new ActorAttackData()
//        {
//            attack_state_param = null,
//            state_param = null,
//            attack_frame_id = 0xffffffff,
//            hit_frame_id = 0xffffffff,
//            hard_frame = false,
//            hit_part_body = 0xffffffff,
//            action_id_on_hit = 0xffffffff,
//            ground_type = EActorGroundType.Ground,
//            action_speed = 1,
//            action_speed_on_hit = 1,
//            state_delta = 0,
//            last_state_delta = 0,
//            stuck_limit = 0,
//            facing_fowraed = false,
//            final_hp = 0,
//            hp_incremnet = 0,
//            hit_position = FVector3.zero,
//            damage_id = 0xffffffff,
//            projectileClassify = 0,
//            damage_level = 1,
//            damage_power = 1,
//            skill_damage_data = null,
//            skill_data = null,
//            critical_hit = false,
//            mul_hit_cnt = 1,
//            target_ptr = null,
//            attacker_ptr = null,
//            attacker_parent_ptr = null,
//            attacker_id = 0,
//            target_id = 0,
//            attacker_type = EActorType.Count,
//            target_type = EActorType.Count,
//            target_config_id = 0,
//            attacker_config_id = 0,
//            attacker_element_flags = 0,
//            target_element_flags = 0,

//            elementEffect = null,
//            buffDamageCall = null,

//            hitType = EHitType.Unknown,
//            hit_type_user_data = null,
//            attack_action_state = null,
//            target_action_state = null,

//            attacker_target_count = 0,
//            frame_attack_index = 0,
//        };
//    }
//}

