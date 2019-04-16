using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;


namespace SchemaObjects
{
    public class Table:IDataBaseObject
    {
        #region Internal Variables

        private Hashtable _Columns = new Hashtable();
        private Column _IdentityColumn = null;
        private Hashtable _PrimaryKeys = new Hashtable();
        private ForiegnKeys _ForiegnKeys = new ForiegnKeys();
        private string _Name;
        private string _Description;
        
        #endregion

        private bool _Selected = false;
        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; }
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

        public Hashtable PrimaryKeys
        {
            get { return _PrimaryKeys; }
            set { _PrimaryKeys = value; }
        }

        public ForiegnKeys ForiegnKeys
        {
            get { return _ForiegnKeys; }
            set { _ForiegnKeys = value; }
        }

        public void AddColumn(Column col)
        {
            this._Columns.Add(col.Name,col);
        }

        public void AddPKey(string pKeyName)
        {
            _PrimaryKeys.Add(pKeyName, _Columns[pKeyName]);
        }

        public void AddfKey(string RefrencedTable, string RefrencedColumnName,Column oColumn)
        {
            if (! _ForiegnKeys.Contains(oColumn.Name))
                _ForiegnKeys.Add(new ForiegnKey(){RefrencedColumnName = RefrencedColumnName,RefrencedTableName = RefrencedTable,ForeginKeyColumn = oColumn});            
        }

        public Column IdentityColumn
        {
            get
            {
                return _IdentityColumn;
            }
            set
            {
                _IdentityColumn = value;
            }
        }

    }
}
