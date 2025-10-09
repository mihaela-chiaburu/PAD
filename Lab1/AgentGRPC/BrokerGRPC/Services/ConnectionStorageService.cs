using BrokerGRPC.Models;
using BrokerGRPC.Services.Interfaces;

namespace BrokerGRPC.Services
{
    public class ConnectionStorageService : IConnectionStorageService
    {
        private List<Connection> _connections = new List<Connection>();
        private readonly object _locker;

        public ConnectionStorageService()
        {
            _connections = new List<Connection>();
            _locker = new object();
        }

        public void AddConnection(Connection connection)
        {
            lock (_locker)
            {
                _connections.Add(connection);
            }
        }

        public IList<Connection> GetConnectionsByTopic(string topic)
        {
            lock (_locker)
            {
                var filteredConnections = _connections.Where(c => c.Topic == topic).ToList();
                return filteredConnections;
            }
        }

        public void RemoveConnection(string address)
        {
            lock (_locker)
            {
                _connections.RemoveAll(c => c.Address == address);
            }
        }
    }
}
