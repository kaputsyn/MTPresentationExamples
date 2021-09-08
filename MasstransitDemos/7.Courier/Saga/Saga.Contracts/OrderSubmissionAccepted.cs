using System;
using System.Collections.Generic;
using System.Text;

namespace Saga.Contracts
{
    public interface OrderSubmissionAccepted
    {
        Guid OrderId { get; }
        DateTime TimeStamp { get; }

        string CustomerNumber { get; }
    }
}
