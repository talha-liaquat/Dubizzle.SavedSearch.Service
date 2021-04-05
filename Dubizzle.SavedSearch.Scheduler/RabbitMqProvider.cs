using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dubizzle.SavedSearch.Scheduler
{
    class RabbitMqProvider : IQueueProvider<InternalMessageEnvelopDto>
    {
        private readonly IConnection _connection;
        private readonly IModel _model;

        public RabbitMqProvider(IConfiguration configuration)
        {
            var rabbitMqSection = configuration.GetSection("RabbitMq");

            var factory = new ConnectionFactory()
            {
                UserName = rabbitMqSection["Username"],
                Password = rabbitMqSection["Password"],
                HostName = rabbitMqSection["Host"],
                VirtualHost = rabbitMqSection["VirtualHost"]
            };

            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
        }

        public event EventHandler OnMessageReceived;

        public void BasicPublish(string exchange, string key, InternalMessageEnvelopDto message)
        {
            var properties = _model.CreateBasicProperties();

            var encodedMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            properties.DeliveryMode = 2;
            properties.Headers = new Dictionary<string, object>()
            {
                { "correlation-id", message.CorrelationId },
                { "time-stamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")}
            };

            _model.BasicPublish(exchange, key, true, properties, encodedMessage);
        }

        public void Commit(InternalMessageEnvelopDto message)
        {
            _model.BasicAck(message.Tag, false);
        }

        public void ExchangeDeclare(string exchange, string type, bool durable = true)
        {
            _model.ExchangeDeclare(exchange, type, durable);
        }

        public void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments = null)
        {
            _model.QueueBind(queue, exchange, routingKey, arguments);
        }

        public void QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            _model.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        }

        public void Read(string queueName)
        {
            _model.BasicQos(0, 1, true);
            var eventingBasicConsumer = new EventingBasicConsumer(_model);
            _model.BasicConsume(queueName, false, eventingBasicConsumer);

            eventingBasicConsumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonConvert.DeserializeObject<InternalMessageEnvelopDto>(Encoding.Default.GetString(body));
                if (message == null)
                    _model.BasicNack(ea.DeliveryTag, false, false);
                else
                {
                    message.Tag = ea.DeliveryTag;
                    OnMessageReceived?.Invoke(message, ea);
                }
            };
        }

        public void BindExchangeAndQueues(string exchange, string exchangeRetry, string queue, int logTtl = 432000000, int retryTtl = 900000)
        {
            ExchangeDeclare(exchange, "topic", true);

            ExchangeDeclare(exchangeRetry, "topic", true);

            QueueDeclare(queue, true, false, false, new Dictionary<string, object> {
                            { "x-dead-letter-exchange", exchangeRetry }});
            QueueBind(queue, exchange, $"{queue}.#", null);

            QueueDeclare($"{queue}.Log", true, false, false, new Dictionary<string, object> {
                            { "x-message-ttl", logTtl }});
            QueueBind($"{queue}.Log", exchange, $"{queue}.#", null);

            QueueDeclare($"{queue}.Retry", true, false, false, new Dictionary<string, object> {
                            { "x-dead-letter-exchange", exchange },
                            { "x-message-ttl", retryTtl }});
            QueueBind($"{queue}.Retry", exchangeRetry, $"{queue}.#", null);
        }

        public void Rollback(InternalMessageEnvelopDto message)
        {
            _model.BasicNack(message.Tag, false, false);
        }
    }
}