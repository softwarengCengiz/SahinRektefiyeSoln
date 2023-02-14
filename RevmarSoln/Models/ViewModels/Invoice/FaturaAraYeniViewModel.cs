using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Invoice
{
	public class FaturaAraYeniViewModel
	{
		public FaturaFilterYeniModel filter { get; set; }
		public int? Page { get; set; }


		public string IsEmriTipi { get; set; }
		public string Musteri { get; set; }
		public int FaturaStatu { get; set; }
		public int FaturaTipi { get; set; }

		public string IsEmriNo { get; set; }
		public string Plaka { get; set; }
		public string Sasi { get; set; }
		public string FaturaFisNo { get; set; }
		public bool SwFaturalandi { get; set; }
		public DateTime? GndrlnBsgTarih { get; set; }
		public DateTime? GndrlnBtsTarih { get; set; }
		public DateTime? BsgTarih { get; set; }
		public DateTime? BtsTarih { get; set; }
		public int? IsEmriTipId { get; set; }

		public string command { get; set; }


	}

	public class FaturaFilterYeniModel
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