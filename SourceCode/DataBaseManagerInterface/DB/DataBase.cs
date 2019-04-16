using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaObjects
{
    [Flags]
    public enum DataBaseObjects
    {
        Tables,
        Views,
        StoredProcedures
    }

    public class DataBase
    {

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
	
        private Tables _Tables=new Tables();
        public Tables Tables
        {
            get { return _Tables; }
            set { _Tables = value; }
        }
        private Views _Views=new Views();

        public Views Views
        {
            get { return _Views; }
            set { _Views = value; }
        }

        private StoredProcedures _SPs=new StoredProcedures();

        public StoredProcedures StoredProcedures
        {
            get { return _SPs; }
            set { _SPs = value; }
        }

        private DataBaseObjects _SupportedObjects;

        public DataBaseObjects SupportedObjects
        {
            get { return _SupportedObjects; }
            set { _SupportedObjects = value; }
        }

        public bool SupportTables
        {
            get { return (SupportedObjects == DataBaseObjects.Tables); }
            
        }

        public bool SupportViews
        {
            get { return (SupportedObjects == DataBaseObjects.Views); }
            
        }        

        public bool supportStoredProcedures
        {
            get { return (SupportedObjects == DataBaseObjects.StoredProcedures); }
            
        }

	
    }
}
