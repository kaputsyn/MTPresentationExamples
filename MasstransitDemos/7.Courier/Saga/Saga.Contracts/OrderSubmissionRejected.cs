using System;
using System.Collections.Generic;
using System.Text;

namespace Saga.Contracts
{
    public interface OrderSubmissionRejected
    {
        Guid OrderId { get; }
        DateTime TimeStamp { get; }

        string CustomerNumber { get; }

        string Reason { get; }
    }
}
