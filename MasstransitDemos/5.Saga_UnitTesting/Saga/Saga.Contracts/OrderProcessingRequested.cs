using System;
using System.Collections.Generic;
using System.Text;

namespace Saga.Contracts
{
    public interface OrderProcessingRequested
    {
        Guid OrderId { get; }
    }
}
