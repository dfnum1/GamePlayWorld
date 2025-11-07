/********************************************************************
生成日期:	3:10:2019  15:03
类    名: 	IGame
作    者:	HappLI
描    述:	游戏对象基础主接口
*********************************************************************/
using System;
using System.Collections;
using Framework.Core;
using UnityEngine;

public interface IGame
{
    Coroutine BeginCoroutine(IEnumerator coroutine);
    void EndAllCoroutine();
    void EndCoroutine(Coroutine cortuine);
    void EndCoroutine(IEnumerator cortuine);
    EFileSystemType GetFileStreamType();
    int GetMaxThread();
    bool IsEditor();

    UISystem GetUISystem();
    AudioManager GetAudioMgr();
    ScriptableObject[] GetDatas();
    Transform GetTransform();
    CameraSetting GetCameraSetting();

    UnityEngine.Rendering.RenderPipelineAsset GetURPAsset();
}

public class AGame : MonoBehaviour, IGame
{
    public EFileSystemType eFileStreamType = EFileSystemType.AssetData;
    public int mutiThread = 4;
    public UnityEngine.Rendering.RenderPipelineAsset urpAsset;
    public CameraSetting mainCammera;
    public UISystem uiSystem;
    public AudioManager audoManager;
    public ScriptableObject[] gameDatas;
    //------------------------------------------------------
    public CameraSetting GetCameraSetting()
    {
        return mainCammera;
    }
    //------------------------------------------------------
    public Transform GetTransform()
    {
        return this.transform;
    }
    //------------------------------------------------------
    public UnityEngine.Rendering.RenderPipelineAsset GetURPAsset()
    {
        return urpAsset;
    }
    //------------------------------------------------------
    public Coroutine BeginCoroutine(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }
    //------------------------------------------------------
    public void EndAllCoroutine()
    {
        StopAllCoroutines();
    }
    //------------------------------------------------------
    public void EndCoroutine(Coroutine cortuine)
    {
        StopCoroutine(cortuine);
    }
    //------------------------------------------------------
    public void EndCoroutine(IEnumerator cortuine)
    {
        StopCoroutine(cortuine);
    }
    //------------------------------------------------------
    public UISystem GetUISystem()
    {
        return uiSystem;
    }
    //------------------------------------------------------
    public AudioManager GetAudioMgr()
    {
        return audoManager;
    }
    //------------------------------------------------------
    public ScriptableObject[] GetDatas()
    {
        return gameDatas;
    }
    //------------------------------------------------------
    public EFileSystemType GetFileStreamType()
    {
        return eFileStreamType;
    }
    //------------------------------------------------------
    public int GetMaxThread()
    {
        return mutiThread;
    }
    //------------------------------------------------------
    public bool IsEditor()
    {
        return false;
    }
}
