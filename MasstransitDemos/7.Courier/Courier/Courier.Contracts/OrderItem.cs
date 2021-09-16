using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.Contracts
{
    public class OrderItem
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
    }
}
