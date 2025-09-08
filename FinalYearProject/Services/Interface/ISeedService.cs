using FinalYearProject.Models;

namespace FinalYearProject.Services.Interface
{
    public interface ISeedService
    {
        Task<List<Seed>> GetAllSeedsAsync();
        Task<List<Seed>> GetSeedsAsync(string searchTerm = null, bool? approvalStatus = null,
                                     int? minStock = null, int? maxStock = null);
        Task<Seed> GetSeedByIdAsync(int id);
        Task<Seed> GetSeedWithDetailsAsync(int id);
        Task<int> CreateSeedAsync(Seed seed);
        Task<int> UpdateSeedAsync(Seed seed);
        Task<int> DeleteSeedAsync(int id);
        Task<List<Seed>> GetSeedsByAgentAsync(int agentId);
        Task<List<Seed>> GetApprovedSeedsAsync();
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Agent>> GetAllAgentsAsync();
    }
}
