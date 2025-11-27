/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	DataSearcher
作    者:	
描    述:	数据搜索器
*********************************************************************/
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Framework.Guide.Editor.GuideSystemEditor;
namespace Framework.Guide.Editor
{
    //------------------------------------------------------
    public partial class DataSearcher : GuideSearcher
    {
        //------------------------------------------------------
        protected override bool OnDragItem(AssetTree.ItemData item)
        {
            return true;
        }
        //------------------------------------------------------
        protected override bool OnDrawItem(Rect cellRect, AssetTree.TreeItemData item, int column, bool bSelected, bool focused)
        {
            AssetTree.ItemData itemData = item.data as AssetTree.ItemData;
            ItemEvent guideItem = itemData as ItemEvent;
            GuideSystemEditor.DataParam dataParam = (GuideSystemEditor.DataParam)guideItem.param;
            item.displayName = dataParam.Data.Name + "[Id=" + dataParam.Data.Guid + "]";
            if (dataParam.Data.Tag >= 0 && dataParam.Data.Tag < ushort.MaxValue)
            {
                item.displayName += "[Tag=" + dataParam.Data.Tag + "]";
            }

            GUIContent content = new GUIContent(item.displayName, "存储文件:" + dataParam.Data.strFile);
            GUI.Label(new Rect(cellRect.x, cellRect.y, cellRect.width - 40, cellRect.height), content);
            if(GUI.Button(new Rect(cellRect.xMax-60, cellRect.y,40, cellRect.height), "移除"))
            {
                if (EditorUtility.DisplayDialog("提示", "确认是否要移除本引导组?", "移除", "取消"))
                {
                    GuideSystem.getInstance().datas.Remove(item.id);
                    if (!string.IsNullOrEmpty(dataParam.Data.strFile))
                    {
                        string path = dataParam.Data.strFile.Replace("\\", "/");
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        string metaPath = path + ".meta";
                        if (System.IO.File.Exists(metaPath))
                        {
                            System.IO.File.Delete(metaPath);
                        }
                    }
                    Search(m_assetTree.searchString);
                }
            }
            if (GUI.Button(new Rect(cellRect.xMax - 110, cellRect.y, 50, cellRect.height), "重命名"))
            {
                RenamePopup.Show(dataParam.Data, dataParam.Data.Name);
            }
            if (GUI.Button(new Rect(cellRect.xMax - 20, cellRect.y, 20, cellRect.height), "☝"))
            {
                if(!string.IsNullOrEmpty(dataParam.Data.strFile))
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(dataParam.Data.strFile));
            }
            return true;
        }
        //------------------------------------------------------
        protected override void OnSawpDatas()
        {
            if (GuideSystem.getInstance().datas == null) return;
            Dictionary<int, GuideGroup> datas = GuideSystem.getInstance().datas;
            datas.Clear();
            List<AssetTree.ItemData> vItems = m_assetTree.GetDatas();
            for(int i = 0; i < vItems.Count; ++i)
            {
                ItemEvent temp = vItems[i] as ItemEvent;
                GuideSystemEditor.DataParam dataParam = (GuideSystemEditor.DataParam)temp.param;
                datas[vItems[i].id] = dataParam.Data;
            }
        }
        //------------------------------------------------------
        protected override void OnSearch(string query)
        {
            if (GuideSystem.getInstance().datas == null) return;
            GuideSystemEditor pEditor = GuideSystemEditor.Instance;
            foreach (var db in GuideSystem.getInstance().datas)
            {
                bool bQuerty = IsQuery(query, db.Value.Guid+db.Value.Name);
                if (!bQuerty) continue;
                GuideSystemEditor.DataParam param = new GuideSystemEditor.DataParam();
                param.Data = db.Value;

                ItemEvent item = new ItemEvent();
                item.param = param;
                item.callback = pEditor.LoadData;

                item.id = db.Key;
                item.name = db.Value.Name + "[Id=" + db.Value.Guid + "]";
                if(db.Value.Tag>=0 && db.Value.Tag < ushort.MaxValue)
                {
                    item.name += "[Tag=" + db.Value.Tag + "]";
                }
                m_assetTree.AddData(item);
            }
        }
        //------------------------------------------------------
        protected override void OnClose()
        {
            GuideSystemEditor.Instance.Save(false);
        }
    }
}
#endif