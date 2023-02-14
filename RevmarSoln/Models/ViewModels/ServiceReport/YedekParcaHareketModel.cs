using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.ServiceReport
{
    public class YedekParcaHareketModel
    {
        public string ParcaNo { get; set; }
        public string ParcaAdi { get; set; }
        public int? IsEmriId { get; set; }
        public int? ParcaId { get; set; }
        public List<YedekParcaHareketResponse> YedekParcaHareketResponseList { get; set; }
        public string command { get; set; }
    }

    public class YedekParcaHareketResponse
    {
        public int IsEmriId { get; set; }
        public string ParcaNo { get; set; }
        public string ParcaAdi { get; set; }
        public string SwFatura { get; set; }
        public string FaturaNo { get; set; }
        public string StokBilgisi { get; set; }
    }
}