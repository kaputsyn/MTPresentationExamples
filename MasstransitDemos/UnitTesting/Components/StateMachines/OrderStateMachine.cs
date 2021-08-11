using Automatonymous;
using Contracts;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
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
            //Specify rule for events corelated with saga instance
            Event(() => OrderSubmited, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderStatusRequested, x => {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {

                    await context.RespondAsync<OrderNotFound>(new
                    {
                        OrderId = context.Message.OrderId
                    });
                }));
            });

            InstanceState(x => x.CurrentState);
            Initially(
                When(OrderSubmited)
                .Then(context => {
                    context.Instance.CustomerNumber = context.Data.CustomerNumber;
                    context.Instance.Updated = DateTime.UtcNow;
                    context.Instance.SubmitDate = context.Data.TimeStamp;


                    throw new Exception("Error occured while submiting order");
                })
                .TransitionTo(Submitted)
                );

            During(Submitted, Ignore(OrderSubmited));

            DuringAny(When(OrderStatusRequested).RespondAsync(x => x.Init<OrderStatus>(new
            {

                OrderId = x.Instance.CorrelationId,
                State = x.Instance.CurrentState
            })));

            DuringAny(When(OrderSubmited).Then(context =>
            {
                context.Instance.CustomerNumber ??= context.Data.CustomerNumber;
                context.Instance.SubmitDate ??= context.Data.TimeStamp;
            }));
        }

        public State Submitted { get; private set; }
        public Event<OrderSubmited> OrderSubmited { get; private set; }
        public Event<CheckOrder> OrderStatusRequested { get; private set; }
    }


    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CustomerNumber { get; set; }

        public DateTime? SubmitDate { get; set; }

        public DateTime? Updated { get; set; }

        public string CurrentState { get; set; }
    }

    public class OrderStateMachineDefinition : SagaDefinition<OrderState> 
    {

        public OrderStateMachineDefinition()
        {
            ConcurrentMessageLimit = 4;
        }
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 5000, 10000));
        }
    }
}
