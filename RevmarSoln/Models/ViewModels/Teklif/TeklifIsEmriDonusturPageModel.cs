using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Teklif
{
    public class TeklifSorgulaPageModel
    {
		public TeklifSorgulaPageModel()
		{
			teklifStatuleri = new List<ListModel>();
			isEmriTipleri = new List<ListModel>();
		}
		public List<Teklifler> teklifler { get; set; }
		public Teklifler filter { get; set; }

		public List<ListModel> teklifStatuleri { get; set; }
		public List<ListModel> isEmriTipleri { get; set; }

		public DateTime? TeklifTarihi { get; set; }

	}

	public class ListModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool Value { get; set; }
	}
}