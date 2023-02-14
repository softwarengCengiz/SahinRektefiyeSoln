using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SahinRektefiyeSoln.Models.HelperModels;

namespace SahinRektefiyeSoln.Models.ViewModels.IsEmri
{
	public class IsEmriDuzenlePageModel
	{
		public IsEmirleri isEmri { get; set; }
		public List<FaturaKalem> isEmriKalemleri { get; set; }

		public bool SwAracInService { get; set; }
	}
}