using System.Collections.Generic;
using System.Linq;

namespace Orcus.DataAccess.Testing.RepositoryPattern
{
    public static class CustomerRepository
    {
        public static IEnumerable<Customers> CustomersByCompany(this IRepository<Customers> repository, string companyName)
        {
            return repository.Get(f => f.ContactName.Contains(companyName)).AsEnumerable();
        }
    }
}
