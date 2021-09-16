using System;
using System.Collections.Generic;
using System.Text;

namespace Saga.Contracts
{
    public interface OrderProcessed
    {
        Guid OrderId { get; }
        DateTime TimeStamp { get; }
    }
}
