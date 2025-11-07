/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	WindowUtil
作    者:	HappLI
描    述:	桌面窗口工具类
            参考链接:https://blog.csdn.net/ReDreamme/article/details/108329608
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
namespace Framework.Base
{
    public class WindowUtil
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
        #region Win函数常量
        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

        [DllImport("Dwmapi.dll")]
        static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        //private const int WS_POPUP = 0x800000;
        private const int GWL_EXSTYLE = -20;
        private const int GWL_STYLE = -16;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_BORDER = 0x00800000;
        private const int WS_CAPTION = 0x00C00000;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int LWA_COLORKEY = 0x00000001;
        private const int LWA_ALPHA = 0x00000002;
        private const int WS_EX_TRANSPARENT = 0x20;
        #endregion
#endif
        //-------------------------------------------------
        /// <summary>
        /// 设置窗口透明
        /// </summary>
        /// <param name="alpha">透明度 0-255</param>
        public static void SetWindowAlpha(int alpha)
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            IntPtr hwnd = GetActiveWindow();
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_LAYERED);
            SetLayeredWindowAttributes(hwnd, 0, alpha, LWA_ALPHA);
#endif
        }
        //-------------------------------------------------
        class WindowInfo
        {
            public IntPtr hwnd;
            public int currentX;
            public int currentY;

            public int backupExStyle;
            public int backupStyle;
        }
        private static Dictionary<string, WindowInfo> m_vWindows = new Dictionary<string, WindowInfo>(1);
        public static void SetWindowPetDockMode(string productName)
        {
            if(!m_vWindows.TryGetValue(productName ,out var windowInfo))
            {
                windowInfo = new WindowInfo();
                windowInfo.hwnd = FindWindow(null, productName);
                int intExTemp = GetWindowLong(windowInfo.hwnd, GWL_EXSTYLE);
                windowInfo.backupExStyle = intExTemp;
                windowInfo.backupStyle = GetWindowLong(windowInfo.hwnd, GWL_STYLE);
                SetWindowLong(windowInfo.hwnd, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT | WS_EX_LAYERED);
                SetWindowLong(windowInfo.hwnd, GWL_STYLE, windowInfo.backupStyle & ~WS_BORDER & ~WS_CAPTION);
                windowInfo.currentX = 0;
                windowInfo.currentY = 0;
                SetWindowPos(windowInfo.hwnd, -1, windowInfo.currentX, windowInfo.currentY, Screen.currentResolution.width, Screen.currentResolution.height, SWP_SHOWWINDOW);
                var margins = new MARGINS() { cxLeftWidth = -1 };
                DwmExtendFrameIntoClientArea(windowInfo.hwnd, ref margins);
            }
        }
    }
}
