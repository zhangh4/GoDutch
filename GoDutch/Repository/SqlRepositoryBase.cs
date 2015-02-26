using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoDutch.Repository
{
    public abstract class SqlRepositoryBase
    {
        private string _connectionString;
        protected SqlHelper Sql;

        protected SqlRepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
            Sql = new SqlHelper(_connectionString);
        }
    }
}