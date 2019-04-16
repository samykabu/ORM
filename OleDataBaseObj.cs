using System;
using System.Data;
using System.Data.OleDb;

namespace DataLayer
{
   
    abstract class OleDataBaseObject : IDisposable
    {

        #region IDisposable Members

        public void Dispose()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        protected OleDbConnection oCon;
        private TransactionMode _TransactionType = TransactionMode.None;

        public TransactionMode TransactionType
        {
            get { return _TransactionType; }
            set { _TransactionType = value; }
        }
        protected OleDbTransaction oCurTransaction = null;
        protected bool IsLocalConnection = false;
        public string ConnectionString = "Server=localhost;database=NoryanRss;UID=sa;PWD=p7z6y1f9";



        #region Connection Management

        protected void Open()
        {
            if (oCon == null)
            {
                oCon = new OleDbConnection(ConnectionString);
                oCon.Open();
                if (TransactionType == TransactionMode.RequireTransaction)
                    oCurTransaction = oCon.BeginTransaction();
                IsLocalConnection = true;
            }
            else if (TransactionType == TransactionMode.RequireTransaction && oCurTransaction == null && IsLocalConnection == true)
            {
                oCurTransaction = oCon.BeginTransaction();
            }
            else
            {
                if (oCon.State == ConnectionState.Closed | oCon.State == ConnectionState.Broken)
                    throw new Exception("DataBase Connection Broken");
                IsLocalConnection = false;
            }
        }

        protected void Close()
        {
            if (IsLocalConnection && TransactionType == TransactionMode.None)
                if (oCon != null && oCon.State != ConnectionState.Broken && oCon.State != ConnectionState.Closed)
                {
                    oCon.Close();
                    oCon.Dispose();
                    oCon = null;
                }
        }

        #endregion

        #region Parameters Creation

        protected OleDbParameter MakeParameter(string ParameterName, object Value)
        {
            OleDbParameter Param = new OleDbParameter(ParameterName, Value);
            Param.Direction = ParameterDirection.Input;
            return Param;
        }

        protected OleDbParameter MakeParameter(string ParameterName, object Value, ParameterDirection Direction)
        {
            OleDbParameter Param = new OleDbParameter(ParameterName, Value);
            Param.Direction = Direction;
            return Param;
        }

        #endregion

        #region Sql Command Creation

        protected OleDbCommand CreateCommand(string StoredProcedureName, OleDbParameter[] Parameters)
        {
            OleDbCommand cmd;

            Open();
            cmd = new OleDbCommand(StoredProcedureName, oCon);
            cmd.CommandType = CommandType.StoredProcedure;
            if (Parameters != null)
                cmd.Parameters.AddRange(Parameters);

            if (oCurTransaction != null)
                cmd.Transaction = oCurTransaction;

            return cmd;
        }

        protected OleDbCommand CreateCommand(string CmdString)
        {
            OleDbCommand cmd;
            Open();
            cmd = new OleDbCommand(CmdString, oCon);

            if (oCurTransaction != null)
                cmd.Transaction = oCurTransaction;
            return cmd;
        }

        #endregion

        #region Command Excution

        protected Int64 RunProcedure(string StoredProcedureName)
        {
            OleDbCommand cmd = null;
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

        protected object RunProcedure(string StoredProcedureName, OleDbParameter[] OleDbParameters)
        {
            OleDbCommand cmd = null;
            object result;
            try
            {
                cmd = CreateCommand(StoredProcedureName, OleDbParameters);
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

        protected void RunProcedure(string StoredProcedureName, OleDbDataReader DataReader)
        {            
            OleDbCommand cmd = null;
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

        protected void RunProcedure(string StoredProcedureName, OleDbParameter[] Parameters, OleDbDataReader Datareader)
        {
            OleDbCommand cmd = null;
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

        protected void RunProcedure(string StoredProcedureName, OleDbParameter[] Parameters, DataSet SqlDataSet)
        {
            OleDbCommand cmd = null;            
            OleDbDataAdapter sqladb = null;
            try
            {
                cmd = CreateCommand(StoredProcedureName, Parameters);
                sqladb = new OleDbDataAdapter(cmd);
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
            OleDbCommand cmd = null;
            OleDbDataAdapter sqladb = null;
            try
            {
                cmd = CreateCommand(StoredProcedureName, null);
                sqladb = new OleDbDataAdapter(cmd);
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

        public void Enlist(OleDataBaseObject Obj)
        {
            oCon = Obj.oCon;
            oCurTransaction = Obj.oCurTransaction;
        }

        public void Commit()
        {
            if (oCurTransaction != null)
            {
                oCurTransaction.Commit();
                oCurTransaction.Dispose();
                oCurTransaction = null;
                oCon.Close();
                oCon.Dispose();
            }
            else
            {
                this.Close();
                throw new Exception("No Transaction to Commit");
            }
        }

        public void RollBack()
        {
            if (oCurTransaction != null)
            {
                oCurTransaction.Rollback();
                oCurTransaction.Dispose();
                oCurTransaction = null;
                oCon.Close();
                oCon.Dispose();
            }
            else
            {
                this.Close();
                throw new Exception("No Transaction to rollback");
            }
        }

        #endregion


        #region Public Methods
        public void RunOleDbCommand(string SqlCmd, ref OleDbDataReader DataReader)
        {
            OleDbCommand cmd = null;
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
        public void RunOleDbCommand(string SqlCmd, OleDbParameter[] Params, ref OleDbDataReader DataReader)
        {
            OleDbCommand cmd = null;
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
        public void RunOleDbCommand(string SqlCmd, DataSet SqlDataSet)
        {
            OleDbCommand cmd = null;
            OleDbDataAdapter Sqladp;

            try
            {
                cmd = CreateCommand(SqlCmd);
                Sqladp = new OleDbDataAdapter(SqlCmd, oCon);
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
        public void RunOleDbCommand(string SqlCmd, OleDbParameter[] Params, DataSet SqlDataSet)
        {
            OleDbCommand cmd = null;
            OleDbDataAdapter Sqladp;

            try
            {
                cmd = CreateCommand(SqlCmd, Params);
                Sqladp = new OleDbDataAdapter(SqlCmd, oCon);
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
        public DataSet LoadInDataSet()
        {
            throw new NotImplementedException();
        }
        public DataSet LoadInDataSet(string FilterBy)
        {
            throw new NotImplementedException();
        }
        public OleDbDataReader LoadInDataReader()
        {
            throw new NotImplementedException();
        }
        public OleDbDataReader LoadInDataReader(string FilterBy)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
