/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	SpeedLifeParticleController
作    者:	HappLI
描    述:	速度和生命周期的特效控制
*********************************************************************/
using UnityEngine;
namespace Framework.Core
{
    public class SpeedLifeParticleController : ParticleController
    {
        public ParticleSystem[] speedLifePars;
        public void SetLaunchSpeed(float fSpeed)
        {
            if(speedLifePars!=null && speedLifePars.Length>0)
            {
                for(int i = 0; i < speedLifePars.Length; ++i)
                {
                    ParticleSystem.MainModule main = speedLifePars[i].main;
                    main.startSpeed = fSpeed;
                }
            }
            else
            {
                if (m_arrSystems == null) return;
                for (int i = 0; i < m_arrSystems.Length; ++i)
                {
                    ParticleSystem.MainModule main = m_arrSystems[i].system.main;
                    main.startSpeed = fSpeed;
                }
            }
        }
        //------------------------------------------------------
        public void SetLaunchDuration(float fDuration)
        {
            if (speedLifePars != null && speedLifePars.Length > 0)
            {
                for (int i = 0; i < speedLifePars.Length; ++i)
                {
                    ParticleSystem.MainModule main = speedLifePars[i].main;
                    main.startLifetime = fDuration;
                }
            }
            else
            {
                if (m_arrSystems == null) return;
                for (int i = 0; i < m_arrSystems.Length; ++i)
                {
                    ParticleSystem.MainModule main = m_arrSystems[i].system.main;
                    main.startLifetime = fDuration;
                }
            }
        }
    }
}
