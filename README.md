# Orcus.DataAccess
RepositoryPattern - UnitOfWork - ServicePattern - SqlDataOperation

# Unit Of Work Üzerinde Repository Pattern Kullanımı

```c#
var context = new NORTHWNDEntities();
using (IUnitOfWork unitOfWork = new UnitOfWork(context))
{
    unitOfWork.BeginTransaction();
    var customerRepository = unitOfWork.Repository<Customers>();
    customerRepository.Insert(new Customers
    {
        CustomerID = "YAMI1",
        CompanyName = "Deneme - CompanyName"
    });
    unitOfWork.SaveChanges();
    unitOfWork.RollbackTransaction();

    unitOfWork.Dispose();
    var isDisposed = (bool)GetInstanceField(typeof(UnitOfWork), unitOfWork, "_disposed");
    Assert.IsTrue(isDisposed);

    unitOfWork.Dispose();
    context.Dispose();

    context.Dispose();
    unitOfWork.Dispose();
}
```

```c#
var context = new NORTHWNDEntities();
using (IUnitOfWork unitOfWork = new UnitOfWork(context))
{
  IRepository<Customers> customerRepository = unitOfWork.Repository<Customers>();
  var retVal = customerRepository.GetFirstOrDefault();
  Assert.IsNotNull(retVal.Address);
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
        CustomerID = "YAMI1",
        CompanyName = "Deneme - CompanyName"
    });

    unitOfWork.SaveChanges();
    unitOfWork.RollbackTransaction();

    unitOfWork.Dispose();
    var isDisposed = (bool)GetInstanceField(typeof(UnitOfWork), unitOfWork, "_disposed");
    Assert.IsTrue(isDisposed);

    unitOfWork.Dispose();
    context.Dispose();

    context.Dispose();
    unitOfWork.Dispose();
}
```

```c#
var context = new NORTHWNDEntities();
using (IUnitOfWork unitOfWork = new UnitOfWork(context))
{
    var customerService = new CustomerService(unitOfWork);
    var result = customerService.GetFirstOrDefault();
    Assert.IsNotNull(result.ResultObject.CompanyName);
}
```
