using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Sql2005Server.SchemaProvider;
using Sql2005Server;
using SchemaObjects;
using View = SchemaObjects.View;

namespace DataObjectGen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (SavePath.Text.Trim().Length < 1)
            {
                MessageBox.Show("Please fill the save path");
                return;
            }

            if (DatabaseName.Text.Trim().Length < 1)
            {
                MessageBox.Show("Please fill the database name field");
                return;
            }
            string expath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            SQL2005SchemaProvider sm = new SQL2005SchemaProvider();
            sm.Connect(@"samyhp");
            System.Collections.ArrayList dbs = sm.DataBases();
            foreach (string dbname in dbs)
            {
                System.Diagnostics.Debug.WriteLine(dbname);
            }
            DataBase db = new DataBase();
            db.Name = DatabaseName.Text;

            SchemaObjects.Tables tbls = sm.Tables(db);
            SchemaObjects.Views views = sm.Views(db);
            TableToClassTemplate codegen = new TableToClassTemplate(); //generate the table class.
            TableToPartialClassTemplate codegen1 = new TableToPartialClassTemplate(); /// generate a partial empty class to add logic in it.
            DataTypedSetTemplate typedset = new DataTypedSetTemplate(); // generate a typed dataset for every table .
            TableStoredProcedures tbsp = new TableStoredProcedures();
            string strClassPrefix = ClassPrefix.Text;
            string strNameSpace = NameSpacetxt.Text;

            string path = Path.Combine(SavePath.Text, db.Name);
            DirectoryInfo df = new DirectoryInfo(path);
            if (!df.Exists)
                df.Create();

            FileInfo oinfo = new FileInfo(Path.Combine( Path.Combine(SavePath.Text, db.Name), "dbStoredProcedures" + @".sql"));
            FileStream sqlFile;

            if (oinfo.Exists)
                sqlFile = oinfo.Open(FileMode.Truncate, FileAccess.ReadWrite);
            else
                sqlFile = oinfo.Create();

            using (StreamWriter sw = new StreamWriter(sqlFile))
            {
                tbsp.Streamwriter = sw;

                foreach (Table tb in db.Tables)
                {
                    codegen.OutputDir = Path.Combine(SavePath.Text, db.Name);
                    codegen1.OutputDir = codegen.OutputDir;
                    typedset.OutputDir = codegen1.OutputDir;
                    tbsp.OutputDir = codegen.OutputDir;

                    GeneratePages gp=new GeneratePages();
                    gp.OutputDir = tbsp.OutputDir;
                    gp.Process(tb,strNameSpace,strClassPrefix,true);
                    tbsp.Process(tb, strNameSpace, strClassPrefix, true);
                    codegen.Process(tb, strNameSpace, strClassPrefix, true);
                    typedset.Process(tb, strNameSpace, strClassPrefix, true);
                    codegen1.Process(tb, strNameSpace, strClassPrefix, true);

                    typedset.SaveToFile();
                    codegen1.SaveToFile();
                    codegen.SaveToFile();
                    tbsp.SaveToFile();

                }

                foreach (View view in db.Views)
                {                    
                    ViewToClass vtc=new ViewToClass();
                    vtc.OutputDir = Path.Combine(SavePath.Text, db.Name);

                    vtc.Process(view,strNameSpace,strClassPrefix,true);
                    vtc.SaveToFile();

                }

                sw.Flush();
                sw.Close();
            }

            SaveBaseObject(strNameSpace,Path.Combine( Path.Combine(SavePath.Text, db.Name),"DataBaseObject.cs"));
            MessageBox.Show("Files Has been generated.");
            //this.Close();
        }

        private void SaveBaseObject(string NameSpace,string Filename)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("DataObjectGen.DataObjectBase.txt");
            StreamReader sr = new StreamReader(stream);

            string filecontent = sr.ReadToEnd().Replace("{0}", NameSpace);

            filecontent = filecontent.Replace("{1}", "localhost");
            filecontent = filecontent.Replace("{2}", DatabaseName.Text);
            filecontent = filecontent.Replace("{3}","sa" );
            filecontent = filecontent.Replace("{4}", "p7z6y1f9");


            FileInfo oinfo = new FileInfo(Filename);
            FileStream sqlFile;
            if (oinfo.Exists)
                sqlFile = oinfo.Open(FileMode.Truncate, FileAccess.ReadWrite);
            else
                sqlFile = oinfo.Create();

            using (StreamWriter sw = new StreamWriter(sqlFile))
            {
                sw.Write(filecontent);
                sw.Flush();
                sw.Close();
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}