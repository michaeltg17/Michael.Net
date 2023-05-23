using Azure;
using Azure.Data.Tables;

namespace Michael.Net.Tests.Persistence.Azure.Tables
{
    public record Entity : ITableEntity
    {
        public string String { get; set; } = default!;
        public int Int { get; set; }
        public DateTime? DateTime { get; set; }
        public bool Bool { get; set; }
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
    }
}
