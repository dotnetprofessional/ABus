using System.Threading.Tasks;

namespace ABus.Contracts
{
    /// <summary>
    /// Specifies a handler for a reply or response message that is sent to the 
    /// ReplyTo queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHandleReplyMessage<T>
    {
        Task HandlerAsync(T message);
    }
}