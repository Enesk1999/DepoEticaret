using ETicaret.Model.Models;
using ETicaretWebUI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Data.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext applicationDbContext;
        public ShoppingCartRepository(ApplicationDbContext rr) : base(rr)
        {
            applicationDbContext = rr;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            applicationDbContext.Sepetlerim.Update(shoppingCart);
        }
    }
}
