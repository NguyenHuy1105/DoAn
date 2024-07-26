using ComputerManagement.Common.Configs;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ComputerManagement.Service.Queue
{
    public abstract class SubcriberBase
    {
        protected IConnection _connection;
        protected IModel _channel;
        protected string _queueName = "";

        public void ReceiveFromQueue(RabbitMQConfig rbConfig)
        {
            var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(100), (exception, timeSpan) =>
            {
                Console.WriteLine($"Error occurred: {exception.Message}. Retrying in {timeSpan.TotalSeconds} seconds...");
            });
            policy.ExecuteAsync(() =>
            {
                try
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = rbConfig.HostName,
                        Port = rbConfig.Port,
                        UserName = rbConfig.Username,
                        Password = rbConfig.Password,
                        VirtualHost = rbConfig.VirtualHost

                    };
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _channel.QueueDeclare(queue: _queueName, durable: rbConfig.Durable, exclusive: rbConfig.Exclusive, autoDelete: rbConfig.AutoDelete, arguments: null);
                    _channel.BasicQos(rbConfig.PerfetchSize, rbConfig.PerfetchCount, rbConfig.Global);
                    var consumer = new EventingBasicConsumer(_channel);

                    consumer.Received += (object? model, BasicDeliverEventArgs ea) =>
                    {
                        // nhận dữ liệu từ queue và xử lý
                        this.DeQueue(model, ea);
                    };
                    _channel.BasicConsume(queue: _queueName, autoAck: rbConfig.AutoAck, consumer: consumer);
                }catch(Exception ex)
                {
                    Console.WriteLine($"Failed to connect to RabbitMQ: {ex.Message}");
                    throw;
                }
                

                return Task.CompletedTask;
            });
        }

        public virtual void DeQueue(object model, BasicDeliverEventArgs ea)
        {
            _channel.BasicAck(ea.DeliveryTag, false);
        }
    }
}
