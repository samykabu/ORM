using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseObjects.DB
{
    public class Column : DatabaseObject
    {
       
        private string _ClrType;
        private bool _ISNUllable;
        private string _DeafultValue;
        private bool _isReadonly;
        private bool _isIdentity;
        private bool _isPrimaryKey;
        private bool _HasConsantrain;
        private bool _HasLenghtRestriction;

   

    }
}
