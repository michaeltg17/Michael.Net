﻿using System.Transactions;
using Michael.Net.Persistence.Azure.Blobs.Operations.Base;

namespace Michael.Net.Persistence.Azure.Blobs
{
    public class BlobStorageResourceManager : IEnlistmentNotification, IBlobStorageResourceManager
    {
        readonly List<TransactionalBlobOperation> executedOperations = new();

        public Task ExecuteOperation(BlobOperation operation)
        {
            AddOperation(operation);
            return operation.ExecuteInTransaction();
        }

        public async Task<T> ExecuteOperation<T>(BlobOperation<T> operation)
        {
            AddOperation(operation);
            return (T)(await operation.ExecuteInTransaction())!;
        }

        void AddOperation(TransactionalBlobOperation operation)
        {
            if (!executedOperations.Any())
            {
                var currentTransaction = Transaction.Current;
                currentTransaction!.EnlistVolatile(this, EnlistmentOptions.None);
            }

            executedOperations.Add(operation);
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Commit(Enlistment enlistment)
        {
            foreach (var operation in executedOperations)
            {
                operation.ClearBackups();
            }
            enlistment.Done();
        }

        public void Rollback(Enlistment enlistment)
        {
            executedOperations.Reverse();
            foreach (var operation in executedOperations)
            {
                operation.Rollback().GetAwaiter().GetResult();
            }

            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            Rollback(enlistment);
        }
    }
}
