namespace SahinRektefiyeSoln.Models.ViewModels.Ticket
{
    public class ShowTicketDetailSaveModel
    {        
        public int TalepId { get; set; }
        public string Iscilik { get; set; }
        public string BirimFiyat { get; set; }
        public string ParcaIskonto { get; set; }
        public string GenelIskonto { get; set; }
        public string BirimToplamFiyat { get; set; }
        public string ToplamFiyat { get; set; }
        public string KDV { get; set; }
        public string GenelToplam { get; set; }
    }
}