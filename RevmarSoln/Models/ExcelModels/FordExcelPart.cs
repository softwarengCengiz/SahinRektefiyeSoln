using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ExcelModels
{
	public class FordExcelPart
	{
		public int ParcaAdet { get; set; }
		public string ServisAdi { get; set; }
		public Nullable<DateTime> SatisTarihi { get; set; }
		public DateTime? TamirTarihi { get; set; }
		public string AracTipi { get; set; }
		public int Km { get; set; }
		public string ParcaNo { get; set; }
		public string KleymNo { get; set; }
		public string Vin { get; set; }
		public string ParcaDurumu { get; set; }
		public string KleymTipi { get; set; }
		public int BarkodNo { get; set; }
		public DateTime? UretimTarihi { get; set; }
		public int IdNo { get; set; }
		public DateTime? Tarih { get; set; }

	}
}