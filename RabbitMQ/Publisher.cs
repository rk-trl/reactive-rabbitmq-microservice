using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class Publisher : IPublisher
    {
        private readonly string _exchangeType;
        private readonly int _timeToLive;
        private readonly IConnectionProvider _connectionProvider;
        private readonly string _exchange;
        private readonly IModel _model;
        private bool _disposed = false;
        public Publisher(IConnectionProvider connectionProvider, string exchange, string exchangeType, int timeToLive = 30000)
        {
            _connectionProvider = connectionProvider;
            _exchange = exchange;
            _exchangeType = exchangeType;
            var ttl = new Dictionary<string, object>
            {
                {"x-message-ttl",timeToLive }
            };
            _model = connectionProvider.GetConnection().CreateModel();
            _timeToLive = timeToLive;
            _model.ExchangeDeclare(_exchange, exchangeType, arguments:ttl);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if(_disposed) return;

            if(disposing)
            {
                _model?.Close();
            }
            _disposed = true;
        }

        public void Publish(string message, string routingKey, IDictionary<string, object> messageAttributes, string timToLive = null)
        {
            var body = Encoding.UTF8.GetBytes(message);
            var properties = _model.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = messageAttributes;
            properties.Expiration = timToLive;

            _model.BasicPublish(_exchange, routingKey, properties, body);
        }
    }
}
