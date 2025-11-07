/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	FileSystem
作    者:	HappLI
描    述:	
*********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework.Core
{
#if UNITY_EDITOR
    [System.Serializable]
    internal class FileSystemDebug
    {
        public List<string> subPathDirs = new List<string>();
        public List<string> buildDirs = new List<string>();
        public List<string> unbuildDirs = new List<string>();
    }
#endif
    //------------------------------------------------------
    public class FileSystem : AFileSystem
    {
        //------------------------------------------------------
        protected override void OnPreBuild()
        {
#if UNITY_EDITOR
            FileSystemDebug setting = null;
            string strPath = Application.dataPath + "/../Publishs/Setting.json";
            if (File.Exists(Application.dataPath + "/../Publishs/Setting_temp.json"))
            {
                strPath = Application.dataPath + "/../Publishs/Setting_temp.json";
            }
            if (System.IO.File.Exists(strPath))
            {
                try
                {
                    string strCode = File.ReadAllText(strPath, System.Text.Encoding.Default);
                    setting = JsonUtility.FromJson<FileSystemDebug>(strCode);
                    for(int i =0; i < setting.buildDirs.Count; ++i)
                        AddDynamicPath(setting.buildDirs[i]);
                    for (int i = 0; i < setting.subPathDirs.Count; ++i)
                        AddSearchPath(setting.subPathDirs[i]);

                    // AddDynamicPath("Assets/DatasRef/Role/xxx/animations/");

                }
                catch/* (System.Exception ex)*/
                {
                }
            }
#endif
        }
        //------------------------------------------------------
        protected override void OnInnerAwake()
        {
        }
        //------------------------------------------------------
        protected override void OnShutdown()
        {

        }
        //------------------------------------------------------
        public override int GetFileSize(string strFile, bool bAbs)
        {
            return GameDelegate.GetFileSize(strFile, bAbs);
        }
        //------------------------------------------------------
        public override int ReadBuffer(string strFile, byte[] buffer, int dataSize, int bufferOffset, int offsetRead, bool bAbs)
        {
            return GameDelegate.ReadBuffer(strFile, buffer, dataSize, bufferOffset, offsetRead, bAbs);
        }
        //------------------------------------------------------
        public override byte[] ReadFile(string strFile, bool bAbs, ref int dataSize)
        {
            return GameDelegate.ReadFile(strFile, bAbs, ref dataSize);
        }
        //------------------------------------------------------
        public override void EnableCatchHandle(bool bFileCatch,int catchCount=64)
        {
            GameDelegate.EnableCatchHandle(bFileCatch, catchCount);
        }
        //------------------------------------------------------
        public override void DeleteAllPackages()
        {
            GameDelegate.DeleteAllPackages();
        }
        //------------------------------------------------------
        public override IntPtr LoadPackage(string strPakFile)
        {
            return GameDelegate.LoadPackage(strPakFile);
        }
        //------------------------------------------------------
        public override void UnloadPackage(string strPakFile)
        {
            GameDelegate.UnloadPackage(strPakFile);
        }
    }
}

