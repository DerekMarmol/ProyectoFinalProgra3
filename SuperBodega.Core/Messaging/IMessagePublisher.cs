using System;
using System.Threading.Tasks;

namespace SuperBodega.Core.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(string topic, T message);
    }
}