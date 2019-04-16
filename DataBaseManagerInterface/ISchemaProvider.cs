using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaObjects
{
    abstract public class ISchemaProvider
    {
        public abstract void Connect(string ConnectionString);
        public abstract void Close();
        public abstract IConnectionStringBuilder ConnectionStringBuilder();
        public abstract System.Collections.ArrayList DataBases();
        public abstract Tables Tables(DataBase db);
        public abstract Views Views(DataBase dataBase);
        public abstract StoredProcedures StoredProcedures(DataBase dataBase);
        public abstract DataBaseObjects SupportedObjects();
        public abstract string DatabaseCreationScript(DataBase dataBase);
        public abstract string DatabaseObjectCreationScript(DataBase dataBase);     
    }
}
