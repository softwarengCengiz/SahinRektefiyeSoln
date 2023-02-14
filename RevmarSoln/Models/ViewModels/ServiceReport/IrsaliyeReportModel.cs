using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.ServiceReport
{
    public class IrsaliyeReportModel
    {
		public IrsaliyeReportModel()
		{
			irsaliyeler = new List<IrsaliyeReportViewModel>();
		}
		public DateTime bsgRaporTarihi { get; set; }

		public DateTime btsRaporTarihi { get; set; }
		public int VehicleMusteriId { get; set; }
		public string VehicleMusteriText { get; set; }
		public string command { get; set; }


		public List<IrsaliyeReportViewModel> irsaliyeler { get; set; }

	}

	public class IrsaliyeReportViewModel
	{
		public DateTime FaturaTarihi { get; set; }
		public DateTime KayitTarihi { get; set; }
		public string Tedarikci { get; set; }
		public int? KalemAdedi { get; set; }
		public int? ParcaAdedi { get; set; }
		public decimal? ToplamTutar { get; set; }
		public decimal? ToplamKDV { get; set; }
		public decimal? IrsaliyeToplami { get; set; }
		public int IrsaliyeId { get; set; }
	}
}