using Microsoft.AspNetCore.Mvc;
using SeedsProject.DTOs;
using SeedsProject.Models;
using SeedsProject.Services.Interface;
using SeedsProject.ViewModels;
using System.Security.Claims;
using System.Text.Json;

namespace SeedsProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Change class inheritance to Controller to enable TempData
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private readonly ISeedService _seedService;
        private const string CartCookieName = "cart";

        public OrderController(
            ILogger<OrderController> logger,
            IOrderService orderService,
            ISeedService seedService)
        {
            _logger = logger;
            _orderService = orderService;
            _seedService = seedService;
        }

        // POST: api/Order/AddToCart
        // Body: { "seedId": 1, "quantity": 2 }
        [HttpPost("AddToCart")]
        public IActionResult AddToCart([FromBody] CartItemDto item)
        {
            if (item == null || item.SeedId <= 0 || item.Quantity <= 0)
                return BadRequest("Invalid cart item.");

            // Read existing cart from cookie
            var cartJson = Request.Cookies[CartCookieName];
            List<CartItemDto> cart;
            if (string.IsNullOrEmpty(cartJson))
            {
                cart = new List<CartItemDto>();
            }
            else
            {
                try
                {
                    cart = JsonSerializer.Deserialize<List<CartItemDto>>(cartJson) ?? new List<CartItemDto>();
                }
                catch
                {
                    cart = new List<CartItemDto>();
                }
            }

            var existing = cart.FirstOrDefault(c => c.SeedId == item.SeedId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(new CartItemDto { SeedId = item.SeedId, Quantity = item.Quantity });
            }

            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                IsEssential = true
            };

            Response.Cookies.Append(CartCookieName, JsonSerializer.Serialize(cart), options);

            return Ok(new { Message = "Added to cart", CartCount = cart.Sum(c => c.Quantity) });
        }

        // GET: api/Order/Cart
        [HttpGet("Cart")]
        public IActionResult GetCart()
        {
            var cartJson = Request.Cookies[CartCookieName];
            if (string.IsNullOrEmpty(cartJson))
                return Ok(new List<CartItemDto>());

            try
            {
                var cart = JsonSerializer.Deserialize<List<CartItemDto>>(cartJson) ?? new List<CartItemDto>();
                return Ok(cart);
            }
            catch
            {
                return Ok(new List<CartItemDto>());
            }
        }

        // POST: api/Order/RemoveFromCart
        // Body: { "seedId": 1 }
        [HttpPost("RemoveFromCart")]
        public IActionResult RemoveFromCart([FromBody] CartItemDto item)
        {
            if (item == null || item.SeedId <= 0)
                return BadRequest("Invalid request.");

            var cartJson = Request.Cookies[CartCookieName];
            if (string.IsNullOrEmpty(cartJson))
                return NotFound("Cart is empty.");

            List<CartItemDto> cart;
            try
            {
                cart = JsonSerializer.Deserialize<List<CartItemDto>>(cartJson) ?? new List<CartItemDto>();
            }
            catch
            {
                return NotFound("Cart is empty.");
            }

            var existing = cart.FirstOrDefault(c => c.SeedId == item.SeedId);
            if (existing != null)
            {
                cart.Remove(existing);
                var options = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    IsEssential = true
                };
                Response.Cookies.Append(CartCookieName, JsonSerializer.Serialize(cart), options);
            }

            return Ok(new { Message = "Item removed", CartCount = cart.Sum(c => c.Quantity) });
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

                // Clear cart cookie after successful checkout
                Response.Cookies.Delete(CartCookieName);

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

        // MVC: Render cart view (reads cookie, loads seed details)
        // Route is absolute so MVC routing hits it: GET /Order/Cart
        [HttpGet("/Order/Cart")]
        public async Task<IActionResult> CartView()
        {
            var cartJson = Request.Cookies[CartCookieName];
            var itemsDto = string.IsNullOrEmpty(cartJson)
                ? new List<CartItemDto>()
                : JsonSerializer.Deserialize<List<CartItemDto>>(cartJson) ?? new List<CartItemDto>();

            var model = new CartViewModel
            {
                Items = new List<CartItemViewModel>()
            };

            foreach (var dto in itemsDto)
            {
                var seed = await _seedService.GetSeedByIdAsync(dto.SeedId);
                if (seed == null) continue;

                model.Items.Add(new CartItemViewModel
                {
                    SeedId = seed.Id,
                    Name = seed.Name,
                    Price = seed.Price,
                    Quantity = dto.Quantity,
                    Stock = seed.Stock,
                    Image = seed.Image
                });
            }

            return new ViewResult { ViewName = "/Views/Order/Cart.cshtml", ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<CartViewModel>(ViewData, model) };
        }

        // MVC: Show checkout page
        [HttpGet("/Order/Checkout")]
        public async Task<IActionResult> CheckoutView()
        {
            var cartJson = Request.Cookies[CartCookieName];
            var itemsDto = string.IsNullOrEmpty(cartJson)
                ? new List<CartItemDto>()
                : JsonSerializer.Deserialize<List<CartItemDto>>(cartJson) ?? new List<CartItemDto>();

            if (!itemsDto.Any())
            {
                TempData["ErrorMessage"] = "Cart is empty.";
                return Redirect("/Order/Cart");
            }

            var model = new CartViewModel { Items = new List<CartItemViewModel>() };

            foreach (var dto in itemsDto)
            {
                var seed = await _seedService.GetSeedByIdAsync(dto.SeedId);
                if (seed == null) continue;

                model.Items.Add(new CartItemViewModel
                {
                    SeedId = seed.Id,
                    Name = seed.Name,
                    Price = seed.Price,
                    Quantity = dto.Quantity,
                    Stock = seed.Stock,
                    Image = seed.Image
                });
            }

            return new ViewResult { ViewName = "/Views/Order/Checkout.cshtml", ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<CartViewModel>(ViewData, model) };
        }

        // MVC: Handle checkout submit (creates order, clears cookie, redirects to confirmation)
        [HttpPost("/Order/Checkout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutConfirm()
        {
            var cartJson = Request.Cookies[CartCookieName];
            var itemsDto = string.IsNullOrEmpty(cartJson)
                ? new List<CartItemDto>()
                : JsonSerializer.Deserialize<List<CartItemDto>>(cartJson) ?? new List<CartItemDto>();

            if (!itemsDto.Any())
            {
                TempData["ErrorMessage"] = "Cart is empty.";
                return Redirect("/Order/Cart");
            }

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

                foreach (var item in itemsDto)
                {
                    var seed = await _seedService.GetSeedByIdAsync(item.SeedId);

                    if (seed == null)
                    {
                        TempData["ErrorMessage"] = $"Seed with ID {item.SeedId} not found.";
                        return Redirect("/Order/Cart");
                    }

                    if (seed.Stock < item.Quantity)
                    {
                        TempData["ErrorMessage"] = $"Seed '{seed.Name}' does not have sufficient stock.";
                        return Redirect("/Order/Cart");
                    }

                    var orderItem = new OrderItem
                    {
                        SeedId = seed.Id,
                        Quantity = item.Quantity,
                        PriceAtPurchase = seed.Price
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += (seed.Price * item.Quantity);
                }

                order.TotalAmount = totalAmount;

                var orderId = await _orderService.CreateOrderAsync(order);

                // Clear cart cookie after successful checkout
                Response.Cookies.Delete(CartCookieName);

                return Redirect($"/Order/Confirmation/{orderId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing checkout for user {UserId}", userId);
                TempData["ErrorMessage"] = "An error occurred while processing your order.";
                return Redirect("/Order/Cart");
            }
        }

        // MVC: Confirmation page
        [HttpGet("/Order/Confirmation/{orderId?}")]
        public IActionResult Confirmation(int? orderId)
        {
            ViewData["OrderId"] = orderId;
            return new ViewResult { ViewName = "/Views/Order/Confirmation.cshtml", ViewData = ViewData };
        }
    }
}