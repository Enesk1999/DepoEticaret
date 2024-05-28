using ETicaret.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Data.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
        
    }
}
