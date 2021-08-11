﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface CheckOrder
    {
        Guid OrderId { get; }
    }

    public interface OrderStatus 
    {
        Guid OrderId { get; }
        string State { get; }
    }
}
