using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Invoice
{
	public class FaturaAraPageModel
	{
		public FaturaFilterModel filter { get; set; }
	}

	public class FaturaFilterModel
	{
		public string IsEmriTipi { get; set; }
		public string Musteri { get; set; }
		public int FaturaStatu { get; set; }
		public int FaturaTipi { get; set; }

		public string IsEmriNo { get; set; }
		public string Plaka { get; set; }
		public string Sasi { get; set; }
		public string FaturaFisNo { get; set; }
		public bool SwFaturalandi { get; set; }
		public DateTime GndrlnBsgTarih { get; set; }
		public DateTime GndrlnBtsTarih { get; set; }
		public DateTime BsgTarih { get; set; }
		public DateTime BtsTarih { get; set; }
	}
}