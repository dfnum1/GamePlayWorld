#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.IMGUI.Controls;
using System.Reflection;
using Framework.Data;
using Framework.Base;
using System.Collections;
using Framework.ED;

namespace Framework.Data.ED
{
    [PluginEditorWindow("FormWindow", "OpenForm")]
    public class FormWindow : EditorWindow
    {
        public class ListItem : FormAssetView.ItemData
        {
            public object pData;
            public override Color itemColor()
            {
                return Color.white;
            }
        }

        UnityEditor.IMGUI.Controls.TreeViewState m_TreeState;
        UnityEditor.IMGUI.Controls.MultiColumnHeaderState m_AssetListMCHState;
        FormAssetView m_pTreeView = null;

        Data_Base m_dataBase;
        string m_strField = "";
        public List<object> BindDatas = new List<object>();
        FieldInfo[] m_Fileds = null;
        //-----------------------------------------------------
        public static FormWindow OpenForm(Data_Base dataBase, string strField="", List<object> vBindDatas = null)
        {
            FormWindow window = ScriptableObject.CreateInstance(typeof(FormWindow)) as FormWindow;
            window.ShowUtility();
            window.m_strField = strField;
            window.m_dataBase = dataBase;
            window.InitTable();
            if (vBindDatas != null)
                window.BindDatas = vBindDatas;
            return window;
        }
        //-----------------------------------------------------
        public static FormWindow OpenForm(System.Type tableType, string strField = "", List<object> vBindDatas = null)
        {
            if (tableType == null) return null;
            FormWindow window = ScriptableObject.CreateInstance(typeof(FormWindow)) as FormWindow;
            window.ShowUtility();
            window.m_strField = strField;
            window.m_dataBase =  DataEditorUtil.GetTable<Data_Base>(tableType,false);
            window.InitTable();
            if (vBindDatas != null)
                window.BindDatas = vBindDatas;
            return window;
        }
        //-----------------------------------------------------
        void InitTable()
        {
            if (m_dataBase != null)
            {
                System.Type dataType = DataEditorUtil.GetTableDataType(m_dataBase.GetType());
                if (dataType == null)
                    return;

                m_Fileds = dataType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (m_Fileds == null || m_Fileds.Length <= 0) return;

                float argvW = minSize.x / m_Fileds.Length;

                MultiColumnHeaderState.Column[] colums = new MultiColumnHeaderState.Column[m_Fileds.Length];
                for (int i = 0; i < m_Fileds.Length; ++i)
                {
                    colums[i] = new MultiColumnHeaderState.Column();
                    colums[i].headerContent = new GUIContent(m_Fileds[i].Name, "");
                    colums[i].canSort = false;
                    colums[i].minWidth = argvW;
                    colums[i].width = argvW;
                    colums[i].maxWidth = position.width;
                    colums[i].headerTextAlignment = TextAlignment.Center;
                    colums[i].canSort = false;
                    colums[i].autoResize = true;
                }
                var headerState = new MultiColumnHeaderState(colums);
                m_AssetListMCHState = headerState;

                m_TreeState = new UnityEditor.IMGUI.Controls.TreeViewState();
                m_pTreeView = new FormAssetView(m_TreeState, m_AssetListMCHState);
                m_pTreeView.Reload();

                m_pTreeView.OnItemDoubleClick = OnSelect;
                m_pTreeView.OnCellDraw += OnCellGUI;

                RefreshList();
            }
        }
        //-----------------------------------------------------
        protected void OnEnable()
        {
            minSize = new Vector2(1280, 720);
            
        }
        //-----------------------------------------------------
        void RefreshList()
        {
            var datasProp = m_dataBase.GetType().GetProperty("datas", BindingFlags.Instance | BindingFlags.Public);
            if (datasProp == null)
                return;

            var dataMap = datasProp.GetValue(m_dataBase);
            if (dataMap == null)
                return;

            if (!typeof(System.Collections.IDictionary).IsAssignableFrom(dataMap.GetType()))
                return;

            PropertyInfo[] properties = dataMap.GetType().GetProperties();
            PropertyInfo keysProp = System.Array.Find(properties, prop => prop.Name == "Keys");
            PropertyInfo valuesProp = System.Array.Find(properties, prop => prop.Name == "Values");

            if (keysProp == null || valuesProp == null)
                return;

            MethodInfo getValueMethod = valuesProp.PropertyType.GetMethod("GetEnumerator");
            if (getValueMethod == null)
                return;

            var genicParams = dataMap.GetType().GenericTypeArguments;
            if (genicParams.Length != 2)
                return;
            List<object> vDatas = new List<object>();
            IEnumerator valueEnumerator = (IEnumerator)getValueMethod.Invoke(valuesProp.GetValue(dataMap), null);
            while (valueEnumerator.MoveNext())
            {
                object value = valueEnumerator.Current;

                if (typeof(IList).IsAssignableFrom(value.GetType()))
                {
                    var list = value as IList;
                    foreach (object item in list)
                    {
                        vDatas.Add(item);
                    }
                }
                else
                {
                    vDatas.Add(value);
                }
            }

            m_pTreeView.BeginTreeData();
            for (int i = 0; i < vDatas.Count; ++i)
            {
                ListItem item = new ListItem();
                item.id = i;
                item.name = m_Fileds[0].GetValue(vDatas[i]).ToString();
                item.pData = vDatas[i];
                m_pTreeView.AddData(item);
            }
            m_pTreeView.EndTreeData();
        }
        //-----------------------------------------------------
        private void OnGUI()
        {
            if (m_pTreeView == null) return;
            m_pTreeView.searchString = EditorGUILayout.TextField("过滤", m_pTreeView.searchString);
            m_pTreeView.OnGUI(new Rect(0,20, position.width, position.height-20));
        }
        //-----------------------------------------------------
        bool OnCellGUI(Rect cellRect, FormAssetView.TreeItemData item, int column, bool bSelected, bool focused)
        {
            if (column < 0 || column >= m_Fileds.Length) return false;

            ListItem list = item.data as ListItem;
            item.displayName = list.id.ToString();

            var val = m_Fileds[column].GetValue(list.pData);
            if(val!=null && val.GetType().IsEnum)
            {
                val = EditorUtil.GetEnumDisplayName((System.Enum)val);
            }

            GUI.Label( cellRect, val!=null? val.ToString():"");
            
            return true;
        }
        //-----------------------------------------------------
        void OnSelect(FormAssetView.ItemData data)
        {
            ListItem list = data as ListItem;
            if(m_dataBase !=null && BindDatas != null && BindDatas.Count== 2)
            {
                FieldInfo field = BindDatas[1] as FieldInfo;
                if(field!=null)
                {
                    for (int i = 0; i < m_Fileds.Length; ++i)
                    {
                        if (m_Fileds[i].Name.ToLower().CompareTo(m_strField.ToLower()) == 0)
                        {
                            try
                            {
                                if (field.FieldType == m_Fileds[i].FieldType)
                                    field.SetValue(BindDatas[0], m_Fileds[i].GetValue(list.pData));
                                else
                                {
                                    string valStr = m_Fileds[i].GetValue(list.pData).ToString();
                                    if (field.FieldType == typeof(int))
                                    {
                                        int temp = 0;
                                        if(int.TryParse(valStr, out temp)) field.SetValue(BindDatas[0], temp);
                                    }
                                    else if (field.FieldType == typeof(uint))
                                    {
                                        uint temp = 0;
                                        if (uint.TryParse(valStr, out temp)) field.SetValue(BindDatas[0], temp);
                                    }
                                    else if (field.FieldType == typeof(byte))
                                    {
                                        byte temp = 0;
                                        if (byte.TryParse(valStr, out temp)) field.SetValue(BindDatas[0], temp);
                                    }
                                    else if (field.FieldType == typeof(short))
                                    {
                                        short temp = 0;
                                        if (short.TryParse(valStr, out temp)) field.SetValue(BindDatas[0], temp);
                                    }
                                    else if (field.FieldType == typeof(ushort))
                                    {
                                        ushort temp = 0;
                                        if (ushort.TryParse(valStr, out temp)) field.SetValue(BindDatas[0], temp);
                                    }
                                    else if (field.FieldType == typeof(long))
                                    {
                                        long temp = 0;
                                        if (long.TryParse(valStr, out temp)) field.SetValue(BindDatas[0], temp);
                                    }
                                    else if (field.FieldType == typeof(ulong))
                                    {
                                        ulong temp = 0;
                                        if (ulong.TryParse(valStr, out temp)) field.SetValue(BindDatas[0], temp);
                                    }
                                    else if (field.FieldType == typeof(float))
                                    {
                                        float temp = 0;
                                        if (float.TryParse(valStr, out temp)) field.SetValue(BindDatas[0], temp);
                                    }
                                    else if (field.FieldType == typeof(string))
                                    {
                                        field.SetValue(BindDatas[0], valStr);
                                    }
                                }
                                
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError(ex.ToString());
                                EditorUtility.DisplayDialog("提示", "请将报错信息截图发给程序排查!!", "好的");
                            }
                            break;
                        }
                    }
                }
  
            }
            Close();
        }
    }
}
#endif