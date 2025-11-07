#if UNITY_EDITOR
/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	ActionEventCoreGeneratorSerializerArray
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
    public class ActionEventCoreGeneratorSerializerArray
    {
        //------------------------------------------------------
        public static bool BuildBinaryArrayCs(FieldInfo field, System.Type fieldType, string strDecName, ref string strRead, ref string strWrite)
        {
            if (fieldType == null) return false;
            if (string.IsNullOrEmpty(strDecName)) strDecName = field.Name;

            strRead += "\t\t\t{\r\n";
            strRead += "\t\t\t\tint cnt = (int)reader.ToUshort();\r\n";
            strRead += "\t\t\t\tif(cnt>0)\r\n";
            strRead += "\t\t\t\t{\r\n";
            strRead += "\t\t\t\t\t" + field.Name + "= new " + strDecName + "[cnt];\r\n";
            strRead += "\t\t\t\t\tfor(int i =0; i < cnt; ++i)\r\n";
            strRead += "\t\t\t\t\t{\r\n";

            strWrite += "\t\t\t{\r\n";
            strWrite += "\t\t\t\tif("+ field.Name + "!=null) writer.WriteUshort((ushort)(" + field.Name + ".Length));\r\n";
            strWrite += "\t\t\t\telse writer.WriteUshort((ushort)0);\r\n";
            strWrite += "\t\t\t\tif (" + field.Name + "!=null && " + field.Name + ".Length > 0)\r\n";
            strWrite += "\t\t\t\t{\r\n";
            strWrite += "\t\t\t\t\tfor(int i =0; i <" + field.Name + ".Length; ++i)\r\n";

            string strReadString = "";
            string strWriteString = "";
            if (fieldType == typeof(bool))
            {
                strReadString += field.Name + "[i] =reader.ToBool();\r\n";
                strWriteString += "writer.WriteBool("+ field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(byte))
            {
                strReadString += field.Name + "[i] =reader.ToByte();\r\n";
                strWriteString += "writer.WriteByte(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(short))
            {
                strReadString += field.Name + "[i] =reader.ToShort();\r\n";
                strWriteString += "writer.WriteShort(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(ushort))
            {
                strReadString += field.Name + "[i] =reader.ToUshort();\r\n";
                strWriteString += "writer.WriteUshort(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(int))
            {
                strReadString += field.Name + "[i] =reader.ToInt32();\r\n";
                strWriteString += "writer.WriteInt32(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(uint))
            {
                strReadString += field.Name + "[i] =reader.ToUint32();\r\n";
                strWriteString += "writer.WriteUint32(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(float))
            {
                strReadString += field.Name + "[i] =reader.ToFloat();\r\n";
                strWriteString += "writer.WriteFloat(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(double))
            {
                strReadString += field.Name + "[i] =reader.ToDouble());\r\n";
                strWriteString += "writer.WriteDouble(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(long))
            {
                strReadString += field.Name + "[i] =reader.ToInt64();\r\n";
                strWriteString += "writer.WriteInt64(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(ulong))
            {
                strReadString += field.Name + "[i] =reader.ToUint64();\r\n";
                strWriteString += "writer.WriteUint64(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType.IsEnum)
            {
                string enumTypeName = fieldType.FullName.Replace("+", ".");

                strReadString += field.Name + "[i] =(" + enumTypeName + ")reader.ToShort();\r\n";
                strWriteString += "writer.WriteShort((short)" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(string))
            {
                strReadString += field.Name + "[i] =reader.ToString();\r\n";
                strWriteString += "writer.WriteString(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(Vector2))
            {
                strReadString += field.Name + "[i] =reader.ToVec2();\r\n";
                strWriteString += "writer.WriteVector2(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(Vector2Int))
            {
                strReadString += field.Name + "[i] =reader.ToVec2Int();\r\n";
                strWriteString += "writer.WriteVector2Int(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(Vector3))
            {
                strReadString += field.Name + "[i] =reader.ToVec3();\r\n";
                strWriteString += "writer.WriteVector3(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(Vector3Int))
            {
                strReadString += field.Name + "[i] =reader.ToVec3Int();\r\n";
                strWriteString += "writer.WriteVector3Int(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(Vector4))
            {
                strReadString += field.Name + "[i] =reader.ToVec4();\r\n";
                strWriteString += "writer.WriteVector4(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(Color))
            {
                strReadString += field.Name + "[i] =reader.ToColor();\r\n";
                strWriteString += "writer.WriteColor(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(Quaternion))
            {
                strReadString += field.Name + "[i] =reader.ToQuaternion();\r\n";
                strWriteString += "writer.WriteQuaternion(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(Framework.Data.KeyValueParam))
            {
                strReadString += "{Framework.Data.KeyValueParam temp = new Framework.Data.KeyValueParam(); temp.Read(ref reader); " + field.Name + "[i] =temp;}\r\n";
                strWriteString += "" + field.Name + "[i].Write(ref writer);\r\n";
            }
            else if (fieldType == typeof(Data.CurveData))
            {
                strReadString += "{Data.CurveData temp = new Data.CurveData(); temp.Read(ref reader); " + field.Name + "[i] =temp;}\r\n";
                strWriteString += "" + field.Name + "[i].Write(ref writer);\r\n";
            }
            else if (fieldType == typeof(AnimationCurve))
            {
                strReadString += field.Name + "[i] =reader.ToCurve();\r\n";
                strWriteString += "writer.WriteCurve(" + field.Name + "[i]);\r\n";
            }
            else if (fieldType == typeof(PhysicPropertyData))
            {
                strReadString += "{PhysicPropertyData temp = new PhysicPropertyData(); temp.Read(ref reader); " + field.Name + "[i] =temp;}\r\n";
                strWriteString += "" + field.Name + "[i].Write(ref writer);\r\n";
            }
            else if (fieldType == typeof(SplineData))
            {
                strRead += "{SplineData temp = new SplineData(); temp.Read(ref reader); " + field.Name + "[i] =temp;}\r\n";
                strWriteString += "" + field.Name + "[i].Write(ref writer);\r\n";
            }
            else
            {
                Debug.LogError(fieldType.Name + " 类成员：" + fieldType.Name + " 不支持序列化");
                return false;
            }

            strRead += "\t\t\t\t\t\t" + strReadString + "\r\n";
            strRead += "\t\t\t\t\t}\r\n";
            strRead += "\t\t\t\t}\r\n";
            strRead += "\t\t\t}\r\n";


            strWrite += "\t\t\t\t\t" + strWriteString + "\r\n";
            strWrite += "\t\t\t\t}\r\n";
            strWrite += "\t\t\t}\r\n";

            return true;
        }

        //------------------------------------------------------
        public static bool BuildArrayCpp(FieldInfo field, System.Type type, string strDecName, ref string strRead, ref string strWrite)
        {
            if (type == null) return false;
            if(string.IsNullOrEmpty(strDecName)) strDecName =  field.Name;
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
        public static bool BuildArraySerializer(FieldInfo field, System.Type type, string xmlEle, ref string strRead, ref string strWrite, ref string xmlRead, ref string xmlWrite, ref string binaryRead, ref string binaryWrite)
        {
            if (type == null) return false;
            if (type == typeof(bool))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = vParams[a].CompareTo(\"1\") == 0;}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrParam += \",\"\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a]?\"1\":\"0\";\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet+ \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = vals[a].CompareTo(\"1\") == 0;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a]?\"1\":\"0\";\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\""+ field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[size];\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + "[a] = reader.ReadBoolean();\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Length:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(byte))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = byte.Parse(vParams[a]);}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet+ \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = byte.Parse(vals[a]);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[size];\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + "[a] = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Length:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(short))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = short.Parse(vParams[a]);}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet+ \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = short.Parse(vals[a]);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[size];\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + "[a] = reader.ReadInt16();\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Length:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(ushort))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = ushort.Parse(vParams[a]);}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet+ \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = ushort.Parse(vals[a]);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \"@\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[size];\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + "[a] = reader.ReadUInt16();\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Length:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(int))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = int.Parse(vParams[a]);}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet+ \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = int.Parse(vals[a]);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[size];\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + "[a] = reader.ReadInt32();\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Length:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(uint))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = uint.Parse(vParams[a]);}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet+ \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = uint.Parse(vals[a]);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[size];\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + "[a] = reader.ReadUInt32();\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Length:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(float))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = float.Parse(vParams[a]);}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet+ \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = float.Parse(vals[a]);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a].ToString(\"F3\");\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[size];\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + "[a] = reader.ReadSingle();\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Length:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(double))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = double.Parse(vParams[a]);}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet+ \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = double.Parse(vals[a]);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[size];\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + "[a] = reader.ReadDouble();\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Length:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(long))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = long.Parse(vParams[a]);}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet+ \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = long.Parse(vals[a]);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[size];\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + "[a] = reader.ReadInt64();\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Length:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(ulong))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = ulong.Parse(vParams[a]);}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet+ \",\";\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = unlong.Parse(vals[a]);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[size];\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + "[a] = reader.ReadUInt64();\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Length:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type.IsEnum)
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = (" + type.FullName.Replace("+", ".") + ")int.Parse(vParams[a]);\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += (int)" + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet+ \",\";\r\n";
                strWrite += "\t\t\t}\r\n";


                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = (" + type.FullName.Replace("+", ".") + ")int.Parse(vals[a]);\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += ((int)" + field.Name + "[a]);\r\n";
                xmlWrite += "\t\t\t\t\t\tif(a < " + field.Name + ".Length-1) strRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";

                binaryRead += "\t\t\t{\r\n";
                binaryRead += "\t\t\t\tbyte size = reader.ReadByte();\r\n";
                binaryRead += "\t\t\t\tif(size>0)\r\n";
                binaryRead += "\t\t\t\t{\r\n";
                binaryRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[size];\r\n";
                binaryRead += "\t\t\t\t\tfor(int a =0; a < size; ++a)\r\n";
                binaryRead += "\t\t\t\t\t\t" + field.Name + "[a] =(" + type.FullName.Replace("+", ".") + ") reader.ReadInt16();\r\n";
                binaryRead += "\t\t\t\t}\r\n";
                binaryRead += "\t\t\t}\r\n";

                binaryWrite += "\t\t\t{\r\n";
                binaryWrite += "\t\t\t\twriter.Write((byte)(" + field.Name + " != null ? " + field.Name + ".Length:0))\r\n";
                binaryWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                binaryWrite += "\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                binaryWrite += "\t\t\t\t\t{\r\n";
                binaryWrite += "\t\t\t\t\t\twriter.Write(" + field.Name + "[a]);\r\n";
                binaryWrite += "\t\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t\t}\r\n";
                binaryWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(string))
            {
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tif(vParams.Count<=0) return false;\r\n";
                strRead += "\t\t\t\tint arrLen =0; if(!int.TryParse(vParams[0], out arrLen)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(arrLen>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[arrLen];\r\n";
                strRead += "\t\t\t\t\tfor(int a =0; a < arrLen; ++a)\r\n";
                strRead += "\t\t\t\t\t{if(a < vParams.Count)" + field.Name + "[a] = vParams[a];}\r\n";
                strRead += "\t\t\t\t\tvParams.RemoveRange(0,arrLen);\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";

                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tif(" + xmlEle + ".HasAttribute(\"" + field.Name + "\"))\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tstring[] vals = " + xmlEle + ".GetAttribute(\"" + field.Name + "\").Split(',');\r\n";
                xmlRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                xmlRead += "\t\t\t\tif(vals!=null && vals.Length>0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + " = new " + type.FullName + "[vals.Length];\r\n";
                xmlRead += "\t\t\t\t\tfor(int a =0; a < vals.Length; ++a)\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[a] = vals[a];\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tstring strRet = \"\";\r\n";
                xmlWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tfor(int a = 0; a < " + field.Name + ".Length; ++a)\r\n";
                xmlWrite += "\t\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += " + field.Name + "[a];\r\n";
                xmlWrite += "\t\t\t\t\t\tstrRet += \",\";\r\n";
                xmlWrite += "\t\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.SetAttribute(\"" + field.Name + "\", strRet );\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(Vector2))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt =0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(tempCnt>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t"+ field.Name + "= new UnityEngine.Vector2[tempCnt];\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 1) return false;\r\n";
                strRead += "\t\t\t\t\t\t"+ field.Name + "[i] = EventHelper.ReadVec2(vParams, 0);\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,2);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor (int i =0; i < " + field.Name + ".Length; ++i)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteVec2(" + field.Name + "[i]); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Vector2[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[index++] = EventHelper.ReadVec2(childKey, \"value\");\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteVec2("+ field.Name + "[i], itemNode, \"value\");\r\n";
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
                strRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Vector2Int[tempCnt];\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 1) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + "[i] = EventHelper.ReadVec2Int(vParams, 0);\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,2);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor (int i =0; i < " + field.Name + ".Length; ++i)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteVec2Int(" + field.Name + "[i]); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Vector2Int[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[index++] = EventHelper.ReadVec2Int(childKey, \"value\");\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteVec2Int(" + field.Name + "[i], itemNode, \"value\");\r\n";
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
                strRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Vector2[tempCnt];\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 2) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + "[i] = EventHelper.ReadVec3(vParams, 0);\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,3);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor (int i =0; i < " + field.Name + ".Length; ++i)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteVec3(" + field.Name + "[i]); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Vector3[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[index++] = EventHelper.ReadVec3(childKey, \"value\");\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteVec3(" + field.Name + "[i], itemNode, \"value\");\r\n";
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
                strRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Vector3Int[tempCnt];\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 3) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + "[i] = EventHelper.ReadVec3Int(vParams, 0);\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,3);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor (int i =0; i < " + field.Name + ".Length; ++i)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteVec3Int(" + field.Name + "[i]); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Vector3Int[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[index++] = EventHelper.ReadVec3Int(childKey, \"value\");\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteVec3Int(" + field.Name + "[i], itemNode, \"value\");\r\n";
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
                strRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Vector4[tempCnt];\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 4) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + "[i] = EventHelper.ReadVec4(vParams, 0);\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,4);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor (int i =0; i < " + field.Name + ".Length; ++i)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteVec4(" + field.Name + "[i]); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Vector4[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[index++] = EventHelper.ReadVec4(childKey, \"value\");\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteVec4(" + field.Name + "[i], itemNode, \"value\");\r\n";
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
                strRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Quaternion[tempCnt];\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 4) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + "[i] = EventHelper.ReadQuaternion(vParams, 0);\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,4);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor (int i =0; i < " + field.Name + ".Length; ++i)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteQuaternion(" + field.Name + "[i]); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Quaternion[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[index++] = EventHelper.ReadQuaternion(childKey, \"value\");\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteQuaternion(" + field.Name + "[i], itemNode, \"value\");\r\n";
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
                strRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Color[tempCnt];\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 4) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + "[i] = EventHelper.ReadColor(vParams, 0);\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,4);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor (int i =0; i < " + field.Name + ".Length; ++i)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteColor(" + field.Name + "[i]); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + "= new UnityEngine.Color[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[index++] = EventHelper.ReadColor(childKey, \"value\");\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteColor(" + field.Name + "[i], itemNode, \"value\");\r\n";
                xmlWrite += "\t\t\t\t\tkeysNode.AppendChild(itemNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
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
                strRead += "\t\t\t\t\t" + field.Name + "= new Framework.Data.KeyValueParam[tempCnt];\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 2) return false;\r\n";
                strRead += "\t\t\t\t\t\t" + field.Name + "[i] = EventHelper.ReadKV(vParams, 0);\r\n";
                strRead += "\t\t\t\t\t\tvParams.RemoveRange(0,2);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstring strRet = (" + field.Name + "!=null)?" + field.Name + ".Length.ToString()" + ":\"0\";\r\n";
                strWrite += "\t\t\t\tstrRet += \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor (int i =0; i < " + field.Name + ".Length; ++i)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrRet += EventHelper.WriteKV(" + field.Name + "[i]); strRet += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t\tstrParam += strRet;\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + "= new Framework.Data.KeyValueParam[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\t" + field.Name + "[index++] = EventHelper.ReadKV(childKey, \"value\");\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tXmlElement itemNode = pDoc.CreateElement(\"item\");\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.WriteKV(" + field.Name + "[i], itemNode, \"value\");\r\n";
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
                strRead += "\t\t\t\tif(tempCnt>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\tfield.Name = new Base.CurveData[tempCnt];\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 0) return false;\r\n";
                strRead += "\t\t\t\t\t\tData.CurveData keyF;if(EventHelper.ReadSplineCurveData(vParams[0], out keyF)) " + field.Name + ".Add(keyF);";
                strRead += "\t\t\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstrParam += (" + field.Name + "!=null?" + field.Name + ".Length.ToString():0) + \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor (int i =0; i < " + field.Name + ".Length; ++i)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrParam += EventHelper.SaveSplineCurveData(" + field.Name + "[i]); strParam += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + "= new Base.CurveData[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\tBase.CurveData keyF;\r\n";
                xmlRead += "\t\t\t\t\t\tif(EventHelper.ReadSplineCurveData(childKey, out keyF)) " + field.Name + "[++index] =keyF;\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)EventHelper.SaveSplineCurveData(pDoc, ref keysNode, " + field.Name + "[i]);\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else if (type == typeof(AnimationCurve))
            {
                strRead += "\t\t\tif(vParams.Count <= 0) return true;\r\n";
                strRead += "\t\t\t{\r\n";
                strRead += "\t\t\t\tint tempCnt = 0; if(!int.TryParse(vParams[0], out tempCnt)) return false;\r\n";
                strRead += "\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\tif(tempCnt>0)\r\n";
                strRead += "\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tfield.Name = new AnimationCurve[tempCnt];\r\n";
                strRead += "\t\t\t\t\tfor(int i =0; i < tempCnt; ++i)\r\n";
                strRead += "\t\t\t\t\t{\r\n";
                strRead += "\t\t\t\t\t\tif(vParams.Count <= 0) return false;\r\n";
                strRead += "\t\t\t\t\t\tAnimationCurve keyF = EventHelper.ReadCurve(vParams[0]); if(keyF!=null)" + field.Name + ".Add(keyF);";
                strRead += "\t\t\t\t\t\tvParams.RemoveAt(0);\r\n";
                strRead += "\t\t\t\t\t}\r\n";
                strRead += "\t\t\t\t}\r\n";
                strRead += "\t\t\t}\r\n";


                strWrite += "\t\t\t{\r\n";
                strWrite += "\t\t\t\tstrParam += (" + field.Name + "!=null?" + field.Name + ".Length.ToString():0) + \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor (int i =0; i < " + field.Name + ".Length; ++i)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrParam += EventHelper.SaveCurve(" + field.Name + "[i]);  strParam += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
                strWrite += "\t\t\t\t}\r\n";
                strWrite += "\t\t\t}\r\n";

                xmlRead += "\t\t\tforeach (XmlElement childKey in ele)\r\n";
                xmlRead += "\t\t\t{\r\n";
                xmlRead += "\t\t\t\tif (childKey.Name.CompareTo(\"" + field.Name + "\") == 0)\r\n";
                xmlRead += "\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t" + field.Name + "= new Base.CurveData[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\tAnimationCurve keyF = EventHelper.ReadCurve(subKey); if(keyF!=null)" + field.Name + "[++index] =keyF;\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)EventHelper.SaveCurve(" + field.Name + "[i],pDoc,keysNode);\r\n";
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
                strWrite += "\t\t\t\tstrParam += (" + field.Name + "!=null?" + field.Name + ".Length.ToString():0) + \",\";\r\n";
                strWrite += "\t\t\t\tif(" + field.Name + "!=null)\r\n";
                strWrite += "\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\tfor (int i =0; i < " + field.Name + ".Length; ++i)\r\n";
                strWrite += "\t\t\t\t\t{\r\n";
                strWrite += "\t\t\t\t\t\tstrParam += EventHelper.SaveActionProperty(" + field.Name + "[i]);  strParam += \",\";\r\n";
                strWrite += "\t\t\t\t\t}\r\n";
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
                xmlRead += "\t\t\t\t\t" + field.Name + "= new Base.CurveData[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\tActionStatePropertyData keyF;\r\n";
                xmlRead += "\t\t\t\t\t\tif(EventHelper.ReadActionProperty(subKey, out keyF)) " + field.Name + "[++index] =keyF;\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)\r\n";
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
                xmlRead += "\t\t\t\t\t" + field.Name + "= new Base.CurveData[childKey.ChildNodes.Count];\r\n";
                xmlRead += "\t\t\t\t\tint index=0;\r\n";
                xmlRead += "\t\t\t\t\tforeach(XmlElement subKey in childKey)\r\n";
                xmlRead += "\t\t\t\t\t{\r\n";
                xmlRead += "\t\t\t\t\t\tFramework.Logic.SpawnSplineData keyF;\r\n";
                xmlRead += "\t\t\t\t\t\tif(EventHelper.ReadSpawnSplineTracker(subKey, out keyF)) " + field.Name + "[++index] = keyF;\r\n";
                xmlRead += "\t\t\t\t\t}\r\n";
                xmlRead += "\t\t\t\t\tbreak;\r\n";
                xmlRead += "\t\t\t\t}\r\n";
                xmlRead += "\t\t\t}\r\n";

                xmlWrite += "\t\t\t{\r\n";
                xmlWrite += "\t\t\t\tXmlElement keysNode = pDoc.CreateElement(\"" + field.Name + "\");\r\n";
                xmlWrite += "\t\t\t\tfor (int i = 0; i < " + field.Name + ".Length; ++i)\r\n";
                xmlWrite += "\t\t\t\t{\r\n";
                xmlWrite += "\t\t\t\t\tEventHelper.SaveSpawnSplineTracker(" + field.Name + "[i], pDoc, keysNode);\r\n";
                xmlWrite += "\t\t\t\t}\r\n";
                xmlWrite += "\t\t\t\tele.AppendChild(keysNode);\r\n";
                xmlWrite += "\t\t\t}\r\n";
            }
            else
                return false;
            return true;
        }
        //------------------------------------------------------
        public static bool bArray(Type paramType, out Type argType)
        {
            argType = null;
            if (paramType == null) return false;
            if (paramType.IsArray && paramType.FullName.Contains("[]"))
            {
                argType = paramType.Assembly.GetType(paramType.FullName.Replace("+", ".").Replace("[]", ""));
                return true;
            }
            return false;
        }
    }
}
#endif