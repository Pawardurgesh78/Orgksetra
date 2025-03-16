using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol;
using OrgkSetra.Data;
using OrgkSetra.Models;

namespace OrgkSetra.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CartDbContext _context;
        

        public CustomersController(CartDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public IActionResult Det()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("Password")))
            {
                return View("Details");
            }
            return RedirectToAction("Index","/Home");
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FirstName,LastName,Email,MobileNo,Passward")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (!_context.Customers.Any<Customer>(a => a.Email == customer.Email || a.MobileNo == customer.MobileNo))
                {
                    _context.Add(customer);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Registration SuccessFul";
                    return RedirectToAction("Index", "/Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User is already Present");
                }    
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,FirstName,LastName,Email,MobileNo,Passward")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> CustomerLogin(Customer customers)
        {

                HttpContext.Session.SetString("UserId",customers.Email);
                HttpContext.Session.SetString("Password", customers.Passward);   
            var isCustomerExists = await _context.Customers.AnyAsync();
                if (isCustomerExists == true)
                {
                var UserSession = HttpContext.Session.GetString("UserId");
                var PassSession = HttpContext.Session.GetString("Password");    
                    foreach (Customer cs in _context.Customers)
                    {
                        if (UserSession == cs.Email && PassSession == cs.Passward)
                        {
                        HttpContext.Session.SetString("CustomerId", cs.CustomerId.ToString());
                        HttpContext.Session.SetString("CustomerName", cs.FirstName);
                        return RedirectToAction("Cust_Login", "/Cust_Item");
                        }
                      
                    }
                }
            TempData["Error"] = "Incorrect Credentials";
            return RedirectToAction("Index", "/Home");
        }
        public ActionResult CustomerLogout()
        {
            HttpContext.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
            HttpContext.Response.Headers.Pragma = "no-cache";
            HttpContext.Response.Headers.Expires = "0";
            var UserSession = HttpContext.Session.GetString("UserId");
            var PassSession = HttpContext.Session.GetString("Password");
            HttpContext.Session.Clear();
            return Json(new { success = true, message="Logout Successful"});
        }
       
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
       
    }
}
