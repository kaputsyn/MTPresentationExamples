using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.Contracts
{
    public interface SubmitOrder
    {
        List<OrderItem> Items { get; }
        string CustomerName { get; }
        string CustomerCard { get; }

        string Address { get; }
    }
}
