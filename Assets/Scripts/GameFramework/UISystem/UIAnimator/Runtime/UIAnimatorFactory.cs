/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	UIAnimatorGroup
作    者:	HappLI
描    述:	UI 动画
*********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using Framework.Core;
using UnityEngine;

namespace Framework.UI
{

    public class UIAnimatorFactory : Base.Singleton<UIAnimatorFactory>
    {
        Dictionary<int, UIAnimatorGroupData> m_vGroups = new Dictionary<int, UIAnimatorGroupData>();

        List<UIAnimatorPlayable> m_vPlayables = new List<UIAnimatorPlayable>(16);
        ObjectPool<UIAnimatorPlayable> m_vPools = new ObjectPool<UIAnimatorPlayable>(16);

        protected bool m_bInited = false;
        protected bool m_bLockDirty = false;
        UIAnimatorAssets m_pAssets = null;
        //------------------------------------------------------
        public Dictionary<int, UIAnimatorGroupData> Groups
        {
            get { return m_vGroups; }
        }
        //------------------------------------------------------
        public void Init(UIAnimatorAssets uiAnimations, bool bForce = false)
        {
            if (!bForce && m_bInited) return;
            m_bInited = true;

            ClearPlayables();
            if (uiAnimations == null) return;
            m_pAssets = uiAnimations;
            if (uiAnimations.animations != null)
            {
                for (int i = 0; i < uiAnimations.animations.Length; ++i)
                {
                    m_vGroups[uiAnimations.animations[i].nID] = uiAnimations.animations[i];
                }
            }
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public void Init(string strFile, bool bForce = false)
        {
            if (!bForce && m_bInited) return;
            m_bInited = true;
            ClearPlayables();
            m_vGroups.Clear();
            m_pAssets = UnityEditor.AssetDatabase.LoadAssetAtPath<UIAnimatorAssets>(strFile);
            if (m_pAssets)
            {
                if(m_pAssets.animations!=null)
                {
                    for(int i = 0; i< m_pAssets.animations.Length; ++i)
                    {
                        m_vGroups[m_pAssets.animations[i].nID] = m_pAssets.animations[i];
                    }
                }
            }
        }
#endif
        //------------------------------------------------------
        public void ClearPlayables()
        {
            for (int i = 0; i < m_vPlayables.Count; ++i)
            {
                m_vPlayables[i].Stop();
                m_vPlayables[i].Clear();
                m_vPools.Release(m_vPlayables[i]);
            }
            m_vPlayables.Clear();
        }
        //------------------------------------------------------
        public void RemovePlayAble(UIAnimatorPlayable playAble,bool bCallback = true)
        {
            if (playAble == null)
                return;
            playAble.StopRecover(bCallback);
            playAble.Clear();
            m_vPlayables.Remove(playAble);
            m_vPools.Release(playAble);
            m_bLockDirty = true;
        }
        //------------------------------------------------------
        public UIAnimatorPlayable CreatePlayAble(int nAnimationID, UnityEngine.Object pController)
        {
            if (nAnimationID <= 0) return null;
            for(int i = 0; i < m_vPlayables.Count; ++i)
            {
                if (m_vPlayables[i].GetController() == pController)
                    return m_vPlayables[i];
            }
            UIAnimatorPlayable playAble = null;
            UIAnimatorGroupData groupData;
            if (m_vGroups.TryGetValue(nAnimationID, out groupData))
            {
                playAble = m_vPools.Get();
                playAble.SetGroupData(groupData);
                playAble.SetController(pController);
                playAble.SetReverse(false);
                m_vPlayables.Add(playAble);
                m_bLockDirty = true;
                return playAble;
            }
            return null;
        }
#if UNITY_EDITOR
        //------------------------------------------------------
        public void Save(string strFile)
        {
            if(m_pAssets == null)
                m_pAssets = UnityEditor.AssetDatabase.LoadAssetAtPath<UIAnimatorAssets>(strFile);
            if(m_pAssets == null)
            {
                m_pAssets = ScriptableObject.CreateInstance<UIAnimatorAssets>();
                m_pAssets.name = Path.GetFileNameWithoutExtension(strFile);
                UnityEditor.AssetDatabase.CreateAsset(m_pAssets, strFile);
                m_pAssets = UnityEditor.AssetDatabase.LoadAssetAtPath<UIAnimatorAssets>(strFile);
            }
            if (m_pAssets == null) return;
            List<UIAnimatorGroupData> groups = new List<UIAnimatorGroupData>();
            foreach (var db in m_vGroups)
            {
                groups.Add(db.Value);
            }
            m_pAssets.animations = groups.ToArray();
            UnityEditor.EditorUtility.SetDirty(m_pAssets);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
        }
#endif
        //------------------------------------------------------
        public void Update(float fFrameTime)
        {
            if (!m_bInited) return;
            m_bLockDirty = false;
            UIAnimatorPlayable playable;
            for(int i =0; i < m_vPlayables.Count;)
            {
                playable = m_vPlayables[i];
                if(playable.Update(fFrameTime))
                {
                    if(m_bLockDirty)
                    {
                        m_vPlayables.Remove(playable);
                        playable.Clear();
                        m_vPools.Release(playable);
                        break;
                    }
                    else
                    {
                        m_vPlayables.RemoveAt(i);
                        playable.Clear();
                        m_vPools.Release(playable);
                    }
                }
                else
                {
                    ++i;
                }
            }
            m_bLockDirty = false;
        }
        //------------------------------------------------------
        public float GetMaxFrameTimeStep()
        {
            return 0.05f;
        }
        //------------------------------------------------------
        public int GetMaxFrameTimeMillisecondStep()
        {
            return 50;
        }
        //------------------------------------------------------
        public bool IsTweening(Transform pObj)
        {
            for(int i = 0; i < m_vPlayables.Count; ++i)
            {
                if (m_vPlayables[i].GetController() == pObj) return true;
            }
            return false;
        }
    }
}

