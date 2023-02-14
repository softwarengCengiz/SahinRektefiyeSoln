using Newtonsoft.Json;
using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Helpers;
using SahinRektefiyeSoln.Helpers.LogHelper;
using SahinRektefiyeSoln.Infrastructure;
using SahinRektefiyeSoln.Models;
using SahinRektefiyeSoln.Models.HelperModels;
using SahinRektefiyeSoln.Models.ViewModels.Teklif;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vereyon.Web;

namespace SahinRektefiyeSoln.Controllers
{
    [LogActionFilter]

    public class TeklifController : BaseController
    {
        public string currentUser
        {
            get
            {
                return Session["UserName"].ToString();
            }
        }

        SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();

        public ActionResult Index()
        {
            return View();
        }

        [SessionAuthorization]
        public ActionResult Teklifler()
        {
            TeklifSorgulaPageModel model = new TeklifSorgulaPageModel();
            //model.teklifler = db.teklifler.Where(x => x.IsEmriStatuId == 1).ToList(); // Açık olanlar gelsin.
            model.teklifler = db.Teklifler.OrderBy(x => x.TeklifNo).ToList();

            foreach (var statu in db.TeklifStatu.ToList())
            {
                ListModel item = new ListModel()
                {
                    Id = statu.TeklifStatuId,
                    Name = statu.Aciklamasi,
                    Value = false
                };
                model.teklifStatuleri.Add(item);
            }

            foreach (var tip in db.IsEmriTipleri.ToList())
            {
                ListModel item = new ListModel()
                {
                    Id = tip.IsEmriTipId,
                    Name = tip.Aciklamasi,
                    Value = false
                };
                model.isEmriTipleri.Add(item);
            }

            //FilltekliflerCombos();
            return View(model);
        }

        [SessionAuthorization]
        [HttpPost]
        public ActionResult Teklifler(TeklifSorgulaPageModel model)
        {
            List<Teklifler> teklifler = db.Teklifler.OrderBy(x => x.TeklifNo).ToList();

            IEnumerable<int> statuler = model.teklifStatuleri.Where(x => x.Value == true).Select(x => x.Id);
            IEnumerable<int> tipler = model.isEmriTipleri.Where(x => x.Value == true).Select(x => x.Id);

            if (statuler.Count() > 0)
            {
                teklifler = teklifler.Where(x => statuler.Contains(x.TeklifStatuId)).ToList();
            }

            if (tipler.Count() > 0)
            {
                teklifler = teklifler.Where(x => tipler.Contains(x.IsEmriTipId)).ToList();
            }

            if (!string.IsNullOrEmpty(model.filter.Arac.Plaka))
            {
                teklifler = teklifler.Where(x => x.Arac.Plaka.Trim().Contains(model.filter.Arac.Plaka.Trim())).ToList();
            }

            if (model.filter.TeklifNo != 0)
            {
                teklifler = teklifler.Where(x => x.TeklifNo == model.filter.TeklifNo).ToList();
            }

            if (model.TeklifTarihi != null)
            {
                teklifler = teklifler.Where(x => x.TeklifTarihi == model.TeklifTarihi).ToList();
            }

            model.teklifler = teklifler;

            FillIsEmriCombos();
            return View(model);
        }


        [SessionAuthorization]
        public ActionResult TeklifOlustur()
        {
            TeklifOlusturPageModel model = new TeklifOlusturPageModel();

            //if (!RevmerHelper.IsNullOrZero(MusteriId))
            //{
            //    //Musteri ile gelmiştir.
            //    model.musteri = db.Musteri.Find(MusteriId);
            //}

            //if (!RevmerHelper.IsNullOrZero(AracId))
            //{
            //    //Araç ile gelmiştir.
            //    model.arac = db.Arac.Find(AracId);
            //    model.Km = model.arac.Km == null ? 0 : (int)model.arac.Km;
            //}

            FillIsEmriCombos();
            return View(model);
        }

        public void FillIsEmriCombos()
        {
            ViewBag.IsEmriTipleri = new SelectList(db.IsEmriTipleri.ToList(), "IsEmriTipId", "Aciklamasi");
        }

        [HttpPost]
        [SessionAuthorization]
        public ActionResult TeklifOlustur(TeklifOlusturPageModel model)
        {
            try
            {
                Teklifler yeniTeklifler = new Teklifler();

                using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        Arac isEmriAraci = new Arac();
                        if (!string.IsNullOrEmpty(model.arac.Plaka))
                        {
                            isEmriAraci = context.Arac.Where(x => x.Plaka == model.arac.Plaka).FirstOrDefault();
                        }
                        else
                            isEmriAraci = context.Arac.Find(model.arac.AracId);

                        context.SaveChanges();

                        yeniTeklifler.TeklifId = Guid.NewGuid();

                        yeniTeklifler.AracId = isEmriAraci.AracId;
                        yeniTeklifler.MusteriId = model.VehicleMusteriId;
                        yeniTeklifler.IsEmriTipId = model.IsEmriTipId;

                        yeniTeklifler.TeklifTarihi = System.DateTime.Now;

                        yeniTeklifler.DtCreated = System.DateTime.Now;
                        yeniTeklifler.Creator = currentUser;

                        yeniTeklifler.TeklifStatuId = 1; // Açık Teklif

                        context.Teklifler.Add(yeniTeklifler);
                        context.SaveChanges();

                        FlashMessage.Confirmation("Teklif başarıyla oluşturuldu.");
                        transaction.Commit();
                    }
                }
                // silinecek
                return RedirectToAction("TeklifDuzenle", "Teklif", new { TeklifId = yeniTeklifler.TeklifId });


            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				ExceptionLog log = new ExceptionLog();
                log.Message = JsonConvert.SerializeObject(ex);

                SFHelper.MailGonder(log, "Teklif olusturulurken hata");

                FlashMessage.Warning("Teklif oluşturulurken hata oluştu!");
            }

            return View();
        }

        [SessionAuthorization]
        public ActionResult TeklifDuzenle(Guid TeklifId)
        {
            TeklifDuzenlePageModel model = new TeklifDuzenlePageModel();

            model.teklifler = db.Teklifler.Find(TeklifId);

            CalculateFaturaHelper helper = new CalculateFaturaHelper();
            model.teklifKalemleri = helper.Hesapla(model.teklifler.TeklifKalemleri.ToList());
            ////Indirimsiz Tutarı Bul

            //decimal? indirimsizTutar = 0;

            //List<IsEmriKalemleri> parcalar = model.isEmri.IsEmriKalemleri.Where(x => x.KalepTipi == "P").ToList();
            //foreach (var parca in parcalar)
            //{
            //    indirimsizTutar += parca.BirimSaatAdet * parca.StokKarti.SatisBirimFiyati;
            //}

            //List<IsEmriKalemleri> iscilikler = model.isEmri.IsEmriKalemleri.Where(x => x.KalepTipi == "I").ToList();

            //foreach (var iscilik in iscilikler)
            //{
            //    indirimsizTutar += iscilik.BirimSaatAdet * iscilik.Iscilikler.IscilikTipleri.Fiyat * iscilik.Iscilikler.BirimSaat;
            //}

            //ViewBag.IndirimsizTutar = indirimsizTutar;

            /////ViewBag.IsReadOnly = CheckMyRole("Revmer Crm Rolü");

            return View(model);
        }

        public ActionResult TeklifNotEkle(Guid TeklifId, Teklifler teklifler)
        {

            //if (CheckMyRole("Revmer Crm Rolü"))
            //{
            //    FlashMessage.Warning("Bu işlemi yapmak için yetkiniz yoktur. -Revmer Crm Rolü");
            //    return RedirectToAction("IsEmriDuzenle", "IsEmri", new { IsEmriId = IsEmriId });
            //}

            Teklifler model = db.Teklifler.Find(TeklifId);
            try
            {
                model.TeklifNot = teklifler.TeklifNot;
                model.Modifier = userName;
                model.DtModified = DateTime.Now;
                db.SaveChanges();

                FlashMessage.Confirmation("Not başarıyla eklendi.");
            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Warning("Not eklenirken hata oluştu.");
            }

            return RedirectToAction("TeklifDuzenle", "Teklif", new { TeklifId = model.TeklifId });
        }

        [SessionAuthorization]
        public ActionResult TeklifIscilikEkle(Guid TeklifId)
        {
            ViewBag.TeklifId = TeklifId;
            List<Iscilikler> model = db.Iscilikler.Where(x => x.IscilikTipleri.Aciklamasi.ToUpper() != "DYI").ToList();
            return View(model);
        }

        [SessionAuthorization]
        public ActionResult TeklifIscilikEkleme(Guid teklifId, int IscilikId, string Adet)
        {
            try
            {
                Teklifler teklif = db.Teklifler.Find(teklifId);

                if (teklif.TeklifStatuId == 1)//Açık
                {
                    Iscilikler iscilik = db.Iscilikler.Find(IscilikId);

                    TeklifKalemleri teklifKalemleri = new TeklifKalemleri();
                    teklifKalemleri.TeklifKalemId = Guid.NewGuid();
                    teklifKalemleri.KalemTipi = "I";
                    teklifKalemleri.IscilikId = IscilikId;
                    teklifKalemleri.BirimSaatAdet = (decimal)double.Parse(Adet, System.Globalization.CultureInfo.InvariantCulture);
                    teklifKalemleri.TeklifId = teklifId;
                    teklifKalemleri.IskontoOrani = 0;

                    //Kalem Tutar 
                    // İşçilikdeki birim saat * İşçilik Tipi Birim Fiyat çarpımı
                    teklifKalemleri.KalemTutar = iscilik.IscilikTipleri.Fiyat * (teklifKalemleri.BirimSaatAdet / iscilik.BirimSaat); //yeniKalem.BirimSaatAdet * iscilik.BirimSaat * (decimal)iscilik.IscilikTipleri.Fiyat;
                    teklifKalemleri.KDVliKalemTutar = teklifKalemleri.KalemTutar * 118 / 100;

                    db.TeklifKalemleri.Add(teklifKalemleri);
                    db.SaveChanges();

                    FlashMessage.Confirmation("İşçilik başarıyla eklendi.");
                    return Json(new { result = 1, message = "Başarılı." });
                }
                else
                {
                    FlashMessage.Warning("Sadece açık teklife işçilik eklenebilir.");
                    return Json(new { result = 0, message = "Başarısız." });
                }

            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Warning("İşçilik eklenirken hata oluştu.");
                return Json(new { result = 0, message = "Başarısız." });
            }

            //return RedirectToAction("IsEmriDuzenle", "IsEmri", new { IsEmriId = IsEmriId });
        }

        [SessionAuthorization]
        public ActionResult TeklifSeciliParcalariSil(Guid teklifId, List<Guid> kalemIds)
        {
            try
            {
                Teklifler teklif = db.Teklifler.Find(teklifId);
                if (teklif.TeklifStatuId == Parameters.TeklifAcikStatu)
                {
                    string Message = string.Empty;
                    using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            foreach (var silinecekKalemId in kalemIds)
                            {
                                TeklifKalemleri kalem = context.TeklifKalemleri.Where(x => x.TeklifKalemId == silinecekKalemId).FirstOrDefault();
                                context.TeklifKalemleri.Remove(kalem);

                            }
                            context.SaveChanges();

                            transaction.Commit();

                            FlashMessage.Confirmation("Seçili tüm parçalar başarıyla silindi.");
                        }
                    }
                    return Json(new { result = 1, message = Message });
                }
                else
                {
                    return Json(new { result = 0, message = "Sadece açık teklifden parça silinebilir." });
                }

            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				return Json(new { result = 0, message = "Hata Oluştu." });
            }

        }

        [SessionAuthorization]
        public ActionResult TeklifKalemSil(Guid TeklifKalemId)
        {
            Guid TeklifId = new Guid();

            try
            {
                TeklifKalemleri kalem = db.TeklifKalemleri.Find(TeklifKalemId);
                TeklifId = kalem.TeklifId;

                Teklifler teklif = db.Teklifler.Find(TeklifId);

                if (teklif.TeklifStatuId == Parameters.TeklifAcikStatu)
                {
                    db.TeklifKalemleri.Remove(kalem);
                    db.SaveChanges();
                    FlashMessage.Confirmation("Kalem başarıyla silindi.");
                }
                else
                {
                    FlashMessage.Warning("Sadece açık teklifden kalem silinebilir.");
                }

            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Warning("Kalem silinirken hata oluştu.");
            }

            return RedirectToAction("TeklifDuzenle", "Teklif", new { TeklifId = TeklifId });
        }


        [SessionAuthorization]
        public ActionResult TeklifGenelIskontoUygula(Guid teklifId, int parcaIskonto, int iscilikIskonto)
        {
            try
            {
                Teklifler teklif = db.Teklifler.Find(teklifId);

                if (teklif.TeklifStatuId == Parameters.TeklifAcikStatu)
                {
                    using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            List<TeklifKalemleri> kalemler = context.TeklifKalemleri.Where(x => x.TeklifId == teklifId).ToList();

                            if (kalemler.Count > 0)
                            {
                                foreach (var kalem in kalemler)
                                {
                                    if (kalem.KalemTipi == "P")
                                    {
                                        kalem.IskontoOrani = parcaIskonto;

                                        decimal? ToplamIndirim = (kalem.BirimSaatAdet * kalem.StokKarti.SatisBirimFiyati) * (kalem.IskontoOrani / 100);

                                        kalem.KalemTutar = (kalem.BirimSaatAdet * kalem.StokKarti.SatisBirimFiyati) - ToplamIndirim;

                                        decimal? Toplamkdv = kalem.KalemTutar * ((decimal)kalem.StokKarti.Kdv / 100);
                                        kalem.KDVliKalemTutar = kalem.KalemTutar + Toplamkdv;
                                        context.SaveChanges();

                                    }
                                    else if (kalem.KalemTipi == "I")
                                    {
                                        kalem.IskontoOrani = iscilikIskonto;
                                        if (kalem.Iscilikler.IscilikTipleri.Aciklamasi.ToUpper().Contains("DYI"))
                                        {
                                            //kalem.KalemTutar = kalem.DyiIlkSatisFiyati - (kalem.DyiIlkSatisFiyati * kalem.IskontoOrani / 100); //todo
                                            decimal dyiToplamKdv = (decimal)kalem.KalemTutar * (decimal)(0.18);

                                            kalem.KDVliKalemTutar = kalem.KalemTutar + dyiToplamKdv;
                                        }
                                        else
                                        {
                                            decimal? ToplamIndirim = (kalem.BirimSaatAdet * kalem.Iscilikler.BirimSaat * kalem.Iscilikler.IscilikTipleri.Fiyat) * (kalem.IskontoOrani / 100);

                                            kalem.KalemTutar = (kalem.BirimSaatAdet * kalem.Iscilikler.BirimSaat * kalem.Iscilikler.IscilikTipleri.Fiyat) - ToplamIndirim;

                                            decimal? Toplamkdv = kalem.KalemTutar * (decimal)(0.18);

                                            kalem.KDVliKalemTutar = kalem.KalemTutar + Toplamkdv;
                                        }

                                        context.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                FlashMessage.Warning("İskonto uygulanacak parça işçilik bulunamadı.");
                            }

                            transaction.Commit();
                        }
                    }
                    FlashMessage.Confirmation("Genel iskonto başarıyla uygulandı.");
                }
                else
                {
                    FlashMessage.Warning("Sadece açık tekliflere iskonto uygulanabilir.");
                }

            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Warning("Genel iskonto uygulanırken hata oluştu.");
            }

            return RedirectToAction("TeklifDuzenle", "Teklif", new { TeklifId = teklifId });
        }

        [SessionAuthorization]
        public ActionResult TeklifKalemineIskontoUygula(Guid kalemIskontoIsEmriId, int kalemIskontoOrani)
        {
            Guid TeklifId = new Guid();
            try
            {
                TeklifKalemleri kalem = db.TeklifKalemleri.Find(kalemIskontoIsEmriId);
                TeklifId = kalem.TeklifId;

                Teklifler teklifler = db.Teklifler.Find(TeklifId);

                if (teklifler.TeklifStatuId == Parameters.TeklifAcikStatu)
                {
                    if (kalem.KalemTipi == "P")
                    {
                        kalem.IskontoOrani = kalemIskontoOrani;

                        decimal? ToplamIndirim = (kalem.BirimSaatAdet * kalem.StokKarti.SatisBirimFiyati) * (kalem.IskontoOrani / 100);

                        kalem.KalemTutar = (kalem.BirimSaatAdet * kalem.StokKarti.SatisBirimFiyati) - ToplamIndirim;

                        decimal? Toplamkdv = kalem.KalemTutar * ((decimal)kalem.StokKarti.Kdv / 100);
                        kalem.KDVliKalemTutar = kalem.KalemTutar + Toplamkdv;

                        db.SaveChanges();

                        FlashMessage.Confirmation("Parça'ya başarıyla indirim uygulandı.");
                    }
                    else if (kalem.KalemTipi == "I")
                    {
                        kalem.IskontoOrani = kalemIskontoOrani;


                        if (kalem.Iscilikler.IscilikTipleri.Aciklamasi.ToUpper().Contains("DYI"))
                        {
                            //  kalem.KalemTutar = kalem.DyiIlkSatisFiyati - (kalem.DyiIlkSatisFiyati * kalem.IskontoOrani / 100);
                            decimal dyiToplamKdv = (decimal)kalem.KalemTutar * (decimal)(0.18);

                            kalem.KDVliKalemTutar = kalem.KalemTutar + dyiToplamKdv;
                        }
                        else
                        {
                            decimal? ToplamIndirim = (kalem.BirimSaatAdet * kalem.Iscilikler.BirimSaat * kalem.Iscilikler.IscilikTipleri.Fiyat) * (kalem.IskontoOrani / 100);

                            kalem.KalemTutar = (kalem.BirimSaatAdet * kalem.Iscilikler.BirimSaat * kalem.Iscilikler.IscilikTipleri.Fiyat) - ToplamIndirim;

                            decimal? Toplamkdv = kalem.KalemTutar * (decimal)(0.18);

                            kalem.KDVliKalemTutar = kalem.KalemTutar + Toplamkdv;
                        }

                        db.SaveChanges();

                        FlashMessage.Confirmation("İşçilik'e başarıyla indirim uygulandı.");
                    }
                }
                else
                {
                    FlashMessage.Warning("Sadece açık iş emrine iskonto uygulanabilir.");
                }

            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Warning("İndirim yapılırken hata oluştu.");
            }

            return RedirectToAction("TeklifDuzenle", "Teklif", new { TeklifId = TeklifId });
        }

        [SessionAuthorization]
        public ActionResult TeklifStokKartEkle(Guid TeklifId)
        {
            ViewBag.TeklifId = TeklifId;
            return View(db.StokKarti.ToList());
        }

        [SessionAuthorization]
        public ActionResult TeklifParcaEkleme(Guid TeklifId, string StokKartId, string parcaAdet)
        {
            try
            {
                int stokKartId = Convert.ToInt32(StokKartId);
                Teklifler teklif = db.Teklifler.Find(TeklifId);

                if (teklif.TeklifStatuId == Parameters.TeklifAcikStatu)
                {
                    StokKarti parca = db.StokKarti.Find(stokKartId);

                    List<TeklifKalemleri> kalemler = db.TeklifKalemleri.Where(x => x.TeklifId == TeklifId).ToList();

                    if (kalemler.Select(x => x.StokKartId).Contains(stokKartId))
                    {
                        TeklifKalemleri kalem = kalemler.Where(x => x.StokKartId == stokKartId).FirstOrDefault();

                        kalem.BirimSaatAdet = kalem.BirimSaatAdet + (decimal)double.Parse(parcaAdet, System.Globalization.CultureInfo.InvariantCulture);

                        kalem.KalemTutar = kalem.BirimSaatAdet * parca.SatisBirimFiyati;

                        decimal? toplamKdv = kalem.KalemTutar * (decimal)(parca.Kdv) / 100;

                        kalem.KDVliKalemTutar = kalem.KalemTutar + toplamKdv;
                    }
                    else
                    {
                        TeklifKalemleri yeniKalem = new TeklifKalemleri();
                        yeniKalem.TeklifKalemId = Guid.NewGuid();
                        yeniKalem.KalemTipi = "P";
                        yeniKalem.StokKartId = stokKartId;
                        yeniKalem.BirimSaatAdet = (decimal)double.Parse(parcaAdet, System.Globalization.CultureInfo.InvariantCulture);
                        yeniKalem.TeklifId = TeklifId;
                        yeniKalem.IskontoOrani = 0;
                        //yeniKalem.BirimIndirimFiyat = 0;

                        //Kalem Tutar 
                        // İşçilikdeki birim saat * İşçilik Tipi Birim Fiyat çarpımı
                        yeniKalem.KalemTutar = yeniKalem.BirimSaatAdet * parca.SatisBirimFiyati;

                        decimal? toplamKdv = yeniKalem.KalemTutar * (decimal)(parca.Kdv) / 100;

                        yeniKalem.KDVliKalemTutar = yeniKalem.KalemTutar + toplamKdv;

                        db.TeklifKalemleri.Add(yeniKalem);
                    }


                    db.SaveChanges();

                    //FlashMessage.Confirmation("Parça başarıyla eklendi.");
                    return Json(new { result = 1, message = "Başarılı." });
                }
                else
                {
                    // Eklenemez 
                    return Json(new { result = 0, message = "Sadece Açık Teklife Kalem Eklenebilir." });
                }


            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				//FlashMessage.Warning("Parça eklenirken hata oluştu.");
				return Json(new { result = 0, message = "Hata oluştu." });
            }

        }

        [SessionAuthorization]
        public ActionResult TeklifPaketEkle(Guid TeklifId, int CompanyId)
        {
            ViewBag.TeklifId = TeklifId;
            return View(db.Paketler.Where(x => x.CompanyId == CompanyId).ToList());
        }

        public ActionResult TeklifPaketEkleme(Guid TeklifId, int PaketId)
        {
            try
            {
                Teklifler teklif = db.Teklifler.Find(TeklifId);

                if (teklif.TeklifStatuId == Parameters.TeklifAcikStatu)
                {
                    Paketler paket = db.Paketler.Find(PaketId);

                    using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            foreach (var paketIscilik in paket.PaketIscilikleri)
                            {
                                TeklifKalemleri yeniKalem = new TeklifKalemleri();
                                yeniKalem.TeklifKalemId = Guid.NewGuid();
                                yeniKalem.KalemTipi = "I";
                                yeniKalem.IscilikId = paketIscilik.IscilikId;
                                yeniKalem.BirimSaatAdet = paketIscilik.Iscilikler.BirimSaat;
                                yeniKalem.TeklifId = TeklifId;
                                yeniKalem.IskontoOrani = 0;
                                //yeniKalem.SwAtolyeTeslim = true;
                                //yeniKalem.BirimIndirimFiyat = 0;

                                //Kalem Tutar 
                                // İşçilikdeki birim saat * İşçilik Tipi Birim Fiyat çarpımı
                                yeniKalem.KalemTutar = paketIscilik.Iscilikler.BirimSaat * (decimal)paketIscilik.Iscilikler.IscilikTipleri.Fiyat;
                                yeniKalem.KDVliKalemTutar = yeniKalem.KalemTutar * 118 / 100;

                                context.TeklifKalemleri.Add(yeniKalem);
                                context.SaveChanges();

                            }
                            foreach (var paketParcasi in paket.PaketStokKartlari)
                            {
                                //Bu parça önceden ekli miydi ? 
                                if (context.TeklifKalemleri.Where(x => x.TeklifId == TeklifId).Select(x => x.StokKartId).Contains(paketParcasi.StokKartId))
                                {
                                    // Evet bu parça önceden varmış

                                    TeklifKalemleri kalem = context.TeklifKalemleri.Where(x => x.StokKartId == paketParcasi.StokKartId).FirstOrDefault();

                                    kalem.BirimSaatAdet = kalem.BirimSaatAdet + paketParcasi.Adet;

                                    kalem.KalemTutar = kalem.BirimSaatAdet * kalem.StokKarti.SatisBirimFiyati;

                                    decimal? toplamKdv = kalem.KalemTutar * (decimal)(kalem.StokKarti.Kdv) / 100;

                                    kalem.KDVliKalemTutar = kalem.KalemTutar + toplamKdv;

                                    context.SaveChanges();

                                }
                                else
                                {
                                    //Hayır Bu parça önceden kalmlere eklenmemiş

                                    TeklifKalemleri yeniKalem = new TeklifKalemleri();
                                    yeniKalem.KalemTipi = "P";
                                    yeniKalem.StokKartId = paketParcasi.StokKartId;
                                    yeniKalem.BirimSaatAdet = paketParcasi.Adet;
                                    yeniKalem.TeklifId = TeklifId;
                                    yeniKalem.IskontoOrani = 0;
                                    yeniKalem.TeklifKalemId = Guid.NewGuid();

                                    //Kalem Tutar 
                                    // İşçilikdeki birim saat * İşçilik Tipi Birim Fiyat çarpımı
                                    yeniKalem.KalemTutar = paketParcasi.Adet * (decimal)paketParcasi.StokKarti.SatisBirimFiyati;
                                    decimal? toplamKdv = yeniKalem.KalemTutar * (decimal)(paketParcasi.StokKarti.Kdv) / 100;

                                    yeniKalem.KDVliKalemTutar = yeniKalem.KalemTutar + toplamKdv;

                                    context.TeklifKalemleri.Add(yeniKalem);
                                    context.SaveChanges();

                                }

                            }

                            transaction.Commit();
                        }
                    }

                    FlashMessage.Confirmation("Paket başarıyla eklendi.");
                }
                else
                {
                    FlashMessage.Warning("Sadece açık teklife paket eklenebilir.");
                }

            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Warning("Paket eklenirken hata oluştu.");
            }

            return RedirectToAction("TeklifDuzenle", "Teklif", new { TeklifId = TeklifId });
        }

        [SessionAuthorization]
        public ActionResult TeklifDyiEkle(Guid teklifId)
        {
            ViewBag.TeklifId = teklifId;

            List<Iscilikler> model = db.Iscilikler.Where(x => x.IscilikTipleri.Aciklamasi.ToUpper().Contains("DYI")).ToList();

            return View(model);
        }

        [SessionAuthorization]
        [HttpPost]
        public ActionResult TeklifDyiEkleme(int iscilikId, Guid teklifId, string maliyet, string satisFiyati)
        {
            decimal Maliyet = (decimal)double.Parse(maliyet, System.Globalization.CultureInfo.InvariantCulture);
            decimal SatisFiyati = (decimal)double.Parse(satisFiyati, System.Globalization.CultureInfo.InvariantCulture);

            try
            {
                Teklifler teklif = db.Teklifler.Find(teklifId);

                if (teklif.TeklifStatuId == Parameters.TeklifAcikStatu)
                {
                    Iscilikler iscilik = db.Iscilikler.Find(iscilikId);

                    TeklifKalemleri yeniKalem = new TeklifKalemleri();
                    yeniKalem.TeklifKalemId = Guid.NewGuid();
                    yeniKalem.KalemTipi = "I";
                    yeniKalem.IscilikId = iscilikId;
                    yeniKalem.BirimSaatAdet = 1;//iscilik.BirimSaat;
                    yeniKalem.TeklifId = teklifId;
                    yeniKalem.IskontoOrani = 0;

                    //Kalem Tutar 
                    // İşçilikdeki birim saat * İşçilik Tipi Birim Fiyat çarpımı
                    yeniKalem.KalemTutar = SatisFiyati;
                    yeniKalem.DyiIlkSatisFiyati = yeniKalem.KalemTutar;
                    yeniKalem.KDVliKalemTutar = yeniKalem.KalemTutar * 118 / 100;
                    yeniKalem.MaliyetDyi = Maliyet;

                    db.TeklifKalemleri.Add(yeniKalem);
                    db.SaveChanges();

                    FlashMessage.Confirmation("İşçilik başarıyla eklendi.");
                    return Json(new { result = 1, message = "Başarılı." });
                }
                else
                {
                    FlashMessage.Warning("Sadece açık teklife işçilik eklenebilir.");
                    return Json(new { result = 0, message = "Başarısız." });
                }

            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Warning("İşçilik eklenirken hata oluştu.");
                return Json(new { result = 0, message = "Başarısız." });
            }

        }

        [SessionAuthorization]
        [HttpPost]
        public ActionResult TeklifIptalEt(Guid TeklifId, string teklifIptalAciklamasi)
        {
            try
            {
                Teklifler tekliflerr = db.Teklifler.Find(TeklifId);

                if (tekliflerr.TeklifStatuId == Parameters.TeklifAcikStatu)
                {
                    using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities
                        ())
                    {

                        Teklifler teklifler = context.Teklifler.Find(TeklifId);

                        teklifler.TeklifStatuId = Parameters.TeklifIptalStatu;
                        teklifler.IptalNedeni = teklifIptalAciklamasi;
                        teklifler.IptalEdenKisi = currentUser;
                        teklifler.IptalTarihi = DateTime.Now;
                        teklifler.Modifier = userName;
                        teklifler.DtModified = DateTime.Now;

                        context.Entry(teklifler).State = EntityState.Modified;

                        context.SaveChanges();

                        FlashMessage.Confirmation("Teklif başarıyla iptal edilmiştir.");
                    }

                }
                else
                {
                    FlashMessage.Warning("Sadece açık teklif iptal edilebilir.");
                }

            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Warning("Teklif iptal edilirken hata oluştu." + ex.Message + ex.InnerException.Message + ex.InnerException.StackTrace);
            }
            return RedirectToAction("TeklifDuzenle", "Teklif", new { TeklifId = TeklifId });
        }

        [SessionAuthorization]
        public ActionResult TeklifIsEmriDonustur(Guid teklifId)
        {
            ViewBag.TeklifId = teklifId;

            ViewBag.Danismanlar = new SelectList(db.Danismanlar.ToList(), "DanismanId", "Aciklamasi");
            ViewBag.Danismanlar = new SelectList(db.Danismanlar.Where(x => x.SwAktif == true).OrderBy(x => x.Adi).Select(x => new { DanismanId = x.DanismanId, DanismanAdi = x.Adi + " " + x.Soyadi }).ToList(), "DanismanId", "DanismanAdi");
            TeklifIsEmriDonusturPageModel model = new TeklifIsEmriDonusturPageModel();

            model.TeklifId = teklifId;


            return View(model);
        }

        [SessionAuthorization]
        [HttpPost]
        public ActionResult TeklifiIsEmrineDonustur(TeklifIsEmriDonusturPageModel model)
        {
            int IsEmriId = 0;
            try
            {
                Teklifler tekliflerr = db.Teklifler.Find(model.TeklifId);

                if (tekliflerr.TeklifStatuId == Parameters.TeklifAcikStatu)
                {
                    using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities
                        ())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            IsEmirleri yeniIsEmri = new IsEmirleri();

                            yeniIsEmri.AracId = tekliflerr.AracId;
                            yeniIsEmri.MusteriId = tekliflerr.MusteriId;
                            yeniIsEmri.IsEmriTipId = tekliflerr.IsEmriTipId;
                            yeniIsEmri.Notlar = tekliflerr.TeklifNot;

                            /*  yeniIsEmri.SwVip = model.isEmri.SwVip;
                              yeniIsEmri.SwInteraktif = model.isEmri.SwInteraktif;
                              yeniIsEmri.SwTekrarlananIs = false;*/

                            yeniIsEmri.DanismanId = model.DanismanId;
                            yeniIsEmri.TahminiTeslimTarihi = model.TahminiTeslimTarihi;
                            yeniIsEmri.Km = model.KM;

                            yeniIsEmri.IsEmriTeklifStatusu = (int)IsEmriTeklifStatu.TeklifIletildi;

                            yeniIsEmri.IsEmriTarihi = System.DateTime.Now;
                            yeniIsEmri.CreatedDate = System.DateTime.Now;
                            yeniIsEmri.Creator = currentUser;

                            yeniIsEmri.IsEmriStatuId = Parameters.IsEmriAcikStatu; // Açık İş Emri

                            context.IsEmirleri.Add(yeniIsEmri);
                            context.SaveChanges();

                            IsEmriId = yeniIsEmri.IsEmriId;

                            List<TeklifKalemleri> teklifKalemleri = context.TeklifKalemleri.Where(x => x.TeklifId == model.TeklifId).ToList();

                            foreach (var item in teklifKalemleri)
                            {
                                IsEmriKalemleri isEmriKalemleri = new IsEmriKalemleri();
                                isEmriKalemleri.DyiIlkSatisFiyati = item.DyiIlkSatisFiyati;
                                isEmriKalemleri.BirimIndirimFiyat = item.BirimIndirimFiyat;
                                isEmriKalemleri.BirimSaatAdet = item.BirimSaatAdet;
                                isEmriKalemleri.IscilikId = item.IscilikId;
                                isEmriKalemleri.IsEmriId = IsEmriId;
                                isEmriKalemleri.IskontoOrani = item.IskontoOrani;
                                isEmriKalemleri.KalepTipi = item.KalemTipi;
                                isEmriKalemleri.KalemTutar = item.KalemTutar;
                                isEmriKalemleri.KdvliKalemTutar = item.KDVliKalemTutar;
                                isEmriKalemleri.MaliyetDyi = item.MaliyetDyi;
                                isEmriKalemleri.StokKartId = item.StokKartId;

                                isEmriKalemleri.SwAtolyeTeslim = false;
                                isEmriKalemleri.TeslimEdilenMiktar = 0;

                                isEmriKalemleri.CreatedDate = DateTime.Now;
                                isEmriKalemleri.Creator = userName;

                                context.IsEmriKalemleri.Add(isEmriKalemleri);
                                context.SaveChanges();
                            }


                            Teklifler teklif = context.Teklifler.Find(model.TeklifId);
                            teklif.IsEmriId = IsEmriId;
                            teklif.TeklifStatuId = Parameters.TeklifeDonusturuldu;
                            teklif.Modifier = userName;
                            teklif.DtModified = DateTime.Now;

                            context.SaveChanges();

                            transaction.Commit();

                            FlashMessage.Confirmation("Teklif başarıyla iş emrine dönüştürülmüştür.");
                        }
                    }
                }
                else
                {
                    FlashMessage.Warning("Sadece açık teklif iş emrine dönüştürülebilir.");
                }
            }
            catch (Exception ex)
            {
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Warning("Teklif iş emrine dönüştürülürken hata oluştu." + ex.Message + ex.InnerException.Message + ex.InnerException.StackTrace);
            }
            return RedirectToAction("IsEmriDuzenle", "IsEmri", new { IsEmriId = IsEmriId });
        }

        public ActionResult TeklifPrintView(Guid TeklifId)
        {
            Teklifler teklif = db.Teklifler.Find(TeklifId);

            CalculateFaturaHelper helper = new CalculateFaturaHelper();
            List<FaturaKalemTeklif> teklifKalemleri = helper.Hesapla(teklif.TeklifKalemleri.ToList());

            TeklifDuzenlePageModel teklifDuzenlePageModel = new TeklifDuzenlePageModel();
            teklifDuzenlePageModel.teklifKalemleri = teklifKalemleri;
            teklifDuzenlePageModel.teklifler = teklif;

            return View(teklifDuzenlePageModel);

        }
    }
}