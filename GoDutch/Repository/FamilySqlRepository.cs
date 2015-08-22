using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using GoDutch.Common.Models;
using GoDutch.Common.Repository;

namespace GoDutch.Repository
{
    public class FamilySqlRepository : SqlRepositoryBase, IFamilyRepository
    {
        public FamilySqlRepository() : this(ConfigurationManager.ConnectionStrings["GoDutch"].ToString())
        {
        }

        private FamilySqlRepository(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<Family> Get()
        {
            const string sql = @"select Id, Name from dbo.Family order by Name";

            using (var reader = Sql.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    yield return new Family()
                    {
                        Id = reader.Get<int>("Id"),
                        Name = reader.Get<string>("Name"),
                    };
                }
            }
        }
    }
}