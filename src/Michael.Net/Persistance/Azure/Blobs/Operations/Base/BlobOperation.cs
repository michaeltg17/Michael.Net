namespace Michael.Net.Persistence.Azure.Blobs.Operations.Base
{
    public abstract class BlobOperation : TransactionalBlobOperation
    {
        public abstract Task Execute();
    }
}
