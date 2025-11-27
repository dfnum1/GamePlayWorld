/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideSystemEditor
作    者:	
描    述:	引导编辑器
*********************************************************************/
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
namespace Framework.Guide.Editor
{
    public class GudieRecodeMode : MonoBehaviour
    {
        private int m_nLastSelectGOId = 0;
        private bool m_bRecodeMode = false;
        //-----------------------------------------------------
        private void Update()
        {
            if (GuideSystemEditor.Instance == null)
                return;
            if(m_bRecodeMode != GuideSystemEditor.IsRecodeMode)
            {
                m_nLastSelectGOId = -1;
                m_bRecodeMode = GuideSystemEditor.IsRecodeMode;
                if (m_bRecodeMode)
                {
                    if (UnityEngine.EventSystems.EventSystem.current && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject)
                        m_nLastSelectGOId = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetInstanceID();

                }
            }
            if (GuideSystemEditor.IsRecodeMode)
            {
                var eventCut = UnityEngine.EventSystems.EventSystem.current;
                if (eventCut)
                {
                    bool isOver = eventCut.IsPointerOverGameObject();
                    if (Camera.main && Input.GetMouseButtonDown(0) &&(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                    {
                        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if(Physics.Raycast(ray, out RaycastHit hitInfo, 10000, -1, QueryTriggerInteraction.Ignore))
                            GuideSystemEditor.Instance.AddRecodeClickZoom(hitInfo.point);
                        else
                            GuideSystemEditor.Instance.ShowNotification(new GUIContent("无法点击3D位置"),2);
                    }
                    else if (isOver && Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                    {
                        var curSelect = eventCut.currentSelectedGameObject;
                        if (curSelect == null)
                        {
                            PointerEventData pointerEventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);

                            pointerEventData.position = Input.mousePosition;

                            List<RaycastResult> results = ListPool<RaycastResult>.Get();
                            UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerEventData, results);
                            int count = results.Count;

                            var components = ListPool<Component>.Get();
                            for (int i = 0; i < results.Count; ++i)
                            {
                                components.Clear();
                                results[i].gameObject.GetComponents(components);
                                var componentsCount = components.Count;
                                for (var j = 0; j < componentsCount; j++)
                                {
                                    var behavor = components[j] as Behaviour;
                                    if (behavor && behavor.isActiveAndEnabled && (components[j] is IPointerClickHandler || components[j] is ICanvasRaycastFilter))
                                    {
                                        curSelect = components[j].gameObject;
                                        break;
                                    }
                                }
                                if (curSelect) break;
                            }
                            ListPool<Component>.Release(components);

                            ListPool<RaycastResult>.Release(results);
                        }


                        if (curSelect && curSelect.GetInstanceID() != m_nLastSelectGOId)
                        {
                            //! 点击了一个按钮
                            GuideEditorUtil.RecodeWidget(GuideSystemEditor.Instance, curSelect);
                        }
                        m_nLastSelectGOId = curSelect ? curSelect.GetInstanceID() : -1;
                    }
                }

            }
        }
    }
}
#endif
