using System;
using System.IO;
using System.Text;
using CodeGenerator;
using SchemaObjects;

namespace Sql2005Server
{
    public class DataTypedSetTemplate : ITemplate
    {
        private Table tb = null;
        public StringBuilder typedset = new StringBuilder();

        public override void Process(IDataBaseObject DbObject, string NameSpace, string Prefix, bool bSerialize)
        {
            typedset = new StringBuilder();
            if ((DbObject) is Table && SupportedDatabaseObject(DbObject))
            {
                tb = (Table)DbObject;
                ProduceCode(tb);
            }
        }

        public override void SaveToFile()
        {
            if (typedset.Length > 0)
            {
                string path = Path.Combine(OutputDir, "Datasets");
                DirectoryInfo df = new DirectoryInfo(path);
                if (!df.Exists)
                    df.Create();

                FileInfo oinfo = new FileInfo(Path.Combine(path, tb.Name + @".xsd"));
                if (oinfo.Exists)
                    oinfo.Delete();

                using (StreamWriter sw = oinfo.CreateText())
                {
                    sw.Write(typedset.ToString());
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
            return typedset.ToString();
        }


        public void WriteLine(string str)
        {
            typedset.Append(str);
            typedset.Append("\r");
        }

        public void ProduceCode(Table Table)
        {
            string TableName = Table.Name;
            string dTableNmae = TableName.Replace(".", "_");
            string cTableNmae = TableName.Replace("[", "").Replace("]", "");
            string fTableNmae = cTableNmae.Replace(".", "_");

            WriteLine(@"<?xml version=""1.0"" encoding=""utf-8"" ?>");
            WriteLine(@"<xs:schema id=""" + fTableNmae + @"DS"" targetNamespace=""http://tempuri.org/" + fTableNmae +
                      @"DS.xsd"" elementFormDefault=""qualified""");
            WriteLine(@"	attributeFormDefault=""qualified"" xmlns=""http://tempuri.org/" + fTableNmae +
                      @"DS.xsd"" xmlns:mstns=""http://tempuri.org/" + fTableNmae + @"DS.xsd""");
            WriteLine(
                @"	xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"" xmlns:msprop=""urn:schemas-microsoft-com:xml-msprop"" >");
            WriteLine(@"	<xs:element name=""" + fTableNmae + @"DS"" msdata:IsDataSet=""true"">");
            WriteLine(@"		<xs:complexType>");
            WriteLine(@"			<xs:choice maxOccurs=""unbounded"">");
            WriteLine(@"				<xs:element name=""" + cTableNmae + @""">");
            WriteLine(@"					<xs:complexType>");
            WriteLine(@"						<xs:sequence>");
            foreach (Column column in Table.Columns.Values)
            {
                if (column.Readonly)
                {
                    WriteLine(@"				<xs:element name=""" + column.Name +
                              @""" msdata:ReadOnly=""true"" msdata:AutoIncrement=""true"" type=""" +
                              GetXMLMappings(column) + @""" />");
                }
                else
                {
                    if (column.IsNullable)
                    {
                        if (ColumnIsString(column))
                        {
                            WriteLine(@"				<xs:element name=""" + column.Name +
                                      @"""  minOccurs=""0"" msprop:nullValue=""_null"">");
                            WriteLine(@"				    <xs:simpleType>");
                            WriteLine(@"				        <xs:restriction base=""" + GetXMLMappings(column) + @""">");
                            if (column.SqlType.ToLower() != "text" && column.SqlType.ToLower() != "ntext")
                                WriteLine(@"				            <xs:maxLength value=""" +
                                          (column.Length == -1 ? Int32.MaxValue.ToString() : column.Length.ToString()) +
                                          @"""/>");
                            WriteLine(@"				        </xs:restriction>");
                            WriteLine(@"				    </xs:simpleType>");
                            WriteLine(@"				</xs:element>");
                        }
                        else
                        {
                            WriteLine(@"				<xs:element name=""" + column.Name + @""" type=""" + GetXMLMappings(column) +
                                      @"""  minOccurs=""0""/>");
                        }
                    }
                    else
                    {
                        if (ColumnIsString(column))
                        {
                            WriteLine(@"				<xs:element name=""" + column.Name + @""" >");
                            WriteLine(@"				    <xs:simpleType>");
                            WriteLine(@"				        <xs:restriction base=""" + GetXMLMappings(column) + @""" >");
                            if (column.SqlType.ToLower() != "text" && column.SqlType.ToLower() != "ntext")
                                WriteLine(@"				            <xs:maxLength value=""" +
                                          (column.Length == -1 ? Int32.MaxValue.ToString() : column.Length.ToString()) +
                                          @"""/>");
                            WriteLine(@"				        </xs:restriction>");
                            WriteLine(@"				    </xs:simpleType>");
                            WriteLine(@"				</xs:element>");
                        }
                        else
                        {
                            WriteLine(@"				<xs:element name=""" + column.Name + @""" type=""" + GetXMLMappings(column) +
                                      @""" />");
                        }
                    }
                }
                //if (column.GetLOV().Count > 0)
                //{
                //    foreach (Column referenceColumn in column.GetLOV())
                //    {
                //        WriteLine(@"				<xs:element name=""" + column.Name + "_" + referencecolumn.Name + @""" type=""" + GetXMLMappings(referenceColumn) + @"""  minOccurs=""0"" msprop:nullValue=""_null""/>");
                //    }
                //}
            }
            WriteLine(@"						</xs:sequence>");
            WriteLine(@"					</xs:complexType>");
            WriteLine(@"				</xs:element>");
            WriteLine(@"			</xs:choice>");
            WriteLine(@"		</xs:complexType>");

            if (Table.PrimaryKeys.Count > 0)
            {
                WriteLine(@"		<xs:unique name=""PK" + cTableNmae + @""" msdata:PrimaryKey=""true"">");
                WriteLine(@"			<xs:selector xpath="".//mstns:" + cTableNmae + @""" />");
                foreach (Column column in Table.PrimaryKeys.Values)
                {
                    WriteLine(@"			<xs:field xpath=""mstns:" + column.Name + @""" />");
                }
                WriteLine(@"		</xs:unique>");
            }
            WriteLine(@"	</xs:element>");
            WriteLine(@"</xs:schema>");
        }

        /// <summary>
        /// http://msdn2.microsoft.com/en-us/library/ms190942.aspx
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetXMLMappings(Column column)
        {
            switch (column.CLRType.ToLower())
            {
                case "string":
                    return "xs:string";
                case "byte":
                    return "xs:base64Binary";
                case "bool":
                    return "xs:boolean";
                case "byte[]":
                    return "xs:base64Binary";
                case "datetime":
                    return "xs:dateTime";
                case "decimal":
                    return "xs:decimal";
                case "double":
                case "float":
                    return "xs:double";
                case "guid":
                    return "xs:string";
                case "image":
                    return "xs:base64Binary";
                case "short":
                case "int16":
                case "int32":
                case "uint16":
                case "uint32":
                    return "xs:int";
                case "int64":
                case "uint64":
                    return "xs:long";
                default:
                    return "xs:string";
            }
        }

        public bool ColumnIsString(Column column)
        {
            switch (column.CLRType.ToLower())
            {
                case "string":
                    return true;
                default:
                    return false;
            }
        }
    }
}