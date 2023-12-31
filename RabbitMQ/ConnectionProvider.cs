﻿using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class ConnectionProvider : IConnectionProvider
    {
        private readonly IConnection _connection;
        private readonly ConnectionFactory _factory;
        private bool _disposed = false;
        public ConnectionProvider(string url)
        {
            _factory = new ConnectionFactory { Uri = new Uri(url) };
            _connection = _factory.CreateConnection();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _connection?.Close();
            }
            _disposed = true;
        }
        public IConnection GetConnection()
        {
            return _connection;
        }
    }
}
