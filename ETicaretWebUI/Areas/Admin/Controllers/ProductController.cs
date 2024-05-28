using ETicaret.Data.Repository;
using ETicaret.Model;
using ETicaret.Model.Models;
using ETicaret.Model.ViewModels;
using ETicaret.Utility;
using ETicaretWebUI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ETicaretWebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(IUnitOfWork unit,IWebHostEnvironment webHostEnvironment)
        {
            unitOfWork = unit;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> getirTumproductler = unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(getirTumproductler);
        }

        [HttpGet]
        public IActionResult Create()
        {
            //ViewBag ile çağırma
            //IEnumerable<SelectListItem> kategoriListesi = unitOfWork.Category.GetAll().ToList().Select(x => new SelectListItem
            //{
            //    Text = x.Name,
            //    Value = x.Id.ToString()
            //});
            //ViewBag.CategoryListe = kategoriListesi;

            ProductViewModel viewModel = new()
            {
                CategoryList = unitOfWork.Category.GetAll().ToList().Select(x=> new SelectListItem { Text=x.Name , Value =x.Id.ToString()}),
                Product = new Product()
            };


            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Create(ProductViewModel productViewModel,IFormFile FileImageUrl)
        {
            if (ModelState.IsValid)
            {

                string wwwRootPath = webHostEnvironment.WebRootPath;
                if(FileImageUrl != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(FileImageUrl.FileName);
                    string urunYol = Path.Combine(wwwRootPath, @"images/product");
                    using (var fileStream = new FileStream(Path.Combine(urunYol, fileName), FileMode.Create))
                    {
                        FileImageUrl.CopyTo(fileStream);
                    }
                    productViewModel.Product.ImageUrl = @"/images/product/" + fileName;
                }


                unitOfWork.Product.Add(productViewModel.Product);
                unitOfWork.Save();
                TempData["basarili"] = productViewModel.Product.Title + " " + "Başarılı bir şekilde eklendi";
                return RedirectToAction("Index");
            }
            else
            {
                productViewModel.CategoryList = unitOfWork.Category.GetAll().ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

                return View(productViewModel);
            }

            
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            ProductViewModel viewModel = new()
            {
                CategoryList = unitOfWork.Category.GetAll().ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }),
                Product = unitOfWork.Product.Get(x => x.Id == id)
            };
        
            
            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Edit(ProductViewModel productViewModel,IFormFile? FileImageUrl)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;
                if (FileImageUrl != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(FileImageUrl.FileName);
                    string urunYol = Path.Combine(wwwRootPath, @"images/product");



                    if (!string.IsNullOrEmpty(productViewModel.Product.ImageUrl))       //Eski resim kaldırma işlemi bu satırda kontrol ediyor resim var mı yok mu
                    {
                        var eskiResim = Path.Combine(wwwRootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));   //Resmin uzantısını buluyor
                        if (System.IO.File.Exists(eskiResim))                                                               //Var olan resmi seçiyor
                        {
                            System.IO.File.Delete(eskiResim);                                                                   //Resmi kaldırıyor
                        }
                    }


                    using (var fileStream = new FileStream(Path.Combine(urunYol, fileName), FileMode.Create))
                    {
                        FileImageUrl.CopyTo(fileStream);
                    }
                    productViewModel.Product.ImageUrl = @"/images/product/" + fileName;
                }
                unitOfWork.Product.Update(productViewModel.Product);
                unitOfWork.Save();
                TempData["basarili"] = productViewModel.Product.Title + " " + "Başarılı bir şekilde güncellendi";

                return RedirectToAction("Index");

            }
            else
            {
                productViewModel.CategoryList = unitOfWork.Category.GetAll().ToList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                return View(productViewModel);
            }
             
        }
             
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? getirSilinecekId = unitOfWork.Product.Get(x => x.Id == id);
            if (getirSilinecekId == null)
            {
                return NotFound();
            }
            return View(getirSilinecekId);


        }
        [HttpPost]
        public IActionResult Delete(Product product)
        {
            if (product != null)
            {
                unitOfWork.Product.Remove(product);
                unitOfWork.Save();
                TempData["basarili"] = "Başarılı bir şekilde Kaldırıldı";
                return RedirectToAction("Index");

            }
            return View(product);
        }

        //API İŞLEMLERİ İÇİN JSON FORMAT RETURN
        #region API CALLS
        public IActionResult GetAll(int? id)
        {
            List<Product> TumUrunleriGetirFormatJson = unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data =TumUrunleriGetirFormatJson});
        }
        [HttpDelete]
        public IActionResult DeleteDatableProduct(int? id)
        {
            var SilinecekIdGet = unitOfWork.Product.Get(x => x.Id == id);
            if(SilinecekIdGet == null)
            {
                return Json(new { success = false, message = "Hata!! silinmiyor site sahibiyle iletişime geç!!" });
                

            }
            
            var eskiResim = Path.Combine(webHostEnvironment.WebRootPath, SilinecekIdGet.ImageUrl.TrimStart('\\'));   //Resmin uzantısını buluyor
            if (System.IO.File.Exists(eskiResim))                                                               //Var olan resmi seçiyor
            {
                System.IO.File.Delete(eskiResim);                                                                   //Resmi kaldırıyor
            }
            unitOfWork.Product.Remove(SilinecekIdGet);
            unitOfWork.Save();
            return Json(new { success = true, message = "Ürün Başarıyla silindi" });
        }
        #endregion
    }
}
