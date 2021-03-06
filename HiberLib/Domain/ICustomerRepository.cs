﻿using System;
using System.Collections.Generic;

namespace HiberLib.Domain
{
	public interface ICustomerRepository
	{

		void Add(Customer customer);
		void Update(Customer customer);
		void Remove(Customer customer);
		void RemoveAll();
		Int32 Count();
		Customer GetByID(Guid customerID);
		Customer GetByFirstname(string firstname);
		ICollection<Customer> GetByDateCreated(DateTime dateFrom, DateTime dateTo);
	}
}
