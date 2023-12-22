namespace TestWithEF.Entities
{
    public class Order : Entity
    {
        private Order() { }

        public string Description { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? ConfirmedAt { get; private set; }

        public DateTime? ProcessedAt { get; private set; }

        public DateTime? CancelledAt { get; private set; }

        public OrderState State { get; private set; }

        public IReadOnlyCollection<OrderProduct> OrderProducts { get; private set; }

        public void UpdateState(OrderState state)
        {
            State = state;
        }

        public void UpdateConfirmedAt(DateTime confirmedAt)
        {
            ConfirmedAt = confirmedAt;
        }

        public void UpdateCanceledAt(DateTime canceledAt)
        {
            CancelledAt = canceledAt;
        }

        public void UpdateProcessedAt(DateTime processedAt)
        {
            ProcessedAt = processedAt;
        }

        public static Order Create(string description, IReadOnlyList<OrderProduct> orderProducts)
        {
            return new Order()
            {
                Id = Guid.NewGuid(),
                Description = description,
                State = new DraftState(),
                CreatedAt = DateTime.UtcNow,
                OrderProducts = orderProducts.ToList(),
            };
        }
    }

    public abstract class OrderState
    {
        public virtual void Confirm(Order order)
        {
            throw new InvalidOperationException("Cannot confirm order in current state");
        }

        public virtual void Process(Order order)
        {
            throw new InvalidOperationException("Cannot process order in current state");
        }

        public virtual void Cancel(Order order)
        {
            throw new InvalidOperationException("Cannot cancel order in current state");
        }
    }

    public class DraftState : OrderState
    {
        public override void Confirm(Order order)
        {
            order.UpdateState(new ConfirmedState());
            order.UpdateConfirmedAt(DateTime.UtcNow);
        }

        public override void Cancel(Order order)
        {
            order.UpdateState(new CancelledState());
            order.UpdateCanceledAt(DateTime.UtcNow);
        }
    }

    public class ConfirmedState : OrderState
    {
        public override void Process(Order order)
        {
            order.UpdateState(new UnderProcessingState());
            order.UpdateProcessedAt(DateTime.UtcNow);
        }

        public override void Cancel(Order order)
        {
            order.UpdateState(new CancelledState());
            order.UpdateCanceledAt(DateTime.UtcNow);
        }
    }

    public class UnderProcessingState : OrderState { }

    public class CancelledState : OrderState { }
}
