/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	EventHelper
作    者:	HappLI
描    述:	
*********************************************************************/
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
#if USE_SERVER
using AnimationCurve = ExternEngine.AnimationCurve;
using Keyframe = ExternEngine.Keyframe;
#endif

namespace Framework.Core
{
    public class EventHelper
    {
        //-------------------------------------------
        public static Framework.Data.KeyValueParam ReadKV(XmlElement node, string strProperty)
        {
            if (string.IsNullOrEmpty(strProperty) || !node.HasAttribute(strProperty)) return Framework.Data.KeyValueParam.DEF;
            string[] props = node.GetAttribute(strProperty).Split(',');
            if (props == null || props.Length < 2) return Framework.Data.KeyValueParam.DEF;
            var result = Framework.Data.KeyValueParam.DEF;
            result.key = props[0];
            result.value = props[1];
            return result;
        }
        //-------------------------------------------
        public static Vector2 ReadVec2(XmlElement node, string strProperty)
        {
            if (string.IsNullOrEmpty(strProperty) || !node.HasAttribute(strProperty)) return Vector2.zero;
            string[] props = node.GetAttribute(strProperty).Split(',');
            if (props == null || props.Length<2) return Vector2.zero;
            Vector2 result = Vector2.zero;
            float.TryParse(props[0], out result.x);
            float.TryParse(props[1], out result.y);
            return result;
        }
        //-------------------------------------------
        public static Vector2Int ReadVec2Int(XmlElement node, string strProperty)
        {
            if (string.IsNullOrEmpty(strProperty) || !node.HasAttribute(strProperty)) return Vector2Int.zero;
            string[] props = node.GetAttribute(strProperty).Split(',');
            if (props == null || props.Length < 2) return Vector2Int.zero;
            Vector2Int result = Vector2Int.zero;
            int temp = 0;
            if(int.TryParse(props[0], out temp)) result.x = temp;
            if(int.TryParse(props[1], out temp)) result.y = temp;
            return result;
        }
        //-------------------------------------------
        public static Vector3Int ReadVec3Int(XmlElement node, string strProperty)
        {
            if (string.IsNullOrEmpty(strProperty) || !node.HasAttribute(strProperty)) return Vector3Int.zero;
            string[] props = node.GetAttribute(strProperty).Split(',');
            if (props == null || props.Length < 3) return Vector3Int.zero;
            Vector3Int result = Vector3Int.zero;
            int temp = 0;
            if (int.TryParse(props[0], out temp)) result.x = temp;
            if (int.TryParse(props[1], out temp)) result.y = temp;
            if (int.TryParse(props[2], out temp)) result.z = temp;
            return result;
        }
        //-------------------------------------------
        public static Vector3 ReadVec3(XmlElement node, string strProperty)
        {
            if (string.IsNullOrEmpty(strProperty) || !node.HasAttribute(strProperty)) return Vector3.zero;
            string[] props = node.GetAttribute(strProperty).Split(',');
            if (props == null || props.Length < 3) return Vector3.zero;
            Vector3 result = Vector3.zero;
            float temp = 0.0f;
            if (float.TryParse(props[0], out temp)) result.x = temp;
            if (float.TryParse(props[1], out temp)) result.y = temp;
            if (float.TryParse(props[2], out temp)) result.z = temp;
            return result;
        }
        //-------------------------------------------
        public static Vector4 ReadVec4(XmlElement node, string strProperty)
        {
            if (string.IsNullOrEmpty(strProperty) || !node.HasAttribute(strProperty)) return Vector4.zero;
            string[] props = node.GetAttribute(strProperty).Split(',');
            if (props == null || props.Length < 4) return Vector4.zero;
            Vector4 result = Vector4.zero;
            float temp = 0.0f;
            if (float.TryParse(props[0], out temp)) result.x = temp;
            if (float.TryParse(props[1], out temp)) result.y = temp;
            if (float.TryParse(props[2], out temp)) result.z = temp;
            if (float.TryParse(props[3], out temp)) result.w = temp;
            return result;
        }
        //-------------------------------------------
        public static Color ReadColor(XmlElement node, string strProperty)
        {
            if (string.IsNullOrEmpty(strProperty) || !node.HasAttribute(strProperty)) return Color.white;
            string[] props = node.GetAttribute(strProperty).Split(',');
            if (props == null || props.Length < 4) return Vector4.zero;
            Color result = Color.white;
            float temp = 0.0f;
            if (float.TryParse(props[0], out temp)) result.r = temp;
            if (float.TryParse(props[1], out temp)) result.g = temp;
            if (float.TryParse(props[2], out temp)) result.b = temp;
            if (float.TryParse(props[3], out temp)) result.a = temp;
            return result;
        }
        //-------------------------------------------
        public static Quaternion ReadQuaternion(XmlElement node, string strProperty)
        {
            if (string.IsNullOrEmpty(strProperty) || !node.HasAttribute(strProperty)) return Quaternion.identity;
            string[] props = node.GetAttribute(strProperty).Split(',');
            if (props == null || props.Length < 4) return Quaternion.identity;
            Quaternion result = Quaternion.identity;
            float temp = 0.0f;
            if (float.TryParse(props[0], out temp)) result.x = temp;
            if (float.TryParse(props[1], out temp)) result.y = temp;
            if (float.TryParse(props[2], out temp)) result.z = temp;
            if (float.TryParse(props[3], out temp)) result.w = temp;
            return result;
        }
        //-------------------------------------------
        public static Framework.Data.KeyValueParam ReadKV(List<string> vParams, int offset)
        {
            if (vParams == null || vParams.Count - offset < 2) return Framework.Data.KeyValueParam.DEF;
            var result = Framework.Data.KeyValueParam.DEF;
            result.key = vParams[offset + 0];
            result.value = vParams[offset + 1];
            return result;
        }
        //-------------------------------------------
        public static Vector2 ReadVec2(List<string> vParams, int offset)
        {
            if (vParams == null || vParams.Count - offset < 2) return Vector2.zero;
            Vector2 result = Vector2.zero;
            result.x = float.Parse(vParams[offset + 0]);
            result.y = float.Parse(vParams[offset + 1]);
            return result;
        }
        //-------------------------------------------
        public static Vector2Int ReadVec2Int(List<string> vParams, int offset)
        {
            if (vParams == null || vParams.Count - offset < 2) return Vector2Int.zero;
            Vector2Int result = Vector2Int.zero;
            result.x = int.Parse(vParams[offset + 0]);
            result.y = int.Parse(vParams[offset + 1]);
            return result;
        }
        //-------------------------------------------
        public static Vector3Int ReadVec3Int(List<string> vParams, int offset)
        {
            if (vParams == null || vParams.Count - offset < 3) return Vector3Int.zero;
            Vector3Int result = Vector3Int.zero;
            result.x = int.Parse(vParams[offset + 0]);
            result.y = int.Parse(vParams[offset + 1]);
            result.z = int.Parse(vParams[offset + 2]);
            return result;
        }
        //-------------------------------------------
        public static Vector3 ReadVec3(List<string> vParams, int offset)
        {
            if (vParams == null || vParams.Count - offset < 3) return Vector3.zero;
            Vector3 result = Vector3.zero;
            result.x = float.Parse(vParams[offset + 0]);
            result.y = float.Parse(vParams[offset + 1]);
            result.z = float.Parse(vParams[offset + 2]);
            return result;
        }
        //-------------------------------------------
        public static Vector4 ReadVec4(List<string> vParams, int offset)
        {
            if (vParams == null || vParams.Count - offset < 4) return Vector4.zero;
            Vector4 result = Vector4.zero;
            result.x = float.Parse(vParams[offset + 0]);
            result.y = float.Parse(vParams[offset + 1]);
            result.z = float.Parse(vParams[offset + 2]);
            result.w = float.Parse(vParams[offset + 3]);
            return result;
        }
        //-------------------------------------------
        public static Color ReadColor(List<string> vParams, int offset)
        {
            if (vParams == null || vParams.Count - offset < 4) return Color.white;
            Color result = Color.white;
            result.r = float.Parse(vParams[offset + 0]);
            result.g = float.Parse(vParams[offset + 1]);
            result.b = float.Parse(vParams[offset + 2]);
            result.a = float.Parse(vParams[offset + 3]);
            return result;
        }
        //-------------------------------------------
        public static Quaternion ReadQuaternion(List<string> vParams, int offset)
        {
            if (vParams == null || vParams.Count - offset < 4) return Quaternion.identity;
            Quaternion result = Quaternion.identity;
            result.x = float.Parse(vParams[offset + 0]);
            result.y = float.Parse(vParams[offset + 1]);
            result.z = float.Parse(vParams[offset + 2]);
            result.w = float.Parse(vParams[offset + 3]);
            return result;
        }
#if UNITY_EDITOR
        //-------------------------------------------
        public static string WriteKV(Framework.Data.KeyValueParam vec)
        {
            return string.Format("{0},{1}", vec.key, vec.value);
        }
        //-------------------------------------------
        public static string WriteVec2(Vector2 vec)
        {
            return string.Format("{0:F3},{1:F3}", vec.x, vec.y);
        }
        //-------------------------------------------
        public static string WriteVec2Int(Vector2Int vec)
        {
            return string.Format("{0},{1}", vec.x, vec.y);
        }
        //-------------------------------------------
        public static string WriteVec3Int(Vector3Int vec)
        {
            return string.Format("{0},{1},{2}", vec.x, vec.y, vec.z);
        }
        //-------------------------------------------
        public static string WriteVec3(Vector3 vec)
        {
            return string.Format("{0:F3},{1:F3},{2:F3}", vec.x, vec.y, vec.z);
        }
        //-------------------------------------------
        public static string WriteVec4(Vector4 vec)
        {
            return string.Format("{0:F3},{1:F3},{2:F3},{3:F3}", vec.x, vec.y, vec.z, vec.w);
        }
        //-------------------------------------------
        public static string WriteColor(Color vec)
        {
            return string.Format("{0},{1},{2},{3}", vec.r, vec.g, vec.b, vec.a);
        }
        //-------------------------------------------
        public static string WriteQuaternion(Quaternion vec)
        {
            return string.Format("{0:F3},{1:F3},{2:F3},{3:F3}", vec.x, vec.y, vec.z, vec.w);
        }
        //-------------------------------------------
        public static void WriteKV(Framework.Data.KeyValueParam vec, XmlElement node, string strLabel)
        {
            node.SetAttribute(strLabel, string.Format("{0},{1}", vec.key, vec.value));
        }
        //-------------------------------------------
        public static void WriteVec2(Vector2 vec, XmlElement node, string strLabel)
        {
            node.SetAttribute(strLabel, string.Format("{0:F3},{1:F3}", vec.x, vec.y));
        }
        //-------------------------------------------
        public static void WriteVec2Int(Vector2Int vec, XmlElement node, string strLabel)
        {
            node.SetAttribute(strLabel, string.Format("{0},{1}", vec.x, vec.y));
        }
        //-------------------------------------------
        public static void WriteVec3Int(Vector3Int vec, XmlElement node, string strLabel)
        {
            node.SetAttribute(strLabel, string.Format("{0},{1},{2}", vec.x, vec.y,vec.z));
        }
        //-------------------------------------------
        public static void WriteVec3(Vector3 vec, XmlElement node, string strLabel)
        {
            node.SetAttribute(strLabel, string.Format("{0:F3},{1:F3},{2:F3}", vec.x, vec.y, vec.z));
        }
        //-------------------------------------------
        public static void WriteVec4(Vector4 vec, XmlElement node, string strLabel)
        {
            node.SetAttribute(strLabel, string.Format("{0:F3},{1:F3},{2:F3},{3:F3}", vec.x, vec.y, vec.z, vec.w));
        }
        //-------------------------------------------
        public static void WriteColor(Color vec, XmlElement node, string strLabel)
        {
            node.SetAttribute(strLabel, string.Format("{0:F3},{1:F3},{2:F3},{3:F3}", vec.r, vec.g, vec.b, vec.a));
        }
        //-------------------------------------------
        public static void WriteQuaternion(Quaternion vec, XmlElement node, string strLabel)
        {
            node.SetAttribute(strLabel, string.Format("{0:F3},{1:F3},{2:F3},{3:F3}", vec.x, vec.y, vec.z, vec.w));
        }
#endif
        //-------------------------------------------

        #region SplineCurveData
        //-------------------------------------------
        public static string SaveSplineCurveData(Data.CurveData data, string split = "@")
        {
            string strParam = "";
            strParam += data.time.ToString() + split;
            strParam += data.position.x.ToString() + split;
            strParam += data.position.y.ToString() + split;
            strParam += data.position.z.ToString() + split;
            strParam += data.inTan.x.ToString() + split;
            strParam += data.inTan.y.ToString() + split;
            strParam += data.inTan.z.ToString() + split;
            strParam += data.outTan.x.ToString() + split;
            strParam += data.outTan.y.ToString() + split;
            strParam += data.outTan.z.ToString() + split;
            strParam += data.rotation.eulerAngles.x.ToString() + split;
            strParam += data.rotation.eulerAngles.y.ToString() + split;
            strParam += data.rotation.eulerAngles.z.ToString() + split;
            strParam += data.fov.ToString();
            return strParam;
        }
        //-------------------------------------------
        public static bool ReadSplineCurveData(string data, out Data.CurveData keyF, char split = '@')
        {
            keyF = new Data.CurveData() { position = Vector3.zero, rotation = Quaternion.identity, inTan = Vector3.zero, outTan = Vector3.zero };
            if (string.IsNullOrEmpty(data)) return false;
            string[] datas = data.Split(split);
            if (datas.Length == 14)
            {
                keyF.time = float.Parse(datas[0]);
                keyF.position.x = float.Parse(datas[1]);
                keyF.position.y = float.Parse(datas[2]);
                keyF.position.z = float.Parse(datas[3]);
                keyF.inTan.x = float.Parse(datas[4]);
                keyF.inTan.y = float.Parse(datas[5]);
                keyF.inTan.z = float.Parse(datas[6]);
                keyF.outTan.x = float.Parse(datas[7]);
                keyF.outTan.y = float.Parse(datas[8]);
                keyF.outTan.z = float.Parse(datas[9]);
                Vector3 eulerAngle = Vector3.zero;
                eulerAngle.x = float.Parse(datas[10]);
                eulerAngle.y = float.Parse(datas[11]);
                eulerAngle.z = float.Parse(datas[12]);
                keyF.rotation = Quaternion.Euler(eulerAngle);
                keyF.fov = float.Parse(datas[13]);
                return true;
            }
            return false;
        }
        //-------------------------------------------
        public static void SaveSplineCurveData(XmlDocument pDoc, ref XmlElement ele, Data.CurveData property)
        {
            XmlElement frams = pDoc.CreateElement("Frames");
            frams.SetAttribute("time", property.time.ToString());
            frams.SetAttribute("positionX", property.position.x.ToString());
            frams.SetAttribute("positionY", property.position.y.ToString());
            frams.SetAttribute("positionZ", property.position.z.ToString());
            frams.SetAttribute("inTanX", property.inTan.x.ToString());
            frams.SetAttribute("inTanY", property.inTan.y.ToString());
            frams.SetAttribute("inTanZ", property.inTan.z.ToString());
            frams.SetAttribute("outTanX", property.outTan.x.ToString());
            frams.SetAttribute("outTanY", property.outTan.y.ToString());
            frams.SetAttribute("outTanZ", property.outTan.z.ToString());
            frams.SetAttribute("eulerAngleX", property.rotation.eulerAngles.x.ToString());
            frams.SetAttribute("eulerAngleY", property.rotation.eulerAngles.y.ToString());
            frams.SetAttribute("eulerAngleZ", property.rotation.eulerAngles.z.ToString());
            frams.SetAttribute("fov", property.fov.ToString());
            ele.AppendChild(frams);
        }
        //-------------------------------------------
        public static bool ReadSplineCurveData(XmlElement childKey, out Data.CurveData keyF)
        {
            keyF = new Data.CurveData() { position = Vector3.zero, rotation = Quaternion.identity, inTan = Vector3.zero, outTan = Vector3.zero };
            keyF.time = float.Parse(childKey.GetAttribute("time"));
            keyF.position.x = float.Parse(childKey.GetAttribute("positionX"));
            keyF.position.y = float.Parse(childKey.GetAttribute("positionY"));
            keyF.position.z = float.Parse(childKey.GetAttribute("positionZ"));
            keyF.inTan.x = float.Parse(childKey.GetAttribute("inTanX"));
            keyF.inTan.y = float.Parse(childKey.GetAttribute("inTanY"));
            keyF.inTan.z = float.Parse(childKey.GetAttribute("inTanZ"));
            keyF.outTan.x = float.Parse(childKey.GetAttribute("outTanX"));
            keyF.outTan.y = float.Parse(childKey.GetAttribute("outTanY"));
            keyF.outTan.z = float.Parse(childKey.GetAttribute("outTanZ"));
            Vector3 eulerAngle = Vector3.zero;
            eulerAngle.x = float.Parse(childKey.GetAttribute("eulerAngleX"));
            eulerAngle.y = float.Parse(childKey.GetAttribute("eulerAngleY"));
            eulerAngle.z = float.Parse(childKey.GetAttribute("eulerAngleZ"));
            keyF.rotation = Quaternion.Euler(eulerAngle);
            keyF.fov = float.Parse(childKey.GetAttribute("fov"));
            return true;
        }
        //-------------------------------------------
        public static void SaveSplineCurveData(ref System.IO.BinaryWriter writer, Data.CurveData property)
        {
            writer.Write(property.time);
            writer.Write(property.position.x);
            writer.Write(property.position.y);
            writer.Write(property.position.z);
            writer.Write(property.inTan.x);
            writer.Write(property.inTan.y);
            writer.Write(property.inTan.z);
            writer.Write(property.outTan.x);
            writer.Write(property.outTan.y);
            writer.Write(property.outTan.z);
            writer.Write(property.rotation.eulerAngles.x);
            writer.Write(property.rotation.eulerAngles.y);
            writer.Write(property.rotation.eulerAngles.z);
            writer.Write(property.fov);
        }
        //-------------------------------------------
        public static bool ReadSplineCurveData(ref System.IO.BinaryReader reader, out Data.CurveData keyF)
        {
            keyF = new Data.CurveData() { position = Vector3.zero, rotation = Quaternion.identity, inTan = Vector3.zero, outTan = Vector3.zero };
            keyF.time = reader.ReadSingle();
            keyF.position.x = reader.ReadSingle();
            keyF.position.y = reader.ReadSingle();
            keyF.position.z = reader.ReadSingle();
            keyF.inTan.x = reader.ReadSingle();
            keyF.inTan.y = reader.ReadSingle();
            keyF.inTan.z = reader.ReadSingle();
            keyF.outTan.x = reader.ReadSingle();
            keyF.outTan.y = reader.ReadSingle();
            keyF.outTan.z = reader.ReadSingle();
            Vector3 eulerAngle = Vector3.zero;
            eulerAngle.x = reader.ReadSingle();
            eulerAngle.y = reader.ReadSingle();
            eulerAngle.z = reader.ReadSingle();
            keyF.rotation = Quaternion.Euler(eulerAngle);
            keyF.fov = reader.ReadSingle();
            return true;
        }
#endregion
        //-------------------------------------------

#region ActionProperty
        //-------------------------------------------
        public static string SaveActionProperty(PhysicPropertyData property, string split = "/")
        {
            string strParam = "";
            strParam += (property.bUseFrame ? "1" : "0") + split;
            strParam += (property.bUseEnd ? "1" : "0") + split;
            strParam += SaveSplineTracker(property.Frames) + split;

            strParam += (property.physic.bUseHorSpeed ? "1" : "0") + split;
            strParam += property.physic.fHorSpeed.ToString() + split;
            strParam += property.physic.fToHorSpeed.ToString() + split;

            strParam += (property.physic.bUseVerSpeed ? "1" : "0") + split;
            strParam += property.physic.fVerSpeed.ToString() + split;
            strParam += property.physic.fToVerSpeed.ToString() + split;

            strParam += (property.physic.bUseDeepSpeed ? "1" : "0") + split;
            strParam += property.physic.fDeepSpeed.ToString() + split;
            strParam += property.physic.fToDeepSpeed.ToString() + split;

            strParam += (property.physic.bUseGravity ? "1" : "0") + split;
            strParam += property.physic.fGravity.ToString() + split;
            strParam += property.physic.fToGravity.ToString() + split;

            strParam += (property.physic.bUseFraction ? "1" : "0") + split;
            strParam += property.physic.fFraction.ToString() + split;
            strParam += property.physic.fToFraction.ToString() + split;
            strParam += property.propertyFlags;
            return strParam;
        }
        //-------------------------------------------
        public static bool ReadActionProperty(string strCmd, out PhysicPropertyData property, char strPlit='/')
        {
            property = new PhysicPropertyData();
            if (string.IsNullOrEmpty(strCmd)) return false;
            string[] vDatas = strCmd.Split(strPlit);
            int offset = 0;
            if (vDatas.Length <= 0) return false;
            property.bUseFrame = vDatas[offset].CompareTo("1") == 0; offset++;

            if (vDatas.Length <= offset) return false;
            property.bUseEnd = vDatas[offset].CompareTo("1") == 0; offset++;

            if (vDatas.Length <= offset) return false;
            property.Frames = EventHelper.ReadSplineTracker(vDatas[offset]); offset++;

            if (vDatas.Length <= offset) return false;
            property.physic.bUseHorSpeed = vDatas[offset].CompareTo("1") == 0; offset++;

            if (vDatas.Length <= offset) return false;
            float.TryParse(vDatas[offset], out property.physic.fHorSpeed); offset++;

            if (vDatas.Length <= offset) return false;
            float.TryParse(vDatas[offset], out property.physic.fToHorSpeed); offset++;

            if (vDatas.Length <= offset) return false;
            property.physic.bUseVerSpeed = vDatas[offset].CompareTo("1") == 0; offset++;

            if (vDatas.Length <= offset) return false;
            float.TryParse(vDatas[offset], out property.physic.fVerSpeed); offset++;

            if (vDatas.Length <= offset) return false;
            float.TryParse(vDatas[offset], out property.physic.fToVerSpeed); offset++;

            if (vDatas.Length <= offset) return false;
            property.physic.bUseDeepSpeed = vDatas[offset].CompareTo("1") == 0; offset++;

            if (vDatas.Length <= offset) return false;
            float.TryParse(vDatas[offset], out property.physic.fDeepSpeed); offset++;

            if (vDatas.Length <= offset) return false;
            float.TryParse(vDatas[offset], out property.physic.fToDeepSpeed); offset++;

            if (vDatas.Length <= offset) return false;
            property.physic.bUseGravity = vDatas[offset].CompareTo("1") == 0; offset++;

            if (vDatas.Length <= offset) return false;
            float.TryParse(vDatas[offset], out property.physic.fGravity); offset++;

            if (vDatas.Length <= offset) return false;
            float.TryParse(vDatas[offset], out property.physic.fToGravity); offset++;

            if (vDatas.Length <= offset) return false;
            property.physic.bUseFraction = vDatas[offset].CompareTo("1") == 0; offset++;

            if (vDatas.Length <= offset) return false;
            float.TryParse(vDatas[offset], out property.physic.fFraction); offset++;

            if (vDatas.Length <= offset) return false;
            float.TryParse(vDatas[offset], out property.physic.fToFraction); offset++;

            property.propertyFlags = 0;
            if (vDatas.Length <= offset) return true;
            byte.TryParse(vDatas[offset], out property.propertyFlags); offset++;
            return true;
        }
        //-------------------------------------------
        public static void SaveActionProperty(XmlDocument pDoc, ref XmlElement ele, PhysicPropertyData property)
        {
            ele.SetAttribute("bUseFrame", property.bUseFrame ? "1" : "0");
            ele.SetAttribute("bUseEnd", property.bUseEnd ? "1" : "0");
            ele.SetAttribute("propertyFlags", property.propertyFlags.ToString());
            if (property.bUseFrame)
                EventHelper.SaveSplineTracker(property.Frames, pDoc, ele, "Frames");
            else
            {
                ele.SetAttribute("bUseHorSpeed", property.physic.bUseHorSpeed ? "1" : "0");
                if (property.physic.bUseHorSpeed)
                {
                    ele.SetAttribute("fHorSpeed", property.physic.fHorSpeed.ToString());
                    ele.SetAttribute("fToHorSpeed", property.physic.fToHorSpeed.ToString());
                }
                ele.SetAttribute("bUseVerSpeed", property.physic.bUseVerSpeed ? "1" : "0");
                if (property.physic.bUseVerSpeed)
                {
                    ele.SetAttribute("fVerSpeed", property.physic.fVerSpeed.ToString());
                    ele.SetAttribute("fToVerSpeed", property.physic.fToVerSpeed.ToString());
                }
                ele.SetAttribute("bUseDeepSpeed", property.physic.bUseDeepSpeed ? "1" : "0");
                if (property.physic.bUseDeepSpeed)
                {
                    ele.SetAttribute("fDeepSpeed", property.physic.fDeepSpeed.ToString());
                    ele.SetAttribute("fToDeepSpeed", property.physic.fToDeepSpeed.ToString());
                }
                ele.SetAttribute("bUseGravity", property.physic.bUseGravity ? "1" : "0");
                if (property.physic.bUseGravity)
                {
                    ele.SetAttribute("fGravity", property.physic.fGravity.ToString());
                    ele.SetAttribute("fToGravity", property.physic.fToGravity.ToString());
                }
                ele.SetAttribute("bUseFraction", property.physic.bUseFraction ? "1" : "0");
                if (property.physic.bUseFraction)
                {
                    ele.SetAttribute("fFraction", property.physic.fFraction.ToString());
                    ele.SetAttribute("fToFraction", property.physic.fToFraction.ToString());
                }
            }
        }
        //-------------------------------------------
        public static bool ReadActionProperty(ref XmlElement ele, out PhysicPropertyData property)
        {
            property = new PhysicPropertyData();
            if (ele.HasAttribute("bUseFrame"))
            {
                property.bUseFrame = ele.GetAttribute("bUseFrame").CompareTo("1") == 0;
            }
            if (ele.HasAttribute("bUseEnd"))
            {
                property.bUseEnd = ele.GetAttribute("bUseEnd").CompareTo("1") == 0;
            }
            property.propertyFlags = 0;
            if (ele.HasAttribute("propertyFlags"))
            {
                byte.TryParse(ele.GetAttribute("propertyFlags"), out property.propertyFlags);
            }
            if (property.bUseFrame)
            {
                property.Frames = EventHelper.ReadSplineTracker(ele);
            }
            else
            {
                property.physic.bUseHorSpeed = ele.GetAttribute("bUseHorSpeed").CompareTo("1") == 0;
                if (property.physic.bUseHorSpeed)
                {
                    float.TryParse(ele.GetAttribute("fHorSpeed"), out property.physic.fHorSpeed);
                    float.TryParse(ele.GetAttribute("fToHorSpeed"), out property.physic.fToHorSpeed);
                }

                property.physic.bUseVerSpeed = ele.GetAttribute("bUseVerSpeed").CompareTo("1") == 0;
                if (property.physic.bUseVerSpeed)
                {
                    float.TryParse(ele.GetAttribute("fVerSpeed"), out property.physic.fVerSpeed);
                    float.TryParse(ele.GetAttribute("fToVerSpeed"), out property.physic.fToVerSpeed);
                }

                property.physic.bUseDeepSpeed = ele.GetAttribute("bUseDeepSpeed").CompareTo("1") == 0;
                if (property.physic.bUseDeepSpeed)
                {
                    float.TryParse(ele.GetAttribute("fDeepSpeed"), out property.physic.fDeepSpeed);
                    float.TryParse(ele.GetAttribute("fToDeepSpeed"), out property.physic.fToDeepSpeed);
                }

                property.physic.bUseGravity = ele.GetAttribute("bUseGravity").CompareTo("1") == 0;
                if (property.physic.bUseGravity)
                {
                    float.TryParse(ele.GetAttribute("fGravity"), out property.physic.fGravity);
                    float.TryParse(ele.GetAttribute("fToGravity"), out property.physic.fToGravity);
                }
                property.physic.bUseFraction = ele.GetAttribute("bUseFraction").CompareTo("1") == 0;
                if (property.physic.bUseFraction)
                {
                    float.TryParse(ele.GetAttribute("fFraction"), out property.physic.fFraction);
                    float.TryParse(ele.GetAttribute("fToFraction"), out property.physic.fToFraction);
                }
            }
            return true;
        }
        //-------------------------------------------
        public static void SaveActionProperty(ref System.IO.BinaryWriter writer, PhysicPropertyData property)
        {
            writer.Write(property.bUseFrame);
            writer.Write(property.bUseEnd);
            writer.Write(property.propertyFlags);
            EventHelper.SaveSplineTracker(property.Frames, writer);
            writer.Write(property.physic.bUseHorSpeed);
            writer.Write(property.physic.fHorSpeed);
            writer.Write(property.physic.fToHorSpeed);

            writer.Write(property.physic.bUseVerSpeed);
            writer.Write(property.physic.fVerSpeed);
            writer.Write(property.physic.fToVerSpeed);

            writer.Write(property.physic.bUseDeepSpeed);
            writer.Write(property.physic.fDeepSpeed);
            writer.Write(property.physic.fToDeepSpeed);

            writer.Write(property.physic.bUseGravity);
            writer.Write(property.physic.fGravity);
            writer.Write(property.physic.fToGravity);

            writer.Write(property.physic.bUseFraction);
            writer.Write(property.physic.fFraction);
            writer.Write(property.physic.fToFraction);
        }
        //-------------------------------------------
        public static bool ReadActionProperty(ref System.IO.BinaryReader reader, out PhysicPropertyData property)
        {
            property = new PhysicPropertyData();
            property.bUseFrame = reader.ReadBoolean();
            property.bUseEnd = reader.ReadBoolean();
            property.propertyFlags = reader.ReadByte();
            property.Frames = EventHelper.ReadSplineTracker(reader);
            property.physic.bUseHorSpeed = reader.ReadBoolean();
            property.physic.fHorSpeed = reader.ReadSingle();
            property.physic.fToHorSpeed = reader.ReadSingle();

            property.physic.bUseVerSpeed = reader.ReadBoolean();
            property.physic.fVerSpeed = reader.ReadSingle();
            property.physic.fToVerSpeed = reader.ReadSingle();

            property.physic.bUseDeepSpeed = reader.ReadBoolean();
            property.physic.fDeepSpeed = reader.ReadSingle();
            property.physic.fToDeepSpeed = reader.ReadSingle();

            property.physic.bUseGravity = reader.ReadBoolean();
            property.physic.fGravity = reader.ReadSingle();
            property.physic.fToGravity = reader.ReadSingle();

            property.physic.bUseFraction = reader.ReadBoolean();
            property.physic.fFraction = reader.ReadSingle();
            property.physic.fToFraction = reader.ReadSingle();
            return true;
        }
#endregion
        //-------------------------------------------

#region SplineTracker
        //-------------------------------------------
        public static string SaveSplineTracker(SplineData.KeyFrame[] keyFrames)
        {
            string strRet = "";
            if (keyFrames == null)
            {
                return strRet;
            }
            for (int i = 0; i < keyFrames.Length; ++i)
            {
                strRet += keyFrames[i].time.ToString() + "@";
                strRet += keyFrames[i].position.x.ToString() + "@";
                strRet += keyFrames[i].position.y.ToString() + "@";
                strRet += keyFrames[i].position.z.ToString() + "@";
                strRet += keyFrames[i].eulerAngle.x.ToString() + "@";
                strRet += keyFrames[i].eulerAngle.y.ToString() + "@";
                strRet += keyFrames[i].eulerAngle.z.ToString() + "@";
                strRet += keyFrames[i].scale.x.ToString() + "@";
                strRet += keyFrames[i].scale.y.ToString() + "@";
                strRet += keyFrames[i].scale.z.ToString() + "@";
                strRet += keyFrames[i].inTan.x.ToString() + "@";
                strRet += keyFrames[i].inTan.y.ToString() + "@";
                strRet += keyFrames[i].inTan.z.ToString() + "@";
                strRet += keyFrames[i].outTan.x.ToString() + "@";
                strRet += keyFrames[i].outTan.y.ToString() + "@";
                strRet += keyFrames[i].outTan.z.ToString();
                if (i < keyFrames.Length - 1)
                    strRet += "|";
            }
            return strRet;
        }

        //-------------------------------------------
        public static void SaveSplineTracker(SplineData.KeyFrame[] keyFrames, XmlDocument pDoc, XmlElement node, string strLabel = "Keys", char split = '@')
        {
            if (keyFrames == null || keyFrames.Length <= 0) return;
            XmlElement keysNode = pDoc.CreateElement(strLabel);
            for (int i = 0; i < keyFrames.Length; ++i)
            {
                XmlElement sub = pDoc.CreateElement("Key");
                sub.SetAttribute("time", keyFrames[i].time.ToString());
                sub.SetAttribute("positionX", keyFrames[i].position.x.ToString());
                sub.SetAttribute("positionY", keyFrames[i].position.y.ToString());
                sub.SetAttribute("positionZ", keyFrames[i].position.z.ToString());
                sub.SetAttribute("eulerAngleX", keyFrames[i].eulerAngle.x.ToString());
                sub.SetAttribute("eulerAngleY", keyFrames[i].eulerAngle.y.ToString());
                sub.SetAttribute("eulerAngleZ", keyFrames[i].eulerAngle.z.ToString());
                sub.SetAttribute("scaleX", keyFrames[i].scale.x.ToString());
                sub.SetAttribute("scaleY", keyFrames[i].scale.y.ToString());
                sub.SetAttribute("scaleZ", keyFrames[i].scale.z.ToString());
                sub.SetAttribute("inTanX", keyFrames[i].inTan.x.ToString());
                sub.SetAttribute("inTanY", keyFrames[i].inTan.y.ToString());
                sub.SetAttribute("inTanZ", keyFrames[i].inTan.z.ToString());
                sub.SetAttribute("outTanX", keyFrames[i].outTan.x.ToString());
                sub.SetAttribute("outTanY", keyFrames[i].outTan.y.ToString());
                sub.SetAttribute("outTanZ", keyFrames[i].outTan.z.ToString());
                keysNode.AppendChild(sub);
            }
            node.AppendChild(keysNode);
        }
        //-------------------------------------------
        public static void SaveSplineTracker(SplineData.KeyFrame[] keyFrames, System.IO.BinaryWriter writer)
        {
            if (keyFrames == null) writer.Write((ushort)0);
            else
            {
                writer.Write((ushort)keyFrames.Length);
                for (int i = 0; i < keyFrames.Length; ++i)
                {
                    writer.Write(keyFrames[i].time);
                    writer.Write(keyFrames[i].position.x);
                    writer.Write(keyFrames[i].position.y);
                    writer.Write(keyFrames[i].position.z);
                    writer.Write(keyFrames[i].eulerAngle.x);
                    writer.Write(keyFrames[i].eulerAngle.y);
                    writer.Write(keyFrames[i].eulerAngle.z);
                    writer.Write(keyFrames[i].scale.x);
                    writer.Write(keyFrames[i].scale.y);
                    writer.Write(keyFrames[i].scale.z);
                    writer.Write(keyFrames[i].inTan.x);
                    writer.Write(keyFrames[i].inTan.y);
                    writer.Write(keyFrames[i].inTan.z);
                    writer.Write(keyFrames[i].outTan.x);
                    writer.Write(keyFrames[i].outTan.y);
                    writer.Write(keyFrames[i].outTan.z);
                }
            }
        }
        //-------------------------------------------
        public static SplineData.KeyFrame[] ReadSplineTracker(string strContext)
        {
            if (string.IsNullOrEmpty(strContext)) return null;
            string[] values = strContext.Split('|');
            if (values == null || values.Length <= 0) return null;
            SplineData.KeyFrame[] frames = new SplineData.KeyFrame[values.Length];
            for (int i = 0; i < frames.Length; ++i)
            {
                string[] sub = values[i].Split('@');
                if (sub.Length != 13) return null;

                SplineData.KeyFrame frame = new SplineData.KeyFrame() { position = Vector3.zero, eulerAngle = Vector3.zero, inTan = Vector3.zero, outTan = Vector3.zero };
                float.TryParse(sub[0], out frame.time);

                float.TryParse(sub[1], out frame.position.x);
                float.TryParse(sub[2], out frame.position.y);
                float.TryParse(sub[3], out frame.position.z);

                float.TryParse(sub[4], out frame.eulerAngle.x);
                float.TryParse(sub[5], out frame.eulerAngle.y);
                float.TryParse(sub[6], out frame.eulerAngle.z);

                float.TryParse(sub[7], out frame.scale.x);
                float.TryParse(sub[8], out frame.scale.y);
                float.TryParse(sub[9], out frame.scale.z);

                float.TryParse(sub[10], out frame.inTan.x);
                float.TryParse(sub[11], out frame.inTan.y);
                float.TryParse(sub[12], out frame.inTan.z);

                float.TryParse(sub[13], out frame.outTan.x);
                float.TryParse(sub[14], out frame.outTan.y);
                float.TryParse(sub[15], out frame.outTan.z);

                frames[i] = frame;
            }
            return frames;
        }
        //-------------------------------------------
        public static SplineData.KeyFrame[] ReadSplineTracker(XmlElement node, string strLabel = "Keys")
        {
            foreach (XmlElement childKey in node)
            {
                if (childKey.Name.CompareTo(strLabel) == 0)
                {
                    SplineData.KeyFrame[] curve = new SplineData.KeyFrame[childKey.ChildNodes.Count];
                    int index = 0;
                    foreach (XmlElement keyNode in childKey)
                    {
                        SplineData.KeyFrame frame = new SplineData.KeyFrame() { position = Vector3.zero, eulerAngle = Vector3.zero, inTan = Vector3.zero, outTan = Vector3.zero };
                        float.TryParse(keyNode.GetAttribute("time"), out frame.time);
                        float.TryParse(keyNode.GetAttribute("positionX"), out frame.position.x);
                        float.TryParse(keyNode.GetAttribute("positionY"), out frame.position.y);
                        float.TryParse(keyNode.GetAttribute("positionZ"), out frame.position.z);
                        float.TryParse(keyNode.GetAttribute("eulerAngleX"), out frame.eulerAngle.x);
                        float.TryParse(keyNode.GetAttribute("eulerAngleY"), out frame.eulerAngle.y);
                        float.TryParse(keyNode.GetAttribute("eulerAngleZ"), out frame.eulerAngle.z);
                        float.TryParse(keyNode.GetAttribute("eulerAngleX"), out frame.scale.x);
                        float.TryParse(keyNode.GetAttribute("scaleY"), out frame.scale.y);
                        float.TryParse(keyNode.GetAttribute("scaleZ"), out frame.scale.z);
                        float.TryParse(keyNode.GetAttribute("inTanX"), out frame.inTan.x);
                        float.TryParse(keyNode.GetAttribute("inTanY"), out frame.inTan.y);
                        float.TryParse(keyNode.GetAttribute("inTanZ"), out frame.inTan.z);
                        float.TryParse(keyNode.GetAttribute("outTanX"), out frame.outTan.x);
                        float.TryParse(keyNode.GetAttribute("outTanY"), out frame.outTan.y);
                        float.TryParse(keyNode.GetAttribute("outTanZ"), out frame.outTan.z);
                        curve[index] = frame;
                    }
                    return curve;
                }
            }
            return null;
        }
        //-------------------------------------------
        public static SplineData.KeyFrame[] ReadSplineTracker(System.IO.BinaryReader reader, bool bCheck = true)
        {
            int frameCnt = reader.ReadUInt16();
            if (frameCnt == 0) return null;
            SplineData.KeyFrame[] frames = new SplineData.KeyFrame[frameCnt];
            int size = 13;
            int index = 0;
            for (int i = 0; i < frameCnt; i += size)
            {
                SplineData.KeyFrame frame = new SplineData.KeyFrame() { position = Vector3.zero, eulerAngle = Vector3.zero, scale = Vector3.one, inTan = Vector3.zero, outTan = Vector3.zero };
                frame.time = reader.ReadSingle();
                frame.position.x = reader.ReadSingle();
                frame.position.y = reader.ReadSingle();
                frame.position.z = reader.ReadSingle();
                frame.eulerAngle.x = reader.ReadSingle();
                frame.eulerAngle.y = reader.ReadSingle();
                frame.eulerAngle.z = reader.ReadSingle();
                frame.scale.x = reader.ReadSingle();
                frame.scale.y = reader.ReadSingle();
                frame.scale.z = reader.ReadSingle();
                frame.inTan.x = reader.ReadSingle();
                frame.inTan.y = reader.ReadSingle();
                frame.inTan.z = reader.ReadSingle();
                frame.outTan.x = reader.ReadSingle();
                frame.outTan.y = reader.ReadSingle();
                frame.outTan.z = reader.ReadSingle();

                frames[index] = frame;
                index++;
            }
            return frames;
        }
#endregion
        //-------------------------------------------
        
#region AnimationCurved
        //-------------------------------------------
        public static string SaveCurve(DeclareKeyFrame keyFrames, char head = '-')
        {
            string strRet = "";
            int totalSize = DeclareKit.CalcTotalSize(keyFrames.bits);
            if(keyFrames.frames == null)
            {
                return strRet;
            }
            for(int i = 0; i < keyFrames.frames.Count; ++i )
            {
                if (keyFrames.frames[i] == null || keyFrames.frames[i].Length != totalSize)
                    return strRet;
            }
            strRet += keyFrames.bits.ToString() + head;
            for (int i = 0; i < keyFrames.frames.Count; ++i)
            {
                for (int j = 0; j < keyFrames.frames[i].Length; ++j)
                {
                    strRet += keyFrames.frames[i][j];
                    if (j < keyFrames.frames[i].Length - 1) strRet += "@";
                }
                if (i < keyFrames.frames.Count - 1)
                    strRet += "|";
            }
            return strRet;
        }
        //-------------------------------------------
        public static string SaveCurve(AnimationCurve curve)
        {
            if (curve == null || curve.length<=0) return "";
            string strRet = ((((int)curve.postWrapMode)<<16) |(int)curve.preWrapMode).ToString() + "#";
            for (int i = 0; i < curve.length; ++i)
            {
                strRet += curve[i].time + "@" + curve[i].value + "@" + curve[i].inTangent + "@" + curve[i].inWeight + "@" + curve[i].outTangent + "@" + curve[i].outWeight + "@" + ((int)curve[i].weightedMode);
                if (i < strRet.Length - 1)
                    strRet += "|";
            }
            return strRet;
        }
        //-------------------------------------------
        public static void SaveCurve(AnimationCurve curve, XmlDocument pDoc, XmlElement node, string strLabel="Keys")
        {
            if (curve == null || curve.length <= 0) return;
            XmlElement keysNode = pDoc.CreateElement(strLabel);
            keysNode.SetAttribute("postWrapMode", ((int)curve.postWrapMode).ToString());
            keysNode.SetAttribute("preWrapMode", ((int)curve.preWrapMode).ToString());
            for (int i = 0; i < curve.length; ++i)
            {
                XmlElement sub = pDoc.CreateElement("Key");
                sub.SetAttribute("time", curve[i].time.ToString());
                sub.SetAttribute("value", curve[i].value.ToString());
                sub.SetAttribute("inTangent", curve[i].inTangent.ToString());
                sub.SetAttribute("inWeight", curve[i].inWeight.ToString());
                sub.SetAttribute("outTangent", curve[i].outTangent.ToString());
                sub.SetAttribute("outWeight", curve[i].outWeight.ToString());
                sub.SetAttribute("weightedMode", ((int)curve[i].weightedMode).ToString());
                keysNode.AppendChild(sub);
            }
            node.AppendChild(keysNode);
        }
        //-------------------------------------------
        public static void SaveCurve(DeclareKeyFrame curve, XmlDocument pDoc, XmlElement node, string strLabel = "Keys", char split='@')
        {
            if (curve.isValid) return;
            XmlElement keysNode = pDoc.CreateElement(strLabel);
            keysNode.SetAttribute("head", curve.bits.ToString());
            int totalSize = curve.totalSize;
            for (int i = 0; i < curve.frames.Count; ++i)
            {
                if (totalSize != curve.frames[i].Length) continue;
                XmlElement sub = pDoc.CreateElement("Key");
                string strParam = "";
                for(int j=0; j < curve.frames[i].Length; ++j)
                {
                    strParam += curve.frames[i][j].ToString();
                    if (j < curve.frames[i].Length - 1) strParam += split;
                }
                sub.SetAttribute("params", strParam);
                keysNode.AppendChild(sub);
            }
            node.AppendChild(keysNode);
        }
        //-------------------------------------------
        public static Framework.Data.KeyValueParam ReadKV(string strData)
        {
            if (string.IsNullOrEmpty(strData)) return Framework.Data.KeyValueParam.DEF;
            string[] keys = strData.Split('|');
            if (keys.Length < 2) return Framework.Data.KeyValueParam.DEF;

            var result = Framework.Data.KeyValueParam.DEF;
            result.key = keys[0];
            result.value = keys[1];
            return result;
        }
        //-------------------------------------------
        public static string SaveKV(Framework.Data.KeyValueParam kv)
        {
            if (kv.IsValid) return "";
            return string.Format("{0}|{1}", kv.key, kv.value);
        }
        //-------------------------------------------
        public static void SaveCurve(AnimationCurve curve, System.IO.BinaryWriter writer)
        {
            if (curve == null) writer.Write((ushort)0);
            else
            {
                writer.Write((ushort)curve.length);
                writer.Write((byte)curve.postWrapMode);
                writer.Write((byte)curve.preWrapMode);
                for (int i = 0; i < curve.length; ++i)
                {
                    writer.Write(curve.keys[i].time);
                    writer.Write(curve.keys[i].value);
                    writer.Write(curve.keys[i].inTangent);
                    writer.Write(curve.keys[i].inWeight);
                    writer.Write(curve.keys[i].outTangent);
                    writer.Write(curve.keys[i].outWeight);
                    writer.Write((byte)curve.keys[i].weightedMode);
                }
            }
        }
        //-------------------------------------------
        public static void SaveCurve(DeclareKeyFrame curve, System.IO.BinaryWriter writer)
        {
            if (!curve.isValid) writer.Write((int)0);
            else
            {
                int totalSize = curve.totalSize;
                writer.Write(curve.bits);
                ushort validCnt = 0;
                for (int i = 0; i < curve.frames.Count; ++i)
                {
                    if (curve.frames[i].Length != totalSize) continue;
                    validCnt++;
                }
                writer.Write(validCnt);
                if(validCnt>0)
                {
                    for (int i = 0; i < curve.frames.Count; ++i)
                    {
                        if (curve.frames[i].Length != totalSize) continue;
                        for (int j = 0; j < curve.frames[i].Length; ++j)
                            writer.Write(curve.frames[i][j]);
                    }
                }
            }
        }
        //-------------------------------------------
        public static DeclareKeyFrame ReadCurve(string strData, char head='-')
        {
            if (string.IsNullOrEmpty(strData)) return DeclareKeyFrame.DEFAULT;
            string[] keys = strData.Split(head);
            if (keys.Length !=2) return DeclareKeyFrame.DEFAULT;
            int bits = 0;
            if(!int.TryParse(keys[0], out bits))
                return DeclareKeyFrame.DEFAULT;

            DeclareKeyFrame curve = new DeclareKeyFrame();
            curve.bits = bits;
            int totalSize = DeclareKit.CalcTotalSize(bits);
            keys = strData.Split('|');
            for (int i = 0; i < keys.Length; ++i)
            {
                string[] data = keys[i].Split('@');
                if (data.Length != totalSize) continue;
                int[] datas = new int[totalSize];
                for(int j=0; j < data.Length; ++j)
                {
                    datas[j] = int.Parse(data[0]);
                }
                curve.frames.Add(datas);
            }
            return curve;
        }
        //-------------------------------------------
        public static AnimationCurve ReadCurve(string strData)
        {
            if (string.IsNullOrEmpty(strData)) return null;
            string[] keys = strData.Split('|');
            if (keys.Length <= 0) return null;

            AnimationCurve curve = new AnimationCurve();
            int index = keys[0].IndexOf('#');
            if (index > 0)
            {
                string warpModeKey = keys[0].Substring(0, index);
                int warpMode;
                if(int.TryParse(warpModeKey, out warpMode))
                {
                    curve.postWrapMode = (WrapMode)(warpMode >> 16);
                    curve.preWrapMode = (WrapMode)(warpMode&0xffff);
                }
                keys[0] = keys[0].Substring(index+1);
            }
            for (int i = 0; i < keys.Length; ++i)
            {
                string[] data = keys[i].Split('@');
                if (data.Length != 7) continue;
                Keyframe key = new Keyframe();
                key.time = float.Parse(data[0]);
                key.value = float.Parse(data[1]);
                key.inTangent = float.Parse(data[2]);
                key.inWeight = float.Parse(data[3]);
                key.outTangent = float.Parse(data[4]);
                key.outWeight = float.Parse(data[5]);
                key.weightedMode = (WeightedMode)int.Parse(data[6]);
                curve.AddKey(key);
            }
            return curve;
        }
        //-------------------------------------------
        public static AnimationCurve ReadCurve(XmlElement childKey)
        {
            AnimationCurve curve = new AnimationCurve();
            int temp;
            if (childKey.HasAttribute("postWrapMode") && int.TryParse(childKey.GetAttribute("postWrapMode"), out temp)) curve.postWrapMode = (WrapMode)temp;
            if (childKey.HasAttribute("preWrapMode") && int.TryParse(childKey.GetAttribute("preWrapMode"), out temp)) curve.preWrapMode = (WrapMode)temp;

            foreach (XmlElement keyNode in childKey)
            {
                Keyframe key = new Keyframe();
                key.time = float.Parse(keyNode.GetAttribute("time"));
                key.inTangent = float.Parse(keyNode.GetAttribute("inTangent"));
                key.inWeight = float.Parse(keyNode.GetAttribute("inWeight"));
                key.outTangent = float.Parse(keyNode.GetAttribute("outTangent"));
                key.outWeight = float.Parse(keyNode.GetAttribute("outWeight"));
                key.weightedMode = (WeightedMode)int.Parse(keyNode.GetAttribute("weightedMode"));
                curve.AddKey(key);
            }
            return curve;
        }
        //-------------------------------------------
        public static AnimationCurve ReadCurve(XmlElement node, string strLabel)
        {
            foreach (XmlElement childKey in node)
            {
                if(childKey.Name.CompareTo(strLabel) == 0)
                {
                    return ReadCurve(childKey);
                }
            }
            return null;
        }
        //-------------------------------------------
        public static DeclareKeyFrame ReadDeclareKeyFrame(XmlElement childKey, char split = '@')
        {
            DeclareKeyFrame curve = new DeclareKeyFrame();
            string headLabel = childKey.GetAttribute("head");
            int bits;
            if (int.TryParse(headLabel, out bits))
            {
                int totalSize = DeclareKit.CalcTotalSize(bits);
                if (totalSize <= 0) return DeclareKeyFrame.DEFAULT;

                curve.bits = bits;
                curve.frames = new List<int[]>();
                foreach (XmlElement keyNode in childKey)
                {
                    string paramLabel = keyNode.GetAttribute("params");
                    string[] temp = paramLabel.Split(split);
                    if (temp.Length == totalSize)
                    {
                        int[] data = new int[temp.Length];
                        for (int i = 0; i < temp.Length; ++i)
                        {
                            int.TryParse(temp[i], out data[i]);
                        }
                        curve.frames.Add(data);
                    }
                }
            }
            return curve;
        }
        //-------------------------------------------
        public static DeclareKeyFrame ReadDeclareKeyFrame(XmlElement node, string strLabel = "Keys", char split = '@')
        {
            foreach (XmlElement childKey in node)
            {
                if (childKey.Name.CompareTo(strLabel) == 0)
                {
                    return ReadDeclareKeyFrame(childKey, split);
                }
            }
            return DeclareKeyFrame.DEFAULT;
        }
        //-------------------------------------------
        public static AnimationCurve ReadCurve(System.IO.BinaryReader reader)
        {
            int keyLen = reader.ReadUInt16();
            if (keyLen == 0) return null;
            AnimationCurve curve = new AnimationCurve();
            curve.postWrapMode = (WrapMode)reader.ReadByte();
            curve.preWrapMode = (WrapMode)reader.ReadByte();

            Keyframe[] keys = new Keyframe[keyLen];
            for(int i = 0; i < keyLen; ++i)
            {
                Keyframe key = new Keyframe();
                key.time = reader.ReadSingle();
                key.value = reader.ReadSingle();
                key.inTangent = reader.ReadSingle();
                key.inWeight = reader.ReadSingle();
                key.outTangent = reader.ReadSingle();
                key.outWeight = reader.ReadSingle();
                key.weightedMode = (WeightedMode)reader.ReadByte();
                keys[i] = key;
            }
            curve.keys = keys;
            return curve;
        }
        //-------------------------------------------
        public static DeclareKeyFrame ReadCurve(System.IO.BinaryReader reader, bool bCheck = true)
        {
            int bits = reader.ReadInt32();
            if (bits == 0) return DeclareKeyFrame.DEFAULT;
            DeclareKeyFrame curve = new DeclareKeyFrame();
            curve.bits = bits;
            ushort frameCnt = reader.ReadUInt16();
            int totalSize = DeclareKit.CalcTotalSize(bits);
            if(bCheck)
            {
                int reaminSize = (int)(reader.BaseStream.Length - reader.BaseStream.Position) / 4;
                if (reaminSize != frameCnt * totalSize) return DeclareKeyFrame.DEFAULT;
            }

            for (int i = 0; i < frameCnt; i += totalSize)
            {
                int[] datas = new int[totalSize];
                for (int j = 0; j < totalSize; ++j)
                    datas[j] = reader.ReadInt32();
                curve.frames.Add(datas);
            }
            return curve;
        }
#endregion
    }
}

