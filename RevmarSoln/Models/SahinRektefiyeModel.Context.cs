﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SahinRektefiyeSoln.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SahinRektefiyeDbEntities : DbContext
    {
        public SahinRektefiyeDbEntities()
            : base("name=SahinRektefiyeDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AktarmaTipi> AktarmaTipi { get; set; }
        public virtual DbSet<Arac> Arac { get; set; }
        public virtual DbSet<AracDetay> AracDetay { get; set; }
        public virtual DbSet<AracGrubu> AracGrubu { get; set; }
        public virtual DbSet<AracTipi> AracTipi { get; set; }
        public virtual DbSet<ArizaBildirim> ArizaBildirim { get; set; }
        public virtual DbSet<ChangeRequest> ChangeRequest { get; set; }
        public virtual DbSet<ChangeRequestStatus> ChangeRequestStatus { get; set; }
        public virtual DbSet<Companies> Companies { get; set; }
        public virtual DbSet<Danismanlar> Danismanlar { get; set; }
        public virtual DbSet<DepoRafTanim> DepoRafTanim { get; set; }
        public virtual DbSet<DpfRevisionTypes> DpfRevisionTypes { get; set; }
        public virtual DbSet<Irsaliyeler> Irsaliyeler { get; set; }
        public virtual DbSet<Iscilikler> Iscilikler { get; set; }
        public virtual DbSet<IscilikTipleri> IscilikTipleri { get; set; }
        public virtual DbSet<IsEmirleri> IsEmirleri { get; set; }
        public virtual DbSet<IsEmriBelirtiKodlari> IsEmriBelirtiKodlari { get; set; }
        public virtual DbSet<IsEmriDosyalari> IsEmriDosyalari { get; set; }
        public virtual DbSet<IsEmriKalemleri> IsEmriKalemleri { get; set; }
        public virtual DbSet<IsEmriStatu> IsEmriStatu { get; set; }
        public virtual DbSet<IsEmriTipleri> IsEmriTipleri { get; set; }
        public virtual DbSet<ilceler> ilceler { get; set; }
        public virtual DbSet<iller> iller { get; set; }
        public virtual DbSet<JobDetails> JobDetails { get; set; }
        public virtual DbSet<Jobs> Jobs { get; set; }
        public virtual DbSet<KlimaFixing> KlimaFixing { get; set; }
        public virtual DbSet<KlimaRevisionTypes> KlimaRevisionTypes { get; set; }
        public virtual DbSet<KontakKategorileri> KontakKategorileri { get; set; }
        public virtual DbSet<KurumTipleri> KurumTipleri { get; set; }
        public virtual DbSet<KvkMusteriLogs> KvkMusteriLogs { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<Mail> Mail { get; set; }
        public virtual DbSet<MailAdresleri> MailAdresleri { get; set; }
        public virtual DbSet<MailGruplari> MailGruplari { get; set; }
        public virtual DbSet<MarsMotorFixing> MarsMotorFixing { get; set; }
        public virtual DbSet<MarsMotorRevisionTypes> MarsMotorRevisionTypes { get; set; }
        public virtual DbSet<Menus> Menus { get; set; }
        public virtual DbSet<Meslekler> Meslekler { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<Musteri> Musteri { get; set; }
        public virtual DbSet<MusteriAraclari> MusteriAraclari { get; set; }
        public virtual DbSet<MusteriMail> MusteriMail { get; set; }
        public virtual DbSet<MusteriTelefon> MusteriTelefon { get; set; }
        public virtual DbSet<PaketIscilikleri> PaketIscilikleri { get; set; }
        public virtual DbSet<Paketler> Paketler { get; set; }
        public virtual DbSet<PaketStokKartlari> PaketStokKartlari { get; set; }
        public virtual DbSet<ParcaTipleri> ParcaTipleri { get; set; }
        public virtual DbSet<PartReviewResult> PartReviewResult { get; set; }
        public virtual DbSet<PartReviewType> PartReviewType { get; set; }
        public virtual DbSet<Parts> Parts { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<Renkler> Renkler { get; set; }
        public virtual DbSet<RevisionType> RevisionType { get; set; }
        public virtual DbSet<RolePermissions> RolePermissions { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<SanzimanFixing> SanzimanFixing { get; set; }
        public virtual DbSet<SanzimanRevisionTypes> SanzimanRevisionTypes { get; set; }
        public virtual DbSet<SayimDetail> SayimDetail { get; set; }
        public virtual DbSet<SayimHeader> SayimHeader { get; set; }
        public virtual DbSet<SayimStatu> SayimStatu { get; set; }
        public virtual DbSet<SayimTipi> SayimTipi { get; set; }
        public virtual DbSet<SensorRevisionTypes> SensorRevisionTypes { get; set; }
        public virtual DbSet<Services> Services { get; set; }
        public virtual DbSet<StokKarti> StokKarti { get; set; }
        public virtual DbSet<StokKartiAlternatif> StokKartiAlternatif { get; set; }
        public virtual DbSet<StokKartiDetay> StokKartiDetay { get; set; }
        public virtual DbSet<StokKartiSayimLoglari> StokKartiSayimLoglari { get; set; }
        public virtual DbSet<SupplierCompany> SupplierCompany { get; set; }
        public virtual DbSet<SupplierParts> SupplierParts { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TalepDetay> TalepDetay { get; set; }
        public virtual DbSet<TalepDosya> TalepDosya { get; set; }
        public virtual DbSet<TalepDosyaSekli> TalepDosyaSekli { get; set; }
        public virtual DbSet<Talepler> Talepler { get; set; }
        public virtual DbSet<TalepSekli> TalepSekli { get; set; }
        public virtual DbSet<TamirParca> TamirParca { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<TeklifKalemleri> TeklifKalemleri { get; set; }
        public virtual DbSet<Teklifler> Teklifler { get; set; }
        public virtual DbSet<TeklifStatu> TeklifStatu { get; set; }
        public virtual DbSet<TurboRevisionTypes> TurboRevisionTypes { get; set; }
        public virtual DbSet<TurboSarjFixing> TurboSarjFixing { get; set; }
        public virtual DbSet<UploadedFordExcels> UploadedFordExcels { get; set; }
        public virtual DbSet<UserAgentInfos> UserAgentInfos { get; set; }
        public virtual DbSet<UserAllowedIsEmriTip> UserAllowedIsEmriTip { get; set; }
        public virtual DbSet<UserMenus> UserMenus { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Vehicles> Vehicles { get; set; }
        public virtual DbSet<VehicleStoks> VehicleStoks { get; set; }
        public virtual DbSet<VitesTipi> VitesTipi { get; set; }
        public virtual DbSet<WayBillParts> WayBillParts { get; set; }
        public virtual DbSet<WayBills> WayBills { get; set; }
        public virtual DbSet<WebErrorLogs> WebErrorLogs { get; set; }
        public virtual DbSet<WebUsageLogs> WebUsageLogs { get; set; }
        public virtual DbSet<WorkOrders> WorkOrders { get; set; }
        public virtual DbSet<YakitTipi> YakitTipi { get; set; }
    
        public virtual int AutoCreateUserMenu(string userName)
        {
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AutoCreateUserMenu", userNameParameter);
        }
    
        public virtual ObjectResult<EnCokSatilanParcalar_Result> EnCokSatilanParcalar()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<EnCokSatilanParcalar_Result>("EnCokSatilanParcalar");
        }
    
        public virtual ObjectResult<FiatPartCountByWorkType_Result> FiatPartCountByWorkType()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<FiatPartCountByWorkType_Result>("FiatPartCountByWorkType");
        }
    
        public virtual ObjectResult<FiatSeriesCountByPartNumber_Result> FiatSeriesCountByPartNumber()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<FiatSeriesCountByPartNumber_Result>("FiatSeriesCountByPartNumber");
        }
    
        public virtual int GetBarkodNoForEmptyRow(ObjectParameter barkodNo)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetBarkodNoForEmptyRow", barkodNo);
        }
    
        public virtual int GetCiro(ObjectParameter kdvli, ObjectParameter kdvsiz, ObjectParameter sadeceKdv)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetCiro", kdvli, kdvsiz, sadeceKdv);
        }
    
        public virtual int GetDailyFaturaAdet(ObjectParameter faturaAdet)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetDailyFaturaAdet", faturaAdet);
        }
    
        public virtual int GetFordExcelRevizyonTakipNo(string workType, ObjectParameter revizyonTakipNumarasi)
        {
            var workTypeParameter = workType != null ?
                new ObjectParameter("WorkType", workType) :
                new ObjectParameter("WorkType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetFordExcelRevizyonTakipNo", workTypeParameter, revizyonTakipNumarasi);
        }
    
        public virtual int GetFordSiparisRevizyonNumarasi(string gecici, ObjectParameter revizyonTakipNumarasi)
        {
            var geciciParameter = gecici != null ?
                new ObjectParameter("Gecici", gecici) :
                new ObjectParameter("Gecici", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetFordSiparisRevizyonNumarasi", geciciParameter, revizyonTakipNumarasi);
        }
    
        public virtual int GetFordSiparisRevmerIsEmriNo()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetFordSiparisRevmerIsEmriNo");
        }
    
        public virtual ObjectResult<Nullable<long>> GetIscilikSequence()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<long>>("GetIscilikSequence");
        }
    
        public virtual ObjectResult<Nullable<long>> GetNextCariKod()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<long>>("GetNextCariKod");
        }
    
        public virtual ObjectResult<Nullable<long>> GetNextFisNumarasi()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<long>>("GetNextFisNumarasi");
        }
    
        public virtual int GetParcaOrtalamaMaliyet(string parcaNo, ObjectParameter toplamFiyat, ObjectParameter toplamAdet, ObjectParameter ortalamaMaliyet)
        {
            var parcaNoParameter = parcaNo != null ?
                new ObjectParameter("ParcaNo", parcaNo) :
                new ObjectParameter("ParcaNo", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetParcaOrtalamaMaliyet", parcaNoParameter, toplamFiyat, toplamAdet, ortalamaMaliyet);
        }
    
        public virtual int GetParcaTipiIrsaliyeDurum(string parcaTipi)
        {
            var parcaTipiParameter = parcaTipi != null ?
                new ObjectParameter("ParcaTipi", parcaTipi) :
                new ObjectParameter("ParcaTipi", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetParcaTipiIrsaliyeDurum", parcaTipiParameter);
        }
    
        public virtual int GetWorkOrderSystemNumber(Nullable<int> taskId, ObjectParameter isEmriNumarasi)
        {
            var taskIdParameter = taskId.HasValue ?
                new ObjectParameter("TaskId", taskId) :
                new ObjectParameter("TaskId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetWorkOrderSystemNumber", taskIdParameter, isEmriNumarasi);
        }
    
        public virtual int IrsaliyeIptalEdilebilirMi(Nullable<int> irsaliyeNo, ObjectParameter durum)
        {
            var irsaliyeNoParameter = irsaliyeNo.HasValue ?
                new ObjectParameter("IrsaliyeNo", irsaliyeNo) :
                new ObjectParameter("IrsaliyeNo", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("IrsaliyeIptalEdilebilirMi", irsaliyeNoParameter, durum);
        }
    }
}
