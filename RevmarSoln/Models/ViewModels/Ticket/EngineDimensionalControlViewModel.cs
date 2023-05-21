using System.Collections.Generic;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Models.ViewModels.Ticket
{
    public class EngineDimensionalControlViewModel
    {
        public int MotorOlcuselKontrolId { get; set; }
        public int TalepId { get; set; }
        public string KapakDolapNo { get; set; }
        public string MotorDolapNo { get; set; }
        public string IzinVerilenMaxEgrilik { get; set; }
        public string TespitEdilenEgrilik { get; set; }
        public string SilindirCaplariStdDeger { get; set; }
        public List<List<string>> SilindirCaplari { get; set; }
        public string MaxAsinma { get; set; }
        public string MaxOvallik { get; set; }
        public string MaxKoniklik { get; set; }
        public string SilindirNo1 { get; set; }
        public string SilindirNo2 { get; set; }
        public string SilindirNo3 { get; set; }
        public string GomlekFaturaStdDeger { get; set; }
        public List<List<string>> GomlekFaturaTasma { get; set; }
        public List<List<string>> GomlekYuvaCapi { get; set; }
        public string PistonCapiStdDeger { get; set; }
        public List<List<string>> PistonCapi { get; set; }
        public string AnaMuyluStdDeger { get; set; }
        public List<List<string>> AnaMuylu { get; set; }
        public string KolMuyluStdDeger { get; set; }
        public List<List<string>> KolMuylu { get; set; }
        public string KrankMiliSalgiDegeri { get; set; }
        public IList<string> MotorIncelemeSonuc { get; set; }
        public IList<SelectListItemWithAttribute> EngineInfoDet { get; set; }
    }
}