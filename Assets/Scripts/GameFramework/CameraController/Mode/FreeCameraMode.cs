/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	FreeCameraMode
作    者:	HappLI
描    述:	自由模式相机
*********************************************************************/
namespace Framework.Core
{
    public class FreeCameraMode : CameraMode
    {
        //------------------------------------------------------
        public FreeCameraMode()
        {
            m_pController = null;
            Reset();
        }
        //------------------------------------------------------
        public override void Reset()
        {
            base.Reset();
        }
        //------------------------------------------------------
        public override void End()
        {
            base.End();
        }
        //------------------------------------------------------
        public override void Update(float fFrameTime)
        {
            base.Update(fFrameTime);

        }
    }
}