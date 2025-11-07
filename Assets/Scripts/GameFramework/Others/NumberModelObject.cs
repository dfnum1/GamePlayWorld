/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	NumberModelObject
作    者:	HappLI
描    述:	3d数字
*********************************************************************/
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Core
{
    public class NumberModelObject : IUserData
    {
        private uint m_nNumber = 0;

        private bool m_bClear = false;
        private List<AInstanceAble> m_vNumberModels = null;

        private Vector3 m_Position = Vector3.zero;
        private Vector3 m_LimitSize = Vector3.zero;
        //--------------------------------------------------------
        public void SetBind(Vector3 position, Vector3 size)
        {
            m_bClear = false;
            m_Position = position;
            m_LimitSize = size;
        }
        //--------------------------------------------------------
        public void SetNumber(uint number)
        {
            if (m_nNumber == number)
                return;

            Clear();
            m_bClear = false;
            m_nNumber = number;
            string strNum = number.ToString();
            if (m_vNumberModels == null) m_vNumberModels = new List<AInstanceAble>(strNum.Length);
            m_vNumberModels.Clear();
            for (int i =0; i < strNum.Length; ++i)
            {
                m_vNumberModels.Add(null);
                string file = $"Assets/Datas/Environments/Numbers/Number{strNum[i]}.prefab";
                var op = FileSystemUtil.SpawnInstance(file);
                op.OnCallback = OnNumber;
                op.OnSign = OnNumberSign;
                op.pByParent = RootsHandler.ScenesRoot;
                op.userData0 = new Variable1() { uintVal = number };
                op.userData1 = new Variable1() { intVal = i };
            }
        }
        //--------------------------------------------------------
        void OnNumberSign(InstanceOperiaon op)
        {
            int index = op.userData1.ToInt();
            op.bUsed = !m_bClear && m_nNumber == op.userData0.ToUInt() && m_nNumber>0 && m_vNumberModels!=null && index< m_vNumberModels.Count && m_vNumberModels.Count>0;
        }
        //--------------------------------------------------------
        void OnNumber(InstanceOperiaon op)
        {
            int index = op.userData1.ToInt();
            if(index < m_vNumberModels.Count&& index>=0)
            {
                m_vNumberModels[index] = op.GetAble();
                UpdateSize();
            }
            else
            {
                if(op.GetAble()) op.GetAble().Destroy();
            }
        }
        //--------------------------------------------------------
        void UpdateSize()
        {
            float totalSize = 0;
            for(int i =0; i < m_vNumberModels.Count; ++i)
            {
                if (m_vNumberModels[i] == null)
                    return;
                var number = m_vNumberModels[i] as NumberModel;
                if (number == null)
                    return;
                totalSize += number.size.x;
            }

            float offset = (totalSize - m_LimitSize.x) / 2;
            totalSize = offset;
            for (int i = 0; i < m_vNumberModels.Count; ++i)
            {
                if (m_vNumberModels[i] == null)
                    return;
                var number = m_vNumberModels[i] as NumberModel;
                if (number == null)
                    return;

                number.SetPosition(Vector3.left * totalSize + m_Position, true);
                totalSize += number.size.x;
            }
        }
        //--------------------------------------------------------
        public void Clear()
        {
            m_nNumber = 0;
            m_bClear = true;
            if (m_vNumberModels == null) return;
            foreach(var db in m_vNumberModels)
            {
                if(db!=null)
                    db.Destroy();
            }
            m_vNumberModels.Clear();
        }
        //--------------------------------------------------------
        public void Destroy()
        {
            Clear();
        }
    }
}