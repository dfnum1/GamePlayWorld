#if USE_URP
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	APostRenderPass
作    者:	HappLI
描    述:	URP
*********************************************************************/
namespace Framework.URP
{
    public partial class URPPostWorker
    {
        //------------------------------------------------------
        void RegisterLogics(APostRenderPass pass)
        {
            RenderPost renderPost = m_vPosts[(int)pass.GetPostPassType()];
            if (renderPost == null) return;
            switch (pass.GetPostPassType())
            {
                case Framework.URP.EPostPassType.Screen:
                    {
                        ScreenFadeLogic logic = new ScreenFadeLogic();
                        logic.SetCode(-1);
                        logic.SetPass(pass);
                        logic.Awake();
                        renderPost.posts.Add(logic);
                    }
                    break;
                case EPostPassType.ForceCustomOpaque:
                    {
                        Plugin.BakerSkinningDrawer bakeSkinDraw = new Plugin.BakerSkinningDrawer();
                        bakeSkinDraw.SetCode(-2);
                        bakeSkinDraw.SetPass(pass);
                        bakeSkinDraw.Awake();
                        renderPost.posts.Add(bakeSkinDraw);
                    }
                    break;
            }
        }
    }
}
#endif