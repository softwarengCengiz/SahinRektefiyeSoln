using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Teklif
{
    public class TeklifIsEmriDonusturPageModel
    {
        public TeklifIsEmriDonusturPageModel()
        {

        }

        public Guid TeklifId { get; set; }
        public int DanismanId { get; set; }
        public int KM { get; set; }

        public DateTime? TahminiTeslimTarihi { get; set; }

    }

}