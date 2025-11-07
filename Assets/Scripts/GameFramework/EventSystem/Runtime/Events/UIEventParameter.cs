/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	SummonEventParameter
作    者:	HappLI
描    述:	
*********************************************************************/
using Framework.Base;
using System.Collections.Generic;
namespace Framework.Core
{
    //------------------------------------------------------
    [EventDeclaration((ushort)EEventType.UIEvent, "UI事件")]
    [System.Serializable]
    public partial class UIEventParameter : BaseEvent
    {
        public enum EType : byte
        {
            Show,
            Hide,
            Close,
            UIRootShow,
            UIRootHide,
        }
        [DisplayNameGUI("UI类型"), DisplayEnumGUI("UI.EUIType"), StateGUIByField("bAllUI", "false")]
        public ushort uiType = 0;
        [DisplayNameGUI("全部")]
        public bool bAllUI = false;
        [DisplayNameGUI("操作类型")]
        public EType type = EType.Show;

        public List<Data.KeyValueParam> Params;

        //-------------------------------------------
        public override void OnExecute(EventSystem pEventSystem)
        {
#if !USE_SERVER
            var uiMgr = pEventSystem.GetFramework().uiManager;
            if (uiMgr == null) return;
            if (type == UIEventParameter.EType.UIRootHide)
            {
                if (uiMgr != null) uiMgr.ShowRoot(false);
                return;
            }
            if (type == UIEventParameter.EType.UIRootShow)
            {
                if (uiMgr != null) uiMgr.ShowRoot(true);
                return;
            }
            if (bAllUI)
            {
                switch (type)
                {
                    case UIEventParameter.EType.Close: uiMgr.CloseAll(); break;
                    case UIEventParameter.EType.Hide: uiMgr.HideAll(); break;
                    case UIEventParameter.EType.Show: uiMgr.ShowAll(); break;
                }
            }
            else
            {
                if (uiType <= 0) return;
                switch (type)
                {
                    case UIEventParameter.EType.Close: uiMgr.CloseUI(uiType); break;
                    case UIEventParameter.EType.Hide: uiMgr.HideUI(uiType); break;
                    case UIEventParameter.EType.Show:
                        {
                            if (Params != null && Params.Count > 0)
                            {
                                var handle = uiMgr.CastGetUI<UI.UIBase>(true, uiType);
                                if (handle != null)
                                {
                                    for (int i = 0; i < Params.Count; ++i)
                                    {
                                        if (string.IsNullOrEmpty(Params[i].key)) continue;
                                        handle.AddUserParam(Params[i].key, new VariableString() { strValue = Params[i].value });
                                    }
                                    handle.Show();
                                }
                            }
                            else
                            {
                                uiMgr.ShowUI(uiType);
                            }
                        }
                        break;
                }
            }
#endif
        }
    }
}

