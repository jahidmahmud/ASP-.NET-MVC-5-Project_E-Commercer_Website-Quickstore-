using OnlineShopProject.Models;
using OnlineShopProject.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopProject.Controllers
{
    public class SliderController : Controller
    {
        SliderRepository SR = new SliderRepository();
        Slider2Repository SR2 = new Slider2Repository();
        // GET: Slider
        public ActionResult Index()
        {
            return View(SR.GetAll());
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(SR.Get(id));
        }
        [HttpPost]
        public ActionResult Edit(SlideImage sd, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/I_SliderImg/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            sd.SlideImage1 = pic;
            SR.Update(sd);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Slider2()
        {
            return View(SR2.GetAll());
        }

        [HttpGet]
        public ActionResult Slide2Edit(int id)
        {
            return View(SR2.Get(id));
        }
        [HttpPost]
        public ActionResult Slide2Edit(SlideImage2 si2, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/I_SliderImg/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            si2.SlideImage = pic;
            SR2.Update(si2);
            return RedirectToAction("Index");
        }
    }
}