/********************************************************************
生成日期:	9:10:2020 18:16
类    名: 	UIAdapter
作    者:	HappLI
描    述:	UI适配器 根据机型适配
*********************************************************************/

using UnityEngine;

namespace Framework.UI
{
    public class UIAdapter : MonoBehaviour
    {
        //适配参数
        public static float AdapterLeft;
        public static float AdapterTop;
        public static float AdapterPosZ;
        public static float AdapterRight;
        public static float AdapterBottom;
        public enum EAdapterType
        {
            Offset,
            OffsetAndKeepSize,
            OffsetSizeLeft,
            OffsetSizeRight,
        }
        RectTransform m_Root;

        public bool offsetType = true;
        public bool bAutoAdapter = true;
        public EAdapterType AdapterType = EAdapterType.OffsetAndKeepSize;


        Vector2 m_LockAnchorMin = Vector2.zero;
        Vector2 m_LockAnchorMax = Vector2.zero;
#if UNITY_EDITOR
        bool m_bInited = false;

        float m_fLastAdapterLeft=0;
        float m_fLastAdapterTop = 0;
        float m_fLastAdapterRight = 0;
        float m_fLastAdapterBottom = 0;
#endif
        private void Awake()
        {
            RectTransform rect = transform as RectTransform;
#if UNITY_EDITOR

            m_bInited = true;

            m_fLastAdapterLeft = AdapterLeft;
            m_fLastAdapterTop = AdapterTop;
            m_fLastAdapterRight = AdapterRight;
            m_fLastAdapterBottom = AdapterBottom;
#endif
            if (offsetType)
            {
                m_LockAnchorMin = rect.offsetMin;
                m_LockAnchorMax = rect.offsetMax;
            }
            else
            {
                m_LockAnchorMin = rect.anchorMin;
                m_LockAnchorMax = rect.anchorMax;
            }

            if (bAutoAdapter)
                UpdateAdpter();
        }
        void UpdateAdpter()
        {
            if (m_Root == null) m_Root = transform as RectTransform;
            if (m_Root == null) return;

            float fScreenW = 1;
            float fScreenH = 1;
            if (!offsetType)
            {
                fScreenW = Screen.width;
                fScreenH = Screen.height;
            }

            float leftOffset = UI.UIAdapter.AdapterLeft / fScreenW;
            float topOffset = UI.UIAdapter.AdapterTop / fScreenH;
            float rightOffset = UI.UIAdapter.AdapterRight / fScreenW;
            float bottomOffset = UI.UIAdapter.AdapterBottom / fScreenH;

            switch (AdapterType)
            {
                case EAdapterType.Offset:
                    {
                        if (offsetType)
                        {
                            m_Root.offsetMin = m_LockAnchorMin + new Vector2(leftOffset, bottomOffset);
                            m_Root.offsetMax = m_LockAnchorMax + new Vector2(leftOffset, bottomOffset);
                        }
                        else
                        {
                            m_Root.anchorMin = m_LockAnchorMin + new Vector2(leftOffset, bottomOffset);
                            m_Root.anchorMax = m_LockAnchorMax + new Vector2(leftOffset, bottomOffset);
                        }
                    }
                    break;
                case EAdapterType.OffsetAndKeepSize:
                    {
                        if (offsetType)
                        {
                            m_Root.offsetMin = m_LockAnchorMin + new Vector2(leftOffset, bottomOffset - topOffset);
                            m_Root.offsetMax = m_LockAnchorMax + new Vector2(leftOffset - rightOffset, bottomOffset - topOffset);
                        }
                        else
                        {
                            m_Root.anchorMin = m_LockAnchorMin + new Vector2(leftOffset, bottomOffset - topOffset);
                            m_Root.anchorMax = m_LockAnchorMax + new Vector2(leftOffset - rightOffset, bottomOffset - topOffset);
                        }
                    }
                    break;
                case EAdapterType.OffsetSizeLeft:
                    {
                        if (offsetType)
                        {
                            m_Root.offsetMin = m_LockAnchorMin + new Vector2(leftOffset, bottomOffset);
                        }
                        else
                        {
                            m_Root.anchorMin = m_LockAnchorMin + new Vector2(leftOffset, bottomOffset);
                        }
                    }
                    break;
                case EAdapterType.OffsetSizeRight:
                    {
                        if (offsetType)
                        {
                            m_Root.offsetMax = m_LockAnchorMax + new Vector2(leftOffset - rightOffset, bottomOffset - topOffset);
                        }
                        else
                        {
                            m_Root.anchorMax = m_LockAnchorMax + new Vector2(leftOffset - rightOffset, bottomOffset - topOffset);
                        }
                    }
                    break;
            }
        }
#if UNITY_EDITOR
        
        void LateUpdate()
        {
            if(m_bInited)
            {
                if(m_fLastAdapterLeft != UI.UIAdapter.AdapterLeft || m_fLastAdapterTop != UI.UIAdapter.AdapterTop ||
                    m_fLastAdapterBottom != UI.UIAdapter.AdapterBottom || m_fLastAdapterRight != UI.UIAdapter.AdapterRight)
                {
                    UpdateAdpter();
                    m_fLastAdapterLeft = UI.UIAdapter.AdapterLeft;
                    m_fLastAdapterTop = UI.UIAdapter.AdapterTop;
                    m_fLastAdapterRight = UI.UIAdapter.AdapterRight;
                    m_fLastAdapterBottom = UI.UIAdapter.AdapterBottom;
                }
            }
        }
#endif
    }
}