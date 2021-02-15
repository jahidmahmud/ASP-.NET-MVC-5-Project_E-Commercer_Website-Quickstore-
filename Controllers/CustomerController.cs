using OnlineShopProject.Models;
using OnlineShopProject.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopProject.Controllers
{
    public class CustomerController : Controller
    {
        QuickStoreDB context = new QuickStoreDB();
        ProductRepository pro = new ProductRepository();
        SliderRepository sr1 = new SliderRepository();
        Slider2Repository sr2 = new Slider2Repository();
        BlogRepository bRpo = new BlogRepository();
        ProductRepository pRepo = new ProductRepository();

        //[AllowAnonymous]
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            var s = sr1.Get(1);
            var s2 = sr2.Get(1);
            Session["slide1Title"] = s.SlideTitle;
            Session["slide1SlideImage"] = s.SlideImage1;
            Session["slide1SlideCollection"] = s.SlideCollection;
            Session["slide2Title"] = s2.SideTitle;
            Session["slide2SlideImage"] = s2.SlideImage;
            Session["slide2SlideCollection"] = s2.SlideCollection;
            // Session["role"] = "Customer";
            Session["Blog"] = bRpo.GetAll();
            
            return View(pro.GetAll().ToList());
        }

        //search
        [HttpPost]
        public ActionResult Index(string name, Product p)
        {
            //var product = context.Product.ToList().Where(e => e.ProductName.StartsWith(name));
            var product = pro.GetByName(name);
            return View(product.ToList());
        }


        [Authorize(Roles ="Customer")]
        [HttpGet]
        public ActionResult Dashboard()
        {
            List<ShippingDetails> sd = new List<ShippingDetails>();
            ShippingRepository sRepo = new ShippingRepository();
            int id = Convert.ToInt32(Session["id"]);
            sd = sRepo.GetShippingDetails(id);
            return View(sd);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Shop()
        {
            return View(pRepo.GetAll());
        }


        //[AllowAnonymous]
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        //[AllowAnonymous]
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Contact(FormCollection fc)
        {
            Contact ct = new Contact();
            ContactRepository cr = new ContactRepository();
            ct.Cname = fc["author"];
            ct.Cemail = fc["email"];
            ct.Csubject = fc["subject"];
            ct.Cmessage = fc["message"];
            cr.Insert(ct);
            ViewBag.msg = "Your mesage is sent to admin! you will be replied soon";
            return RedirectToAction("Contact");
        }

        [AllowAnonymous]
        //FAQ
        [HttpGet]
        public ActionResult Faq()
        {
            return View();
        }
        //cart
        //[Authorize]
        [Authorize(Roles = "Customer,Admin")]
        [HttpPost]
        public ActionResult AddToCart(int id)
        {
            if (Session["cart"] == null)
            {
                List<Item> cart = new List<Item>();
                var product = context.Product.Find(id);
                cart.Add(new Item
                {
                    Product = product,
                    Quantity = 1
                });
                Session["cart"] = cart;
            }
            else
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = context.Product.Find(id);
                foreach (var item in cart)
                {
                    if (item.Product.ProductId == id)
                    {
                        int prevQty = item.Quantity;
                        cart.Remove(item);
                        cart.Add(new Item()
                        {
                            Product = product,
                            Quantity = prevQty + 1
                        });
                        break;
                    }
                    else
                    {
                        cart.Add(new Item()
                        {
                            Product = product,
                            Quantity = 1
                        });
                        break;
                    }

                }
                Session["cart"] = cart;
            }


            return RedirectToAction("Index");
        }

        //[Authorize]
        [Authorize(Roles = "Customer,Admin")]
        public ActionResult RemoveFromCart(int id)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            foreach (var item in cart)
            {
                if (item.Product.ProductId == id)
                {
                    cart.Remove(item);
                    break;
                }
            }

            Session["cart"] = cart;
            return RedirectToAction("Index");
        }

        //View sorting page
        //[AllowAnonymous]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Sorting(int id)
        {
            SortingRepository sort = new SortingRepository();
            return View(sort.GetSorting(id).ToList());
        }

        [Authorize(Roles ="Customer,Admin")]
        public ActionResult Checkout()
        {
            return View();
        }

        //Checkout
        [Authorize(Roles = "Customer,Admin")]
        public ActionResult Add(int id)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            var product = context.Product.Find(id);
            foreach (var item in cart)
            {
                if (item.Product.ProductId == id)
                {
                    int prevQty = item.Quantity;
                    cart.Remove(item);
                    cart.Add(new Item()
                    {
                        Product = product,
                        Quantity = prevQty + 1
                    });
                    break;
                }

            }
            Session["cart"] = cart;
            return RedirectToAction("Checkout");
        }

        //Checkout decrese
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet]
        public ActionResult DecreaseQty(int productId)
        {
            if (Session["cart"] != null)
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = context.Product.Find(productId);
                foreach (var item in cart)
                {
                    if (item.Product.ProductId == productId)
                    {
                        int prevQty = item.Quantity;
                        if (prevQty > 0)
                        {
                            cart.Remove(item);
                            cart.Add(new Item()
                            {
                                Product = product,
                                Quantity = prevQty - 1
                            });
                        }
                        break;
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect("Checkout");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult CheckoutDetails()
        {
            return View();
        }


        //Blogging section

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Blog(int id)
        {
            return View();
        }

        //details view

        //[AllowAnonymous]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Details(int id)
        {
            Blogging result = bRpo.Get(id);
            Session["Blogging"] = result;
            return View(bRpo.Get(id));
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ProductView(int id)
        {
            Product result = pRepo.Get(id);
            Session["Product"] = result;
            return View(pRepo.Get(id));
        }

        [AllowAnonymous]
        [HttpPost]
        //newsletter
        public ActionResult Newsletter(FormCollection fc)
        {
            NewslettetRepository nrepo = new NewslettetRepository();
            Newsletter nl = new Newsletter();
            nl.NEmail = fc["email"];
            nrepo.Insert(nl);
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Payment()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Payment(FormCollection fs)
        {
            ShippingDetails sp = new ShippingDetails();
            sp.Uid = Convert.ToInt32(Session["id"]);
            sp.Adress = fs["address"];
            sp.City = fs["city"];
            sp.State = fs["state"];
            sp.Country = fs["country"];
            sp.ZipCode = fs["zip"];
            sp.CreditCardNumber = int.Parse(fs["cardno"]);
            sp.PaymentType = fs["cardProvider"];
            sp.AmountPaid = Convert.ToDecimal(Session["total"]);
            sp.PaymentDate = DateTime.Now;
            context.ShippingDetails.Add(sp);
            context.SaveChanges();
            Session["cart"] = null;

            return RedirectToAction("Success");
        }
        [AllowAnonymous]
        public ActionResult Success()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult UpdateQuanttity(int id, FormCollection fs)
        {
            int quan = Math.Abs(Convert.ToInt32(fs["quantity"]));
            List<Item> cart = (List<Item>)Session["cart"];
            var product = context.Product.Find(id);
            foreach (var item in cart)
            {
                if (item.Product.ProductId == id)
                {
                    int prevQty = item.Quantity;
                    cart.Remove(item);
                    cart.Add(new Item()
                    {
                        Product = product,
                        Quantity = quan
                    });
                    break;
                }

            }
            Session["cart"] = cart;
            return RedirectToAction("Checkout");
        }


        //Wishlist
        [Authorize(Roles ="Customer")]
        public ActionResult AddToWishlist(int id)
        {
            var usid = Convert.ToInt32(Session["id"]);
            Wishlist w = new Wishlist();
            w.uid = usid;
            w.pid = id;
            context.Wishlist.Add(w);
            context.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult MyWishlist()
        {

            var usid = Convert.ToInt32(Session["id"]);
            List<Wishlist> wish = new List<Wishlist>();
            List<Product> p = new List<Product>();
            wish = context.Wishlist.Where(e => e.uid == usid).ToList();
            foreach (var item in wish)
            {
                var x = pro.Get(item.pid);
                p.Add(x);
            }
            return View(p.Distinct());
        }
        public ActionResult AddFromWishlist(int id)
        {
            if (Session["cart"] == null)
            {
                List<Item> cart = new List<Item>();
                var product = context.Product.Find(id);
                cart.Add(new Item
                {
                    Product = product,
                    Quantity = 1
                });
                Session["cart"] = cart;
            }
            else
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = context.Product.Find(id);
                foreach (var item in cart)
                {
                    if (item.Product.ProductId == id)
                    {
                        int prevQty = item.Quantity;
                        cart.Remove(item);
                        cart.Add(new Item()
                        {
                            Product = product,
                            Quantity = prevQty + 1
                        });
                        break;
                    }
                    else
                    {
                        cart.Add(new Item()
                        {
                            Product = product,
                            Quantity = 1
                        });
                        break;
                    }

                }
                Session["cart"] = cart;
            }


            return RedirectToAction("MyWishlist");
        }
        public ActionResult RemoveFromList(int id)
        {
            WishlistRepository w = new WishlistRepository();
            w.Remove(id);
            return RedirectToAction("MyWishlist");
        }

    }
    
}