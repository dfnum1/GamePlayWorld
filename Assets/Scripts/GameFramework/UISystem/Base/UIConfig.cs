/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UIConfig
作    者:	HappLI
描    述:	UI配置
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using Framework.Base;

namespace Framework.UI
{
    public enum EUIBackupFlag
    {
        [PluginDisplay("开启")] Toggle = 1<<0,
        [PluginDisplay("通用继承")] InheritCommon = 1<<1,
        [PluginDisplay("移至屏幕外")] MoveOutside = 1 << 2,
        [PluginDisplay("备份所有已开界面")] BackupAllShow = 1 << 3,
    }
    [CreateAssetMenu(fileName ="UIConfigs", menuName ="GameData/UIConfig")]
    public class UIConfig : ScriptableObject
    {
        [System.Serializable]
        public class UI
        {
            public int      type;
            public string   prefab;
            public bool     permanent;
            public bool     alwayShow;
            public int      Order;
            public bool     fullUI = false;
            public int      uiZValue;
            public bool     trackAble=true;

            //打开该UI时，备份之前的ui，且关闭备份列表中的ui, 直到界面关闭时还原
            [PluginDisplay("备份还原标志"), DisplayEnumBitGUI(typeof(EUIBackupFlag))]
            public byte canBackupFlag = 0;
            public List<int> backups;
            public bool IsCanBackup()
            {
                return (canBackupFlag & (int)EUIBackupFlag.Toggle) != 0;
            }
            public bool IsInheritCommonBackup()
            {
                return (canBackupFlag & (int)EUIBackupFlag.InheritCommon) != 0;
            }

            public bool IsContainBasckup(int ui)
            {
                if (backups == null || ui == 0) return false;
                return backups.Contains(ui);
            }
        }

        public UI[] UIS = null;
        public int[] CommonBackupUIs = null;

        public UIAnimatorAssets uiAnimators = null;

        private Dictionary<int, UI> m_vData = null;
        private void OnEnable()
        {
            if (UIS == null || UIS.Length <=0) return;
            if (m_vData == null) m_vData = new Dictionary<int, UI>(UIS.Length);
            m_vData.Clear();
            for (int i =0; i < UIS.Length; ++i)
            {
                if (UIS[i] == null || UIS[i].type == 0) continue;
                if (m_vData.ContainsKey(UIS[i].type))
                {
                    Debug.LogError("请检查UIConfig 配置文件是否有红色标记，请解决后再操作!");
                    continue;
                }
                m_vData.Add(UIS[i].type, UIS[i]);
            }
        }
        //------------------------------------------------------
        public  UI GetUI(int type)
        {
            if (m_vData == null || m_vData.Count<=0) return null;
            UI pOut = null;
            if (m_vData.TryGetValue(type, out pOut))
                return pOut;
            return null;
        }
    }
}

