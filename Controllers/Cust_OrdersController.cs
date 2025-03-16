using Microsoft.AspNetCore.Mvc;
using Orgksetra.ViewModel;
using OrgkSetra.Data;
using System.Security.Cryptography;

namespace OrgkSetra.Controllers
{
    public class Cust_OrdersController : Controller
    {
        private readonly CartDbContext _cartdb; 
        private readonly ApiService _apiService;
        public Cust_OrdersController(CartDbContext cartdb, ApiService apiService) 
        {
            _cartdb = cartdb;   
            _apiService = apiService;   
        }
        public IActionResult Cust_Orders()
        {
           return View();   
        }
        [HttpGet]
        public async Task<ActionResult> MyOrders() 
        {
            int customerId = Convert.ToInt32(HttpContext.Session.GetString("CustomerId"));
            //Get session id from orders table where customer id is logged in customer
            var orderedItems = from order in _cartdb.Orders
                               join item in _cartdb.CartItems
                         on order.SessionId equals item.SessionId
                               where order.CustomerId == customerId
                               orderby order.CreatedAt descending
                               select new{ order, item };

            foreach (var order in orderedItems)
            {
                order.item.ItemDetails = await _apiService.GetItemDetails(order.item.ItemId);
                //using image name for imgBase 64 string
               order.item.ItemDetails.ImageName = "data:image/png;base64," + Convert.ToBase64String(order.item.ItemDetails.ImageData);

                switch (order.order.OrderStatus)
                {
                    case 1:
                        order.order.Order_Status = "Pending";
                        break;
                    case 2:
                        order.order.Order_Status = "Shipped";
                        break;
                    case 3:
                        order.order.Order_Status = "Dispatched";
                        break;
                    case 4:
                        order.order.Order_Status = "Delivered";
                        break;


                }
            }

            return Json(new { orderedItems = orderedItems });
        }
    }
}
