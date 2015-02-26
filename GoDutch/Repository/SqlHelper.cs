using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using log4net;

namespace GoDutch.Repository
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

        public T ExecuteScalar<T>(string sql, params SqlParameter[] parameters)
        {
            return (T)ExecuteScalar(sql, parameters);
        }

        public Object ExecuteScalar(string sql, params SqlParameter[] parameters)
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
                return cmd.ExecuteScalar();
            }
        }

        //public SqlDataReader ExecuteReader(string sql, IEnumerable<SqlParameter> parameters)
        //{
        //    return ExecuteReader(sql, parameters.ToArray());
        //}

        public int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandTimeout = 240;
                cmd.CommandText = sql;
                foreach (var p in parameters)
                {
                    if (p.Value == null) p.Value = DBNull.Value;
                    cmd.Parameters.Add(p);
                }

                // todo: log4net still not working... at least for me
//                if (log.IsDebugEnabled) log.Debug(cmd.ToStringWithParameterValues());

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

    }
}