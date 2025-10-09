using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broker
{
    static class ConnectionsStorage
    {
        private static List<ConnectionInfo> _connections;
        private static object _locker;

        static ConnectionsStorage()
        {
            _connections = new List<ConnectionInfo>();
            _locker = new object();
        }

        public static void AddOrUpdate(ConnectionInfo connection, string topic)
        {
            lock (_locker)
            {
                var existing = _connections.FirstOrDefault(c => c.Adress == connection.Adress);
                if (existing != null)
                {
                    if (!existing.Topics.Contains(topic))
                        existing.Topics.Add(topic);
                }
                else
                {
                    connection.Topics.Add(topic);
                    _connections.Add(connection);
                }
            }
        }

        public static void Remove(string address)
        {
            lock (_locker)
            {
                _connections.RemoveAll(x => x.Adress == address);
            }
        }

        public static List<ConnectionInfo> GetSubscribersByTopic(string topic)
        {
            lock (_locker)
            {
                return _connections.Where(c => c.Topics.Contains(topic)).ToList();
            }
        }

        public static List<ConnectionInfo> GetAll()
        {
            lock (_locker)
            {
                return _connections.ToList();
            }
        }
    }
}
