using SeedsProject.Services.Interface;
using Microsoft.Extensions.Options;
using SeedsProject.Models;
using System.Data;
using System.Data.SqlClient;

namespace SeedsProject.Services.Model
{
    public class CategoryService: ICategoryService
    {
        private readonly string _connectionString;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IOptions<DatabaseSettings> databaseSettings, ILogger<CategoryService> logger)
        {
            _connectionString = databaseSettings.Value.DefaultConnection;
            _logger = logger;   
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var categories = new List<Category>();
            const string query = "SELECT Id, Name, Description, CreatedDate FROM Categories";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            categories.Add(new Category
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            });
                        }
                    }
                }
            }
            return categories;
        }

        public async Task<int> CreateCategoryAsync(Category category)
        {
            const string query = "INSERT INTO Categories (Name, Description, CreatedDate) VALUES (@Name, @Description, @CreatedDate); SELECT SCOPE_IDENTITY();";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", category.Name);
                    cmd.Parameters.AddWithValue("@Description", category.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    await conn.OpenAsync();
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
        }

        public async Task<int> UpdateCategoryAsync(Category category)
        {
            const string query = "UPDATE Categories SET Name = @Name, Description = @Description WHERE Id = @Id";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", category.Id);
                    cmd.Parameters.AddWithValue("@Name", category.Name);
                    cmd.Parameters.AddWithValue("@Description", category.Description ?? (object)DBNull.Value);
                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteCategoryAsync(int id)
        {
            const string query = "DELETE FROM Categories WHERE Id = @Id";
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

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            const string query = "SELECT * FROM Categories WHERE Id = @Id";
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
                            return new Category
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}