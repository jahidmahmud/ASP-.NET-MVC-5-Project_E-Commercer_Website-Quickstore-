using OnlineShopProject.Models;
using OnlineShopProject.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopProject.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        ProductRepository proRepo = new ProductRepository();
        public ActionResult Index()
        {
            return View(proRepo.GetAll());
        }

        [HttpGet]
        public ActionResult Create()
        {
            CategoryRepository catRepo = new CategoryRepository();
            ViewData["categories"] = catRepo.GetAll();
            return View();
        }
        [HttpPost]
        public ActionResult Create(Product p, HttpPostedFileBase file)
        {
           
            if (ModelState.IsValid)
            {
                string pic = null;
                if (file != null)
                {
                    pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/I_ProductImg/"), pic);
                    // file is uploaded
                    file.SaveAs(path);
                }
                var img = "~/I_ProductImg/" + pic;
                p.ProductImage = img;
                p.CreatedDate = DateTime.Now;
                proRepo.Insert(p);
                return RedirectToAction("Index");
            }
            
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            CategoryRepository catRepo = new CategoryRepository();
            ViewData["categories"] = catRepo.GetAll();
            return View(proRepo.Get(id));
        }

        [HttpPost]
        public ActionResult Edit(Product p, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/I_ProductImg/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            var img = "~/I_ProductImg/" + pic;
            p.ProductImage = file != null ? img : img;
            p.ModifiedDate = DateTime.Now;
            proRepo.Update(p);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View(proRepo.Get(id));
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            proRepo.Delete(id);
            Session["msg"] = "Deleted Successfylly!";
            return RedirectToAction("Delete");
        }
        [HttpGet]
        public ActionResult Top()
        {
            return View(proRepo.GetTopProducts(2));
        }
    }
}