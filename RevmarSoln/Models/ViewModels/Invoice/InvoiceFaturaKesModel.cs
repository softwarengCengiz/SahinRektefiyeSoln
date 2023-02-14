using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Invoice
{
	public class InvoiceFaturaKesModel
	{
		public Guid mdlFaturaId { get; set; }
		public string mdlIsEmriId { get; set; }
		public string mdlIsEmriTipi { get; set; }
		public string ddlFaturaTipi { get; set; }
		public string faturaNumarasi { get; set; }
		//public string FisNumarasi { get; set; }
		
	}
}