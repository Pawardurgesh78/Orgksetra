using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.Extensions.Caching.Memory;
using OrgkSetra.Models;

namespace OrgkSetra.Controllers
{
    public class Cust_ItemController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMemoryCache memoryCache;
        public Cust_ItemController(ApiService apiService, IMemoryCache cache)
        { 
            _apiService = apiService;
            memoryCache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        public async Task<IActionResult> Index()
        {
          
            return View();
        }
        public async Task<IActionResult> GetItems()
        {
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("Password")))
                {
                    var cacheKey = "ShowAllItems";
                    
                    if(!this.memoryCache.TryGetValue<IEnumerable<Item>>(cacheKey,out IEnumerable<Item>? it))
                    {
                        var itemList = await _apiService.GetItemListAsync();
                        it = itemList.ToList();
                        //// Set cache options.
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            //// Keep in cache for this time, reset time if accessed.
                            .SetSlidingExpiration(TimeSpan.FromDays(1));
                        this.memoryCache.Set(cacheKey, it, cacheEntryOptions);
                        return View("GetItem", itemList);
                    }
                    return View("GetItem", it);
                }
                else
                {
                    return View("Index");
                }
            }
            catch (Exception ex) { string msg = ex.Message.ToString(); return View("index"); }
        }
        public async Task<IActionResult> Cust_Login()
        {
            try
            {
               
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("Password")))
                {
                    var itemList = await _apiService.GetItemListAsync();
                    if (itemList != null)
                    {
                        TempData["Success"] = "Login Successful";
                        return View("GetItem", itemList);
                    }
                    TempData["Error"] = "Error";
                    return View("Index");
                }
                else
                {
                    return View("Index");
                }
            }
            catch (Exception ex) { string msg = ex.Message.ToString(); return View("index"); }
        }
       
    }
}
