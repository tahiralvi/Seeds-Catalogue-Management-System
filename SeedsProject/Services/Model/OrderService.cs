using Microsoft.Extensions.Options;
using SeedsProject.Models;
using SeedsProject.Services.Interface;
using System.Data.SqlClient;

namespace SeedsProject.Services.Model
{
    public class OrderService : IOrderService
    {
        private readonly string _connectionString;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOptions<DatabaseSettings> databaseSettings, ILogger<OrderService> logger)
        {
            _logger = logger;
            _connectionString = databaseSettings.Value.DefaultConnection;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException(nameof(_connectionString), "Connection string not found.");
            }
        }

        public async Task<int> CreateOrderAsync(Order order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert Order Header
                        var orderQuery = @"
                            INSERT INTO Orders (UserId, OrderDate, TotalAmount, OrderStatus)
                            OUTPUT INSERTED.Id
                            VALUES (@UserId, @OrderDate, @TotalAmount, @OrderStatus)";

                        int orderId;
                        using (var cmd = new SqlCommand(orderQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@UserId", order.UserId);
                            cmd.Parameters.AddWithValue("@OrderDate", DateTime.UtcNow);
                            cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                            cmd.Parameters.AddWithValue("@OrderStatus", "Pending");
                            orderId = (int)await cmd.ExecuteScalarAsync();
                        }

                        // 2. Insert Order Items and Update Stock
                        foreach (var item in order.OrderItems)
                        {
                            // Check and Update Stock
                            var stockQuery = "UPDATE Seeds SET Stock = Stock - @Qty WHERE Id = @SeedId AND Stock >= @Qty";
                            using (var stockCmd = new SqlCommand(stockQuery, connection, transaction))
                            {
                                stockCmd.Parameters.AddWithValue("@Qty", item.Quantity);
                                stockCmd.Parameters.AddWithValue("@SeedId", item.SeedId);

                                int rowsAffected = await stockCmd.ExecuteNonQueryAsync();
                                if (rowsAffected == 0)
                                    throw new Exception($"Insufficient stock for Seed ID {item.SeedId}");
                            }

                            // Insert Item
                            var itemQuery = @"
                                INSERT INTO OrderItems (OrderId, SeedId, PriceAtPurchase, Quantity)
                                VALUES (@OrderId, @SeedId, @Price, @Qty)";
                            using (var itemCmd = new SqlCommand(itemQuery, connection, transaction))
                            {
                                itemCmd.Parameters.AddWithValue("@OrderId", orderId);
                                itemCmd.Parameters.AddWithValue("@SeedId", item.SeedId);
                                itemCmd.Parameters.AddWithValue("@Price", item.PriceAtPurchase);
                                itemCmd.Parameters.AddWithValue("@Qty", item.Quantity);
                                await itemCmd.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();
                        return orderId;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Transaction failed during order creation.");
                        throw;
                    }
                }
            }
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            Order order = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT o.*, oi.Id as ItemId, oi.SeedId, oi.Quantity, oi.PriceAtPurchase, s.Name as SeedName
                    FROM Orders o
                    LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
                    LEFT JOIN Seeds s ON oi.SeedId = s.Id
                    WHERE o.Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (order == null)
                            {
                                order = MapReaderToOrder(reader);
                                order.OrderItems = new List<OrderItem>();
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("ItemId")))
                            {
                                order.OrderItems.Add(new OrderItem
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ItemId")),
                                    SeedId = reader.GetInt32(reader.GetOrdinal("SeedId")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    PriceAtPurchase = reader.GetDecimal(reader.GetOrdinal("PriceAtPurchase")),
                                    Seed = new Seed { Name = reader.GetString(reader.GetOrdinal("SeedName")) }
                                });
                            }
                        }
                    }
                }
            }
            return order;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = new List<Order>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT * FROM Orders WHERE UserId = @UserId ORDER BY OrderDate DESC", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            orders.Add(MapReaderToOrder(reader));
                        }
                    }
                }
            }
            return orders;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE Orders SET OrderStatus = @Status WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@Id", orderId);
                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = new List<Order>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT * FROM Orders ORDER BY OrderDate DESC", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        orders.Add(MapReaderToOrder(reader));
                    }
                }
            }
            return orders;
        }

        private Order MapReaderToOrder(SqlDataReader reader) => new Order
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            UserId = reader.GetString(reader.GetOrdinal("UserId")),
            OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
            TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
            OrderStatus = reader.GetString(reader.GetOrdinal("OrderStatus"))
        };
    }
}
