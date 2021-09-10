using Automatonymous;
using GreenPipes;
using MassTransit;
using MassTransit.RedisIntegration;
using Saga.Components.StateMachines.OrderStateMachineActivities;
using Saga.Contracts;
using System;

namespace Saga.Components
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {

            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderAccepted, x => x.CorrelateById(m => m.Message.OrderId));



            Event(() => OrderStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId != null) 
                    {
                        await context.RespondAsync<OrderNotFound>(new
                        {
                            OrderId = context.Message.OrderId
                        });
                    }
                }));
            });

            Event(() => AccountClosed, x => x.CorrelateBy((instance, context) => instance.CustomerNumber == context.Message.CustomerNumber));
            InstanceState(x => x.CurrentState);

            Initially(When(OrderSubmitted)
                .Then(context => 
                {
                    context.Instance.CustomerNumber = context.Data.CustomerNumber;
                    context.Instance.Updated = DateTime.UtcNow;
                    context.Instance.SubmitDate = context.Data.TimeStamp;
                })
                .TransitionTo(Submitted));

            During(Submitted, Ignore(OrderSubmitted),
                When(AccountClosed).TransitionTo(Canceled)
                ,When(OrderAccepted).Activity(x => x.OfType<AcceptOrderActivity>())
                .TransitionTo(Accepted)
                );

            //Not include initial and final states
            DuringAny(When(OrderSubmitted)
                .Then(context =>
            {
                context.Instance.CustomerNumber = context.Data.CustomerNumber;
                context.Instance.SubmitDate = context.Data.TimeStamp;
            }));

            DuringAny(
            When(OrderStatusRequested)
                .RespondAsync(context => context.Init<OrderStatus>(new
                {

                    OrderId = context.Instance.CorrelationId,
                    State = context.Instance.CurrentState
                })));
        }

        public State Submitted { get; private set; }
        public State Canceled { get; private set; }

        public State Accepted { get; private set; }
        public Event<OrderSubmitted> OrderSubmitted { get; private set; }
        public Event<OrderAccepted> OrderAccepted { get; private set; }
        public Event<CheckOrder> OrderStatusRequested { get; private set; }

        public Event<CustomerAccountClosed> AccountClosed { get; private set; }
    }
}
