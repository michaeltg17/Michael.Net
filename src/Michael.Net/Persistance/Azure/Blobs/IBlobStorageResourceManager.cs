using Michael.Net.Persistence.Azure.Blobs.Operations.Base;

namespace Michael.Net.Persistence.Azure.Blobs
{
    public interface IBlobStorageResourceManager
    {
        public Task ExecuteOperation(BlobOperation operation);
        public Task<T> ExecuteOperation<T>(BlobOperation<T> operation);
    }
}
