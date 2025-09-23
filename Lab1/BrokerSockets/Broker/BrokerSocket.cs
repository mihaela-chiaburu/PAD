using Helpers;
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
                if (response == SocketError.Success)
                {
                    byte[] payload = new byte[buffSize];
                    Array.Copy(connection.Data, payload, payload.Length);

                    PayloadHandler.Handle(payload, connection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving data from {connection.Adress}: {ex.Message}");
                //connection.socket.Close();
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
                    Console.WriteLine($"{e.Message}");
                    var address = connection.socket.RemoteEndPoint.ToString();
                    ConnectionsStorage.Remove(address);
                    connection.socket.Close();
                }
            }
        }
    }
}
