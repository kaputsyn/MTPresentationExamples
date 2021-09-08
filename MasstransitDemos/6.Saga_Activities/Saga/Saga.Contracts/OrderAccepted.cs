using System;

namespace Saga.Contracts
{
    public interface OrderAccepted
    {
        Guid OrderId { get; }
        DateTime TimeStamp { get; }
    }
}
