/********************************************************************
生成日期:	1:11:2020 13:07
类    名: 	SceneMgr
作    者:	HappLI
描    述:	
*********************************************************************/
namespace Framework.Core
{
    public class SceneMgr : ASceneMgr
    {
        private ASceneTheme m_pSceneTheme = null;

        //------------------------------------------------------
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_pSceneTheme != null) m_pSceneTheme.Clear();
        }
        //------------------------------------------------------
        public void SetThemer(ASceneTheme sceneTheme)
        {
            m_pSceneTheme = sceneTheme;
        }
        //------------------------------------------------------
        public static ASceneTheme GetThemer()
        {
            if (AFramework.mainFramework == null || AFramework.mainFramework.sceneManager == null) return null;
            return AFramework.mainFramework.sceneManager.m_pSceneTheme;
        }
        //------------------------------------------------------
        public bool LoadScene(ushort sceneId, float fPopDelay = 0, ESceneSignType load = ESceneSignType.PopAll, ESceneSignType unload = ESceneSignType.PopAll)
        {
            if (AFramework.mainFramework == null)
                return false;
            SceneParam sceParam = AFramework.mainFramework.GetSceneByID(sceneId);

            if(!sceParam.IsValid())
            {
                DoCallback(sceParam);
                return false;
            }

            sceParam.sceneID = sceneId;
            sceParam.isCompled = false;
            sceParam.sceneFile = null;
            sceParam.load = load;
            sceParam.unload = unload;
            return LoadScene(sceParam, fPopDelay);
        }
        //------------------------------------------------------
        protected override void InnerUpdate(float fFrame)
        {
            if (m_pSceneTheme != null)
                m_pSceneTheme.Update(fFrame);
        }
    }
}

