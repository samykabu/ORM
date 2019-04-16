using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseObjects.DB;

namespace DatabaseObjects
{    
    public abstract class ICLRTemplate
    {
        public abstract string Description { get; }
        public abstract bool CanProcess(DatabaseObject obj);
        public abstract string Process(DatabaseObject obj);
    }
}
