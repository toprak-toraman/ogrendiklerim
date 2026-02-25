using System.Diagnostics;
using ilkwebsitesi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ilkwebsitesi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ilkwebsitesiDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(ILogger<HomeController> logger, ilkwebsitesiDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        // --- HARCAMA (EXPENSE) BÖLÜMÜ ---

        [Authorize]
        public IActionResult Expenses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userExpenses = _context.Expenses.Where(x => x.UserId == userId).ToList();

            var totaExpenses = userExpenses.Sum(x => x.Value);
            ViewBag.Expenses = totaExpenses.ToString("N2");

            return View(userExpenses);
        }

        [Authorize]
        public IActionResult CreateEditExpense(int? id)
        {
            if (id != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var expenseInDb = _context.Expenses.SingleOrDefault(x => x.Id == id && x.UserId == userId);

                if (expenseInDb == null) return NotFound();

                return View(expenseInDb);
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateEditExpenseForm(Expense model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId = userId;

            var expenseInId = _context.Expenses.SingleOrDefault(x => x.Id == model.Id);

            if (expenseInId != null)
            {
                if (expenseInId.UserId == userId)
                {
                    _context.Entry(expenseInId).CurrentValues.SetValues(model);
                }
            }
            else
            {
                _context.Expenses.Add(model);
            }

            _context.SaveChanges();
            return RedirectToAction("Expenses");
        }

        [Authorize]
        public IActionResult ItemDelete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var expenseInDb = _context.Expenses.SingleOrDefault(x => x.Id == id && x.UserId == userId);

            if (expenseInDb != null)
            {
                _context.Expenses.Remove(expenseInDb);
                _context.SaveChanges();
            }

            return RedirectToAction("Expenses");
        }

        // --- IDENTITY (KAYIT/GÝRÝÞ) BÖLÜMÜ ---

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model, string password)
        {
            var result = await _userManager.CreateAsync(model, password);
            if (result.Succeeded)
            {
                //  Kayýt baþarýlýysa kullanýcýyý direkt Giriþ sayfasýna yönlendiriyoruz.
                return RedirectToAction("Login");
            }

            // Hata varsa kullanýcýya nedenini göstermek için model hatalarýný ekliyoruz.
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Hata = "Kullanýcý adý veya þifre yanlýþ!";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}