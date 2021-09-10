using Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    internal class FaultConsumer : IConsumer<Fault<ITransienrErrorCommand>>
    {
        public Task Consume(ConsumeContext<Fault<ITransienrErrorCommand>> context)
        {
            Console.WriteLine($"Error logged error: {context.Message.Message.ErrorCode}");

            return Task.CompletedTask;
        }
    }
}
