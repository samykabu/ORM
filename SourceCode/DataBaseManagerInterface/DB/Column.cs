using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaObjects
{
    public class Column
    {
        #region Private variables

        private string _Name;

        private string _Description;

        private string _SqlType;
        private string _ClrType;
        private bool _ISNUllable;

        private int _Length;

        private bool _Readonly;

        private string _DeafultValue;
        
        #endregion

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public bool IsForignKey { get; set; }
        public bool IsComputed { get; set; }
        public string ComputedSQLText { get; set; }
        public bool InPrimaryKey { get; set; }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }        

        public string SqlType
        {
            get { return _SqlType; }
            set { _SqlType = value; }
        }

        public string CLRType
        {
            get { return _ClrType; }
            set { _ClrType = value; }
        }

        public bool IsNullable
        {
            get { return _ISNUllable; }
            set { _ISNUllable = value; }
        }

        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        public bool Readonly
        {
            get { return _Readonly; }
            set { _Readonly = value; }
        }

        public string DefaultValue
        {
            get { return _DeafultValue; }
            set { _DeafultValue = value; }
        }

        private bool _SelectBy = false;
        public bool SelectBy
        {
            get { return _SelectBy; }
            set { _SelectBy = value; }
        }

        private bool _DeleteBy = false;
        public bool DeleteBy
        {
            get { return _DeleteBy; }
            set { _DeleteBy = value; }
        }       	
        
    }
}
