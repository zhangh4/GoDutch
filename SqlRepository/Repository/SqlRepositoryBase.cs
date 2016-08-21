namespace SqlRepository.Repository
{
    public abstract class SqlRepositoryBase
    {
        protected string _connectionString;
        protected SqlHelper Sql;

        protected SqlRepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
            Sql = new SqlHelper(_connectionString);
        }
    }
}