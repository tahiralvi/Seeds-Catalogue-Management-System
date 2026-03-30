using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SeedsProject.DTOs;
using SeedsProject.Models;
using System.Security.Claims;

namespace SeedsProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: api/Order/Checkout
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            if (checkoutDto == null || !checkoutDto.Items.Any())
                return BadRequest("Cart is empty.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Using a transaction to ensure either the whole order is saved or nothing is
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = "Pending",
                    OrderItems = new List<OrderItem>()
                };

                decimal totalAmount = 0;

                foreach (var item in checkoutDto.Items)
                {
                    var seed = await _context.Seeds.FindAsync(item.SeedId);

                    if (seed == null || seed.Stock < item.Quantity)
                        return BadRequest($"Seed with ID {item.SeedId} is unavailable or out of stock.");

                    var orderItem = new OrderItem
                    {
                        SeedId = seed.Id,
                        Quantity = item.Quantity,
                        PriceAtPurchase = seed.Price // Snapshot the current price
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += (seed.Price * item.Quantity);

                    // Update stock levels
                    seed.Stock -= item.Quantity;
                }

                order.TotalAmount = totalAmount;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { OrderId = order.Id, Message = "Order placed successfully!" });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while processing your order.");
            }
        }

        // GET: api/Order/MyOrders
        [HttpGet("MyOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Seed)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
