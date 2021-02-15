using OnlineShopProject.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopProject.Controllers
{
    public class OrderController : Controller
    {
        OrderRepository or = new OrderRepository();
        // GET: Order
        public ActionResult Index()
        {
            return View(or.GetAll());
        }
    }
}