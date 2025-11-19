using FinalYearProject.Models;
using FinalYearProject.Services.Interface;
using Microsoft.Extensions.Options;

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
        public Task<int> CreateAgentAsync(Agent agent)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAgentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Agent> GetAgentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Agent> GetAgentWithDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Agent>> GetAllAgentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAgentAsync(Agent agent)
        {
            throw new NotImplementedException();
        }
    }
}
