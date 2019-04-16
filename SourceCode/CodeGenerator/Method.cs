using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator
{
    public class Methods : List<Method>
    {        
        public Method Item(string MethodName)
        {
            foreach (Method oMethod in this)
            {
                if (oMethod.Name == MethodName)
                    return oMethod;
            }
            return null;
        }
    }

    public class Method
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private StringBuilder _Body=new StringBuilder();
        
        public StringBuilder Body
        {
            get { return _Body; }
            set { _Body = value; }
        }

        public override string ToString()
        {
            return this._Body.ToString();
        }
    }

    public class Properties : List<Property>
    {
        public Property Item(string PropertyName)
        {
            foreach (Property oProperty in this)
            {
                if (oProperty.Name == PropertyName)
                    return oProperty;
            }
            return null;
        }
    }
    public class Property
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private StringBuilder _Body = new StringBuilder();

        public StringBuilder Body
        {
            get { return _Body; }
            set { _Body = value; }
        }

        public override string ToString()
        {
            return this._Body.ToString();
        }
    }
}
