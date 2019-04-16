using System.IO;
using CodeGenerator;
using SchemaObjects;

namespace Sql2005Server
{
    public class TableToPartialClassTemplate : ITemplate
    {
        private Class oClass = null;
        private string ClassName;
        private Table tb = null;

        public override void Process(IDataBaseObject DbObject, string NameSpace, string Prefix, bool bSerialize)
        {
            if ((DbObject) is Table && SupportedDatabaseObject(DbObject))
            {
                tb = (Table) DbObject;
                oClass = new Class();
                ClassName = tb.Name;
                ClassName = ClassName.Replace(".", "_").Replace("[", "").Replace("]", "");
                oClass.Name = ClassName;

                oClass.NameSpace = NameSpace;
                oClass.Prefix = Prefix;

                oClass.UsingLibrary.Add("using System;\r");
                oClass.UsingLibrary.Add("using System.Text;\r");
                oClass.UsingLibrary.Add("using System.Data;\r");
                oClass.UsingLibrary.Add("using System.Data.SqlClient;\r");

                oClass.Attributes.Add("\t partial ");
            }
        }

        public override void SaveToFile()
        {
            if (oClass != null)
            {
                string path = Path.Combine(OutputDir, "DbBusObjects");
                DirectoryInfo df = new DirectoryInfo(path);
                if (!df.Exists)
                    df.Create();

                FileInfo oinfo = new FileInfo(Path.Combine(path, oClass.Prefix + tb.Name + @".cs"));
                FileStream sqlFile ;
                if (oinfo.Exists)
                    sqlFile = oinfo.Open(FileMode.Truncate, FileAccess.ReadWrite);
                else
                    sqlFile = oinfo.Create();

                using (StreamWriter sw = new StreamWriter(sqlFile))
                {
                    sw.Write(oClass.ToString());
                    sw.Flush();
                    sw.Close();
                }
            }
        }

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
            if (oClass != null)
            {
                return oClass.ToString();
            }
            return "";
        }
    }
}