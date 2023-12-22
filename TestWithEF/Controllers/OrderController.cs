using Microsoft.AspNetCore.Mvc;
using TestWithEF.Features.Orders.Commands.CreateOrder;
using TestWithEF.Features.Orders.Queries.GetAllOrders;

namespace TestWithEF.Controllers
{
    public class OrderController : ApiControllerBase
    {
        private readonly TestDbContext _dbContext;

        public OrderController(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IReadOnlyList<GetAllOrdersDto>> Get() => await Mediator.Send(new GetAllOrdersQuery());

        [HttpPut("{id}/confirm")]
        public IActionResult Confirm(int id)
        {
            try
            {
                var order = _dbContext.Orders.Find(id);

                if (order == null)
                {
                    return NotFound();
                }

                order.State.Confirm(order);
                _dbContext.SaveChanges();

                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/process")]
        public IActionResult Process(int id)
        {
            try
            {
                var order = _dbContext.Orders.Find(id);

                if (order == null)
                {
                    return NotFound();
                }

                order.State.Process(order);
                _dbContext.SaveChanges();

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                var order = _dbContext.Orders.Find(id);

                if (order == null)
                {
                    return NotFound();
                }

                order.State.Cancel(order);
                _dbContext.SaveChanges();

                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateOrderCommand createOrderCommand)
        {
            var orderId = await Mediator.Send(createOrderCommand);

            return CreatedAtAction(nameof(Get), new
            {
                id = orderId
            }, createOrderCommand);
        }
    }
}
