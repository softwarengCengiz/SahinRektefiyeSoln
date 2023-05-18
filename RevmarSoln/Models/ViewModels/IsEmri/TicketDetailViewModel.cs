using SahinRektefiyeSoln.Models.ViewModels.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Models.ViewModels.IsEmri
{
	public class TicketDetailViewModel
    {
        public TicketDetailViewModel()
        {
            ArizaChck = new List<string>();
            ParcalarChck = new List<string>();
            ParcalarText = new List<string>();
            ParcalarId = new List<string>(); 
        }
        public int Id { get; set; }
        public int TalepId { get; set; }
        public string MotorDolapNo { get; set; }
        public string KapakDolapNo { get; set; }
        public Nullable<System.DateTime> BildirimTarihi { get; set; }
        public string ServisAdı { get; set; }
		public string Marka { get; set; }
        public string Model { get; set; }
        public string MotorTipi { get; set; }
		public int YakıtTipi { get; set; }
		public int SilindirSayisi { get; set; }
		public bool Garanti { get; set; }
		public bool Revizyon { get; set; }
        public string RevizyonAciklama { get; set; }
        public string ServisNo { get; set; }
        public int AlınanIs { get; set; }
        public bool AlınanIsM { get; set; }
        public bool AlınanIsK { get; set; }
		public string Plaka { get; set; }
        public int KM { get; set; }
        public string VinNo { get; set; }
        public string MotorNo { get; set; }
		public string SupapSayisi { get; set; }
        public string MusteriNot { get; set; }
        public IList<SelectListItem> Ariza { get; set; }
        public IList<SelectListItemWithAttribute> Parcalar { get; set; }
        public IList<string> ParcalarChck { get; set; }
        public IList<string> ArizaChck { get; set; }
        public IList<string> ParcalarText { get; set; }
        public IList<string> ParcalarId { get; set; }
        
        public string ArizaDiger { get; set; }
        public string ParcaDiger { get; set; }
        public string ParcaAdet { get; set; }
        public int IsLogoEnable { get; set; }
        public HttpPostedFileBase[] FileInput { get; set; }
        public string FileInputView { get; set; }
        public int FlagSave { get; set; }
        public string BrandName { get; set; }
        public string BrandModelName { get; set; }
    }
}