using System;
using System.IO;
using System.Text;
using CodeGenerator;
using SchemaObjects;

namespace Sql2005Server
{
    /// <summary>
    /// Generate a script for stored procedure for each table (insert,update,delete)
    /// </summary>
    public class TableStoredProcedures : ITemplate
    {
        private StringBuilder TableSp = new StringBuilder();
        private Table tb = null;
        public  StreamWriter Streamwriter;

        public override void Process(IDataBaseObject DbObject, string NameSpace, string Prefix, bool bSerialize)
        {
            
            if ((DbObject) is Table && SupportedDatabaseObject(DbObject))
            {
                tb = (Table)DbObject;
                TableSp = new StringBuilder();                                
                TableSp.Append(spInsert(tb));
                TableSp.Append("\n\r");
                TableSp.Append(spUpdate(tb));
                TableSp.Append("\n\r");
                TableSp.Append(spDelete(tb));
                TableSp.Append("\n\r");
                TableSp.Append("\n\r");
            }
        }

        /// <summary>
        /// this will save to a file (append for each database object)
        /// </summary>
        public override void SaveToFile()
        {

            Streamwriter.Write(TableSp.ToString());
            Streamwriter.Flush();
                //_streamwriter.Close();
          
        }

        /// <summary>
        /// class support only table objects
        /// </summary>
        /// <param name="DbObject"></param>
        /// <returns></returns>
        protected override bool SupportedDatabaseObject(IDataBaseObject DbObject)
        {
            if (DbObject is Table)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #region Stored Procedures

        //public string DbUSe(string DBname)
        //{
        //    return string.Format("Use [{0}] \n\r GO \n\r", DBname);
        //}

        public string DropProcedure(string ProName)
        {
            ProName=ProName.Replace("[", "").Replace("]", "");
            string drpst = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'P', N'PC')) \n\r  DROP PROCEDURE  [{0}] \n\r GO \n\r ";
            return string.Format(drpst, ProName);
        }

        public string spInsert(Table tb)
        {
            StringBuilder Insert = new StringBuilder();
            Insert.Append(DropProcedure("Insert_" + tb.Name));
            Insert.AppendFormat("EXEC dbo.sp_executesql @statement = N'create procedure [Insert_{0}]\r", tb.Name.Replace("[", "").Replace("]", ""));
            Insert.Append("(\r");
            foreach (Column cm in tb.Columns.Values)
            {
                if (!cm.Readonly)
                    Insert.AppendFormat("@{0} {1},\r", cm.Name, cm.SqlType);
            }
            Insert.Remove(Insert.Length - 2, 1);
            Insert.Append(")\r");
            Insert.AppendFormat("as insert into {0}\r(\r", tb.Name);
            foreach (Column cm in tb.Columns.Values)
            {
                if (!cm.Readonly)
                    Insert.AppendFormat("[{0}],\r", cm.Name, cm.SqlType);
            }
            Insert.Remove(Insert.Length - 2, 1);
            Insert.Append(")\r values\r(\r");
            foreach (Column cm in tb.Columns.Values)
            {
                if (!cm.Readonly)
                    Insert.AppendFormat("@{0},\r", cm.Name, cm.SqlType);
            }
            Insert.Remove(Insert.Length - 2, 1);
            if(tb.IdentityColumn!=null)
                Insert.Append(")\r if @@error=0\r select Result=''true'',@@identity as RecordIdentity\r else\r select Result=''false''\r'\r");
            else
                Insert.Append(")\r if @@error=0\r select Result=''true''\r else\r select Result=''false''\r'\r");

            return Insert.ToString();
        }

        public string spUpdate(Table tb)
        {
            StringBuilder UpdateStr = new StringBuilder();
            UpdateStr.Append(DropProcedure("Update_" + tb.Name));
            UpdateStr.AppendFormat("EXEC dbo.sp_executesql @statement = N'create procedure [Update_{0}]\r", tb.Name.Replace("[", "").Replace("]", ""));
            UpdateStr.Append("(\r");
            foreach (Column cm in tb.Columns.Values)
            {
                UpdateStr.AppendFormat("@{0} {1},\r", cm.Name, cm.SqlType);
            }
            UpdateStr.Remove(UpdateStr.Length - 2, 1);
            UpdateStr.Append(")\r");
            UpdateStr.AppendFormat("as update {0}\rset\r", tb.Name);
            foreach (Column cm in tb.Columns.Values)
            {
                if (!cm.Readonly)
                    UpdateStr.AppendFormat("[{0}]=@{0},\r", cm.Name, cm.SqlType);
            }
            UpdateStr.Remove(UpdateStr.Length - 2, 1);
            string pks = "";
            foreach (Column cm in tb.PrimaryKeys.Values)
            {
                pks += string.Format("[{0}]=@{0} and", cm.Name);
            }
            if (pks != string.Empty)
            {
                pks = pks.Substring(0, pks.Length - 4);
                UpdateStr.AppendFormat(" where\r {0}\r if @@error=0\r select Result=''true''\r else\r select Result=''false''\r'\r",
                                       pks);
            }
            return UpdateStr.ToString();
        }

        public string spDelete(Table tb)
        {
            StringBuilder Deletestr = new StringBuilder();
            Deletestr.Append(DropProcedure("Delete_" + tb.Name));
            Deletestr.AppendFormat("EXEC dbo.sp_executesql @statement = N'create procedure [Delete_{0}]\r", tb.Name.Replace("[", "").Replace("]", ""));
            Deletestr.Append("(\r");
            foreach (Column cm in tb.PrimaryKeys.Values)
                Deletestr.AppendFormat("@{0} {1},\r", cm.Name, cm.SqlType);

            Deletestr.Remove(Deletestr.Length - 2, 1);
            Deletestr.Append(")\r");
            Deletestr.AppendFormat("as delete from  {0}\r", tb.Name);

            string pks = "";

            foreach (Column cm in tb.PrimaryKeys.Values)
                pks += string.Format("[{0}]=@{0} and", cm.Name);

            if (pks != string.Empty)
            {
                pks = pks.Substring(0, pks.Length - 4);
                Deletestr.AppendFormat(
                    " where\r {0}\r if @@error=0\r select Result=''true''\r else\r select Result=''false''\r'\r\r", pks);
            }
            return Deletestr.ToString();
        }

        #endregion
    }
}