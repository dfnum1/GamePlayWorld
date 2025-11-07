/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ConfigUtil
作    者:	HappLI
描    述:	框架配置
*********************************************************************/
using Framework.Core;
using System.Collections.Generic;
using UnityEngine;
#if USE_SERVER
using Material = ExternEngine.Material;
#endif

using ExternEngine;
#if !USE_FIXEDMATH
using FVector3 = UnityEngine.Vector3;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
namespace Framework.Base
{
    public class ConfigUtil
    {
        public static HashSet<int> vCollisionFilterElements = new HashSet<int>();
        public static HashSet<int> vMonsterCollisionFilterElements = new HashSet<int>();

        public static FFloat ConversionRatio = 0;
        public static uint[] runSkillSections = null;
        public static int globalCommandSkillCD = 2000;
        public static FFloat ConstMinHurt = 0;
        public static FFloat ConstHurt1 = 1;
        public static FFloat ConstHurt2 = 0;
        public static FFloat ConstDef1 = 1;
        public static FFloat ConstDef2 = 1;
        public static FFloat fBattleRunSpeed = 10;
        public static FFloat DefaultBattleRegionWidth = 11;
        public static int RoadCount = 3;

        public static FFloat timeHorizon = 5;
        public static FFloat timeHorizonObst = 5;

        public static FFloat lowerBattleSpeed = 1;

        public static ushort[] bitObstacleIgnoreFilter = new ushort[(int)EActorType.Count];

        public static string ImprisonLinkEffect = null;
        public static string LinkEffect = null;
        public static string PartAimPoint = null;

        public static int globalTeamSoulFlagDefer = 1000; 

        public static bool bDamageDebug = false;
        public static bool bSkillDebug = false;
        public static bool bBattleFameWriteLogFile = false;

        public static bool bShowSpatial = false;
        public static bool bShowNodeDebugFrame = false;
        public static bool bEventTriggerDebug = false;

        public static bool bEnableGuide = true;
        public static bool bEnableGuideSkip = false;
        public static bool bGuideLogEnable = false;

        private static bool ms_bSupportInstance = true;
        private static bool m_bProfilerDebug = false;
        public static bool bProfilerDebug
        {
            get
            {
#if UNITY_EDITOR || USE_SERVER
                return m_bProfilerDebug;
#else
                return false;
#endif
            }
            set
            {
                m_bProfilerDebug = value;
            }
        }

        //------------------------------------------------------
        public static void Init()
        {
        }
        //------------------------------------------------------
        public static ushort GetObstacleFilter(EActorType type)
        {
            if (bitObstacleIgnoreFilter == null || (int)type >= bitObstacleIgnoreFilter.Length)
                return 0;
            return bitObstacleIgnoreFilter[(int)type];
        }
        //------------------------------------------------------
        public static void EnableSupportInstancing(bool bEnable)
        {
            ms_bSupportInstance = bEnable;
#if !USE_SERVER
            if (!ms_bSupportInstance)
                Shader.DisableKeyword("UNITY_SUPPORT_INSTANCING");
#endif
        }
        //------------------------------------------------------
        public static bool IsSupportInstancing
        {
            get { return ms_bSupportInstance; }
            set
            {
                EnableSupportInstancing(value);
            }
        }
        //------------------------------------------------------
        public static long ConverVersion(string version)
        {
            long result = 0;
            long.TryParse(version.Replace(".", ""), out result);
            return result;
        }
    }
}
