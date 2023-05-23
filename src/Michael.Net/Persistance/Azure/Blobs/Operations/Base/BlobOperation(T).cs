namespace Michael.Net.Persistence.Azure.Blobs.Operations.Base
{
    public abstract class BlobOperation<T> : TransactionalBlobOperation
    {
        public abstract Task<T> Execute();
    }
}
