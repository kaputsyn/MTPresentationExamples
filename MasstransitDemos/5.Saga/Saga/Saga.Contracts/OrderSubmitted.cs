using System;
using System.Collections.Generic;
using System.Text;

namespace Saga.Contracts
{
    public interface OrderSubmitted
    {
        Guid OrderId { get; }
        DateTime TimeStamp { get; }

        string CustomerNumber { get; }
    }
}
