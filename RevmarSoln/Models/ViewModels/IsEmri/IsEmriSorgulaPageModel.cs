using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Models.ViewModels.IsEmri
{
	public class IsEmriSorgulaPageModel
	{
		public IsEmriSorgulaPageModel()
		{
			isEmriStatuleri = new List<ListModel>();
			isEmriTipleri = new List<ListModel>();
		}
		public List<IsEmirleri> isEmirleri { get; set; }
		public IsEmirleri  filter{ get; set; }

		public List<ListModel> isEmriStatuleri { get; set; }
		public List<ListModel> isEmriTipleri { get; set; }

		public DateTime? IsEmriBaslangicTarihi { get; set; }
		public DateTime? IsEmriBitisTarihi { get; set; }

		public DateTime? FaturaBaslangicTarihi { get; set; }
		public DateTime? FaturaBitisTarihi { get; set; }

		public bool SwAracInServisOrAll { get; set; }
	}

	public class ListModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool Value { get; set; }
	}
}