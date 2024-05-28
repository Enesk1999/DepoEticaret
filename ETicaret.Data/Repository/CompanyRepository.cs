using ETicaret.Model.Models;
using ETicaretWebUI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Data.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext applicationDbContext;
        public CompanyRepository(ApplicationDbContext rr) : base(rr)
        {
            applicationDbContext = rr;
        }

        public void Update(Company company)
        {
            applicationDbContext.Update(company);
        }
    }
}
