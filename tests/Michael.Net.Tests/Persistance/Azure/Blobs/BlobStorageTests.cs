using FluentAssertions;
using Xunit;
using Michael.Net.Extensions;
using System.Transactions;
using Azure;
using Michael.Net.Persistence.Azure.Blobs;

namespace Michael.Net.Tests.Persistence.Azure.Blobs
{
    public class BlobStorageTests : IDisposable
    {
        readonly List<string> blobsToDelete = new();

        static BlobStorage GetBlobStorage()
        {
            return new BlobStorage(
                Settings.AzureStorageAccountConnectionString,
                "tests",
                new BlobStorageResourceManager());
        }

        static (Stream stream, byte[] content, string fullFileName) GetRandomFile()
        {
            var content = new byte[1024];
            Random.Shared.NextBytes(content);
            var stream = new MemoryStream(content);
            var fullFileName = $"{nameof(BlobStorageTests)}_{Guid.NewGuid()}.txt";

            return (stream, content, fullFileName);
        }

        [Fact]
        public async Task GivenFile_WhenUpload_Uploaded()
        {
            //Given
            var storage = GetBlobStorage();
            var (stream, blobContent, blob) = GetRandomFile();

            //When
            await storage.Upload(blob, stream);
            blobsToDelete.Add(blob);

            //Then
            var downloadedBlob = await storage.GetOrThrow(blob);
            downloadedBlob.ReadAllBytes().Should().BeEquivalentTo(blobContent);
            await ValidateNoTransactionalBackupBlobsLeft();
        }

        [Fact]
        public async Task GivenFile_WhenUploadInOkTransaction_Uploaded()
        {
            //Given
            var storage = GetBlobStorage();
            var (stream, blobContent, blob) = GetRandomFile();

            //When
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await storage.Upload(blob, stream);
                blobsToDelete.Add(blob);
                transactionScope.Complete();
            }

            //Then
            var downloadedBlob = await storage.GetOrThrow(blob);
            downloadedBlob.ReadAllBytes().Should().BeEquivalentTo(blobContent);
            await ValidateNoTransactionalBackupBlobsLeft();
        }

        [Fact]
        public async Task GivenFile_WhenUploadInFailedTransaction_Deleted()
        {
            //Given
            var storage = GetBlobStorage();
            var (stream, uploadedFile, uploadedFileFullName) = GetRandomFile();

            //When
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await storage.Upload(uploadedFileFullName, stream);
            }

            //Then
            var getUploadedFile = () => storage.GetOrThrow(uploadedFileFullName);
            (await getUploadedFile.Should().ThrowAsync<RequestFailedException>()).Which.ErrorCode.Should().Be("BlobNotFound");
            await ValidateNoTransactionalBackupBlobsLeft();
        }

        [Fact]
        public async Task GivenBlob_WhenDelete_Deleted()
        {
            //Given
            var storage = GetBlobStorage();
            var (stream, _, uploadedFileFullName) = GetRandomFile();
            await storage.Upload(uploadedFileFullName, stream);

            //When
            await storage.Delete(uploadedFileFullName);

            //Then
            (await storage.Get(uploadedFileFullName)).Should().BeNull();
            await ValidateNoTransactionalBackupBlobsLeft();
        }

        [Fact]
        public async Task GivenBlob_WhenDeleteInOkTransaction_Deleted()
        {
            //Given
            var storage = GetBlobStorage();
            var (stream, _, uploadedFileFullName) = GetRandomFile();
            await storage.Upload(uploadedFileFullName, stream);

            //When
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await storage.Delete(uploadedFileFullName);
                transactionScope.Complete();
            }

            //Then
            (await storage.Get(uploadedFileFullName)).Should().BeNull();
            await ValidateNoTransactionalBackupBlobsLeft();
        }

        [Fact]
        public async Task GivenBlob_WhenDeleteInFailedTransaction_NotDeleted()
        {
            //Given
            var storage = GetBlobStorage();
            var (stream, blobContent, blob) = GetRandomFile();
            await storage.Upload(blob, stream);
            blobsToDelete.Add(blob);

            //When
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await storage.Delete(blob);
            }

            //Then
            (await storage.Get(blob))!.ReadAllBytes().Should().BeEquivalentTo(blobContent);
            await ValidateNoTransactionalBackupBlobsLeft();
        }

        [Fact]
        public async Task GivenBlob_WhenDeleteUploadInFailedTransaction_OriginalBlob()
        {
            //Given
            var storage = GetBlobStorage();
            var (stream1, blob1Content, blob1) = GetRandomFile();
            await storage.Upload(blob1, stream1);
            blobsToDelete.Add(blob1);
            var (stream2, _, blob2) = GetRandomFile();

            //When
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await storage.Delete(blob1);
                await storage.Upload(blob2, stream2);
            }

            //Then
            (await storage.Get(blob1))!.ReadAllBytes().Should().BeEquivalentTo(blob1Content);
            (await storage.Get(blob2)).Should().BeNull();
            await ValidateNoTransactionalBackupBlobsLeft();
        }

        static async Task ValidateNoTransactionalBackupBlobsLeft()
        {
            var storage = GetBlobStorage();
            (await storage.HasTransactionalBackupBlobs()).Should().BeFalse();
        }

        public void Dispose()
        {
            var storage = GetBlobStorage();
            foreach (var blob in blobsToDelete)
            {
                storage.Delete(blob).GetAwaiter().GetResult();
            }
            GC.SuppressFinalize(this);
        }
    }
}
