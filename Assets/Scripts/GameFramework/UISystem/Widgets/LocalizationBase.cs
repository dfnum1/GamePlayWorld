/********************************************************************
生成日期:	2022-03-24
类    名: 	LocalizationBase
作    者:	HappLI
描    述:	多语言组件基类
*********************************************************************/
using UnityEngine;

public abstract class LocalizationBase : MonoBehaviour
{
    public uint ID = 0;

    public abstract void OnLanguageChangeCallback(SystemLanguage languageType);

    public abstract void RefreshShow();

    public virtual void Start()
    {
       // ALocalizationManager.OnLanguageChangeEvent += OnLanguageChangeCallback;
    }
    //------------------------------------------------------
    public virtual void OnEnable()
    {
        RefreshShow();
    }
    //------------------------------------------------------
    public virtual void OnDestroy()
    {
      //  ALocalizationManager.OnLanguageChangeEvent -= OnLanguageChangeCallback;
    }
}
