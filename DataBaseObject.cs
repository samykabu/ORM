using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace FamilyPortal
{
    [Flags]
    public enum TransactionMode
    {
        None = 0,
        RequireTransaction = 1
    }
    public struct ValidateRecord
    {
        public string PropertyName;
        public string Message;
        public string ValidationError;
    }

    public class ValidationException : Exception
    {
        public List<ValidateRecord> RequiredProperties { get; set; }
        public ValidationException(string msg, List<ValidateRecord> RequiredProp)
            : base(msg)
        {
            RequiredProperties = RequiredProp;
        }
    }
    public class TransactionException : Exception
    {
        public TransactionException(string msg) : base(msg) { }
    }
    public class ConnectionException : Exception
    {
        public ConnectionException(string msg) : base(msg) { }
    }
    internal class Transaction
    {
        public string TransactionName;
        public SqlTransaction sqltransaction = null;
        public DataBaseObject owner;
        public Transaction(DataBaseObject Owner)
        {
            TransactionName = System.Threading.Thread.CurrentContext.ContextID.ToString();
            owner = Owner;
        }
        public Transaction(SqlTransaction trans, DataBaseObject Owner)
        {
            TransactionName = Guid.NewGuid().ToString();
            sqltransaction = trans;
            owner = Owner;
        }
    }
    internal class TransactionManager
    {
        private static TransactionManager _sTransactionManager;
        private Dictionary<int,Transaction> Trans = new Dictionary<int,Transaction>();
        private TransactionManager()
        { }
        public static TransactionManager ManagerInstance
        {
            get
            {
                if (_sTransactionManager == null)
                    _sTransactionManager = new TransactionManager();
                return _sTransactionManager;
            }
        }
        public bool TransactionAvailable { get { return (Trans.ContainsKey((Thread.CurrentThread.ExecutionContext.GetHashCode()))); } }
        public void StartNewTransaction(DataBaseObject owner)
        {            
            Trans.Add(Thread.CurrentThread.ExecutionContext.GetHashCode(), new Transaction(owner));
        }
        public DataBaseObject ActiveTransactionOwner
        {
            get { return Trans[Thread.CurrentThread.ExecutionContext.GetHashCode()].owner; }
        }
        public Transaction ActiveTransaction
        {
            get { return Trans[Thread.CurrentThread.ExecutionContext.GetHashCode()]; }
        }

        public void CommitActive()
        {
               if (TransactionAvailable)
                {
                    Trans[Thread.CurrentThread.ExecutionContext.GetHashCode()].sqltransaction.Commit();
                    Trans.Remove(Thread.CurrentThread.ExecutionContext.GetHashCode());
                }
                else
                   throw new TransactionException("No transaction in Scope");
            
        }
        public void Roleback()
        {

            if (TransactionAvailable)
            {
                Trans[Thread.CurrentThread.ExecutionContext.GetHashCode()].sqltransaction.Rollback();
                Trans.Remove(Thread.CurrentThread.ExecutionContext.GetHashCode());
            }
            else
                throw new TransactionException("No transaction in Scope");
        }
    }

    public class DataBaseObject : IDisposable
    {
        public string ConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString"); // "Server={1};database={2};UID={3};PWD={4}";

        internal string TableName;
        private bool isDisposing = false;

        #region abstract Methods
        /// <summary>
        /// this is called to validate the required columns (Not nullable) before executeing the database command , the validation would throw an exception of type ValidationException if it fails
        /// </summary>
        protected virtual void OnValidate() { throw new NotImplementedException("OnValidate is not Implemented yet"); }

        /// <summary>
        /// it is called before the Insert event take place , this is the right place to have your business validation logic
        /// </summary>
        /// <param name="Cancel">set this to false in order to cancel the insert command</param>
        protected virtual void OnBeforeInsert(ref bool Cancel) { Cancel = false; }
        /// <summary>
        /// it is called before the Update event take place , this is the right place to have your business validation logic
        /// </summary>
        /// <param name="Cancel">set this to false in order to cancel the Update command</param>
        protected virtual void OnBeforeUpdate(ref bool Cancel) { Cancel = false; }
        /// <summary>
        /// it is called before the Delete event take place , this is the right place to have your business validation logic or other none DB clean up code.
        /// </summary>
        /// <param name="Cancel">set this to false in order to cancel the Delete command</param>
        protected virtual void OnBeforeDelete(ref bool Cancel) { Cancel = false; }

        /// <summary>
        /// this is where the actual code for the record insertion goes , do not override / change this unless you know what your doing
        /// </summary>        
        protected virtual bool OnInsert() { throw new NotImplementedException("OnInsert is not Implemented yet"); }
        /// <summary>
        /// this is where the actual code for the record update goes , do not override / change this unless you know what your doing
        /// </summary>        
        protected virtual bool OnUpdate() { throw new NotImplementedException("OnUpdate is not Implemented yet"); }
        /// <summary>
        /// this is where the actual code for the record deletion goes , do not override / change this unless you know what your doing
        /// </summary>        
        protected virtual bool OnDelete() { throw new NotImplementedException("OnDelete is not Implemented yet"); }

        /// <summary>
        /// this procedure is called if the insert has taken place and it is successful
        /// </summary>
        protected virtual void OnAfterInsert() { }
        /// <summary>
        /// this procedure is called if the Update has taken place and it is successful
        /// </summary>
        protected virtual void OnAfterUpdate() { }
        /// <summary>
        /// this procedure is called if the Delete has taken place and it is successful
        /// </summary>
        protected virtual void OnAfterDelete() { }

        public bool Insert()
        {       
            bool res = false;
            bool isCanceled = false;
            OnValidate();
            OnBeforeInsert(ref isCanceled);
            if (!isCanceled)
            {
                res = OnInsert();
                if (res)
                    OnAfterInsert();
            }
            return res;
        }
        public bool Update()
        {       
            bool res = false;
            bool isCanceled = false;
            OnValidate();
            OnBeforeUpdate(ref isCanceled);
            if (!isCanceled)
                res = OnUpdate();
            if (res)
                OnAfterUpdate();
            return res;
        }
        public bool Delete()
        {                      
            bool res = false;
            bool isCanceled = false;
            OnBeforeDelete(ref isCanceled);
            if (!isCanceled)
                res = OnDelete();
            if (res)
                OnAfterDelete();
            return res;
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (!isDisposing)
            {
                isDisposing = true;
                this.Close();
            }

        }



        #endregion

        protected SqlConnection oCon;
        private TransactionMode _TransactionType = TransactionMode.None;

        public TransactionMode TransactionType
        {
            get { return _TransactionType; }
            set { _TransactionType = value; }
        }
        //protected SqlTransaction oCurTransaction = null;
        protected bool IsLocalConnection = false;


        #region Connection Management

        protected void Open()
        {
            if (TransactionManager.ManagerInstance.TransactionAvailable)
                oCon = TransactionManager.ManagerInstance.ActiveTransactionOwner.oCon;
            if (oCon == null)
            {
                oCon = new SqlConnection(ConnectionString);
                oCon.Open();
                if (TransactionManager.ManagerInstance.TransactionAvailable && this == TransactionManager.ManagerInstance.ActiveTransactionOwner)
                {
                    //oCurTransaction = oCon.BeginTransaction();
                    TransactionManager.ManagerInstance.ActiveTransaction.sqltransaction = oCon.BeginTransaction();
                }
                IsLocalConnection = true;
            }
            else if (TransactionManager.ManagerInstance.TransactionAvailable && this != TransactionManager.ManagerInstance.ActiveTransactionOwner && IsLocalConnection == true)
            {
                TransactionManager.ManagerInstance.ActiveTransaction.sqltransaction = oCon.BeginTransaction();
                //oCurTransaction = oCon.BeginTransaction();
            }
            else
            {
                if (oCon.State == ConnectionState.Closed | oCon.State == ConnectionState.Broken)
                    throw new ConnectionException("DataBase Connection Broken");
                IsLocalConnection = false;
            }
        }

        protected void Close()
        {
            if (IsLocalConnection && !TransactionManager.ManagerInstance.TransactionAvailable)
                if (oCon != null && oCon.State != ConnectionState.Broken && oCon.State != ConnectionState.Closed)
                {
                    oCon.Close();
                    oCon.Dispose();
                    oCon = null;
                }
           // GC.Collect();
        }

        #endregion

        #region Parameters Creation

        protected SqlParameter MakeParameter(string ParameterName, object Value)
        {
            SqlParameter Param = new SqlParameter(ParameterName, Value);
            Param.Direction = ParameterDirection.Input;
            return Param;
        }
        protected SqlParameter MakeImageParameter(string ParameterName, object Value)
        {
            SqlParameter Param = new SqlParameter(ParameterName, Value);
            Param.DbType = DbType.Binary;
            Param.Direction = ParameterDirection.Input;
            return Param;
        }
        protected SqlParameter MakeParameter(string ParameterName, object Value, ParameterDirection Direction)
        {
            SqlParameter Param = new SqlParameter(ParameterName, Value);
            Param.Direction = Direction;
            return Param;
        }

        protected SqlParameter MakeImageParameter(string ParameterName, object Value, ParameterDirection Direction)
        {
            SqlParameter Param = new SqlParameter(ParameterName, Value);
            Param.DbType = DbType.Binary;
            Param.Direction = Direction;
            return Param;
        }
        #endregion

        #region Sql Command Creation

        protected SqlCommand CreateCommand(string StoredProcedureName, SqlParameter[] Parameters)
        {
            SqlCommand cmd;

            Open();
            cmd = new SqlCommand(StoredProcedureName, oCon);
            cmd.CommandType = CommandType.StoredProcedure;
            if (Parameters != null)
                cmd.Parameters.AddRange(Parameters);

            //if (oCurTransaction != null)
            //    cmd.Transaction = oCurTransaction;
            if (TransactionManager.ManagerInstance.TransactionAvailable)
                cmd.Transaction = TransactionManager.ManagerInstance.ActiveTransaction.sqltransaction;

            return cmd;
        }

        protected SqlCommand CreateCommand(string CmdString)
        {
            SqlCommand cmd;
            Open();
            cmd = new SqlCommand(CmdString, oCon);

            //if (oCurTransaction != null)
            //    cmd.Transaction = oCurTransaction;
            if (TransactionManager.ManagerInstance.TransactionAvailable)
                cmd.Transaction = TransactionManager.ManagerInstance.ActiveTransaction.sqltransaction;
            return cmd;
        }

        #endregion

        #region Command Excution

        protected Int64 RunProcedure(string StoredProcedureName)
        {
            SqlCommand cmd = null;
            Int64 result;
            try
            {
                cmd = CreateCommand(StoredProcedureName, null);
                result = (Int64)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                if (cmd != null)
                    cmd.Dispose();
                throw ex;
            }
            cmd.Dispose();
            cmd = null;
            return result;
        }

        protected object RunProcedure(string StoredProcedureName, SqlParameter[] SqlParameters)
        {
            SqlCommand cmd = null;
            object result;
            try
            {
                cmd = CreateCommand(StoredProcedureName, SqlParameters);
                result = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                if (cmd != null)
                    cmd.Dispose();
                throw ex;
            }
            cmd.Dispose();
            cmd = null;
            return result;
        }

        protected void RunProcedure(string StoredProcedureName, ref SqlDataReader DataReader)
        {
            SqlCommand cmd = null;
            try
            {
                cmd = CreateCommand(StoredProcedureName, null);
                DataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                DataReader = null;
                if (cmd != null)
                    cmd.Dispose();
                throw ex;
            }
            cmd.Dispose();
            cmd = null;
        }

        protected void RunProcedure(string StoredProcedureName, SqlParameter[] Parameters, ref SqlDataReader Datareader)
        {
            SqlCommand cmd = null;
            try
            {
                cmd = CreateCommand(StoredProcedureName, Parameters);
                Datareader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Datareader = null;
                if (cmd != null)
                    cmd.Dispose();
                throw ex;
            }
            cmd.Dispose();
            cmd = null;
        }

        protected void RunProcedure(string StoredProcedureName, SqlParameter[] Parameters, DataSet SqlDataSet)
        {
            SqlCommand cmd = null;
            SqlDataAdapter sqladb = null;
            try
            {
                cmd = CreateCommand(StoredProcedureName, Parameters);
                sqladb = new SqlDataAdapter(cmd);
                sqladb.Fill(SqlDataSet);
            }
            catch (Exception ex)
            {
                if (sqladb != null)
                    sqladb.Dispose();
                if (cmd != null)
                    cmd.Dispose();
                throw ex;
            }
            cmd.Dispose();
            sqladb.Dispose();
            sqladb = null;
            cmd = null;
        }

        protected void RunProcedure(string StoredProcedureName, DataSet SqlDataSet)
        {
            SqlCommand cmd = null;
            SqlDataAdapter sqladb = null;
            try
            {
                cmd = CreateCommand(StoredProcedureName, null);
                sqladb = new SqlDataAdapter(cmd);
                sqladb.Fill(SqlDataSet);
            }
            catch (Exception ex)
            {
                if (sqladb != null)
                    sqladb.Dispose();
                if (cmd != null)
                    cmd.Dispose();
                throw ex;
            }
            cmd.Dispose();
            sqladb.Dispose();
            sqladb = null;
            cmd = null;
        }

        #endregion

        #region Transaction Functions

        public void StartTransaction()
        {         
            TransactionType = TransactionMode.RequireTransaction;
            TransactionManager.ManagerInstance.StartNewTransaction(this);
        }

        //public void EnlistInTransaction(DataBaseObject Obj)
        //{
        //    oCon = Obj.oCon;
        //    //oCurTransaction = Obj.oCurTransaction;
        //}

        public void Commit()
        {
            if (TransactionManager.ManagerInstance.TransactionAvailable)
            {
                TransactionManager.ManagerInstance.CommitActive();
                //oCurTransaction.Commit();
                //oCurTransaction.Dispose();
                //oCurTransaction = null;
                oCon.Close();
                oCon.Dispose();
            }
            else
            {
                this.Close();
                throw new TransactionException("No Transaction to Commit");
            }
        }

        public void RollBack()
        {
            if (TransactionManager.ManagerInstance.TransactionAvailable)
            {
                TransactionManager.ManagerInstance.Roleback();
                //oCurTransaction.Rollback();
                //oCurTransaction.Dispose();
                //oCurTransaction = null;
                oCon.Close();
                oCon.Dispose();
            }
            else
            {
                this.Close();
                throw new TransactionException("No Transaction to rollback");
            }
        }

        #endregion


        #region Public Methods
        public void RunSqlCommand(string SqlCmd, ref SqlDataReader DataReader)
        {
            SqlCommand cmd = null;
            try
            {
                cmd = CreateCommand(SqlCmd);
                DataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                DataReader = null;
                if (cmd != null)
                    cmd.Dispose();
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                cmd = null;
            }

        }
        public void RunSqlCommand(string SqlCmd, SqlParameter[] Params, ref SqlDataReader DataReader)
        {
            SqlCommand cmd = null;
            try
            {
                cmd = CreateCommand(SqlCmd, Params);
                DataReader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                DataReader = null;
                if (cmd != null)
                    cmd.Dispose();
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                cmd = null;
            }

        }
        public void RunSqlCommand(string SqlCmd, DataSet SqlDataSet)
        {
            SqlCommand cmd = null;
            SqlDataAdapter Sqladp;

            try
            {
                cmd = CreateCommand(SqlCmd);
                Sqladp = new SqlDataAdapter(cmd);
                Sqladp.Fill(SqlDataSet);
            }
            catch (Exception ex)
            {
                SqlDataSet = null;
                if (cmd != null)
                    cmd.Dispose();
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                cmd = null;
                Sqladp = null;
            }
        }


        public void RunSqlCommand(string SqlCmd, SqlParameter[] Params, DataSet SqlDataSet)
        {
            SqlCommand cmd = null;
            SqlDataAdapter Sqladp;

            try
            {
                cmd = CreateCommand(SqlCmd, Params);
                Sqladp = new SqlDataAdapter(cmd);
                Sqladp.Fill(SqlDataSet);
            }
            catch (Exception ex)
            {
                SqlDataSet = null;
                if (cmd != null)
                    cmd.Dispose();
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                cmd = null;
                Sqladp = null;
            }
        }

        public void RunSqlCommand(string SqlCmd, DataSet SqlDataSet, string TableName)
        {
            SqlCommand cmd = null;
            SqlDataAdapter Sqladp;

            try
            {
                cmd = CreateCommand(SqlCmd);
                Sqladp = new SqlDataAdapter(cmd);
                Sqladp.Fill(SqlDataSet, TableName);
            }
            catch (Exception ex)
            {
                SqlDataSet = null;
                if (cmd != null)
                    cmd.Dispose();
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                cmd = null;
                Sqladp = null;
            }
        }

        public static void RunStaticSqlCommand(string SqlCmd, DataSet SqlDataSet, string TableName)
        {
            SqlCommand cmd = null;
            SqlDataAdapter Sqladp;
            DataBaseObject dbobj = new DataBaseObject();
            dbobj.TableName = TableName;
            try
            {
                cmd = dbobj.CreateCommand(SqlCmd);
                Sqladp = new SqlDataAdapter(cmd);
                Sqladp.Fill(SqlDataSet, TableName.Replace("[", "").Replace("]", ""));
            }
            catch (Exception ex)
            {
                SqlDataSet = null;
                if (cmd != null)
                    cmd.Dispose();
                dbobj.Dispose();
                Sqladp = null;
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                dbobj.Dispose();
                cmd = null;
                Sqladp = null;
            }
        }

        public DataSet ListAll()
        {
            DataSet ds = new DataSet();
            RunSqlCommand("Select * from " + TableName, ds, TableName);
            return ds;
        }
        public DataSet ListTop(int count)
        {
            DataSet ds = new DataSet();
            RunSqlCommand("Select top(" + count.ToString() + ") * from " + TableName, ds, TableName);
            return ds;
        }

        public DataSet ListWhere(string FilterBy)
        {
            DataSet ds = new DataSet();
            RunSqlCommand("Select * from " + TableName + " where " + FilterBy, ds, TableName);
            return ds;
        }

        public DataSet ListTopWhere(string FilterBy, int count)
        {
            DataSet ds = new DataSet();
            RunSqlCommand("Select  top(" + count.ToString() + ") * from " + TableName + " where " + FilterBy, ds, TableName);
            return ds;
        }
        public SqlDataReader ListAllDataReader()
        {
            SqlDataReader Res = null;
            RunSqlCommand("Select * from " + TableName, ref Res);
            return Res;
        }
        public SqlDataReader ListTopDataReader(int count)
        {
            SqlDataReader Res = null;
            RunSqlCommand("Select  top(" + count.ToString() + ")  * from " + TableName, ref Res);
            return Res;
        }
        public SqlDataReader ListWhereDataReader(string FilterBy)
        {
            SqlDataReader Res = null;
            RunSqlCommand("Select * from " + TableName + " where " + FilterBy, ref Res);
            return Res;
        }
        public SqlDataReader ListTopWhereDataReader(string FilterBy, int count)
        {
            SqlDataReader Res = null;
            RunSqlCommand("Select  top(" + count.ToString() + ") * from " + TableName + " where " + FilterBy, ref Res);
            return Res;
        }

        #endregion

    }
}
