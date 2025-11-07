/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	UISerialized
作    者:	HappLI
描    述:	UI 界面序列化
*********************************************************************/

using System.Collections.Generic;
using Framework.Base;
using UnityEngine;

namespace Framework.UI
{
    //------------------------------------------------------
    public enum UIEventType
    {
        [PluginDisplay("显示")] Show,
        [PluginDisplay("隐藏")] Hide,
        [PluginDisplay("移出屏")] MoveOut,
        [PluginDisplay("移进屏")] MoveIn,
        [PluginDisplay("移除")] Destroy,
        [DisableGUI] Count,
    }
    //------------------------------------------------------
    [System.Serializable]
    public class UIEventData
    {
        public int animationID;
        public RtgTween.TweenerGroup tweenGroup;
        public Transform ApplyRoot;

        public AnimationClip Animation;
#if USE_FMOD
        public FMODUnity.EventReference fmodEvent;
#else
        public AudioClip Audio;
#endif
        public bool bReverse;
        public float speedScale = 1;
    }
    //------------------------------------------------------
    public interface IUIEventLogic
    {
        bool ExcudeEvent(Transform pTransform, UIEventType eventType, UIEventData uiEvent);
    }
    public class UISerialized : Core.AComSerialized
    {
#if UNITY_EDITOR
        [System.NonSerialized]
        public string PrefabAsset = "";
#endif
        public UIEventData[] UIEventDatas;

        public UnityEngine.Object[] Elements;
        Dictionary<string, UnityEngine.Object> m_vFinder = null;
        //------------------------------------------------------
        protected override void OnSerialized()
        {
            if (Elements != null && Elements.Length > 0)
            {
                m_vFinder = new Dictionary<string, UnityEngine.Object>(Elements.Length);
                for (int i = 0; i < Elements.Length; ++i)
                {
                    if (Elements[i] == null) continue;
                    if (m_vFinder.ContainsKey(Elements[i].name)) continue;
                    m_vFinder.Add(Elements[i].name, Elements[i]);
                }
            }
        }
        //------------------------------------------------------
        public UIEventData GetEventData(UIEventType type)
        {
            if (UIEventDatas == null) return null;
            if ((int)type >= UIEventDatas.Length) return null;
            return UIEventDatas[(int)type];
        }
        //------------------------------------------------------
        public T GetRefObject<T>(string name) where T : UnityEngine.Object
        {
            if (m_vFinder == null) return null;
            if (m_vFinder.TryGetValue(name, out var prefab))
                return prefab as T;
            return null;
        }
        //------------------------------------------------------
        public void SetVisible(bool bVisible)
        {
            this.gameObject.SetActive(bVisible);
        }
    }
}
