using BrokerGRPC.Models;

namespace BrokerGRPC.Services.Interfaces
{
    public interface IConnectionStorageService
    {
        void AddConnection(Connection connection);
        void RemoveConnection(string address);
        IList<Connection> GetConnectionsByTopic(string topic);
    }
}
