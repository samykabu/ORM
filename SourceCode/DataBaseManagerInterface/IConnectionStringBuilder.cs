using System;
using System.Collections.Generic;
using System.Text;

namespace SchemaObjects
{
    public interface IConnectionStringBuilder
    {
         string ConnectionString();
         void ShowForm();
    }
}
