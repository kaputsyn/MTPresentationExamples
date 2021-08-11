using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface OrderSubmited
    {
        Guid OrderId { get; }
        DateTime TimeStamp { get; }
        string CustomerNumber { get; }
    }
}
