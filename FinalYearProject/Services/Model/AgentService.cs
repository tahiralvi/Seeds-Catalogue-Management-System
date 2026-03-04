using FinalYearProject.Models;
using FinalYearProject.Services.Interface;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace FinalYearProject.Services.Model
{
    public class AgentService : IAgentService
    {
        private readonly string _connectionString;
        private readonly ILogger<AgentService> _logger;

        public AgentService(IOptions<DatabaseSettings> databaseSettings, ILogger<AgentService> logger)
        {
            _logger = logger;
            _connectionString = databaseSettings.Value.DefaultConnection;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException(nameof(_connectionString), "Connection string not found in configuration.");
            }
        }

        public async Task<int> CreateAgentAsync(Agent agent)
        {
            const string query = "INSERT INTO Agents (Name, Email, Phone, CreatedDate) VALUES (@Name, @Email, @Phone, @CreatedDate); SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", agent.Name);
                    cmd.Parameters.AddWithValue("@Email", agent.Email);
                    cmd.Parameters.AddWithValue("@Phone", (object)agent.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedDate", agent.CreatedDate);

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<List<Agent>> GetAllAgentsAsync()
        {
            var agents = new List<Agent>();
            const string query = "SELECT Id, Name, Email, Phone, CreatedDate FROM Agents";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            agents.Add(MapAgentFromReader(reader));
                        }
                    }
                }
            }
            return agents;
        }

        public async Task<Agent> GetAgentByIdAsync(int id)
        {
            const string query = "SELECT Id, Name, Email, Phone, CreatedDate FROM Agents WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapAgentFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task<int> UpdateAgentAsync(Agent agent)
        {
            const string query = "UPDATE Agents SET Name = @Name, Email = @Email, Phone = @Phone WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", agent.Id);
                    cmd.Parameters.AddWithValue("@Name", agent.Name);
                    cmd.Parameters.AddWithValue("@Email", agent.Email);
                    cmd.Parameters.AddWithValue("@Phone", agent.Phone);

                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteAgentAsync(int id)
        {
            const string query = "DELETE FROM Agents WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();

                    // Returns the number of rows affected (1 if successful)
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<Agent> GetAgentWithDetailsAsync(int id)
        {
            const string query = @"
                                SELECT a.Id, a.Name, a.Email, a.Phone, a.CreatedDate,
                                       s.Id AS SeedId, s.Name AS SeedName, s.AgentId
                                FROM Agents a
                                LEFT JOIN Seeds s ON a.Id = s.AgentId
                                WHERE a.Id = @Id";

            Agent agent = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // 1. Create the Agent object only once (on the first row)
                            if (agent == null)
                            {
                                agent = new Agent
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = reader["Name"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Phone = reader["Phone"]?.ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    Seeds = new List<Seed>() // Initialize the collection
                                };
                            }

                            // 2. If a Seed exists in this row, add it to the collection
                            if (reader["SeedId"] != DBNull.Value)
                            {
                                var seed = new Seed
                                {
                                    Id = Convert.ToInt32(reader["SeedId"]),
                                    Name = reader["SeedName"].ToString(),
                                    AgentID = Convert.ToInt32(reader["AgentID"])
                                };
                                agent.Seeds.Add(seed);
                            }
                        }
                    }
                }
            }
            return agent;
        }

        // Helper method to keep code DRY
        private Agent MapAgentFromReader(SqlDataReader reader)
        {
            return new Agent
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"].ToString(),
                Email = reader["Email"].ToString(),
                Phone = reader["Phone"].ToString(),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
            };
        }
    }
}