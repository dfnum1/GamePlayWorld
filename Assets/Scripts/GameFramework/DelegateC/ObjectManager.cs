/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	ObjectManager
作    者:	HappLI
描    述:	Plus 代理
*********************************************************************/
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Framework.Core
{
    public class ObjectManager : Base.Singleton<ObjectManager>
    {
        int m_GeneID = 1;
        List<ObjectNode> m_vCatchNode = new List<ObjectNode>(64);
        Dictionary<int, ObjectNode> m_vNodes = new Dictionary<int, ObjectNode>();
        //-------------------------------------------------
        public ObjectManager()
        {
            m_GeneID = 1;
        }
        //-------------------------------------------------
        public bool Contains(int guid)
        {
            return m_vNodes.ContainsKey(guid);
        }
        //-------------------------------------------------
        public void Clear()
        {
            foreach (var db in m_vNodes)
            {
                m_vCatchNode.Add(db.Value);
                db.Value.Destroy();
            }
            m_vNodes.Clear();
        }
        //-------------------------------------------------
        public void SetTransform(int guid, Vector3 pos, Vector3 euler, Vector3 scale)
        {
            ObjectNode node;
            if (m_vNodes.TryGetValue(guid, out node))
            {
                node.SetPosition(pos);
                node.SetEulerAngle(euler);
                node.SetScale(scale);
            }
        }
        //-------------------------------------------------
        public void PauseAnimation(int guid, bool bPause)
        {
            ObjectNode node;
            if (m_vNodes.TryGetValue(guid, out node))
            {
                node.PauseAnimation(bPause);
            }
        }
        //-------------------------------------------------
        public void SpeedAnimation(int guid, float speed)
        {
            ObjectNode node;
            if (m_vNodes.TryGetValue(guid, out node))
            {
                node.SetAnimSpeed(speed);
            }
        }
        //-------------------------------------------------
        public void SetVisible(int guid, bool visible)
        {
            ObjectNode node;
            if (m_vNodes.TryGetValue(guid, out node))
            {
                node.SetVisible(visible);
            }
        }
        //-------------------------------------------------
        public void PlayAnimation(int guid, int animation, float blendDuration, float blendOffset, int layer = 0)
        {
            ObjectNode node;
            if (m_vNodes.TryGetValue(guid, out node))
            {
                node.PlayAnimation(animation, blendDuration, blendOffset, layer);
            }
        }
        //-------------------------------------------------
        public void SetPosition(int guid, Vector3 pos)
        {
            ObjectNode node;
            if (m_vNodes.TryGetValue(guid, out node))
            {
                node.SetPosition(pos);
            }
        }
        //-------------------------------------------------
        public void SetEulerAngle(int guid, Vector3 angle)
        {
            ObjectNode node;
            if (m_vNodes.TryGetValue(guid, out node))
            {
                node.SetEulerAngle(angle);
            }
        }
        //-------------------------------------------------
        public void SetDirection(int guid, Vector3 forwad, Vector3 Up)
        {
            ObjectNode node;
            if (m_vNodes.TryGetValue(guid, out node))
            {
                Quaternion rot = Quaternion.LookRotation(forwad, Up);
                node.SetEulerAngle(rot.eulerAngles);
            }
        }
        //-------------------------------------------------
        public void SetScale(int guid, Vector3 scale)
        {
            ObjectNode node;
            if (m_vNodes.TryGetValue(guid, out node))
            {
                node.SetScale(scale);
            }
        }
        //-------------------------------------------------
        public int CreateModel(int model)
        {
            return 0;
//             Data.CsvData_Models csvModel = Data.DataManager.getInstance().Models;
//             Data.CsvData_Models.ModelsData modelData = csvModel.GetData((uint)model);
//             if (modelData == null) return 0;
//             return CreateObject(modelData.strFile, model.ToString());
        }
        //-------------------------------------------------
        public int CreateObject(string file, string name)
        {
            ObjectNode entity = new ObjectNode();
#if UNITY_EDITOR
            entity.strFile = file;
#endif
            entity.strName = name;
            entity.GUID = m_GeneID++;

            InstanceOperiaon pCallback = FileSystemUtil.SpawnInstance(file);
            pCallback.OnCallback = entity.OnSpawnCallback;
            pCallback.OnSign = entity.OnSpawnSign;
            pCallback.pByParent = RootsHandler.ActorsRoot;

            m_vNodes.Add(entity.GUID, entity);
            return entity.GUID;
        }
        //-------------------------------------------------
        public void RemoveObject(int guid)
        {
            ObjectNode pAble;
            if (m_vNodes.TryGetValue(guid, out pAble))
            {
                pAble.Destroy();
                m_vNodes.Remove(guid);
            }
        }
        //-------------------------------------------------
        public int CreateScene(uint sceneID, Vector3 position, Vector3 eulerAngle, Vector3 scale)
        {
            return 0;
//            Data.CsvData_RunScenes runScene = Data.DataManager.getInstance().RunScenes;
//            if (runScene == null) return 0;
//            Data.DungonScene sceneData = runScene.GetData(sceneID);
//            if (sceneData == null) return 0;

//            ObjectNode entity = new ObjectNode();
//#if UNITY_EDITOR
//            entity.strName = sceneData.strName;
//#endif
//            entity.GUID = m_GeneID++;
//            entity.SetPosition(position);
//            entity.SetScale(scale);
//            entity.SetEulerAngle(eulerAngle);

//            InstanceOperiaon pCallback = FileSystemUtil.SpawnInstance(sceneData.strFile);
//            pCallback.OnCallback = entity.OnSpawnCallback;
//            pCallback.OnSign = entity.OnSpawnSign;
//            pCallback.pByParent = RootsHandler.ScenesRoot;

//            m_vNodes.Add(entity.GUID, entity);
//            return entity.GUID;
        }
        //-------------------------------------------------
        public void RemoveScene(int guid)
        {
            ObjectNode pAble;
            if (m_vNodes.TryGetValue(guid, out pAble))
            {
                pAble.Destroy();
                m_vNodes.Remove(guid);
            }
        }
        //-------------------------------------------------
        public void Update(float fFrameTime)
        {
        }
    }
}