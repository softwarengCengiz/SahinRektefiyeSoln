using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Customer
{
	public class MusteriIletisimBilgileriYonetPageModel
	{
		public Musteri musteri { get; set; }
		public List<MusteriMail> mailler { get; set; }
		public List<MusteriTelefon> telefonlar { get; set; }

	}
}