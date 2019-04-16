using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaObjects
{
    public class Tables:List<Table>
    {
        public Table Item(string TableName)
        {
            foreach (Table tb in this)
            {
                if (tb.Name == TableName)
                    return tb;
            }
            return null;
        }
    }

    public class Views : List<View>
    {
        public View Item(string ViewName)
        {
            foreach (View oView in this)
            {
                if (oView.Name == ViewName)
                    return oView;
            }
            return null;
        }
    }

    public class StoredProcedures : List<StoredProcedure>
    {
        public StoredProcedure Item(string StoredProcedureName)
        {
            foreach (StoredProcedure oStoredProcedure in this)
            {
                if (oStoredProcedure.Name == StoredProcedureName)
                    return oStoredProcedure;
            }
            return null;
        }
    }

    public class StoredProcedureParameters : List<Parameter>
    {
        public Parameter Item(string ParamName)
        {
            foreach (Parameter oParam in this)
            {
                if (oParam.Name == ParamName)
                    return oParam;
            }
            return null;
        }
    }
}
