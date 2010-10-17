using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

using HiberLib.Domain;
using HiberLib.Repositories;

namespace HiberLib.Tests
{
	[TestFixture]
	public class ProductRepository_Fixture
	{
		private ISessionFactory _sessionFactory;
		private Configuration _configuration;


		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_configuration = new Configuration();
			_configuration.Configure();
			_configuration.AddAssembly(typeof(Product).Assembly);
			_sessionFactory = _configuration.BuildSessionFactory();
		}
		
		[SetUp]
		public void SetupContext()
		{
			new SchemaExport(_configuration).Execute(true, true, false);
		}

		[Test]
		public void Can_add_new_product()
		{
			var product = new Product { Name = "Apple", Category = "Fruits" };
			IProductRepository repository = new ProductRepository();
			repository.Add(product);

			// use session to try to load the product
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Product>(product.Id);
				// Test that the product was successfully inserted
				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(product, fromDb);
				Assert.AreEqual(product.Name, fromDb.Name);
				Assert.AreEqual(product.Category, fromDb.Category);
			}
		}
	}
}