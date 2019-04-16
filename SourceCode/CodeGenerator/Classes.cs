using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator
{
    public class Classes: List<Class>
        {
        public Class Item(string ClassName)
            {
                foreach (Class oClass in this)
                {
                    if (oClass.Name == ClassName)
                        return oClass;
                }
                return null;
            }
        }    
}
