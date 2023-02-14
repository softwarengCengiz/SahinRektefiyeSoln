using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.ServiceReport
{
    public class IsEmriYaslanma
    {
        public IsEmriYaslanma()
        {
			YaslanmaList = new List<IsEmriYaslanmaDetay>();
        }
        public List<IsEmriYaslanmaDetay> YaslanmaList { get; set; }

	}


    public class IsEmriYaslanmaDetay
    {
        public int IsEmriId { get; set; }
        public int YaslanmaGunu { get; set; }
        public DateTime IsEmriTarihi { get; set; }
        public int YaslanmaGunSayisi { get; set; }
		public string IsEmriTipi { get; set; }
        public string MusteriAdi { get; set; }
        public string Plaka { get; set; }
    }
}