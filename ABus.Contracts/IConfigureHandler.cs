namespace ABus.Contracts
{
    public interface IConfigureHandler<T>
    {
        void HandlerConfig(RegisteredHandler handler);
    }
}