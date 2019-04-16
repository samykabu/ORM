using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseObjects.DB;
using DatabaseObjects.DB.Collections;

namespace DatabaseObjects
{
    abstract public class ISchemaGenerator
    {
        public abstract string GenerateDatabaseScript(Database db);
        protected abstract string GenerateTablesScript(Tables tables);
        protected abstract string GenerateViewsScript(Views views);
        protected abstract string GenerateStoredProceduresScript(StoredProcedures sps);
    }
}
