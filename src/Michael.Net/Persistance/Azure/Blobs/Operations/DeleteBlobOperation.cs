﻿using Azure.Storage.Blobs;
using Michael.Net.Extensions;
using Michael.Net.Persistence.Azure.Blobs.Operations.Base;

namespace Michael.Net.Persistence.Azure.Blobs.Operations
{
    public class DeleteBlobOperation : BlobOperation
    {
        readonly BlobContainerClient containerClient;
        readonly string fullFileName;
        string tempFileFullName = default!;

        public DeleteBlobOperation(
            BlobContainerClient containerClient,
            string fullFileName)
        {
            this.containerClient = containerClient.ThrowIfNull();
            this.fullFileName = fullFileName.ThrowIfNullEmptyOrWhiteSpace();
        }

        public override Task Execute()
        {
            var subjectBlobClient = containerClient.GetBlobClient(fullFileName);
            return subjectBlobClient.DeleteAsync();
        }

        public override async Task<object?> ExecuteInTransaction()
        {
            tempFileFullName = GetBackupBlobName();
            var subjectBlobClient = containerClient.GetBlobClient(fullFileName);
            var tempBlobClient = containerClient.GetBlobClient(tempFileFullName);
            await tempBlobClient.SyncCopyFromUriAsync(subjectBlobClient.Uri);
            await Execute();
            return null;
        }

        public override async Task Rollback()
        {
            var subjectBlobClient = containerClient.GetBlobClient(fullFileName);
            var tempBlobClient = containerClient.GetBlobClient(tempFileFullName);
            await subjectBlobClient.SyncCopyFromUriAsync(tempBlobClient.Uri);
            await ClearBackups();
        }

        public override async Task ClearBackups()
        {
            var tempBlobClient = containerClient.GetBlobClient(tempFileFullName);
            await tempBlobClient.DeleteAsync();
        }
    }
}
