#if UNITY_EDITOR
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ActionEventCoreGeneratorSerializerList
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
    public class ActionEventCoreGeneratorSerializerList
    {
        //------------------------------------------------------
        public static bool BuildBinaryListCs(FieldInfo field, System.Type fieldType, string strDecName, ref string strRead, ref string strWrite)
        {
            if (fieldType == null) return false;
            if (string.IsNullOrEmpty(strDecName)) strDecName = field.Name;

            strRead += "\t\t\t{\r\n";
            strRead += "\t\t\t\tint cnt = (int)reader.ToUshort();\r\n";
            strRead += "\t\t\t\tif(cnt>0)\r\n";
            strRead += "\t\t\t\t{\r\n";
            strRead += "\t\t\t\t\t"+ field.Name + "= new System.Collections.Generic.List<" + strDecName + ">(cnt);\r\n";
            strRead += "\t\t\t\t\tfor(int i =0; i < cnt; ++i)\r\n";
            strRead += "\t\t\t\t\t{\r\n";

            strWrite += "\t\t\t{\r\n";
            strWrite += "\t\t\t\tif(" + field.Name + "!=null) writer.WriteUshort((ushort)(" + field.Name + ".Count));\r\n";
            strWrite += "\t\t\t\telse writer.WriteUshort((ushort)0);\r\n";
            strWrite += "\t\t\t\tif (" + field.Name + "!=null && " + field.Name + ".Count > 0)\r\n";
            strWrite += "\t\t\t\t{\r\n";
            strWrite += "\t\t\t\t\tforeach(var db in " + field.Name + ")\r\n";

            string strReadString = "";
            string strWriteString = "";
            if (fieldType == typeof(bool))
            {
                strReadString += field.Name + ".Add(reader.ToBool());\r\n";
                strWriteString += "writer.WriteBool(db);\r\n";
            }
            else if (fieldType == typeof(byte))
            {
                strReadString += field.Name + ".Add(reader.ToByte());\r\n";
                strWriteString += "writer.WriteByte(db);\r\n";
            }
            else if (fieldType == typeof(short))
            {
                strReadString += field.Name + ".Add(reader.ToShort());\r\n";
                strWriteString += "writer.WriteShort(db);\r\n";
            }
            else if (fieldType == typeof(ushort))
            {
                strReadString += field.Name + ".Add(reader.ToUshort());\r\n";
                strWriteString += "writer.WriteUshort(db);\r\n";
            }
            else if (fieldType == typeof(int))
            {
                strReadString += field.Name + ".Add(reader.ToInt32());\r\n";
                strWriteString += "writer.WriteInt32(db);\r\n";
            }
            else if (fieldType == typeof(uint))
            {
                strReadString += field.Name + ".Add(reader.ToUint32());\r\n";
                strWriteString += "writer.WriteUint32(db);\r\n";
            }
            else if (fieldType == typeof(float))
            {
                strReadString += field.Name + ".Add(reader.ToFloat());\r\n";
                strWriteString += "writer.WriteFloat(db);\r\n";
            }
            else if (fieldType == typeof(double))
            {
                strReadString += field.Name + ".Add(reader.ToDouble());\r\n";
                strWriteString += "writer.WriteDouble(db);\r\n";
            }
            else if (fieldType == typeof(long))
            {
                strReadString += field.Name + ".Add(reader.ToInt64());\r\n";
                strWriteString += "writer.WriteInt64(db);\r\n";
            }
            else if (fieldType == typeof(ulong))
            {
                strReadString += field.Name + ".Add(reader.ToUint64());\r\n";
                strWriteString += "writer.WriteUint64(val);\r\n";
            }
            else if (fieldType.IsEnum)
            {
                string enumTypeName = fieldType.FullName.Replace(fieldType.Namespace, "").Replace("+", ".");
                if (enumTypeName.StartsWith(".")) enumTypeName = enumTypeName.Substring(1);

                strReadString += field.Name + ".Add((" + enumTypeName + ")reader.ToShort());\r\n";
                strWriteString += "writer.WriteShort((short)db);\r\n";
            }
            else if (fieldType == typeof(string))
            {
                strReadString += field.Name + ".Add(reader.ToString());\r\n";
                strWriteString += "writer.WriteString(db);\r\n";
            }
            else if (fieldType == typeof(Vector2))
            {
                strReadString += field.Name + ".Add(reader.ToVec2());\r\n";
                strWriteString += "writer.WriteVector2(db);\r\n";
            }
            else if (fieldType == typeof(Vector2Int))
            {
                strReadString += field.Name + ".Add(reader.ToVec2Int());\r\n";
                strWriteString += "writer.WriteVector2Int(db);\r\n";
            }
            else if (fieldType == typeof(Vector3))
            {
                strReadString += field.Name + ".Add(reader.ToVec3());\r\n";
                strWriteString += "writer.WriteVector3(db);\r\n";
            }
            else if (fieldType == typeof(Vector3Int))
            {
                strReadString += field.Name + ".Add(reader.ToVec3Int());\r\n";
                strWriteString += "writer.WriteVector3Int(db);\r\n";
            }
            else if (fieldType == typeof(Vector4))
            {
                strReadString += field.Name + ".Add(reader.ToVec4());\r\n";
                strWriteString += "writer.WriteVector4(db);\r\n";
            }
            else if (fieldType == typeof(Color))
            {
                strReadString += field.Name + ".Add(reader.ToColor());\r\n";
                strWriteString += "writer.WriteColor(db);\r\n";
            }
            else if (fieldType == typeof(Quaternion))
            {
                strReadString += field.Name + ".Add(reader.ToQuaternion());\r\n";
                strWriteString += "writer.WriteQuaternion(db);\r\n";
            }
            else if (fieldType == typeof(Data.CurveData))
            {
                strReadString += "{Data.CurveData temp = new Data.CurveData(); temp.Read(ref reader); " + field.Name + ".Add(temp);}\r\n";
                strWriteString += "db.Write(ref writer);\r\n";
            }
            else if (fieldType == typeof(Framework.Data.KeyValueParam))
            {
                strReadString += "{Framework.Data.KeyValueParam temp = new Framework.Data.KeyValueParam(); temp.Read(ref reader); " + field.Name + ".Add(temp);}\r\n";
                strWriteString += "db.Write(ref writer);\r\n";
            }
            else if (fieldType == typeof(AnimationCurve))
            {
                strReadString += field.Name + ".Add(reader.ToCurve());\r\n";
                strWriteString += "writer.WriteCurve(db);\r\n";
            }
            else if (fieldType == typeof(PhysicPropertyData))
            {
                strReadString += "{PhysicPropertyData temp = new PhysicPropertyData(); temp.Read(ref reader); " + field.Name + ".Add(temp);}\r\n";
                strWriteString += "db.Write(ref writer);\r\n";
            }
            else if (fieldType == typeof(SplineData))
            {
                strRead += "{SplineData temp = new SplineData(); temp.Read(ref reader); " + field.Name + ".Add(temp);}\r\n";
                strWriteString += "db.Write(ref writer);\r\n";
            }
            else
            {
                Debug.LogError(fieldType.Name + " 类成员：" + fieldType.Name + " 不支持序列化");
                return false;
            }

            strRead += "\t\t\t\t\t\t"+ strReadString + "\r\n";
            strRead += "\t\t\t\t\t}\r\n";
            strRead += "\t\t\t\t}\r\n";
            strRead += "\t\t\t}\r\n";


            strWrite += "\t\t\t\t\t"+ strWriteString + "\r\n";
            strWrite += "\t\t\t\t}\r\n";
            strWrite += "\t\t\t}\r\n";
            
            return true;
        }
        //------------------------------------------------------
        public static bool BuildListCpp(FieldInfo field, System.Type type, string strDecName, ref string strRead, ref string strWrite)
        {
            if (type == null) return false;
            if (string.IsNullOrEmpty(strDecName)) strDecName = field.Name;

            strRead += "\t\t{\r\n";
            strRead += "\t\t\ttUint16 cnt = seralizer->readUShort();\r\n";
            strRead += "\t\t\tif(cnt>0)\r\n";
            strRead += "\t\t\t{\r\n";
            strRead += "\t\t\t\t" + field.Name + ".resize(cnt);\r\n";
            strRead += "\t\t\t\tsize_t dataLen = cnt*sizeof(" + strDecName + ");\r\n";
            strRead += "\t\t\t\tmemcpy(" + field.Name + ".get(), seralizer->getCurPtr(), dataLen);\r\n";
            strRead += "\t\t\t\tseralizer->seek((long)dataLen, SEEK_CUR);\r\n";
            strRead += "\t\t\t}\r\n";
            strRead += "\t\t}\r\n";

            strWrite += "\t\t{\r\n";
            strWrite += "\t\t\tseralizer->writeUShort((tUint16)" + field.Name + ".size());\r\n";
            strWrite += "\t\t\tif (" + field.Name + ".size() > 0)\r\n";
            strWrite += "\t\t\t{\r\n";
            strWrite += "\t\t\t\tseralizer->writeUCharArray((const tUint8*)" + field.Name + ".get(), " + field.Name + ".size()*sizeof(" + strDecName + "));\r\n";
            strWrite += "\t\t\t}\r\n";
            strWrite += "\t\t}\r\n";
            return true;
        }
        //------------------------------------------------------
        public static bool BuildListSerializer(FieldInfo field, System.Type type, string xmlEle, ref string strRead, ref string strWrite, ref string xmlRead, ref string xmlWrite, ref string binaryRead, ref string binaryWrite)
        {
            if (type == null) return false;
            string typeFullName = type.FullName.Replace("+", ".");
            if (type == typeof(bool))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(vParams[a].CompareTo(\"1\") == 0);}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(vals[a].CompareTo(\"1\") == 0);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a]?\"1\":\"0\";\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\""+ field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(reader.ReadBoolean());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(byte))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(byte.Parse(vParams[a]));}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(byte.Parse(vals[a]));\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(reader.ReadByte());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(short))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(short.Parse(vParams[a]));}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(short.Parse(vals[a]));\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(reader.ReadInt16());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(ushort))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(ushort.Parse(vParams[a]));}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(ushort.Parse(vals[a]));\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(reader.ReadUInt16());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(int))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(int.Parse(vParams[a]));}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(int.Parse(vals[a]));\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(reader.ReadInt32());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(uint))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(uint.Parse(vParams[a]));}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(uint.Parse(vals[a]));\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(reader.ReadUInt32());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(float))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(float.Parse(vParams[a]));}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(float.Parse(vals[a]));\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(reader.ReadSingle());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(double))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(double.Parse(vParams[a]));}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(double.Parse(vals[a]));\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(reader.ReadDouble());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(long))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(long.Parse(vParams[a]));}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(long.Parse(vals[a]));\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(reader.ReadInt64());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(ulong))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(ulong.Parse(vParams[a]));}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(ulong.Parse(vals[a]));\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(reader.ReadUInt64());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type.IsEnum)
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(("+ typeFullName + ")int.Parse(vParams[a]));}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += (int)" + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(("+ typeFullName + ")int.Parse(vals[a]));\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += ((int)" + field.Name + "[a]));\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(("+ typeFullName + ")reader.ReadInt16());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write((short)" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(string))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return true;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(arrLen);\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + ".Add(vParams[a]);}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet + \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split('@');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(vals[a]);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Count-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">(vals.Length);\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + ".Add(reader.ReadString());\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Count:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Count; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(Vector2))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt =0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(tempCnt>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 1) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadVec2(vParams, 0));\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,2);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteVec2(sub); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadVec2(childKey, \"value\"));\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteVec2(sub, itemNode, \"value\");\r\n";
                xmlWrite += "\t\t\t\t\tkeysNode.AppendChild(itemNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(Vector2Int))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt =0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(tempCnt>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 1) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadVec2Int(vParams, 0));\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,2);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteVec2Int(sub); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadVec2Int(childKey, \"value\"));\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteVec2Int(sub, itemNode, \"value\");\r\n";
                xmlWrite += "\t\t\t\t\tkeysNode.AppendChild(itemNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(Vector3))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt =0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(tempCnt>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 2) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadVec3(vParams, 0));\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,3);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteVec3(sub); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadVec3(childKey, \"value\"));\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteVec3(sub, itemNode, \"value\");\r\n";
                xmlWrite += "\t\t\t\t\tkeysNode.AppendChild(itemNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(Vector3Int))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt =0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(tempCnt>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 2) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadVec3Int(vParams, 0));\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,3);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteVec3Int(sub); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadVec3Int(childKey, \"value\"));\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteVec3Int(sub, itemNode, \"value\");\r\n";
                xmlWrite += "\t\t\t\t\tkeysNode.AppendChild(itemNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(Vector4))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt =0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(tempCnt>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 3) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadVec4(vParams, 0));\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,4);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteVec4(sub); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadVec4(childKey, \"value\"));\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteVec4(sub, itemNode, \"value\");\r\n";
                xmlWrite += "\t\t\t\t\tkeysNode.AppendChild(itemNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(Quaternion))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt =0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(tempCnt>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 3) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadQuaternion(vParams, 0));\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,4);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteQuaternion(sub); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadQuaternion(childKey, \"value\"));\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteQuaternion(sub, itemNode, \"value\");\r\n";
                xmlWrite += "\t\t\t\t\tkeysNode.AppendChild(itemNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(Color))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt =0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(tempCnt>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 3) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadColor(vParams, 0));\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,4);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteColor(sub); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadColor(childKey, \"value\"));\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteColor(sub, itemNode, \"value\");\r\n";
                xmlWrite += "\t\t\t\t\tkeysNode.AppendChild(itemNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }

            else if (type == typeof(Data.CurveData))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt =0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\tif(vParams.Count <= 0) return false;\r\n";
                strRead += "\t\t\t\t\tData.CurveData keyF;if(EventHelper.ReadSplineCurveData(vParams[0], out keyF)) " + field.Name + ".Add(keyF);";
                strRead += "\t\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tPlugin.SortUtility.QuickSortUp<Base.CurveData>(ref "+ field.Name + ");\r\n";
                strWrite += "\t\t\t\tstrParam += " + field.Name + ".Count.ToString() + \",\";\r\n";
                strWrite += "\t\t\t\tfor (int i =0; i < "+ field.Name + ".Count; ++i)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tstrParam += EventHelper.SaveSplineCurveData(" + field.Name + "[i]); strParam += \",\";\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\tBase.CurveData keyF;\r\n";
                xmlRead += "\t\t\t\t\t\tif(EventHelper.ReadSplineCurveData(childKey, out keyF)) " + field.Name + ".Add(keyF);\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Count; ++i)EventHelper.SaveSplineCurveData(pDoc, ref keysNode, " + field.Name + "[i]);\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }

            else if (type == typeof(Framework.Data.KeyValueParam))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt =0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(tempCnt>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 1) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadKV(vParams, 0));\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,2);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Count.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteKV(sub); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new System.Collections.Generic.List<" + typeFullName + ">();\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + ".Add(EventHelper.ReadKV(childKey, \"value\"));\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\t\tforeach (var sub in " + field.Name + ")\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteKV(sub, itemNode, \"value\");\r\n";
                xmlWrite += "\t\t\t\t\tkeysNode.AppendChild(itemNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(AnimationCurve))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt = 0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\tif(vParams.Count <= 0) return false;\r\n";
                strRead += "\t\t\t\t\tAnimationCurve keyF = EventHelper.ReadCurve(vParams[0]); if(keyF!=null)" + field.Name + ".Add(keyF);";
                strRead += "\t\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstrParam += " + field.Name + ".Count.ToString() + \",\";\r\n";
                strWrite += "\t\t\t\tfor (int i =0; i < "+ field.Name + ".Count; ++i)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tstrParam += EventHelper.SaveCurve(" + field.Name + "[i]);  strParam += \",\";\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\tAnimationCurve keyF = EventHelper.ReadCurve(subKey); if(keyF!=null)" + field.Name + ".Add(keyF);\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Count; ++i)EventHelper.SaveCurve("+field.Name + "[i],pDoc,keysNode);\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(PhysicPropertyData))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt = 0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\tif(vParams.Count <= 0) return false;\r\n";
                strRead += "\t\t\t\t\tPhysicPropertyData keyF; if(EventHelper.ReadActionProperty(vParams[0], out keyF)) " + field.Name + ".Add(keyF);";
                strRead += "\t\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstrParam += " + field.Name + ".Count.ToString() + \",\";\r\n";
                strWrite += "\t\t\t\tfor (int i =0; i < " + field.Name + ".Count; ++i)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tstrParam += EventHelper.SaveActionProperty(" + field.Name + "[i]);  strParam += \",\";\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\tBase.CurveData keyF;\r\n";
                xmlRead += "\t\t\t\t\tif(EventHelper.ReadSplineCurveData(childKey, out keyF)) " + field.Name + ".Add(keyF);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\tActionStatePropertyData keyF;\r\n";
                xmlRead += "\t\t\t\t\t\tif(EventHelper.ReadActionProperty(subKey, out keyF)) " + field.Name + ".Add(keyF);\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Count; ++i)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"Items\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.SaveActionProperty(pDoc, ref itemNode, " + field.Name + "[i]);\r\n";
                xmlWrite += "\t\t\t\t\tkeysNode.AppendChild(itemNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(SplineData))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt = 0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\tif(vParams.Count <= 0) return false;\r\n";
                strRead += "\t\t\t\t\tSplineData keyF; if(EventHelper.ReadSpawnSplineTracker(vParams[0], out keyF)) " + field.Name + ".Add(keyF);";
                strRead += "\t\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstrParam += " + field.Name + ".Count.ToString() + \",\";\r\n";
                strWrite += "\t\t\t\tfor (int i =0; i < " + field.Name + ".Count; ++i)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tstrParam += EventHelper.SaveSpawnSplineTracker(" + field.Name + "[i]);  strParam += \",\";\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\tFramework.Logic.SpawnSplineData keyF;\r\n";
                xmlRead += "\t\t\t\t\t\tif(EventHelper.ReadSpawnSplineTracker(subKey, out keyF)) " + field.Name + ".Add(keyF);\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Count; ++i)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.SaveSpawnSplineTracker(" + field.Name + "[i], pDoc, keysNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type != null && (type.IsClass || type.BaseType == typeof(System.ValueType)))
            {
                ActionEventCoreGenerator.EventType evTyp = new ActionEventCoreGenerator.EventType();
                evTyp.type = type;
                evTyp.FullName = type.FullName;
                evTyp.Name = type.Name;
                evTyp.xmlName = field.Name;
                evTyp.vFields = new List<FieldInfo>();
                FieldInfo[] fiels = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                for (int f = 0; f < fiels.Length; ++f)
                {
                    if (fiels[f].IsNotSerialized) continue;
                    evTyp.vFields.Add(fiels[f]);
                }

                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif (vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t\tstring[] param_datas = m_vParserDatas[0].Split('|');\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\t\t" + type.FullName.Replace("+", ".") + " sub = new System.Collections.Generic.List<" + type.FullName.Replace("+", ".") + ">(param_datas.Length);\r\n";
                strRead += "\t\t\t\tfor(int i = 0; i < param_datas.Length; ++i)\r\n";
                strRead += "\t\t\t\t{\r\n";
                ActionEventCoreGeneratorSerializerField.BuildFieldSerializer(evTyp, "sub", ref strRead, ref strWrite, ref xmlRead, ref xmlWrite, ref binaryRead, ref binaryWrite, "param_datas[i]");
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";
            }
            else
                return false;
            return true;
        }
        //------------------------------------------------------
        public static bool bList(Type paramType, out Type argType)
        {
            argType = null;
            if (paramType == null) return false;
            if (paramType.IsGenericType)
            {
                if (paramType.Name.Contains("List`1") && paramType.GenericTypeArguments != null && paramType.GenericTypeArguments.Length == 1)
                {
                    argType = paramType.GenericTypeArguments[0];
                    return true;
                }
            }
            return false;
        }
    }
}
#endif