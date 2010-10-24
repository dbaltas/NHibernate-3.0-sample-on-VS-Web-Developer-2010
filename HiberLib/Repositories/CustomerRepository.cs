using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HiberLib.Domain;

using NHibernate;
using NHibernate.Criterion;

namespace HiberLib.Repositories
{
	public class CustomerRepository : ICustomerRepository
	{
		public void Add(Customer customer)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Save(customer);
				transaction.Commit();
			}
		}

		public void Update(Customer customer)
		{
			using (ISession session = NHibernateHelper.OpenSession()) {
				using (ITransaction transaction = session.BeginTransaction()) {
					session.Update(customer);
					transaction.Commit();
				}
			}
		}

		public void Remove(Customer customer)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.Delete(customer);
					transaction.Commit();
				}
			}
		}

		public Customer GetByID(Guid customerID)
		{
			using (ISession session = NHibernateHelper.OpenSession())
				return session.Get<Customer>(customerID);
		}

		public Customer GetByFirstname(string name)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				Customer customer = session
					.CreateCriteria(typeof(Customer))
					.Add(Restrictions.Eq("Firstname", name))
					.UniqueResult<Customer>();
				return customer;
			}
		}

		public ICollection<Customer> GetByDateCreated(DateTime dateFrom, DateTime dateTo)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var customers = session
					.CreateCriteria(typeof(Customer))
					.Add(Restrictions.Between("DateCreated", dateFrom, dateTo)).List<Customer>();
				return customers;
			}
		}

		public ICollection<Customer> GetByCategory(string category)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var customers = session.
					CreateCriteria(typeof(Customer)).
					Add(Restrictions.Eq("Category", category)).List<Customer>();

				return customers;
			}
		}
	}
}

