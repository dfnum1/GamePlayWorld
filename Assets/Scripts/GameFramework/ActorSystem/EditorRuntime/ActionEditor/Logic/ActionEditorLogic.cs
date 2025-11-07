#if UNITY_EDITOR && USE_ACTORSYSTEM
/********************************************************************
生成日期:	11:06:2023
类    名: 	ActionEditorLogic
作    者:	HappLI
描    述:	
*********************************************************************/
using UnityEngine;
using UnityEditor;
using Framework.ED;
using Framework.Core;

namespace ActorSystem.ED
{
    public abstract class ActionEditorLogic : AEditorLogic
    {
        AssetDrawLogic m_pAssetDraw = null;
        //--------------------------------------------------------
        public Framework.Core.Actor GetActor()
        {
            if (m_pAssetDraw == null) m_pAssetDraw = GetLogic<AssetDrawLogic>();
            if (m_pAssetDraw == null)
                return null;
            return m_pAssetDraw.GetActor();
        }
        //--------------------------------------------------------
        public virtual void OnSelectActor(Framework.Core.Actor pActor) { }
    }
}
#endif
