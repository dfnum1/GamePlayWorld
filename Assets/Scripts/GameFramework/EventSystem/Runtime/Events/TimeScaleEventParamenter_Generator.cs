//auto generator
using System.Xml;
namespace Framework.Core
{
	public partial class TimeScaleEventParamenter
	{
		public override ushort GetEventType()
		{
			return 3;
		}
		public override bool Read(ref Framework.Data.BinaryUtil reader)
		{
			base.Read(ref reader);
			timeScale = reader.ToFloat();
			fDuration = reader.ToFloat();
			bUsedCurve = reader.ToBool();
			timeCurve = reader.ToCurve();
			return true;
		}
		#if UNITY_EDITOR
		public override void Write(ref Framework.Data.BinaryUtil writer)
		{
			base.Write(ref writer);
			writer.WriteFloat(timeScale);
			writer.WriteFloat(fDuration);
			writer.WriteBool(bUsedCurve);
			writer.WriteCurve(timeCurve);
		}
		#endif
		public override bool Copy(BaseEvent evtParam)
		{
			if(!base.Copy(evtParam)) return false;
			TimeScaleEventParamenter oth = evtParam as TimeScaleEventParamenter;
			timeScale = oth.timeScale;
			fDuration = oth.fDuration;
			bUsedCurve = oth.bUsedCurve;
			timeCurve = oth.timeCurve;
			return true;
		}
		protected override bool InnerRead(System.Collections.Generic.List<string> vParams)
		{
			if(vParams.Count <= 0 || !float.TryParse(vParams[0], out timeScale)) return true;
			vParams.RemoveAt(0);
			if(vParams.Count <= 0 || !float.TryParse(vParams[0], out fDuration)) return true;
			vParams.RemoveAt(0);
			if(vParams.Count <= 0 ) return true;
			bUsedCurve = vParams[0].CompareTo("1") == 0;
			vParams.RemoveAt(0);
			if(vParams.Count <= 0) return true;
			timeCurve = EventHelper.ReadCurve(vParams[0]);
			vParams.RemoveAt(0);
			return true;
		}
		#if UNITY_EDITOR
		protected override string InnerWrite()
		{
			string strParam = "";
			strParam +=timeScale.ToString()+",";
			strParam +=fDuration.ToString()+",";
			strParam += (bUsedCurve?"1":"0")+",";
			strParam += EventHelper.SaveCurve(timeCurve)+",";
			return strParam;
		}
		#endif
		public override void InnerRead(ref XmlElement ele)
		{
			base.InnerRead(ref ele);
			if(ele.HasAttribute("timeScale")) timeScale = float.Parse(ele.GetAttribute("timeScale"));
			if(ele.HasAttribute("fDuration")) fDuration = float.Parse(ele.GetAttribute("fDuration"));
			if(ele.HasAttribute("bUsedCurve")) bUsedCurve = ele.GetAttribute("bUsedCurve").CompareTo("1") == 0;
		}
		#if UNITY_EDITOR
		public override void InnerWrite(XmlDocument pDoc, ref XmlElement ele)
		{
			base.InnerWrite(pDoc, ref ele);
			ele.SetAttribute("timeScale", timeScale.ToString());
			ele.SetAttribute("fDuration", fDuration.ToString());
			ele.SetAttribute("bUsedCurve", bUsedCurve?"1":"0");
		}
		#endif
	}
}
