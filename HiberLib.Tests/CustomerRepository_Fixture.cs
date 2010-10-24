using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Criterion;
using NUnit.Framework;

using HiberLib.Domain;
using HiberLib.Repositories;

namespace HiberLib.Tests
{
	[TestFixture]
	public class CustomerRepository_Fixture
	{
		private ISessionFactory _sessionFactory;
		private Configuration _configuration;

		private readonly Customer[] _customers = new[]
		{
			new Customer {Firstname= "John", Lastname = "Smith", DateCreated = DateTime.Now},
			new Customer {Firstname= "Lock", Lastname = "Goldman", DateCreated = DateTime.Now.AddDays(-100)},
			new Customer {Firstname= "Dokimis", Lastname = "Dokimakis", DateCreated = DateTime.Now.AddHours(-3)},
			new Customer {Firstname= "Michel", Lastname ="Manolo", DateCreated = DateTime.Now},
			new Customer {Firstname= "Elvis", Lastname = "Manuel", DateCreated = DateTime.Now}
		};

		private void CreateInitialData()
		{
			using (ISession session = _sessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				foreach (var customer in _customers)
					session.Save(customer);

				transaction.Commit();
			}
		}


		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_configuration = new Configuration();
			_configuration.Configure();
			_configuration.AddAssembly(typeof(Customer).Assembly);
			_sessionFactory = _configuration.BuildSessionFactory();
		}

		[SetUp]
		public void SetupContext()
		{
			new SchemaExport(_configuration).Execute(true, true, false);
			CreateInitialData();
		}

		[Test]
		public void Can_add_new_customer()
		{
			var customer = new Customer { Firstname= "Apple", Lastname = "Dethero", DateCreated = DateTime.Now};
			ICustomerRepository repository = new CustomerRepository();
			repository.Add(customer);

			// use session to try to load the customer
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Customer>(customer.CustomerID);
				// Test that the customer was successfully inserted
				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(customer, fromDb);
				Assert.AreEqual(customer.Firstname, fromDb.Firstname);
			}
		}

		[Test]
		public void Can_update_existing_customer()
		{
			var customer = _customers[0];
			customer.Firstname= "John";
			ICustomerRepository repository = new CustomerRepository();
			repository.Update(customer);
			
			// use session to try to load the customer
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Customer>(customer.CustomerID);
				Assert.AreEqual(customer.Firstname, fromDb.Firstname);
			}
		}

		[Test]
		public void Can_remove_existing_customer()
		{
			var customer = _customers[0];
			ICustomerRepository repository = new CustomerRepository();
			repository.Remove(customer);
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDB = session.Get<Customer>(customer.CustomerID);
				Assert.IsNull(fromDB);
			}
		}

		[Test]
		public void Can_Delete_All_Customers()
		{
			ICustomerRepository repository = new CustomerRepository();
			repository.RemoveAll();
			Assert.AreEqual(repository.Count(), 0);
		}

		[Test]
		public void Can_Count()
		{
			ICustomerRepository repository = new CustomerRepository();
			Assert.AreEqual(repository.Count(), _customers.Length);
		}

		[Test]
		public void Can_get_existing_customer_by_id()
		{
			ICustomerRepository repository = new CustomerRepository();
			var fromDb = repository.GetByID(_customers[1].CustomerID);
			Assert.IsNotNull(fromDb);
			Assert.AreNotSame(_customers[1], fromDb);
			Assert.AreEqual(_customers[1].Firstname, fromDb.Firstname);
		}

		[Test]
		public void Can_get_existing_customer_by_name()
		{
			ICustomerRepository repository = new CustomerRepository();
			var fromDb = repository.GetByFirstname(_customers[1].Firstname);

			Assert.IsNotNull(fromDb);
			Assert.AreNotSame(_customers[1], fromDb);
			Assert.AreEqual(_customers[1].CustomerID, fromDb.CustomerID);
		}


		[Test]
		public void Fullname_Formula_works()
		{
			ICustomerRepository repository = new CustomerRepository();
			var fromDb = repository.GetByID(_customers[1].CustomerID);
			Assert.IsNotNull(fromDb);
			Assert.AreNotSame(_customers[1], fromDb);
			StringAssert.Contains(fromDb.Firstname, fromDb.Fullname);
			StringAssert.Contains(fromDb.Lastname, fromDb.Fullname);
		}

		[Test]
		public void Can_get_Customers_By_DateCreated()
		{
			ICustomerRepository repository = new CustomerRepository();
			var fromDB = repository.GetByDateCreated(DateTime.Now.AddMinutes(-5), DateTime.Now.AddMinutes(5));

			Assert.IsNotNull(fromDB);
			// needs refactoring, hardcoded 3 from _Customers with dateCreated = now.
			Assert.AreEqual(fromDB.Count, 3);
		}


		private bool IsInCollection(Customer customer, ICollection<Customer> fromDb)
		{
			foreach (var item in fromDb)
				if (customer.CustomerID == item.CustomerID)
					return true;
			return false;
		}
	}
}