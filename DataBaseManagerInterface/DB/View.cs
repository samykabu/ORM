using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SchemaObjects
{
    public class View:IDataBaseObject
    {
        private string _Name;
        private string _Description;
        private Hashtable _Columns = new Hashtable();

        public void AddColumn(Column col)
        {
            this._Columns.Add(col.Name, col);
        }

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

        public Hashtable Columns
        {
            get { return _Columns; }
            set { _Columns = value; }
        }

        private bool _Selected = false;
        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; }
        }
    }
}
