/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	InstanceEventParameter
作    者:	HappLI
描    述:	实例化对象
*********************************************************************/
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
namespace Framework.Core
{
    //------------------------------------------------------
    [EventDeclaration((ushort)EEventType.BornAble, "实例化对象", true)]
    [System.Serializable]
    public partial class InstanceEventParameter : BaseEvent
    {
        [Base.StringViewGUI(typeof(GameObject))]
        public string strFile = "";

        [Base.DisplayNameGUI("绝对")]
        public bool bAbs = true;

        [Base.DisplayNameGUI("绑点")]
        [Base.BindSlotGUI]
        public string parent_slot = "";

        [Base.DisplayNameGUI("偏移")]
        [Base.DisplayNameByField("bAbs", "true", "位置")]
        public Vector3 offset = Vector3.zero;
        [Base.DisplayNameGUI("偏移角度")]
        [Base.DisplayNameByField("bAbs", "true", "角度")]
        public Vector3 euler = Vector3.zero;
        [Base.DisplayNameGUI("缩放值")]
        public float scale = 1;

        [Base.DisplayNameGUI("绑定方式")]
        [Base.DisplayEnumBitGUI(typeof(ESlotBindBit))]
        public byte bindBit = (byte)ESlotBindBit.All;
        //-------------------------------------------
        public override Vector3 GetOffset()
        {
            return offset;
        }
        //-------------------------------------------
        public override byte GetBindBit()
        {
            return bindBit;
        }
        //-------------------------------------------
        public override void CollectPreload(AFramework pFramework, List<string> vFiles, HashSet<string> vAssets)
        {
            if (!string.IsNullOrEmpty(strFile))
            {
                vFiles.Add(strFile);
            }
        }
        //-------------------------------------------
        public override void OnExecute(EventSystem pEventSystem)
        {
#if !USE_SERVER
            if (pEventSystem.InstanceSpawner != null)
            {
                Transform parent = null;
                if (pEventSystem.pGameObject) parent = pEventSystem.pGameObject.transform;
                if (parent == null && pEventSystem.pInstnaceAble != null)
                    parent = pEventSystem.pInstnaceAble.GetTransorm();

                pEventSystem.InstanceSpawner.Spawn(strFile, bAbs, pEventSystem.TriggerEventPos, euler, parent);
            }
            else
            {
                FileSystemUtil.SpawnInstance(strFile);
            }
#endif
        }
    }
}

