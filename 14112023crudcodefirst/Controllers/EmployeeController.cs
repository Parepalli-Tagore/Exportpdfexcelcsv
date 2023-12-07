using _14112023crudcodefirst.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace _14112023crudcodefirst.Controllers
{
    
    public class EmployeeController : Controller
    {
        private readonly EmployeeDbContext _context;

        public EmployeeController(EmployeeDbContext context)
        {
            _context = context;
        }

        // GET: Employee
        [Authorize(Roles ="User,Admin")]

        public IActionResult Index()
        {
            var employees = _context.Employee.ToList();
            return View(employees);
        }

        //public IActionResult Index1()
        //{
        //    var employees = _context.Employee.ToList();
        //    return View(employees);
        //}
        //public IActionResult Index2()
        //{
        //    var employees = _context.Employee.ToList();
        //    return View(employees);
        //}
        //public IActionResult Error()
        //{
        //    return View();
        //}


        // GET: Employee/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Employee.Add(employee);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the record.");
            }

            return View(employee);
        }

        // GET: Employee/Edit
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var employee = _context.Employee.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Edit
        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Entry(employee).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the record.");
            }

            return View(employee);
        }

        // GET: Employee/Delete
         [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var employee = _context.Employee.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = _context.Employee.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employee.Remove(employee);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> login(String username, string password)
        {
            if (username != null && password != null)
            {
                var user = await _context.person.FirstOrDefaultAsync(u => u.username == username && u.password == password);
                ClaimsIdentity identity = null;
                bool isAuthenticated = false;
                if (user != null)
                {
                    identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.username),
                        new Claim(ClaimTypes.Role, user.role)
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme);
                    isAuthenticated = true;
                }
                else
                {
                    return View("Invalid Credentials");
                }
                if (isAuthenticated)
                {
                    var principal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View("Email & Password are not provided.");
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("login");
        }
        //public IActionResult login(string username, string password)
        //{
        //    var user = Authenticate(username, password);
        //    if (user == null)
        //    {
        //        ViewBag.message = "User Not Found";
        //        return View();
        //    }
        //    else
        //    {
        //        if (user.role == "Admin")
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        if (user.role == "User")
        //        {
        //            return RedirectToAction("Index1");
        //        }
        //        if (user.role == "Manager")
        //        {
        //            return RedirectToAction("Index2");
        //        }
        //        //else
        //        //{
        //        //    return RedirectToAction("Error");
        //        //}
        //    }

        //    return Unauthorized();
        //}
        //public person Authenticate(string username, string password)
        //{
        //    var user = _context.person.FirstOrDefault(u => u.username == username && u.password == password);

        //    return user;
        //}
    }

}
