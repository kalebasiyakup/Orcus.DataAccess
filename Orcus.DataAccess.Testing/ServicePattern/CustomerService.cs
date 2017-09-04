using Orcus.DataAccess.Testing.RepositoryPattern;
using System.Collections.Generic;

namespace Orcus.DataAccess.Testing.ServicePattern
{
    public interface ICustomerService : IService<Customers>
    {
        IEnumerable<Customers> CustomersByCompany(string companyName);
    }

    public class CustomerService : Service<Customers>, ICustomerService
    {
        private readonly IRepository<Customers> _repository;

        public CustomerService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = unitOfWork.Repository<Customers>();
        }

        public IEnumerable<Customers> CustomersByCompany(string companyName)
        {
            return _repository.CustomersByCompany(companyName);
        }
    }
}
