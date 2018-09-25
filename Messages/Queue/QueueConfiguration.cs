using Microsoft.Extensions.Configuration;

namespace Domotique.Model
{
    public class QueueConfiguration
    {
        public string QueueApplicationTag { get; private set; }

        public string ServerName { get; private set; }

        public int ServerPort { get; private set; }

        public string Exchange { get; private set; }

        public string DefaultRoutingKey { get; private set; }

        public string Login { get; private set; }

        public string Password { get; private set; }


        public QueueConfiguration(IConfigurationSection Configurationnode)
        {
            QueueApplicationTag = Configurationnode.GetValue<string>("Tag");
            ServerName = Configurationnode.GetValue("Server", "rabbitmq");
            ServerPort = Configurationnode.GetValue("Port", 5672);
            Login = Configurationnode.GetValue("Login", "guest");
            Password = Configurationnode.GetValue("Password", "guest");
            DefaultRoutingKey = Configurationnode.GetValue("MessageTag", "");
            Exchange = Configurationnode.GetValue("Exchange", "");
        }
    }
}
