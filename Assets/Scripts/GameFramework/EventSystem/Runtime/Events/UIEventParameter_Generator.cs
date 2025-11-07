//auto generator
using System.Xml;
namespace Framework.Core
{
	public partial class UIEventParameter
	{
		public override ushort GetEventType()
		{
			return 1;
		}
		public override bool Read(ref Framework.Data.BinaryUtil reader)
		{
			base.Read(ref reader);
			uiType = reader.ToUshort();
			bAllUI = reader.ToBool();
			type = (Framework.Core.UIEventParameter.EType)reader.ToShort();
			{
				int cnt = (int)reader.ToUshort();
				if(cnt>0)
				{
					Params= new System.Collections.Generic.List<Framework.Data.KeyValueParam>(cnt);
					for(int i =0; i < cnt; ++i)
					{
						{Framework.Data.KeyValueParam temp = new Framework.Data.KeyValueParam(); temp.Read(ref reader); Params.Add(temp);}

					}
				}
			}
			return true;
		}
		#if UNITY_EDITOR
		public override void Write(ref Framework.Data.BinaryUtil writer)
		{
			base.Write(ref writer);
			writer.WriteUshort(uiType);
			writer.WriteBool(bAllUI);
			writer.WriteShort((short)type);
			{
				if(Params!=null) writer.WriteUshort((ushort)(Params.Count));
				else writer.WriteUshort((ushort)0);
				if (Params!=null && Params.Count > 0)
				{
					foreach(var db in Params)
					db.Write(ref writer);

				}
			}
		}
		#endif
		public override bool Copy(BaseEvent evtParam)
		{
			if(!base.Copy(evtParam)) return false;
			UIEventParameter oth = evtParam as UIEventParameter;
			uiType = oth.uiType;
			bAllUI = oth.bAllUI;
			type = oth.type;
			Params = oth.Params;
			return true;
		}
		protected override bool InnerRead(System.Collections.Generic.List<string> vParams)
		{
			if(vParams.Count <= 0 || !ushort.TryParse(vParams[0], out uiType)) return true;
			vParams.RemoveAt(0);
			if(vParams.Count <= 0 ) return true;
			bAllUI = vParams[0].CompareTo("1") == 0;
			vParams.RemoveAt(0);
			if(vParams.Count <= 0) return true;
			{
				int temp=0;
				if(int.TryParse(vParams[0], out temp))type= (Framework.Core.UIEventParameter.EType)temp;
				vParams.RemoveAt(0);
			}
			if(vParams.Count <= 0) return true;
			{
				int tempCnt =0; if(!int.TryParse(vParams[0], out tempCnt)) return false;
				vParams.RemoveAt(0);
				if(tempCnt>0)
				{
					Params = new System.Collections.Generic.List<Framework.Data.KeyValueParam>();
					for(int i =0; i < tempCnt; ++i)
					{
						if(vParams.Count <= 1) return false;
						Params.Add(EventHelper.ReadKV(vParams, 0));
						vParams.RemoveRange(0,2);
					}
				}
			}
			return true;
		}
		#if UNITY_EDITOR
		protected override string InnerWrite()
		{
			string strParam = "";
			strParam +=uiType.ToString()+",";
			strParam += (bAllUI?"1":"0")+",";
			strParam += ((int)type).ToString()+",";
			{
				string strRet = (Params!=null)?Params.Count.ToString():"0";
				strRet += ",";
				if(Params!=null)
				{
					foreach (var sub in Params)
					{
						strRet += EventHelper.WriteKV(sub); strRet += ",";
					}
				}
				strParam += strRet;
			}
			return strParam;
		}
		#endif
		public override void InnerRead(ref XmlElement ele)
		{
			base.InnerRead(ref ele);
			if(ele.HasAttribute("uiType")) uiType = ushort.Parse(ele.GetAttribute("uiType"));
			if(ele.HasAttribute("bAllUI")) bAllUI = ele.GetAttribute("bAllUI").CompareTo("1") == 0;
			{
				int temp=0;
				if(ele.HasAttribute("type") && int.TryParse(ele.GetAttribute("type"), out temp)) type = (Framework.Core.UIEventParameter.EType)temp;
			}
			foreach (XmlElement childKey in ele)
			{
				if (childKey.Name.CompareTo("Params") == 0)
				{
					Params = new System.Collections.Generic.List<Framework.Data.KeyValueParam>();
					foreach(XmlElement subKey in childKey)
					{
						Params.Add(EventHelper.ReadKV(childKey, "value"));
					}
					break;
				}
			}
		}
		#if UNITY_EDITOR
		public override void InnerWrite(XmlDocument pDoc, ref XmlElement ele)
		{
			base.InnerWrite(pDoc, ref ele);
			ele.SetAttribute("uiType", uiType.ToString());
			ele.SetAttribute("bAllUI", bAllUI?"1":"0");
			ele.SetAttribute("type", ((int)type).ToString());
			{
				XmlElement keysNode = pDoc.CreateElement("Params");
					foreach (var sub in Params)
				{
					XmlElement itemNode = pDoc.CreateElement("item");
					EventHelper.WriteKV(sub, itemNode, "value");
					keysNode.AppendChild(itemNode);
				}
				ele.AppendChild(keysNode);
			}
		}
		#endif
	}
}
