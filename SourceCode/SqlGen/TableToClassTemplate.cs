using System.Collections;
using System.IO;
using System.Text;
using CodeGenerator;
using SchemaObjects;

namespace Sql2005Server
{
    public class TableToClassTemplate : ITemplate
    {
        private string _NameSpace = "";
        private string _Prefix = "";
        private Class oClass = null;
        private Table oTable = null;
        private bool Serializeable = false;
        private Table tb = null;
        private Hashtable TypeNull = new Hashtable();
        private Hashtable TypeNull1 = new Hashtable();
        private string ClassName;

        public TableToClassTemplate()
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

        public override void Process(IDataBaseObject DbObject, string NameSpace, string Prefix, bool bSerializeable)
        {
            _NameSpace = NameSpace;
            _Prefix = Prefix;
            Serializeable = bSerializeable;

            if ((DbObject) is Table && SupportedDatabaseObject(DbObject))
            {
                oTable = (Table) DbObject;
                tb = oTable;
                oClass = new Class();
                ClassName = tb.Name;
                ClassName = ClassName.Replace(".", "_").Replace("[", "").Replace("]", "") ;
                oClass.Name = ClassName;

                oClass.NameSpace = _NameSpace;

                oClass.Prefix = _Prefix;
                ProcessClass();
            }
        }

        public override void SaveToFile()
        {
            if (oClass != null)
            {
                string path = Path.Combine(OutputDir, "DbObjects");
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

        public override string ToString()
        {
            if (oClass != null)
            {
                return oClass.ToString();
            }
            return "";
        }

        protected override bool SupportedDatabaseObject(IDataBaseObject DbObject)
        {
            if (DbObject is Table)
            {
                return true;
            }
            return false;
        }

        public void ProcessClass()
        {                        
            oClass.UsingLibrary.Add("using System;\r");
            oClass.UsingLibrary.Add("using System.Text;\r");
            oClass.UsingLibrary.Add("using System.Data;\r");
            oClass.UsingLibrary.Add("using System.Data.SqlClient;\r");
            oClass.UsingLibrary.Add("using System.Collections.Generic;");

            #region Constructors

            Method EventsList = new Method();
            EventsList.Body.Append("\r\t#region Events");
          
            EventsList.Body.Append("\r\t\t public List<ValidateRecord> ValidationSummary = new List<ValidateRecord>();\r");
            EventsList.Body.Append("\r\t#endregion");
            oClass.Methods.Add(EventsList);

            Method DC = new Method(); //Default Constructor
            DC.Name = _Prefix.Length > 0 ? (_Prefix + tb.Name) : tb.Name;

            DC.Body.AppendFormat("\r\t\tpublic {0}()\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            DC.Body.Append("\t\t{\r");
            DC.Body.AppendFormat("\t\t\tTableName=\"{0}\";\r", tb.Name);
            DC.Body.Append("\t\t}\r");

            oClass.Methods.Add(DC);


            //Create a new instance using the primary keys            
            if (tb.PrimaryKeys.Count > 0)
            {
                Method PrimaryKeyConstrctor = new Method();
                PrimaryKeyConstrctor.Name = _Prefix.Length > 0 ? (_Prefix + tb.Name) : tb.Name;

                PrimaryKeyConstrctor.Body.AppendFormat("\r\t\tpublic {0}(",
                                                       _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
                StringBuilder tmp = new StringBuilder();
                foreach (Column clm in tb.PrimaryKeys.Values)
                {
                    PrimaryKeyConstrctor.Body.Append(clm.CLRType);
                    PrimaryKeyConstrctor.Body.AppendFormat(" {0},", clm.Name);
                    tmp.AppendFormat("\t\t\t_{0}={0};\r", clm.Name);
                }

                PrimaryKeyConstrctor.Body.Remove(PrimaryKeyConstrctor.Body.Length - 1, 1);
                PrimaryKeyConstrctor.Body.Append(")\r");
                PrimaryKeyConstrctor.Body.Append("\t\t{\r");
                PrimaryKeyConstrctor.Body.AppendFormat("\t\t\tTableName=\"{0}\";\r", tb.Name);
                PrimaryKeyConstrctor.Body.Append(tmp.ToString());
                PrimaryKeyConstrctor.Body.Append("\t\t\tLoadByPrimaryKey();\r");
                PrimaryKeyConstrctor.Body.Append("\t\t}\r");

                oClass.Methods.Add(PrimaryKeyConstrctor);
            }

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


                //Create an clone instance of the class form a deserialized version
                Method clone = new Method();
                clone.Name = "clone";

                clone.Body.AppendFormat("\r\t\tpublic {0} Clone()\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
                clone.Body.Append("\t\t{\r");
                clone.Body.AppendFormat("\t\t\tXmlSerializer xml = new XmlSerializer(typeof({0}));\r",
                                        _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
                clone.Body.Append(
                    "\t\t\tSystem.IO.TextReader textreader = new System.IO.StringReader(this.ToString());\r");
                clone.Body.AppendFormat("\t\t\treturn ({0})xml.Deserialize(textreader);\r",
                                        _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
                clone.Body.Append("\t\t}\r");
                oClass.Methods.Add(clone);
            }

            #endregion

            #region Load By Primary Keys

            Method LoadByPrimaryKey = new Method();
            LoadByPrimaryKey.Name = "loadbypk";

            LoadByPrimaryKey.Body.Append("\r\t\tpublic void LoadByPrimaryKey()\r");
            LoadByPrimaryKey.Body.Append("\t\t{\r");
            if (tb.PrimaryKeys.Count > 0)
            {
                LoadByPrimaryKey.Body.Append("\t\t\tDataSet Internalds=new DataSet();\r");

                StringBuilder sb = new StringBuilder();
                foreach (Column PkClm in tb.PrimaryKeys.Values)
                {
                    sb.Append(PkClm.Name);
                    sb.Append("=");
                    if (PkClm.CLRType == "string")
                        sb.Append("'");
                    sb.Append("\"+_");
                    sb.Append(PkClm.Name);
                    sb.Append(".ToString() +\"");
                    if (PkClm.CLRType == "string")
                        sb.Append("'\"+\"");
                    sb.Append(" and ");
                }

                LoadByPrimaryKey.Body.AppendFormat(
                    "\t\t\tthis.RunSqlCommand(\"Select top 1 * from {0} where {1}, Internalds);\r", tb.Name,
                    sb.ToString().Substring(0, sb.Length - 7));
                sb = null;

                LoadByPrimaryKey.Body.Append("\t\t\tif (Internalds!=null && Internalds.Tables[0].Rows.Count>0)\r");
                LoadByPrimaryKey.Body.Append("\t\t\t{\r");

                foreach (Column cm in tb.Columns.Values)
                {
                    if (cm.IsNullable)
                        LoadByPrimaryKey.Body.AppendFormat(
                            "\t\t\t\t_{0} = (Internalds.Tables[0].Rows[0][\"{0}\"]==System.Convert.DBNull ? {2} : ({1})Internalds.Tables[0].Rows[0][\"{0}\"]) ;\r",
                            cm.Name, cm.CLRType, TypeNull[cm.CLRType]);
                    else
                        LoadByPrimaryKey.Body.AppendFormat(
                            "\t\t\t\t_{0} =({1}) Internalds.Tables[0].Rows[0][\"{0}\"];\r", cm.Name, cm.CLRType);
                }
                LoadByPrimaryKey.Body.Append("\t\t\t\tInternalds.Dispose();\r");
                LoadByPrimaryKey.Body.Append("\t\t\t\tthis.Close();\r");
                LoadByPrimaryKey.Body.Append("\t\t\t}\r");
                LoadByPrimaryKey.Body.Append("\t\t\telse\r");
                LoadByPrimaryKey.Body.Append("\t\t\t{\r");
                LoadByPrimaryKey.Body.Append("\t\t\t\tthis.Close();\r");
                LoadByPrimaryKey.Body.Append("\t\t\t}\r");
            }
            LoadByPrimaryKey.Body.Append("\t\t}\r");

            oClass.Methods.Add(LoadByPrimaryKey);

            #endregion

            #region LoadFromDataset
            Method LoadFromDataset = new Method();
            LoadFromDataset.Name = "loadfromDataSet";

            LoadFromDataset.Body.AppendFormat("\r\t\tprotected static {0} LoadFromDataSet(DataSet Internalds,int rowindex)\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
            LoadFromDataset.Body.Append("\t\t{\r");

            LoadFromDataset.Body.Append("\t\t\tif (Internalds!=null && Internalds.Tables[0].Rows.Count>0 && Internalds.Tables[0].Rows.Count>=(rowindex-1))\r");
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
            if (tb.PrimaryKeys.Count > 0)
            { 
                //TODO: Check if the rows count +offset is larger than the dataset rows then return only the remainin in the dataset
                //LoadCollectionFromDataset.Body.Append(
                //    "\t\t\tif (Internalds.Tables[0].Rows.Count< rowsCount) { rowsCount=Internalds.Tables[0].Rows.Count;}\r");
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
                oprop.Body.AppendFormat("\t\t/// {0}\r", cm.IsNullable==false?"<b>[Required]</b>"+cm.Description:cm.Description);
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

            oClass.Methods.Add(OnValidateProcedure());
            oClass.Methods.Add(InsertProcedure());
            oClass.Methods.Add(UpdateProcedure());
            oClass.Methods.Add(DeleteProcedure());
            oClass.Methods.Add(DisposProcedure());
            //oClass.Methods.Add(DataTypedSet(ClassName));
            oClass.Methods.Add(StrongTypedCollection(ClassName));
        }
       
        #region DataBase Methods Insert /Update / Delete

        private Method InsertProcedure()
        {
            Method Insert = new Method();
            Insert.Name = "insert";


            Insert.Body.Append("\t\tprotected override bool OnInsert()\r");
            Insert.Body.Append("\t\t{\r");           
            Insert.Body.Append("\t\t\tSqlDataReader mReader=null;\r");            
            
            StringBuilder Parameters = new StringBuilder();
            int i = 0;
            foreach (Column cm in tb.Columns.Values)
            {
                if (cm.Readonly == false)
                {
                    if (!cm.IsNullable) //if does not accept null
                    {
                       
                        if (cm.CLRType != "byte[]")
                            Parameters.AppendFormat("\t\t\tStoredProcedureParam[{0}]=MakeParameter(\"@{1}\",_{1});\r",
                                                    i.ToString(), cm.Name);
                        else
                            Parameters.AppendFormat(
                                "\t\t\tStoredProcedureParam[{0}]=MakeImageParameter(\"@{1}\",_{1});\r", i.ToString(),
                                cm.Name);
                    } // nullable column
                    else
                    {
                        //if (tb.IdentityColumn != null && tb.IdentityColumn.Name != cm.Name)
                            if (cm.CLRType != "byte[]")
                                Parameters.AppendFormat(
                                    "\t\t\tStoredProcedureParam[{0}]=MakeParameter(\"@{1}\",(_{1}=={2}? System.Convert.DBNull : _{1}));\r",
                                    i.ToString(), cm.Name, TypeNull[cm.CLRType]);
                            else
                                Parameters.AppendFormat(
                                    "\t\t\tStoredProcedureParam[{0}]=MakeImageParameter(\"@{1}\",(_{1}=={2}? System.Convert.DBNull : _{1}));\r",
                                    i.ToString(), cm.Name, TypeNull[cm.CLRType]);
                        //else // table has no Identity Column
                            //Parameters.AppendFormat(
                                //"\t\t\tStoredProcedureParam[{0}]=MakeImageParameter(\"@{1}\",(_{1}=={2}? System.Convert.DBNull : _{1}));\r",
                                //i.ToString(), cm.Name, TypeNull[cm.CLRType]);    
                    }

                    i++;
                }
            }
            Insert.Body.AppendFormat("\t\t\tSqlParameter[] StoredProcedureParam=new SqlParameter[{0}];\r", i.ToString());            
            Insert.Body.Append(Parameters.ToString());
           

            Insert.Body.Append("\t\t\ttry\r");
            Insert.Body.Append("\t\t\t{\r");
            Insert.Body.AppendFormat("\t\t\t\tRunSqlCommand(\"[Insert_{0}]\",StoredProcedureParam,ref mReader);\r",
                                     tb.Name);
            Insert.Body.Append("\t\t\t\tif(mReader!=null)\r");
            Insert.Body.Append("\t\t\t\t\t{\r");
            Insert.Body.Append("\t\t\t\t\t\tif(mReader.Read())\r");
            Insert.Body.Append("\t\t\t\t\t\t{\r");
            if (tb.IdentityColumn != null)
            {
                Insert.Body.AppendFormat("\t\t\t\t\t\t{0} Ident={0}.Parse(mReader[1].ToString());\r",
                                            tb.IdentityColumn.CLRType);
                Insert.Body.AppendFormat("\t\t\t\t\t\tbool Res = bool.Parse(mReader[0].ToString());\r");
            }
            else
            {
                Insert.Body.AppendFormat("\t\t\t\t\t\tbool Res = bool.Parse(mReader[0].ToString());\r");
            }

            Insert.Body.Append("\t\t\t\t\t\tmReader.Close();\r");
            Insert.Body.Append("\t\t\t\t\t\tmReader=null;\r");
            Insert.Body.Append("\t\t\t\t\t\tthis.Close();\r");

            if (tb.IdentityColumn != null)
            {
                Insert.Body.AppendFormat("\t\t\t\t\t\t_{0}=Ident;\r", tb.IdentityColumn.Name);                
            }

            Insert.Body.Append("\t\t\t\t\t\treturn Res;\r");

            Insert.Body.Append("\t\t\t\t\t\t}\r");
            Insert.Body.Append("\t\t\t\t\telse\r");
            Insert.Body.Append("\t\t\t\t\t\t{\r");
            Insert.Body.Append("\t\t\t\t\t\t\tmReader.Close();\r");
            Insert.Body.Append("\t\t\t\t\t\t\tmReader=null;\r");
            Insert.Body.Append("\t\t\t\t\t\t\tthrow new Exception(\"Data Reader is null\");\r");
            Insert.Body.Append("\t\t\t\t\t\t}\r");
            Insert.Body.Append("\t\t\t\t\t}\r");
            Insert.Body.Append("\t\t\t\t\telse\r");
            Insert.Body.Append("\t\t\t\t\t{\r");
            Insert.Body.Append("\t\t\t\t\t\t throw new Exception(\"Data Reader is null\");\r");
            Insert.Body.Append("\t\t\t\t\t}\r");
            Insert.Body.Append("\t\t\t}\r");
            Insert.Body.Append("\t\t\t\tcatch (Exception ex)\r");
            Insert.Body.Append("\t\t\t{\r");
            Insert.Body.Append("\t\t\t\tif(mReader!=null)\r");
            Insert.Body.Append("\t\t\t\t\tmReader=null;\r");
            Insert.Body.Append("\t\t\t\t\tthis.Close();\r");
            Insert.Body.Append("\t\t\t\tthrow ex;\r");
            Insert.Body.Append("\t\t\t}\r");

            Insert.Body.Append("\t\t}\r");
            return Insert;
        }

        private Method OnValidateProcedure()
        {
            Method Validate = new Method();
            Validate.Name = "OnValidate";
            Validate.Body.Append("\t\tprotected override void OnValidate()\r");
            Validate.Body.Append("\t\t{\r");
            Validate.Body.Append("\t\t\tValidationSummary.Clear();\r");
            
            StringBuilder FiledCheck = new StringBuilder();
            int i = 0;
            foreach (Column cm in tb.Columns.Values)
            {
                if (!cm.IsNullable)
                {                    
                    if (TypeNull1.Contains(cm.CLRType) ||cm.IsForignKey)
                    {
                        FiledCheck.AppendFormat("\r\t\t\tif (_{0}=={1})\r", cm.Name, TypeNull[cm.CLRType]);
                        FiledCheck.Append("\t\t\t{\r");
                        FiledCheck.Append(
                                "\t\t\t\tValidationSummary.Add	(new ValidateRecord	(){ Message = \"" + cm.Name + " Value can not be null \",PropertyName = \"" + cm.Name + "\",ValidationError = \" \" });\r");
                        FiledCheck.AppendFormat("\t\t\t\t //throw new Exception(\"{0} Value can not be null\");\r", cm.Name);
                        FiledCheck.Append("\t\t\t}\r");
                    }
                    i++;
                }
            }
            Validate.Body.Append(FiledCheck.ToString());
            Validate.Body.Append("\t\t\tif (ValidationSummary.Count>0)\r");
            Validate.Body.Append("\t\t\t\tthrow new ValidationException(\"Missing Properties\", ValidationSummary);\r");
            Validate.Body.Append("\t\t}\r");
            return Validate;

        }

        private Method UpdateProcedure()
        {
            Method Update = new Method();
            Update.Name = "update";


            Update.Body.Append("\t\tprotected override bool OnUpdate()\r");
            Update.Body.Append("\t\t{\r");            
            Update.Body.Append("\t\t\tSqlDataReader mReader=null;\r");            
            Update.Body.AppendFormat("\t\t\tSqlParameter[] StoredProcedureParam=new SqlParameter[{0}];\r",
                                     (tb.Columns.Count).ToString());
            
            StringBuilder Parameters = new StringBuilder();
            int i = 0;
            foreach (Column cm in tb.Columns.Values)
            {
                if (!cm.IsNullable)
                {
                   
                    if (cm.CLRType != "byte[]")
                        Parameters.AppendFormat("\t\t\tStoredProcedureParam[{0}]=MakeParameter(\"@{1}\",_{1});\r",
                                                i.ToString(), cm.Name);
                    else
                        Parameters.AppendFormat("\t\t\tStoredProcedureParam[{0}]=MakeImageParameter(\"@{1}\",_{1});\r",
                                                i.ToString(), cm.Name);
                }
                else
                {
                    if (cm.CLRType != "byte[]")
                        Parameters.AppendFormat(
                            "\t\t\tStoredProcedureParam[{0}]=MakeParameter(\"@{1}\",(_{1}=={2}? System.Convert.DBNull : _{1}));\r",
                            i.ToString(), cm.Name, TypeNull[cm.CLRType]);
                    else
                        Parameters.AppendFormat(
                            "\t\t\tStoredProcedureParam[{0}]=MakeImageParameter(\"@{1}\",(_{1}=={2}? System.Convert.DBNull : _{1}));\r",
                            i.ToString(), cm.Name, TypeNull[cm.CLRType]);
                }
                i++;
            }
            
            Update.Body.Append(Parameters.ToString());
            
            Update.Body.Append("\t\t\ttry\r");
            Update.Body.Append("\t\t\t{\r");
            Update.Body.AppendFormat("\t\t\t\tRunSqlCommand(\"[Update_{0}]\",StoredProcedureParam,ref mReader);\r",
                                     tb.Name);
            Update.Body.Append("\t\t\t\tif(mReader!=null)\r");
            Update.Body.Append("\t\t\t\t\t{\r");
            Update.Body.Append("\t\t\t\t\t\tif(mReader.Read())\r");
            Update.Body.Append("\t\t\t\t\t\t{\r");
            // if (tb.IdentityColumn != null)  bool Res = int.Parse(mReader[0].ToString()) == 1 ? true : false;
              //  Update.Body.AppendFormat("\t\t\t\t\t\t{0} Res={0}.Parse(mReader[0].ToString());\r",
              //                           tb.IdentityColumn.CLRType);

            Update.Body.AppendFormat("\t\t\t\t\t\tbool Res = bool.Parse(mReader[0].ToString());\r");

            Update.Body.Append("\t\t\t\t\t\tmReader.Close();\r");
            Update.Body.Append("\t\t\t\t\t\tmReader=null;\r");
            Update.Body.Append("\t\t\t\t\t\tthis.Close();\r");

            Update.Body.Append("\t\t\t\t\t\treturn Res;\r");

            Update.Body.Append("\t\t\t\t\t\t}\r");
            Update.Body.Append("\t\t\t\t\telse\r");
            Update.Body.Append("\t\t\t\t\t\t{\r");
            Update.Body.Append("\t\t\t\t\t\t\tmReader.Close();\r");
            Update.Body.Append("\t\t\t\t\t\t\tmReader=null;\r");
            Update.Body.Append("\t\t\t\t\t\t\treturn false;\r");
            Update.Body.Append("\t\t\t\t\t\t}\r");
            Update.Body.Append("\t\t\t\t\t}\r");
            Update.Body.Append("\t\t\t\t\telse\r");
            Update.Body.Append("\t\t\t\t\t{\r");
            Update.Body.Append("\t\t\t\t\t\t throw new Exception(\"Data Reader is null\");\r");
            Update.Body.Append("\t\t\t\t\t}\r");
            Update.Body.Append("\t\t\t}\r");
            Update.Body.Append("\t\t\t\tcatch (Exception ex)\r");
            Update.Body.Append("\t\t\t{\r");
            Update.Body.Append("\t\t\t\tif(mReader!=null)\r");
            Update.Body.Append("\t\t\t\t\tmReader=null;\r");
            Update.Body.Append("\t\t\t\t\tthis.Close();\r");
            Update.Body.Append("\t\t\t\tthrow ex;\r");
            Update.Body.Append("\t\t\t}\r");

            Update.Body.Append("\t\t}\r");
            return Update;
        }

        private Method DisposProcedure()
        {
            Method disp = new Method();
            disp.Name = "Dispose";            
            disp.Body.Append("\t\tpublic new void Dispose()\r");
            disp.Body.Append("\t\t{\r");
            disp.Body.Append("\t\t\tbase.Dispose();\r");
            disp.Body.Append("\t\t\t//GC.SuppressFinalize(this);\r");
            disp.Body.Append("\t\t}\r");
            return disp;
        }
        private Method DeleteProcedure()
        {
            Method delete = new Method();
            delete.Name = "delete";


            delete.Body.Append("\t\tprotected override bool OnDelete()\r");
            delete.Body.Append("\t\t{\r");
            delete.Body.Append("\t\t\tSqlDataReader mReader=null;\r");
            delete.Body.AppendFormat("\t\t\tSqlParameter[] StoredProcedureParam=new SqlParameter[{0}];\r",
                                     (tb.PrimaryKeys.Count).ToString());

            StringBuilder FiledCheck = new StringBuilder();
            StringBuilder Parameters = new StringBuilder();
            int i = 0;
            foreach (Column cm in tb.PrimaryKeys.Values)
            {
                if (cm.IsNullable)
                {
                    FiledCheck.AppendFormat("\r\t\t\tif (_{0}=={1})\r", cm.Name, TypeNull[cm.CLRType]);
                    FiledCheck.Append("\t\t\t{\r");
                    FiledCheck.AppendFormat("\t\t\t\tthrow new Exception(\"{0} Value can not be null\");\r", cm.Name);
                    FiledCheck.Append("\t\t\t}\r");
                    Parameters.AppendFormat(
                        "\t\t\tStoredProcedureParam[{0}]=MakeParameter(\"@{1}\",(_{1}=={2}? System.Convert.DBNull : _{1}));\r",
                        i.ToString(), cm.Name, TypeNull[cm.CLRType]);
                }
                else
                {
                    Parameters.AppendFormat("\t\t\tStoredProcedureParam[{0}]=MakeParameter(\"@{1}\",_{1});\r",
                                            i.ToString(), cm.Name);
                }
                i++;
            }

            delete.Body.Append(FiledCheck.ToString());
            delete.Body.Append(Parameters.ToString());

            delete.Body.Append("\t\t\ttry\r");
            delete.Body.Append("\t\t\t{\r");
            delete.Body.AppendFormat("\t\t\t\tRunSqlCommand(\"[Delete_{0}]\",StoredProcedureParam,ref mReader);\r",
                                     tb.Name);
            delete.Body.Append("\t\t\t\tbool Res=false;\r");
            delete.Body.Append("\t\t\t\tif(mReader.Read())\r");            
            delete.Body.AppendFormat("\t\t\t\t\t\tRes=bool.Parse(mReader[0].ToString());\r");

            delete.Body.Append("\t\t\t\tmReader.Close();\r");
            delete.Body.Append("\t\t\t\tmReader=null;\r");
            delete.Body.Append("\t\t\t\tthis.Close();\r");
            delete.Body.Append("\t\t\t\treturn Res;\r");
            delete.Body.Append("\t\t\t}\r");
            delete.Body.Append("\t\t\t\tcatch (Exception ex)\r");
            delete.Body.Append("\t\t\t{\r");
            delete.Body.Append("\t\t\t\tif(mReader!=null)\r");
            delete.Body.Append("\t\t\t\t\tmReader=null;\r");
            delete.Body.Append("\t\t\t\t\tthis.Close();\r");
            delete.Body.Append("\t\t\t\tthrow ex;\r");
            delete.Body.Append("\t\t\t}\r");
            delete.Body.Append("\t\t}\r");
            return delete;
        }

        //private Method DataTypedSet(string ClassName)
        //{
        //    //Method loadinDataset = new Method();
        //    //loadinDataset.Name = "loadinDataset";

        //    ////Select All
        //    //loadinDataset.Body.AppendFormat("\t\tpublic static {0}Datasets.{1}DS SelectAll()\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append("\t\t{\r");
        //    //loadinDataset.Body.AppendFormat("\t\t\t {0}Datasets.{1}DS ResDS=new {0}Datasets.{1}DS();\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);

        //    //loadinDataset.Body.Append("\t\t\t RunStaticSqlCommand(\"Select * from " + tb.Name + "\", ResDS,\"" + tb.Name + "\");\r");

        //    //loadinDataset.Body.AppendFormat("\t\t\t return ({0}Datasets.{1}DS) ResDS ;\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append("\t\t}\r");
            
        //    ////Select All Order By
        //    //loadinDataset.Body.AppendFormat("\t\tpublic static {0}Datasets.{1}DS SelectAll(string OrderBY)\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append("\t\t{\r");
        //    //loadinDataset.Body.AppendFormat("\t\t\t {0}Datasets.{1}DS ResDS=new {0}Datasets.{1}DS();\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);

        //    //loadinDataset.Body.Append("\t\t\t RunStaticSqlCommand(\"Select * from " + tb.Name + " order by \" + OrderBY , ResDS,\"" + tb.Name + "\");\r");

        //    //loadinDataset.Body.AppendFormat("\t\t\t return ({0}Datasets.{1}DS) ResDS ;\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append("\t\t}\r");

        //    ////Select All Order By , Top
        //    //loadinDataset.Body.AppendFormat("\t\tpublic static {0}Datasets.{1}DS SelectAll(string OrderBY,int TopCount)\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append("\t\t{\r");
        //    //loadinDataset.Body.AppendFormat("\t\t\t {0}Datasets.{1}DS ResDS=new {0}Datasets.{1}DS();\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);

        //    //loadinDataset.Body.Append("\t\t\t RunStaticSqlCommand(\"Select TOP \" + TopCount.ToString() + \" * from " + tb.Name + " order by \" + OrderBY , ResDS,\"" + tb.Name + "\");\r");

        //    //loadinDataset.Body.AppendFormat("\t\t\t return ({0}Datasets.{1}DS) ResDS ;\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append("\t\t}\r");

        //    ////Select All offset and Count (paging)
        //    ////loadinDataset.Body.AppendFormat("\t\tpublic static {0}Datasets.{1}DS SelectAll(int offset,int count)\r",
        //    ////                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    ////loadinDataset.Body.Append("\t\t{\r");
        //    ////loadinDataset.Body.AppendFormat("\t\t\t {0}Datasets.{1}DS ResDS=new {0}Datasets.{1}DS();\r",
        //    ////                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);

        //    ////loadinDataset.Body.Append("\t\t\t RunStaticSqlCommand(\"Select * from " + tb.Name + "\", ResDS,\"" + tb.Name + "\");\r");

        //    ////loadinDataset.Body.AppendFormat("\t\t\t return ({0}Datasets.{1}DS) ResDS ;\r",
        //    ////                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    ////loadinDataset.Body.Append("\t\t}\r");


        //    /////---------------------------------------------------------------------------------------------------------------------///

        //    ////Select Where
        //    //loadinDataset.Body.AppendFormat("\t\tpublic static {0}Datasets.{1}DS SelectWhere(string FilterBy)\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append("\t\t{\r");
        //    //loadinDataset.Body.AppendFormat("\t\t\t {0}Datasets.{1}DS  ResDS=new {0}Datasets.{1}DS();\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append(
        //    //    "\t\t\t RunStaticSqlCommand(\"Select * from " + tb.Name + " where \" + FilterBy, ResDS, \"" + tb.Name + "\");\r");
        //    //loadinDataset.Body.AppendFormat("\t\t\t return ({0}Datasets.{1}DS) ResDS ; \r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append("\t\t}\r");


        //    ////Select Where Top OrderBY
        //    //loadinDataset.Body.AppendFormat("\t\tpublic static {0}Datasets.{1}DS SelectWhere(string FilterBy,int TopCount,string OrderBY)\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append("\t\t{\r");
        //    //loadinDataset.Body.AppendFormat("\t\t\t {0}Datasets.{1}DS  ResDS=new {0}Datasets.{1}DS();\r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append(
        //    //    "\t\t\t RunStaticSqlCommand(\"Select top \" + TopCount.ToString() + \" * from " + tb.Name + " where \" + FilterBy +\" order by \" + OrderBY , ResDS, \"" + tb.Name + "\");\r");
        //    //loadinDataset.Body.AppendFormat("\t\t\t return ({0}Datasets.{1}DS) ResDS ; \r",
        //    //                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    //loadinDataset.Body.Append("\t\t}\r");


        //    ////Select Where offset and count (Paging)
        //    ////loadinDataset.Body.AppendFormat("\t\tpublic static {0}Datasets.{1}DS SelectWhere(string FilterBy,int offset,int count)\r",
        //    ////                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    ////loadinDataset.Body.Append("\t\t{\r");
        //    ////loadinDataset.Body.AppendFormat("\t\t\t {0}Datasets.{1}DS  ResDS=new {0}Datasets.{1}DS();\r",
        //    ////                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    ////loadinDataset.Body.Append(
        //    ////    "\t\t\t RunStaticSqlCommand(\"Select * from " + tb.Name + " where \" + FilterBy, ResDS, \"" + tb.Name + "\");\r");
        //    ////loadinDataset.Body.AppendFormat("\t\t\t return ({0}Datasets.{1}DS) ResDS ; \r",
        //    ////                                _NameSpace.Length > 0 ? (_NameSpace + @".") : @"", ClassName);
        //    ////loadinDataset.Body.Append("\t\t}\r");

        //    //return loadinDataset;
            
        //}

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
            loadinDataset.Body.AppendFormat("\t\t\t{0} result=LoadFromDataSet(ResDS,0) ;\r", _Prefix.Length > 0 ? (_Prefix + ClassName) : ClassName);
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
        #endregion
    }
}