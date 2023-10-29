using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public interface IPublisher: IDisposable
    {
        public void Publish(string message, string routingKey, IDictionary<string, object> messageAttributes, string timToLive = null);
    }
}
