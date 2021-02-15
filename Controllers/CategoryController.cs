using OnlineShopProject.Models;
using OnlineShopProject.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        CategoryRepository catRepo = new CategoryRepository();
        // GET: Category
        public ActionResult Index()
        {
            return View(catRepo.GetAll());
        }
        [HttpGet]
        public ActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddCategory(Category cat)
        {
            
            ViewBag.success = "Category Added Successfyll!";
            catRepo.Insert(cat);
            return RedirectToAction("Index");
            
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(catRepo.Get(id));
        }
        [HttpPost]
        public ActionResult Edit(Category cat)
        {
            catRepo.Update(cat);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View(catRepo.Get(id));
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            catRepo.Delete(id);
            return RedirectToAction("Delete");
        }
    }
}