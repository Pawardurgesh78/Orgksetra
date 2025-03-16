using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.Extensions.Caching.Memory;
using OrgkSetra.Models;
using Orgksetra.ViewModel;
using Microsoft.AspNetCore.Diagnostics;
using OrgkSetra.Data;

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
        public async Task<IActionResult> ItemDetails(int id)
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("Password")))
            {

                var item = await _apiService.GetItemDetails(id);
                return View("ItemDetails", item);

            }
            else
            {
                return View("Index");
            }
        }
        public async Task<IActionResult> GetItems()
        {
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("Password")))
                {
                    var cacheKey = "ShowAllItems";
                    string? CustomerName = HttpContext.Session.GetString("CustomerName");
                    ViewBag.CustomerName = CustomerName;
                    if (!this.memoryCache.TryGetValue<IEnumerable<Item>>(cacheKey, out IEnumerable<Item>? it))
                    {
                        var itemList = await _apiService.GetItemListAsync();
                        if (itemList != null)
                        {
                            it = itemList.ToList();
                            //// Set cache options.
                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                //// Keep in cache for this time, reset time if accessed.
                                .SetSlidingExpiration(TimeSpan.FromDays(1));
                            this.memoryCache.Set(cacheKey, it, cacheEntryOptions);
                            return View("GetItem", itemList);
                        }
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
                        string? CustomerName = HttpContext.Session.GetString("CustomerName");
                        ViewBag.CustomerName = CustomerName;
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
        public async Task<ActionResult> Cart(int id)
        {
            try
            {

                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("Password")))
                {
                    ItemDetails? itemDetails = await _apiService.GetItemDetails(id);
                    return View("Cart", itemDetails);
                  
                }
                return RedirectToAction("GetItems");
            }
            catch (Exception ex) { string msg = ex.Message.ToString(); return View("Error"); }
        }
    }
}   