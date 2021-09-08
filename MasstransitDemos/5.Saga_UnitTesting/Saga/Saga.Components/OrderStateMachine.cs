using Automatonymous;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using MassTransit.RedisIntegration;
using MassTransit.Saga;
using Saga.Contracts;
using System;

namespace Saga.Components
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {

            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));

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

            During(Submitted, Ignore(OrderSubmitted),When(AccountClosed).TransitionTo(Canceled));

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
        public Event<OrderSubmitted> OrderSubmitted { get; private set; }
        public Event<CheckOrder> OrderStatusRequested { get; private set; }

        public Event<CustomerAccountClosed> AccountClosed { get; private set; }
    }

    public class OrderStateMachineDefinition : SagaDefinition<OrderState> 
    {
        public OrderStateMachineDefinition()
        {
            ConcurrentMessageLimit = 4;
        }
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
        {
          //  endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 5000, 10000));
          //  endpointConfigurator.UseInMemoryOutbox();
        }
    }

    public class OrderState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }

        public string CurrentState { get; set; }

        public string CustomerNumber { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? SubmitDate { get; set; }
    }
}
