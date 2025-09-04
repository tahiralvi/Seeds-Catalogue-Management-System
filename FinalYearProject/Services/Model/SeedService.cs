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

        public async Task<List<Seed>> GetAllSeedsAsync()
        {
            var seeds = new List<Seed>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT * FROM Seeds", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        seeds.Add(MapReaderToSeed(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "An error occurred while retrieving all seeds.");   
            }           

            return seeds;
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

        private static Seed MapReaderToSeed(SqlDataReader reader)
        {
            return new Seed
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                Approval = reader.GetBoolean(reader.GetOrdinal("Approval")),
                Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                Image = reader.GetString(reader.GetOrdinal("Image")),
                ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                AgentID = reader.GetInt32(reader.GetOrdinal("AgentID")),
                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID"))
            };
        }

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
