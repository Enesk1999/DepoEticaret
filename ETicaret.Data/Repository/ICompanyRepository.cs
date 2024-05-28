using ETicaret.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Data.Repository
{
    public interface ICompanyRepository:IRepository<Company>
    {
        void Update(Company company);
    }
}
