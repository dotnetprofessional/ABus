using ABus.Contracts;

namespace ABus.Config.Transactions
{
    public class TransactionOptions
    {
        public TransactionOptions()
        {
            this.TransactionsEnabled = true;
        }

        public bool TransactionsEnabled { get; set; }

        public string TransactionManagerType { get; set; }

        public string TransactionManagerConnectionString { get; set; }
    }
}
