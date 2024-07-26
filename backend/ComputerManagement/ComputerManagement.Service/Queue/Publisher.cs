using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputerManagement.Common.Configs;
using RabbitMQ.Client;

namespace ComputerManagement.Service.Queue
{
    public class Publisher : IPublisher
    {
        private IModel _channel;
        private readonly RabbitMQConfig _rbConfig;
        public Publisher(IModel model, RabbitMQConfig rbConfig)
        {
            _channel = model;
            _rbConfig = rbConfig;
        }
        public bool EnQueue(string queueName, string message)
        {
            if(_channel == null)
            {
                return false;
            }
            // declare queue
            _channel.QueueDeclare(queue: queueName, durable: _rbConfig.Durable, exclusive: _rbConfig.Exclusive, autoDelete: _rbConfig.AutoDelete, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            return true;
        }
    }
}
