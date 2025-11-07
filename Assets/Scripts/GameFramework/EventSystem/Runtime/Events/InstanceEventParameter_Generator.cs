//auto generator
using System.Xml;
namespace Framework.Core
{
	public partial class InstanceEventParameter
	{
		public override ushort GetEventType()
		{
			return 2;
		}
		public override bool Read(ref Framework.Data.BinaryUtil reader)
		{
			base.Read(ref reader);
			strFile = reader.ToString();
			bAbs = reader.ToBool();
			parent_slot = reader.ToString();
			offset = reader.ToVec3();
			euler = reader.ToVec3();
			scale = reader.ToFloat();
			bindBit = reader.ToByte();
			return true;
		}
		#if UNITY_EDITOR
		public override void Write(ref Framework.Data.BinaryUtil writer)
		{
			base.Write(ref writer);
			writer.WriteString(strFile);
			writer.WriteBool(bAbs);
			writer.WriteString(parent_slot);
			writer.WriteVector3(offset);
			writer.WriteVector3(euler);
			writer.WriteFloat(scale);
			writer.WriteByte(bindBit);
		}
		#endif
		public override bool Copy(BaseEvent evtParam)
		{
			if(!base.Copy(evtParam)) return false;
			InstanceEventParameter oth = evtParam as InstanceEventParameter;
			strFile = oth.strFile;
			bAbs = oth.bAbs;
			parent_slot = oth.parent_slot;
			offset = oth.offset;
			euler = oth.euler;
			scale = oth.scale;
			bindBit = oth.bindBit;
			return true;
		}
		protected override bool InnerRead(System.Collections.Generic.List<string> vParams)
		{
			if(vParams.Count <= 0) return true;
			strFile= vParams[0];
			vParams.RemoveAt(0);
			if(vParams.Count <= 0 ) return true;
			bAbs = vParams[0].CompareTo("1") == 0;
			vParams.RemoveAt(0);
			if(vParams.Count <= 0) return true;
			parent_slot= vParams[0];
			vParams.RemoveAt(0);
				if(vParams.Count <= 0 || !float.TryParse(vParams[0], out offset.x)) return true;
				vParams.RemoveAt(0);
				if(vParams.Count <= 0 || !float.TryParse(vParams[0], out offset.y)) return true;
				vParams.RemoveAt(0);
				if(vParams.Count <= 0 || !float.TryParse(vParams[0], out offset.z)) return true;
				vParams.RemoveAt(0);
				if(vParams.Count <= 0 || !float.TryParse(vParams[0], out euler.x)) return true;
				vParams.RemoveAt(0);
				if(vParams.Count <= 0 || !float.TryParse(vParams[0], out euler.y)) return true;
				vParams.RemoveAt(0);
				if(vParams.Count <= 0 || !float.TryParse(vParams[0], out euler.z)) return true;
				vParams.RemoveAt(0);
			if(vParams.Count <= 0 || !float.TryParse(vParams[0], out scale)) return true;
			vParams.RemoveAt(0);
			if(vParams.Count <= 0 || !byte.TryParse(vParams[0], out bindBit)) return true;
			vParams.RemoveAt(0);
			return true;
		}
		#if UNITY_EDITOR
		protected override string InnerWrite()
		{
			string strParam = "";
			strParam +=strFile+",";
			strParam += (bAbs?"1":"0")+",";
			strParam +=parent_slot+",";
				strParam +=offset.x.ToString("F3")+",";
				strParam +=offset.y.ToString("F3")+",";
				strParam +=offset.z.ToString("F3")+",";
				strParam +=euler.x.ToString("F3")+",";
				strParam +=euler.y.ToString("F3")+",";
				strParam +=euler.z.ToString("F3")+",";
			strParam +=scale.ToString()+",";
			strParam +=bindBit.ToString()+",";
			return strParam;
		}
		#endif
		public override void InnerRead(ref XmlElement ele)
		{
			base.InnerRead(ref ele);
			if(ele.HasAttribute("strFile")) strFile = ele.GetAttribute("strFile");
			if(ele.HasAttribute("bAbs")) bAbs = ele.GetAttribute("bAbs").CompareTo("1") == 0;
			if(ele.HasAttribute("parent_slot")) parent_slot = ele.GetAttribute("parent_slot");
				if(ele.HasAttribute("offset"))
				{
					string[] split = ele.GetAttribute("offset").ToString().Split(',');
					if(split.Length == 3)
					{
						offset.x= float.Parse(split[0]);
						offset.y= float.Parse(split[1]);
						offset.z= float.Parse(split[2]);
					}
				}
				if(ele.HasAttribute("euler"))
				{
					string[] split = ele.GetAttribute("euler").ToString().Split(',');
					if(split.Length == 3)
					{
						euler.x= float.Parse(split[0]);
						euler.y= float.Parse(split[1]);
						euler.z= float.Parse(split[2]);
					}
				}
			if(ele.HasAttribute("scale")) scale = float.Parse(ele.GetAttribute("scale"));
			if(ele.HasAttribute("bindBit")) bindBit = byte.Parse(ele.GetAttribute("bindBit"));
		}
		#if UNITY_EDITOR
		public override void InnerWrite(XmlDocument pDoc, ref XmlElement ele)
		{
			base.InnerWrite(pDoc, ref ele);
			ele.SetAttribute("strFile", strFile.ToString());
			ele.SetAttribute("bAbs", bAbs?"1":"0");
			ele.SetAttribute("parent_slot", parent_slot.ToString());
			ele.SetAttribute("offset", string.Format("{0},{1},{2}",offset.x,offset.y,offset.z));
			ele.SetAttribute("euler", string.Format("{0},{1},{2}",euler.x,euler.y,euler.z));
			ele.SetAttribute("scale", scale.ToString());
			ele.SetAttribute("bindBit", bindBit.ToString());
		}
		#endif
	}
}
