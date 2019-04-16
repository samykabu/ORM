using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseObjects.DB
{
    public class PrimaryKey:DatabaseObject
    {

        private Dictionary<string, Column> _Columns = new Dictionary<string, Column>();
        
      
    }
}
