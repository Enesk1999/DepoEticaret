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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CompanyController(IUnitOfWork unit,IWebHostEnvironment webHostEnvironment)
        {
            unitOfWork = unit;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Company> getirTumCompanyler = unitOfWork.Company.GetAll().ToList();
            return View(getirTumCompanyler);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Company.Add(company);
                unitOfWork.Save();
                TempData["basarili"] = company.Name + " " + "Başarılı bir şekilde eklendi";
                return RedirectToAction("Index");
            }
            else
            {
                

                return View(company);
            }

            
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var getirFirmaId = unitOfWork.Company.Get(x => x.Id == id);
     
            return View(getirFirmaId);
        }
        [HttpPost]
        public IActionResult Edit(Company company)
        {
            if (ModelState.IsValid)
            {
                
              
                unitOfWork.Company.Update(company);
                unitOfWork.Save();
                TempData["basarili"] = company.Name + " " + "Başarılı bir şekilde güncellendi";

                return RedirectToAction("Index");

            }
            else
            {
                
                return View(company);
            }
             
        }
             
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company? getirSilinecekId = unitOfWork.Company.Get(x => x.Id == id);
            if (getirSilinecekId == null)
            {
                return NotFound();
            }
            return View(getirSilinecekId);


        }
        [HttpPost]
        public IActionResult Delete(Company Company)
        {
            if (Company != null)
            {
                unitOfWork.Company.Remove(Company);
                unitOfWork.Save();
                TempData["basarili"] = "Başarılı bir şekilde Kaldırıldı";
                return RedirectToAction("Index");

            }
            return View(Company);
        }

        //API İŞLEMLERİ İÇİN JSON FORMAT RETURN
        #region API CALLS
        public IActionResult GetAll(int? id)
        {
            List<Company> TumUrunleriGetirFormatJson = unitOfWork.Company.GetAll().ToList();
            return Json(new {data =TumUrunleriGetirFormatJson});
        }
        [HttpDelete]
        public IActionResult DeleteTableCompany(int? id)
        {
            var SilinecekIdGet = unitOfWork.Company.Get(x => x.Id == id);
            if(SilinecekIdGet == null)
            {
                return Json(new { success = false, message = "Hata!! silinmiyor site sahibiyle iletişime geç!!" });
                

            }
            unitOfWork.Company.Remove(SilinecekIdGet);
            unitOfWork.Save();
            return Json(new { success = true, message = "Firma Başarıyla silindi" });
        }
        #endregion
    }
}
