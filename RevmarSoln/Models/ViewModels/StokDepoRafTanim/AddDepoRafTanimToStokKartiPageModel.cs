using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.StokDepoRafTanim
{
	public class AddDepoRafTanimToStokKartiPageModel
	{
		public StokKarti StokKarti { get; set; }
		public int StokKartiId { get; set; }
		public int DepoIds { get; set; }
	}
}