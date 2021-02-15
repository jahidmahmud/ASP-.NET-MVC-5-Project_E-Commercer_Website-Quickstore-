using OnlineShopProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShopProject.Repositories
{
    public class ProductRepository : Repository<Product>
    {
        QuickStoreDB Context = new QuickStoreDB();
        public List<Product> GetTopProducts(int top)
        {
            return this.GetAll().OrderByDescending(x => x.Price).Take(top).ToList();
        }

        //search
        public List<Product> GetByName(string name)
        {
            List<Product> pro = Context.Product.ToList().Where(e => e.ProductName.StartsWith(name) || e.Category.CategoryName.StartsWith(name)
            || e.Category.CategoryName.Contains(name) || e.ProductName.Contains(name) || e.ProductName.Equals(name)
            || e.Category.CategoryName.Equals(name) || e.Category.CategoryName.Contains(name) ||
            e.ProductName.ToLower().StartsWith(name) || e.Category.CategoryName.ToLower().StartsWith(name)
            || e.Category.CategoryName.ToLower().Contains(name) || e.ProductName.ToLower().Contains(name) || e.ProductName.ToLower().Equals(name)
            || e.Category.CategoryName.ToLower().Equals(name) || e.Category.CategoryName.ToLower().Contains(name)).ToList();
            return pro;
        }
    }
}