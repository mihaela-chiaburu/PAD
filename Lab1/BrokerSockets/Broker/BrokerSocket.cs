using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Broker
{
    public class BrokerSocket
    {
        private const int CONNECTIONS_LIMIT = 8;
        private Socket _socket;

        public BrokerSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(string ip, int port)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            _socket.Listen(CONNECTIONS_LIMIT);
            Console.WriteLine($"Broker listening on {ip}:{port}");
            Accept();
        }

        private void Accept()
        {
            _socket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connection = new ConnectionInfo();
            try
            {
                connection.socket = _socket.EndAccept(asyncResult);
                connection.Adress = connection.socket.RemoteEndPoint.ToString();
                connection.socket.BeginReceive(connection.Data, 0, connection.Data.Length, 
                    SocketFlags.None, ReceiveCallback, connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting connection: {ex.Message}");
            }
            finally
            {
                Accept(); 
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connection = (ConnectionInfo)asyncResult.AsyncState;
            try
            {
                Socket senderSocket = connection.socket;
                SocketError response;
                int buffSize = senderSocket.EndReceive(asyncResult, out response);

                if (buffSize == 0 || response != SocketError.Success)
                {
                    Console.WriteLine($"Subscriber disconnected: {connection.Adress}");
                    ConnectionsStorage.Remove(connection.Adress);
                    connection.socket.Close();
                    return;
                }

                byte[] payload = new byte[buffSize];
                Array.Copy(connection.Data, payload, payload.Length);

                PayloadHandler.Handle(payload, connection);

                var payloadString = Encoding.UTF8.GetString(payload);
                if (!payloadString.StartsWith("subscribe#"))
                {
                    Payload message = JsonConvert.DeserializeObject<Payload>(payloadString);
                    foreach (var conn in ConnectionsStorage.GetSubscribersByTopic(message.Topic))
                    {
                        try
                        {
                            conn.socket.Send(Encoding.UTF8.GetBytes(payloadString));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving data from {connection.Adress}: {ex.Message}");
            }
            finally
            {
                try
                {
                    connection.socket.BeginReceive(connection.Data, 0, connection.Data.Length,
                        SocketFlags.None, ReceiveCallback, connection);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Subscriber disconnected (left): {connection.Adress}");
                    ConnectionsStorage.Remove(connection.Adress);
                    connection.socket.Close();
                }
            }
        }
    }
}
