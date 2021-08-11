﻿using System;

namespace Contracts
{
    public interface OrderSubmissionAccepted 
    {
        Guid OrderId { get; }

        DateTime TimeStamp { get; }
        string CustomerNumber { get; }

    }
}
