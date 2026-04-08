using Microsoft.AspNetCore.Mvc;
using SeedsProject.DTOs;
using SeedsProject.Models;
using SeedsProject.Services.Interface;
using System.Security.Claims;

namespace SeedsProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private readonly ISeedService _seedService;

        public OrderController(
            ILogger<OrderController> logger,
            IOrderService orderService,
            ISeedService seedService)
        {
            _logger = logger;
            _orderService = orderService;
            _seedService = seedService;
        }

        // POST: api/Order/Checkout
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            if (checkoutDto == null || !checkoutDto.Items.Any())
                return BadRequest("Cart is empty.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
                    var seed = await _seedService.GetSeedByIdAsync(item.SeedId);

                    if (seed == null)
                        return BadRequest($"Seed with ID {item.SeedId} not found.");

                    if (seed.Stock < item.Quantity)
                        return BadRequest($"Seed '{seed.Name}' is out of stock.");

                    var orderItem = new OrderItem
                    {
                        SeedId = seed.Id,
                        Quantity = item.Quantity,
                        PriceAtPurchase = seed.Price // Snapshot the current price
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += (seed.Price * item.Quantity);
                }

                order.TotalAmount = totalAmount;

                var orderId = await _orderService.CreateOrderAsync(order);

                return Ok(new { OrderId = orderId, Message = "Order placed successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing checkout for user {UserId}", userId);
                return StatusCode(500, "An error occurred while processing your order.");
            }
        }

        // GET: api/Order/MyOrders
        [HttpGet("MyOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }
    }
}