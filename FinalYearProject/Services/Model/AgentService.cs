using FinalYearProject.Models;
using FinalYearProject.Services.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
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
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public Task<Agent> GetAgentWithDetailsAsync(int id)
        {
            // Note: Implementing deep joins in raw ADO.NET requires complex manual mapping 
            // of the Seeds collection. For brevity, this is often handled by a second query 
            // or a complex JOIN with a while(reader.Read()) logic.
            throw new NotImplementedException("Complex join mapping requires manual Seed collection population.");
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