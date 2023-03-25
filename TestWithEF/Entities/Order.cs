namespace TestWithEF.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public OrderState State { get; set; }
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
            order.State = new ConfirmedState();
            order.ConfirmedAt = DateTime.UtcNow;
        }

        public override void Cancel(Order order)
        {
            order.State = new CancelledState();
            order.CancelledAt = DateTime.UtcNow;
        }
    }

    public class ConfirmedState : OrderState
    {
        public override void Process(Order order)
        {
            order.State = new UnderProcessingState();
            order.ProcessedAt = DateTime.UtcNow;
        }

        public override void Cancel(Order order)
        {
            order.State = new CancelledState();
            order.CancelledAt = DateTime.UtcNow;
        }
    }

    public class UnderProcessingState : OrderState
    {
       
    }

    public class CancelledState : OrderState
    {
       
    }
}
