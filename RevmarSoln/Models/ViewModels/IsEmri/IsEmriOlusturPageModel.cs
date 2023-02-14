using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.IsEmri
{
	public class IsEmriOlusturPageModel
	{
		public Musteri musteri { get; set; }
		public Arac arac { get; set; }
		public IsEmirleri isEmri { get; set; }

		public int Km{ get; set; }
		public int IsEmriTipId { get; set; }
		public int VehicleMusteriId { get; set; }
		public int DanismanId { get; set; }

		public bool SwAracInService { get; set; }

	}
}