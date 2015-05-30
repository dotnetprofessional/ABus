namespace ABus.Config.Transactions
{
    public class TransactionsGrammar
    {
        internal ConfigurationGrammar Parent { get; set; }

        public TransactionsGrammar(ConfigurationGrammar parent)
        {
            this.Parent = parent;
        }

        public TransactionsGrammar WithTransactionManager<T>(string connectionstring)
        {
            var tm = typeof (T);
            this.Parent.Configuration.Transactions.TransactionManagerType = tm.AssemblyQualifiedName;
            this.Parent.Configuration.Transactions.TransactionManagerConnectionString = connectionstring;

            return this;
        }

        public TransactionsGrammar Disable()
        {
            this.Parent.Configuration.Transactions.TransactionsEnabled = false;
            return this;
        }
    }
}
