using System;
using System.Collections.Generic;
using System.Text;

namespace Saga.Contracts
{
    public interface OrderCanceled
    {
        Guid OrderId { get; }
        DateTime TimeStamp { get; }
    }
}
