using FinalYearProject.Models;

namespace FinalYearProject.Services.Interface
{
    public interface ISeedService
    {
        Task<List<Seed>> GetAllSeedsAsync();
        Task<Seed> GetSeedByIdAsync(int id);
        Task<int> CreateSeedAsync(Seed seed);
        Task<int> UpdateSeedAsync(Seed seed);
        Task<int> DeleteSeedAsync(int id);
        Task<List<Seed>> GetSeedsByAgentAsync(int agentId);
        Task<List<Seed>> GetApprovedSeedsAsync();
    }
}
