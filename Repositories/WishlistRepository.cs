using OnlineShopProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShopProject.Repositories
{
    public class WishlistRepository : Repository<Wishlist>
    {
        QuickStoreDB Context = new QuickStoreDB();
        public void Remove(int id)
        {
            var x = this.Context.Wishlist.Where(e => e.pid == id).ToList();
            Context.Wishlist.RemoveRange(x);
            Context.SaveChanges();

        }
    }
}