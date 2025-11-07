/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	
作    者:	HappLI
描    述:	回调接口
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public interface IInstanceSpawner
    {
        void Spawn(string strFile, bool bAbs, Vector3 offset, Vector3 euler, Transform pParent = null);
    }
    public interface IInstanceCallback
    {
        void OnSpawnCallback(InstanceOperiaon pCallback);
    }
    public interface IInstanceSign
    {
        void OnSpawnSign(InstanceOperiaon pCallback);
    }
    //------------------------------------------------------
    public interface IAssetCallback
    {
        void OnAssetCallback(AssetOperiaon pCallback);
    }
}

