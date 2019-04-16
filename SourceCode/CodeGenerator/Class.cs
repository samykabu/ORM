using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace CodeGenerator
{
    public class Class
    {        
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Prefix=string.Empty;

        public string Prefix
        {
            get { return _Prefix; }
            set { _Prefix = value; }
        }

        private string _NameSpace=string.Empty;

        public string NameSpace
        {
            get { return _NameSpace; }
            set { _NameSpace = value; }
        }

        private string _Inherits;

        public string Inherits
        {
            get { return _Inherits; }
            set { _Inherits = value; }
        }

        private List<string> _Implements=new List<string>();

        public List<string> Implements
        {
            get { return _Implements; }
            set { _Implements = value; }
        }

        private List<string> _Usings=new List<string>();

        public List<string> UsingLibrary
        {
            get { return _Usings; }
            set { _Usings= value; }
        }

        private List<string> _Attributes = new List<string>();

        public List<string> Attributes
        {
            get { return _Attributes; }
            set { _Attributes = value; }
        }
        private Properties _Properties = new Properties();

        public Properties Properties
        {
            get { return _Properties; }
            set { _Properties = value; }
        }

        private Methods _Methods=new Methods() ;
            
        public Methods Methods
        {
            get { return _Methods ; }
            set { _Methods  = value; }
        }

        private List<string> _PrivateVariables = new List<string>();

        public List<string> DeclareVariable
        {       
            get { return _PrivateVariables; }
            set { _PrivateVariables = value; }
        }

        public override string ToString()
        {
            //Construct the class by assymbling all parts togather.
            StringBuilder Body = new StringBuilder();
            foreach (string str in this.UsingLibrary)
            {
                Body.Append(str);
            }
            Body.Append("\r\r");
            if (NameSpace.Length > 0)
            {
                Body.AppendFormat("namespace {0} \r\t", NameSpace);
                Body.Append("{\r");
            }

            foreach (string atr in Attributes)
            {
                Body.Append(atr);
            }

            Body.AppendFormat(" class {0}{1}:{2}DataBaseObject,IDisposable\r", Prefix, Name, NameSpace.Length > 0 ? NameSpace + @"." : "");
            Body.Append("\t{\r");

            Body.Append("\t\t#region Private Variables\r");
            foreach (string pval in DeclareVariable)
            {
                Body.Append(pval);
            }
            Body.Append("\t\t#endregion\r\r");

            Body.Append("\t\t#region Public properties\r");
            foreach (Property prop in this.Properties)
            {
                Body.Append(prop.ToString());
            }
            Body.Append("\t\t#endregion\r\r");

            Body.Append("\t\t#region methods\r");
            foreach (Method oMethod in this.Methods)
            {
                Body.Append(oMethod.ToString());
            }
            Body.Append("\t\t#endregion\r\r");

            Body.Append("\t}\r");

            if (NameSpace.Length > 0)
            {
             
                Body.Append("}\r");
            }

            return Body.ToString();
        }
	
    }
}
