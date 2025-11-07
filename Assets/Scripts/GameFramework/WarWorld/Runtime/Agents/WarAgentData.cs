/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	WarAgentData
作    者:	HappLI
描    述:	War行为体数据
*********************************************************************/
#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
using FQuaternion = UnityEngine.Quaternion;
using FMatrix4x4 = UnityEngine.Matrix4x4;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public class WarAgentData : ScriptableObject
    {
    }
}
