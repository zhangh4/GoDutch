using StackExchange.Redis;

namespace GoDutch.Redis
{
    public abstract class RedisRepositoryBase
    {
        protected IConnectionMultiplexer connection;
        protected string host;
        protected int port;

        public RedisRepositoryBase(IConnectionMultiplexer connection, string host, int port)
        {
            this.connection = connection;
            this.host = host;
            this.port = port;
        }
    }
}