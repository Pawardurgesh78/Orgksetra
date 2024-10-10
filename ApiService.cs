using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using OrgkSetra.Models;
namespace OrgkSetra
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<Item>?> GetItemListAsync()
        {

                var response = await _httpClient.GetAsync("https://localhost:7174/api/ItemApi");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<Item>?>();

        }
    }
}
