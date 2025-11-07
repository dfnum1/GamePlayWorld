/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	TweenerGroup
作    者:	HappLI
描    述:	动画组
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
namespace Framework.RtgTween
{
    [UI.UIWidgetExport]
    [ExecuteInEditMode]
    public class TweenerGroup : MonoBehaviour
    {
        [System.Serializable]
        public class GroupItem
        {
            public string Name;
            public bool bAutoHide;
            public RtgTweenerParam[] Propertys;

            public string fmodEvent;

            public void ClearTweenID()
            {
                if (Propertys == null) return;
                for (int i = 0; i < Propertys.Length; ++i)
                    Propertys[i].tweenerID = 0;
            }
        }
        public short nUseIndex = -1;
        public List<GroupItem> Groups;

        [System.NonSerialized]
        private short m_nCurrentIndex = -1;
        //------------------------------------------------------
        public short GetCurrentIndex()
        {
            return m_nCurrentIndex;
        }
        //------------------------------------------------------
        void Backup()
        {
            if (Groups != null)
            {
                for (int i = 0; i < Groups.Count; ++i)
                {
                    GroupItem gp = Groups[i];
                    if (gp.Propertys == null) continue;
                    for (int j = 0; j < gp.Propertys.Length; ++j)
                    {
                        gp.Propertys[j].BackUp();
                    }
                }
            }
        }
        //------------------------------------------------------
        public void OnEnable()
        {
            if (Groups == null) return;
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            Play(nUseIndex);
        }
        //------------------------------------------------------
        public void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            Stop();
        }
        //------------------------------------------------------
        public bool isPlaying()
        {
            if (Groups == null) return false;
            if (m_nCurrentIndex < 0 || m_nCurrentIndex >= Groups.Count) return false;
            GroupItem gp = Groups[m_nCurrentIndex];
            for (int i = 0; i < gp.Propertys.Length; ++i)
            {
                if(gp.Propertys[i].tweenerID!=0)
                {
                    if (RtgTweenerManager.getInstance().findTween(gp.Propertys[i]))
                        return true;
                }
            }
            return false;
        }
        //------------------------------------------------------
        public void ForcePlay(short index)
        {
            if (Groups == null) return;
            if (index < 0 || index >= Groups.Count) return;
            m_nCurrentIndex = index;
            if (Groups[m_nCurrentIndex].Propertys == null) return;
            Backup();
            if (!this.gameObject.activeInHierarchy) this.gameObject.SetActive(true);
            GroupItem gp = Groups[m_nCurrentIndex];
            for (int i = 0; i < gp.Propertys.Length; ++i)
            {
                gp.Propertys[i].pController = this;
                gp.Propertys[i].listerner += OnTweenComplete;
                gp.Propertys[i].tweenerID = RtgTweenerManager.getInstance().addTween(ref gp.Propertys[i]);
            }
        }
        //------------------------------------------------------
        public void Play(short index, bool bForce =true)
        {
            if (bForce)
            {
                ForcePlay(index);
                return;
            }
            if (Groups == null) return;
            if (index < 0 || index >= Groups.Count) return;
            if (m_nCurrentIndex == index/* && isPlaying()*/)
                return;
            m_nCurrentIndex = index;
            if (Groups[m_nCurrentIndex].Propertys == null) return;
            Backup();
            if (!this.gameObject.activeInHierarchy) this.gameObject.SetActive(true);
            GroupItem gp = Groups[m_nCurrentIndex];
            for (int i = 0; i < gp.Propertys.Length; ++i)
            {
                gp.Propertys[i].pController = this;
                gp.Propertys[i].listerner += OnTweenComplete;
                gp.Propertys[i].tweenerID = RtgTweenerManager.getInstance().addTween(ref gp.Propertys[i]);
            }
        }
        //------------------------------------------------------
        public bool Play(short index,OnTweenerDelegate listener, bool bForce = true)
        {
            if (Groups == null) return false;
            if (index < 0 || index >= Groups.Count) return false;
            if (!bForce && m_nCurrentIndex == index && isPlaying())
                return true;
            m_nCurrentIndex = index;
            if (Groups[m_nCurrentIndex].Propertys == null) return false;
            Backup();
            if (!this.gameObject.activeInHierarchy) this.gameObject.SetActive(true);

            GroupItem gp = Groups[m_nCurrentIndex];
            for (int i = 0; i < gp.Propertys.Length; ++i)
            {
                gp.Propertys[i].pController = this;
                if(listener!=null)
                    gp.Propertys[i].listerner += listener;
                gp.Propertys[i].listerner += OnTweenComplete;
                gp.Propertys[i].tweenerID = RtgTweenerManager.getInstance().addTween(ref gp.Propertys[i]);
            }
            return true;
        }
        //------------------------------------------------------
        public void Stop()
        {
            if (m_nCurrentIndex < 0 || m_nCurrentIndex >= Groups.Count)
                return;

            int index = m_nCurrentIndex;
            m_nCurrentIndex = -1;
            if (Groups[index].Propertys == null) return;
            for (int i = 0; i < Groups[index].Propertys.Length; ++i)
            {
                RtgTweenerManager.getInstance().removeTween(Groups[index].Propertys[i]);
            }
        }
        //------------------------------------------------------
        void OnTweenComplete(RtgTweenerParam param, ETweenStatus status)
        {
            if(status == ETweenStatus.Complete)
            {
                if (m_nCurrentIndex >= 0 && m_nCurrentIndex < Groups.Count)
                {
                    Groups[m_nCurrentIndex].ClearTweenID();
                    if (Groups[m_nCurrentIndex].bAutoHide)
                    {
                        this.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(TweenerGroup), true)]
    public class UITweenerGroupEditor : Editor
    {
        bool m_bEditor = false;
        class GroupDatas
        {
            public string Name="";
            public bool bAutoHide = false;
            public bool useDefaut = false;
            public string fmodEvent = null;
            public TweenerEditorDrawer.ParamData[] Datas = null;
            public RtgTweenerParam[] Propertys = null;
        }
        
        List<string> m_vPops = new List<string>();
        List<GroupDatas> m_vDatas = new List<GroupDatas>();
        private GroupDatas m_pCurrentGroup = null;

        UI.Backuper m_Backuper = new UI.Backuper();
        //------------------------------------------------------
        private void OnEnable()
        {
            TweenerEditorDrawer.OnEnable();
            TweenerGroup tweener = target as TweenerGroup;
            if (tweener.Groups != null)
            {
                for (int i = 0; i < tweener.Groups.Count; ++i)
                {
                    GroupDatas group = new GroupDatas();
                    group.Datas = TweenerEditorDrawer.BuildCheck(tweener.Groups[i].Propertys);
                    group.Name = tweener.Groups[i].Name;
                    group.fmodEvent = tweener.Groups[i].fmodEvent;
                    group.bAutoHide = tweener.Groups[i].bAutoHide;
                    group.useDefaut = tweener.nUseIndex == i;
                    group.Propertys = tweener.Groups[i].Propertys;
                    m_vDatas.Add(group);
                }
            }

            m_pCurrentGroup = null;
            if (tweener.nUseIndex >=0 && tweener.nUseIndex < m_vDatas.Count)
                m_pCurrentGroup = m_vDatas[tweener.nUseIndex];

            RefreshPop();
        }
        //------------------------------------------------------
        private void OnDisable()
        {
            TweenerGroup tweener = target as TweenerGroup;
            if (tweener == null) return;
            for(int i =0; i < tweener.Groups.Count; ++i)
                TweenerEditorDrawer.CheckTweenAlphaParam(tweener.gameObject, tweener.Groups[i].Propertys);
            TweenerEditorDrawer.OnDisable();
            m_Backuper.Recovert();
        }
        //------------------------------------------------------
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TweenerGroup tweener = target as TweenerGroup;
            m_bEditor = EditorGUILayout.Toggle("进入编辑", m_bEditor);
            EditorGUILayout.LabelField("playIndex:" + tweener.GetCurrentIndex());
            if (!m_bEditor)
                return;
            m_Backuper.SetController(tweener.transform);

            GUILayout.BeginHorizontal();
            int index = m_vDatas.IndexOf(m_pCurrentGroup)+1;
            index = EditorGUILayout.Popup(index, m_vPops.ToArray());
            if (index <= 0) m_pCurrentGroup = null;
            else if (index < m_vPops.Count)
            {
                m_pCurrentGroup = m_vDatas[index-1];
            }
            if(GUILayout.Button("添加"))
            {
                GroupDatas group = new GroupDatas();
                group.Datas = TweenerEditorDrawer.BuildCheck(null);
                group.Name = m_vDatas.Count.ToString();
                group.fmodEvent = null;
                group.bAutoHide = false;
                group.useDefaut = false;
                group.Propertys = null;
                m_vDatas.Add(group);
                m_pCurrentGroup = group;
                RefreshPop();
            }
            if (m_pCurrentGroup!=null && GUILayout.Button("删除"))
            {
                if(EditorUtility.DisplayDialog("提示", "确定是否删除?","删除", "取消"))
                {
                    m_vDatas.Remove(m_pCurrentGroup);
                    RefreshPop();
                }
            }
            GUILayout.EndHorizontal();

            if(m_pCurrentGroup!=null)
            {
                EditorGUI.BeginChangeCheck();
                m_pCurrentGroup.Name = EditorGUILayout.TextField("描述", m_pCurrentGroup.Name);
                if(EditorGUI.EndChangeCheck())
                {
                    RefreshPop();
                }
                bool bDefualt = EditorGUILayout.Toggle("作为缺省", m_pCurrentGroup.useDefaut);
                if(m_pCurrentGroup.useDefaut!= bDefualt)
                {
                    if(bDefualt)
                    {
                        for (int i = 0; i < m_vDatas.Count; ++i)
                            m_vDatas[i].useDefaut = false;
                    }

                    m_pCurrentGroup.useDefaut = bDefualt;
                }
                m_pCurrentGroup.bAutoHide = EditorGUILayout.Toggle("自动显隐", m_pCurrentGroup.bAutoHide);
                m_pCurrentGroup.fmodEvent = EditorGUILayout.TextField("FMODEvent", m_pCurrentGroup.fmodEvent);

                List<RtgTweenerParam> datas = TweenerEditorDrawer.DrawTween(tweener, ref m_pCurrentGroup.Datas, m_Backuper);
                m_pCurrentGroup.Propertys = datas.ToArray();
            }

            tweener.nUseIndex = -1;
            if (tweener.Groups == null) tweener.Groups = new List<TweenerGroup.GroupItem>();
            tweener.Groups.Clear();
            for (int i = 0; i < m_vDatas.Count; ++i)
            {
                TweenerGroup.GroupItem group = new TweenerGroup.GroupItem();
                group.Name = m_vDatas[i].Name;
                group.fmodEvent = m_vDatas[i].fmodEvent;
                group.bAutoHide = m_vDatas[i].bAutoHide;
                group.Propertys = m_vDatas[i].Propertys;
                tweener.Groups.Add(group);

                if(m_vDatas[i].useDefaut)
                    tweener.nUseIndex = (short)i;
            }

            serializedObject.ApplyModifiedProperties();
        }
        //------------------------------------------------------
        public void RefreshPop()
        {
            m_vPops.Clear();
            m_vPops.Add("None");
            for (int i = 0; i < m_vDatas.Count; ++i)
            {
                m_vPops.Add(m_vDatas[i].Name + "[" + i + "]");
            }
        }
    }

#endif
}

