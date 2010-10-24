using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HiberLib.Domain
{
	public class Customer
	{
		public virtual Guid CustomerID { get; set; }
		public virtual string Firstname { get; set; }
		public virtual string Lastname { get; set; }
		public virtual string Fullname { get; set; }
		public virtual DateTime DateCreated { get; set; }
	}
}
