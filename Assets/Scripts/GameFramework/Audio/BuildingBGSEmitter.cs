using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_FMOD
using FMODUnity;
using FMOD.Studio;
#endif
using Framework.Base;
using Framework.Core;

public class BuildingBGSEmitter : MonoBehaviour
{
#if USE_FMOD
    public StudioEventEmitter m_pEmitter;
    public float m_cameraDistance;
    private bool m_played;
    // Start is called before the first frame update
    void Start()
    {
        if (m_pEmitter == null)
            m_pEmitter = GetComponent<StudioEventEmitter>();
        m_pEmitter.AllowFadeout = true;
        m_played = false;
    }

    // Update is called once per frame
    void Update()
    {
        Transform cameraTranform = CameraKit.GetTransform();
        if (cameraTranform == null) return;
        m_cameraDistance = Vector3.Distance(m_pEmitter.transform.position, cameraTranform.position);
        
        // 主城功能建筑 逻辑 ///////////////////////
        if (gameObject.layer == (int)EMaskLayer.EffectLayer && m_cameraDistance < m_pEmitter.OverrideMaxDistance )
        {
            if(!m_pEmitter.IsPlaying())
            {
                if (!m_played)
                {
                    m_pEmitter.Play();
                    m_played = true;
                    SetBGMVolume(-0.5f);
                }
            }            
        }
        else //if (gameObject.layer != (int)EMaskLayer.EffectLayer && m_pEmitter.IsPlaying())
        {
            m_pEmitter.Stop();
            m_played = false;
            SetBGMVolume(0f);
        }
        
        ////////////////////////////////////////////
    }

    void SetBGMVolume(float volume)
    {
        FMOD.GUID guid = new FMOD.GUID();
        //FMODUnity.RuntimeManager.GetBus()
        //Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/Master/BGM");

        //if (bus.isValid())
        //{
        //    bus.setVolume(volume);
        //}

        //VCA vca = FMODUnity.RuntimeManager.GetVCA("vca:/VCABGM");

        //if(vca.isValid())
        //{
        //    vca.setVolume(volume);
        //}
    }
#endif
}
