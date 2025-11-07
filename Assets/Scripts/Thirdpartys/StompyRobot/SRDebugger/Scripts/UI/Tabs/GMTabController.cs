#if USE_REPORTVIEW
using System.Linq;

namespace SRDebugger.UI.Tabs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Controls;
    using Controls.Data;
    using Internal;
    using Other;
    using Services;
    using SRF;
    using UnityEngine;
    using UnityEngine.UI;

    public class GMTabController : SRMonoBehaviourEx
    {
        public Button gm;
        protected override void Awake()
        {
            base.Awake();
            gm.onClick.AddListener(OnGMClick);
        }
        void OnGMClick()
        {
            if (SRDebug.OnGameController != null)
            {
                SRDebug.OnGameController();
                SRF.Service.SRServiceManager.GetService<IDebugService>().DestroyDebugPanel();
            }
        }
    }
}

#endif