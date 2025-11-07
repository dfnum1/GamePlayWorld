#if UNITY_EDITOR
using Framework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Framework.ED
{
	public class EditorFilePath
	{
		static string m_strLastMd5 = null;
		static FileSystemDebug ms_FileSettingDebug = null;
        //------------------------------------------------------
        public static string GetFileMD5(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                var sb = new StringBuilder();
                foreach (var b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
        //-----------------------------------------------------
        public static string GetObjectAdjustPath(UnityEngine.Object pObj)
        {
            if (pObj == null) return "";
            return AdjustAssetPath(AssetDatabase.GetAssetPath(pObj));
        }
        //------------------------------------------------------
        public static T LoadAssetAtPath<T>(string path) where T : UnityEngine.Object
        {
            string filePath = GetAssetPath(path);
            if (string.IsNullOrEmpty(filePath)) return null;
            return AssetDatabase.LoadAssetAtPath<T>(filePath);
        }
        //-----------------------------------------------------
        public static string GetAdjustPath(UnityEngine.Object pObj)
        {
            if (pObj == null) return "";
            return AdjustAssetPath(AssetDatabase.GetAssetPath(pObj));
        }
        //-----------------------------------------------------
        public static string AdjustAssetPath(string path)
		{
            if (string.IsNullOrEmpty(path)) return "";
            path = path.Replace("\\", "/");
            string strPath = Application.dataPath + "/../Publishs/Setting.json";
            if (System.IO.File.Exists(strPath))
            {
                try
                {
					string md5 = GetFileMD5(strPath);
                    if (ms_FileSettingDebug == null || m_strLastMd5 != md5)
					{
						m_strLastMd5 = md5;
                        string strCode = System.IO.File.ReadAllText(strPath, System.Text.Encoding.Default);
                        ms_FileSettingDebug = JsonUtility.FromJson<FileSystemDebug>(strCode);
                    }
                    if (ms_FileSettingDebug.subPathDirs!=null)
					{
                        for (int i = 0; i < ms_FileSettingDebug.subPathDirs.Count; ++i)
                        {
							string temPath = ms_FileSettingDebug.subPathDirs[i].Replace("\\", "/");
							if (path.StartsWith(temPath))
								path = path.Substring(temPath.Length);
							if(path.StartsWith("/")) path= path.Substring(1);
                        }
                    }
     
                }
                catch/* (System.Exception ex)*/
                {
                }
            }
            return path;
		}
        //-----------------------------------------------------
        public static string GetAssetPath(string path)
        {
            if (File.Exists(path)) return path;
            path = path.Replace("\\", "/");
            string strPath = Application.dataPath + "/../Publishs/Setting.json";
            if (System.IO.File.Exists(strPath))
            {
                try
                {
                    string md5 = GetFileMD5(strPath);
                    if (ms_FileSettingDebug == null || m_strLastMd5 != md5)
                    {
                        m_strLastMd5 = md5;
                        string strCode = System.IO.File.ReadAllText(strPath, System.Text.Encoding.Default);
                        ms_FileSettingDebug = JsonUtility.FromJson<FileSystemDebug>(strCode);
                    }
                    if (ms_FileSettingDebug.subPathDirs != null)
                    {
                        for (int i = 0; i < ms_FileSettingDebug.subPathDirs.Count; ++i)
                        {
                            string tempPath = ms_FileSettingDebug.subPathDirs[i].Replace("\\", "/");
                            if(tempPath.EndsWith("/")) tempPath = tempPath.Substring(0, tempPath.Length -1);
                            string filePath = string.Format("{0}/{1}", tempPath, path);
                            if (File.Exists(filePath))
                                return filePath.Replace("\\", "/");
                        }
                    }

                }
                catch/* (System.Exception ex)*/
                {
                }
            }
            return path;
        }
        //-----------------------------------------------------
        public static string DrawPathObject(GUIContent lable, string path, System.Type displayType = null, bool bScene = true, params GUILayoutOption[] layotOps)
		{
			string filePath = GetAssetPath(path);
            if(displayType == null) displayType = typeof(UnityEngine.Object);
            UnityEngine.Object pObject = EditorGUILayout.ObjectField(lable, AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath), displayType, bScene, layotOps);
            if (pObject != null)
            {
                string newPath = AssetDatabase.GetAssetPath(pObject);
                path = AdjustAssetPath(newPath);
            }
            else path = "";
            return path;
        }
        //-----------------------------------------------------
        public static string AssetPathToAbsotionPath(string assetPath)
		{
			const string assetPathStartStr = "Assets/";
			if (string.IsNullOrEmpty(assetPath) || !assetPath.StartsWith(assetPathStartStr))
				return "";

			return string.Format("{0}/{1}", Application.dataPath, assetPath.Substring(AssetPathStartStr.Length, assetPath.Length - AssetPathStartStr.Length));
		}
        //-----------------------------------------------------
        public static string AbsotionPathToAssetPath(string absPath)
		{
			if (string.IsNullOrEmpty(absPath) || !absPath.StartsWith(Application.dataPath))
				return "";

			return string.Format("Assets{0}", absPath.Substring(Application.dataPath.Length, absPath.Length - Application.dataPath.Length));
		}

        //-----------------------------------------------------
        public string absotionPath = "";
		public bool   isAssetPath = false;
		public string assetPath = "";

		public void Clear()
		{
			absotionPath = "";
			isAssetPath = false;
			assetPath = "";
		}
        //-----------------------------------------------------
        public void SettingFromAbsotionPath(string _absotionPath)
		{
			absotionPath = _absotionPath;
			isAssetPath = _absotionPath.StartsWith(Application.dataPath);
			if (isAssetPath)
				assetPath = string.Format("Assets{0}", absotionPath.Substring(Application.dataPath.Length, absotionPath.Length - Application.dataPath.Length));
			else
				assetPath = "";
		}

		const string AssetPathStartStr = "Assets/";
        //-----------------------------------------------------
        public void SettingFromAssetPath(string _assetPath)
		{
			if (string.IsNullOrEmpty(_assetPath))
				return;

			assetPath = _assetPath;
			isAssetPath = true;

			absotionPath = string.Format("{0}/{1}", Application.dataPath, _assetPath.Substring( AssetPathStartStr.Length, _assetPath.Length - AssetPathStartStr.Length) );
		}
        //-----------------------------------------------------
        public void UISettingPath(string title, string ext = "*.*", string directory = "", bool IsSave = false, string defaultSaveName = "default")
		{
			string basePath  = string.Format( "{0}/{1}", Application.dataPath.Substring(0, Application.dataPath.Length - "/assets".Length), directory);

			string path = null;
			if (IsSave)
			{
				path = EditorUtility.SaveFilePanel(title, basePath, defaultSaveName, ext);
			}
			else
				path = EditorUtility.OpenFilePanel(title, basePath, ext);
			SettingFromAbsotionPath(path);
		}
        //-----------------------------------------------------
        /// <summary>
        /// Call this funtion in the editor windows OnGUI function to active UI Control
        /// </summary>
        /// <param name="title"></param>
        /// <returns>Is Change path</returns>
        public bool OnGUI(string title, string ext = "*.*", string btnShow = "...",string directory = "", bool isSave = false, string defaultSaveName = "default")
		{
			string oldAbsotionPath = absotionPath.ToString();

			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(title);
			absotionPath = EditorGUILayout.TextField(absotionPath);
			if (GUILayout.Button(btnShow))
			{
				UISettingPath(title, ext, directory, isSave, defaultSaveName);
			}
			GUILayout.EndHorizontal();
			//if (!string.IsNullOrEmpty(absotionPath))
			//{
			//	if (isAssetPath)
			//		EditorGUILayout.LabelField(string.Format("{0}:{1}", title, assetPath));
			//	else
			//		EditorGUILayout.LabelField(string.Format("{0}:{1}", title, absotionPath));
			//}
			GUILayout.EndVertical();

			return oldAbsotionPath.CompareTo(absotionPath) != 0;
		}
	}
}
#endif