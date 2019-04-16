using System;
using System.Collections.Generic;
using System.Text;
using SchemaObjects;

namespace CodeGenerator
{
    public abstract class ITemplate
    {
        public delegate void OnProcessingDatabaseObject(string DbObjectName);
        public delegate void OnCompleteProcessing();

        public event OnProcessingDatabaseObject ProcessingDatabaseObject;
        public event OnCompleteProcessing ProcessingCompleted;

        public string TemplateName = string.Empty;
        public string OutputDir=string.Empty;
        
        public abstract void Process(IDataBaseObject DbObject,string NameSpace,string Prefix,bool bSerialize);
        public abstract void SaveToFile();
        protected abstract bool SupportedDatabaseObject(IDataBaseObject DbObject);
        public abstract override string ToString();

    }
}
