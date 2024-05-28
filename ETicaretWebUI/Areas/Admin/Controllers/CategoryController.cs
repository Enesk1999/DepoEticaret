using ETicaret.Data.Repository;
using ETicaret.Model;
using ETicaret.Utility;
using ETicaretWebUI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretWebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public CategoryController(IUnitOfWork unit)
        {
            unitOfWork = unit;
        }
        public IActionResult Index()
        {
            List<Category> getirTumKategoriler = unitOfWork.Category.GetAll().ToList();
            return View(getirTumKategoriler);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category kategori)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Add(kategori);
                unitOfWork.Save();
                TempData["basarili"] = kategori.Name + " " + "Başarılı bir şekilde eklendi";
                return RedirectToAction("Index");
            }

            return View(kategori);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category degistirilecekId = unitOfWork.Category.Get(x => x.Id == id);
            if (degistirilecekId == null)
            {
                return NotFound();
            }
            return View(degistirilecekId);
        }
        [HttpPost]
        public IActionResult Edit(Category kategori)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Update(kategori);
                unitOfWork.Save();
                TempData["basarili"] = kategori.Name + " " + "Başarılı bir şekilde güncellendi";

                return RedirectToAction("Index");

            }
            return View(kategori);
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? getirSilinecekId = unitOfWork.Category.Get(x => x.Id == id);
            if (getirSilinecekId == null)
            {
                return NotFound();
            }
            return View(getirSilinecekId);


        }
        [HttpPost]
        public IActionResult Delete(Category kategori)
        {
            if (kategori != null)
            {
                unitOfWork.Category.Remove(kategori);
                unitOfWork.Save();
                TempData["basarili"] = "Başarılı bir şekilde Kaldırıldı";
                return RedirectToAction("Index");

            }
            return View(kategori);
        }
    }
}
