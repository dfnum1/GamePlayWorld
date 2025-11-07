/********************************************************************
生成日期:	07:23:2025
类    名: 	CutsceneUtil
作    者:	HappLI
描    述:	工具类
*********************************************************************/
using UnityEngine;
namespace Framework.Cutscene.Runtime
{
    public class CutsceneUtil
    {
        public static bool RayInsectionFloor(out Vector3 retPos, Vector3 pos, Vector3 dir, float floorY = 0)
        {
            retPos = Vector3.zero;
            Vector3 vPlanePos = Vector3.zero;
            vPlanePos.y = floorY;

            Vector3 vPlaneNor = Vector3.up;

            float fdot = Vector3.Dot(dir, vPlaneNor);
            if (fdot == 0.0f)
                return false;

            float fRage = ((vPlanePos.x - pos.x) * vPlaneNor.x + (vPlanePos.y - pos.y) * vPlaneNor.y + (vPlanePos.z - pos.z) * vPlaneNor.z) / fdot;

            retPos = pos + dir * fRage;
            return true;
        }
        //-----------------------------------------------------
        public static Vector3 GetPosition(Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }
    }
}