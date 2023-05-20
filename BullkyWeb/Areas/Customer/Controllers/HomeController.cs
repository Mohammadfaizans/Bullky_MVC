using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categorybd;

        public HomeController(ILogger<HomeController> logger, IProductRepository db, ICategoryRepository categorydb)
        {
            _logger = logger;
            _productRepo = db;
            _categorybd = categorydb;
        }

        public IActionResult Index()
        {
            var prductsList = _productRepo.GetAll(includeProperties: "Category").ToList();
            return View(prductsList);
        }

        public IActionResult Details(int productId)
        {
            var prduct = _productRepo.Get(u=>u.Id == productId , includeProperties: "Category");
            return View(prduct);
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