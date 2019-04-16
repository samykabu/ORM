using System;
using System.Collections.Generic;
using System.Text;
using DatabaseObjects.DB.Collections;
using DatabaseObjects.DB;

namespace DatabaseObjects
{
    abstract public class ISchemaProvider
    {
        public abstract void OpenDataBase(string ConnectionString);
        public abstract void Close();        
        public abstract Databases ReadDataBases();
        public abstract Database ReadDataBase(string databasename);

        public abstract Tables ReadTables(Database db);
        public abstract Tables ReadTables(Database db,List<string> tableslist);
        public abstract Table ReadTable(Database db,string tablename);

        public abstract Views ReadViews(Database db);
        public abstract Views ReadViews(Database db,List<string> viewslist);
        public abstract View ReadView(Database db,string viewname);

        public abstract StoredProcedures ReadStoredProcedures(Database db);
        public abstract StoredProcedures ReadStoredProcedures(Database db,List<string> storedproclist);
        public abstract StoredProcedure ReadStoredProcedure(Database db,string storedproname);        
    }

   
}
