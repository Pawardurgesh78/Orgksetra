using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using OrgkSetra.Models;
using Orgksetra.ViewModel;
using System.Text;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
        //Getting List of ItemDetails from AdminOrgksetra db
        public async Task<IEnumerable<ItemDetails>?> GetItemDetailsListByIds(string idList)
        {
            var json = JsonConvert.SerializeObject(idList);
            var Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7174/api/ItemApi/GetCartItems", Content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ItemDetails>?>();
        }
        public async Task<ItemDetails?> GetItemDetails(int id)
        {

            var response = await _httpClient.GetAsync("https://localhost:7174/api/ItemApi/" + id);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ItemDetails>();


        }
    }
}
