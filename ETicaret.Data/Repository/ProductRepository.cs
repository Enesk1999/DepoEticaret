using ETicaret.Model.Models;
using ETicaretWebUI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Data.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext applicationDbContext;
        public ProductRepository(ApplicationDbContext rr) : base(rr)
        {
            applicationDbContext = rr;
        }

        public void Update(Product product)
        {
            applicationDbContext.Urunler.Update(product);
        }
     
    }
}
