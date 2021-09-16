using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.Contracts
{
    public interface OrderSubmitted
    {
        Guid OrderId { get; }
    }
}
