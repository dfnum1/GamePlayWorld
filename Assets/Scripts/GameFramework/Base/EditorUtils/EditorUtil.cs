#if UNITY_EDITOR
using Framework.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Framework.ED
{
    //-----------------------------------------------------
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorGameModuleAttribute : Attribute
    {
    }
    public class EditorTimer
    {
        public float m_PreviousTime;
        public float deltaTime = 0.02f;
        public float fixedDeltaTime = 0.02f;
        public float m_fDeltaTime = 0f;
        public float m_currentSnap = 1f;

        //-----------------------------------------------------
        public void Update()
        {
            if (Application.isPlaying)
            {
               // Application.targetFrameRate = 30;
                deltaTime = Time.deltaTime;
                m_fDeltaTime = (float)(deltaTime * m_currentSnap);
            }
            else
            {
                float curTime = Time.realtimeSinceStartup;
                m_PreviousTime = Mathf.Min(m_PreviousTime, curTime);//very important!!!

                deltaTime = curTime - m_PreviousTime;
                m_fDeltaTime = (float)(deltaTime * m_currentSnap);
            }

            m_PreviousTime = Time.realtimeSinceStartup;
        }
    }
    public class EditorUtil
    {
        //-----------------------------------------------------
        static System.Type ms_EditorGameModule = null;
        public static Core.AFramework BuildEditorInstnace()
        { 
            if(ms_EditorGameModule == null)
            {
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    System.Type[] types = assembly.GetTypes();
                    for (int i = 0; i < types.Length; ++i)
                    {
                        System.Type tp = types[i];
                        if (tp.IsDefined(typeof(EditorGameModuleAttribute), false) && tp.IsSubclassOf(typeof(Core.AFramework)))
                        {
                            ms_EditorGameModule = tp;
                            break;
                        }
                    }
                    if (ms_EditorGameModule != null) break;
                }
            }
            if (ms_EditorGameModule == null) return null;
            var instance = Activator.CreateInstance(ms_EditorGameModule, true);
            Core.AFramework aFramework = (Core.AFramework)instance;
            return aFramework;
        }
        //------------------------------------------------------
        public static MethodInfo GetPluginEditorWindowOpenFunc(string strWindow)
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    System.Type tp = types[i];
                    if (tp.IsDefined(typeof(PluginEditorWindowAttribute), false))
                    {
                        PluginEditorWindowAttribute attr = (PluginEditorWindowAttribute)tp.GetCustomAttribute(typeof(PluginEditorWindowAttribute));
                        if (attr.widnowName == strWindow)
                        {
                            var methods = tp.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public| BindingFlags.DeclaredOnly);
                            for(int m =0; m < methods.Length; ++m)
                            {
                                var methodParams = methods[m].GetParameters();
                                if (methodParams == null || methodParams.Length <= 0)
                                    continue;
                                if(methods[m].Name.Equals(attr.method, StringComparison.OrdinalIgnoreCase) && methodParams[0].ParameterType == typeof(Data.Data_Base))
                                {
                                    return methods[m];
                                }
                            }
                            methods = tp.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                            for (int m = 0; m < methods.Length; ++m)
                            {
                                var methodParams = methods[m].GetParameters();
                                if (methodParams == null || methodParams.Length <= 0)
                                    continue;
                                if (methods[m].Name.Equals(attr.method, StringComparison.OrdinalIgnoreCase) && methodParams[0].ParameterType == typeof(Data.Data_Base))
                                {
                                    return methods[m];
                                }
                            }
                        }
                        break;
                    }
                }
            }
            return null;
        }
        //-----------------------------------------------------
        public static Type FindInheirtTypeType<T>()
        {
            System.Type parentType = typeof(T);
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    if (types[i].IsSubclassOf(parentType))
                    {
                        return types[i];
                    }
                }
            }
            return null;
        }
        //-----------------------------------------------------
        public static void OpenStartUpApplication(string strSceneFile)
        {
            if (EditorApplication.isPlaying)
            {
                //if(Event.current!=null && Event.current.control)如果按住ctrl +f5 那么不会执行这个函数,so
                // EditorApplication.isPlaying = false;
            }
            else
            {
                //保存当前场景
                var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
                if (scene != null && scene.isDirty && !string.IsNullOrWhiteSpace(scene.name) && !string.IsNullOrWhiteSpace(scene.path))
                {
                    if (UnityEditor.EditorUtility.DisplayDialog("提示", "当前场景未保存,是否保存?", "保存", "不保存"))
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
                    }
                }
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(strSceneFile);
                EditorApplication.isPlaying = true;
            }
        }
        //-----------------------------------------------------
        public static void NewScene()
        {
            if (!EditorApplication.isPlaying)
            {
                try
                {
                    //   EditorApplication.ExecuteMenuItem("File/New Scene");
                    UnityEngine.SceneManagement.Scene scen = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
                    RenderSettings.ambientSkyColor = Color.white;
                    //                     Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
                    //                     if(cameras == null || cameras.Length<=0)
                    //                     {
                    //                         GameObject cameraGo = new GameObject("EditorCaemra");
                    //                         Camera camera = cameraGo.AddComponent<Camera>();
                    //                         camera.tag = "MainCamera";
                    //                         cameraGo.hideFlags |= HideFlags.DontSave;
                    //                     }
                }
                catch/* (System.Exception ex)*/
                {

                }
            }
        }
        //-----------------------------------------------------
        public static string GetDisplayName(System.Enum curVar, string strDefault = null)
        {
            if (string.IsNullOrEmpty(strDefault)) strDefault = curVar.ToString();
            FieldInfo fi = curVar.GetType().GetField(curVar.ToString());
            if (fi == null) return strDefault;
            if (fi.IsDefined(typeof(PluginDisplayAttribute)))
                strDefault = fi.GetCustomAttribute<PluginDisplayAttribute>().displayName;
            return strDefault;
        }
        //------------------------------------------------------
        public static string GetEnumDisplayName(System.Enum curVar)
        {
            System.Type enumType = curVar.GetType();
            System.Reflection.FieldInfo fi = enumType.GetField(curVar.ToString());
            string strTemName = curVar.ToString();
            if (fi != null && fi.IsDefined(typeof(PluginDisplayAttribute)))
            {
                strTemName = fi.GetCustomAttribute<PluginDisplayAttribute>().displayName;
            }
            return strTemName;
        }
        //------------------------------------------------------
        static Dictionary<string, System.Type> ms_vBindTypes = null;
        public static System.Type GetTypeByName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            if (ms_vBindTypes == null)
            {
                ms_vBindTypes = new Dictionary<string, System.Type>();
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    System.Type[] types = assembly.GetTypes();
                    for (int i = 0; i < types.Length; ++i)
                    {
                        System.Type tp = types[i];
                        if (tp.IsDefined(typeof(PluginBinderTypeAttribute), false))
                        {
                            PluginBinderTypeAttribute attr = (PluginBinderTypeAttribute)tp.GetCustomAttribute(typeof(PluginBinderTypeAttribute));
                            ms_vBindTypes[attr.bindName] = tp;
                        }
                    }
                }
            }
            System.Type returnType;
            if (ms_vBindTypes.TryGetValue(typeName, out returnType))
                return returnType;
            returnType = Type.GetType(typeName);
            if (returnType != null) return returnType;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                returnType = assembly.GetType(typeName, false, true);
                if (returnType != null) return returnType;
            }
            int index = typeName.LastIndexOf(".");
            if (index > 0 && index + 1 < typeName.Length)
            {
                string strTypeName = typeName.Substring(0, index) + "+" + typeName.Substring(index + 1, typeName.Length - index - 1);
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    returnType = assembly.GetType(strTypeName, false, true);
                    if (returnType != null) return returnType;
                }
            }

            return null;
        }
        //------------------------------------------------------
        private static List<FileInfo> FindDirFiles(string strDir)
        {
            List<FileInfo> vRets = new List<FileInfo>();
            if (!Directory.Exists(strDir))
                return vRets;

            FindDirFiles(strDir, vRets);

            return vRets;

        }
        //------------------------------------------------------
        public static void FindDirFiles(string strDir, List<FileInfo> vRes)
        {
            if (!Directory.Exists(strDir)) return;

            string[] dir = Directory.GetDirectories(strDir);
            DirectoryInfo fdir = new DirectoryInfo(strDir);
            FileInfo[] file = fdir.GetFiles();
            if (file.Length != 0 || dir.Length != 0)
            {
                foreach (FileInfo f in file)
                {
                    vRes.Add(f);
                }
                foreach (string d in dir)
                {
                    FindDirFiles(d, vRes);
                }
            }
        }
        //------------------------------------------------------
        public static void CopyDir(string srcDir, string destDir, HashSet<string> vFilerExtension = null, HashSet<string> vIgoreExtension = null)
        {
            if (srcDir.Length <= 0 || destDir.Length < 0) return;

            srcDir = srcDir.Replace("\\", "/");
            if (srcDir[srcDir.Length - 1] == '/') srcDir = srcDir.Substring(0, srcDir.Length - 1);
            string[] split = srcDir.Split('/');
            List<string> vPop = new List<string>();
            int preLen = 0;
            for (int i = 0; i < split.Length; ++i)
            {
                if (split[i].CompareTo("..") == 0)
                {
                    vPop.RemoveAt(vPop.Count - 1);
                    continue;
                }
                preLen = srcDir.Length;
                vPop.Add(split[i]);
            }
            srcDir = "";
            foreach (var db in vPop)
            {
                srcDir += db + "/";
            }

            if (!Directory.Exists(srcDir)) return;
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            destDir = destDir.Replace("\\", "/");
            if (destDir[destDir.Length - 1] != '/') destDir += "/";

            List<FileInfo> vFiles = FindDirFiles(srcDir);

            string tile = "Copy:" + srcDir + "->" + destDir;

            int total = vFiles.Count;
            int cur = 0;
            EditorUtility.DisplayProgressBar(tile, "...", 0);
            foreach (FileInfo db in vFiles)
            {
                string file = db.FullName.Replace("\\", "/").Replace(srcDir, "");
                cur++;
                string extension = Path.GetExtension(file);
                EditorUtility.DisplayProgressBar(tile, file, (float)((float)cur / (float)total));

                if (vIgoreExtension != null && vIgoreExtension.Contains(extension)) continue;
                if (vFilerExtension != null && !vFilerExtension.Contains(extension)) continue;
                string destFile = destDir + file;

                string tmpFolder = Path.GetDirectoryName(destFile);
                if (!Directory.Exists(tmpFolder))
                    Directory.CreateDirectory(tmpFolder);
                File.Copy(db.FullName, destFile);
            }
            EditorUtility.ClearProgressBar();
        }
        //------------------------------------------------------
        public static void CopyFile(string srcFile, string destFile)
        {
            if (srcFile.Length <= 0 || destFile.Length < 0) return;

            srcFile = srcFile.Replace("\\", "/");
            if (!File.Exists(srcFile)) return;

            destFile = destFile.Replace("\\", "/");
            if (!Directory.Exists(Path.GetDirectoryName(destFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
            File.Copy(srcFile, destFile, true);
        }
        //------------------------------------------------------
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }
        }
        //------------------------------------------------------
        public static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string file in files)
            {
                DeleteFile(file);
            }
            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }
            Directory.Delete(path);
        }
        //------------------------------------------------------
        public static void ClearDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string file in files)
            {
                DeleteFile(file);
            }
        }
        //------------------------------------------------------
        public static void OpenPathInExplorer(string path)
        {
            if (path.Length <= 0f) return;
            System.Diagnostics.Process[] prpgress = System.Diagnostics.Process.GetProcesses();

            string args = "";
            if (!path.Contains(":/") && !path.Contains(":\\"))
            {
                if ((path[0] == '/') || (path[0] == '\\'))
                    path = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length) + path;
                else
                    path = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + path;
            }

            args = path.Replace(":/", ":\\");
            args = args.Replace("/", "\\");
            if (path.Contains("."))
            {
                args = string.Format("/Select, \"{0}\"", args);
            }
            else
            {
                if (args[args.Length - 1] != '\\')
                {
                    args += "\\";
                }
            }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start("Explorer.exe", args);
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            Debug.Log("IOS 打包路径: " + path);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("open", path));
#endif
        }
        //------------------------------------------------------
        public static void RepaintPlayModeView()
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.PlayModeView");
            var method = audioUtilClass.GetMethod("RepaintAll", BindingFlags.Static | BindingFlags.Public);
            if (method == null) method = audioUtilClass.GetMethod("RepaintAll", BindingFlags.Static | BindingFlags.NonPublic);
            if (method == null) return;
            method.Invoke(null, null);
        }
        //------------------------------------------------------
        public static List<T> FindComponents<T>(GameObject pObj, List<T> comps) where T : MonoBehaviour
        {
            T com = pObj.GetComponent<T>();
            if (com)
            {
                comps.Add(com);
                return comps;
            }
            for (int i = 0; i < pObj.transform.childCount; ++i)
                FindComponents(pObj.transform.GetChild(i).gameObject, comps);
            return comps;
        }
        //------------------------------------------------------
        public static void SetGameViewTargetSize(int width, int height)
        {
            try
            {
                var unityEditorAssembly = typeof(UnityEditor.GameViewSizeGroupType).Assembly;
                var gameView = unityEditorAssembly.GetType("UnityEditor.GameView");

                var gameViewSizesType = unityEditorAssembly.GetType("UnityEditor.GameViewSizes");
                var selectedSizeIndex = gameView.GetProperty("selectedSizeIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var GameViewSizeGroupType = gameView.GetProperty("currentSizeGroupType", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var SizeSelectionCallback = gameView.GetMethod("SizeSelectionCallback", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                var gameViewIns = UnityEditor.EditorWindow.GetWindow(gameView);
                if (gameViewIns == null) return;
                UnityEditor.GameViewSizeGroupType groupType = (UnityEditor.GameViewSizeGroupType)GameViewSizeGroupType.GetValue(null);
                int selectIndex = (int)selectedSizeIndex.GetValue(gameViewIns);

                string file = UnityEditorInternal.InternalEditorUtility.unityPreferencesFolder + "/GameViewSizes.asset";
                UnityEngine.Object[] objects = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(file);
                for (int i = 0; i < objects.Length; ++i)
                {
                    if (objects[i].GetType() == gameViewSizesType)
                    {
                        var method = gameViewSizesType.GetMethod("GetGroup", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (method != null)
                        {
                            var gameViewSizeGroupInst = method.Invoke(objects[i], new System.Object[] { groupType });
                            if (gameViewSizeGroupInst != null)
                            {
                                var GetTotalCountMethod = gameViewSizeGroupInst.GetType().GetMethod("GetTotalCount", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                if (GetTotalCountMethod != null)
                                {
                                    var GetGameViewSizeMethod = gameViewSizeGroupInst.GetType().GetMethod("GetGameViewSize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                    int TotalCount = (int)GetTotalCountMethod.Invoke(gameViewSizeGroupInst, null);
                                    for (int j = 0; j < TotalCount; ++j)
                                    {
                                        var gameviewSize = GetGameViewSizeMethod.Invoke(gameViewSizeGroupInst, new System.Object[] { j });
                                        if (gameviewSize != null)
                                        {
                                            var widthProp = gameviewSize.GetType().GetProperty("width", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                            var heightProp = gameviewSize.GetType().GetProperty("height", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                            int widthGV = (int)widthProp.GetValue(gameviewSize);
                                            int heightGV = (int)heightProp.GetValue(gameviewSize);

                                            if (widthGV == width && heightGV == height)
                                            {
                                                SizeSelectionCallback.Invoke(gameViewIns, new System.Object[] { j, gameviewSize });

                                                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (System.Exception expection)
            {
                Debug.Log("更改分辨率失败");
                Debug.LogWarning(expection.ToString());
            }
        }
        //------------------------------------------------------
        public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public);
            if (method == null)
            {
                method = audioUtilClass.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public);
            }
            if (method == null)
                return;

            method.Invoke(null, new object[] { clip, startSample, loop });
        }
        static MethodInfo ms_pExternAudioPlayer = null;
        //------------------------------------------------------
        public static void PlayClip(string audioClipFile, int startSample = 0, bool loop = false)
        {
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioClipFile);
            if (clip == null)
            {
                if (ms_pExternAudioPlayer == null)
                {
                    foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                    {
                        System.Type[] types = assembly.GetTypes();
                        for (int i = 0; i < types.Length; ++i)
                        {
                            System.Type tp = types[i];
                            if (tp.IsDefined(typeof(PluginExternAudioAttribute), false))
                            {
                                PluginExternAudioAttribute attr = (PluginExternAudioAttribute)tp.GetCustomAttribute(typeof(PluginExternAudioAttribute));
                                ms_pExternAudioPlayer = tp.GetMethod(attr.strMethod, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                                break;
                            }
                        }
                    }
                }
                if (ms_pExternAudioPlayer != null)
                {
                    ms_pExternAudioPlayer.Invoke(null, new object[] { audioClipFile });
                }
                return;
            }

            PlayClip(clip, startSample, loop);
        }
        //------------------------------------------------------
        public static void StopAllAudioClips()
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public);
            if (method == null) return;
            method.Invoke(null, null);
        }
        //------------------------------------------------------
        public static void PopMessageBox(string title, string context, string ok)
        {
            EditorUtility.DisplayDialog(title, context, ok);
        }
        //------------------------------------------------------
        public static bool PopMessageBox(string title, string context, string ok = "确定", string cancel = "取消")
        {
            return EditorUtility.DisplayDialog(title, context, ok, cancel);
        }
        //-------------------------------------------------
        static System.Type ms_Type;
        static MethodInfo ms_MetInfo;
        public static long GetStorgeMemory(UnityEngine.Object asset)
        {
            if (asset != null)
            {
                if (asset.GetType() == typeof(UnityEngine.Texture) || asset.GetType().BaseType == typeof(UnityEngine.Texture))
                {
                    ms_Type = System.Reflection.Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
                    ms_MetInfo = ms_Type.GetMethod("GetStorageMemorySize", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                    return (long)(int)ms_MetInfo.Invoke(null, new object[] { asset });
                    //    return 0;
                }
                else
                {
                    string filePath = AssetDatabase.GetAssetPath(asset);
                    if (filePath.Length > 0)
                    {
                        FileInfo fileInfo = new FileInfo(filePath);
                        return fileInfo.Length;
                    }
                }

            }
            return 0;
        }
        //------------------------------------------------------
        public static bool IsTextureMipMap(UnityEngine.Object asset)
        {
            if (asset != null)
            {
                if (asset.GetType() == typeof(UnityEngine.Texture) || asset.GetType().BaseType == typeof(UnityEngine.Texture))
                {
                    ms_Type = System.Reflection.Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
                    ms_MetInfo = ms_Type.GetMethod("HasMipMap", BindingFlags.Static | BindingFlags.Public);
                    return (bool)ms_MetInfo.Invoke(null, new object[] { asset });
                    //    return 0;
                }

            }
            return false;
        }
        //------------------------------------------------------
        public static bool IsReadWriteAble(UnityEngine.Object asset)
        {
            if (asset == null) return false;
            if (asset is MonoScript)
                return false;

            string path = AssetDatabase.GetAssetPath(asset);
            if (asset.GetType() == typeof(UnityEngine.Texture) || asset.GetType().BaseType == typeof(UnityEngine.Texture))
            {
                TextureImporter texture = AssetImporter.GetAtPath(path) as TextureImporter;
                return texture.isReadable;
            }
            else if (path.ToLower().Contains(".fbx"))
            {
                ModelImporter model = AssetImporter.GetAtPath(path) as ModelImporter;
                if (model == null) return false;
                return model.isReadable;
            }
            return false;
        }
        //------------------------------------------------------
        public static bool IsOptimizeMeshVertices(UnityEngine.Object asset)
        {
            if (asset == null) return false;

            string path = AssetDatabase.GetAssetPath(asset);
            if (path.ToLower().Contains(".fbx"))
            {
                ModelImporter model = AssetImporter.GetAtPath(path) as ModelImporter;
                return model.optimizeMeshVertices;
            }
            return false;
        }
        //------------------------------------------------------
        public static bool IsOptimizeMeshPolygon(UnityEngine.Object asset)
        {
            if (asset == null) return false;

            string path = AssetDatabase.GetAssetPath(asset);
            if (path.ToLower().Contains(".fbx"))
            {
                ModelImporter model = AssetImporter.GetAtPath(path) as ModelImporter;
                return model.optimizeMeshPolygons;
            }
            return false;
        }
        //------------------------------------------------------
        public static ModelImporterMeshCompression CompressMesh(UnityEngine.Object asset)
        {
            if (asset == null) return ModelImporterMeshCompression.Off;

            string path = AssetDatabase.GetAssetPath(asset);
            if (path.ToLower().Contains(".fbx"))
            {
                ModelImporter model = AssetImporter.GetAtPath(path) as ModelImporter;
                return model.meshCompression;
            }
            return ModelImporterMeshCompression.Off;
        }
        //-----------------------------------------------------
        static void DrawBoundingBoxLine(Vector3 vCenter, Vector3 vHalf, Matrix4x4 mWorld, Vector3 vStart, Vector3 vEnd, Color dwColor, bool bGizmos)
        {
            Vector3 v1 = new Vector3(vHalf.x * vStart.x, vHalf.y * vStart.y, vHalf.z * vStart.z);
            Vector3 v2 = new Vector3(vHalf.x * vEnd.x, vHalf.y * vEnd.y, vHalf.z * vEnd.z);
            v1 = v1 + vCenter;
            v2 = v2 + vCenter;
            v1 = mWorld.MultiplyPoint(v1);
            v2 = mWorld.MultiplyPoint(v2);
            DrawLine(v1, v2, dwColor, bGizmos);
        }
        //------------------------------------------------------
        static void DrawLine(Vector3 start, Vector3 end, Color dwColor, bool bGizmos)
        {
            if (bGizmos)
            {
                Color colr = UnityEngine.Gizmos.color;
                UnityEngine.Gizmos.color = dwColor;
                UnityEngine.Gizmos.DrawLine(start, end);
                UnityEngine.Gizmos.color = colr;
            }
            else
            {
#if UNITY_EDITOR
                Color colr = UnityEditor.Handles.color;
                UnityEditor.Handles.color = dwColor;
                UnityEditor.Handles.DrawLine(start, end);
                UnityEditor.Handles.color = colr;
#endif
            }
        }

        //------------------------------------------------------
        public static string GetVolumeColor(Core.EVolumeType type)
        {
            if (type == Core.EVolumeType.Target) return "#00ff00ff";
            if (type == Core.EVolumeType.Attack) return "#ff0000ff";
            if (type == Core.EVolumeType.AttackInvert) return "#ff8800ff";
            return "#fffffff";
        }
        //------------------------------------------------------
        public static Color GetVolumeToColor(Core.EVolumeType type)
        {
            if (type == Core.EVolumeType.Target) return new Color(0, 1, 0, 1);
            if (type == Core.EVolumeType.Attack) return new Color(1, 0, 0, 1);
            if (type == Core.EVolumeType.AttackInvert) return new Color(136 / 255f, 0, 1, 1);
            return Color.white;
        }
        //------------------------------------------------------
        public static void DrawBoundingBox(Vector3 vCenter, Vector3 vHalf, Matrix4x4 mWorld, Color dwColor, bool bGizmos)
        {
            //CheckArray();
            //FVector3 vTransCenter1 = mWorld.MultiplyPoint(vCenter);

            //vDirArray1[0] = mWorld.GetColumn(2); //dir
            //vDirArray1[1] = mWorld.GetColumn(1); // up
            //vDirArray1[2] = mWorld.GetColumn(0); //right
            //vVertexArray1[0] = vTransCenter1 + vDirArray1[2] * vHalf.x + vDirArray1[1] * vHalf.y + vDirArray1[0] * vHalf.z;
            //vVertexArray1[1] = vTransCenter1 - vDirArray1[2] * vHalf.x + vDirArray1[1] * vHalf.y + vDirArray1[0] * vHalf.z;
            //vVertexArray1[2] = vTransCenter1 + vDirArray1[2] * vHalf.x + vDirArray1[1] * vHalf.y - vDirArray1[0] * vHalf.z;
            //vVertexArray1[3] = vTransCenter1 - vDirArray1[2] * vHalf.x + vDirArray1[1] * vHalf.y - vDirArray1[0] * vHalf.z;
            //vVertexArray1[4] = vTransCenter1 + vDirArray1[2] * vHalf.x - vDirArray1[1] * vHalf.y + vDirArray1[0] * vHalf.z;
            //vVertexArray1[5] = vTransCenter1 - vDirArray1[2] * vHalf.x - vDirArray1[1] * vHalf.y + vDirArray1[0] * vHalf.z;
            //vVertexArray1[6] = vTransCenter1 + vDirArray1[2] * vHalf.x - vDirArray1[1] * vHalf.y - vDirArray1[0] * vHalf.z;
            //vVertexArray1[7] = vTransCenter1 - vDirArray1[2] * vHalf.x - vDirArray1[1] * vHalf.y - vDirArray1[0] * vHalf.z;

            ////top
            //DrawLine(vVertexArray1[0], vVertexArray1[1], dwColor, bGizmos);
            //DrawLine(vVertexArray1[1], vVertexArray1[3], dwColor, bGizmos);
            //DrawLine(vVertexArray1[3], vVertexArray1[2], dwColor, bGizmos);
            //DrawLine(vVertexArray1[2], vVertexArray1[0], dwColor, bGizmos);

            ////bottom
            //DrawLine(vVertexArray1[4], vVertexArray1[5], dwColor, bGizmos);
            //DrawLine(vVertexArray1[5], vVertexArray1[7], dwColor, bGizmos);
            //DrawLine(vVertexArray1[7], vVertexArray1[6], dwColor, bGizmos);
            //DrawLine(vVertexArray1[6], vVertexArray1[4], dwColor, bGizmos);

            ////left
            //DrawLine(vVertexArray1[1], vVertexArray1[5], dwColor, bGizmos);
            //DrawLine(vVertexArray1[3], vVertexArray1[7], dwColor, bGizmos);

            ////right
            //DrawLine(vVertexArray1[0], vVertexArray1[4], dwColor, bGizmos);
            //DrawLine(vVertexArray1[2], vVertexArray1[6], dwColor, bGizmos);

            //return;
            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), dwColor, bGizmos);
            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, -1.0f), dwColor, bGizmos);
            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), dwColor, bGizmos);
            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, 1.0f), dwColor, bGizmos);

            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f), dwColor, bGizmos);
            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, -1.0f), dwColor, bGizmos);
            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), dwColor, bGizmos);
            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, 1.0f), dwColor, bGizmos);

            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, -1.0f, 1.0f), dwColor, bGizmos);
            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f), dwColor, bGizmos);
            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), dwColor, bGizmos);
            DrawBoundingBoxLine(vCenter, vHalf, mWorld, new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, -1.0f, -1.0f), dwColor, bGizmos);
        }
        //------------------------------------------------------
        public static string DrawUIObjectByPath<T>(string label, string strFile, bool bClear = true, Action onDel = null) where T : UnityEngine.Object
        {
            Color color = GUI.color;
            T asset = AssetDatabase.LoadAssetAtPath<T>(strFile);
            if (asset == null)
            {
                GUI.color = Color.red;
            }
            EditorGUILayout.BeginHorizontal();
            asset = EditorGUILayout.ObjectField(label, asset, typeof(T), false) as T;
            if (asset != null)
                strFile = AssetDatabase.GetAssetPath(asset);
            if (bClear && GUILayout.Button("清除", new GUILayoutOption[] { GUILayout.Width(50) }))
            {
                strFile = "";
            }
            if (onDel != null)
            {
                if (GUILayout.Button("清除", new GUILayoutOption[] { GUILayout.Width(50) }))
                {
                    onDel();
                }
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = color;
            return strFile;
        }
        //------------------------------------------------------
        public static string DrawUIObjectByPathNoLayout<T>(string label, string strFile) where T : UnityEngine.Object
        {
            if (strFile == null) strFile = "";
            Color color = GUI.color;
            T asset = AssetDatabase.LoadAssetAtPath<T>(strFile);
            if (asset == null)
            {
                GUI.color = Color.red;
            }
            asset = EditorGUILayout.ObjectField(label, asset, typeof(T), false) as T;
            if (asset != null)
                strFile = AssetDatabase.GetAssetPath(asset);
            GUI.color = color;
            return strFile;
        }
        //-----------------------------------------------------
        static public AnimatorState[] GetStates(AnimatorStateMachine sm)
        {
            AnimatorState[] stateArray = new AnimatorState[sm.states.Length];
            for (int i = 0; i < sm.states.Length; i++)
            {
                stateArray[i] = sm.states[i].state;
            }
            return stateArray;
        }
        //-----------------------------------------------------
        static public List<AnimatorState> GetStatesRecursive(AnimatorStateMachine sm, bool bChild = true)
        {
            List<AnimatorState> list = new List<AnimatorState>();
            list.AddRange(GetStates(sm));
            if (bChild)
            {
                for (int i = 0; i < sm.stateMachines.Length; i++)
                {
                    list.AddRange(GetStatesRecursive(sm.stateMachines[i].stateMachine));
                }
            }

            return list;
        }
        //-----------------------------------------------------
        static public AnimatorState FindStatesRecursive(AnimatorController controller, string name, bool bChild = true)
        {
            if (controller == null) return null;
            for (int i = 0; i < controller.layers.Length; ++i)
            {
                var stateMachine = controller.layers[i].stateMachine;
                for (int j = 0; j < stateMachine.stateMachines.Length; ++j)
                {
                    var states = stateMachine.stateMachines[j].stateMachine.states;
                    for (int k = 0; k < states.Length; ++i)
                    {
                        if (states[k].state)
                        {
                            if (states[k].state.name.CompareTo(name) == 0)
                                return states[k].state;
                        }
                    }
                }
            }
            return null;
        }
        //-----------------------------------------------------
        public static void ShowGameViewNotification(string message, float seconds = 2f)
        {
            // 获取 GameView 类型
            var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            if (gameViewType == null) return;

            // 获取 GameView 实例
            EditorWindow gameView = EditorWindow.GetWindow(gameViewType);
            if (gameView == null) return;

            // 创建 GUIContent
            GUIContent content = new GUIContent(message);

            // 调用 ShowNotification
            MethodInfo showNotification = typeof(EditorWindow).GetMethod("ShowNotification", new[] { typeof(GUIContent), typeof(float) });
            if (showNotification != null)
            {
                showNotification.Invoke(gameView, new object[] { content, seconds });
            }
            else
            {
                // 兼容旧版本
                gameView.ShowNotification(content);
            }
        }
        //-----------------------------------------------------
        public static string GetScriptNameAtPath(string scriptName, bool bParentPath = false)
        {
            var installPath = AssetDatabase.FindAssets("t:Script " + scriptName);
            if (installPath != null && installPath.Length > 0)
            {
                for (int i = 0; i < installPath.Length; ++i) 
                {
                    string installRoot = AssetDatabase.GUIDToAssetPath(installPath[i]);
                    if(Path.GetFileNameWithoutExtension(installRoot).Equals(scriptName, StringComparison.OrdinalIgnoreCase))
                    {
                        installRoot = installRoot.Replace("\\", "/");
                        if (bParentPath) installRoot = Path.GetDirectoryName(installRoot).Replace("\\", "/");
                        return installRoot;
                    }
                }
            }
            return null;
        }
        //------------------------------------------------------
        public static Texture2D LoadExternalImage(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            byte[] fileData = File.ReadAllBytes(filePath);

            // PNG 文件头宽高解析
            int width = 2, height = 2;
            if (fileData.Length > 24 &&
                fileData[0] == 0x89 && fileData[1] == 0x50 && fileData[2] == 0x4E && fileData[3] == 0x47)
            {
                // PNG: 宽高在第16-23字节（大端序）
                width = (fileData[16] << 24) | (fileData[17] << 16) | (fileData[18] << 8) | fileData[19];
                height = (fileData[20] << 24) | (fileData[21] << 16) | (fileData[22] << 8) | fileData[23];
            }
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.LoadImage(fileData);
            return tex;
        }
        //------------------------------------------------------
        public static bool IsSvnProject()
        {
            string projectPath = Application.dataPath;
            string svnPath = Path.Combine(projectPath, ".svn");
            if(Directory.Exists(svnPath))
                return true;
            svnPath = Path.Combine(projectPath, "../.svn");
            if (Directory.Exists(svnPath))
                return true;
            svnPath = Path.Combine(projectPath, "../../.svn");
            if (Directory.Exists(svnPath))
                return true;
            return false;
        }
        //-----------------------------------------------------
        public static void AddAllChildPaths(Transform parent, string parentPath, List<string> pathList)
        {
            foreach (Transform child in parent)
            {
                string path = string.IsNullOrEmpty(parentPath) ? child.name : parentPath + "/" + child.name;
                pathList.Add(path);
                AddAllChildPaths(child, path, pathList);
            }
        }
        //------------------------------------------------------
        public static bool IsMp4ByHeader(string filePath)
        {
            if (!File.Exists(filePath)) return false;
            byte[] buffer = new byte[32];
            using (var fs = new FileStream(filePath, System.IO.FileMode.Open, FileAccess.Read))
            {
                fs.Read(buffer, 0, buffer.Length);
            }
            string header = System.Text.Encoding.ASCII.GetString(buffer);
            return header.Contains("ftyp");
        }
        //------------------------------------------------------
        public static float GetVideoDuration(string filePath)
        {
            if (!File.Exists(filePath)) return 0;
            Assembly TagLibSharp = null;
            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (ass.GetName().Name == "TagLibSharp")
                {
                    TagLibSharp = ass;
                    break;
                }
            }
            if (TagLibSharp == null)
            {
                string dllFile = "Assets/Plugins/TagLibSharp.dll";
                if (!File.Exists(dllFile))
                {
                    string[] guids = UnityEditor.AssetDatabase.FindAssets("TagLibSharp t:Dll");
                    foreach (string guid in guids)
                    {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                        if (path.EndsWith("TagLibSharp.dll", StringComparison.OrdinalIgnoreCase))
                        {
                            dllFile = path;
                            break;
                        }
                    }
                }

                if (!File.Exists(dllFile))
                    return 0;

                TagLibSharp = Assembly.LoadFrom(dllFile);
            }

            if (TagLibSharp == null) return 0;

            // 获取 TagLib.File 类型
            var fileType = TagLibSharp.GetType("TagLib.File");
            if (fileType == null) return 0;

            // 调用静态方法 File.Create
            var createMethod = fileType.GetMethod("Create", new[] { typeof(string) });
            if (createMethod == null) return 0;

            var tfile = createMethod.Invoke(null, new object[] { filePath });
            if (tfile == null) return 0;

            // 获取 Properties 属性
            var propertiesProp = fileType.GetProperty("Properties");
            var properties = propertiesProp.GetValue(tfile);

            // 获取 Duration 属性
            var durationProp = properties.GetType().GetProperty("Duration");
            var duration = (TimeSpan)durationProp.GetValue(properties);

            return (float)duration.TotalSeconds;
        }
    }
}
#endif