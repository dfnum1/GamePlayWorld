/********************************************************************
生成日期:	2020-06-12
类    名: 	LocalizationAbsImage
作    者:	zdq
描    述:   多语言图片类型抽象类
*********************************************************************/
using UnityEngine;

namespace Framework.Core
{
    public abstract class LocalizationAbsImage : LocalizationBase
    {
        protected DynamicLoader m_pLoader = new DynamicLoader();


        public override void OnDestroy()
        {
            base.OnDestroy();
            m_pLoader?.ClearLoaded();
        }
        public abstract bool IsEmpty();

        public virtual void OnDisable()
        {
            m_pLoader?.ClearLoaded();
        }
    }
}