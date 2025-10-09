using BrokerGRPC.Models;

namespace BrokerGRPC.Services.Interfaces
{
    public interface IMessageStorageService
    {
        void Add(Message message);
        Message? GetNext();
        bool IsEmpty();
    }
}
