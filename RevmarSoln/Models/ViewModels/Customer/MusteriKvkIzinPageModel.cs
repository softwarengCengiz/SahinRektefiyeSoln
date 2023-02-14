using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Customer
{
	public class MusteriKvkIzinPageModel
	{
		public int MusteriId { get; set; }
		public List<MusteriTelefon> musteriTelefonlari { get; set; }
		public List<MusteriMail> musteriMailleri { get; set; }
		public MusteriMail mail { get; set; }
		public MusteriTelefon telefon { get; set; }

	}
}