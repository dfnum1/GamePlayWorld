#if UNITY_EDITOR
/********************************************************************
生成日期:	25:7:2019   14:35
类    名: 	ContextMenu
作    者:	HappLI
描    述:	菜单
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Linq;

namespace Framework.ED
{
    static public class ContextMenu
    {
        static List<string> mEntries = new List<string>();
        static GenericMenu mMenu;

        static public void AddItem(string item, bool isChecked, GenericMenu.MenuFunction callback)
        {
            if (callback != null)
            {
                if (mMenu == null) mMenu = new GenericMenu();
                int count = 0;

                for (int i = 0; i < mEntries.Count; ++i)
                {
                    string str = mEntries[i];
                    if (str == item) ++count;
                }
                mEntries.Add(item);

                if (count > 0) item += " [" + count + "]";
                mMenu.AddItem(new GUIContent(item), isChecked, callback);
            }
            else AddDisabledItem(item);
        }
        //------------------------------------------------------
        static public void AddItemWithArge(string item, bool isChecked, GenericMenu.MenuFunction2 callback, object arge)
        {
            if (callback != null)
            {
                if (mMenu == null) mMenu = new GenericMenu();
                int count = 0;

                for (int i = 0; i < mEntries.Count; ++i)
                {
                    string str = mEntries[i];
                    if (str == item) ++count;
                }
                mEntries.Add(item);

                if (count > 0) item += " [" + count + "]";
                mMenu.AddItem(new GUIContent(item), isChecked, callback, arge);
            }
            else AddDisabledItem(item);
        }
        //------------------------------------------------------
        static public void Show()
        {
            if (mMenu != null)
            {
                mMenu.ShowAsContext();
                mMenu = null;
                mEntries.Clear();
            }
        }
        //------------------------------------------------------
        static public void Show(Vector2 pos, float width = 300, float height = 300)
        {
            if (mMenu != null)
            {
                mMenu.DropDown(new Rect(pos.x, pos.y- height, width, height));
                mMenu = null;
                mEntries.Clear();
            }
        }
        //------------------------------------------------------
        static public void AddSeparator(string path)
        {
            if (mMenu == null) mMenu = new GenericMenu();

            if (Application.platform != RuntimePlatform.OSXEditor)
                mMenu.AddSeparator(path);
        }
        //------------------------------------------------------
        static public void AddDisabledItem(string item)
        {
            if (mMenu == null) mMenu = new GenericMenu();
            mMenu.AddDisabledItem(new GUIContent(item));
        }
    }
}
#endif