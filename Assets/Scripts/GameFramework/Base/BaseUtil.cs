using Framework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public enum EResetType
{
    Local,
    World,
    All,
}

public static class BaseUtil
{
    public static float POINGPONG_MAX_TIME = 1000000;
    public static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private static StringBuilder ms_StringBuilder = null;
    public static List<int> ms_TempList = null;
    public static StringBuilder stringBuilder
    {
        get
        {
            if (ms_StringBuilder == null) ms_StringBuilder = new StringBuilder(128);
            ms_StringBuilder.Clear();
            return ms_StringBuilder;
        }
    }
#if UNITY_EDITOR
    //------------------------------------------------------
    public static List<string> GetDirectoryFilesName(string path)
    {
        List<string> fileNames = new List<string>();
        if (Directory.Exists(path))
        {
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                fileNames.Add(f.Name);
            }
        }
        return fileNames;
    }
#endif
    //------------------------------------------------------
    public static int Strlen(byte[] pChar)
    {
        if (pChar == null) return 0;
        int valid_lenth = pChar.Length;
        for (; valid_lenth > 0; --valid_lenth)
        {
            if (pChar[valid_lenth - 1] != 0) break;
        }
        return valid_lenth;
    }
    //-----------------------------------------------------
    public static bool Equal(Vector2 v0, Vector2 v1, float failover = 0.01f)
    {
        if (Mathf.Abs(v0.x - v1.x) > failover || Mathf.Abs(v0.y - v1.y) > failover) return false;
        return true;
    }
    //-----------------------------------------------------
    public static bool Equal(Vector3 v0, Vector3 v1, float failover = 0.01f)
    {
        if (Mathf.Abs(v0.x - v1.x) > failover || Mathf.Abs(v0.y - v1.y) > failover || Mathf.Abs(v0.z - v1.z) > failover) return false;
        return true;
    }
    //-----------------------------------------------------
    public static bool Equal(Color v0, Color v1, float failover = 0.01f)
    {
        if (Mathf.Abs(v0.r - v1.r) > failover || Mathf.Abs(v0.g - v1.g) > failover || Mathf.Abs(v0.b - v1.b) > failover || Mathf.Abs(v0.a - v1.a) > failover) return false;
        return true;
    }
    //------------------------------------------------------
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
    //------------------------------------------------------
    public static Vector3 RayHitPos(Ray ray, float floorY = 0)
    {
        Vector3 retPos;
        if (RayInsectionFloor(out retPos, ray.origin, ray.direction, floorY))
            return retPos;
        return Vector3.zero;
    }
    //------------------------------------------------------
    public static Vector3 RayHitPos(Vector3 pos, Vector3 dir, float floorY = 0)
    {
        Vector3 retPos;
        if (RayInsectionFloor(out retPos, pos, dir, floorY))
            return retPos;
        return Vector3.zero;
    }
    //-----------------------------------------------------------------------------
    public static void CU_GetQuaternionFromDirection(Vector3 vDirection, Vector3 vUp, ref Quaternion qRot)
    {
        if (vDirection.sqrMagnitude <= 0.001f) vDirection = Vector3.forward;
        if (vUp.sqrMagnitude <= 0.001f) vUp = Vector3.up;
        qRot = Quaternion.LookRotation(vDirection, vUp);
    }
    //-----------------------------------------------------------------------------
    public static void CU_GetDirectionFromQuaternionLerp(ref Quaternion qRot0, ref Quaternion qRot1, float t, ref Vector3 vDirection)
    {
        Quaternion q = Quaternion.Lerp(qRot1, qRot0, t);
        vDirection = q * Vector3.forward;
    }
    //-----------------------------------------------------
    static public Quaternion LookRotation(Vector3 lookAt, Vector3 pos, Vector3 up)
    {
        Vector3 vDir = lookAt - pos;
        if (vDir.sqrMagnitude <= 0) return Quaternion.identity;
        return Quaternion.LookRotation(vDir, up);
    }
    //-----------------------------------------------------
    static public Quaternion LookRotation(Vector3 lookAt, Vector3 pos)
    {
        Vector3 vDir = lookAt - pos;
        if (vDir.sqrMagnitude <= 0) return Quaternion.identity;
        return Quaternion.LookRotation(vDir, Vector3.up);
    }
    //-----------------------------------------------------
    static public Vector3 EulersAngleToDirection(Vector3 eulerAngle)
    {
        Quaternion qt = Quaternion.identity;
        qt.eulerAngles = eulerAngle;
        return qt * Vector3.forward;
    }
    //-----------------------------------------------------
    static public Vector3 DirectionToEulersAngle(Vector3 dir)
    {
        if (dir.sqrMagnitude > 1) dir.Normalize();
        return Quaternion.FromToRotation(Vector3.forward, dir).eulerAngles;
    }
    //-----------------------------------------------------
    static public Vector3 DirectionToEulersAngle(Vector3 dir, Vector3 up)
    {
        return Quaternion.LookRotation(dir, up).eulerAngles;
    }
#if USE_FIXEDMATH
    //-----------------------------------------------------
    static public void UpdatePosition(ref ExternEngine.FMatrix4x4 mtWorld, ExternEngine.FVector3 position)
    {
        ExternEngine.FVector4 colum = mtWorld.GetColumn(3);
        colum.x = position.x;
        colum.y = position.y;
        colum.z = position.z;
        mtWorld.SetColumn(3, colum);
    }
    //-----------------------------------------------------
    static public void OffsetPosition(ref ExternEngine.FMatrix4x4 mtWorld, ExternEngine.FVector3 offset)
    {
        if (offset.sqrMagnitude <= 0) return;
        ExternEngine.FVector4 colum = mtWorld.GetColumn(3);
        colum.x += offset.x;
        colum.y += offset.y;
        colum.z += offset.z;
        mtWorld.SetColumn(3, colum);
    }
    //-----------------------------------------------------
    static public void UpdateScale(ref ExternEngine.FMatrix4x4 mtWorld, ExternEngine.FVector3 scale)
    {
        mtWorld.m00 = scale.x;
        mtWorld.m11 = scale.y;
        mtWorld.m22 = scale.z;
    }
#endif
    //-----------------------------------------------------
    static public void UpdatePosition(ref Matrix4x4 mtWorld, Vector3 position)
    {
        Vector4 colum = mtWorld.GetColumn(3);
        colum.x = position.x;
        colum.y = position.y;
        colum.z = position.z;
        mtWorld.SetColumn(3, colum);
    }
    //-----------------------------------------------------
    static public void OffsetPosition(ref Matrix4x4 mtWorld, Vector3 offset)
    {
        if (offset.sqrMagnitude <= 0) return;
        Vector4 colum = mtWorld.GetColumn(3);
        colum.x += offset.x;
        colum.y += offset.y;
        colum.z += offset.z;
        mtWorld.SetColumn(3, colum);
    }
    //-----------------------------------------------------
    static public void UpdateScale(ref Matrix4x4 mtWorld, Vector3 scale)
    {
        mtWorld.m00 = scale.x;
        mtWorld.m11 = scale.y;
        mtWorld.m22 = scale.z;
    }
    //-----------------------------------------------------
    static public Vector3 GetPosition(Matrix4x4 mtWorld)
    {
        return mtWorld.GetColumn(3);
    }
    //------------------------------------------------------
    static public Vector3 RoateAround(Vector3 anchor, Vector3 point, Quaternion rot)
    {
        return rot * (point - anchor) + anchor;
    }
    //------------------------------------------------------
    public static float ClampAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        if (angle < -180f)
        {
            angle += 360f;
        }

        return angle;
    }
    //------------------------------------------------------
    public static Vector3 ClampAngle(Vector3 angle)
    {
        if (angle.x > 180.0f) angle.x -= 360.0f;
        if (angle.y > 180.0f) angle.y -= 360.0f;
        if (angle.z > 180.0f) angle.z -= 360.0f;
        if (angle.x < -180.0f) angle.x += 360.0f;
        if (angle.y < -180.0f) angle.y += 360.0f;
        if (angle.z < -180.0f) angle.z += 360.0f;

        //             if (angle.x < -360) angle.x += 360;
        //             if (angle.x > 360) angle.x -= 360;
        //             if (angle.y < -360) angle.y += 360;
        //             if (angle.y > 360) angle.y -= 360;
        //             if (angle.z < -360) angle.z += 360;
        //             if (angle.z > 360) angle.z -= 360;
        return angle;
    }
    //-----------------------------------------------------
    public static bool PositionInView(Matrix4x4 clipMatrix, Vector3 worldPos, float factor = 1f)
    {
        worldPos = clipMatrix.MultiplyPoint(worldPos);

        if (Mathf.Abs(worldPos.x) < factor
         && Mathf.Abs(worldPos.y) < factor
         && worldPos.z <= factor)
        {
            return true;
        }
        return false;
    }
    //------------------------------------------------------
    public static Vector3 ProjectLinePos(ref float factor, Vector3 linePos, Vector3 lineDir, Vector3 point, bool bProjectFacor = true)
    {
        Vector3 pointVec = point - linePos;

        factor = Vector3.Dot(pointVec, lineDir);
        Vector3 projPos = linePos + lineDir * factor;
        if (bProjectFacor) factor = Vector3.Distance(projPos, point);
        return projPos;
    }
    //------------------------------------------------------
    public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        if (planeNormal.sqrMagnitude > 1) planeNormal = planeNormal.normalized;
        float distance = Vector3.Dot(planeNormal, (point - planePoint));
        return point - planeNormal * distance;
    }
    //------------------------------------------------------
    public static Vector3 ProjectPointOnPlane(ref float distance, Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        if (planeNormal.sqrMagnitude > 1) planeNormal = planeNormal.normalized;
        float temp = Vector3.Dot(planeNormal, (point - planePoint));
        distance = Mathf.Abs(temp);
        return point - planeNormal * temp;
    }
    //------------------------------------------------------
    public static float PointDistancePlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        if (planeNormal.sqrMagnitude > 1) planeNormal = planeNormal.normalized;
        return Vector3.Dot(planeNormal, (point - planePoint));
    }
    //-----------------------------------------------------
    public static void Desytroy(UnityEngine.Object pObj)
    {
        if (pObj == null) return;
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
            UnityEngine.Object.Destroy(pObj);
        else
            UnityEngine.Object.DestroyImmediate(pObj);
#else
            UnityEngine.Object.Destroy(pObj);
#endif
    }
    //------------------------------------------------------
    public static void DesytroyDelay(UnityEngine.Object pObj, float fDelay)
    {
        if (pObj == null) return;
        UnityEngine.Object.Destroy(pObj, fDelay);
    }
    //------------------------------------------------------
    public static void DestroyImmediate(UnityEngine.Object pObj)
    {
        if (pObj == null) return;
#if UNITY_EDITOR
        UnityEngine.Object.DestroyImmediate(pObj);
#else
            UnityEngine.Object.Destroy(pObj);
#endif
    }
    //------------------------------------------------------
    public static void DestroyChilds(UnityEngine.GameObject pObj)
    {
        if (pObj == null) return;
        UnityEngine.Transform pTransform = pObj.transform;
        int count = pTransform.childCount;
        for (int i = 0; i < count; ++i)
        {
            Desytroy(pTransform.GetChild(i).gameObject);
        }
        pTransform.DetachChildren();
    }
    //------------------------------------------------------
    public static void DestroyChildsDelay(UnityEngine.GameObject pObj, float fDelay)
    {
        if (pObj == null) return;
        UnityEngine.Transform pTransform = pObj.transform;
        int count = pTransform.childCount;
        for (int i = 0; i < count; ++i)
        {
            DesytroyDelay(pTransform.GetChild(i).gameObject, fDelay);
        }
        pTransform.DetachChildren();
    }
    //------------------------------------------------------
    public static void DestroyChildsImmediate(UnityEngine.GameObject pObj)
    {
        if (pObj == null) return;
#if UNITY_EDITOR
        UnityEngine.Transform pTransform = pObj.transform;
        int count = pTransform.childCount;
        while (pTransform.childCount > 0)
        {
            DestroyImmediate(pTransform.GetChild(0).gameObject);
        }
#else
            UnityEngine.Transform pTransform = pObj.transform;
            int count = pTransform.childCount;
            for(int i = 0; i < count; ++i)
            {
                Transform pTrans = pTransform.GetChild(i);
                pTrans.position = new Vector3(-100000,-100000,-100000);
                Desytroy(pTrans.gameObject);
            }
            pTransform.DetachChildren();
#endif
    }
    //------------------------------------------------------
    public static bool IsSubTransform(Transform pTran, Transform pParent)
    {
        if (pTran == null || pParent == null) return false;
        if (pParent == pTran) return true;
        Transform check = pTran;
        while (check)
        {
            if (check == pParent) return true;
            check = check.parent;
        }
        return false;
    }
    //------------------------------------------------------
    public static string GetTransformToPath(Transform current, Transform root = null, bool bIncludeRoot = true)
    {
        if (current == null) return string.Empty;
        System.Text.StringBuilder sb = stringBuilder;
        Transform cur = current;
        while (cur != null)
        {
            if (root && cur == root) break;
            sb.Insert(0, cur.name);
            sb.Insert(0, "/");
            cur = cur.parent;
        }
        if (root && bIncludeRoot)
            sb.Insert(0, root.name);
        else return sb.ToString(1, sb.Length - 1);
        return sb.ToString();
    }
    //------------------------------------------------------
    public static Transform FindTransform(Transform root, string strValue, bool bPath)
    {
        if (bPath) return FindTransformByPath(root, strValue);
        return FindTransform(root, strValue);
    }
    //------------------------------------------------------
    public static Transform FindTransformByPath(Transform root, string path)
    {
        if (root == null) return null;
        if (root.name.CompareTo(path) == 0) return root;

        Transform current = root;
        Transform find = null;
        string[] mul = path.Split('/');
        for (int i = 0; i < mul.Length; ++i)
        {
            current = FindTransform(current, mul[i]);
            if (current == null) return null;
            find = current;
        }
        return find;
    }
    //------------------------------------------------------
    public static void CollectSubTransform(Transform root, string name, List<Transform> vCollect, bool bAbsMatch = false)
    {
        if (root == null || string.IsNullOrEmpty(name)) return;

        for (int i = 0; i < root.childCount; ++i)
        {
            InnerCollectSubTransform(root.GetChild(i), name, vCollect, bAbsMatch);
        }
    }
    //------------------------------------------------------
    static void InnerCollectSubTransform(Transform node, string name, List<Transform> vCollect, bool bAbsMatch = false)
    {
        bool bMatch = false;
        if (bAbsMatch)
        {
            if (node.name.CompareTo(name) == 0)
            {
                vCollect.Add(node);
                bMatch = true;
            }
        }
        else
        {
            if (node.name.Contains(name))
            {
                vCollect.Add(node);
                bMatch = true;
            }
        }
        if (bMatch) return;
        for (int i = 0; i < node.childCount; ++i)
        {
            InnerCollectSubTransform(node.GetChild(i), name, vCollect, bAbsMatch);
        }
    }
    //------------------------------------------------------
    public static Transform FindTransform(Transform root, string name, int nDepth = 0)
    {
        if (root == null) return null;
        if (root.name.CompareTo(name) == 0) return root;

        if (nDepth > 0)
        {
            for (int j = 0; j < root.childCount; ++j)
            {
                Transform result = FindTransform(root.GetChild(j), name, nDepth - 1);
                if (result != null) return result;
            }
        }
        else
        {
            for (int j = 0; j < root.childCount; ++j)
            {
                if (root.GetChild(j).name.CompareTo(name) == 0)
                {
                    return root.GetChild(j);
                }
            }
        }

        return null;
    }
    //------------------------------------------------------
    public static Transform FindTransformByTag(Transform root, string tag, string name)
    {
#if !USE_SERVER
        if (root == null) return null;
        UnityEngine.Object[] tags = GameObject.FindGameObjectsWithTag(tag);
        if (tags == null) return null;
        for (int i = 0; i < tags.Length; ++i)
        {
            GameObject pObj = tags[i] as GameObject;
            if (pObj != null)
            {
                if (pObj.transform.name.CompareTo(name) == 0)
                {
                    Transform cur = pObj.transform;
                    while (cur)
                    {
                        if (cur == root) return pObj.transform;
                        cur = cur.parent;
                    }
                    return null;
                }
            }
        }
#endif
        return null;
    }
    //------------------------------------------------------
    public static string FormBytes(long b)
    {
        if (b < 0)
            return "Unknown";
        if (b < 512)
#if UNITY_64 && UNITY_LINUX
		return string.Format("{0:f2} B",b);
#else
            return string.Format("{0:f2} B", b);
#endif
        if (b < 512 * 1024)
            return string.Format("{0:f2} KB", b / 1024.0);

        b /= 1024;
        if (b < 512 * 1024)
            return string.Format("{0:f2} MB", b / 1024.0);

        b /= 1024;
        return string.Format("{0:f2} GB", b / 1024.0);
    }
    //------------------------------------------------------
    public static bool IsValidCurve(AnimationCurve curve)
    {
        if (curve == null || curve.length <= 0) return false;
        return true;
    }
    //------------------------------------------------------
    public static float EvaluateCurve(AnimationCurve curve, float time, float fDefault = 0, bool bReverse = false)
    {
        if (curve == null || curve.length <= 0) return fDefault;
        if (bReverse) return curve.Evaluate(curve.keys[curve.length - 1].time - time);
        return curve.Evaluate(time);
    }
    //-----------------------------------------------------
    public static float GetCurveFirstTime(AnimationCurve curve)
    {
        if (curve == null || curve.length <= 0) return 0;
        return curve.keys[0].time;
    }
    //-----------------------------------------------------
    public static float GetCurveMaxTime(AnimationCurve curve, bool bCheckLoop = false)
    {
        if (curve == null || curve.length <= 0) return 0;
        if (bCheckLoop && (curve.postWrapMode == WrapMode.Loop || curve.postWrapMode == WrapMode.PingPong)) return POINGPONG_MAX_TIME;
        return curve.keys[curve.length - 1].time;
    }
    //-----------------------------------------------------
    public static float GetCurveMaxValue(AnimationCurve curve)
    {
        if (curve == null || curve.length <= 0) return 0;
        float fMaxValue = float.MinValue;
        for (int i = 0; i < curve.length; ++i)
            fMaxValue = Mathf.Max(fMaxValue, curve[i].value);
        return fMaxValue;
    }
    //-----------------------------------------------------
    public static float GetCurveMinValue(AnimationCurve curve)
    {
        if (curve == null || curve.length <= 0) return 0;
        float fMinValue = float.MaxValue;
        for (int i = 0; i < curve.length; ++i)
            fMinValue = Mathf.Min(fMinValue, curve[i].value);
        return fMinValue;
    }
    //-----------------------------------------------------
    public static void AddCurveKey(AnimationCurve curve, float fTime, float var, float gapTime = 0.01f)
    {
        if (curve == null) curve = new AnimationCurve();
#if UNITY_EDITOR
        int len = curve.length;
        for (int i = 0; i < curve.length; ++i)
        {
            if (Mathf.Abs(curve[i].time - fTime) <= gapTime)
            {
                curve.RemoveKey(i);
                i = 0;
            }
        }
#endif
        curve.AddKey(fTime, var);
    }
    //-----------------------------------------------------
    public static void AddCurveKey(AnimationCurve curve, Keyframe key, float gapTime = 0.01f)
    {
        if (curve == null) curve = new AnimationCurve();
#if UNITY_EDITOR
        int len = curve.length;
        for (int i = 0; i < curve.length; ++i)
        {
            if (Mathf.Abs(curve[i].time - key.time) <= gapTime)
            {
                curve.RemoveKey(i);
                i = 0;
            }
        }
#endif
        curve.AddKey(key);
    }
    //-----------------------------------------------------
    public static void SetActive(UnityEngine.Transform pObj, bool bActive)
    {
        if (pObj == null) return;
        pObj.gameObject.SetActive(bActive);
    }
    //-----------------------------------------------------
    public static void SetActive(UnityEngine.GameObject pObj, bool bActive)
    {
        if (pObj == null) return;
        pObj.SetActive(bActive);
    }
    //-----------------------------------------------------
    public static void ResetGameObject(GameObject gameObject, EResetType type = EResetType.Local)
    {
        if (gameObject == null) return;
        ResetGameObject(gameObject.transform, type);
    }
    //-----------------------------------------------------
    public static void ResetGameObject(Transform pTrans, EResetType type = EResetType.Local)
    {
        if (pTrans == null) return;
        if (type == EResetType.World || type == EResetType.All)
        {
            pTrans.position = Vector3.zero;
            pTrans.rotation = Quaternion.identity;
            pTrans.eulerAngles = Vector3.zero;
        }

        if (type == EResetType.Local || type == EResetType.All)
        {
            pTrans.localPosition = Vector3.zero;
            pTrans.localRotation = Quaternion.identity;
            pTrans.localEulerAngles = Vector3.zero;
        }
    }
    //------------------------------------------------------
    public static float GetParticleStartDelayTime(this ParticleSystem particle)
    {
        var startDelayCurve = particle.main.startDelay;
        float maxStartDelay = 0f;
        switch (startDelayCurve.mode)
        {
            case ParticleSystemCurveMode.Constant:
                maxStartDelay = startDelayCurve.constantMax;
                break;

            case ParticleSystemCurveMode.TwoConstants:
                maxStartDelay = startDelayCurve.constantMax;
                break;

            case ParticleSystemCurveMode.Curve:
                maxStartDelay = startDelayCurve.curveMax.Evaluate(0f);
                break;

            case ParticleSystemCurveMode.TwoCurves:
                maxStartDelay = Mathf.Max(
                    startDelayCurve.curveMin.Evaluate(0f),
                    startDelayCurve.curveMax.Evaluate(0f)
                );
                break;
        }
        return maxStartDelay * particle.main.startDelayMultiplier;
    }
    //-----------------------------------------------------
    public static bool IsInView(Vector3 pos, float factor = 0.1f, Camera mainCam = null)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return true;
#endif
        if(mainCam == null)
            mainCam = Camera.main;
        if (mainCam == null) return false;
        Vector2 viewPos = mainCam.WorldToViewportPoint(pos);
        Vector3 dir = (pos - mainCam.transform.position).normalized;
        float dot = Vector3.Dot(mainCam.transform.forward, dir);
        if (dot > 0 && viewPos.x >= -factor && viewPos.x <= 1 + factor && viewPos.y >= -factor && viewPos.y <= 1 + factor)
            return true;
        return false;
    }
    //------------------------------------------------------
    public static string SetNum(long left, long right, bool isShowRed = true)
    {
        string leftShort = GetNumString(left, 10000, 10000000, false);
        string rightShort = GetNumString(right, 10000, 10000000);
        if (isShowRed && left < right)
        {
            leftShort = BaseUtil.stringBuilder.Append("<color=#FF8C8C>").Append(leftShort).Append("</color>").ToString();
        }

        return BaseUtil.stringBuilder.Append(leftShort).Append("/").Append(rightShort).ToString();
    }
    //-----------------------------------------------------
    public static string GetShortNum(long amount)
    {
        if (amount > 1000000000) return BaseUtil.stringBuilder.Append(Mathf.FloorToInt(amount / 1000000000)).Append("B").ToString();
        if (amount > 1000000) return BaseUtil.stringBuilder.Append(Mathf.FloorToInt(amount / 1000000)).Append("M").ToString();
        if (amount > 1000) return BaseUtil.stringBuilder.Append(Mathf.FloorToInt(amount / 1000)).Append("K").ToString();
        return amount.ToString();
    }
    //-----------------------------------------------------
    public static long StringToHashID64(string str)
    {
        if (string.IsNullOrEmpty(str)) return 0;
        try
        {
            return BitConverter.ToInt64(System.Text.Encoding.UTF8.GetBytes(str), 0);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
        return 0;
    }
    //------------------------------------------------------
    public static string GetNumString(long money, long k, long m, bool isFloor = true)
    {
        if (money >= m)
        {
            if (isFloor)
            {
                return BaseUtil.stringBuilder.Append(Mathf.FloorToInt(money / 1000000)).Append("M").ToString();
            }
            return BaseUtil.stringBuilder.Append(Mathf.CeilToInt(money / 1000000)).Append("M").ToString();
        }
        else if (money >= k)
        {
            if (isFloor)
            {
                return BaseUtil.stringBuilder.Append(Mathf.FloorToInt(money / 1000)).Append("K").ToString();
            }
            return BaseUtil.stringBuilder.Append(Mathf.CeilToInt(money / 1000)).Append("K").ToString();
        }
        return money.ToString();
    }
#if USE_FIXEDMATH
    //------------------------------------------------------
    static public ExternEngine.FVector3 FRoateAround(ExternEngine.FVector3 anchor, ExternEngine.FVector3 point, ExternEngine.FQuaternion rot)
    {
        return rot * (point - anchor) + anchor;
    }
    //------------------------------------------------------
    public static ExternEngine.FFloat GetFixedFrame(AFramework frameWork, bool bTimeScale = false, int targetFrame = 0)
    {
        if (frameWork != null && (frameWork.IsLogicLock() || frameWork.IsPause())) return ExternEngine.FFloat.zero;
        if (bTimeScale && frameWork != null)
        {
            if (targetFrame == 0) targetFrame = frameWork.TargetFrameRate;
            if (targetFrame == 45) return ExternEngine.FFloat.FRAMEFPS45 * frameWork.TimeScale;
            else if (targetFrame == 60) return ExternEngine.FFloat.FRAMEFPS60 * frameWork.TimeScale;
            else return ExternEngine.FFloat.FRAMEFPS30 * frameWork.TimeScale;
        }
        return ExternEngine.FFloat.FRAMEFPS30;// * Time.timeScale;
    }
    //------------------------------------------------------
    public static ExternEngine.FFloat GetRealFixedFrame(AFramework frameWork, bool bTimeScale = false)
    {
        if (frameWork != null && (frameWork.IsLogicLock() || frameWork.IsPause())) return ExternEngine.FFloat.zero;
        if (frameWork != null)
        {
            if (bTimeScale) return frameWork.InvTargetFrameRate * frameWork.TimeScale;
            return frameWork.InvTargetFrameRate;
        }
        return ExternEngine.FFloat.FRAMEFPS30;// * Time.timeScale;
    }
    //------------------------------------------------------
    public static ExternEngine.FFloat GetRealFrame(AFramework frameWork)
    {
        if (frameWork == null) return ExternEngine.FFloat.FRAMEFPS30;
        if (frameWork != null && (frameWork.IsLogicLock() || frameWork.IsPause())) return ExternEngine.FFloat.zero;
        return frameWork.GetDeltaTime();
    }
    //-----------------------------------------------------------------------------
    public static void CU_GetQuaternionFromDirection(ExternEngine.FVector3 vDirection, ExternEngine.FVector3 vUp, ref ExternEngine.FQuaternion qRot)
    {
        if (vDirection.sqrMagnitude <= 0.001f) vDirection = ExternEngine.FVector3.forward;
        if (vUp.sqrMagnitude <= 0.001f) vUp = ExternEngine.FVector3.up;
        qRot = ExternEngine.FQuaternion.LookRotation(vDirection, vUp);
    }
#else
    //------------------------------------------------------
    public static float GetFixedFrame(AFramework frameWork, bool bTimeScale = false, int targetFrame = 0)
    {
        if (frameWork != null && (frameWork.IsLogicLock() || frameWork.IsPause())) return 0.0f;
#if !USE_SERVER
        if (bTimeScale) return Time.deltaTime;
        else return Time.unscaledDeltaTime;
#endif
        if (bTimeScale && frameWork != null)
        {
            if (targetFrame == 0) targetFrame = frameWork.TargetFrameRate;
            if (targetFrame == 45) return (0.022222f) * frameWork.TimeScale;
            else if (targetFrame == 60) return (0.016666f) * frameWork.TimeScale;
            else return (0.033333f) * frameWork.TimeScale;
        }
        return 0.033333f;// * Time.timeScale;
    }
    //------------------------------------------------------
    public static float GetRealFixedFrame(AFramework frameWork, bool bTimeScale = false)
    {
        if (frameWork != null && (frameWork.IsLogicLock() || frameWork.IsPause())) return 0.0f;
#if !USE_SERVER
        if (bTimeScale) return Time.deltaTime;
        else return Time.unscaledDeltaTime;
#endif
        if (frameWork != null)
        {
            if (bTimeScale) return frameWork.InvTargetFrameRate * frameWork.TimeScale;
            return frameWork.InvTargetFrameRate;
        }
        return 0.033333f;// * Time.timeScale;
    }
    //------------------------------------------------------
    public static float GetRealFrame(AFramework frameWork)
    {
        if (frameWork == null) return 0.033333f;
        if (frameWork != null && (frameWork.IsLogicLock() || frameWork.IsPause())) return 0.0f;
        return frameWork.GetDeltaTime();
    }
#endif
    //------------------------------------------------------
    public static int Get2Pow(int into)
    {
        int outo = 1;
        for (int i = 0; i < 10; i++)
        {
            outo *= 2;
            if (outo > into)
            {
                break;
            }
        }

        return outo;
    }

}