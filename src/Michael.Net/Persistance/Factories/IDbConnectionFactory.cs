using System.Data;

namespace Michael.Net.Persistence.Factories
{
    public interface IDbConnectionFactory<out T> where T : IDbConnection
    {
        T CreateAndOpen(string connectionString);
    }
}
