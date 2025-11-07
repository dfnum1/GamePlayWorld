using System;
using Framework.Base;
namespace Framework.Core
{
    public enum ESlotBindBit : byte
    {
        [DisableGUI]
        None = 0,
        [PluginDisplay("位置")]
        Position = 1 << 0,
        [PluginDisplay("角度")]
        Rotation = 1 << 1,
        [PluginDisplay("缩放")]
        Scale = 1 << 2,
        [DisableGUI]
        All = Position | Rotation | Scale,
    }
    public enum EAttrValueType
    {
        Value,
        Rate,
    }
    public enum EActorType
    {
        Actor,
        Character,
        Projectile,
        Count,
        None = Count,
    }
}
