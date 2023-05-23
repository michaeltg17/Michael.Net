using Microsoft.Data.SqlClient;

namespace Michael.Net.Persistence.Factories
{
    public class SqlConnectionFactory : IDbConnectionFactory<SqlConnection>
    {
        public SqlConnection CreateAndOpen(string connectionString)
        {
            var sqlConnection = new SqlConnection(connectionString);

            sqlConnection.Open();

            return sqlConnection;
        }
    }
}
