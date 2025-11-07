/********************************************************************
生成日期:	19:11:2021   14:4
类    名: 	UIModelHudRTLogic
作    者:	happli
描    述:	3d模型显示
*********************************************************************/
using System.Collections.Generic;
using Framework.Base;
using Framework.Core;
using UnityEngine;

namespace Framework.UI
{
    public class UIModelHudRTLogic : UILogic
    {
        struct ModelHudData
        {
            public string m_strModelFile;
            public HudCamera m_pRTCamera;

            public ActorGraphicAgent graphicAgent;

            public string m_PlayAciton;
            public Vector3 m_vModelScale;
            public Vector3 m_vModelPos;
            public Vector3 m_vModelEulerAngle;
            public InstanceOperiaon m_pLoadOperiaon;
            public AInstanceAble m_pModel;

            public string m_strRtImage;
            public HUDImage m_SyncRT;

            public void Clear()
            {
                if (m_pRTCamera) Core.RenderHudManager.getInstance().Destroy(m_pRTCamera);
                m_pRTCamera = null;
                if (m_pModel != null) m_pModel.RecyleDestroy(1);
                m_pModel = null;
                if (graphicAgent != null) graphicAgent.Destroy();
                graphicAgent = null;
                if (m_pLoadOperiaon != null) m_pLoadOperiaon.Earse();
                m_pLoadOperiaon = null;
                m_strRtImage = null;
                m_PlayAciton = null;
                m_vModelScale = Vector3.one;
                m_vModelPos = Vector3.zero;
                m_vModelEulerAngle = Vector3.zero;
                m_strModelFile = null;
                m_SyncRT = null;
            }
        };

        List<ModelHudData> m_vDatas = new List<ModelHudData>();

        UIBase m_pBase;
        public override void Awake(UIBase pBase)
        {
            base.Awake(pBase);
            m_pBase = pBase;
        }
        //-------------------------------------------
        public override void OnDestroy()
        {
            base.OnDestroy();
            m_pBase = null;
        }
        //-------------------------------------------
        public override void OnHide()
        {
            base.OnHide();
            Clear();
        }
        //-------------------------------------------
        public override void OnShow()
        {
            base.OnShow();
            //! if show before set hud param,check set agin
            // if(m_pBase !=null && !string.IsNullOrEmpty(m_strRtImage))
            // {
            //     if(m_pRTCamera ==null)
            //         SetHudParam(m_strModelFile, m_strRtImage);
            //     m_strRtImage = null;
            // }
        }
        //-------------------------------------------
        void Clear()
        {
            for (int i = 0; i < m_vDatas.Count; i++)
            {
                m_vDatas[i].Clear();
            }
            m_vDatas.Clear();
        }
        //-------------------------------------------
        public void PayAction(string rtImage, string action)
        {
            for (int i = 0; i < m_vDatas.Count; i++)
            {
                if (m_vDatas[i].m_strRtImage.CompareTo(rtImage) == 0)
                {
                    if (m_vDatas[i].graphicAgent != null)
                        m_vDatas[i].graphicAgent.PlayAnimation(action);
                    break;
                }
            }
        }
        //-------------------------------------------
        public void PayAction(string rtImage,EActionStateType eType, uint nTag = 0)
        {
            for (int i = 0; i < m_vDatas.Count; i++)
            {
                if (m_vDatas[i].m_strRtImage.CompareTo(rtImage) == 0)
                {
                    if (m_vDatas[i].graphicAgent != null)
                        m_vDatas[i].graphicAgent.PlayAnimation(eType, nTag);
                    break;
                }
            }
        }
        //-------------------------------------------
        public bool SetHudParam(string strModel, string rtImage, bool bAsync= false)
        {
            int index = -1;
            for (int i = 0; i < m_vDatas.Count; i++)
            {
                if (m_vDatas[i].m_strRtImage.CompareTo(rtImage) == 0)
                {
                    index = i;
                    break;
                }
            }
            ModelHudData data;
            if (index < 0)
            {
                data = new ModelHudData();
                m_vDatas.Add(data);
                index = m_vDatas.Count - 1;
            }
            else
            {
                data = m_vDatas[index];
            }

            if (m_pBase == null)
            {
                m_vDatas.RemoveAt(index);
                return false;
            }
            if (strModel == null)
            {
                data.Clear();
                m_vDatas.RemoveAt(index);
                return false;
            }
            if (strModel.CompareTo(data.m_strModelFile) == 0)
                return true;

            data.Clear();
            HUDImage hudRT = m_pBase.GetWidget<HUDImage>(rtImage);
            if (hudRT == null)
            {
                m_vDatas.RemoveAt(index);
                return false;
            }
#if UNITY_EDITOR
            data.m_SyncRT = hudRT;
#endif
            data.m_strRtImage = rtImage;
            data.m_strModelFile = strModel;
            data.m_vModelScale = hudRT.ModelScale;
            data.m_vModelPos = hudRT.ModelPos + Vector3.right * 100 * index;
            data.m_vModelEulerAngle = hudRT.ModelRotate;
            data.m_PlayAciton = hudRT.playAction;
            data.graphicAgent = null;

            data.m_pRTCamera = Core.RenderHudManager.getInstance().CreateRenderHud(m_pBase, hudRT, hudRT.CameraPos + Vector3.right * 100 * index, (int)hudRT.rectTransform.sizeDelta.x, (int)hudRT.rectTransform.sizeDelta.y);
            if (data.m_pRTCamera == null) return false;
            data.m_pRTCamera.SetEulerAngle(hudRT.CameraRotate);
            data.m_pRTCamera.SetFov(hudRT.CameraFOV);
            data.m_pLoadOperiaon =FileSystemUtil.SpawnInstance(strModel, bAsync);
            m_vDatas[index] = data;
            if(data.m_pLoadOperiaon != null)
            {
                data.m_pLoadOperiaon.OnCallback = OnLoadModel;
                data.m_pLoadOperiaon.OnSign = OnLoadSign;
                data.m_pLoadOperiaon.pByParent = data.m_pRTCamera.transform;
          //      data.m_pLoadOperiaon.Refresh();
            }
            return true;
        }
        //-------------------------------------------
        void OnLoadModel(InstanceOperiaon model)
        {
            for (int i = 0; i < m_vDatas.Count; i++)
            {
                if (m_vDatas[i].m_pLoadOperiaon == model)
                {
                    var data = m_vDatas[i];
                    data.m_pLoadOperiaon = null;
                    data.m_pModel = model.pPoolAble;
                    if(data.m_pModel!=null)
                    {
                        data.m_pModel.SetRenderLayer(LayerUtil.RenderUIModelLayer);
                        data.m_pModel.SetPosition(data.m_vModelPos);
                        data.m_pModel.SetScale(data.m_vModelScale);
                        data.m_pModel.SetEulerAngle(data.m_vModelEulerAngle);

                        var actorComp = data.m_pModel.GetBehaviour<ActorComponent>();
                        if (actorComp != null)
                        {
                            data.graphicAgent = new ActorGraphicAgent();
                            data.graphicAgent.SetActor(null);
                            data.graphicAgent.LoadedAble(actorComp);
                        }
                        if (!string.IsNullOrEmpty(data.m_PlayAciton))
                        {
                            if(actorComp != null)
                            {
                                data.graphicAgent.PlayAnimation(data.m_PlayAciton);
                            }
                            Animator animator = data.m_pModel.GetBehaviour<Animator>();
                            if (animator)
                            {
                                animator.enabled = true;
                                animator.Play(data.m_PlayAciton);
                            }
                        }
#if USE_BONESPRING
                        SpringManager springManager = data.m_pModel.GetBehaviour<SpringManager>();
                        if (springManager)
                        {
                            springManager.enabled = false;
                        }
#endif
                    }
                    m_vDatas[i] = data;
                    break;
                }
            }
            
        }
        //-------------------------------------------
        void OnLoadSign(InstanceOperiaon model)
        {
            for (int i = 0; i < m_vDatas.Count; i++)
            {
                if (m_vDatas[i].m_pLoadOperiaon == model)
                {
                    var data = m_vDatas[i];
                    model.bUsed = m_pBase != null && m_pBase.IsVisible() && model.strFile.CompareTo(data.m_strModelFile) == 0;
                    if (!model.bUsed)
                    {
                        data.Clear();
                        m_vDatas.RemoveAt(i);
                    }
                    return;
                }
            }
        }
        //-------------------------------------------
#if UNITY_EDITOR
        protected override void OnUpdate(float fFrame)
        {
            for (int i = 0; i < m_vDatas.Count; i++)
            {
                var data = m_vDatas[i];
                if (data.m_SyncRT)
                {
                    if(data.m_pRTCamera)
                    {
                        data.m_pRTCamera.SetPosition(data.m_SyncRT.CameraPos + Vector3.right * 100 * i);
                        data.m_pRTCamera.SetEulerAngle(data.m_SyncRT.CameraRotate);
                        data.m_pRTCamera.SetFov(data.m_SyncRT.CameraFOV);
                    }
                    if (data.m_pModel)
                    {
                        data.m_pModel.SetPosition(data.m_SyncRT.ModelPos + Vector3.right * 100 * i);
                        data.m_pModel.SetScale(data.m_SyncRT.ModelScale);
                        data.m_pModel.SetEulerAngle(data.m_SyncRT.ModelRotate);
                    }
                }
            }
        }
#endif
    }
}
