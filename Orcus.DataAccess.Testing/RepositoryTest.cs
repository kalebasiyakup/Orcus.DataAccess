using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orcus.DataAccess.Testing.ServicePattern;
using System;
using System.Reflection;

namespace Orcus.DataAccess.Testing
{
    [TestClass]
    public class RepositoryTest
    {
        [TestMethod]
        public void UnitOfWork_Test()
        {
            using (IUnitOfWork unitOfWork = new UnitOfWork(new NORTHWNDEntities()))
            {
                unitOfWork.BeginTransaction();
                unitOfWork.CommitTransaction();
            }
        }

        [TestMethod]
        public void UnitOfWork_Pattern_Test()
        {
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
        }

        [TestMethod]
        public void Repository_Pattern_GetData_Test()
        {
            var context = new NORTHWNDEntities();
            using (IUnitOfWork unitOfWork = new UnitOfWork(context))
            {
                IRepository<Customers> customerRepository = unitOfWork.Repository<Customers>();
                var retVal = customerRepository.GetFirstOrDefault();
                Assert.IsNotNull(retVal.Address);
            }
        }

        [TestMethod]
        public void UnitOfWork_Service_Pattern_Test()
        {
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
        }

        [TestMethod]
        public void Service_Pattern_GetData()
        {
            var context = new NORTHWNDEntities();
            using (IUnitOfWork unitOfWork = new UnitOfWork(context))
            {
                var customerService = new CustomerService(unitOfWork);
                var result = customerService.GetFirstOrDefault();
                Assert.IsNotNull(result.ResultObject.CompanyName);
            }
        }

        [TestMethod]
        public void Service_Pattern_GetData_CustomersByCompany()
        {
            var context = new NORTHWNDEntities();
            using (IUnitOfWork unitOfWork = new UnitOfWork(context))
            {
                var customerService = new CustomerService(unitOfWork);
                var result = customerService.CustomersByCompany("bon");
                Assert.IsNotNull(result);
            }
        }
        
        private static object GetInstanceField(Type type, object instance, string fieldName)
        {
            const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var field = type.GetField(fieldName, bindFlags);
            return field != null ? field.GetValue(instance) : null;
        }
    }
}
