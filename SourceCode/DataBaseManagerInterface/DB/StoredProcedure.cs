using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SchemaObjects
{
    public class StoredProcedure:IDataBaseObject
    {
        private string _Name;
        private string _Description;
        private StoredProcedureParameters _Parameters = new StoredProcedureParameters();

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public StoredProcedureParameters Parameters
        {
            get { return _Parameters; }
            set { _Parameters = value; }
        }

        private bool _Selected = false;
        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; }
        }
    }


    public class Parameter
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private string _DbOrignalType;

        public string DbOrignalType
        {
            get { return _DbOrignalType; }
            set { _DbOrignalType = value; }
        }
        private string _CLrType;

        public string CLrType
        {
            get { return _CLrType; }
            set { _CLrType = value; }
        }

        public Hashtable Attributes = new Hashtable();
    }
}
