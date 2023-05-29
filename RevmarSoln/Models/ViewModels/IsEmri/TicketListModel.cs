using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.IsEmri
{
	public class TicketListModel
    {
		public int TalepNo { get; set; }
        public string Musteri { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? MusteriAramaTarihi { get; set; }
        public string AtananKisi { get; set; }
        public string OlustuanKisi { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? OlusturmaTarihi { get; set; }
        public string Durum { get; set; }
        public bool IsDetailAvailable { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? MusteriAtolyeGelisTarihi { get; set; }
        public string Sube { get; set; }
    }
}