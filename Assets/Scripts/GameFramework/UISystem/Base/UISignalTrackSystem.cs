/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	UISignalTrackSystem
作    者:	HappLI
描    述:	UI信栈槽系统，用于界面跳转还原
*********************************************************************/

using System.Collections.Generic;

namespace Framework.UI
{
    public class UISignalTrackSystem
    {
        struct Signal 
        {
            public int uiType;
            public byte flags;
            public List<int> vSlots;
        }

        Stack<Signal> m_vSignal = new Stack<Signal>();
        AUIManager m_pUIMgr = null;
        //------------------------------------------------------
        public UISignalTrackSystem(AUIManager uiMgr)
        {
            m_pUIMgr = uiMgr;
        }
        //------------------------------------------------------
        public void AddSignalTrack(UIBase ui, UIConfig uiConfig)
        {
            if (uiConfig == null)
                return;
            if (!ui.CanBackup())
                return;
            int type = ui.GetUIType();
            if (m_pUIMgr == null) return;
            foreach (var db in m_vSignal)
            {
                if (db.uiType == type)
                    return;
            }

            Signal signal = new Signal();
            signal.uiType = type;
            signal.flags = ui.GetBackupFlags();
            Dictionary<int, UIBase> uis = m_pUIMgr.GetUIS();
            if (ui.IsBackupAllShowed())
            {
                foreach (var db in uis)
                {
                    if (db.Key != ui.GetUIType() && m_pUIMgr.IsShow(db.Key))
                    {
                        if(db.Value.CanTrack())
                        {
                            if (signal.vSlots == null)
                                signal.vSlots = new List<int>();
                            signal.vSlots.Add(db.Key);
                        }
                    }
                }
            }
            else
            {
                if (ui.CanInheritCommonBackup() && uiConfig.CommonBackupUIs != null)
                {
                    for (int i = 0; i < uiConfig.CommonBackupUIs.Length; ++i)
                    {
                        if (m_pUIMgr.IsShow(uiConfig.CommonBackupUIs[i]))
                        {
                            UIConfig.UI subUi = uiConfig.GetUI(uiConfig.CommonBackupUIs[i]);
                            if (subUi != null && subUi.trackAble && !subUi.IsContainBasckup(type))
                            {
                                if (signal.vSlots == null)  signal.vSlots = new List<int>(2);
                                signal.vSlots.Add(uiConfig.CommonBackupUIs[i]);
                            }
                        }
                    }
                }
                UIConfig.UI uiData = uiConfig.GetUI(type);
                if (uiData != null && uiData.backups != null)
                {
                    for (int i = 0; i < uiData.backups.Count; ++i)
                    {
                        if (m_pUIMgr.IsShow(uiData.backups[i]))
                        {
                            UIConfig.UI subUi = uiConfig.GetUI(uiData.backups[i]);
                            if (subUi != null && subUi.trackAble && !subUi.IsContainBasckup(type))
                            {
                                if (signal.vSlots == null) signal.vSlots = new List<int>(2);
                                signal.vSlots.Add(uiData.backups[i]);
                            }
                        }
                    }
                }
            }
            if (signal.vSlots == null || signal.vSlots.Count <= 0)
                return;

            m_vSignal.Push(signal);

            if (ui.IsMoveOutSideBackup())
            {
                for (int i = 0; i < signal.vSlots.Count; ++i)
                {
                    m_pUIMgr.MoveOutside(signal.vSlots[i]);
                }
            }
            else
            {
                for (int i = 0; i < signal.vSlots.Count; ++i)
                {
                    m_pUIMgr.HideUI(signal.vSlots[i]);
                }
            }
        }
        //------------------------------------------------------
        public void RemoveSignalTrack(UIBase ui, UIConfig uiConfig)
        {
            if (m_vSignal.Count <= 0) return;
            Signal signal = m_vSignal.Peek();
            int type = ui.GetUIType();
            if (signal.uiType == type)
            {
                m_vSignal.Pop();
                if ((signal.flags & (int)EUIBackupFlag.Toggle) != 0)
                {
                    bool moveType = (signal.flags & (int)EUIBackupFlag.MoveOutside) != 0;

                    if (signal.vSlots != null)
                    {
                        if (moveType)
                        {
                            for (int i = 0; i < signal.vSlots.Count; ++i)
                            {
                                m_pUIMgr.MoveInside(signal.vSlots[i]);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < signal.vSlots.Count; ++i)
                            {
                                m_pUIMgr.ShowUI(signal.vSlots[i]);
                            }
                        }
                    }
                }
            }
        }
        //------------------------------------------------------
        public void Clear()
        {
            m_vSignal.Clear();
        }
    }
}
