using Courier.Contracts;
using System;
using System.Collections.Generic;

namespace Courier.Components.Deliver
{
    public interface DeliverActivityArguments
    {
        string Address { get; }
        Guid OrderId { get; }

        List<OrderItem> Items { get; }
    }

    
}