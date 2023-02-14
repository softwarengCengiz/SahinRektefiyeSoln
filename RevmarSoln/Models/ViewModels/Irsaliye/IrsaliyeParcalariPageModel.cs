using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Irsaliye
{
	public class IrsaliyeParcalariPageModel
	{
		public int IrsaliyeParcaId { get; set; }
		public int IrsaliyeId { get; set; }
		public int StokKartId { get; set; }
		public int Adet { get; set; }
		public decimal BirimFiyat { get; set; }
		public decimal Iskonto { get; set; }
		public decimal ToplamFiyat { get; set; }
		public string Creator { get; set; }
		public int IadeAdet { get; set; }
		
	}
}