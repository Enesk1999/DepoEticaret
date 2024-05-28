using ETicaretWebUI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext applicationDbContext;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IAppUserRepository AppUser { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public UnitOfWork(ApplicationDbContext rr)
        {
            applicationDbContext = rr;
            Category = new CategoryRepository(applicationDbContext);
            Product = new ProductRepository(applicationDbContext);
            Company = new CompanyRepository(applicationDbContext);
            ShoppingCart = new ShoppingCartRepository(applicationDbContext);
            AppUser = new AppUserRepository(applicationDbContext);
            OrderHeader = new OrderHeaderRepository(applicationDbContext);
            OrderDetail = new OrderDetailRepository(applicationDbContext);
        }

        public void Save()
        {
            applicationDbContext.SaveChanges();
        }
    }
}
