using SeedsProject.Models;

namespace SeedsProject.Services.Interface
{
    public interface IAgentService
    {
        Task<List<Agent>> GetAllAgentsAsync();

        Task<Agent> GetAgentByIdAsync(int id);

        Task<Agent> GetAgentWithDetailsAsync(int id);

        Task<int> CreateAgentAsync(Agent agent);

        Task<int> UpdateAgentAsync(Agent agent);

        Task<int> DeleteAgentAsync(int id);
    }
}