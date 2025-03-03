//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orgksetra.ViewModel;
using OrgkSetra.Data;
using OrgkSetra.Models;
using OrgkSetra.Repository;
using System.Security.Principal;
using System.Transactions;
//using System.Web.Http;

namespace OrgkSetra.Controllers
{
    //  [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApiService _apiService;
        private readonly CartDbContext _cartdb;
        private readonly ManagerCartItem _manageCartItem;
        protected static int deliveryId;
        // CartController Constructor
        public CartController(ApplicationDbContext context, ApiService apiservice, CartDbContext cartDb, ManagerCartItem manageCartItem)
        {
            _apiService = apiservice;
            _context = context;
            _cartdb = cartDb;
            _manageCartItem = manageCartItem;
        }

        // GET: CartController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CartController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CartController/Create
        public async Task<ActionResult> Add_To_Cart(int id)
        {
            try
            {
                CartItem cartItem = new CartItem();
                using (TransactionScope Ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    int customerId = Convert.ToInt32(HttpContext.Session.GetString("CustomerId"));  //Here we are assuming that customerId will not be a zero or null
                    if (customerId > 0)
                    {
                        Cart_Session? Cust_Session = (from session in _cartdb.Cart_Session where session.CustomerId == customerId select session).FirstOrDefault();
                        ItemDetails? itemAdded = await _apiService.GetItemDetails(id);

                        if (Cust_Session == null)
                        {
                            Cust_Session = new Cart_Session { CustomerId = customerId, CreateAt = DateTime.Now, ModifiedAt = DateTime.Now, Total = itemAdded.ItemPrice };
                            await _cartdb.Cart_Session.AddAsync(Cust_Session);
                            await _cartdb.SaveChangesAsync();
                            cartItem.SessionId = Cust_Session.SessionId;
                        }
                        else
                        {

                            cartItem.SessionId = Cust_Session.SessionId;
                            Cust_Session.Total += itemAdded.ItemPrice;
                        }
                        var ItemPresent = _cartdb.CartItems.Where(item => item.ItemId == id && item.SessionId == Cust_Session.SessionId);
                        if (!ItemPresent.Any())
                        {
                            ViewBag.Total = Cust_Session.Total;
                            cartItem.ItemId = id;
                            cartItem.CreatedAt = DateTime.Now;
                            cartItem.ModifiedAt = DateTime.Now;
                            await _cartdb.CartItems.AddAsync(cartItem);
                            await _cartdb.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                    Ts.Complete();
                }

                //TO display cartItem values in cart    
                IEnumerable<CartItem>? cartItems = _cartdb.CartItems.Where(c => c.SessionId == cartItem.SessionId).ToList();
                foreach (var item in cartItems)
                {
                    item.ItemDetails =  await _apiService.GetItemDetails(item.ItemId);
                }

                TempData["SessionId"] = cartItem.SessionId;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string Message = ex.Message.ToString();
            }
            return NotFound();
        }

        public async Task<ActionResult> Add_Cart()
        {
            IEnumerable<ItemDetails>? itemDetails = null;
            IEnumerable<CartItem>? cartItems = null;
            //TO display cartItem values in cart
            using (TransactionScope Ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int customerId = Convert.ToInt32(HttpContext.Session.GetString("CustomerId"));  //Here we are assuming that customerId will not be a zero or null
                Cart_Session? Cust_Session = (from session in _cartdb.Cart_Session where session.CustomerId == customerId select session).FirstOrDefault();
                if (Cust_Session != null)
                {
                    TempData["SessionId"] = Cust_Session.SessionId;
                    CartItem cartItem = new CartItem();
                    ViewBag.Total = Cust_Session.Total;
                    cartItems = _cartdb.CartItems.Where(c => c.SessionId == Cust_Session.SessionId).ToList();
                    foreach (var item in cartItems)
                    {
                        item.ItemDetails = await _apiService.GetItemDetails(item.ItemId);
                    }
                }
            }
            return View(cartItems); ;
        }

        public async Task<ActionResult> Delete_Item(int? id)
        {

            try
            {
                using (TransactionScope Ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    string total = string.Empty;
                    if (id != 0)
                    {
                        var cartItem = _cartdb.CartItems
                                                    .Include(c => c.Session)
                                                    .Where(a => a.CartId == id).FirstOrDefault();
                        if (cartItem != null)
                        {
                            cartItem.ItemDetails = await _apiService.GetItemDetails(cartItem.ItemId);
                            var Cart_SessionId = cartItem.SessionId;
                            if (cartItem.ItemDetails != null)
                                cartItem.Session.Total -= cartItem.ItemDetails.ItemPrice * cartItem.Quantity;
                            total = cartItem.Session.Total.ToString();
                            _cartdb.CartItems.Remove(cartItem);
                        }

                    }
                    await _cartdb.SaveChangesAsync();
                    //Getting CartItems to display updated cart item list
                    int customerId = Convert.ToInt32(HttpContext.Session.GetString("CustomerId"));
                    List<CartItem>? cartItems = _manageCartItem.GetCartItemListByCustomerId(customerId);
                    if (cartItems != null)
                    {
                        foreach (var item in cartItems)
                        {
                            item.ItemDetails = await _apiService.GetItemDetails(item.ItemId);       //Getting Item details using api
                        }
                    }
                    Ts.Complete();
                    //   return Json(new { success = true, total = total, cartItems = cartItems } );
                    return RedirectToAction("Add_Cart");                   //temporary reloading entire page
                }
            }
            catch (Exception ex)
            {
                string Message = ex.Message.ToString();
                return NotFound();
            }
        }
        public async Task<ActionResult> UpdateCartItemQty(int itemid, int itemqty)
        {
            string CartTotal = string.Empty;
            try
            {
                int customerId = Convert.ToInt32(HttpContext.Session.GetString("CustomerId"));  //Here we are assuming that customerId will not be a zero or null
                Cart_Session? Cust_Session = (from session in _cartdb.Cart_Session where session.CustomerId == customerId select session).FirstOrDefault();
                var CartItem = _cartdb.CartItems.Where(i => i.SessionId == Cust_Session.SessionId && i.ItemId == itemid).FirstOrDefault();
                CartItem.ItemDetails = await _apiService.GetItemDetails(itemid);
                //Calculating Total price
                Cust_Session.Total -= CartItem.ItemDetails.ItemPrice * Convert.ToInt32(CartItem.Quantity);
                Cust_Session.Total += CartItem.ItemDetails.ItemPrice * itemqty;
                CartTotal = Cust_Session.Total.ToString();
                if (CartItem != null)
                {
                    CartItem.Quantity = itemqty;
                    _cartdb.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                string Message = ex.Message.ToString();
                return NotFound();
            }
            return Json(new { total = CartTotal });
        }
        public ActionResult GetDeliveryAddress()
        {
            try
            {
                int customerId = Convert.ToInt32(HttpContext.Session.GetString("CustomerId"));
                var SessionId = _cartdb.Cart_Session.Where(i => i.CustomerId == customerId).Select(i => i.SessionId).FirstOrDefault();
                if (SessionId > 0)
                {
                    var deliveryAddress = _cartdb.DeliveryAddress.Where(i => i.SessionId == SessionId).ToList();
                    if (deliveryAddress != null)
                        return Json(new { success = true, deliveryAddress = deliveryAddress });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(new { success = false });
        }
        [HttpPost]
        //     public ActionResult AddDeliveryAddress(string DeliveryAddress, string Pin, string MobileNo)
        public ActionResult AddDeliveryAddress([FromBody] DeliveryAddress deliveryAddress)
        {
            try
            {
                int customerId = Convert.ToInt32(HttpContext.Session.GetString("CustomerId"));
                var SessionId = _cartdb.Cart_Session.Where(i => i.CustomerId == customerId).Select(i => i.SessionId).FirstOrDefault();
                if (SessionId > 0)
                {
                    deliveryAddress.SessionId = SessionId;
                    _cartdb.DeliveryAddress.Add(deliveryAddress);
                    _cartdb.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(new { success = false });
        }
        [HttpGet]
        public async Task<ActionResult> LoadOrderSummary() 
        {
           
            int customerId = Convert.ToInt32(HttpContext.Session.GetString("CustomerId")); 
            //Get Customer Cart session 
            Cart_Session? Cust_Session = (from session in _cartdb.Cart_Session where session.CustomerId == customerId select session).FirstOrDefault();
            var cartItems = _cartdb.CartItems.Where(c => c.SessionId == Cust_Session.SessionId).ToList();

            //Get item info in item's dto to show in order details page
            List<Items_dto> items_Dto = new List<Items_dto>();
            
            foreach (var item in cartItems)
            {
                var items = new Items_dto();
                items.CartId = item.CartId; 
                items.Quantity = item.Quantity; 
                items.ItemId = item.ItemId; 
                items.ItemDetails = await _apiService.GetItemDetails(item.ItemId);
                items.ImgSrc = "data:image/png;base64," + Convert.ToBase64String(items.ItemDetails.ImageData);
                items_Dto.Add(items);
            }

           return Json(new {success = true, items= items_Dto, total=Cust_Session.Total});
      //      return StatusCode(200, new { success = false, message = "An internal server error occurred.", items = cartItems, total = Cust_Session.Total });
        }
     
        public ActionResult OrderSummary(string id)
        {
            deliveryId = Convert.ToInt32(id);
            return View();
          
        }
        [HttpPost]
        public ActionResult PlaceOrder()
        {
            try
            {
                if (deliveryId > 0)
                {
                    int customerId = Convert.ToInt32(HttpContext.Session.GetString("CustomerId"));  //Here we are assuming that customerId will not be a zero or null
                    if (customerId > 0)
                    {
                        Cart_Session? Cust_Session = (from session in _cartdb.Cart_Session where session.CustomerId == customerId select session).FirstOrDefault();
                        Orders order = new Orders
                        {
                            CustomerId = customerId,
                            SessionId = Cust_Session.SessionId,
                            DeliveryId = deliveryId,
                            OrderStatus = OrderStatus.OrderPending,
                            CreatedAt = DateTime.Now,
                            ModifiedAt = DateTime.Now,
                            
                        };
                        _cartdb.Orders.Add(order);
                        _cartdb.SaveChanges();
                        return Json(new { success = true });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(new { success = false });
        }


    }
}
