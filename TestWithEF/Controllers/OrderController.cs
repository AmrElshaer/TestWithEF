using Microsoft.AspNetCore.Mvc;
using TestWithEF.Entities;

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
        public IEnumerable<Order> Get()
        {
            try
            {
                return _dbContext.Orders.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

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
        public IActionResult Post([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            order.State = new DraftState();
            order.CreatedAt = DateTime.UtcNow;

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Get), new
            {
                id = order.Id
            }, order);
        }
    }
}
