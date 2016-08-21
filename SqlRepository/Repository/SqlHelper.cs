using System;
using System.Data;
using System.Data.SqlClient;

namespace SqlRepository.Repository
{
    public class SqlHelper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string connectionString;

        public SqlHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public SqlDataReader ExecuteReader(string sql, params SqlParameter[] parameters)
        {
            var conn = new SqlConnection(connectionString);
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandTimeout = 240;
                cmd.CommandText = sql;
                foreach (var p in parameters)
                {
                    if (p.Value == null) p.Value = DBNull.Value;
                    cmd.Parameters.Add(p);
                }
                conn.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public T ExecuteScalar<T>(string sql, SqlConnection conn, SqlTransaction tran, params SqlParameter[] parameters)
        {
            if (conn == null)
            {
                using (conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return (T) ExecuteScalarInner(sql, conn, null, parameters);
                }
            }
            else
            {
                return (T)ExecuteScalarInner(sql, conn, tran, parameters);    
            }
        }

        private Object ExecuteScalarInner(string sql, SqlConnection conn, SqlTransaction tran, params SqlParameter[] parameters)
        {
            using (var cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.CommandTimeout = 240;
//                cmd.CommandText = sql;
                foreach (var p in parameters)
                {
                    if (p.Value == null) p.Value = DBNull.Value;
                    cmd.Parameters.Add(p);
                }
                return cmd.ExecuteScalar();
            }
        }

        //public SqlDataReader ExecuteReader(string sql, IEnumerable<SqlParameter> parameters)
        //{
        //    return ExecuteReader(sql, parameters.ToArray());
        //}

        public int ExecuteNonQuery(string sql, SqlConnection conn, SqlTransaction tran, params SqlParameter[] parameters)
        {
            if (conn == null)
            {
                using (conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return ExecuteNonQueryInner(sql, conn, null, parameters);
                }
            }
            else
            {
                return ExecuteNonQueryInner(sql, conn, tran, parameters);
            }   
        }

        private int ExecuteNonQueryInner(string sql, SqlConnection conn, SqlTransaction tran, params SqlParameter[] parameters)
        {
            using (var cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.CommandTimeout = 240;
//                cmd.CommandText = sql;
                foreach (var p in parameters)
                {
                    if (p.Value == null) p.Value = DBNull.Value;
                    cmd.Parameters.Add(p);
                }

                // todo: log4net still not working... at least for me
//                if (log.IsDebugEnabled) log.Debug(cmd.ToStringWithParameterValues());

                return cmd.ExecuteNonQuery();
            }
        }

    }
}