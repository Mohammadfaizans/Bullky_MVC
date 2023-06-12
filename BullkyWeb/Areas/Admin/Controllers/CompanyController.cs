using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
  //  [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller 
    {
        private  ICategoryRepository _categoryRepo;


        private ICompanyRepository _CompanyRepo;

        public CompanyController(ICompanyRepository db, ICategoryRepository categorydb)
        {
            _CompanyRepo = db;
            _categoryRepo = categorydb;

        }
        public IActionResult Index()
         {
            var company = _CompanyRepo.GetAll().ToList(); // here we feaching data from SQL and passing to view
           
            return View(company);
        }
        public IActionResult Upsert(int? id) //UpdateInsert when u creating u will not have an Id but when u update i have an Id
        {
            
            if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                Company companyObj = _CompanyRepo.Get(u => u.Id ==  id);
                return View(companyObj);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {

            if (ModelState.IsValid)
            {
                
                if(CompanyObj.Id == 0)
                {
                    _CompanyRepo.Add(CompanyObj);
                }
                else
                {
                    _CompanyRepo.Update(CompanyObj);
                }
                
                _CompanyRepo.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(CompanyObj);

            }

        }


        #region API CALLS 
        //[HttpGet("ViewActiveBooking")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var objCompanyList = _CompanyRepo.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _CompanyRepo.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, meassage = "Error while Deleteing " });
            }
            
            _CompanyRepo.Remove(CompanyToBeDeleted);
            _CompanyRepo.Save();

            return Json(new { success = true, meassage = "Delete Successfully " });
        }
        #endregion
    }
}
