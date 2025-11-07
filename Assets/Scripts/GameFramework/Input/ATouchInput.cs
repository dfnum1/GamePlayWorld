/********************************************************************
生成日期:	23:3:2020   16:23
类    名: 	ATouchInput
作    者:	HappLI
描    述:	输入模块
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
    public abstract class ATouchInput : AModule
    {
        public enum EState
        {
            None,
            Down,
            Up,
        }
        public enum ETouchType
        {
            Begin,
            Move,
            Wheel,
            End,
        }

        public struct TouchData
        {
            public int touchID;
            public EState   status;
            public Vector2  position;
            public Vector2  lastPosition;
            public Vector2 deltaPosition;
            public bool isUITouched;

            public void Clear()
            {
                touchID = -1; status = EState.None;
                position = lastPosition = Vector2.zero;
                isUITouched = false;
            }

            public Framework.Plugin.AT.ATMouseData ToATData(Plugin.AT.EATMouseType type)
            {
                Framework.Plugin.AT.ATMouseData atData = new Plugin.AT.ATMouseData();
                atData.state = type;
                atData.position = position;
                atData.lastPosition = lastPosition;
                atData.deltaPosition = deltaPosition;
                atData.isUITouched = isUITouched;
                return atData;
            }
        }
            
        public System.Action<TouchData>             OnMouseDown = null;
        public System.Action<TouchData>             OnMouseUp = null;
        public System.Action<TouchData>             OnMouseMove = null;
        public System.Action<float, Vector2>        OnMouseWheel = null;

        protected TouchData[] m_vTouchs = new TouchData[5];

        protected bool m_bUITrigger = false;
        protected int m_nForbidTouchCnt = 0;

        Vector2 m_PinchFingerLastPos0 = Vector2.zero;
        Vector2 m_PinchFingerLastPos1 = Vector2.zero;
        int m_nPinchFinger0 = -1;
        int m_nPinchFinger1 = -1;
        //-------------------------------------------------
        public bool isUITouch
        {
            get
            {
                if (UnityEngine.EventSystems.EventSystem.current != null)
                {
                    if (m_bUITrigger) return true;
                    if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() ||
                        UnityEngine.EventSystems.EventSystem.current.alreadySelecting ||
                        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null ||
                        UnityEngine.EventSystems.EventSystem.current.firstSelectedGameObject != null)
                        return true;
                    for (int i = 0; i < Input.touchCount; ++i)
                    {
                        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                            return true;
                    }
                }
                return false;
            }
        }
        //------------------------------------------------- 
        public void ReleaseTouchCnt()
        {
            m_nForbidTouchCnt--;
            if (m_nForbidTouchCnt < 0) m_nForbidTouchCnt = 0;
        }
        //------------------------------------------------- 
        public void GrabTouchCnt()
        {
            m_nForbidTouchCnt++;
        }
        //-------------------------------------------------
        protected override void OnInit()
        {
            for (int i = 0; i < m_vTouchs.Length; ++i)
            {
                m_vTouchs[i] = new TouchData();
                m_vTouchs[i].Clear();
            }
            m_nForbidTouchCnt = 0;
        }
        //-------------------------------------------------
        public void ResetRuntime()
        {
            m_nForbidTouchCnt = 0;
        }
        //-------------------------------------------------
        protected override void OnUpdate(float fFrameTime)
        {
            if (m_nForbidTouchCnt > 0) return;
            if(Input.touchCount >0)
            {
                //! mobile
                for(int i = 0; i < Input.touchCount; ++i)
                {
                    if (i >= m_vTouchs.Length) continue;
                    Touch touch = Input.GetTouch(i);
                    if(m_vTouchs[i].status == EState.None)
                    {
                        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
                        {
                            CheckTocuh(ref m_vTouchs[i], true, touch.fingerId, touch.position, touch.deltaPosition, false);
                        }
                    }
                    else
                    {
                        bool bPress = touch.phase != TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved;
                        bool bMove = touch.phase == TouchPhase.Moved;
                        CheckTocuh(ref m_vTouchs[i], bPress, touch.fingerId, touch.position, touch.deltaPosition, bMove);
                    }
                }
            }
            else
            {
                //! pc
                Vector2 deltaPos = new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
                bool bMoved = deltaPos.sqrMagnitude > 0;
                CheckTocuh(ref m_vTouchs[0], Input.GetMouseButton(0), 0, Input.mousePosition, deltaPos, bMoved);
                CheckTocuh(ref m_vTouchs[1], Input.GetMouseButton(1), 1, Input.mousePosition, deltaPos, bMoved);
                CheckTocuh(ref m_vTouchs[2], Input.GetMouseButton(2), 2, Input.mousePosition, deltaPos, bMoved);

                float wheel = Input.GetAxis("Mouse ScrollWheel");
                if ( wheel!=0)
                {
                    DoWheel(wheel, Input.mousePosition, Input.mousePosition);
                }
            }
        }
        //-------------------------------------------------
        void DoWheel(float wheel, Vector2 mousePosition0, Vector2 mousePosition1)
        {
            if (OnMouseWheel != null)
                OnMouseWheel(wheel, mousePosition0);
            m_pFramework.OnTouchWheel(wheel, mousePosition0);
        }
        //-------------------------------------------------
        void CheckTocuh(ref TouchData data, bool bPress, int touchID, Vector2 mousePos, Vector2 deltaPos, bool bMoved )
        {
            if (bPress)
            {
                data.touchID = touchID;
                data.deltaPosition = deltaPos;
                data.lastPosition = data.position;
                data.position = mousePos;
                data.isUITouched = this.isUITouch;
                if (data.status == EState.None)
                {
                    data.status = EState.Down;

                    if (m_nPinchFinger0 == -1)
                    {
                        m_nPinchFinger0 = touchID;
                        m_PinchFingerLastPos0 = mousePos;
                    }
                    if (m_nPinchFinger0 != -1 && m_nPinchFinger0 != touchID)
                    {
                        if (m_nPinchFinger1 == -1)
                        {
                            m_nPinchFinger1 = touchID;
                            m_PinchFingerLastPos1 = mousePos;
                        }
                    }

                    if (OnMouseDown != null) OnMouseDown(data);
                    m_pFramework.OnTouchBegin(data);
                }
                else if (data.status == EState.Down)
                {
                    if (bMoved)
                    {
                        bool bPinch = false;
                        if (m_nPinchFinger0 != -1 && m_nPinchFinger1 != -1)
                        {
                            if (m_nPinchFinger0 == touchID)
                            {
                                bPinch = Vector2.SqrMagnitude(m_PinchFingerLastPos0 - mousePos) != 0;
                            }
                            else if (m_nPinchFinger1 == touchID)
                            {
                                bPinch = Vector2.SqrMagnitude(m_PinchFingerLastPos1 - mousePos) != 0;
                            }
                        }
                           
                        if (OnMouseMove != null) OnMouseMove(data);
                        m_pFramework.OnTouchMove(data);

                        if(bPinch && CameraUtil.mainCamera)
                        {
                            var mainCamera = CameraUtil.mainCamera;
                            Vector2 touch0 = m_nPinchFinger0 == touchID ? mousePos : m_PinchFingerLastPos0;
                            Vector3 world0 = BaseUtil.RayHitPos(mainCamera.ScreenPointToRay(touch0));
                            Vector3 world0_last = BaseUtil.RayHitPos(mainCamera.ScreenPointToRay(m_PinchFingerLastPos0));

                            Vector2 touch1 = m_nPinchFinger1 == touchID ? mousePos : m_PinchFingerLastPos1;
                            Vector3 world1 = BaseUtil.RayHitPos(mainCamera.ScreenPointToRay(touch1));
                            Vector3 world1_last = BaseUtil.RayHitPos(mainCamera.ScreenPointToRay(m_PinchFingerLastPos1));

                            float fSqCur = (world1 - world0).magnitude;
                            float fSqLast = (world1_last - world0_last).magnitude;
                            float lenth = fSqCur - fSqLast;
                            if(lenth!=0.0f)
                                DoWheel(lenth, touch0, touch1);
                        }
                    }
                }
            }
            else
            {
                if (data.status != EState.None)
                {
                    if (OnMouseUp != null) OnMouseUp(data);
                    m_pFramework.OnTouchEnd(data);
                    data.Clear();
                }
            }
        }
    }
}