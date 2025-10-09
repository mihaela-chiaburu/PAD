namespace BrokerGRPC.Models
{
    public class Connection
    {
        public string Address { get; set; }
        public string Topic { get; set; }

        public Connection(string address, string topic)
        {
            Address = address;
            Topic = topic;
        }
    }
}
