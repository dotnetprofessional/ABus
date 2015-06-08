namespace ABus.Contracts
{
    /// <summary>
    /// NOTES:
    /// 
    /// With Outbox pattern
    /// AzureSQLTransaction
    /// 
    /// Without outbox pattern
    /// AzureBasicTransaction
    /// 
    /// incomming message: new AzureSQLTransaction
    ///  
    /// 
    /// Logic
    /// Unit of Work handler is generic and not specific to a persistence layer so is part of the IncommingMessageContext
    ///   but does take an instance of IManageTransactions which can be wired up via boostrapper. Commit delegates to IManageTransactions
    /// Define transaction strategy ie SQLServerTransaction
    /// dequeue msg 
    /// Start msg tx (via a peek)
    /// Start Tx
    /// if(!msg previously processed)
    ///   
    ///   Invoke Handler
    ///      Record Send/Publish/Reply via UoW
    ///   Commit UoW
    /// end if
    /// commit tx
    /// 
    /// *** No Db tx as 
    /// foreach msg in GetMessages from IManageTransactions
    ///   Send Msg - (RawMessage will have details necessary to select correct transport and intent)
    ///   Mark as complete
    /// 
    /// End msg tx (via CompleteAsync)
    /// 
    /// 
    /// Package:
    /// No special logic needed for providers
    /// SQLGuaranteedDelivery
    ///   IManageTransactions implementation
    /// 
    /// </summary>


    /// <summary>
    /// Maintains a list of messages affected by a business transaction and coordinates the writing 
    /// out of changes and the resolution of concurrency problems.
    /// </summary>
    /// <remarks> 
    /// Azure doesn't support DTC transactions, this interface provides a way to keep all transactions
    /// within the same resource context. Examples would be SQL Server used to record message transactions.
    /// </remarks>
    public interface IManageOutboundMessages
    {
        string InboundMessageId { get; set; }
        void Begin();

        void Commit();

        /// <summary>
        /// Record them in a stack
        /// </summary>
        /// <param name="message"></param>
        void AddItem(RawMessage message);

        bool AlreadyProcessed();

        IManageTransactions TransactionManager { get; }
    }
}