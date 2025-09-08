using FinalYearProject.Models;
using FinalYearProject.Services.Interface;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace FinalYearProject.Services.Model
{
    public class SeedService : ISeedService
    {
        private readonly string _connectionString;

        private readonly ILogger<SeedService> _logger;

        public SeedService(IOptions<DatabaseSettings> databaseSettings, ILogger<SeedService> logger)
        {
            _logger = logger;
            _connectionString = databaseSettings.Value.DefaultConnection;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException(nameof(_connectionString), "Connection string not found in configuration.");
            }
        }

        public async Task<List<Seed>> GetSeedsAsync(string searchTerm = null, bool? approvalStatus = null,
                                          int? minStock = null, int? maxStock = null)
        {
            var seeds = new List<Seed>();
            var query = @"
        SELECT s.*, a.Name as AgentName, c.Name as CategoryName 
        FROM Seeds s
        INNER JOIN Agents a ON s.AgentID = a.Id
        INNER JOIN Category c ON s.CategoryID = c.Id
        WHERE 1=1";

            var parameters = new List<SqlParameter>();

            // Build dynamic WHERE clause
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (s.Name LIKE @SearchTerm OR c.Name LIKE @SearchTerm)";
                parameters.Add(new SqlParameter("@SearchTerm", $"%{searchTerm}%"));
            }

            if (approvalStatus.HasValue)
            {
                query += " AND s.Approval = @ApprovalStatus";
                parameters.Add(new SqlParameter("@ApprovalStatus", approvalStatus.Value));
            }

            if (minStock.HasValue)
            {
                query += " AND s.Stock >= @MinStock";
                parameters.Add(new SqlParameter("@MinStock", minStock.Value));
            }

            if (maxStock.HasValue)
            {
                query += " AND s.Stock <= @MaxStock";
                parameters.Add(new SqlParameter("@MaxStock", maxStock.Value));
            }

            query += " ORDER BY s.Name";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var seed = MapReaderToSeed(reader);
                            // You can add AgentName and CategoryName if needed
                            seeds.Add(seed);
                        }
                    }
                }
            }

            return seeds;
        }
        public async Task<List<Seed>> GetAllSeedsAsync()
        {
            return await GetSeedsAsync();
        }

        public async Task<Seed> GetSeedByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT * FROM Seeds WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapReaderToSeed(reader);
                        }
                    }
                }
            }

            return null;
        }

        public async Task<int> CreateSeedAsync(Seed seed)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(
                    @"INSERT INTO Seeds (Name, Description, Price, Approval, Stock, Image, ExpiryDate, AgentID, CategoryID) 
                  OUTPUT INSERTED.Id 
                  VALUES (@Name, @Description, @Price, @Approval, @Stock, @Image, @ExpiryDate, @AgentID, @CategoryID)",
                    connection))
                {
                    AddSeedParameters(command, seed);

                    return (int)await command.ExecuteScalarAsync();
                }
            }
        }

        public async Task<int> UpdateSeedAsync(Seed seed)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(
                    @"UPDATE Seeds 
                    SET Name = @Name, 
                        Description = @Description, 
                        Price = @Price, 
                        Approval = @Approval, 
                        Stock = @Stock, 
                        Image = @Image, 
                        ExpiryDate = @ExpiryDate, 
                        AgentID = @AgentID, 
                        CategoryID = @CategoryID
                  WHERE Id = @Id",
                    connection))
                {
                    command.Parameters.AddWithValue("@Id", seed.Id);
                    AddSeedParameters(command, seed);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteSeedAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("DELETE FROM Seeds WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Seed>> GetSeedsByAgentAsync(int agentId)
        {
            var seeds = new List<Seed>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT * FROM Seeds WHERE AgentID = @AgentID", connection))
                {
                    command.Parameters.AddWithValue("@AgentID", agentId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            seeds.Add(MapReaderToSeed(reader));
                        }
                    }
                }
            }

            return seeds;
        }

        public async Task<List<Seed>> GetApprovedSeedsAsync()
        {
            var seeds = new List<Seed>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT * FROM Seeds WHERE Approval = 1", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        seeds.Add(MapReaderToSeed(reader));
                    }
                }
            }

            return seeds;
        }

        public async Task<Seed> GetSeedWithDetailsAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                SELECT s.*, a.Name as AgentName, a.Email as AgentEmail, a.Phone as AgentPhone,
                       c.Name as CategoryName, c.Description as CategoryDescription
                FROM Seeds s
                INNER JOIN Agents a ON s.AgentID = a.Id
                INNER JOIN Category c ON s.CategoryID = c.Id
                WHERE s.Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var seed = MapReaderToSeed(reader);

                            // Add agent details
                            seed.Agent = new Agent
                            {
                                Id = seed.AgentID,
                                Name = reader.GetString(reader.GetOrdinal("AgentName")),
                                Email = reader.IsDBNull(reader.GetOrdinal("AgentEmail")) ? null : reader.GetString(reader.GetOrdinal("AgentEmail")),
                                Phone = reader.IsDBNull(reader.GetOrdinal("AgentPhone")) ? null : reader.GetString(reader.GetOrdinal("AgentPhone"))
                            };

                            // Add category details
                            seed.Category = new Category
                            {
                                Id = seed.CategoryID,
                                Name = reader.GetString(reader.GetOrdinal("CategoryName")),
                                Description = reader.IsDBNull(reader.GetOrdinal("CategoryDescription")) ? null : reader.GetString(reader.GetOrdinal("CategoryDescription"))
                            };

                            return seed;
                        }
                    }
                }
            }

            return null;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var categories = new List<Category>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT * FROM Category ORDER BY Name", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                        });
                    }
                }
            }

            return categories;
        }

        public async Task<List<Agent>> GetAllAgentsAsync()
        {
            var agents = new List<Agent>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT * FROM Agents ORDER BY Name", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        agents.Add(new Agent
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                            Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                        });
                    }
                }
            }

            return agents;
        }
        private Seed MapReaderToSeed(SqlDataReader reader) => new Seed
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString(reader.GetOrdinal("Description")),
            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
            Approval = reader.GetBoolean(reader.GetOrdinal("Approval")),
            Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
            Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? string.Empty : reader.GetString(reader.GetOrdinal("Image")),
            ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
            AgentID = reader.GetInt32(reader.GetOrdinal("AgentID")),
            CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
            // Fix: Assign Agent and Category objects to their respective properties
            Agent =  new Agent { Name = reader.GetString(reader.GetOrdinal("AgentName")) },
            Category = new Category { Name = reader.GetString(reader.GetOrdinal("CategoryName")) }
        };
        

        private static void AddSeedParameters(SqlCommand command, Seed seed)
        {
            command.Parameters.AddWithValue("@Name", seed.Name);
            command.Parameters.AddWithValue("@Description", seed.Description);
            command.Parameters.AddWithValue("@Price", seed.Price);
            command.Parameters.AddWithValue("@Approval", seed.Approval);
            command.Parameters.AddWithValue("@Stock", seed.Stock);
            command.Parameters.AddWithValue("@Image", seed.Image ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ExpiryDate", seed.ExpiryDate);
            command.Parameters.AddWithValue("@AgentID", seed.AgentID);
            command.Parameters.AddWithValue("@CategoryID", seed.CategoryID);
        }
    }
}
