using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.ServiceReport
{
    public class ServisAracTakipModel
    {
        public ServisAracTakipModel()
        {
            ServisAracTakipList = new List<ServisAracTakipResponse>();
        }
        public DateTime bsgRaporTarihi { get; set; }

        public DateTime btsRaporTarihi { get; set; }
        public int VehicleMusteriId { get; set; }
        public string VehicleMusteriText { get; set; }
        public string command { get; set; }


        public List<ServisAracTakipResponse> ServisAracTakipList { get; set; }

    }

    public class ServisAracTakipResponse
    {
        public string FirmaIsmi { get; set; }
        public string IsEmriTipi { get; set; }
        public string AracCinsi { get; set; }
        public string Plaka { get; set; }
        public string Ariza { get; set; }
        public string Durumu { get; set; }
        public DateTime? GelisTarihi { get; set; }
    }
}