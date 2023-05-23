using Azure.Data.Tables;
using Michael.Net.Extensions;

namespace Michael.Net.Persistence.Azure.Tables
{
    public class TableStorage
    {
        readonly string connectionString;
        readonly string? tableName;

        public TableStorage(string connectionString, string tableName = null)
        {
            this.connectionString = connectionString.ThrowIfNullEmptyOrWhiteSpace();
            this.tableName = tableName;
        }

        string GetTableName<T>() => string.IsNullOrWhiteSpace(tableName) ? typeof(T).Name : tableName;

        TableClient GetTableClient<T>()
        {
            var tableServiceClient = new TableServiceClient(connectionString);
            return tableServiceClient.GetTableClient(GetTableName<T>());
        }

        public async Task<T> GetOrThrow<T>(T entity) where T : class, ITableEntity
        {
            var tableClient = GetTableClient<T>();
            return (await tableClient.GetEntityAsync<T>(entity.PartitionKey, entity.RowKey)).Value;
        }

        public async Task Insert<T>(T entity) where T : ITableEntity
        {
            var tableClient = GetTableClient<T>();
            await tableClient.CreateIfNotExistsAsync();

            await tableClient.AddEntityAsync(entity);
        }

        public Task Delete<T>(T entity) where T : ITableEntity
        {
            var tableClient = GetTableClient<T>();
            return tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
        }
    }
}
