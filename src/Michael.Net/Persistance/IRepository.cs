using Michael.Net.Domain;

namespace Michael.Net.Persistence
{
    public interface IRepository
    {
        Task<T?> Get<T>(int id) where T : class, IIdentifiable, new();

        Task<T> GetOrThrow<T>(int id) where T : class, IIdentifiable, new();

        Task<IEnumerable<T>> Get<T>() where T : class, IIdentifiable, new();

        Task<T> Get<T>(IQuery<T> query) where T : class, IIdentifiable, new();

        Task Insert<T>(T entity) where T : class, IIdentifiable, new();

        Task Update<T>(T entity) where T : class, IIdentifiable, new();

        Task Delete<T>(int id) where T : class, IIdentifiable, new();

        Task Delete<T>(T entity) where T : class, IIdentifiable, new();
    }
}
