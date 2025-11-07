/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Visualizer
作    者:	HappLI
描    述:	Plus 代理
*********************************************************************/
using System.Runtime.InteropServices;
using UnityEngine;

namespace Framework.Core
{
    public enum ELogType
    {
        Info = 1 << 0,
        Warning = 1 << 1,
        Error = 1 << 2,
        Break = 1 << 3,
        Asset = 1 << 4,
        Exception = 1 << 5,
    }
    public enum EStateStatus
    {
        Init = 0,
        PreStart,
        Start,
    }
    enum EDeviceType
    {
        DT_Editor = 0,
        DT_Window,
        DT_IOS,
        DT_Android,

        DT_SvrWindow = 101,
        DT_SvrLinux = 102,
    };
    public enum EAppStatus
    {
        LowMemory = 0,
        Play,
        Pause,
        Focus,
        Exit,
    }
    public class GameDelegate
    {
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_LOG_FUNC))]
        static void LogEngine(ref sLogsaver LogSave)
        {
            if (LogSave.szStr == null) return;
            string msg = System.Text.Encoding.ASCII.GetString(LogSave.szStr);
            if ((ELogType)LogSave.loglevel == ELogType.Error)
            {
                Debug.LogError(msg);
            }
            else if ((ELogType)LogSave.loglevel == ELogType.Info)
            {
                Debug.Log(msg);
            }
            else
            {
                Debug.LogWarning(msg);
            }
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_DOCHANGESTATE_FUNC))]
        static void DoChangeState(byte state, int userData)
        {
           // GameInstance.getInstance().ChangeState((Logic.EGameState)state);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_CREATESCENE_FUNC))]
        static int CreateScene(int sceneId, ref Vector3 pos, ref Quaternion rotate, ref Vector3 scale)
        {
             return ObjectManager.getInstance().CreateScene((uint)sceneId, pos, rotate.eulerAngles, scale);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_REMOVESCENE_FUNC))]
        static void RemoveScene(int guid)
        {
            if (guid == 0) return;
            ObjectManager.getInstance().RemoveObject(guid);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_CREATEOBJECT_FUNC))]
        static int CreateObjectByModel(int model)
        {
            return ObjectManager.getInstance().CreateModel(model);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_CREATEOBJECT_FUNC))]
        static int CreateObject(ref sCreateObject parameter)
        {
            int valid_lenth = BaseUtil.Strlen(parameter.fileName);
            if (valid_lenth <= 0) return -1;
            string file = System.Text.Encoding.ASCII.GetString(parameter.fileName, 0, valid_lenth);
            string name = System.Text.Encoding.UTF8.GetString(parameter.name);
            return ObjectManager.getInstance().CreateObject(file, name);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_REMOVEOBJECT_FUNC))]
        static void RemoveObject(int guid)
        {
            if (guid == 0) return;
            ObjectManager.getInstance().RemoveObject(guid);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_OBJECT_TRANSFORM_FUNC))]
        static void SetObjectTransform(int guid, ref Vector3 pos, ref Vector3 euler, ref Vector3 scale)
        {
            if (guid == 0) return;
            ObjectManager.getInstance().SetTransform(guid, pos, euler, scale);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_OBJECT_POSITION_FUNC))]
        static void SetObjectPosition(int guid, ref Vector3 pos)
        {
            if (guid == 0) return;
            ObjectManager.getInstance().SetPosition(guid, pos);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_OBJECT_DIRECTION_FUNC))]
        static void SetObjectDirection(int guid, ref Vector3 dir)
        {
            if (guid == 0) return;
            ObjectManager.getInstance().SetDirection(guid, dir, Vector3.up);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_OBJECT_EULERANGLE_FUNC))]
        static void SetObjectEulerAngle(int guid, ref Vector3 eulerAngle)
        {
            if (guid == 0) return;
            ObjectManager.getInstance().SetEulerAngle(guid, eulerAngle);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_OBJECT_DIRECTIONUP_FUNC))]
        static void SetObjectDirectionUp(int guid, ref Vector3 direction, ref Vector3 up)
        {
            if (guid == 0) return;
            ObjectManager.getInstance().SetDirection(guid, direction, up);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_OBJECT_SCALE_FUNC))]
        static void SetObjectScale(int guid, ref Vector3 scale)
        {
            if (guid == 0) return;
            ObjectManager.getInstance().SetScale(guid, scale);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_OBJECT_ANIMATION_FUNC))]
        static void PlayObjectAnimation(ref sTargetAnimation animData)
        {
            ObjectManager.getInstance().PlayAnimation(animData.guid, animData.animation, animData.blendDuration, animData.blendOffset, animData.layer);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_OBJECT_PAUSE_ANIMATION_FUC))]
        static void PauseObjectAnimation(int guid, bool bPause)
        {
            if (guid == 0) return;
            ObjectManager.getInstance().PauseAnimation(guid, bPause);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_OBJECT_SPEED_ANIMATION_FUC))]
        static void SpeedObjectAnimation(int guid, float fspeed)
        {
            if (guid == 0) return;
            ObjectManager.getInstance().SpeedAnimation(guid, fspeed);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_OBJECT_VISIBLE_FUC))]
        static void SetVisibleObject(int guid, bool bVisible)
        {
            if (guid == 0) return;
            ObjectManager.getInstance().SetVisible(guid, bVisible);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_CHECKGUID_FUNC))]
        static bool CheckGUID(int guid)
        {
            if (guid == 0) return false;
            return ObjectManager.getInstance().Contains(guid);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_GETKEYDOWN_FUNC))]
        static bool GetKeyDown(int key)
        {
            return Input.GetKeyDown((KeyCode)key);
        }
        //-------------------------------------------------
        [AOT.MonoPInvokeCallback(typeof(ENGINE_GETKEYUP_FUNC))]
        static bool GetKeyUp(int key)
        {
            return Input.GetKeyUp((KeyCode)key);
        }
        //-------------------------------------------------
        private static sBridgeInterface m_BI;
        private static sInvBridgeInterface m_InvBI;
        private static Visualizer m_pVisualizer;
        private static byte[] ms_Buffer = new byte[512];
        //-------------------------------------------------
        public static void StartUp(AFileSystem fileSystem)
        {
            if (fileSystem == null)
            {
                return;
            }
#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log("Web Platform disable CorePlus");
            fileSystem.InitPackages();
#else
            InnerStartUp(fileSystem);

            fileSystem.InitPackages();
#endif
        }
        //-------------------------------------------------
        public static System.Collections.IEnumerator CoroutineStartUp(AFileSystem fileSystem, System.Action<bool> callback)
        {
            if (fileSystem == null)
            {
                yield return false;
                yield break;
            }
#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log("Web Platform disable CorePlus");
#else
            InnerStartUp(fileSystem);
#endif
            yield return fileSystem.StartCoroutine(fileSystem.CoroutineInitPackages(callback));
        }
        //-------------------------------------------------
        static void InnerStartUp(AFileSystem fileSystem)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
#else
            if (fileSystem == null)
            {
                return;
            }
    #if UNITY_EDITOR
            CorePlus_SetPath(fileSystem.AssetPath, Application.dataPath + "/../Binarys/", fileSystem.PersistenDataPath);
#else
                    CorePlus_SetPath(fileSystem.AssetPath, fileSystem.StreamPath, fileSystem.PersistenDataPath);
#endif
            try
            {
                m_pVisualizer = new Visualizer();
                m_BI = new sBridgeInterface();
                m_InvBI = new sInvBridgeInterface();
                m_InvBI.ToggleLog = 1;
                m_InvBI.LogLevel = 0xffffffff;
                m_InvBI.logFunc = LogEngine;
                m_InvBI.changeState = DoChangeState;
                m_InvBI.createScene = CreateScene;
                m_InvBI.removeScene = RemoveScene;
                m_InvBI.createObjectByModel = CreateObjectByModel;
                m_InvBI.createObject = CreateObject;
                m_InvBI.removeObject = RemoveObject;
                m_InvBI.setTransform = SetObjectTransform;
                m_InvBI.setPosition = SetObjectPosition;
                m_InvBI.setEulerAngle = SetObjectEulerAngle;
                m_InvBI.setDirection = SetObjectDirection;
                m_InvBI.setDirectionUp = SetObjectDirectionUp;
                m_InvBI.setScale = SetObjectScale;
                m_InvBI.playAnimation = PlayObjectAnimation;
                m_InvBI.pauseAnimation = PauseObjectAnimation;
                m_InvBI.speedAnimation = SpeedObjectAnimation;
                m_InvBI.setVisible = SetVisibleObject;
                m_InvBI.checkGuid = CheckGUID;
                m_InvBI.getKeyDown = GetKeyDown;
                m_InvBI.getKeyUp = GetKeyUp;

                m_pVisualizer.Init(ref m_InvBI);
                CorePlus_Inv_BI(ref m_InvBI);

                string strArgvs = "version:100;thread:0";
#if UNITY_EDITOR
                strArgvs += string.Format(";device:{0}", EDeviceType.DT_Editor);
#elif UNITY_ANDROID
            strArgvs += string.Format(";device:{0}", EDeviceType.DT_Android);
#elif UNITY_IOS
            strArgvs += string.Format(";device:{0}", EDeviceType.DT_IOS);
#endif
                strArgvs += string.Format(";visualizer:{0}", 1);
                strArgvs += string.Format(";spatial:{0}", 0);
                strArgvs += string.Format(";navmesh:{0}", 1);
                strArgvs += string.Format(";threadCnt:{0}", 0);
                strArgvs += string.Format(";package:{0}", fileSystem.eType == EFileSystemType.EncrptyPak ? 1 : 0);
                strArgvs += string.Format(";filesys_log:{0}", 0);
                strArgvs += string.Format(";catch_file_limit:{0}", 0);
                strArgvs += string.Format(";catch_file_maxsize:{0}", 0);
                strArgvs += string.Format(";catch_file_suffixs:{0}", 0);
                CorePlus_Startup(ref m_BI, strArgvs);
                JniPlugin plugin = new JniPlugin();
#endif
            }
            catch(System.Exception ex)
            {
                Debug.LogWarning(ex);
            }

        }
        //-------------------------------------------------
        public static System.IntPtr LoadCsv(int type, byte[] bytes)
        {
            return System.IntPtr.Zero;
            //if (m_BI.OnLoader == null) return System.IntPtr.Zero;
            //return m_BI.OnLoader((int)EBufferType.BT_Csv, type, ref bytes[0], bytes.Length);
        }
        //-------------------------------------------------
        public static void OnStatus(EAppStatus status)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported OnStatus API");
#else
            CorePlus_OnStatus((uint)status);
#endif
        }
        //-------------------------------------------------
        public static void OnChangeState(uint state, EStateStatus status, int userData)
        {
           // CorePlus_ChangeState(state, (int)status, userData);
        }
        //-------------------------------------------------
        public static float GetLoadingProcess()
        {
            // return CorePlus_GetLoadingProcess();
            return 0;
        }
        //-------------------------------------------------
        public static System.IntPtr OnLoadAssetCallBack(int type, int guid, uint userData, bool bSuccssed)
        {
            //  return CorePlus_OnLoadAssetCallBack(type, guid, userData, bSuccssed);
            return System.IntPtr.Zero;
        }
        //-------------------------------------------------
        public static bool GameRun(float fFrameTime)
        {
//             if (CorePlus_Run((uint)(Time.time * 1000), (uint)(fFrameTime * 1000), Time.frameCount))
//             {
//                 return true;
//             }
            return false;
        }
        //-------------------------------------------------
        public static void PostRender(Camera camera)
        {
            //        m_pVisualizer.renderQuad3D(Vector3.zero, new Vector3(Screen.width-5, 0, 0), new Vector3(Screen.width-5, Screen.height-5, 0), new Vector3(0, Screen.height-5, 0), new Vector4(1, 0, 0, 1));
            //        m_pVisualizer.renderQuad2D(Vector3.zero, new Vector3(100, 0, 0), new Vector3(100, 100, 0), new Vector3(0, 100, 0), new Vector4(1, 1, 0, 0.5f));
            m_pVisualizer.Render(camera);
        }
        //-------------------------------------------------
        public static void GameTouchBegin(int touchId, Vector2 mouse)
        {
          //  CorePlus_OnTouchBegin(touchId, mouse.x, mouse.y);
        }
        //-------------------------------------------------
        public static void GameOnTouchMove(int touchId, Vector2 mouse)
        {
          //  CorePlus_OnTouchMove(touchId, mouse.x, mouse.y);
        }
        //-------------------------------------------------
        public static void GameOnMouseWheel(float wheel)
        {
        //    CorePlus_OnMouseWheel(wheel);
        }
        //-------------------------------------------------
        public static void GameOnTouchEnd(int touchId, Vector2 mouse)
        {
           // CorePlus_OnTouchEnd(touchId, mouse.x, mouse.y);
        }
        //-------------------------------------------------
        public static void PorcessCmd(string cmd)
        {
            if (m_BI.OnCommond == null) return;
            m_BI.OnCommond(cmd);
        }
        //-------------------------------------------------
        public static void EnablePackage(bool bEnable)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus EnablePackage API");
#else
            CorePlus_EnablePackage(bEnable);
#endif
        }
        //-------------------------------------------------
        public static void EnableCatchHandle(bool bEnable, int nCatchCount=64)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus EnableCatchHandle API");
#else
            CorePlus_EnableCatchHandle(bEnable, nCatchCount);
#endif
        }
        //-------------------------------------------------
        public static System.IntPtr LoadPackage(string file)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus LoadPackage API");
            return System.IntPtr.Zero;
#else
            return CorePlus_LoadPackage(file);
#endif
        }
        //-------------------------------------------------
        public static void UnloadPackage(string file)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus UnloadPackage API");
#else
            CorePlus_UnloadPackage(file);
#endif
        }
        //-------------------------------------------------
        public static void DeleteAllPackages()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus DeleteAllPackages API");
#else
            CorePlus_DeleteAllPackages();
#endif
        }
        //-------------------------------------------------

        public static System.IntPtr CreateEmptyPackage()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
           UnityEngine.Debug.LogWarning("not spported CorePlus CreateEmptyPackage API");
            return System.IntPtr.Zero;
#else
            return CorePlus_CreateEmptyPackage();
#endif
        }
        //-------------------------------------------------
        public static System.IntPtr FindEntryPackage(string pakFileName)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus FindEntryPackage API");
            return System.IntPtr.Zero;
#else
            return CorePlus_FindEntryPackage(pakFileName);
#endif
        }
        //-------------------------------------------------
        public static System.IntPtr GetEntryPackage(System.IntPtr packageHanlde, int index)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus GetEntryPackage API");
            return System.IntPtr.Zero;
#else
            return CorePlus_GetEntryPackage(packageHanlde, index);
#endif
        }
        //-------------------------------------------------
        public static void CreatePackageEntry(System.IntPtr pPackageHandle, byte[] pData, uint nDataSize, string strNamne, bool bEncrpty)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus CreatePackageEntry API");
#else
            CorePlus_CreatePackageEntry(pPackageHandle, ref pData[0], nDataSize, strNamne, bEncrpty);
#endif
        }
        //-------------------------------------------------
        public static string GetEntryPackageMd5(System.IntPtr pEnterPackageHandle)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus GetEntryPackageMd5 API");
            return string.Empty;
#else
            if (CorePlus_GetEntryPackageMd5(pEnterPackageHandle, ref ms_Buffer[0], 32))
            {
                return System.Text.Encoding.ASCII.GetString(ms_Buffer, 0, 32);
            }
            else
                return string.Empty;
#endif
        }
        //-------------------------------------------------
        public static string GetEntryPackageFileName(System.IntPtr pEnterPackageHandle)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus GetEntryPackageFileName API");
            return string.Empty;
#else
            int cnt = CorePlus_GetEntryPackageFileName(pEnterPackageHandle, ref ms_Buffer[0], 256);
            if (cnt>0)
            {
                return System.Text.Encoding.ASCII.GetString(ms_Buffer, 0, cnt);
            }
            else
                return string.Empty;
#endif
        }
        //------------------------------------------------------
        public static int GetPackageEntryCount(System.IntPtr pPackageHandle)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus GetPackageEntryCount API");
            return 0;
#else
            return CorePlus_GetPackageEntryCount(pPackageHandle);
#endif
        }
        
        //-------------------------------------------------
        public static bool SavePackage(System.IntPtr pPackageHandle, bool bAutoDestroy = false)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus SavePackage API");
            return false; 
#else
            return CorePlus_SavePackage(pPackageHandle, bAutoDestroy);
#endif
        }
        //-------------------------------------------------
        public static void SetPackageAbsFilePath(System.IntPtr pPackageHandle, string strAbsFilePath)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus SetPackageAbsFilePath API");
#else
            CorePlus_SetPackageAbsFilePath(pPackageHandle, strAbsFilePath);
#endif
        }
        //-------------------------------------------------
        public static void SetPackageVersion(System.IntPtr pPackageHandle, uint nVersion)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus SetPackageVersion API");
#else
            CorePlus_SetPackageVersion(pPackageHandle, nVersion);
#endif
        }
        //-------------------------------------------------
        public static int DecompressLZ4(ref byte source, ref byte dest, int compressedSize, int maxDecompressedSize)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus DecompressLZ4 API");
            return 0;
#else
            return CorePlus_decompressLZ4(ref source, ref dest, compressedSize, maxDecompressedSize);
#endif
        }
        //-------------------------------------------------
        public static string Md5(byte[] bytes, int len)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus Md5 API"); 
            return string.Empty;
#else
            if (CorePlus_buildMd5(ref bytes[0], len, ref ms_Buffer[0], 32))
            {
                return System.Text.Encoding.ASCII.GetString(ms_Buffer,0, 32);
            }
            else
                return string.Empty;
#endif
        }
        //-------------------------------------------------
        public static bool EncryptBuffer(ref byte[] pInBuffer, int nBuffSize, int cipherRemains)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus EncryptBuffer API");
            return false;
#else
            return CorePlus_EncryptBuffer(ref pInBuffer[0], nBuffSize, null, cipherRemains);
#endif
        }
        //-------------------------------------------------
        public static bool DecodeBuffer(ref byte[] pInBuffer, int nBuffSize, int cipherRemains)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus DecodeBuffer API");
            return false;
#else
            return CorePlus_DecodeBuffer(ref pInBuffer[0], nBuffSize, null, cipherRemains);
#endif
        }
        //-------------------------------------------------
        public static bool EncryptBuffer1(ref byte[] pInBuffer, int nOffset, int nBuffLen, byte[] arrKeys)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus EncryptBuffer1 API");
            return false;
#else
            return CorePlus_EncryptBuffer1(ref pInBuffer[0], nOffset, nBuffLen, arrKeys, arrKeys != null ? arrKeys.Length : 0);
#endif
        }
        //-------------------------------------------------
        public static bool DecodeBuffer1(ref byte[] pInBuffer, int nOffset, int nBuffLen, byte[] arrKeys)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus DecodeBuffer1 API");
            return false;
#else
            return CorePlus_DecodeBuffer1(ref pInBuffer[0], nOffset, nBuffLen, arrKeys, arrKeys!=null?arrKeys.Length:0);
#endif
        }
        //-------------------------------------------------
        public static int GetFileSize(string strFile, bool bAbs)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus GetFileSize API");
            return 0;
#else
            return CorePlus_GetFileSize(strFile, bAbs);
#endif
        }
        //-------------------------------------------------
        public static bool FileExist(string strFile, bool bAbs)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus FileExist API");
            return false;
#else
            return CorePlus_FileExist(strFile, bAbs);
#endif
        }
        //-------------------------------------------------
        public static byte[] ReadFile(string strFile, bool bAbs, ref int dataSize)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus ReadFile API");
            return null;
#else
            dataSize = GetFileSize(strFile, bAbs);
            if (dataSize <= 0) return null;
            if (ms_Buffer.Length <= dataSize)
                ms_Buffer = new byte[dataSize];
            if (CorePlus_ReadFile(strFile, ref ms_Buffer[0], dataSize, bAbs) > 0)
            {
                return ms_Buffer;
            }
            return null;
#endif
        }
        //-------------------------------------------------
        public static int ReadBufferFile(byte[] buffer, string strFile, bool bAbs, ref int dataSize)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus ReadBufferFile API");
            return 0;
#else
            return CorePlus_ReadFile(strFile, ref buffer[0], dataSize, bAbs);
#endif
        }
        //-------------------------------------------------
        public static int ReadBuffer(string strFile, byte[] buffer, int dataSize, int bufferOffset, int offsetRead, bool bAbs)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.LogWarning("not spported CorePlus ReadBuffer API");
            return 0;
#else
            return CorePlus_ReadBuffer(strFile, ref buffer[bufferOffset], dataSize, offsetRead, bAbs);
#endif
        }
#if UNITY_WEBGL && !UNITY_EDITOR
#else
        //-------------------------------------------------
#if !UNITY_EDITOR && UNITY_IPHONE
        const string CoreDLL = "__Internal";
#else
        const string CoreDLL = "CorePlus";
#endif

        [DllImport(CoreDLL)]
        static extern void CorePlus_SetPath(string path, string streamPath, string externPath);

        [DllImport(CoreDLL)]
        static extern void CorePlus_Startup(ref sBridgeInterface BI, string argvs);

        [DllImport(CoreDLL, CharSet = CharSet.Unicode, SetLastError =true)]
        static extern void CorePlus_OnStatus(uint status);

     //   [DllImport(CoreDLL, CharSet = CharSet.Unicode, SetLastError = true)]
     //   static extern void CorePlus_ChangeState(uint state, int status, int userData);

     //   [DllImport(CoreDLL)]
     //   static extern System.IntPtr CorePlus_OnLoadAssetCallBack(int type, int guid, uint userData, bool bSuccssed);

     //   [DllImport(CoreDLL)]
     //   static extern float CorePlus_GetLoadingProcess();

     //   [DllImport(CoreDLL)]
    //    static extern bool CorePlus_Run(uint runTime, uint timeFrame,int frameCnt);

        [DllImport(CoreDLL)]
        static extern void CorePlus_OnTouchBegin(int touchId, float x, float y);

        [DllImport(CoreDLL)]
        static extern void CorePlus_OnTouchMove(int touchId, float x, float y);

        [DllImport(CoreDLL)]
        static extern void CorePlus_OnMouseWheel( float wheel);

        [DllImport(CoreDLL)]
        static extern void CorePlus_OnTouchEnd(int touchId, float x, float y);

        [DllImport(CoreDLL)]
        static extern void CorePlus_Inv_BI(ref sInvBridgeInterface InvBI);

        [DllImport(CoreDLL)]
        static extern int CorePlus_decompressLZ4(ref byte source, ref byte dest, int compressedSize, int maxDecompressedSize);

        [DllImport(CoreDLL)]
        static extern bool CorePlus_buildMd5(ref byte source, int size, ref byte outMd5, int md5Size);

        [DllImport(CoreDLL)]
        static extern bool CorePlus_EncryptBuffer(ref byte pInBuffer, int nBuffSize, int[] arrKeys, int cipherRemains);

        [DllImport(CoreDLL)]
        static extern bool CorePlus_DecodeBuffer(ref byte pInBuffer, int nBuffSize, int[] arrKeys, int cipherRemains);

        [DllImport(CoreDLL)]
        static extern bool CorePlus_EncryptBuffer1(ref byte pInBuffer, int nOffset, int nBuffLen, byte[] arrKeys, int keyLen);

        [DllImport(CoreDLL)]
        static extern bool CorePlus_DecodeBuffer1(ref byte pInBuffer, int nOffset, int nBuffLen, byte[] arrKeys, int keyLen);

        [DllImport(CoreDLL)]
        static extern bool CorePlus_FileExist(string file, bool bAbs);

        [DllImport(CoreDLL)]
        static extern int CorePlus_GetFileSize(string file, bool bAbs);

        [DllImport(CoreDLL)]
        static extern int CorePlus_ReadFile(string file, ref byte data, int dataSize, bool bAbs);

        [DllImport(CoreDLL)]
        static extern int CorePlus_ReadBuffer(string file, ref byte data, int dataSize, int offset, bool bAbs);

        [DllImport(CoreDLL)]
        static extern void CorePlus_EnablePackage(bool enable);
        [DllImport(CoreDLL)]
        static extern void CorePlus_EnableCatchHandle(bool enable,int nCatchCnt);

        [DllImport(CoreDLL)]
        static extern System.IntPtr CorePlus_LoadPackage(string file);

        [DllImport(CoreDLL)]
        static extern void CorePlus_UnloadPackage(string file);

        [DllImport(CoreDLL)]
        static extern void CorePlus_DeleteAllPackages();

        [DllImport(CoreDLL)]
        static extern System.IntPtr CorePlus_CreateEmptyPackage();

        [DllImport(CoreDLL)]
        static extern System.IntPtr CorePlus_FindEntryPackage(string pakFileName);

        [DllImport(CoreDLL)]
        static extern System.IntPtr CorePlus_GetEntryPackage(System.IntPtr pPackageHandle, int index);

        [DllImport(CoreDLL)]
        static extern void CorePlus_CreatePackageEntry(System.IntPtr pPackageHandle, ref byte pData, uint nDataSize, string strNamne, bool bEncrpty);

        [DllImport(CoreDLL)]
        static extern bool CorePlus_GetEntryPackageMd5(System.IntPtr pEnterPackageHandle, ref byte pData, uint nDataSize);

        [DllImport(CoreDLL)]
        static extern int CorePlus_GetEntryPackageFileName(System.IntPtr pEnterPackageHandle, ref byte pFile, uint nLen);

        [DllImport(CoreDLL)]
        static extern int CorePlus_GetPackageEntryCount(System.IntPtr pPackageHandle);

        [DllImport(CoreDLL)]
        static extern bool CorePlus_SavePackage(System.IntPtr pPackageHandle, bool bAutoDestroy);

        [DllImport(CoreDLL)]
        static extern void CorePlus_SetPackageAbsFilePath(System.IntPtr pPackageHandle, string strAbsFilePath);

        [DllImport(CoreDLL)]
        static extern void CorePlus_SetPackageVersion(System.IntPtr pPackageHandle, uint nVersion);
#endif
    }
}