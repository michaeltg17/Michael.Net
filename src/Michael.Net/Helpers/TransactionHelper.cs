using System.Transactions;

namespace Michael.Net.Helpers
{
    public static class TransactionHelper
    {
        public static bool IsInTransaction()
        {
            return Transaction.Current != null;
        }
    }
}
