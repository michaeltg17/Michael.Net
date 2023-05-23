using System.Data;

namespace Michael.Net.Persistence
{
    public interface IQuery<T>
    {
        public Task<T> Execute(IDbConnection connection);
    }
}
