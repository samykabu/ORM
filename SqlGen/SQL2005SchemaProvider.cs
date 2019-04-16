using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Smo;
using SchemaObjects;
using Sql2005Server.SchemaProvider.Forms;
using Column=Microsoft.SqlServer.Management.Smo.Column;
using Table=Microsoft.SqlServer.Management.Smo.Table;
using View = SchemaObjects.View;

namespace Sql2005Server.SchemaProvider
{
    public class SQL2005SchemaProvider : ISchemaProvider
    {
        private ConnectionBuilder _ConnectionBuilder = new ConnectionBuilder();
        private Server sqlserver;
        private StringDictionary SQLToCLR = new StringDictionary();

        public SQL2005SchemaProvider()
        {
            SQLToCLR.Add("char", "string");
            SQLToCLR.Add("varchar", "string"); /* the is not bug*/

            SQLToCLR.Add("nchar", "string");
            SQLToCLR.Add("nvarchar", "string");

            SQLToCLR.Add("ntext", "string");
            SQLToCLR.Add("text", "string");

            SQLToCLR.Add("binary", "byte[]");
            SQLToCLR.Add("varbinary", "byte[]");

            SQLToCLR.Add("image", "byte[]");
            SQLToCLR.Add("timestamp", "byte[]");

            SQLToCLR.Add("bit", "bool");
            SQLToCLR.Add("tinyint", "byte");
            SQLToCLR.Add("smallint", "short");
            SQLToCLR.Add("int", "Int32");
            SQLToCLR.Add("bigint", "Int64");
            SQLToCLR.Add("datetime", "DateTime");
            SQLToCLR.Add("datetime2", "DateTime");
            SQLToCLR.Add("date", "DateTime");
            SQLToCLR.Add("time", "DateTime");
            SQLToCLR.Add("smalldatetime", "DateTime");
            SQLToCLR.Add("money", "decimal");
            SQLToCLR.Add("smallmoney", "decimal");
            SQLToCLR.Add("numeric", "decimal");
            SQLToCLR.Add("decimal", "decimal");
            SQLToCLR.Add("float", "double");
            SQLToCLR.Add("real", "Single");
            SQLToCLR.Add("", "string"); //xml
            SQLToCLR.Add("uniqueidentifier", "Guid");
        }

        public override void Connect(string ConnectionString)
        {
            sqlserver = new Server(ConnectionString);
        }

        public override void Close()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override IConnectionStringBuilder ConnectionStringBuilder()
        {
            return _ConnectionBuilder;
        }

        public override ArrayList DataBases()
        {
            if (sqlserver == null)
                return null;

            ArrayList dbs = new ArrayList();
            foreach (Database db in sqlserver.Databases)
            {
                dbs.Add(db.Name);
            }
            return dbs;
        }

        public override Tables Tables(DataBase Db)
        {
            Tables tbs = Db.Tables;
            foreach (Table tb in sqlserver.Databases[Db.Name].Tables)
            {
                SchemaObjects.Table tbl = new SchemaObjects.Table();
                tbl.Name = tb.Name;
                //tbl.Name = tb.Name;
                if (!tb.IsSystemObject)
                {
                    try
                    {
                        tbl.Description = tb.ExtendedProperties["MS_Description"] != null
                                              ? (string) tb.ExtendedProperties["MS_Description"].Value.ToString().Replace('\r',' ').Replace('\n',' ')
                                              : " No Description Implemented";
                        
                        foreach (Column cm in tb.Columns)
                        {
                            if (cm.Computed == false)
                            {
                                SchemaObjects.Column clm = new SchemaObjects.Column();
                                clm.Name = cm.Name;
                                switch (cm.DataType.Name)
                                {
                                    case "":
                                        clm.SqlType = "xml";
                                        break;

                                    case "decimal":
                                        clm.SqlType = cm.DataType.Name + "(" + cm.DataType.NumericPrecision.ToString() +
                                                      "," + cm.DataType.NumericScale.ToString() + ")";
                                        break;
                                    case "numeric":
                                        clm.SqlType = cm.DataType.Name + "(" + cm.DataType.NumericPrecision.ToString() +
                                                      "," + cm.DataType.NumericScale.ToString() + ")";
                                        break;
                                    default:
                                        clm.SqlType = cm.DataType.Name;
                                        break;
                                }
                                if (cm.DataType.Name.ToLower().IndexOf("var") >= 0)
                                {
                                    if (cm.DataType.MaximumLength != -1)
                                        clm.SqlType = cm.DataType.Name + "(" + cm.DataType.MaximumLength + ")";
                                    else
                                        clm.SqlType = cm.DataType.Name + "(max)";
                                }

                                
                                clm.IsComputed = cm.Computed;
                                clm.ComputedSQLText = cm.ComputedText;
                                
                                clm.DefaultValue = cm.Default;
                                clm.Description = cm.ExtendedProperties["MS_Description"] != null
                                                      ? (string)cm.ExtendedProperties["MS_Description"].Value.ToString().Replace('\r', ' ').Replace('\n', ' ')
                                                      : "  No Description Implemented";
                                clm.IsNullable = cm.Nullable;
                                clm.Length = cm.DataType.MaximumLength;
                                clm.Readonly = cm.Identity;
                                if (clm.Readonly)
                                    tbl.IdentityColumn = clm;
                                clm.CLRType = SQLToCLR[cm.DataType.Name];
                                clm.IsForignKey = cm.IsForeignKey;                                

                                tbl.AddColumn(clm);                                    
                                
                                if (cm.InPrimaryKey)
                                {
                                    clm.InPrimaryKey = true;
                                    tbl.AddPKey(cm.Name);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    foreach (ForeignKey foreignKey in tb.ForeignKeys)
                    {
                        foreach (ForeignKeyColumn column in foreignKey.Columns)
                        {
                            if (tbl.Columns.Contains(column.Name))
                            {
                                tbl.AddfKey(column.Parent.ReferencedTable, column.Parent.Columns[0].Name, (SchemaObjects.Column)tbl.Columns[column.Name]);                                    
                            }
                        }
                    }
                    tbs.Add(tbl);
                }
            }
            return tbs;
        }
       
        public override Views Views(DataBase dataBase)
        {
            Views views = dataBase.Views;

            foreach (Microsoft.SqlServer.Management.Smo.View view in sqlserver.Databases[dataBase.Name].Views)
            {
                if (!view.IsSystemObject)
                {
                    View dview=new View();

                    dview.Name = view.Name;
                    System.Diagnostics.Debug.WriteLine(view.Name);
                    foreach (Column cm in view.Columns)
                    {                       
                            if (cm.Computed == false)
                            {
                                SchemaObjects.Column clm = new SchemaObjects.Column();
                                clm.Name = cm.Name;
                                switch (cm.DataType.Name)
                                {
                                    case "":
                                        clm.SqlType = "xml";
                                        break;

                                    case "decimal":
                                        clm.SqlType = cm.DataType.Name + "(" + cm.DataType.NumericPrecision.ToString() +
                                                      "," + cm.DataType.NumericScale.ToString() + ")";
                                        break;
                                    case "numeric":
                                        clm.SqlType = cm.DataType.Name + "(" + cm.DataType.NumericPrecision.ToString() +
                                                      "," + cm.DataType.NumericScale.ToString() + ")";
                                        break;
                                    default:
                                        clm.SqlType = cm.DataType.Name;
                                        break;
                                }
                                if (cm.DataType.Name.ToLower().IndexOf("var") >= 0)
                                {
                                    if (cm.DataType.MaximumLength != -1)
                                        clm.SqlType = cm.DataType.Name + "(" + cm.DataType.MaximumLength + ")";
                                    else
                                        clm.SqlType = cm.DataType.Name + "(max)";
                                }

                                clm.DefaultValue = cm.Default;
                                clm.Description = cm.ExtendedProperties["MS_Description"] != null
                                                      ? (string)cm.ExtendedProperties["MS_Description"].Value.ToString().Replace('\r', ' ').Replace('\n', ' ')
                                                      : "  No Description Implemented";
                                clm.IsNullable = cm.Nullable;
                                clm.Length = cm.DataType.MaximumLength;
                                clm.Readonly = cm.Identity;                                
                                clm.CLRType = SQLToCLR[cm.DataType.Name];                                
                                dview.AddColumn(clm);                                                            
                        }
                    }
                    views.Add(dview);
                }
            }
            return views;
        }

        public override StoredProcedures StoredProcedures(DataBase dataBase)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataBaseObjects SupportedObjects()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string DatabaseCreationScript(DataBase dataBase)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string DatabaseObjectCreationScript(DataBase dataBase)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}