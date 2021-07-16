using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleAPI.Services
{
    using Models;

    public class CompanyService : BaseService<Company>, ICompanyService
    {
        public CompanyService(IDataContext dataContext)
            : base(dataContext)
        {
        }
    }

    public interface ICompanyService : IService<Company>
    {
    }
}
