using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SahinRektefiyeSoln.Models.HelperModels;

namespace SahinRektefiyeSoln.Models.ViewModels.Teklif
{
    public class TeklifDuzenlePageModel
    {
        public Teklifler teklifler { get; set; }
        public List<FaturaKalemTeklif> teklifKalemleri { get; set; }
    }
}