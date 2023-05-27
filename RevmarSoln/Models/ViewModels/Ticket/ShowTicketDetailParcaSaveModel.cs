using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Ticket
{
    public class ShowTicketDetailParcaSaveModel
    {
        public int TalepId { get; set; }
        public string Parca { get; set; }
        public string Adet { get; set; }
        public string BirimFiyat { get; set; }
        public string ParcaIskonto { get; set; }
        public string GenelIskonto { get; set; }
        public string BirimToplamFiyat { get; set; }
        public string ToplamFiyat { get; set; }
        public string KDV { get; set; }
        public string GenelToplam { get; set; }
    }
}