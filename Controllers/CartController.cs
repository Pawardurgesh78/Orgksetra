using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orgksetra.ViewModel;
using OrgkSetra.Data;
using OrgkSetra.Models;
using System.Transactions;
using System.Web.Http;

namespace OrgkSetra.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApiService _apiService;
        private readonly CartDbContext _cartdb;

        // CartController Constructor
        public CartController(ApplicationDbContext context, ApiService apiservice, CartDbContext cartDb)
        {
            _apiService = apiservice;
            _context = context;
            _cartdb = cartDb;
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
        public async Task<ActionResult> Add_Cart(int id)
        {
            try
            {
                CartItem cartItem = new CartItem();
                using (TransactionScope Ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    int customerId = Convert.ToInt32(HttpContext.Session.GetString("CustomerId"));  //Here we are assuming that customerId will not be a zero or null
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
                    ViewBag.Total = Cust_Session.Total;
                    cartItem.ItemId = id;
                    cartItem.OrderStatus = OrderStatus.OrderPending;
                    cartItem.CreatedAt = DateTime.Now;
                    cartItem.ModifiedAt = DateTime.Now;
                    await _cartdb.CartItems.AddAsync(cartItem);
                    await _cartdb.SaveChangesAsync();

                    Ts.Complete();
                }

                //TO display cartItem values in cart
                IEnumerable<CartItem>? cartItems = _cartdb.CartItems.Where(c => c.SessionId == cartItem.SessionId).ToList();
                foreach (var item in cartItems)
                {
                    item.ItemDetails = await _apiService.GetItemDetails(item.ItemId);
                }

                TempData["SessionId"] = cartItem.SessionId;
                return View(cartItems);
            }
            catch (Exception ex)
            {
                string Message = ex.Message.ToString();
            }
            return NotFound();
        }

        public async Task<ActionResult> View_Cart()
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
            return View("Add_Cart", cartItems); ;
        }
        public async Task<ActionResult> Delete_Item(int? id)
        {

            try
            {
                using (TransactionScope Ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
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
                                cartItem.Session.Total -= cartItem.ItemDetails.ItemPrice;
                            _cartdb.CartItems.Remove(cartItem);
                        }

                    }
                    await _cartdb.SaveChangesAsync();

                    Ts.Complete();
                }
            }
            catch (Exception ex)
            {
                string Message = ex.Message.ToString();
                return NotFound();
            }
            return RedirectToAction("View_Cart");
        }










    }
}
