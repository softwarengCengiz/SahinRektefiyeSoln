using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Customer
{
	public class MusteriEditPageModel
	{
		public Musteri musteri { get; set; }

		public string Telefon { get; set; }
		public string Mail { get; set; }

		public int? IlId { get; set; }
		public int? IlceId { get; set; }
		public int? KurumTipId { get; set; }
		public int? KontakKatId { get; set; }
		public int? MeslekId { get; set; }
		public int? VergiDaireIlId { get; set; }
		public int? VergiDaireIlceId { get; set; }
	}
}