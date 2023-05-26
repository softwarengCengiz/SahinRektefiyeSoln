using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Models.ViewModels.Ticket
{
    public class SelectListItemWithAttribute : SelectListItem
    {
        public int? IsContainPiece { get; set; }
        public int? HdrId { get; set; }
        public string BirimFiyat { get; set; }
        public string ParcaIskonto { get; set; }
        public string BirimToplamFiyat { get; set; }
    }
}