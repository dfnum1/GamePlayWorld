/********************************************************************
生成日期:	5:11:2020  20:36
类    名: 	ActorComponent
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.Base;
using Framework.Plugin.AT;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public class ActorComponent : AInstanceAble
    {
        public struct Slot
        {
            public string name;
            public Transform slot;
            public Vector3 offset;
        }
        public Slot[] slots;
#if USE_ACTORSYSTEM
        [HideInInspector] public List<ActorCommonAction> commonActions = new List<ActorCommonAction>(2);
        [DisableGUI] public List<ActorAvatarMask> avatarMasks = new List<ActorAvatarMask>(2);
#endif
        public TextAsset ActionGraphData = null;
        public AgentTreeCoreData ATData;
        //-----------------------------------------------------
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        //-----------------------------------------------------
        public Transform GetSlot(string name, out Vector3 offset)
        {
            offset = Vector3.zero;
            if (slots == null) return null;
            for(int i =0; i < slots.Length; ++i)
            {
                if (slots[i].name.CompareTo(name) == 0)
                {
                    offset = slots[i].offset;
                    return slots[i].slot;
                }
            }
            return null;
        }
    }
}