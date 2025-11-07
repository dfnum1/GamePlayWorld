#if USE_GUIDESYSTEM
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	GuideData
作    者:	HappLI
描    述:	引导单组数据
*********************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Plugin.Guide
{
    [System.Serializable]
	public class GuideGroup : IUserData
	{
        public int Guid = 0;
#if UNITY_EDITOR
        public string Name = "";
        [System.NonSerialized]
        public string strFile = "";
#endif
        public int priority = 0;
        public ushort Tag = 0xffff;
        public bool bRemoveOver = false;
        public bool bChangeStateBreak = false;

        public List<TriggerNode> vTriggers = null;
        public List<ExcudeNode> vExcudes = null;
        public List<StepNode> vSteps = null;
        public List<GuideOperate> vOperates = null;
        public List<ArgvPort> vPorts = null;

        [System.NonSerialized]
        Dictionary<int, ArgvPort> m_vPorts = new Dictionary<int, ArgvPort>();

        [System.NonSerialized]
        Dictionary<int, BaseNode> m_vNodes = new Dictionary<int, BaseNode>();

        [System.NonSerialized]
        private bool m_bInited = false;
        public void Init(bool bForece = false)
        {
            if (m_bInited && !bForece) return;
            m_bInited = true;

            BuildMapping();
            if (vPorts != null)
            {
                for (int i = 0; i < vPorts.Count; ++i)
                {
                    vPorts[i].Init();
                }
            }
            if(vExcudes != null)
            {
                for (int i = 0; i < vExcudes.Count; ++i)
                    vExcudes[i].Init(this);
            }

            if (vSteps!=null)
            {
                for (int i = 0; i < vSteps.Count; ++i)
                    vSteps[i].Init(this);
            }
            if(vOperates!=null)
            {
                for (int i = 0; i < vOperates.Count; ++i)
                    vOperates[i].Init(this);
            }
            if (vTriggers != null)
            {
                for (int i = 0; i < vTriggers.Count; ++i)
                    vTriggers[i].Init(this);
            }
        }
        //------------------------------------------------------
        public void BuildMapping()
        {
            m_vPorts.Clear();
            m_vNodes.Clear();
            if (vPorts != null)
            {
                for (int i = 0; i < vPorts.Count; ++i)
                {
                    m_vPorts[vPorts[i].guid] = vPorts[i];
                }
            }
            if (vSteps != null)
            {
                for (int i = 0; i < vSteps.Count; ++i)
                {
                    m_vNodes[vSteps[i].Guid] = vSteps[i];
                }
            }
            if (vOperates != null)
            {
                for (int i = 0; i < vOperates.Count; ++i)
                {
                    m_vNodes[vOperates[i].Guid] = vOperates[i];
                }
            }
            if (vTriggers != null)
            {
                for (int i = 0; i < vTriggers.Count; ++i)
                {
                    m_vNodes[vTriggers[i].Guid] = vTriggers[i];
                }
            }
            if (vExcudes != null)
            {
                for (int i = 0; i < vExcudes.Count; ++i)
                    m_vNodes[vExcudes[i].Guid] = vExcudes[i];
            }
        }
        //------------------------------------------------------
        public T GetNode<T>(int guide) where T : BaseNode
        {
            BaseNode pNode;
            if (m_vNodes.TryGetValue(guide, out pNode))
                return pNode as T;
            return null;
        }
        //------------------------------------------------------
        public ArgvPort GetPort(int guide)
        {
            ArgvPort pNode;
            if (m_vPorts.TryGetValue(guide, out pNode))
                return pNode;
            return null;
        }
        //------------------------------------------------------
        public Dictionary<int, BaseNode> GetAllNodes()
        {
            return m_vNodes;
        }
#if UNITY_EDITOR
        public int BuildStepGUID()
        {
            int guide = 0;
            foreach(var db in m_vNodes)
            {
                guide = Mathf.Max(guide, db.Key);
            }
            return ++guide;
        }
        //------------------------------------------------------
        internal void Save()
        {
            if (vSteps != null)
            {
                for (int i = 0; i < vSteps.Count; ++i)
                {
                    vSteps[i].CheckPorts();
                    vSteps[i].Save();
                }
            }
            if (vOperates != null)
            {
                for (int i = 0; i < vOperates.Count; ++i)
                {
                    vOperates[i].CheckPorts();
                    vOperates[i].Save();
                }
            }
            if (vTriggers != null)
            {
                for (int i = 0; i < vTriggers.Count; ++i)
                {
                    vTriggers[i].CheckPorts();
                    vTriggers[i].Save();
                }
            }
            if (vExcudes != null)
            {
                for (int i = 0; i < vExcudes.Count; ++i)
                {
                    vExcudes[i].CheckPorts();
                    vExcudes[i].Save();
                }
            }

            if (string.IsNullOrEmpty(strFile)) return;
            System.IO.FileStream fs = new System.IO.FileStream(strFile, System.IO.FileMode.OpenOrCreate);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);
            sw.BaseStream.SetLength(0);
            sw.BaseStream.Position = 0;
            sw.Write(JsonUtility.ToJson(this, true));
            sw.Close();
        }
#endif
        public void Destroy()
        {

        }
    }
}
#endif