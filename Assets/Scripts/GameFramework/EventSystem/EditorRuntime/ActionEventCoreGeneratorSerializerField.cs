#if UNITY_EDITOR
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ActionEventCoreGeneratorSerializerField
作    者:	HappLI
描    述:	事件参数序列化生成器
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;

namespace Framework.Core
{
    public class ActionEventCoreGeneratorSerializerField
    {
        static Dictionary<string, string> TYPE_TO_CPP_TYPE = new Dictionary<string, string>();
        //------------------------------------------------------
        public static bool BuildBinaryFieldCs(ActionEventCoreGenerator.EventType evType, ref string strCopy, ref List<string> vCode, ref string strReadString, ref string strWriteString)
        {
            object instance = Activator.CreateInstance(evType.type);
            for (int j = 0; j < evType.vFields.Count; ++j)
            {
                System.Type fieldType = evType.vFields[j].FieldType;
                string strFielName = evType.vFields[j].Name.Trim();
              //  string strCode = "";
                if (fieldType == typeof(bool))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToBool();\r\n";
                    strWriteString += "\t\t\twriter.WriteBool(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(byte))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToByte();\r\n";
                    strWriteString += "\t\t\twriter.WriteByte(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(short))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToShort();\r\n";
                    strWriteString += "\t\t\twriter.WriteShort(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(ushort))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToUshort();\r\n";
                    strWriteString += "\t\t\twriter.WriteUshort(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(int))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToInt32();\r\n";
                    strWriteString += "\t\t\twriter.WriteInt32(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(uint))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToUint32();\r\n";
                    strWriteString += "\t\t\twriter.WriteUint32(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(float))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToFloat();\r\n";
                    strWriteString += "\t\t\twriter.WriteFloat(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(double))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToDouble();\r\n";
                    strWriteString += "\t\t\twriter.WriteDouble(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(long))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToInt64();\r\n";
                    strWriteString += "\t\t\twriter.WriteInt64(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(ulong))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToUint64();\r\n";
                    strWriteString += "\t\t\twriter.WriteUint64(" + strFielName + ");\r\n";
                }
                else if (fieldType.IsEnum)
                {
                    string enumTypeName = fieldType.FullName.Replace("+", ".");

                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = (" + enumTypeName + ")reader.ToShort();\r\n";
                    strWriteString += "\t\t\twriter.WriteShort((short)" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(string))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToString();\r\n";
                    strWriteString += "\t\t\twriter.WriteString(" + strFielName + ");\r\n";
                }
                else if (fieldType.Name.ToLower().CompareTo("char[]") == 0)
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToString();\r\n";
                    strWriteString += "\t\t\twriter.WriteString(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Vector2))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToVec2();\r\n";
                    strWriteString += "\t\t\twriter.WriteVector2(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Vector2Int))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToVec2Int();\r\n";
                    strWriteString += "\t\t\twriter.WriteVector2Int(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Vector3))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToVec3();\r\n";
                    strWriteString += "\t\t\twriter.WriteVector3(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Vector3Int))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToVec3Int();\r\n";
                    strWriteString += "\t\t\twriter.WriteVector3Int(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Vector4))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToVec4();\r\n";
                    strWriteString += "\t\t\twriter.WriteVector4(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Color))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToColor();\r\n";
                    strWriteString += "\t\t\twriter.WriteColor(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Quaternion))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToQuaternion();\r\n";
                    strWriteString += "\t\t\twriter.WriteQuaternion(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Data.CurveData))
                {
                    strCopy += "\t\t\t" + strFielName + ".Copy(oth." + strFielName + ");\r\n";
                    strReadString += "\t\t\t" + strFielName + ".Read(ref reader);\r\n";
                    strWriteString += "\t\t\t" + strFielName + ".Write(ref writer);\r\n";
                }
                else if (fieldType == typeof(Framework.Data.KeyValueParam))
                {
                    strCopy += "\t\t\t" + strFielName + ".Copy(oth." + strFielName + ");\r\n";
                    strReadString += "\t\t\t" + strFielName + ".Read(ref reader);\r\n";
                    strWriteString += "\t\t\t" + strFielName + ".Write(ref writer);\r\n";
                }
                else if (fieldType == typeof(AnimationCurve))
                {
                    strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";
                    strReadString += "\t\t\t" + strFielName + " = reader.ToCurve();\r\n";
                    strWriteString += "\t\t\twriter.WriteCurve(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(PhysicPropertyData))
                {
                    strCopy += "\t\t\t" + strFielName + ".Copy(oth." + strFielName + ");\r\n";
                    strReadString += "\t\t\t" + strFielName + ".Read(ref reader);\r\n";
                    strWriteString += "\t\t\t" + strFielName + ".Write(ref writer);\r\n";
                }
                else if (fieldType == typeof(SplineData))
                {
                  //  string include = "#include \"Data/Other/SplineData.h\"\r\n";
                    strCopy += "\t\t\t" + strFielName + ".Copy(oth." + strFielName + ");\r\n";
                    strReadString += "\t\t\t" + strFielName + ".Read(ref reader);\r\n";
                    strWriteString += "\t\t\t" + strFielName + ".Write(ref writer);\r\n";
                }
                else if (fieldType.IsArray || fieldType.IsGenericType)
                {
                    bool bOk = false;
                    Type decType = null;
                    if (ActionEventCoreGeneratorSerializerArray.bArray(fieldType, out decType))
                    {
                        string decName;
                        ParseGenericDecType(null, decType, out decName);
                        decName = decType.FullName.Replace("+", ".");
                        strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";

                        bOk = ActionEventCoreGeneratorSerializerArray.BuildBinaryArrayCs(evType.vFields[j], decType, decName, ref strReadString, ref strWriteString);
                    }
                    else if (ActionEventCoreGeneratorSerializerList.bList(fieldType, out decType))
                    {
                        string decName;
                        ParseGenericDecType(null, decType, out decName);
                        decName = decType.FullName.Replace("+", ".");
                        strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";

                        bOk = ActionEventCoreGeneratorSerializerList.BuildBinaryListCs(evType.vFields[j], decType, decName, ref strReadString, ref strWriteString);
                    }
                    else if (ActionEventCoreGeneratorSerializerHashSet.bHashSet(fieldType, out decType))
                    {
                        string decName;
                        ParseGenericDecType(null, decType, out decName);
                        decName = decType.FullName.Replace("+", ".");
                        strCopy += "\t\t\t" + strFielName + " = oth." + strFielName + ";\r\n";

                        bOk = ActionEventCoreGeneratorSerializerHashSet.BuildBinaryHashSetCs(evType.vFields[j], decType, decName, ref strReadString, ref strWriteString);
                    }
                    if (!bOk)
                    {
                        Debug.LogError(evType.type + " 类成员：" + evType.vFields[j].Name + " 不支持序列化");
                        return false;
                    }
                }
                else
                {
                    Debug.LogError(evType.type + " 类成员：" + evType.vFields[j].Name + " 不支持序列化");
                    return false;
                }
            }
            return true;
        }
        //------------------------------------------------------
        public static bool BuildFieldCpp(ActionEventCoreGenerator.EventType evType, ref string strConstruction, ref string strDestruction, ref string strCopy, ref List<string> vCode, ref string strReadString, ref string strWriteString)
        {
            if(TYPE_TO_CPP_TYPE.Count<=0)
            {
                TYPE_TO_CPP_TYPE.Add("EActorType", "tByte");
                TYPE_TO_CPP_TYPE.Add("EObjectType", "EObstacleType");
            }

            object instance = Activator.CreateInstance(evType.type);
            for (int j = 0; j < evType.vFields.Count; ++j)
            {
                System.Type fieldType = evType.vFields[j].FieldType;
                string strFielName = evType.vFields[j].Name.Trim();
                string strCode = "";
                if (fieldType == typeof(bool))
                {
                    strConstruction += "\t\t" + strFielName + " = "+ evType.vFields[j].GetValue(instance).ToString().ToLower() + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\tbool " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = seralizer->readBool();\r\n";
                    strWriteString += "\t\tseralizer->writeBool(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(byte))
                {
                    short x = (short)(byte)evType.vFields[j].GetValue(instance);
                    if (x < 0)
                        strConstruction += "\t\t" + strFielName + " = 0xff;\r\n";
                    else
                        strConstruction += "\t\t" + strFielName + " = " + x + ";\r\n";

                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttByte " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = seralizer->readUChar();\r\n";
                    strWriteString += "\t\tseralizer->writeUChar(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(short))
                {
                    strConstruction += "\t\t" + strFielName + " = " + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttInt16 " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = seralizer->readShort();\r\n";
                    strWriteString += "\t\tseralizer->writeShort(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(ushort))
                {
                    short x = (short)(ushort)evType.vFields[j].GetValue(instance);
                    if (x < 0)
                        strConstruction += "\t\t" + strFielName + " = 0xffff;\r\n";
                    else
                        strConstruction += "\t\t" + strFielName + " = " + x + ";\r\n";

                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttUint16 " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = seralizer->readUShort();\r\n";
                    strWriteString += "\t\tseralizer->writeUShort(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(int))
                {
                    strConstruction += "\t\t" + strFielName + " = " + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttInt32 " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = seralizer->readInt();\r\n";
                    strWriteString += "\t\tseralizer->writeInt(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(uint))
                {
                    int x = (int)(uint)evType.vFields[j].GetValue(instance);
                    if(x <0)
                        strConstruction += "\t\t" + strFielName + " = 0xffffffff;\r\n";
                    else
                        strConstruction += "\t\t" + strFielName + " = " + x+";\r\n";

                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttUint32 " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = seralizer->readUInt();\r\n";
                    strWriteString += "\t\tseralizer->writeUInt(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(float))
                {
                    strConstruction += "\t\t" + strFielName + " = " + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttFloat " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = seralizer->readFloat();\r\n";
                    strWriteString += "\t\tseralizer->writeFloat(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(double))
                {
                    strConstruction += "\t\t" + strFielName + " = " + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\tdouble " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = seralizer->readDouble();\r\n";
                    strWriteString += "\t\tseralizer->writeDouble(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(long))
                {
                    strConstruction += "\t\t" + strFielName + " = " + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttInt64 " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = seralizer->readLong();\r\n";
                    strWriteString += "\t\tseralizer->writeLong(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(ulong))
                {
                    long x = (long)(ulong)evType.vFields[j].GetValue(instance);
                    if (x < 0)
                        strConstruction += "\t\t" + strFielName + " = 0xffffffffffffffff;\r\n";
                    else
                        strConstruction += "\t\t" + strFielName + " = " + x + ";\r\n";

                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttUint64 " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = seralizer->readULong();\r\n";
                    strWriteString += "\t\tseralizer->writeULong(" + strFielName + ");\r\n";
                }
                else if (fieldType.IsEnum)
                {
                    if(fieldType.FullName.Contains(evType.Name+"+"))
                    {
                        string strEnumCode = "";
                        strEnumCode += "\t\tenum " + fieldType.Name + "// : tByte\r\n";
                        strEnumCode += "\t\t{\r\n";
                        foreach (Enum v in Enum.GetValues(fieldType))
                        {
                            string strTemName = v.ToString();
                            FieldInfo fi = fieldType.GetField(v.ToString());
                            strEnumCode += "\t\t\t" + fieldType.Name + "_" + strTemName + ",";
                            if (fi != null && fi.IsDefined(typeof(Base.PluginDisplayAttribute)))
                            {
                                strEnumCode += "//" + fi.GetCustomAttribute<Base.PluginDisplayAttribute>().displayName;
                            }
                            strEnumCode += "\r\n";
                        }             
                        strEnumCode += "\t\t};\r\n";
                        vCode.Insert(16, strEnumCode);
                    }
                    string enumTypeName = fieldType.FullName.Replace(fieldType.Namespace, "").Replace("+", "::").Replace(".","");
                    string mappinType = enumTypeName;
                    if (TYPE_TO_CPP_TYPE.TryGetValue(enumTypeName, out mappinType))
                        enumTypeName = mappinType;

                    int vla = Convert.ToInt32((Enum)evType.vFields[j].GetValue(instance));
                    strConstruction += "\t\t" + strFielName + " = (" + enumTypeName + ")" + vla.ToString() + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\t" + enumTypeName + " " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = (" + enumTypeName + ")seralizer->readShort();\r\n";
                    strWriteString += "\t\tseralizer->writeShort((tInt16)" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(string))
                {
                    object value = evType.vFields[j].GetValue(instance);
                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                        strConstruction += "\t\t" + strFielName + " = \"\";\r\n";
                    else
                        strConstruction += "\t\t" + strFielName + " = \"" + evType.vFields[j].GetValue(instance) + "\";\r\n";

                    strCopy += "\t\t" + strFielName + " = " + strFielName + ";\r\n";
                    strCode += "\t\tstring " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + " = seralizer->readString();\r\n";
                    strWriteString += "\t\tseralizer->writeString(" + strFielName + ".c_str());\r\n";
                }
                else if (fieldType.Name.ToLower().CompareTo("char[]") == 0)
                {
                    object value = evType.vFields[j].GetValue(instance);
                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                        strConstruction += "\t\t" + strFielName + " = \"\";\r\n";
                    else
                        strConstruction += "\t\t" + strFielName + " = \"" + evType.vFields[j].GetValue(instance) + "\";\r\n";

                    strCopy += "\t\t" + strFielName + " = " + strFielName + ";\r\n";
                    strCode += "\t\tstring " + strFielName + ";";
                    strReadString += "\t\t"+ strFielName + " = seralizer->readString();\r\n";
                    strWriteString += "\t\tseralizer->writeString(" + strFielName + ".c_str());\r\n";
                }
                else if (fieldType == typeof(Vector2))
                {
                    strConstruction += "\t\t" + strFielName + " = tVector2" + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttVector2 " + strFielName + ";";
                    strReadString += "\t\tseralizer->readVector2(" + strFielName + ");\r\n";
                    strWriteString += "\t\tseralizer->writerVector2(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Vector2Int))
                {
                    strConstruction += "\t\t" + strFielName + " = IVector2" + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\tIVector2 " + strFielName + ";";
                    strReadString += "\t\tseralizer->readIVector2(" + strFielName + ");\r\n";
                    strWriteString += "\t\tseralizer->writerIVector2(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Vector3))
                {
                    strConstruction += "\t\t" + strFielName + " = tVector3" + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttVector3 " + strFielName + ";";
                    strReadString += "\t\tseralizer->readVector3(" + strFielName + ");\r\n";
                    strWriteString += "\t\tseralizer->writerVector3(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Vector3Int))
                {
                    strConstruction += "\t\t" + strFielName + " = IVector3" + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\tIVector3 " + strFielName + ";";
                    strReadString += "\t\tseralizer->readIVector3(" + strFielName + ");\r\n";
                    strWriteString += "\t\tseralizer->writerIVector3(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Vector4))
                {
                    strConstruction += "\t\t" + strFielName + " = tVector4" + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttVector4 " + strFielName + ";";
                    strReadString += "\t\tseralizer->readVector4(" + strFielName + ");\r\n";
                    strWriteString += "\t\tseralizer->writerVector4(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Vector4))
                {
                    strConstruction += "\t\t" + strFielName + " = tVector4" + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttVector4 " + strFielName + ";";
                    strReadString += "\t\tseralizer->readVector4(" + strFielName + ");\r\n";
                    strWriteString += "\t\tseralizer->writerVector4(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Quaternion))
                {
                    strConstruction += "\t\t" + strFielName + " = tQuaternion" + evType.vFields[j].GetValue(instance) + ";\r\n";
                    strCopy += "\t\t" + strFielName + " = oth->" + strFielName + ";\r\n";
                    strCode += "\t\ttQuaternion " + strFielName + ";";
                    strReadString += "\t\tseralizer->readQuaternion(" + strFielName + ");\r\n";
                    strWriteString += "\t\tseralizer->writerQuaternion(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Data.CurveData))
                {
                    string include = "#include \"Data/Other/CurveData.h\"\r\n";
                    if(!vCode.Contains(include)) vCode.Insert(11, include);
                    strCopy += "\t\t" + strFielName + ".Copy(&oth->" + strFielName + ");\r\n";
                    strCode += "\t\tCameraSplineData " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + ".Read(seralizer);\r\n";
                    strWriteString += "\t\t" + strFielName + ".Write(seralizer);\r\n";
                }
                else if (fieldType == typeof(Framework.Data.KeyValueParam))
                {
                    string include = "#include \"Data/Other/KeyValueParam.h\"\r\n";
                    if (!vCode.Contains(include)) vCode.Insert(11, include);
                    strCopy += "\t\t" + strFielName + ".Copy(&oth->" + strFielName + ");\r\n";
                    strCode += "\t\tKeyValueParam " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + ".Read(seralizer);\r\n";
                    strWriteString += "\t\t" + strFielName + ".Write(seralizer);\r\n";
                }
                else if (fieldType == typeof(AnimationCurve))
                {
                    string include = "#include \"Base/Math/AnimationCurve.h\"\r\n";
                    if (!vCode.Contains(include)) vCode.Insert(11, include);
                    strCopy += "\t\t" + strFielName + ".Copy(&oth->" + strFielName + ");\r\n";
                    strCode += "\t\tAnimationCurve " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + ".Read(seralizer);\r\n";
                    strWriteString += "\t\t" + strFielName + ".Write(seralizer);\r\n";
                }
                else if (fieldType == typeof(PhysicPropertyData))
                {
                    string include = "#include \"Core/Actor/PhysicPropertyData.h\"\r\n";
                    if (!vCode.Contains(include)) vCode.Insert(11, include);
                    strCopy += "\t\t" + strFielName + ".Copy(&oth->" + strFielName + ");\r\n";
                    strCode += "\t\tSPhysicPropertyData " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + ".Read(seralizer);\r\n";
                    strWriteString += "\t\t" + strFielName + ".Write(seralizer);\r\n";
                }
                else if (fieldType == typeof(SplineData))
                {
                    string include = "#include \"Data/Other/SplineData.h\"\r\n";
                    if (!vCode.Contains(include)) vCode.Insert(11, include);
                    strCopy += "\t\t" + strFielName + ".Copy(&oth->" + strFielName + ");\r\n";
                    strCode += "\t\tSplineData " + strFielName + ";";
                    strReadString += "\t\t" + strFielName + ".Read(seralizer);\r\n";
                    strWriteString += "\t\t" + strFielName + ".Write(seralizer);\r\n";
                }
                else if (fieldType.IsArray || fieldType.IsGenericType)
                {
                    Type decType = null;
                    bool bOk = false;
                    if (ActionEventCoreGeneratorSerializerArray.bArray(fieldType, out decType))
                    {
                        string decName;
                        ParseGenericDecType(vCode, decType, out decName);
                        strCode += "\t\tArray<" + decName + "> " + strFielName + ";";
                        strCopy += "\t\t{\r\n";
                        strCopy += "\t\t\t" + strFielName + ".resize(oth->" + strFielName + ".size());\r\n";
                        strCopy += "\t\t\tmemcpy(" + strFielName + ".get(),oth->"+ strFielName + ".get(),sizeof("+ decName + ")*" + strFielName + ".size());\r\n";
                        strCopy += "\t\t}\r\n";

                        strDestruction += "\t\t" + strFielName + ".clear();\r\n";
                        bOk = ActionEventCoreGeneratorSerializerArray.BuildArrayCpp(evType.vFields[j], decType, decName, ref strReadString, ref strWriteString);
                    }
                    else if (ActionEventCoreGeneratorSerializerList.bList(fieldType, out decType))
                    {
                        string decName;
                        ParseGenericDecType(vCode, decType, out decName);
                        strCode += "\t\tArray<" + decName + "> " + strFielName + ";";
                        strCopy += "\t\t{\r\n";
                        strCopy += "\t\t\t" + strFielName + ".resize(oth->" + strFielName + ".size());\r\n";
                        strCopy += "\t\t\tmemcpy(" + strFielName + ".get(),oth->" + strFielName + ".get(),sizeof(" + decName + ")*" + strFielName + ".size());\r\n";
                        strCopy += "\t\t}\r\n";

                        strDestruction += "\t\t" + strFielName + ".clear();\r\n";
                        bOk = ActionEventCoreGeneratorSerializerList.BuildListCpp(evType.vFields[j], decType, decName, ref strReadString, ref strWriteString);
                    }
                    else if (ActionEventCoreGeneratorSerializerHashSet.bHashSet(fieldType, out decType))
                    {
                        string decName;
                        ParseGenericDecType(vCode, decType, out decName);
                        strCode += "\t\tArray<" + decName + "> " + strFielName + ";";
                        strCopy += "\t\t{\r\n";
                        strCopy += "\t\t\t" + strFielName + ".resize(oth->" + strFielName + ".size());\r\n";
                        strCopy += "\t\t\tmemcpy(" + strFielName + ".get(),oth->" + strFielName + ".get(),sizeof(" + decName + ")*" + strFielName + ".size());\r\n";
                        strCopy += "\t\t}\r\n";

                        strDestruction += "\t\t" + strFielName + ".clear();\r\n";
                        bOk = ActionEventCoreGeneratorSerializerHashSet.BuildHashSetCpp(evType.vFields[j], decType, decName, ref strReadString, ref strWriteString);
                    }
                    if (!bOk)
                    {
                        Debug.LogError(evType.type + " 类成员：" + evType.vFields[j].Name + " 不支持序列化");
                        return false;
                    }
                }
                else
                {
                    Debug.LogError(evType.type + " 类成员：" + evType.vFields[j].Name + " 不支持序列化");
                    return false;
                }

                Base.PluginDisplayAttribute displayName = fieldType.GetCustomAttribute<Base.PluginDisplayAttribute>();
                if (displayName != null && !string.IsNullOrEmpty(displayName.displayName))
                    strCode +="\t//"+ displayName.displayName;
                strCode += "\r\n";
                vCode.Add(strCode);
            }
            return true;
        }
        //------------------------------------------------------
        static void ParseGenericDecType(List<string> vCode, System.Type decType, out string decName)
        {
            decName = decType.FullName.Replace(decType.Namespace, "").Replace("+", "").Replace(".", "");
            if (decType == typeof(bool)) decName = "bool";
            else if (decType == typeof(short)) decName = "tInt16";
            else if (decType == typeof(ushort)) decName = "tUint16";
            else if (decType == typeof(int)) decName = "tInt32";
            else if (decType == typeof(uint)) decName = "tUint32";
            else if (decType == typeof(float)) decName = "tFloat";
            else if (decType == typeof(double)) decName = "double";
            else if (decType == typeof(long)) decName = "tInt64";
            else if (decType == typeof(ulong)) decName = "tUint64";
            else if (decType.IsEnum) decName = decType.FullName.Replace(decType.Namespace, "").Replace("+", "::").Replace(".", "");
            else if (decType == typeof(string)) decName = "string";
            else if (decType.Name.ToLower().CompareTo("char[]") == 0) decName = "string";
            else if (decType == typeof(Vector2)) decName = "tVector2";
            else if (decType == typeof(Vector2Int)) decName = "IVector2";
            else if (decType == typeof(Vector3)) decName = "tVector3";
            else if (decType == typeof(Vector3Int)) decName = "IVector3";
            else if (decType == typeof(Vector4)) decName = "tVector4";
            else if (decType == typeof(Quaternion)) decName = "tQuaternion";
            else if (decType == typeof(Data.CurveData)) 
            {
                decName = "CurveData";
                string include = "#include \"Data/Other/CurveData.h\"\r\n";
                if (vCode!=null && !vCode.Contains(include)) vCode.Insert(11, include);
            }
            else if (decType == typeof(Framework.Data.KeyValueParam))
            {
                decName = "KeyValueParam";
                string include = "#include \"Base/Math/KeyValueParam.h\"\r\n";
                if (vCode != null && !vCode.Contains(include)) vCode.Insert(11, include);
            }
            else if (decType == typeof(AnimationCurve))
            {
                decName = "AnimationCurve";
                string include = "#include \"Base/Math/AnimationCurve.h\"\r\n";
                if (vCode != null && !vCode.Contains(include)) vCode.Insert(11, include);
            }
            else if (decType == typeof(PhysicPropertyData))
            {
                decName = "PhysicPropertyData";
                string include = "#include \"Core/Actor/PhysicPropertyData.h\"\r\n";
                if (vCode != null && !vCode.Contains(include)) vCode.Insert(11, include);
            }
            else if (decType == typeof(SplineData))
            {
                decName = "SplineData";
                string include = "#include \"Data/Other/SplineData.h\"\r\n";
                if (vCode != null && !vCode.Contains(include)) vCode.Insert(11, include);
            }
        }
        //------------------------------------------------------
        public static bool BuildFieldSerializer(ActionEventCoreGenerator.EventType Types, string strPreName, ref string strReadString, ref string strWriteString, ref string xmlRead, ref string xmlWrite, ref string binaryRead, ref string binaryWrite, string inpuDataName = null)
        {
            bool isvData = inpuDataName == null;
            for (int j = 0; j < Types.vFields.Count; ++j)
            {
                System.Type fieldType = Types.vFields[j].FieldType;
                string strFielName = strPreName;
                if (string.IsNullOrEmpty(strPreName)) strFielName = Types.vFields[j].Name;
                else strFielName = strPreName + "." + Types.vFields[j].Name;

                if (fieldType == typeof(bool))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0 ) return true;\r\n";
                        strReadString += "\t\t\t" + strFielName + " = vParams[0].CompareTo(\"1\") == 0;\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\t" + strFielName + " = " + inpuDataName + ".CompareTo(\"1\") == 0;\r\n";
                    }

                    strWriteString += "\t\t\tstrParam += (" + strFielName + "?\"1\":\"0\")" + "+\",\"" + ";\r\n";

                    xmlRead += "\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\")) " + strFielName + " = " + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\").CompareTo(\"1\") == 0;\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", " + strFielName + "?\"1\":\"0\");\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=reader.ReadBoolean();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(byte))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0 || !byte.TryParse(vParams[0], out " + strFielName + ")) return true;\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\tif(!byte.TryParse(" + inpuDataName + ", out " + strFielName + ")) return false;\r\n";
                    }

                    strWriteString += "\t\t\tstrParam +=" + strFielName + ".ToString()" + "+\",\"" + ";\r\n";

                    xmlRead += "\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\")) " + strFielName + " = byte.Parse(" + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\"));\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", " + strFielName + ".ToString());\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=reader.ReadByte();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(short))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0 || !short.TryParse(vParams[0], out " + strFielName + ")) return true;\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\tif(!short.TryParse(" + inpuDataName + ", out " + strFielName + ")) return false;\r\n";
                    }
                    strWriteString += "\t\t\tstrParam +=" + strFielName + ".ToString()" + "+\",\"" + ";\r\n";
                    xmlRead += "\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\")) " + strFielName + " = short.Parse(" + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\"));\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", " + strFielName + ".ToString());\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=reader.ReadInt16();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(ushort))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0 || !ushort.TryParse(vParams[0], out " + strFielName + ")) return true;\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\tif(!ushort.TryParse(" + inpuDataName + ", out " + strFielName + ")) return false;\r\n";
                    }
                    strWriteString += "\t\t\tstrParam +=" + strFielName + ".ToString()" + "+\",\"" + ";\r\n";

                    xmlRead += "\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\")) " + strFielName + " = ushort.Parse(" + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\"));\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", " + strFielName + ".ToString());\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=reader.ReadUInt16();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(int))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0 || !int.TryParse(vParams[0], out " + strFielName + ")) return true;\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\tif(!int.TryParse(" + inpuDataName + ", out " + strFielName + ")) return false;\r\n";
                    }
                    strWriteString += "\t\t\tstrParam +=" + strFielName + ".ToString()" + "+\",\"" + ";\r\n";
                    xmlRead += "\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\")) " + strFielName + " = int.Parse(" + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\"));\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", " + strFielName + ".ToString());\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=reader.ReadInt32();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ");\r\n";

                }
                else if (fieldType == typeof(uint))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0 || !uint.TryParse(vParams[0], out " + strFielName + ")) return true;\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\tif(!uint.TryParse(" + inpuDataName + ", out " + strFielName + ")) return false;\r\n";
                    }
                    strWriteString += "\t\t\tstrParam +=" + strFielName + ".ToString()" + "+\",\"" + ";\r\n";
                    xmlRead += "\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\")) " + strFielName + " = uint.Parse(" + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\"));\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", " + strFielName + ".ToString());\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=reader.ReadUInt32();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(float))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ")) return true;\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\tif(!float.TryParse(" + inpuDataName + ", out " + strFielName + ")) return false;\r\n";
                    }

                    strWriteString += "\t\t\tstrParam +=" + strFielName + ".ToString()" + "+\",\"" + ";\r\n";
                    xmlRead += "\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\")) " + strFielName + " = float.Parse(" + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\"));\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", " + strFielName + ".ToString());\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=reader.ReadSingle();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(double))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0 || !double.TryParse(vParams[0], out " + strFielName + ")) return true;\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\tif(!double.TryParse(" + inpuDataName + ", out " + strFielName + ")) return false;\r\n";
                    }
                    strWriteString += "\t\t\tstrParam +=" + strFielName + ".ToString()" + "+\",\"" + ";\r\n";
                    xmlRead += "\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\")) " + strFielName + " = double.Parse(" + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\"));\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", " + strFielName + ".ToString());\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=reader.ReadDouble();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(long))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0 || !long.TryParse(vParams[0], out " + strFielName + ")) return true;\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\tif(!long.TryParse(" + inpuDataName + ", out " + strFielName + ")) return false;\r\n";
                    }
                    strWriteString += "\t\t\tstrParam +=" + strFielName + ".ToString()" + "+\",\"" + ";\r\n";
                    xmlRead += "\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\")) " + strFielName + " = long.Parse(" + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\"));\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", " + strFielName + ".ToString());\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=reader.ReadInt64();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(ulong))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0 || !ulong.TryParse(vParams[0], out " + strFielName + ")) return true;\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\tif(!ulong.TryParse(" + inpuDataName + ", out " + strFielName + ")) return false;\r\n";
                    }
                    strWriteString += "\t\t\tstrParam +=" + strFielName + ".ToString()" + "+\",\"" + ";\r\n";
                    xmlRead += "\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\")) " + strFielName + " = long.Parse(" + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\"));\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", " + strFielName + ".ToString());\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=reader.ReadUInt64();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ");\r\n";
                }
                else if (fieldType.IsEnum)
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                        strReadString += "\t\t\t{\r\n";
                        strReadString += "\t\t\t\tint temp=0;\r\n";
                        strReadString += "\t\t\t\tif(int.TryParse(vParams[0], out temp))" + strFielName + "= (" + fieldType.FullName.Replace("+", ".") + ")temp;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                        strReadString += "\t\t\t}\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\t{\r\n";
                        strReadString += "\t\t\t\tint temp=0;\r\n";
                        strReadString += "\t\t\t\tif(int.TryParse(" + inpuDataName + ", out temp))" + strFielName + "= (" + fieldType.FullName.Replace("+", ".") + ")temp;\r\n";
                        strReadString += "\t\t\t}\r\n";
                    }
                    strWriteString += "\t\t\tstrParam += ((int)" + strFielName + ").ToString()" + "+\",\"" + ";\r\n";

                    xmlRead += "\t\t\t{\r\n";
                    xmlRead += "\t\t\t\tint temp=0;\r\n";
                    xmlRead += "\t\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\") && int.TryParse(" + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\"), out temp)) " + strFielName + " = (" + fieldType.FullName.Replace("+", ".") + ")temp;\r\n";
                    xmlRead += "\t\t\t}\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", ((int)" + strFielName + ").ToString());\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=(" + fieldType.FullName.Replace("+", ".") + ")reader.ReadInt16();\r\n";
                    binaryWrite += "\t\t\twriter.Write((short)" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(string))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                        strReadString += "\t\t\t"+ strFielName + "= vParams[0];\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\t" + strFielName + "= " + inpuDataName + ";\r\n";
                    }
                    strWriteString += "\t\t\tstrParam +=" + strFielName + "+\",\"" + ";\r\n";
                    xmlRead += "\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\")) " + strFielName + " = " + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\");\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", " + strFielName + ".ToString());\r\n";

                    binaryRead += "\t\t\t" + strFielName + "=reader.ReadString();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ");\r\n";
                }
                else if (fieldType == typeof(Vector2))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".x)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".y)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\t{\r\n";
                        strReadString += "\t\t\t\tstring[] split = " + inpuDataName + ".Split(',');\r\n";
                        strReadString += "\t\t\t\tif(split.Length == 2)\r\n";
                        strReadString += "\t\t\t\t{\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".x= float.Parse(split[0]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".y= float.Parse(split[1]);\r\n";
                        strReadString += "\t\t\t\t}\r\n";
                        strReadString += "\t\t\t}\r\n";
                    }
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".x.ToString(\"F3\")" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".y.ToString(\"F3\")" + "+\",\"" + ";\r\n";

                    xmlRead += "\t\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\"))\r\n";
                    xmlRead += "\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\tstring[] split = " + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\").ToString().Split(',');\r\n";
                    xmlRead += "\t\t\t\t\tif(split.Length == 2)\r\n";
                    xmlRead += "\t\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".x= float.Parse(split[0]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".y= float.Parse(split[1]);\r\n";
                    xmlRead += "\t\t\t\t\t}\r\n";
                    xmlRead += "\t\t\t\t}\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", string.Format(\"{0},{1}\"," + strFielName + ".x," + strFielName + ".y));\r\n";

                    binaryRead += "\t\t\t" + strFielName + ".x=reader.ReadSingle();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".y=reader.ReadSingle();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".x);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".y);\r\n";
                }
                else if (fieldType == typeof(Vector2Int))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\t\tif(vParams.Count <= 0) return true;\r\n";
                        strReadString += "\t\t\t\t" + strFielName + ".x = int.Parse(vParams[0]);\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0) return true;\r\n";
                        strReadString += "\t\t\t\t" + strFielName + ".y = int.Parse(vParams[0]);\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\t{\r\n";
                        strReadString += "\t\t\t\tstring[] split = " + inpuDataName + ".Split(',');\r\n";
                        strReadString += "\t\t\t\tif(split.Length == 2)\r\n";
                        strReadString += "\t\t\t\t{\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".x= int.Parse(split[0]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".y= int.Parse(split[1]);\r\n";
                        strReadString += "\t\t\t\t}\r\n";
                        strReadString += "\t\t\t}\r\n";
                    }
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".x.ToString()" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".y.ToString()" + "+\",\"" + ";\r\n";

                    xmlRead += "\t\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\"))\r\n";
                    xmlRead += "\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\tstring[] split = " + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\").ToString().Split(',');\r\n";
                    xmlRead += "\t\t\t\t\tif(split.Length == 3)\r\n";
                    xmlRead += "\t\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".x= int.Parse(split[0]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".y= int.Parse(split[1]);\r\n";
                    xmlRead += "\t\t\t\t\t}\r\n";
                    xmlRead += "\t\t\t\t}\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", string.Format(\"{0},{1},{2}\"," + strFielName + ".x," + strFielName + ".y," + strFielName + ".z));\r\n";

                    binaryRead += "\t\t\t" + strFielName + ".x=reader.ReadInt32();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".y=reader.ReadInt32();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".x);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".y);\r\n";
                }
                else if (fieldType == typeof(Vector3))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".x)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".y)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".z)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\t{\r\n";
                        strReadString += "\t\t\t\tstring[] split = " + inpuDataName + ".Split(',');\r\n";
                        strReadString += "\t\t\t\tif(split.Length == 3)\r\n";
                        strReadString += "\t\t\t\t{\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".x= float.Parse(split[0]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".y= float.Parse(split[1]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".z= float.Parse(split[2]);\r\n";
                        strReadString += "\t\t\t\t}\r\n";
                        strReadString += "\t\t\t}\r\n";
                    }
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".x.ToString(\"F3\")" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".y.ToString(\"F3\")" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".z.ToString(\"F3\")" + "+\",\"" + ";\r\n";

                    xmlRead += "\t\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\"))\r\n";
                    xmlRead += "\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\tstring[] split = " + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\").ToString().Split(',');\r\n";
                    xmlRead += "\t\t\t\t\tif(split.Length == 3)\r\n";
                    xmlRead += "\t\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".x= float.Parse(split[0]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".y= float.Parse(split[1]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".z= float.Parse(split[2]);\r\n";
                    xmlRead += "\t\t\t\t\t}\r\n";
                    xmlRead += "\t\t\t\t}\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", string.Format(\"{0},{1},{2}\"," + strFielName + ".x," + strFielName + ".y," + strFielName + ".z));\r\n";

                    binaryRead += "\t\t\t" + strFielName + ".x=reader.ReadSingle();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".y=reader.ReadSingle();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".z=reader.ReadSingle();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".x);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".y);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".z);\r\n";
                }
                else if (fieldType == typeof(Vector3Int))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\t\tif(vParams.Count <= 0) return true;\r\n";
                        strReadString += "\t\t\t\t" + strFielName + ".x = int.Parse(vParams[0]);\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0) return true;\r\n";
                        strReadString += "\t\t\t\t" + strFielName + ".y = int.Parse(vParams[0]);\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0) return true;\r\n";
                        strReadString += "\t\t\t\t" + strFielName + ".z = int.Parse(vParams[0]);\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\t{\r\n";
                        strReadString += "\t\t\t\tstring[] split = " + inpuDataName + ".Split(',');\r\n";
                        strReadString += "\t\t\t\tif(split.Length == 3)\r\n";
                        strReadString += "\t\t\t\t{\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".x= int.Parse(split[0]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".y= int.Parse(split[1]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".z= int.Parse(split[2]);\r\n";
                        strReadString += "\t\t\t\t}\r\n";
                        strReadString += "\t\t\t}\r\n";
                    }
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".x.ToString()" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".y.ToString()" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".z.ToString()" + "+\",\"" + ";\r\n";

                    xmlRead += "\t\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\"))\r\n";
                    xmlRead += "\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\tstring[] split = " + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\").ToString().Split(',');\r\n";
                    xmlRead += "\t\t\t\t\tif(split.Length == 3)\r\n";
                    xmlRead += "\t\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".x= int.Parse(split[0]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".y= int.Parse(split[1]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".z= int.Parse(split[2]);\r\n";
                    xmlRead += "\t\t\t\t\t}\r\n";
                    xmlRead += "\t\t\t\t}\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", string.Format(\"{0},{1},{2}\"," + strFielName + ".x," + strFielName + ".y," + strFielName + ".z));\r\n";

                    binaryRead += "\t\t\t" + strFielName + ".x=reader.ReadInt32();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".y=reader.ReadInt32();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".z=reader.ReadInt32();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".x);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".y);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".z);\r\n";
                }
                else if (fieldType == typeof(Vector4))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".x)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".y)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".z)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".w)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\t{\r\n";
                        strReadString += "\t\t\t\tstring[] split = " + inpuDataName + ".Split(',');\r\n";
                        strReadString += "\t\t\t\tif(split.Length == 4)\r\n";
                        strReadString += "\t\t\t\t{\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".x= float.Parse(split[0]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".y= float.Parse(split[1]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".z= float.Parse(split[2]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".w= float.Parse(split[3]);\r\n";
                        strReadString += "\t\t\t\t}\r\n";
                        strReadString += "\t\t\t}\r\n";
                    }
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".x.ToString(\"F3\")" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".y.ToString(\"F3\")" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".z.ToString(\"F3\")" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".w.ToString(\"F3\")" + "+\",\"" + ";\r\n";

                    xmlRead += "\t\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\"))\r\n";
                    xmlRead += "\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\tstring[] split = " + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\").ToString().Split(',');\r\n";
                    xmlRead += "\t\t\t\t\tif(split.Length == 4)\r\n";
                    xmlRead += "\t\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".x= float.Parse(split[0]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".y= float.Parse(split[1]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".z= float.Parse(split[2]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".w= float.Parse(split[3]);\r\n";
                    xmlRead += "\t\t\t\t\t}\r\n";
                    xmlRead += "\t\t\t\t}\r\n";
                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", string.Format(\"{0},{1},{2},{3}\"," + strFielName + ".x," + strFielName + ".y," + strFielName + ".z," + strFielName + ".w));\r\n";

                    binaryRead += "\t\t\t" + strFielName + ".x=reader.ReadSingle();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".y=reader.ReadSingle();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".z=reader.ReadSingle();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".w=reader.ReadSingle();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".x);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".y);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".z);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".w);\r\n";
                }
                else if (fieldType == typeof(Quaternion))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".x)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".y)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".z)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";

                        strReadString += "\t\t\t\tif(vParams.Count <= 0 || !float.TryParse(vParams[0], out " + strFielName + ".w)) return true;\r\n";
                        strReadString += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\t{\r\n";
                        strReadString += "\t\t\t\tstring[] split = " + inpuDataName + ".Split(',');\r\n";
                        strReadString += "\t\t\t\tif(split.Length == 4)\r\n";
                        strReadString += "\t\t\t\t{\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".x= float.Parse(split[0]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".y= float.Parse(split[1]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".z= float.Parse(split[2]);\r\n";
                        strReadString += "\t\t\t\t\t" + strFielName + ".w= float.Parse(split[3]);\r\n";
                        strReadString += "\t\t\t\t}\r\n";
                        strReadString += "\t\t\t}\r\n";
                    }
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".x.ToString(\"F3\")" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".y.ToString(\"F3\")" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".z.ToString(\"F3\")" + "+\",\"" + ";\r\n";
                    strWriteString += "\t\t\t\tstrParam +=" + strFielName + ".w.ToString(\"F3\")" + "+\",\"" + ";\r\n";

                    xmlRead += "\t\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\"))\r\n";
                    xmlRead += "\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\tstring[] split = " + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\").ToString().Split(',');\r\n";
                    xmlRead += "\t\t\t\t\tif(split.Length == 4)\r\n";
                    xmlRead += "\t\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".x= float.Parse(split[0]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".y= float.Parse(split[1]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".z= float.Parse(split[2]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".w= float.Parse(split[3]);\r\n";
                    xmlRead += "\t\t\t\t\t}\r\n";
                    xmlRead += "\t\t\t\t}\r\n";

                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", string.Format(\"{0},{1},{2},{3}\"," + strFielName + ".x," + strFielName + ".y," + strFielName + ".z," + strFielName + ".w));\r\n";

                    binaryRead += "\t\t\t" + strFielName + ".x=reader.ReadSingle();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".y=reader.ReadSingle();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".z=reader.ReadSingle();\r\n";
                    binaryRead += "\t\t\t" + strFielName + ".w=reader.ReadSingle();\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".x);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".y);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".z);\r\n";
                    binaryWrite += "\t\t\twriter.Write(" + strFielName + ".w);\r\n";
                }
                else if (fieldType == typeof(Data.CurveData))
                {
                    if (isvData)
                    {
                        strReadString += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                        strReadString += "\t\t\t" + strFielName + " = EventHelper.ReadSplineCurveData(vParams[0]);\r\n";
                        strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    }
                    else
                    {
                        strReadString += "\t\t\t" + strFielName + " = EventHelper.ReadSplineCurveData(" + inpuDataName + ");\r\n";
                    }
                    strWriteString += "\t\t\tstrParam += EventHelper.SaveCurve(" + strFielName + ")" + "+\",\"" + ";\r\n";

                    xmlRead += "\t\t\t\tif(" + Types.xmlName + ".HasAttribute(\"" + Types.vFields[j].Name + "\"))\r\n";
                    xmlRead += "\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\tstring[] split = " + Types.xmlName + ".GetAttribute(\"" + Types.vFields[j].Name + "\").ToString().Split(',');\r\n";
                    xmlRead += "\t\t\t\t\tif(split.Length == 4)\r\n";
                    xmlRead += "\t\t\t\t\t{\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".x= float.Parse(split[0]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".y= float.Parse(split[1]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".z= float.Parse(split[2]);\r\n";
                    xmlRead += "\t\t\t\t\t\t" + strFielName + ".w= float.Parse(split[3]);\r\n";
                    xmlRead += "\t\t\t\t\t}\r\n";
                    xmlRead += "\t\t\t\t}\r\n";

                    xmlWrite += "\t\t\t" + Types.xmlName + ".SetAttribute(\"" + Types.vFields[j].Name + "\", string.Format(\"{0},{1},{2},{3}\"," + strFielName + ".x," + strFielName + ".y," + strFielName + ".z," + strFielName + ".w));\r\n";

                }
                else if (fieldType == typeof(Framework.Data.KeyValueParam))
                {
                    strReadString += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                    strReadString += "\t\t\t" + strFielName + " = EventHelper.ReadKV(vParams[0]);\r\n";
                    strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    strWriteString += "\t\t\tstrParam += EventHelper.SaveKV(" + strFielName + ")" + "+\",\"" + ";\r\n";
                }
                else if (fieldType == typeof(AnimationCurve))
                {
                    strReadString += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                    strReadString += "\t\t\t" + strFielName + " = EventHelper.ReadCurve(vParams[0]);\r\n";
                    strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";
                    strWriteString += "\t\t\tstrParam += EventHelper.SaveCurve(" + strFielName + ")" + "+\",\"" + ";\r\n";
                }
                else if (fieldType == typeof(PhysicPropertyData))
                {
                    strReadString += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                    strReadString += "\t\t\tif (!EventHelper.ReadActionProperty(vParams[0], out property)) return false;\r\n";
                    strReadString += "\t\t\tvParams.RemoveAt(0);\r\n";

                    strWriteString += "\t\t\tstrParam +=EventHelper.SaveActionProperty(property)+\",\";\r\n";
                }
                else if (fieldType == typeof(SplineData))
                {
                    strReadString += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                    strReadString += "\t\t\tif (!EventHelper.ReadSpawnSplineTracker(vParams[0], out "+ strFielName + ")) return false;\r\n";

                    strWriteString += "\t\t\tstrParam += EventHelper.SaveSpawnSplineTracker(spawnData)+\",\";\r\n";
                }
                else if (fieldType.IsArray || fieldType.IsGenericType)
                {
                    Type decType = null;
                    bool bOk = false;
                    if (ActionEventCoreGeneratorSerializerArray.bArray(fieldType, out decType))
                    {
                        bOk = ActionEventCoreGeneratorSerializerArray.BuildArraySerializer(Types.vFields[j], decType, Types.xmlName, ref strReadString, ref strWriteString, ref xmlRead, ref xmlWrite, ref binaryRead, ref binaryWrite);
                    }
                    else if (ActionEventCoreGeneratorSerializerList.bList(fieldType, out decType))
                    {
                        bOk = ActionEventCoreGeneratorSerializerList.BuildListSerializer(Types.vFields[j], decType, Types.xmlName, ref strReadString, ref strWriteString, ref xmlRead, ref xmlWrite, ref binaryRead, ref binaryWrite);
                    }
                    else if (ActionEventCoreGeneratorSerializerHashSet.bHashSet(fieldType, out decType))
                    {
                        bOk = ActionEventCoreGeneratorSerializerHashSet.BuildHashSetSerializer(Types.vFields[j], decType, Types.xmlName, ref strReadString, ref strWriteString, ref xmlRead, ref xmlWrite, ref binaryRead, ref binaryWrite);
                    }
                    if (!bOk)
                    {
                        Debug.LogError(Types.type + " 类成员：" + Types.vFields[j].Name + " 不支持序列化");
                        return false;
                    }
                }
                else
                {
                    Debug.LogError(Types.type + " 类成员：" + Types.vFields[j].Name + " 不支持序列化");
                    return false;
                }
            }
            return true;
        }
    }
}
#endif