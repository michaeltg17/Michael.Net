using FluentAssertions;
using Xunit;
using Michael.Net.Persistence.Azure.Tables;
using Azure.Data.Tables;

namespace Michael.Net.Tests.Persistence.Azure.Tables
{
    public class TableStorageTests : IDisposable
    {
        //const string SkipOrNot = null; // Run all tests
        const string SkipOrNot = "Payment service"; // Skip all tests

        readonly List<ITableEntity> entitiesToDelete = new();

        static TableStorage GetTableStorage()
        {
            return new TableStorage(Settings.AzureStorageAccountConnectionString, "tests");
        }

        [Fact(Skip = SkipOrNot)]
        public async Task GivenEntity_WhenInsert_Inserted()
        {
            //Given
            var storage = GetTableStorage();
            var entity = new Entity
            {
                PartitionKey = "1",
                RowKey = "1",
                Bool = true,
                DateTime = DateTime.UtcNow,
                Int = 100,
                String = "hello"
            };
            var entity2 = new Entity2
            {
                PartitionKey = "1",
                RowKey = "2",
                PropertyInt = 100,
                PropertyString = "hello"
            };
            entitiesToDelete.Add(entity);
            entitiesToDelete.Add(entity2);

            //When
            await storage.Insert(entity);
            await storage.Insert(entity2);

            //Then
            var entity1FromStorage = await storage.GetOrThrow(entity);
            entity1FromStorage.Should().BeEquivalentTo(entity, config => config
                .Excluding(e => e.Timestamp)
                .Excluding(e => e.ETag));

            var entity2FromStorage = await storage.GetOrThrow(entity);
            entity2FromStorage.Should().BeEquivalentTo(entity, config => config
                .Excluding(e => e.Timestamp)
                .Excluding(e => e.ETag));
        }

        public void Dispose()
        {
            var storage = GetTableStorage();
            foreach(var entity in entitiesToDelete)
            {
                storage.Delete(entity).GetAwaiter().GetResult();
            }
            GC.SuppressFinalize(this);
        }
    }
}
