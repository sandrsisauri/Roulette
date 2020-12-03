using System;
using System.Data;
using System.Data.SqlClient;

namespace Roulette.Data
{
    public interface IDbConnectionProvider
    {
        IDbConnection Connection { get; }
    }
    public class DataContext : IDbConnectionProvider
    {
        private readonly IDbConnection _connection;
        public DataContext(string connection)
        {
            _connection = new SqlConnection(connection);
        }

        public IDbConnection Connection { get => _connection; }
    }
}
