using OrgkSetra.Models;
using Orgksetra.ViewModel;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using OrgkSetra.Data;

namespace OrgkSetra.Repository
{
    public class ManagerCartItem : ICartItemRepository
    {
        private readonly CartDbContext _cartdb;
        public ManagerCartItem(CartDbContext cartdb) 
        {
           _cartdb = cartdb;
        }    
        public List<CartItem>? GetCartItemListByCustomerId(int customerId)
        {
            try
            {
                List<CartItem>? cartItems = null;

                //  int customerId = Convert.ToInt32(HttpContext.Session.GetString("CustomerId"));  //Here we are assuming that customerId will not be a zero or null
                Cart_Session? Cust_Session = (from session in _cartdb.Cart_Session where session.CustomerId == customerId select session).FirstOrDefault();
                if (Cust_Session != null)
                {
                    //  TempData["SessionId"] = Cust_Session.SessionId;
                    CartItem cartItem = new CartItem();
                    //    ViewBag.Total = Cust_Session.Total;
                    cartItems = _cartdb.CartItems.Where(c => c.SessionId == Cust_Session.SessionId).ToList();

                }
                return cartItems;
            }
            catch (Exception)
            {

                throw;
            }
          
            
        }
    }
}
