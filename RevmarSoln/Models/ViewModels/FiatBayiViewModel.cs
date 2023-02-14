using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels
{
	public class FiatBayiViewModel
	{
		public string PartNumber { get; set; }
		public IEnumerable<SupplierParts> SupplierParts { get; set; }
	}
}