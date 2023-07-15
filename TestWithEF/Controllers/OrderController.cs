using Microsoft.AspNetCore.Mvc;
using TestWithEF.Entities;

namespace TestWithEF.Controllers
{
    public class OrderController : ApiControllerBase
    {
        private readonly TestContext _context;

        public OrderController(TestContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Order> Get()
        {
            try
            {
                return _context.Orders.ToList();
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
                var order = _context.Orders.Find(id);

                if (order == null)
                {
                    return NotFound();
                }

                order.State.Confirm(order);
                _context.SaveChanges();

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
                var order = _context.Orders.Find(id);

                if (order == null)
                {
                    return NotFound();
                }

                order.State.Process(order);
                _context.SaveChanges();

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
                var order = _context.Orders.Find(id);

                if (order == null)
                {
                    return NotFound();
                }

                order.State.Cancel(order);
                _context.SaveChanges();

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

            _context.Orders.Add(order);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Get), new
            {
                id = order.Id
            }, order);
        }
    }
}
