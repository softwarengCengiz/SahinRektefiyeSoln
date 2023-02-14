using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Reports
{
	public class CustomerModel
	{
		public string kullaniciAdi { get; set; }
		public string telno { get; set; }
		
		public List<Users> kullanicilar { get; set; }
	}
}