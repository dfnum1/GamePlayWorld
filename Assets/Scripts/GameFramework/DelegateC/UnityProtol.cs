/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Visualizer
作    者:	HappLI
描    述:	Plus 层协议定义
*********************************************************************/
using UnityEngine;
using System.Runtime.InteropServices;
namespace Framework.Core
{
    //-------------------------------------------------
    //! EBufferType
    //-------------------------------------------------
    public enum EBufferType : byte
    {
        BT_None = 0,
        BT_Csv = 1,
    };
    //-------------------------------------------------
    //! ECsvDataType
    //-------------------------------------------------
    public enum ECsvDataType : byte
    {
        CsvDataType_Min = 0,
        CsvDataType_Player,
        CsvDataType_Npc,
        CsvDataType_ActionProperty,
        CsvDataType_Max
    };
    //-------------------------------------------------
    //! sLogsaver
    //-------------------------------------------------
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct sLogsaver
    {
        public uint loglevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] szStr;
    };
    //-------------------------------------------------
    //! sCreateObject
    //-------------------------------------------------
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct sCreateObject
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[]           fileName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[]           name;
    };
    //-------------------------------------------------
    //! sTargetAnimation
    //-------------------------------------------------
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct sTargetAnimation
    {
        public int      guid;
        public float    blendDuration;
        public float    blendOffset;
        public int      animation;
        public int      layer;

        public static sTargetAnimation Default = new sTargetAnimation()
        {
            guid = 0,
            blendDuration = 0,
            blendOffset = 0,
            animation = 0,
            layer = 0,
        };
    };
}