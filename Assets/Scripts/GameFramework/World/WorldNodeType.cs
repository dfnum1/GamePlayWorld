/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	World
作    者:	HappLI
描    述:	世界
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public enum EWorldNodeStatus
    {
        Create = 0,
        Active,
        Visible,
        Hide,
        Killed,
        Revive,
        Destroy,
        Loaded,
    }
    public enum ENodeType
    {
        [Base.PluginDisplay("玩家")] Player = 0,
        [Base.PluginDisplay("怪物")] Monster = 1,
        [Base.PluginDisplay("地表元素")] Element = 2,
        [Base.PluginDisplay("召唤物")] Summon = 3,
        [Base.PluginDisplay("飞行道具")] Projectile = 5,
        [Base.PluginDisplay("场景节点")] Scene = 6,
        [Base.DisableGUI] Count,
        [Base.DisableGUI] None = Count,

    }

    public enum EWorldNodeFlag
    {
        Active = 1<<0,
        Destroy = 1<<1,
        Visible = 1<<2,
        Killed = 1<<3,
        Logic = 1<<4,
        AI = 1<<5,
        Spatial = 1<<6,
        CollectAble = 1<<7,
        Debug = 1<<8,
        RVO = 1<<9,
        SvrSyncIn = 1 << 10,
        SvrSyncOut = 1 << 11,
        HudBar = 1<<12,
        ColliderAble = 1<<13,
        Facing2D = 1 << 14,
        Default = EWorldNodeFlag.Visible | EWorldNodeFlag.Active | Spatial | CollectAble | RVO | Debug | HudBar,
    }
}
