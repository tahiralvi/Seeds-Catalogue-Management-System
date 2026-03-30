using SeedsProject.Models;

namespace SeedsProject.Services.Interface
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(Order order);

        Task<Order> GetOrderByIdAsync(int id);

        Task<List<Order>> GetOrdersByUserIdAsync(string userId);

        Task<bool> UpdateOrderStatusAsync(int orderId, string status);

        Task<List<Order>> GetAllOrdersAsync(); // For Admin dashboard
    }
}
