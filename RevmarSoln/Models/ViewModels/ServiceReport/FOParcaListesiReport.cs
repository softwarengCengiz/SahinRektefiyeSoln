using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.ServiceReport
{
    public class FOParcaListesiModel
    {
        public FOParcaListesiModel()
        {
            FOParcaListesiResponse = new List<FOParcaListesiResponse>();
        }
        public string command { get; set; }
        public List<FOParcaListesiResponse> FOParcaListesiResponse { get; set; }
    }
    public class FOParcaListesiResponse
    {
        public string SiraNo { get; set; }
        public string RevmerIsEmriNo { get; set; }
        public DateTime TalepTarihi { get; set; }
        public string Name { get; set; }
        public string ParcaAdi { get; set; }
        public string ParcaNo { get; set; }
        public string SeriNo { get; set; }
        public int? BarkodNo { get; set; }
        public string Kleymno { get; set; }
        public string AracModel { get; set; }//Static herzaman FORD
        public string Vin { get; set; }
        public int? RevTipi { get; set; }
        public decimal? RevizyonTutari { get; set; }
        public DateTime? SevkTarihi { get; set; }
        public string KargoFirmasi { get; set; }
        public string IrsaliyeNo { get; set; }
        public string FaturaNo { get; set; }
        public int FordPartTypeId { get; set; }
    }
}