using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller 
    {
        private  ICategoryRepository _categoryRepo;

        private readonly IWebHostEnvironment _webHostEnvironment;

        private IProductRepository _productRepo;

        public ProductController(IProductRepository db, ICategoryRepository categorydb, IWebHostEnvironment webHostEnvironment)
        {
            _productRepo = db;
            _categoryRepo = categorydb;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
         {
            var prducts = _productRepo.GetAll(includeProperties:"Category").ToList(); // here we feaching data from SQL and passing to view
           
            return View(prducts);
        }
        public IActionResult Upsert(int? id) //UpdateInsert when u creating u will not have an Id but when u update i have an Id
        {
            ProductVM productVm = new()
            {
                CategoryList = _categoryRepo.GetAll().Select(u => new SelectListItem
                {

                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productVm);
            }
            else
            {
                //update
                productVm.Product = _productRepo.Get(u => u.Id ==  id);
                return View(productVm);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); //we are adding ramdon file and getting file extention
                    string productPath = Path.Combine(wwwRootPath, @"images\product"); // here we have path where we want to save the file

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old img
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                      
                    }
                    using (var fileStrem = new FileStream(Path.Combine(productPath, fileName), FileMode.Create)) 
                    {
                        file.CopyTo(fileStrem);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if(productVM.Product.Id == 0)
                {
                    _productRepo.Add(productVM.Product);
                }
                else
                {
                    _productRepo.Update(productVM.Product);
                }
                
                _productRepo.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();

        }


        #region API CALLS 

        [HttpGet]
        public IActionResult GetAll()
        {
            var objProductList = _productRepo.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var ProductToBeDeleted = _productRepo.Get(u => u.Id == id);
            if (ProductToBeDeleted == null)
            {
                return Json(new { success = false, meassage = "Error while Deleteing " });
            }
            //delete the old img
            var oldImagePath =  Path.Combine(_webHostEnvironment.WebRootPath, ProductToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _productRepo.Remove(ProductToBeDeleted);
            _productRepo.Save();

            return Json(new { success = true, meassage = "Delete Successfully " });
        }
        #endregion
    }
}
