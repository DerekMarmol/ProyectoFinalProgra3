using System;
using System.Threading.Tasks;

namespace SuperBodega.Core.Messaging
{
    public interface IMessageConsumer
    {
        void Subscribe<T>(string topic, Func<T, Task> handler);
        void StartConsuming();
        void StopConsuming();
    }
}