/********************************************************************
生成日期:	1:11:2020 10:06
类    名: 	Data_Base
作    者:	HappLI
描    述:	
*********************************************************************/

namespace Framework.Data
{
    public class BaseData : IUserData
    {
        public void Destroy() { }
    }
    public class Data_Base : IUserData
    {
#if UNITY_EDITOR
        public string strFilePath;
#endif
        private int m_nHashID = 0;
        //-------------------------------------------
        public int GetHashID() { return m_nHashID; }
        public void SetHashID(int hashID) { m_nHashID = hashID; }
        //-------------------------------------------
        public Data_Base()
        {
            m_nHashID = 0;
        }
        //-------------------------------------------
        public virtual bool LoadBinary(System.IO.BinaryReader reader)
        {

            return true;
        }
		//-------------------------------------------
        public virtual bool LoadJson(string strJson)
        {
            return true;
        }
        //-------------------------------------------
        public virtual bool LoadData(string csvContent, CsvParser csv = null)
        {
            return true;
        }
        //-------------------------------------------
        protected virtual void OnLoadCompleted() { }
        protected virtual void OnAddData(IUserData baseData) { }
        //-------------------------------------------
        public virtual void Save(string filename=null)
        {

        }
        //-------------------------------------------
        public virtual void ClearData()
        {
            OnClearData();
        }
        //-------------------------------------------
        protected virtual void OnClearData() { }
        //-------------------------------------------
        protected string ReadString(System.IO.BinaryReader reader)
        {
            ushort len = reader.ReadUInt16();
            if (len <= 0) return "";
            return System.Text.Encoding.UTF8.GetString(reader.ReadBytes(len));
        }
        //-------------------------------------------
        protected void WriterString(System.IO.BinaryWriter writer, string strValue)
        {
            if(string.IsNullOrEmpty(strValue))
            {
                writer.Write((ushort)0);
                return;
            }
            writer.Write((ushort)strValue.Length);
            byte[] val = System.Text.Encoding.UTF8.GetBytes(strValue);
            writer.Write(val);
        }
        //-------------------------------------------
        public void Destroy() { }
    }
}