using Azure.Storage.Blobs;
using Michael.Net.Extensions;
using Michael.Net.Persistence.Azure.Blobs.Operations;
using Michael.Net.Persistence.Azure.Blobs.Operations.Base;
using static Michael.Net.Helpers.TransactionHelper;

namespace Michael.Net.Persistence.Azure.Blobs
{
    public class BlobStorage : IObjectStorage
    {
        readonly string connectionString;
        readonly string containerName;
        readonly IBlobStorageResourceManager blobStorageResourceManager;

        public BlobStorage(
            string connectionString,
            string containerName,
            IBlobStorageResourceManager blobStorageResourceManager)
        {
            this.connectionString = connectionString.ThrowIfNullEmptyOrWhiteSpace();
            this.containerName = containerName.ThrowIfNullEmptyOrWhiteSpace();
            this.blobStorageResourceManager = blobStorageResourceManager.ThrowIfNull();
        }

        public Task Delete(string fullFileName)
        {
            var container = new BlobContainerClient(connectionString, containerName);
            var operation = new DeleteBlobOperation(container, fullFileName);

            if (IsInTransaction())
            {
                return blobStorageResourceManager.ExecuteOperation(operation);
            }

            return operation.Execute();
        }

        public Task<bool> HasTransactionalBackupBlobs()
        {
            var containerClient = new BlobContainerClient(connectionString, containerName);
            return containerClient
                .GetBlobsAsync(prefix: TransactionalBlobOperation.BackupPrefix)
                .AnyAsync()
                .AsTask();
        }

        public async Task<Stream?> Get(string fullFileName)
        {
            var containerClient = new BlobContainerClient(connectionString, containerName);
            var blobClient = containerClient.GetBlobClient(fullFileName);

            return await blobClient.ExistsAsync()
                ? await blobClient.OpenReadAsync()
                : null;
        }

        public Task<Stream> GetOrThrow(string fullFileName)
        {
            var containerClient = new BlobContainerClient(connectionString, containerName);
            var blobClient = containerClient.GetBlobClient(fullFileName);

            return blobClient.OpenReadAsync();
        }

        public async Task<string> Upload(string fullFileName, Stream stream)
        {
            var container = new BlobContainerClient(connectionString, containerName);
            var operation = new UploadBlobOperation(container, fullFileName, stream);

            if (IsInTransaction())
            {
                return await blobStorageResourceManager.ExecuteOperation(operation);
            }

            return await operation.Execute();
        }
    }
}
