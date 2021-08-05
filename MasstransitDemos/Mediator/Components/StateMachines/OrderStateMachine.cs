using Automatonymous;
using System;
using System.Collections.Generic;
using System.Text;

namespace Components.StateMachines
{
    public class OrderStateMachine 
        : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {

        }
    }


    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }
    }
}
