using ETicaret.Data.Repository;
using ETicaret.Model;
using ETicaret.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ETicaretWebUI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unit)
        {
            _logger = logger;
            unitOfWork = unit;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> urunListe = unitOfWork.Product.GetAll(includeProperties:"Category");
            return View(urunListe);
        }
        public IActionResult Details(int sepetId)
        {
            ShoppingCart sepet = new()
            {
                Product = unitOfWork.Product.Get(u => u.Id == sepetId , includeProperties:"Category"),      //Ürün bilgileri
                Count = 1,                  //Müþteri 1 ürünün detailsini seçtiðinde otomatik olarak 1 tane seçili olarak gelsin
                ProductId = sepetId             //Sepet deki ürünün Id bilgileri(kimlik)

            };
            return View(sepet);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.AppUserId = userId;

            ShoppingCart cartVeriTabaný = unitOfWork.ShoppingCart.Get(u =>u.AppUserId ==shoppingCart.AppUserId && u.ProductId == shoppingCart.ProductId);   //kullanýcý kimlik kontrolü ve ürün Id kontrolü
            if(cartVeriTabaný != null)  //sepet de ürün var ise
            {
                cartVeriTabaný.Count += shoppingCart.Count;
                unitOfWork.ShoppingCart.Update(cartVeriTabaný);
            }
            else
            {
                unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            
            unitOfWork.Save();
            TempData["basarili"] = "Sepet Baþarýyla Güncellendi";
            return RedirectToAction(nameof(Index));
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
