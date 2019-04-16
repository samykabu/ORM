using System.Collections;
using System.IO;
using CodeGenerator;
using SchemaObjects;

namespace Sql2005Server
{
    public class ViewToClass : ITemplate
    {
        private Class oClass = null;
        private string ClassName;
        private View tb = null;
        private string _Prefix = "";
        private string _NameSpace = "";
        private bool Serializeable = false;

        private Hashtable TypeNull = new Hashtable();
        private Hashtable TypeNull1 = new Hashtable();

        public  ViewToClass()
        {
            TypeNull.Add("string", "string.Empty");
            TypeNull.Add("Int64", "0");
            TypeNull.Add("int", "0");
            TypeNull.Add("Int16", "0");
            TypeNull.Add("Int32", "0");
            TypeNull.Add("short", "(short)0");
            TypeNull.Add("byte[]", "null");
            TypeNull.Add("DateTime", "DateTime.MinValue");
            TypeNull.Add("bit", "false");
            TypeNull.Add("bool", "false");
            TypeNull.Add("decimal", "0");
            TypeNull.Add("double", "0");
            TypeNull.Add("Single", "0");
            TypeNull.Add("Guid", "Guid.Empty");

            TypeNull1.Add("string", "string.Empty");
            TypeNull1.Add("byte[]", "null");
            TypeNull1.Add("DateTime", "DateTime.MinValue");
            TypeNull1.Add("Guid", "Guid.Empty");
        }
        public override void Process(IDataBaseObject DbObject, string NameSpace, string Prefix, bool bSerialize)
        {

            _NameSpace = NameSpace;
            _Prefix = Prefix;
            Serializeable = bSerialize;

            if (SupportedDatabaseObject(DbObject))
            {
                tb = (View)DbObject;
                oClass = new Class();
                ClassName = tb.Name;
                ClassName = ClassName.Replace(".", "_").Replace("[", "").Replace("]", "");
                oClass.Name = ClassName;

                if(NameSpace.Trim().Length>0)
                    oClass.NameSpace = NameSpace;
                oClass.Prefix = Prefix;

                oClass.UsingLibrary.Add("using System;\r");
                oClass.UsingLibrary.Add("using System.Text;\r");
                oClass.UsingLibrary.Add("using System.Data;\r");
                oClass.UsingLibrary.Add("using System.Data.SqlClient;\r");                
                ProcessClass();
            }
        }

        public void ProcessClass()
        {            
            oClass.UsingLibrary.Add("using System.Collections.Generic;");

            #region Constructors          

            Method DC = new Method(); //Default Constructor
            DC.Name = _Prefix.Length > 0 ? (_Prefix + tb.Name) : tb.Name;

            DC.Body.AppendFormat("\r\t\tpublic {0}()\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            DC.Body.Append("\t\t{\r");
            DC.Body.AppendFormat("\t\t\tTableName=\"{0}\";\r", tb.Name);
            DC.Body.Append("\t\t}\r");

            oClass.Methods.Add(DC);

          
            #endregion

            #region Serialization And Clone Methods

            if (Serializeable)
            {
                //Add refrence to xml serialization lib
                oClass.UsingLibrary.Add("using System.Xml.Serialization;\r");
                oClass.Attributes.Add("\t[Serializable]\r");

                Method Tostring = new Method();
                Tostring.Name = "tostring";

                Tostring.Body.Append("\r\t\tpublic override string ToString()\r");
                Tostring.Body.Append("\t\t{\r");
                Tostring.Body.AppendFormat("\t\t\tXmlSerializer xml = new XmlSerializer(typeof({0}));\r",
                                           _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
                Tostring.Body.Append("\t\t\tStringBuilder outstr = new StringBuilder();\r");
                Tostring.Body.Append("\t\t\tSystem.IO.TextWriter textwriter = new System.IO.StringWriter(outstr);\r");
                Tostring.Body.Append("\t\t\txml.Serialize(textwriter, this);\r");
                Tostring.Body.Append("\t\t\txml = null;\r");
                Tostring.Body.Append("\t\t\ttextwriter.Flush();\r");
                Tostring.Body.Append("\t\t\ttextwriter.Close();\r");
                Tostring.Body.Append("\t\t\ttextwriter = null;\r");
                Tostring.Body.Append("\t\t\treturn outstr.ToString();\r");
                Tostring.Body.Append("\t\t}\r");

                oClass.Methods.Add(Tostring);
               
            }

            #endregion
            

            #region LoadFromDataset
            Method LoadFromDataset = new Method();
            LoadFromDataset.Name = "loadfromDataSet";

            LoadFromDataset.Body.AppendFormat("\r\t\tprotected static {0} LoadFromDataSet(DataSet Internalds,int rowindex)\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            LoadFromDataset.Body.Append("\t\t{\r");

            LoadFromDataset.Body.Append("\t\t\tif (Internalds!=null && Internalds.Tables[0].Rows.Count >0 && Internalds.Tables[0].Rows.Count>=(rowindex-1))\r");
            LoadFromDataset.Body.Append("\t\t\t{\r");
            LoadFromDataset.Body.AppendFormat("\t\t\t\t{0} ColRecord=new {0}();\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);

            foreach (Column cm in tb.Columns.Values)
            {
                if (cm.IsNullable)
                    LoadFromDataset.Body.AppendFormat(
                        "\t\t\t\tColRecord.{0} = (Internalds.Tables[0].Rows[rowindex][\"{0}\"]==System.Convert.DBNull ? {2} : ({1})Internalds.Tables[0].Rows[rowindex][\"{0}\"]) ;\r",
                        cm.Name, cm.CLRType, TypeNull[cm.CLRType]);
                else
                    LoadFromDataset.Body.AppendFormat(
                        "\t\t\t\tColRecord.{0} =({1}) Internalds.Tables[0].Rows[rowindex][\"{0}\"];\r", cm.Name, cm.CLRType);
            }
            LoadFromDataset.Body.Append("\t\t\t\treturn ColRecord;\r");
            LoadFromDataset.Body.Append("\t\t\t}\r");
            LoadFromDataset.Body.Append("\t\t\treturn null;\r");
            LoadFromDataset.Body.Append("\t\t}\r");

            oClass.Methods.Add(LoadFromDataset);
            #endregion
            #region LoadFromDataset Collection
            Method LoadCollectionFromDataset = new Method();
            LoadCollectionFromDataset.Name = "loadfromDataSet";

            LoadCollectionFromDataset.Body.AppendFormat("\r\t\tprotected static List<{0}> LoadCollectionFromDataSet(DataSet Internalds)\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            LoadCollectionFromDataset.Body.Append("\t\t{\r");            
            {
                
                LoadCollectionFromDataset.Body.Append("\t\t\tif (Internalds!=null && Internalds.Tables[0].Rows.Count>0)\r");
                LoadCollectionFromDataset.Body.Append("\t\t\t{\r");

                LoadCollectionFromDataset.Body.AppendFormat("\t\t\t\t List<{0}> result=new List<{0}>();\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);

                LoadCollectionFromDataset.Body.Append("\t\t\t\tforeach (DataRow row in Internalds.Tables[0].Rows)\r");
                LoadCollectionFromDataset.Body.Append("\t\t\t\t{\r");
                LoadCollectionFromDataset.Body.AppendFormat("\t\t\t\t\t{0} ColRecord=new {0}();\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);

                foreach (Column cm in tb.Columns.Values)
                {
                    if (cm.IsNullable)
                        LoadCollectionFromDataset.Body.AppendFormat(
                            "\t\t\t\t\tColRecord.{0} = (row[\"{0}\"]==System.Convert.DBNull ? {2} : ({1})row[\"{0}\"]) ;\r",
                            cm.Name, cm.CLRType, TypeNull[cm.CLRType]);
                    else
                        LoadCollectionFromDataset.Body.AppendFormat(
                            "\t\t\t\t\tColRecord.{0} =({1}) row[\"{0}\"];\r", cm.Name, cm.CLRType);
                }
                LoadCollectionFromDataset.Body.Append("\t\t\t\t\tresult.Add(ColRecord);\r");
                LoadCollectionFromDataset.Body.Append("\t\t\t\t}\r");
                LoadCollectionFromDataset.Body.Append("\t\t\t\treturn result;\r");
                LoadCollectionFromDataset.Body.Append("\t\t\t}\r");
                LoadCollectionFromDataset.Body.Append("\t\t\treturn null;\r");
            }
            LoadCollectionFromDataset.Body.Append("\t\t}\r");

            oClass.Methods.Add(LoadCollectionFromDataset);
            #endregion

            #region Class Variables & Properties

            foreach (Column cm in tb.Columns.Values)
            {
                if (cm.IsNullable)
                    oClass.DeclareVariable.Add(
                        string.Format("\t\tprivate {0} _{1}={3};{2}", cm.CLRType, cm.Name, "\r", TypeNull[cm.CLRType]));
                else
                    oClass.DeclareVariable.Add(string.Format("\t\tprivate {0} _{1};{2}", cm.CLRType, cm.Name, "\r"));
                Property oprop = new Property();
                oprop.Name = cm.Name;

                oprop.Body.Append("\r\t\t/// <summary>\r");
                oprop.Body.AppendFormat("\t\t/// {0}\r", cm.Description);
                oprop.Body.Append("\t\t/// </summary>\r");
                oprop.Body.AppendFormat("\t\tpublic {0} {1}\r", cm.CLRType, cm.Name);
                oprop.Body.Append("\t\t{\r");
                oprop.Body.AppendFormat("\t\t\t get {{ return _{0}; }}\r", cm.Name);
                oprop.Body.AppendFormat("\t\t\t set {{ _{0}=value; }}\r", cm.Name);
                oprop.Body.Append("\r\t\t}\r");

                oClass.Properties.Add(oprop);
            }

            #endregion

            oClass.Attributes.Add("\t public partial ");
            oClass.Methods.Add(StrongTypedCollection(ClassName));     
        }

        private Method StrongTypedCollection(string ClassName)
        {
            Method loadinDataset = new Method();
            loadinDataset.Name = "loadinDataset";

            //Select All
            loadinDataset.Body.AppendFormat("\t\tpublic static List<{0}> SelectAll()\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.Append("\t\t{\r");
            loadinDataset.Body.Append("\t\t\t DataSet ResDS=new DataSet();\r");

            loadinDataset.Body.Append("\t\t\t RunStaticSqlCommand(\"Select * from " + tb.Name + "\", ResDS,\"" + tb.Name + "\");\r");

            loadinDataset.Body.AppendFormat("\t\t\tList<{0}> result=LoadCollectionFromDataSet(ResDS) ;\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.AppendFormat("\t\t\tResDS.Dispose();\r");
            loadinDataset.Body.AppendFormat("\t\t\t return result;\r");
            loadinDataset.Body.Append("\t\t}\r");

            //Select All Order By
            loadinDataset.Body.AppendFormat("\t\tpublic static  List<{0}> SelectAll(string OrderBY)\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.Append("\t\t{\r");
            loadinDataset.Body.Append("\t\t\t DataSet ResDS=new DataSet();\r");

            loadinDataset.Body.Append("\t\t\t RunStaticSqlCommand(\"Select * from " + tb.Name + " order by \" + OrderBY , ResDS,\"" + tb.Name + "\");\r");

            loadinDataset.Body.AppendFormat("\t\t\tList<{0}> result=LoadCollectionFromDataSet(ResDS) ;\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.AppendFormat("\t\t\tResDS.Dispose();\r");
            loadinDataset.Body.AppendFormat("\t\t\t return result;\r");
            loadinDataset.Body.Append("\t\t}\r");

            //Select All Order By , Top
            loadinDataset.Body.AppendFormat("\t\tpublic static List<{0}> SelectAll(string OrderBY,int TopCount)\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.Append("\t\t{\r");
            loadinDataset.Body.Append("\t\t\t DataSet ResDS=new DataSet();\r");

            loadinDataset.Body.Append("\t\t\t RunStaticSqlCommand(\"Select TOP \" + TopCount.ToString() + \" * from " + tb.Name + " order by \" + OrderBY , ResDS,\"" + tb.Name + "\");\r");

            loadinDataset.Body.AppendFormat("\t\t\tList<{0}> result=LoadCollectionFromDataSet(ResDS) ;\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.AppendFormat("\t\t\tResDS.Dispose();\r");
            loadinDataset.Body.AppendFormat("\t\t\t return result;\r");
            loadinDataset.Body.Append("\t\t}\r");


            ///---------------------------------------------------------------------------------------------------------------------///

            //Select Where
            loadinDataset.Body.AppendFormat("\t\tpublic static List<{0}> SelectWhere(string FilterBy)\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.Append("\t\t{\r");
            loadinDataset.Body.Append("\t\t\t DataSet ResDS=new DataSet();\r");

            loadinDataset.Body.Append(
                "\t\t\t RunStaticSqlCommand(\"Select * from " + tb.Name + " where \" + FilterBy, ResDS, \"" + tb.Name + "\");\r");
            loadinDataset.Body.AppendFormat("\t\t\tList<{0}> result=LoadCollectionFromDataSet(ResDS) ;\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.AppendFormat("\t\t\tResDS.Dispose();\r");
            loadinDataset.Body.AppendFormat("\t\t\t return result;\r");
            loadinDataset.Body.Append("\t\t}\r");

            //Select First
            loadinDataset.Body.AppendFormat("\t\tpublic static {0} SelectFirst(string FilterBy)\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.Append("\t\t{\r");
            loadinDataset.Body.Append("\t\t\t DataSet ResDS=new DataSet();\r");

            loadinDataset.Body.Append(
                "\t\t\t RunStaticSqlCommand(\"Select top 1 * from " + tb.Name + " where \" + FilterBy, ResDS, \"" + tb.Name + "\");\r");
            loadinDataset.Body.AppendFormat("\t\t\t{0} result=LoadFromDataSet(ResDS,1) ;\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.AppendFormat("\t\t\tResDS.Dispose();\r");
            loadinDataset.Body.AppendFormat("\t\t\treturn result;\r");
            loadinDataset.Body.Append("\t\t}\r");

            //Select Where Top OrderBY
            loadinDataset.Body.AppendFormat("\t\tpublic static List<{0}> SelectWhere(string FilterBy,int TopCount,string OrderBY)\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.Append("\t\t{\r");
            loadinDataset.Body.Append("\t\t\t DataSet ResDS=new DataSet();\r");
            loadinDataset.Body.Append(
                "\t\t\t RunStaticSqlCommand(\"Select top \" + TopCount.ToString() + \" * from " + tb.Name + " where \" + FilterBy +\" order by \" + OrderBY , ResDS, \"" + tb.Name + "\");\r");
            loadinDataset.Body.AppendFormat("\t\t\tList<{0}> result=LoadCollectionFromDataSet(ResDS) ;\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            loadinDataset.Body.AppendFormat("\t\t\tResDS.Dispose();\r");
            loadinDataset.Body.AppendFormat("\t\t\t return result;\r");
            loadinDataset.Body.Append("\t\t}\r");


            return loadinDataset;
        }

        public override void SaveToFile()
        {
            if (oClass != null)
            {
                string path = Path.Combine(OutputDir, "DBViews");
                DirectoryInfo df = new DirectoryInfo(path);
                if (!df.Exists)
                    df.Create();

                FileInfo oinfo = new FileInfo(Path.Combine(path, oClass.Prefix + tb.Name + @".cs"));
                FileStream sqlFile;
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
            if (DbObject is View)
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