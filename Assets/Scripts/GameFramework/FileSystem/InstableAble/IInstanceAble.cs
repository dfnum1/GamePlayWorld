// /********************************************************************
// 生成日期:	1:11:2020 13:16
// 类    名: 	AInstanceAble
// 作    者:	HappLI
// 描    述:	
// *********************************************************************/
// using System;
// using System.Collections.Generic;
// using UnityEngine;
// 
// namespace Framework.Core
// {
//     public interface InstanceOperiaon
//     {
//         void Earse();
//         void Clear();
//         void Refresh();
// 
//         string GetFile();
//         AInstanceAble GetAble();
// 
//         bool IsUsed();
//         void SetUsed(bool bUsed);
// 
//         bool IsAsync();
//         void SetAsync(bool bAsync);
// 
//         bool IsScene();
//         void SetScene(bool bScene);
// 
//         bool IsPreload();
//         void SetIsPreload(bool isPreload);
// 
//         void AddSignCallback(Action<InstanceOperiaon> Call);
//         void AddCallback(Action<InstanceOperiaon> Call);
// 
//         void SetUserData(int index, VariablePoolAble userData);
//         T GetUserData<T>(int index) where T : VariablePoolAble;
//         bool HasData(int index);
//         bool HasData<T>(int index) where T : VariablePoolAble;
// 
//         void SetLimitCheckCnt(int cnt);
//         void SetByParent(Transform pParent);
//         Transform GetByParent();
//     }
//     //------------------------------------------------------
//     public enum EInstanceCallbackType
//     {
//         Enable,
//         Disiable,
//         Recyled,
//         Destroy,
//     }
//     //------------------------------------------------------
//     public interface IInstanceAbleCallback
//     {
//         void OnInstanceCallback(AInstanceAble pAble, EInstanceCallbackType eType);
//     }
//     //------------------------------------------------------
//     [Plugin.AT.ATConvertBehaviour]
//     public interface AInstanceAble : Plugin.AT.IUserData
//     {
//         void RegisterCallback(IInstanceAbleCallback callback);
//         void UnRegisterCallback(IInstanceAbleCallback callback);
//         bool CanRecyle();
//         void RecyleDestroy(int recyleMax = 2);
//         void ResetDelayDestroyParam();
//         void DelayDestroy(float fDelay, int recyleMax = 2);
//         bool IsRecyled();
//         void SetPosition(Vector3 postion, bool bLocal = false);
//         void SetRotation(Quaternion rot, bool bLocal = false);
//         void SetForward(Vector3 forward);
//         void SetUp(Vector3 up);
//         void SetEulerAngle(Vector3 eulerAngles, bool bLocal = false);
//         void SetScale(Vector3 scale);
//       //  void ResetTranform(bool bScale = false);
//         Transform GetTransorm();
//         GameObject GetObject();
//         void SetActive();
//         void SetUnActive();
//         bool IsActive();
//         int GetDefaultLayer();
//         int GetLayer();
//         void ResetLayer();
//         void SetRenderLayer(int layer);
//         void OnFreezed(bool bFreezed);
// 
//         T GetBehaviour<T>() where T : Behaviour;
//         T AddBehaviour<T>(System.Type type) where T : Behaviour;
// 
//         void EnableKeyWorld(string keyWorld, bool bEnable, int materialIndex = -1);
//         void LerpToMaterial(Material material, float fLerpTime, int index = -1, float fKeepTime = 0, string propertyName = null, bool bAutoDestroy = true);
//         void SetMaterial(Material material, int index = 1, bool bAutoDestroy = true);
//         void FadeoutMaterial();
//         void RemoveMaterial(Material pMaterial);
//         void RemoveMaterialByIndex(int index);
//         void ReplaceShader(string name, int materialIndex = -1);
// 
//         void SetFloat(string propName, float fValue, bool bBlock = true, bool bShare = true, int index = -1);
//         float GetFloat(string propName, int index = 0);
//         void SetInt(string propName, int nValue, bool bBlock = true, bool bShare = true, int index = -1);
//         int GetInt(string propName, int index = 0);
// 
//         void SetColor(string propName, Color color, bool bBlock = true, bool bShare = true, int index = -1);
//         Color GetColor(string propName, int index = 0);
//         void SetVector(string propName, Vector4 vec4, bool bBlock = true, bool bShare = true, int index = -1);
//         Vector4 GetVector(string propName, int index = 0);
// 
//     }
// }