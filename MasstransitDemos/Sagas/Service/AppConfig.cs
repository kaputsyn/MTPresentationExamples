using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal class AppConfig
    {
        public RabbitMqConfig RabbitMq { get; set; }
    }

    internal class RabbitMqConfig 
    {
        public string HostAddresses { get; set; }

        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}
