/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	EActionStateType
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.Base;

namespace Framework.Core
{
    [PluginUnFilter]
    public enum EActionStateType : byte
    {
        [PluginDisplay("无")] None = 0,
        [PluginDisplay("待机")] Idle = 1,
        [PluginDisplay("跑")] Run = 2,
        [PluginDisplay("死亡")] Die = 4,

        [PluginDisplay("跳跃")] Jump = 50,
        [PluginDisplay("下落")] Fall = 51,

        [PluginDisplay("效果组")] EffectGroup = 100,
        [PluginDisplay("状态组")] StatusGroup,
        [PluginDisplay("攻击组")] AttackGroup,
        [PluginDisplay("受击组")] HurtGroup,

        [DisableGUI] Count,
    }
    //------------------------------------------------------
    public enum EActionStateTag : byte
    {
        [PluginDisplay("开始")] Start =0,
        [PluginDisplay("开始循环")] Starting = 1,
        [PluginDisplay("结束循环")] Ending = 2,
        [PluginDisplay("结束")] End =3,
        [PluginDisplay("循环")] Looping = 4,
    }
    //------------------------------------------------------
    public enum EActorGroundType : byte
    {
        [PluginDisplay("地面")] Ground,
        [PluginDisplay("空中")] Air,
        [PluginDisplay("滑铲")] Lying,
        [PluginDisplay("游泳")] Swimming,
        [PluginDisplay("飞行")] Flying,
    };
    //------------------------------------------------------
    public enum ECollisionFlag : byte
    {
        UP = (1 << 0),
        DOWN = (1 << 1),
        SIDES = (1 << 2),
    };
    //------------------------------------------------------
    [Framework.Plugin.AT.ATEvent("AT事件", ownerType:typeof(ActorComponent))]
    public enum EActorATType : int
    {
        [Framework.Plugin.AT.ATEvent("onGround")] onGround = 60000,
        [Framework.Plugin.AT.ATEvent("onHit")] onHit = 60001,
        [Framework.Plugin.AT.ATEvent("onAttack")] onAttack = 60002,
    }
}