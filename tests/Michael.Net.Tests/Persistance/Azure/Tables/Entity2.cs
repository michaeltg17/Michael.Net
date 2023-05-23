using Azure;
using Azure.Data.Tables;

namespace Michael.Net.Tests.Persistence.Azure.Tables
{
    public record Entity2 : ITableEntity
    {
        public string PropertyString { get; set; } = default!;
        public int PropertyInt { get; set; }
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
    }
}
