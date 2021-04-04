using System;
using System.Collections.Generic;

namespace Dubizzle.SavedSearch.Contracts
{
    public interface IQueueProvider<T>
    {
        void ExchangeDeclare(string exchange, string type, bool durable = true);
        void QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments);
        void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments = null);
        void BasicPublish(string exchange, string key, T message);
        event EventHandler OnMessageReceived;
        void Read(string queueName);
        void Commit(T message);
        void Rollback(T message);
    }
}
