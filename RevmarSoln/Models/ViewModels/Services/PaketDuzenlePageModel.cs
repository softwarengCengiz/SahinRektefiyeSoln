using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class PaketDuzenlePageModel
	{
		public Paketler paket { get; set; }

		public int PaketId { get; set; }
		public double StokKartiAdedi { get; set; }
		public int StokKartId { get; set; }

		public int IscilikId { get; set; }

		public int CompanyId { get; set; }


	}
}