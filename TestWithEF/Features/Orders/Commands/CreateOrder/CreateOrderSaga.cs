using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;

namespace TestWithEF.Features.Orders.Commands.CreateOrder;

// you should consider when to using
// orchestorater when send a command or cheogograph when publish evet
// if the order mater using orchestoar els eheo gogr
// consider ignnro, retry , undo , coordintate
// events
public record OrderCreatedEvent(Guid OrderId);

public record OrderConfirmationEmailSent(Guid OrderId);
public record OrderPaymentRequestSent(Guid OrderId);
// commands
public record SendOrderConfirmationEmail(Guid OrderId);

public record CreateOrderPaymentRequest(Guid OrderId);
public sealed class SendOrderConfirmationEmailHandler
: IHandleMessages<SendOrderConfirmationEmail>
{
    private readonly ILogger<SendOrderConfirmationEmailHandler> _logger;
    private readonly IBus _bus;

    public SendOrderConfirmationEmailHandler(ILogger<SendOrderConfirmationEmailHandler> logger,IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }
    public async Task Handle(SendOrderConfirmationEmail message)
    {
        _logger.LogInformation("Send confirmation {OrderId}",message.OrderId );

        await Task.Delay(2000);
        _logger.LogInformation("Order confirmation send {OrderId}",message.OrderId);
        await _bus.Reply(new OrderConfirmationEmailSent(message.OrderId));
    }
}
public sealed class CreateOrderPaymentRequestHandler
    : IHandleMessages<CreateOrderPaymentRequest>
{
    private readonly ILogger<CreateOrderPaymentRequestHandler> _logger;
    private readonly IBus _bus;

    public CreateOrderPaymentRequestHandler(ILogger<CreateOrderPaymentRequestHandler> logger,IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }
    public async Task Handle(CreateOrderPaymentRequest message)
    {
        
        _logger.LogInformation("Start Payment request {OrderId}",message.OrderId );
        await Task.Delay(2000);
        _logger.LogInformation("End Payment  request {OrderId}",message.OrderId);
        await _bus.Reply(new OrderPaymentRequestSent(message.OrderId));
    }
}
// saga data
public class OrderCreateSagaData : ISagaData
{
    public Guid Id { get; set; }

    public int Revision { get; set; }

    public Guid OrderId { get; set; }

    public bool WelcomeEmailSent {get; set;}

    public bool PaymentRequestSent { get; set; }
}
// Orchestrator
public class OrderCreateSaga:Saga<OrderCreateSagaData>,
    IAmInitiatedBy<OrderCreatedEvent>,
    IHandleMessages<OrderConfirmationEmailSent>,
    IHandleMessages<OrderPaymentRequestSent>
{
    private readonly IBus _bus;

    public OrderCreateSaga(IBus bus)
    {
        _bus = bus;
    }
    // if have muilt saga instance
    protected override void CorrelateMessages(ICorrelationConfig<OrderCreateSagaData> config)
    {
        config.Correlate<OrderCreatedEvent>(m=>m.OrderId
            ,s=>s.OrderId);
        config.Correlate<OrderConfirmationEmailSent>(m=>m.OrderId
            ,s=>s.OrderId);
        config.Correlate<OrderPaymentRequestSent>(m=>m.OrderId
            ,s=>s.OrderId);

    }

    public async Task Handle(OrderCreatedEvent message)
    {
        if (!IsNew)
        {
            return;
        }

        await _bus.Send(new SendOrderConfirmationEmail(message.OrderId));
    }

    public async Task Handle(OrderConfirmationEmailSent message)
    {
        Data.WelcomeEmailSent = true;
        await _bus.Send(new CreateOrderPaymentRequest(message.OrderId));
    }

    public Task Handle(OrderPaymentRequestSent message)
    {
        Data.PaymentRequestSent = true;
        MarkAsComplete();

        return Task.CompletedTask;
    }
}

