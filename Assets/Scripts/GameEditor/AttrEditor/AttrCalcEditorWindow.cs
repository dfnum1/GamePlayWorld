using Framework.Core;
using Framework.Data.ED;
using Framework.ED;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
//using TopGame.Data;
using UnityEditor;
using UnityEngine;

namespace TopGame.ED
{
    public class AttrCalcEditorWindow : EditorWindow
    {
        private List<string> m_vAttris = new List<string>();
        private List<uint> m_vAttriTypes = new List<uint>();
        private List<string> m_vOps = new List<string> { "+", "-", "*", "/", "(", ")" };
        private List<string> m_vFuncs = new List<string> { "Mathf.Max", "Mathf.Min", "Mathf.Floor", "Mathf.Ceil", "Mathf.Round", "Mathf.Abs", "UnityEngine.Random.Range" };

        ExprSegmentType m_nAddSegmentType = ExprSegmentType.Text;
        private enum ExprSegmentType { Text, Attr, Func, Op, Pop }
        [System.Serializable]
        class ExprSegment
        {
            public ExprSegmentType Type;
            public string Text; // 普通文本或属性名
            public string lamda; // 仅函数片段有效
            public EAttackGroup Group; // 仅属性片段有效
        }
        [System.Serializable]
        class AttrFormula
        {
            public string formulaName = "NewFormula";
            public string formulaFunction = "";
            public int formulaType = 0;
            public uint applayAttrType = 0;
            public List<ExprSegment> epxrs = new List<ExprSegment>();
        }

        [System.Serializable]
        class AttrFormulaData
        {
            public string codeSaveDirory = "Assets/Scripts/GameMain/Generated/Attributes";
            public List<AttrFormula> formulas = new List<AttrFormula>();
        }
        enum EAttackGroup
        {
            Attacker = 0,
            Target = 1,
        }
        public string[] m_arrAttackGroups = new string[] { "攻击方", "防守方" };

        [MenuItem("Tools/属性计算编辑器")]
        public static void ShowWindow()
        {
            GetWindow<AttrCalcEditorWindow>("属性计算编辑器");
        }

        private Vector2 m_scrollFormulaList = Vector2.zero;
        private Vector2 m_scrollAttrList = Vector2.zero;

        private AttrFormula m_CurrentFormula;

        private static GUIStyle s_attackerStyle;
        private static GUIStyle s_targetStyle;

        AttrFormulaData m_Data = new AttrFormulaData();
        //------------------------------------------------------
        private void OnEnable()
        {

          //  CsvData_AttributeType attrTable = DataEditorUtil.GetTable<CsvData_AttributeType>(true);
          //  if(attrTable!=null)
          //  {
         //       foreach (var db in attrTable.datas)
         //       {
         //           m_vAttris.Add(db.Value.attrName);
         //           m_vAttriTypes.Add(db.Key);
         //       }
          //  }

            LoadAttrFormula();
        }
        //------------------------------------------------------
        private void OnDisable()
        {
       //     SaveAttrFormula();
        }
        //------------------------------------------------------
        void LoadAttrFormula()
        {
            string file = Application.dataPath + "/../EditorData/AttrFormula.json";
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                m_Data = JsonUtility.FromJson<AttrFormulaData>(json);
            }
        }
        //------------------------------------------------------
        void SaveAttrFormula()
        {
            string file = Application.dataPath + "/../EditorData/AttrFormula.json";
            if (!Directory.Exists(Path.GetDirectoryName(file)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
            }
            var json = JsonUtility.ToJson(m_Data, true);
            File.WriteAllText(file, json);
        }

        //------------------------------------------------------
        private void OnGUI()
        {
            // 初始化样式（只初始化一次）
            if (s_attackerStyle == null)
            {
                s_attackerStyle = new GUIStyle(GUI.skin.button);
                s_attackerStyle.normal.textColor = Color.red;
                s_attackerStyle.focused.textColor = Color.red;
                s_attackerStyle.active.textColor = Color.red;
                s_attackerStyle.hover.textColor = Color.red;

                s_targetStyle = new GUIStyle(GUI.skin.button);
                s_targetStyle.normal.textColor = Color.green;
                s_targetStyle.focused.textColor = Color.green;
                s_targetStyle.active.textColor = Color.green;
                s_targetStyle.hover.textColor = Color.green;
            }

            EditorGUILayout.BeginHorizontal();

            // 左侧区域
            EditorGUILayout.BeginVertical(GUILayout.Width(300));

            // 上半部分：表达式列表（带滚动）
            float leftPanelHeight = position.height;
            float halfHeight = leftPanelHeight-100;

            EditorGUILayout.BeginVertical("box", GUILayout.Height(halfHeight));
            EditorGUILayout.LabelField("代码导出目录：");
            m_Data.codeSaveDirory = EditorGUILayout.TextField(m_Data.codeSaveDirory);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("表达式列表：");

            m_scrollFormulaList = EditorGUILayout.BeginScrollView(m_scrollFormulaList, GUILayout.Height(halfHeight - 100));
            for (int i = 0; i < m_Data.formulas.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(m_Data.formulas[i].formulaName))
                {
                    m_CurrentFormula = m_Data.formulas[i];
                }
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("确认删除", $"确认删除表达式 {m_Data.formulas[i].formulaName} 吗？", "是", "否"))
                    {
                        if (m_CurrentFormula == m_Data.formulas[i])
                            m_CurrentFormula = null;
                        m_Data.formulas.RemoveAt(i);
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("新增表达式"))
            {
                var newFormula = new AttrFormula { formulaName = "NewFormula"};
                m_Data.formulas.Add(newFormula);
                m_CurrentFormula = newFormula;
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            if(GUILayout.Button("生成代码"))
            {
                GenerateAllCode();
            }
            if (GUILayout.Button("保存"))
            {
                SaveAttrFormula();
            }
            EditorGUILayout.EndVertical();

            // 右侧区域
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("表达式详情：");
            if (m_CurrentFormula!=null)
            {
                m_CurrentFormula.formulaName = EditorGUILayout.TextField("表达式名称：", m_CurrentFormula.formulaName);
                m_CurrentFormula.formulaFunction = EditorGUILayout.TextField("函数名称：", m_CurrentFormula.formulaFunction);
                m_CurrentFormula.formulaType = EditorGUILayout.IntField("表达式类型：", m_CurrentFormula.formulaType);
                {
                    int currentIndex = Mathf.Max(0, m_vAttriTypes.IndexOf(m_CurrentFormula.applayAttrType));
                    int newIndex = EditorGUILayout.Popup("应用属性类型:",currentIndex, m_vAttris.ToArray());
                    if (newIndex != currentIndex && newIndex >= 0 && newIndex < m_vAttris.Count)
                    {
                        m_CurrentFormula.applayAttrType = m_vAttriTypes[newIndex];
                    }
                }
                for (int i =0; i < m_CurrentFormula.epxrs.Count; ++i)
                {
                    var seg = m_CurrentFormula.epxrs[i];
                    EditorGUILayout.BeginHorizontal();
                    seg.Type = (ExprSegmentType)EditorGUILayout.EnumPopup(seg.Type, GUILayout.Width(100));
                    if (seg.Type == ExprSegmentType.Attr)
                    {
                        int currentIndex = Mathf.Max(0, m_vAttris.IndexOf(seg.Text));
                        int newIndex = EditorGUILayout.Popup(currentIndex, m_vAttris.ToArray(), GUILayout.Width(200));
                        if (newIndex != currentIndex && newIndex >= 0 && newIndex < m_vAttris.Count)
                        {
                            seg.Text = m_vAttris[newIndex];
                        }
                        seg.Group = (EAttackGroup)EditorGUILayout.Popup((int)seg.Group, m_arrAttackGroups, GUILayout.Width(300));
                    }
                    else if (seg.Type == ExprSegmentType.Func)
                    {
                        int currentIndex = Mathf.Max(0, m_vFuncs.IndexOf(seg.Text));
                        int newIndex = EditorGUILayout.Popup(currentIndex, m_vFuncs.ToArray(), GUILayout.Width(200));
                        if (newIndex != currentIndex && newIndex >= 0 && newIndex < m_vFuncs.Count)
                        {
                            seg.Text = m_vFuncs[newIndex];
                        }
                        seg.lamda = EditorGUILayout.TextField(seg.lamda, GUILayout.Width(300));
                    }
                    else if (seg.Type == ExprSegmentType.Op)
                    {
                        int currentIndex = Mathf.Max(0, m_vOps.IndexOf(seg.Text));
                        int newIndex = EditorGUILayout.Popup(currentIndex, m_vOps.ToArray(), GUILayout.Width(500));
                        if (newIndex != currentIndex && newIndex >= 0 && newIndex < m_vOps.Count)
                        {
                            seg.Text = m_vOps[newIndex];
                        }
                    }
                    else if (seg.Type == ExprSegmentType.Pop)
                    {
                        EditorGUILayout.LabelField(";");
                    }
                    else
                        seg.Text = EditorGUILayout.TextField(seg.Text, GUILayout.Width(500));

                    if (GUILayout.Button("↑", GUILayout.Width(30)) && i > 0)
                    {
                        var temp = m_CurrentFormula.epxrs[i - 1];
                        m_CurrentFormula.epxrs[i - 1] = m_CurrentFormula.epxrs[i];
                        m_CurrentFormula.epxrs[i] = temp;
                    }
                    if (GUILayout.Button("↓", GUILayout.Width(30)) && i < m_CurrentFormula.epxrs.Count - 1)
                    {
                        var temp = m_CurrentFormula.epxrs[i + 1];
                        m_CurrentFormula.epxrs[i + 1] = m_CurrentFormula.epxrs[i];
                        m_CurrentFormula.epxrs[i] = temp;
                    }
                    if (GUILayout.Button("X", GUILayout.Width(30)))
                    {
                        m_CurrentFormula.epxrs.RemoveAt(i);
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.Space(50);
                GUILayout.BeginHorizontal();
                m_nAddSegmentType = (ExprSegmentType)EditorGUILayout.EnumPopup(m_nAddSegmentType, GUILayout.Width(150));
                if (GUILayout.Button("添加"))
                {
                    var newSeg = new ExprSegment { Type = m_nAddSegmentType, Text = "" };
                    if (m_nAddSegmentType == ExprSegmentType.Attr && m_vAttris.Count > 0)
                    {
                        newSeg.Text = m_vAttris[0];
                        newSeg.Group = EAttackGroup.Attacker;
                    }
                    else if (m_nAddSegmentType == ExprSegmentType.Func && m_vFuncs.Count > 0)
                    {
                        newSeg.Text = m_vFuncs[0];
                    }
                    else if (m_nAddSegmentType == ExprSegmentType.Op && m_vOps.Count > 0)
                    {
                        newSeg.Text = m_vOps[0];
                    }
                    m_CurrentFormula.epxrs.Add(newSeg);
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("请选择左侧表达式以查看详情。", MessageType.Info);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
        //------------------------------------------------------
        private void GenerateAllCode()
        {
            List<string> vCodes= new List<string>();
            System.Text.StringBuilder mappCode = new System.Text.StringBuilder();
            System.Text.StringBuilder applyFormulaTypeCode = new System.Text.StringBuilder();

            applyFormulaTypeCode.AppendLine("\t\tpublic static int GetFormulaApplayAttr(int formulaType)");
            applyFormulaTypeCode.AppendLine("\t\t{");
            applyFormulaTypeCode.AppendLine("\t\t\tswitch(formulaType)");
            applyFormulaTypeCode.AppendLine("\t\t\t{");

            mappCode.AppendLine("\t\tpublic static FFloat CalcAttrByType(int formulaType, Actor attacker, Actor target, out byte applayAttrType)");
            mappCode.AppendLine("\t\t{");
            mappCode.AppendLine("\t\t\tapplayAttrType=byte.MaxValue;");
            mappCode.AppendLine("\t\t\tswitch(formulaType)");
            mappCode.AppendLine("\t\t\t{");
            foreach (var db in m_Data.formulas)
            {
                var formula = db;
                // 生成函数名
                string funcName = string.IsNullOrWhiteSpace(formula.formulaFunction)
                    ? $"Calc_{formula.formulaName}"
                    : formula.formulaFunction;

                // 生成表达式字符串
                System.Text.StringBuilder exprBuilder = new System.Text.StringBuilder();
                System.Text.StringBuilder comments = new System.Text.StringBuilder();
                exprBuilder.Append("\t\t\t");
                foreach (var seg in formula.epxrs)
                {
                    switch (seg.Type)
                    {
                        case ExprSegmentType.Text:
                            exprBuilder.Append(seg.Text);
                            break;
                        case ExprSegmentType.Attr:
                            // 属性访问方式可根据实际项目调整
                            comments.AppendLine($"\t\t\t//{m_vAttriTypes[m_vAttris.IndexOf(seg.Text)]} 为 {seg.Text}");
                            string groupPrefix = seg.Group == EAttackGroup.Attacker ? "attacker" : "target";
                            exprBuilder.Append($"{groupPrefix}.GetAttr({m_vAttriTypes[m_vAttris.IndexOf(seg.Text)]})");
                            break;
                        case ExprSegmentType.Func:
                            // 函数调用，lamda为参数
                            exprBuilder.Append($"{seg.Text}{seg.lamda}");
                            break;
                        case ExprSegmentType.Op:
                            exprBuilder.Append(seg.Text);
                            break;
                        case ExprSegmentType.Pop:
                            exprBuilder.AppendLine(";");
                            exprBuilder.Append("\t\t\t");
                            break;
                    }
                }
                // 生成完整C#代码
                string code =
            $@"         //自动生成的属性计算代码:{formula.formulaName}
        public static FFloat {funcName}(Actor attacker, Actor target)
        {{
{comments.ToString()}
{exprBuilder.ToString()}
        }}";
                vCodes.Add(code);

                mappCode.AppendLine($"\t\t\t\tcase {formula.formulaType}: applayAttrType={formula.applayAttrType}; return {funcName}(attacker, target);");
                applyFormulaTypeCode.Append($"\t\t\t\tcase {formula.formulaType}: return {formula.applayAttrType};");
                applyFormulaTypeCode.AppendLine("//" + formula.formulaName + " 最终作用属性:" + formula.applayAttrType + "[" + m_vAttris[(int)formula.applayAttrType] + "]");
            }

            applyFormulaTypeCode.AppendLine("\t\t\t\tdefault: return -1;");
            applyFormulaTypeCode.AppendLine("\t\t\t}");
            applyFormulaTypeCode.AppendLine("\t\t}");


            mappCode.AppendLine("\t\t\t\tdefault:");
            mappCode.AppendLine("\t\t\t\t{");
            mappCode.AppendLine("\t\t\t\t\tDebug.LogWarning(\"AttrFormulaHelper::CalcAttrByType unknown formulaType \"+formulaType);");
            mappCode.AppendLine("\t\t\t\t\treturn 0;");
            mappCode.AppendLine("\t\t\t\t}");
            mappCode.AppendLine("\t\t\t}");
            mappCode.AppendLine("\t\t}");

            // 保存到指定目录
            string dir = m_Data.codeSaveDirory;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string filePath = Path.Combine(dir, $"AttrFormulaHelper.cs");

            System.Text.StringBuilder classCode = new System.Text.StringBuilder();
            classCode.AppendLine("//auto generator");
            classCode.AppendLine("#if USE_ACTORSYSTEM");
            classCode.AppendLine("using UnityEngine;");
            classCode.AppendLine("#if USE_FIXEDMATH");
            classCode.AppendLine("using ExternEngine;");
            classCode.AppendLine("#else");
            classCode.AppendLine("using FFloat = System.Single;");
            classCode.AppendLine("#endif");
            classCode.AppendLine("namespace Framework.Core");
            classCode.AppendLine("{");
            classCode.AppendLine("    public partial class AttrFormulaHelper");
            classCode.AppendLine("    {");
            foreach(var code in vCodes)
                classCode.AppendLine(code);

            classCode.AppendLine("\t\t//--------------------------------------------------------");
            classCode.Append(mappCode);

            classCode.AppendLine("\t\t//--------------------------------------------------------");
            classCode.Append(applyFormulaTypeCode);

            classCode.AppendLine("    }");
            classCode.AppendLine("}");
            classCode.AppendLine("#endif");
            File.WriteAllText(filePath, classCode.ToString());

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("生成成功", $"代码已生成到:\n{filePath}", "确定");
        }
    }
}