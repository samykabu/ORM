using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sql2005Server
{
    public class CollectionGenerator
    {
    }

    public class Employess : List<Employee>
    {
        public Employess()
        {
        }
        public Employess(DataSet ds)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Employee emp = new Employee();
                emp.MyProperty =(int)row["ColumnName"];
                this.Add(emp);
            }
        }        
    }
    public class Employee
    {
        public int MyProperty { get; set; }
    }
}
