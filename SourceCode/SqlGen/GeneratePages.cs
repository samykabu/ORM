using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CodeGenerator;
using SchemaObjects;

namespace Sql2005Server
{
    public class GeneratePages : ITemplate
    {
        private string _NameSpace = "";
        private string _Prefix = "";
        private Table oTable = null;
        Dictionary<string, string> ParseCLR = new Dictionary<string, string>();

        private string RequiredTextBoxField =
            "<asp:Label Text=\"{0}\" ID=\"{0}lbl\" runat=\"server\" AssociatedControlID=\"{0}txb\" /><asp:TextBox ID=\"{0}txb\" runat=\"server\" /><asp:RequiredFieldValidator ID=\"{0}Val\" runat=\"server\" ControlToValidate=\"{0}txb\" ErrorMessage=\"*\" Text=\"*\"/>\r\n";

        private string RequiredDropDownField =
            "<asp:Label Text=\"{0}\" ID=\"{0}lbl\" runat=\"server\" AssociatedControlID=\"{0}drp\" /><asp:DropDownList ID=\"{0}drp\" runat=\"server\" /><asp:RequiredFieldValidator ID=\"{0}Val\" runat=\"server\" ControlToValidate=\"{0}drp\" ErrorMessage=\"*\" Text=\"*\"/>\r\n";

        private string RequiredFileField =
    "<asp:Label Text=\"{0}\" ID=\"{0}lbl\" runat=\"server\" AssociatedControlID=\"{0}fil\" /><asp:FileUpload ID=\"{0}fil\" runat=\"server\" /><asp:RequiredFieldValidator ID=\"{0}Val\" runat=\"server\" ControlToValidate=\"{0}fil\" ErrorMessage=\"*\" Text=\"*\"/>\r\n";


        private string TextBoxField =
    "<asp:Label Text=\"{0}\" ID=\"{0}lbl\" runat=\"server\" AssociatedControlID=\"{0}txb\" /><asp:TextBox ID=\"{0}txb\" runat=\"server\" />\r\n";

        private string DropDownField =
            "<asp:Label Text=\"{0}\" ID=\"{0}lbl\" runat=\"server\" AssociatedControlID=\"{0}drp\" /><asp:DropDownList ID=\"{0}drp\" runat=\"server\" />\r\n";
        private string FileField =
    "<asp:Label Text=\"{0}\" ID=\"{0}lbl\" runat=\"server\" AssociatedControlID=\"{0}fil\" /><asp:FileUpload ID=\"{0}fil\" runat=\"server\" />\r\n";

        private string saveTextcolumn = "{0}={1};\r\n";
        private string LoadTextcolumn = "{1}={0};\r\n";
        private string saveDropDown = "{0}={1};\r\n";
        private string LoadDropDown = "{1}.DataSource={2};\r\n {1}.DataBind();\r\n {3}={0};\r\n";

        private StringBuilder formfileds = new StringBuilder();
        private StringBuilder saveFunction = new StringBuilder();
        private StringBuilder LoadFunction = new StringBuilder();
        private StringBuilder UpdateFunction = new StringBuilder();
        private StringBuilder ForiegnKeyLoaders = new StringBuilder();
        private string pKeys = "";
        private string pKeysParam = "";
        public override void Process(SchemaObjects.IDataBaseObject DbObject, string NameSpace, string Prefix, bool bSerialize)
        {
            _NameSpace = NameSpace;
            _Prefix = Prefix;
            IntializeParser();

            if ((DbObject) is Table)
            {
                oTable = (Table)DbObject;
                string classname = _Prefix.Length > 0 ? (_Prefix + oTable.Name) : oTable.Name;

                foreach (Column cm in oTable.Columns.Values)
                {
                    System.Diagnostics.Debug.WriteLine(cm.CLRType + " <---->" + cm.Name);
                    if (!cm.InPrimaryKey)
                    {
                        if (cm.IsNullable && !cm.IsForignKey && !cm.IsComputed)
                        {

                            switch (cm.CLRType)
                            {
                                case "byte[]":
                                    formfileds.AppendFormat(FileField, cm.Name);
                                    saveFunction.AppendFormat(saveTextcolumn, classname + "." + cm.Name, cm.Name + "fil.HasFile ? " + cm.Name + ".fil.FileBytes : null");
                                    break;
                                default:
                                    formfileds.AppendFormat(TextBoxField, cm.Name);
                                    saveFunction.AppendFormat(saveTextcolumn, classname + "." + cm.Name, string.Format(ParseCLR[cm.CLRType], cm.Name + "txb.Text"));
                                    LoadFunction.AppendFormat(LoadTextcolumn, "o" + oTable.Name + "." + cm.Name, cm.Name + "txb.Text");
                                    break;
                            }
                        }
                        else if (!cm.IsForignKey && !cm.IsComputed)
                        {
                            switch (cm.CLRType)
                            {
                                case "byte[]":
                                    formfileds.AppendFormat(RequiredFileField, cm.Name);
                                    //.FileBytes
                                    saveFunction.AppendFormat(saveTextcolumn, classname + "." + cm.Name, string.Format(ParseCLR[cm.CLRType], cm.Name + "fil.FileBytes"));
                                    //LoadFunction.AppendFormat(LoadTextcolumn, classname + "." + cm.Name, cm.Name + "txb.Text");
                                    break;
                                default:
                                    formfileds.AppendFormat(RequiredTextBoxField, cm.Name);
                                    saveFunction.AppendFormat(saveTextcolumn, classname + "." + cm.Name, string.Format(ParseCLR[cm.CLRType], cm.Name + "txb.Text"));
                                    LoadFunction.AppendFormat(LoadTextcolumn, "o" + oTable.Name + "." + cm.Name, cm.Name, cm.Name + "txb.Text");
                                    break;
                            }

                        }
                        else if (cm.IsForignKey)
                        {
                            if (cm.IsNullable)
                                formfileds.AppendFormat(DropDownField, cm.Name);
                            else
                                formfileds.AppendFormat(RequiredDropDownField, cm.Name);

                            saveFunction.AppendFormat(saveDropDown, classname + "." + cm.Name, string.Format(ParseCLR[cm.CLRType], cm.Name + "drp.SelectedValue"));
                            LoadFunction.AppendFormat(LoadDropDown, "o" + oTable.Name + "." + cm.Name, cm.Name + "drp", oTable.ForiegnKeys.Item(cm.Name).RefrencedTableName + ".SelectAll();", cm.Name + "drp.SelectedValue");
                        }
                    }
                    else
                    {
                        pKeysParam += cm.CLRType + " " + cm.Name.ToLower() + ",";
                        pKeys += cm.Name.ToLower() + ",";
                    }
                }


                UpdateFunction.Insert(0, "private bool UpdateRecord(" + pKeysParam.Substring(0, pKeysParam.Length - 1) + ")\r\n{\r\n " + classname + " o" + oTable.Name + " =new " + classname + "(" + pKeys.Substring(0, pKeys.Length - 1) + ");\r\n");
                UpdateFunction.Append(saveFunction.ToString());
                UpdateFunction.Append("\r\n bool res=" + classname + ".Update();\r\n " + classname +
                                    ".Dispose();\r\n return res;\r\n}\r\n");

                LoadFunction.Insert(0, "private bool loadRecordInUI(" + pKeysParam.Substring(0, pKeysParam.Length - 1) + ")\r\n{\r\n " + classname + " o" + oTable.Name + " =new " + classname + "(" + pKeys.Substring(0, pKeys.Length - 1) + ");\r\n");
                LoadFunction.AppendFormat("o" + oTable.Name + ".Dispose();\r\n " + "o" + oTable.Name + "=null;\r\n");
                LoadFunction.Append("}\r\n");

                saveFunction.Insert(0, "private bool saveRecord()\r\n{\r\n " + classname + " o" + oTable.Name + " =new " + classname + "();\r\n");
                saveFunction.Append("\r\n bool res=" + classname + ".Insert();\r\n " + classname +
                                    ".Dispose();\r\n return res;\r\n}\r\n");

                string path = Path.Combine(OutputDir, "Pages");
                DirectoryInfo df = new DirectoryInfo(path);
                if (!df.Exists)
                    df.Create();

                FileInfo oinfo = new FileInfo(Path.Combine(path, classname + @"CodeBehind.cs"));
                FileStream sqlFile;
                if (oinfo.Exists)
                    sqlFile = oinfo.Open(FileMode.Truncate, FileAccess.ReadWrite);
                else
                    sqlFile = oinfo.Create();

                using (StreamWriter sw = new StreamWriter(sqlFile))
                {
                    sw.Write(LoadFunction.ToString());
                    sw.Write(saveFunction.ToString());
                    sw.Write(UpdateFunction.ToString());
                    sw.Flush();
                    sw.Close();
                }
                sqlFile.Close();

                oinfo = new FileInfo(Path.Combine(path, classname + @"Web.Aspx"));

                if (oinfo.Exists)
                    sqlFile = oinfo.Open(FileMode.Truncate, FileAccess.ReadWrite);
                else
                    sqlFile = oinfo.Create();

                using (StreamWriter sw = new StreamWriter(sqlFile))
                {
                    sw.Write(formfileds.ToString());
                    sw.Flush();
                    sw.Close();
                }
                sqlFile.Close();
            }

            System.Diagnostics.Debug.WriteLine(formfileds.ToString());
            System.Diagnostics.Debug.WriteLine(saveFunction.ToString());
            System.Diagnostics.Debug.WriteLine(UpdateFunction.ToString());
            System.Diagnostics.Debug.WriteLine(LoadFunction.ToString());
        }

        private void IntializeParser()
        {
            ParseCLR.Add("string", "{0}");
            ParseCLR.Add("Int64", "Int64.Parse({0})");
            ParseCLR.Add("int", "int.Parse({0})");
            ParseCLR.Add("Int16", "Int16.Parse({0})");
            ParseCLR.Add("Int32", "Int32.Parse({0})");
            ParseCLR.Add("short", "short.Parse({0})");
            ParseCLR.Add("byte[]", "{0}");
            ParseCLR.Add("DateTime", "DateTime.Parse({0})");

            ParseCLR.Add("bool", "bool.Parse({0})");
            ParseCLR.Add("decimal", "decimal.Parse({0})");

            ParseCLR.Add("double", "double.Parse({0})");
            ParseCLR.Add("Single", "Single.Parse({0})");

            ParseCLR.Add("Guid", "(Guid){0}");
        }

        public override void SaveToFile()
        {


        }

        protected override bool SupportedDatabaseObject(SchemaObjects.IDataBaseObject DbObject)
        {
            return false;
        }

        public override string ToString()
        {
            return "";
        }
    }
}

