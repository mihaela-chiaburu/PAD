using Grpc.Net.Client;

namespace BrokerGRPC.Models
{
    public class Connection
    {
        public string Address { get; set; }
        public string Topic { get; set; }
        public GrpcChannel Channel { get; }

        public Connection(string address, string topic)
        {
            Address = address;
            Topic = topic;
            Channel = GrpcChannel.ForAddress(address);
        }
    }
}
