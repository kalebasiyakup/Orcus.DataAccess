# Orcus.DataAccess

Projeyi indirmeniz halinde içindeki test projesi ile istediğiniz detaylı bilgiye ulaşabilirsiniz.


# Unit Of Work İle Repository Pattern Kullanımı

```c#
var context = new NORTHWNDEntities();
using (IUnitOfWork unitOfWork = new UnitOfWork(context))
{
    unitOfWork.BeginTransaction();
    var customerRepository = unitOfWork.Repository<Customers>();
    customerRepository.Insert(new Customers
    {
        CustomerID = "125",
        CompanyName = "Deneme - CompanyName"
    });
    unitOfWork.SaveChanges();
}
```

```c#
var context = new NORTHWNDEntities();
using (IUnitOfWork unitOfWork = new UnitOfWork(context))
{
  IRepository<Customers> customerRepository = unitOfWork.Repository<Customers>();
  var retVal = customerRepository.GetFirstOrDefault();
}
```

# Unit Of Work İle Service Pattern Kullanımı 

```c#
var context = new NORTHWNDEntities();
using (IUnitOfWork unitOfWork = new UnitOfWork(context))
{
    unitOfWork.BeginTransaction();
    var customerService = new CustomerService(unitOfWork);
    customerService.Insert(new Customers
    {
        CustomerID = "125",
        CompanyName = "Deneme - CompanyName"
    });

    unitOfWork.SaveChanges();
}
```

```c#
var context = new NORTHWNDEntities();
using (IUnitOfWork unitOfWork = new UnitOfWork(context))
{
    var customerService = new CustomerService(unitOfWork);
    var result = customerService.GetFirstOrDefault();
}
```

Service Pattern İçeriği; 

```c#
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
```
