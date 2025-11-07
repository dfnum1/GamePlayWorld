/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Visualizer
作    者:	HappLI
描    述:	unity 接口交互定义
*********************************************************************/
using UnityEngine;
using System.Runtime.InteropServices;
namespace Framework.Core
{
    public delegate void    ENGINE_LOG_FUNC(ref sLogsaver LogSave);
    public delegate void    ENGINE_DOCHANGESTATE_FUNC(byte state, int userData);
    public delegate bool    ENGINE_CHECKGUID_FUNC(int guid);
    public delegate bool    ENGINE_GETKETDOWN_FUNC(int key);
    public delegate bool    ENGINE_GETKETUP_FUNC(int key);
    public delegate int     ENGINE_CREATESCENE_FUNC(int sceneId, ref Vector3 pos, ref Quaternion rotate, ref Vector3 scale);
    public delegate void    ENGINE_REMOVESCENE_FUNC(int guid);
    public delegate int     ENGINE_CREATEOBJECTBYMODEL_FUNC(int modelId);
    public delegate int     ENGINE_CREATEOBJECT_FUNC (ref sCreateObject obj);
	public delegate void    ENGINE_REMOVEOBJECT_FUNC (int guid);
	public delegate void    ENGINE_OBJECT_TRANSFORM_FUNC(int guid,ref Vector3 pos, ref Vector3 eulerAngle, ref Vector3 scale);
	public delegate void    ENGINE_OBJECT_POSITION_FUNC(int guid,ref Vector3 pos);
	public delegate void    ENGINE_OBJECT_DIRECTION_FUNC (int guid,ref Vector3 dir);
	public delegate void    ENGINE_OBJECT_DIRECTIONUP_FUNC (int guid,ref Vector3 dir,ref Vector3 up);
	public delegate void    ENGINE_OBJECT_EULERANGLE_FUNC (int guid,ref Vector3 eulerAngle);
	public delegate void    ENGINE_OBJECT_SCALE_FUNC (int guid,ref Vector3 scale);
	public delegate void    ENGINE_OBJECT_ANIMATION_FUNC (ref sTargetAnimation anim);
	public delegate void    ENGINE_OBJECT_PAUSE_ANIMATION_FUC(int guid, bool pause);
	public delegate void    ENGINE_OBJECT_SPEED_ANIMATION_FUC(int guid, float speed);
    public delegate void    ENGINE_OBJECT_VISIBLE_FUC(int guid, bool visible);
    public delegate bool    ENGINE_GETKEYUP_FUNC (int key);
	public delegate bool    ENGINE_GETKEYDOWN_FUNC (int key);
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct sInvBridgeInterface
    {
        public byte                                 ToggleLog;
        public uint                                 LogLevel;
        public ENGINE_LOG_FUNC logFunc;
        public ENGINE_DOCHANGESTATE_FUNC changeState;
        public ENGINE_CREATESCENE_FUNC createScene;
        public ENGINE_REMOVESCENE_FUNC removeScene;
        public ENGINE_CREATEOBJECTBYMODEL_FUNC createObjectByModel;
        public ENGINE_CREATEOBJECT_FUNC createObject;
        public ENGINE_REMOVEOBJECT_FUNC removeObject;
        public ENGINE_OBJECT_TRANSFORM_FUNC setTransform;
        public ENGINE_OBJECT_POSITION_FUNC setPosition;
        public ENGINE_OBJECT_EULERANGLE_FUNC setEulerAngle;
        public ENGINE_OBJECT_DIRECTION_FUNC setDirection;
        public ENGINE_OBJECT_DIRECTIONUP_FUNC setDirectionUp;
        public ENGINE_OBJECT_SCALE_FUNC setScale;
        public ENGINE_OBJECT_ANIMATION_FUNC playAnimation;
        public ENGINE_OBJECT_PAUSE_ANIMATION_FUC pauseAnimation;
        public ENGINE_OBJECT_SPEED_ANIMATION_FUC speedAnimation;
        public ENGINE_OBJECT_VISIBLE_FUC setVisible;
        public ENGINE_CHECKGUID_FUNC checkGuid;
        public ENGINE_GETKETDOWN_FUNC getKeyDown;
        public ENGINE_GETKETUP_FUNC getKeyUp;
        public Visualizer.sInvInterface visualizer;
    };

    public delegate System.IntPtr DLL_BI_OnLoadType(int type, int subType, ref byte buffer, int size);
    public delegate void DLL_BI_Commond(string cmd);
    public struct sBridgeInterface
    {
        public DLL_BI_OnLoadType            OnLoader;
        public DLL_BI_Commond               OnCommond;
        public void Clear()
        {

        }
    };
}