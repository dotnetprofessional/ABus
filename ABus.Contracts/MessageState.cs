namespace ABus.Contracts
{
    public enum MessageState
    {
        Active = 0,
        Deferred,
        Scheduled,
        Deadlettered,
        Reply,
    }
}
