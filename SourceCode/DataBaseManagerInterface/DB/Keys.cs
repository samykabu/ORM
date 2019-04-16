using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaObjects
{
    public class Key
    {
        private string _KeyName;

        public string KeyName
        {
            get { return _KeyName; }
            set { _KeyName = value; }
        }
        private Dictionary<string,Column> _Columns=new Dictionary<string,Column>();

        public Dictionary<string,Column> Columns
        {
            get { return _Columns; }        
            set { _Columns = value; }
        }
	
	
    }
    public class Keys : List<Key>
    {
        public bool Contains(string KeyName)
        {
            if (this.Item(KeyName) != null)
                return true;
            return false;
        }
        public Key Item(string KeyName)
        {
            foreach (Key oKey in this)
            {
                if (oKey.KeyName == KeyName)
                    return oKey;
            }
            return null;
        }
    }

    public class ForiegnKey
    {
        public string RefrencedTableName { get; set; }
        public string RefrencedColumnName { get; set; }
        public Column ForeginKeyColumn { get; set; }

    }

    public class ForiegnKeys : List<ForiegnKey>
    {
        public bool Contains(string KeyName)
        {
            if (this.Item(KeyName) != null)
                return true;
            return false;
        }
        public ForiegnKey Item(string KeyName)
        {
            foreach (ForiegnKey oKey in this)
            {
                if (oKey.ForeginKeyColumn.Name == KeyName)
                    return oKey;
            }
            return null;
        }
    }
}
