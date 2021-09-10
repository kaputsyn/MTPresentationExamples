using System;

namespace Saga.Contracts
{
    public interface SubmitOrder
    {
        Guid OrderId { get; }
        DateTime TimeStamp { get; }

        string CustomerNumber { get; }
    }
}
