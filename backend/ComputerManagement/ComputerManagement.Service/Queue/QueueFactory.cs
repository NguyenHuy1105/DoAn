using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using ComputerManagement.Common.Configs;
using RabbitMQ.Client;

namespace ComputerManagement.Service.Queue
{
    public static class QueueFactory
    {
        private static IConnection _connection;
        private static IModel _channel;
        private static IPublisher _publisher;

        private static void CheckConnection(RabbitMQConfig rbConfig)
        {
            if (_publisher == null)
            {
                var factory = new ConnectionFactory()
                {
                    HostName = rbConfig.HostName,
                    Port = rbConfig.Port,
                    VirtualHost = rbConfig.VirtualHost,
                    UserName = rbConfig.Username,
                    Password = rbConfig.Password
                };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _publisher = new Publisher(_channel, rbConfig);
            }
        }

        public static bool EnQueue(RabbitMQConfig rbConfig,string queueName ,string message)
        {
            CheckConnection(rbConfig);
            return _publisher.EnQueue(queueName,message);
        }
    }
}
