using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.IsEmri
{
	public class TicketListModel
    {
		public int TalepNo { get; set; }
        public string Musteri { get; set; }
        public DateTime? MusteriAramaTarihi { get; set; }
        public string AtananKisi { get; set; }
        public string OlustuanKisi { get; set; }
        public DateTime? OlusturmaTarihi { get; set; }
        public string Durum { get; set; }
        public bool IsDetailAvailable { get; set; }
    }
}