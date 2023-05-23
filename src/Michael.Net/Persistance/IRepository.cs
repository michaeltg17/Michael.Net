using Michael.Net.Domain;

namespace Michael.Net.Persistence
{
    public interface IRepository
    {
        Task<T?> Get<T>(int id) where T : class, IEntity, new();

        Task<T> GetOrThrow<T>(int id) where T : class, IEntity, new();

        Task<IEnumerable<T>> Get<T>() where T : class, IEntity, new();

        Task<T> Get<T>(IQuery<T> query) where T : class, IEntity, new();

        Task Insert<T>(T entity) where T : class, IEntity, new();

        Task Update<T>(T entity) where T : class, IEntity, new();

        Task Delete<T>(int id) where T : class, IEntity, new();

        Task Delete<T>(T entity) where T : class, IEntity, new();
    }
}
