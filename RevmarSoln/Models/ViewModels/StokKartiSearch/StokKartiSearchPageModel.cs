using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.StokKartiSearch
{
	public class StokKartiSearchPageModel
	{
		public List<StokKarti> stokKartlari { get; set; }
		public StokKarti filter { get; set; }
	}
}