﻿using DocumentFormat.OpenXml.Office2010.Excel;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using PdfSharp.Pdf.IO;
using Remotion.Reflection;
using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Helpers;
using SahinRektefiyeSoln.Helpers.Enums;
using SahinRektefiyeSoln.Infrastructure;
using SahinRektefiyeSoln.Models;
using SahinRektefiyeSoln.Models.ViewModels.IsEmri;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static SahinRektefiyeSoln.Helpers.SFHelper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SahinRektefiyeSoln.Controllers
{
    [LogActionFilter]
    public class TicketController : BaseController
    {
        SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();

        public string currentUser
        {
            get
            {
                return Session["UserName"].ToString();
            }
        }
        [SessionAuthorization]
        public ActionResult Open()
        {
            FillIsEmriCombos();
            return View();
        }


        [SessionAuthorization]
        public ActionResult Tickets()
        {
            var talepler = db.Talepler.ToList().OrderByDescending(x => x.CreatedDate);
            var model = new List<TicketListModel>();
            var isDanisman = SFHelper.CheckMyRole(currentUser, "DANISMAN");
            ViewBag.MusteriKabul = SFHelper.CheckMyRole(currentUser, "MUSTERIKABUL");
            bool tempBool = false;
            foreach (var item in talepler)
            {
                var talepDetay = db.TalepDetay.Where(x => x.TalepId == item.TalepId).FirstOrDefault();
                if (talepDetay != null)
                {
                    tempBool = true;
                }
                model.Add(new TicketListModel
                {
                    AtananKisi = item.AtananSofor,
                    Musteri = item.Musteri.MusteriTipi == "B" ? (item.Musteri.MusteriAdi.ToString() + " " + item.Musteri.MusteriSoyadi.ToString()) : item.Musteri.KurumAdi.ToString(),
                    MusteriAramaTarihi = item.MusteriAramaTarihi,
                    OlustuanKisi = item.Creator,
                    OlusturmaTarihi = item.CreatedDate,
                    TalepNo = item.TalepId,
                    Durum = item.Durum != null ? ((TicketStatus)item.Durum).GetTicketStatusText() : TicketStatus.TicketOpened.GetTicketStatusText(),
                    IsDetailAvailable = tempBool,
                    MusteriAtolyeGelisTarihi = item.MusteriAtolyeGelisTarihi
                });
            }
            ViewBag.CanEdit = SFHelper.CheckMyRole(currentUser, "ADMIN");

            if (isDanisman)
                model = model.Where(x => x.AtananKisi == currentUser).ToList();

            return View(model);
        }

        [SessionAuthorization]
        public ActionResult Edit(int id)
        {
            var talepler = db.Talepler.FirstOrDefault(x => x.TalepId == id);
            TicketOpenViewModel model = new TicketOpenViewModel();
            if (talepler != null)
            {
                model.HizliIsEmriVehicleMusteriId = talepler.MusteriId;
                model.MusteriDesc = talepler.Musteri.MusteriTipi == "B" ? (talepler.Musteri.MusteriAdi.ToString() + " " + talepler.Musteri.MusteriSoyadi.ToString()) : talepler.Musteri.KurumAdi.ToString();
                model.MusteriTipi = talepler.Musteri.MusteriTipi;
                model.MusteriAdi = talepler.Musteri.MusteriAdi;
                model.MusteriSoyadi = talepler.Musteri.MusteriSoyadi;
                model.Telefon = talepler.Musteri.MusteriTelefon.FirstOrDefault().TelefonNumarasi;
                model.Adres = talepler.Musteri.Adres;
                model.ArayanKisiAdSoyad = talepler.ArayanKisi;
                model.KayitTarihi = talepler.CreatedDate;
                model.MusteriAramaTarihi = talepler.MusteriAramaTarihi;
                model.Not = talepler.Note;
                model.SoforUserName = talepler.AtananSofor;
                model.Plate = talepler.Plate;
                model.KM = talepler.Km.HasValue ? talepler.Km.Value : 0;
                model.SaseNo = talepler.VinNo;
                model.SoforUserName = talepler.AtananSofor;
                model.PartId = (int)talepler.PartId;
                model.VehicleId = talepler.VehicleId.HasValue ? talepler.VehicleId.Value : 0;
                model.AracGrubuId = talepler.AracGrupId.HasValue ? talepler.AracGrupId.Value : 0;
                model.Id = talepler.TalepId;
                model.TalepSekliId = talepler.TalepSekliId.HasValue ? talepler.TalepSekliId.Value : 0;
                model.KargoyaVerilisTarihi = talepler.KargoyaVerilisTarihi;
                model.AramaTarihi = talepler.AramaTarihi;
                model.KargoFirmasi = talepler.KargoFirmasi;
                model.GönderiKodu = talepler.GönderiKodu;
                model.MusteriAtolyeGelisTarihi = talepler.MusteriAtolyeGelisTarihi;
                ViewBag.CurrentTalepSekli = model.TalepSekliId.ToString();
            }

            FillIsEmriCombos();
            return View(model);
        }

        public ActionResult DetailEdit(int id)
        {
            var talepler = db.Talepler.FirstOrDefault(x => x.TalepId == id);
            var talepDetay = db.TalepDetay.FirstOrDefault(x => x.TalepId == id);
            var talepDosya = db.TalepDosya.FirstOrDefault(x => x.TalepDosyaId == talepler.TalepDosyaId);
            TicketDetailViewModel model = new TicketDetailViewModel();
            model.TalepId = id;

            var ariza = ArizaListesi();
            var tamir = TamirListesi();

            model.KM = talepler.Km ?? 0;
            model.VinNo = talepler.VinNo ?? "";
            model.Plaka = talepler.Plate ?? "";
            model.Marka = talepler.Vehicles.Companies.Name;
            model.Model = talepler.Vehicles.Name;

            if (talepDetay != null)
            {
                model.MotorDolapNo = talepDetay.MotorDolapNo;
                model.KapakDolapNo = talepDetay.KapakDolapNo;
                model.BildirimTarihi = talepDetay.BildirimTarihi;
                model.ServisAdı = talepDetay.ServisAdı;
                model.Marka = talepDetay.Marka;
                model.Model = talepDetay.Model;
                model.MotorTipi = talepDetay.MotorTipi;
                model.YakıtTipi = talepDetay.YakıtTipi.Value;
                model.SilindirSayisi = talepDetay.SilindirSayisi.Value;
                model.Garanti = talepDetay.Garanti.Value == 0 ? false : true;
                model.Revizyon = talepDetay.Revizyon.Value == 0 ? false : true;
                model.RevizyonAciklama = talepDetay.RevizyonAciklama;
                model.ServisNo = talepDetay.ServisNo;
                model.AlınanIs = talepDetay.AlınanIs.Value;
                model.Plaka = talepDetay.Plaka;
                model.KM = talepDetay.KM.Value;
                model.VinNo = talepDetay.VinNo;
                model.MotorNo = talepDetay.MotorNo;
                model.SupapSayisi = talepDetay.SupapSayisi;
                model.MusteriNot = talepDetay.MusteriNot;
                model.ArizaDiger = talepDetay.ArizaDiger;
                model.ParcaDiger = talepDetay.ParcaDiger;
                model.ParcaAdet = talepDetay.ParcaAdet;
                model.IsLogoEnable = (int)talepDetay.IsLogoEnable;
                if (talepDosya != null)
                {
                    model.FileInputView = talepDosya.TalepDosyaUrl;
                }
                model.ParcalarText = new List<string>();
                var arizalist = talepDetay.ArizaList != null ? talepDetay.ArizaList.Split(',') : null;
                var tamirList = talepDetay.ParcaList != null ? talepDetay.ParcaList.Split(',') : null;
                talepDetay.ParcaListAdet = talepDetay.ParcaListAdet;
                //"1-1;3-5;5-3;13-9;"
                foreach (var item in ariza)
                {
                    if (arizalist != null)
                    {
                        foreach (var itemList in arizalist)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }
                var adetList = talepDetay.ParcaListAdet.Split(';');
                foreach (var item in tamir)
                {
                    if (tamirList != null)
                    {
                        foreach (var itemList in tamirList)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }

                    if (adetList.Any(x => x.Split('-')[0] == item.Value))
                        model.ParcalarText.Add(adetList.Where(x => x.Split('-')[0] == item.Value).FirstOrDefault().Split('-')[1]);
                    else
                        model.ParcalarText.Add("0");
                }
            }

            model.Ariza = ariza;
            model.Parcalar = tamir;

            return View(model);
        }


        [HttpPost]
        public ActionResult DetailEdit(TicketDetailViewModel model)
        {
            if (model.FileInput[0] != null)
            {
                var processedFileInput = ProcessFileInput(model.FileInput);

                var json = JsonConvert.SerializeObject(new
                {
                    FileInput = processedFileInput,
                    TalepId = model.TalepId
                });

                TempData["UploadModel"] = json as object;
            }

            var talepDetay = db.TalepDetay.FirstOrDefault(x => x.TalepId == model.TalepId);
            //"1-1;3-5;5-3;13-9;"
            string parcaTextMap = "";

            for (int i = 0; i < model.ParcalarText.Count; i++)
            {
                if (model.ParcalarText[i] != "0")
                {
                    parcaTextMap = parcaTextMap + model.ParcalarId[i] + "-" + model.ParcalarText[i] + ";";
                }
            }

            Talepler talep = db.Talepler.Where(x => x.TalepId == model.Id).FirstOrDefault();
            if ((talep.TalepSekliId != 3 && talep.TalepSekliId != 4) && model.FlagSave == 1)
            {
                talep.Durum = (int)TicketStatus.Received;
                db.SaveChanges();
            }

            if (talepDetay == null)
            {
                using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    //using blokları arasında transaction'ımızı açtık ve artık transaction'ımız bir commit() fonksiyonunu kullanana kadar işlem yaptığımız tabloyu kilitleyecek.
                    {
                        //2. Bilet 
                        TalepDetay yeniTalep = talepDetay ?? new TalepDetay();
                        yeniTalep.TalepId = model.TalepId;
                        yeniTalep.MotorDolapNo = model.MotorDolapNo;
                        yeniTalep.KapakDolapNo = model.KapakDolapNo;
                        yeniTalep.BildirimTarihi = model.BildirimTarihi;
                        yeniTalep.ServisAdı = model.ServisAdı;
                        yeniTalep.Marka = model.Marka;
                        yeniTalep.Model = model.Model;
                        yeniTalep.MotorTipi = model.MotorTipi;
                        yeniTalep.YakıtTipi = model.YakıtTipi;
                        yeniTalep.SilindirSayisi = model.SilindirSayisi;
                        yeniTalep.Garanti = model.Garanti == false ? 0 : 1;
                        yeniTalep.Revizyon = model.Revizyon == false ? 0 : 1;
                        yeniTalep.RevizyonAciklama = model.RevizyonAciklama;
                        yeniTalep.ServisNo = model.ServisNo;
                        yeniTalep.AlınanIs = model.AlınanIs;
                        yeniTalep.Plaka = model.Plaka;
                        yeniTalep.KM = model.KM;
                        yeniTalep.VinNo = model.VinNo;
                        yeniTalep.MotorNo = model.MotorNo;
                        yeniTalep.SupapSayisi = model.SupapSayisi;
                        yeniTalep.MusteriNot = model.MusteriNot;
                        yeniTalep.ArizaDiger = model.ArizaDiger;
                        yeniTalep.ParcaDiger = model.ParcaDiger;
                        yeniTalep.ParcaAdet = model.ParcaAdet;
                        yeniTalep.ArizaList = string.Join(",", model.ArizaChck);
                        yeniTalep.ParcaList = string.Join(",", model.ParcalarChck);
                        yeniTalep.ParcaListAdet = parcaTextMap;
                        yeniTalep.IsLogoEnable = model.IsLogoEnable;

                        context.TalepDetay.Add(yeniTalep);

                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            else
            {
                talepDetay.TalepId = model.TalepId;
                talepDetay.MotorDolapNo = model.MotorDolapNo;
                talepDetay.KapakDolapNo = model.KapakDolapNo;
                talepDetay.BildirimTarihi = model.BildirimTarihi;
                talepDetay.ServisAdı = model.ServisAdı;
                talepDetay.Marka = model.Marka;
                talepDetay.Model = model.Model;
                talepDetay.MotorTipi = model.MotorTipi;
                talepDetay.YakıtTipi = model.YakıtTipi;
                talepDetay.SilindirSayisi = model.SilindirSayisi;
                talepDetay.Garanti = model.Garanti == false ? 0 : 1;
                talepDetay.Revizyon = model.Revizyon == false ? 0 : 1;
                talepDetay.RevizyonAciklama = model.RevizyonAciklama;
                talepDetay.ServisNo = model.ServisNo;
                talepDetay.AlınanIs = model.AlınanIs;
                talepDetay.Plaka = model.Plaka;
                talepDetay.KM = model.KM;
                talepDetay.VinNo = model.VinNo;
                talepDetay.MotorNo = model.MotorNo;
                talepDetay.SupapSayisi = model.SupapSayisi;
                talepDetay.MusteriNot = model.MusteriNot;
                talepDetay.ArizaDiger = model.ArizaDiger;
                talepDetay.ParcaDiger = model.ParcaDiger;
                talepDetay.ParcaAdet = model.ParcaAdet;
                talepDetay.ArizaList = string.Join(",", model.ArizaChck);
                talepDetay.ParcaList = string.Join(",", model.ParcalarChck);
                talepDetay.ParcaListAdet = parcaTextMap;
                talepDetay.IsLogoEnable = model.IsLogoEnable;

                db.SaveChanges();
            }

            if (model.FileInput[0] != null)
            {
                return RedirectToAction("UploadFiles");
            }
            else
            {
                return RedirectToAction("Tickets");
            }

        }


        public ActionResult UploadFiles()
        {
            var json = TempData["UploadModel"].ToString();

            var data = JsonConvert.DeserializeAnonymousType(json, new
            {
                FileInput = new[]
                {
                    new
                    {
                        ContentType = "",
                        FileName = "",
                        InputStream = new byte[0],
                        ContentLength = 0
                    }
                },
                TalepId = 0
            });

            string folderPath = Server.MapPath("~/Content/TicketImages/" + data.TalepId);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            List<string> fileUrls = new List<string>();
            foreach (var file in data.FileInput)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(folderPath, fileName);
                System.IO.File.WriteAllBytes(filePath, file.InputStream);
                fileUrls.Add(fileName);
            }

            string formattedImages = string.Join(";", fileUrls);

            using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var talep = db.Talepler.FirstOrDefault(x => x.TalepId == data.TalepId);
                        if (string.IsNullOrEmpty(talep.TalepDosyaId.ToString())) //Eğer kayıt oluşturumamış ise
                        {
                            TalepDosya talepDosya = new TalepDosya();
                            talepDosya.TalepDosyaSekliId = 1;
                            talepDosya.TalepDosyaUrl = formattedImages; //Yeni dosyalar eklenir

                            context.TalepDosya.Add(talepDosya);
                            context.SaveChanges();
                            transaction.Commit();

                            talep.TalepDosyaId = talepDosya.TalepDosyaId;
                            db.SaveChanges();
                        }
                        else
                        {
                            var talepDosya = db.TalepDosya.FirstOrDefault(x => x.TalepDosyaId == talep.TalepDosyaId);
                            talepDosya.TalepDosyaUrl += ";" + formattedImages;  //Daha önceden kayıt var ise mevcut dosyaların yanına eklenir.
                            db.SaveChanges();
                            transaction.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }

            return RedirectToAction("Tickets");
        }

        [HttpPost]
        public ActionResult DeleteImage(int id, string imageName)
        {
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == id);
            var talepDosyaId = talep.TalepDosyaId;
            var talepDosya = db.TalepDosya.FirstOrDefault(x => x.TalepDosyaId == talepDosyaId);

            try
            {
                //Veritabanı güncelleniyor.
                string[] talepDosyaUrls = talepDosya.TalepDosyaUrl.Split(';');
                string newTalepDosyaUrl = "";
                foreach (var item in talepDosyaUrls)
                {
                    if (item != imageName)
                    {
                        newTalepDosyaUrl += item + ";";
                    }
                }
                // Son karakter ";" olduğu için siliniyor
                newTalepDosyaUrl = newTalepDosyaUrl.TrimEnd(';');
                talepDosya.TalepDosyaUrl = newTalepDosyaUrl;
                db.SaveChanges();

                //Klasördeki dosya siliniyor.
                string filePath = Server.MapPath("~/Content/TicketImages/" + id + "/" + imageName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                //Eğer talebe bağlı dosya kalmadıysa talepten ve talepDosyadan sil.
                if (string.IsNullOrEmpty(newTalepDosyaUrl))
                {
                    talep.TalepDosyaId = null;
                    db.TalepDosya.Remove(talepDosya);
                    db.SaveChanges();

                    string path = Server.MapPath("~/Content/TicketImages/" + id);
                    Directory.Delete(path);
                }

                return Json(true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult PrintAllEmployee()
        {
            var report = new Rotativa.ActionAsPdf("DetailEdit", new { id = 7 });
            return report;
        }


        [HttpPost]
        public ActionResult Edit(TicketOpenViewModel model)
        {
            Talepler yeniTalep = db.Talepler.Where(x => x.TalepId == model.Id).FirstOrDefault();
            var musteriModel = db.Musteri.Find(model.HizliIsEmriVehicleMusteriId);
            //2. Bilet 
            yeniTalep.MusteriId = musteriModel.MusteriId;
            yeniTalep.ArayanKisi = model.ArayanKisiAdSoyad;
            yeniTalep.MusteriAramaTarihi = model.MusteriAramaTarihi;
            yeniTalep.Note = model.Not;
            yeniTalep.AtananSofor = model.SoforUserName;
            yeniTalep.CreatedDate = model.KayitTarihi ?? System.DateTime.Now;
            yeniTalep.Creator = this.userName;
            yeniTalep.Modifier = null;
            yeniTalep.ModifiedDate = null;

            //yeniTalep.PartId = model.PartId;
            if (model.TalepSekliId != 3 && model.TalepSekliId != 4)
            {
                yeniTalep.PartId = 1;
            }

            //Araç bilgileri taşınacak. Geçici olarak 1
            yeniTalep.AracGrupId = 1;
            yeniTalep.VehicleId = 1;
            //yeniTalep.AracGrupId = model.AracGrubuId;
            //yeniTalep.VehicleId = model.VehicleId;
            //yeniTalep.Km = model.KM;
            //yeniTalep.VinNo = model.SaseNo;
            //yeniTalep.Plate = model.Plate;
            yeniTalep.Durum = (int)TicketStatus.TicketOpened;
            yeniTalep.TalepSekliId = model.TalepSekliId;
            yeniTalep.KargoyaVerilisTarihi = model.KargoyaVerilisTarihi;
            yeniTalep.AramaTarihi = model.AramaTarihi;
            yeniTalep.KargoFirmasi = model.KargoFirmasi;
            yeniTalep.GönderiKodu = model.GönderiKodu;
            yeniTalep.MusteriAtolyeGelisTarihi = model.MusteriAtolyeGelisTarihi;

            db.SaveChanges();
            FillIsEmriCombos();

            return RedirectToAction("Tickets");
        }

        [HttpPost]
        public ActionResult Open(TicketOpenViewModel model)
        {
            FillIsEmriCombos();
            using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                //using blokları arasında transaction'ımızı açtık ve artık transaction'ımız bir commit() fonksiyonunu kullanana kadar işlem yaptığımız tabloyu kilitleyecek.
                {
                    var talepSekli = db.TalepSekli.ToList();
                    db.Talepler.Where(x => x.TalepId == model.Id).FirstOrDefault();
                    var musteriModel = context.Musteri.Find(model.HizliIsEmriVehicleMusteriId);
                    //2. Bilet 
                    Talepler yeniTalep = new Talepler();
                    yeniTalep.MusteriId = musteriModel.MusteriId;
                    yeniTalep.ArayanKisi = model.ArayanKisiAdSoyad;
                    yeniTalep.MusteriAramaTarihi = model.MusteriAramaTarihi;
                    yeniTalep.Note = model.Not;
                    yeniTalep.AtananSofor = model.SoforUserName;
                    yeniTalep.CreatedDate = model.KayitTarihi ?? System.DateTime.Now;
                    yeniTalep.Creator = this.userName;
                    yeniTalep.Modifier = null;
                    yeniTalep.ModifiedDate = null;
                    //Araç bilgileri taşınacak. Geçici olarak 1
                    yeniTalep.AracGrupId = 1;
                    yeniTalep.VehicleId = 1;
                    //yeniTalep.AracGrupId = model.AracGrubuId;
                    //yeniTalep.VehicleId = model.VehicleId;
                    //yeniTalep.Km = model.KM;
                    //yeniTalep.VinNo = model.SaseNo;
                    //yeniTalep.Plate = model.Plate;
                    if (model.TalepSekliId == 3 || model.TalepSekliId == 4)
                    {
                        yeniTalep.PartId = 1;
                    }
                    else
                    {
                        yeniTalep.PartId = model.PartId;
                    }
                    yeniTalep.Durum = (int)TicketStatus.TicketOpened;
                    yeniTalep.TalepSekliId = model.TalepSekliId;
                    yeniTalep.KargoyaVerilisTarihi = model.KargoyaVerilisTarihi;
                    yeniTalep.AramaTarihi = model.AramaTarihi;
                    yeniTalep.KargoFirmasi = model.KargoFirmasi;
                    yeniTalep.GönderiKodu = model.GönderiKodu;
                    yeniTalep.MusteriAtolyeGelisTarihi = model.MusteriAtolyeGelisTarihi;

                    context.Talepler.Add(yeniTalep);
                    context.SaveChanges();
                    transaction.Commit();
                }
            }


            return RedirectToAction("Tickets");
        }


        [HttpPost]
        public ActionResult ApproveTicket(int id)
        {
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == id);

            if (talep.TalepSekliId == 1 || talep.TalepSekliId == 2)
            {
                talep.Durum = (int)TicketStatus.DriverAppointed;
            }
            else
            {
                talep.Durum = (int)TicketStatus.EngineAcceptance;
            }

            db.SaveChanges();
            return RedirectToAction("Tickets");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectTicket(int id)
        {
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == id);

            if (talep != null)
            {
                db.Talepler.Remove(talep);
                db.SaveChanges();
            }

            return RedirectToAction("Tickets");
        }


        [HttpPost]
        public ActionResult DetailEditWithPdf(TicketDetailViewModel model)
        {
            var talepDetay = db.TalepDetay.FirstOrDefault(x => x.TalepId == model.TalepId);
            //"1-1;3-5;5-3;13-9;"
            string parcaTextMap = "";

            for (int i = 0; i < model.ParcalarText.Count; i++)
            {
                if (model.ParcalarText[i] != "0")
                {
                    parcaTextMap = parcaTextMap + model.ParcalarId[i] + "-" + model.ParcalarText[i] + ";";
                }
            }

            if (talepDetay == null)
            {
                using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    //using blokları arasında transaction'ımızı açtık ve artık transaction'ımız bir commit() fonksiyonunu kullanana kadar işlem yaptığımız tabloyu kilitleyecek.
                    {

                        Talepler talep = db.Talepler.Where(x => x.TalepId == model.Id).FirstOrDefault();
                        talep.Durum = (int)TicketStatus.TicketOpened;
                        //2. Bilet 
                        TalepDetay yeniTalep = talepDetay ?? new TalepDetay();
                        yeniTalep.TalepId = model.TalepId;
                        yeniTalep.MotorDolapNo = model.MotorDolapNo;
                        yeniTalep.KapakDolapNo = model.KapakDolapNo;
                        yeniTalep.BildirimTarihi = model.BildirimTarihi;
                        yeniTalep.ServisAdı = model.ServisAdı;
                        yeniTalep.Marka = model.Marka;
                        yeniTalep.Model = model.Model;
                        yeniTalep.MotorTipi = model.MotorTipi;
                        yeniTalep.YakıtTipi = model.YakıtTipi;
                        yeniTalep.SilindirSayisi = model.SilindirSayisi;
                        yeniTalep.Garanti = model.Garanti == false ? 0 : 1;
                        yeniTalep.Revizyon = model.Revizyon == false ? 0 : 1;
                        yeniTalep.RevizyonAciklama = model.RevizyonAciklama;
                        yeniTalep.ServisNo = model.ServisNo;
                        yeniTalep.AlınanIs = model.AlınanIs;
                        yeniTalep.Plaka = model.Plaka;
                        yeniTalep.KM = model.KM;
                        yeniTalep.VinNo = model.VinNo;
                        yeniTalep.MotorNo = model.MotorNo;
                        yeniTalep.SupapSayisi = model.SupapSayisi;
                        yeniTalep.MusteriNot = model.MusteriNot;
                        yeniTalep.ArizaDiger = model.ArizaDiger;
                        yeniTalep.ParcaDiger = model.ParcaDiger;
                        yeniTalep.ParcaAdet = model.ParcaAdet;
                        yeniTalep.ArizaList = string.Join(",", model.ArizaChck);
                        yeniTalep.ParcaList = string.Join(",", model.ParcalarChck);
                        yeniTalep.ParcaListAdet = parcaTextMap;
                        yeniTalep.IsLogoEnable = model.IsLogoEnable;

                        context.TalepDetay.Add(yeniTalep);

                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            else
            {
                talepDetay.TalepId = model.TalepId;
                talepDetay.MotorDolapNo = model.MotorDolapNo;
                talepDetay.KapakDolapNo = model.KapakDolapNo;
                talepDetay.BildirimTarihi = model.BildirimTarihi;
                talepDetay.ServisAdı = model.ServisAdı;
                talepDetay.Marka = model.Marka;
                talepDetay.Model = model.Model;
                talepDetay.MotorTipi = model.MotorTipi;
                talepDetay.YakıtTipi = model.YakıtTipi;
                talepDetay.SilindirSayisi = model.SilindirSayisi;
                talepDetay.Garanti = model.Garanti == false ? 0 : 1;
                talepDetay.Revizyon = model.Revizyon == false ? 0 : 1;
                talepDetay.RevizyonAciklama = model.RevizyonAciklama;
                talepDetay.ServisNo = model.ServisNo;
                talepDetay.AlınanIs = model.AlınanIs;
                talepDetay.Plaka = model.Plaka;
                talepDetay.KM = model.KM;
                talepDetay.VinNo = model.VinNo;
                talepDetay.MotorNo = model.MotorNo;
                talepDetay.SupapSayisi = model.SupapSayisi;
                talepDetay.MusteriNot = model.MusteriNot;
                talepDetay.ArizaDiger = model.ArizaDiger;
                talepDetay.ParcaDiger = model.ParcaDiger;
                talepDetay.ParcaAdet = model.ParcaAdet;
                talepDetay.ArizaList = string.Join(",", model.ArizaChck);
                talepDetay.ParcaList = string.Join(",", model.ParcalarChck);
                talepDetay.ParcaListAdet = parcaTextMap;
                talepDetay.IsLogoEnable = model.IsLogoEnable;
                db.SaveChanges();
            }
            return RedirectToAction("CreatePDF", new { id = model.TalepId });
        }


        public ActionResult CreatePDF(int id)
        {
            var talepler = db.Talepler.FirstOrDefault(x => x.TalepId == id);
            var talepDetay = db.TalepDetay.FirstOrDefault(x => x.TalepId == id);
            TicketDetailViewModel model = new TicketDetailViewModel();
            model.TalepId = id;

            var ariza = ArizaListesi();
            var tamir = TamirListesi();

            model.KM = talepler.Km ?? 0;
            model.VinNo = talepler.VinNo ?? "";
            model.Plaka = talepler.Plate ?? "";
            model.Marka = talepler.Vehicles.Companies.Name;
            model.Model = talepler.Vehicles.Name;

            if (talepDetay != null)
            {
                model.MotorDolapNo = talepDetay.MotorDolapNo;
                model.KapakDolapNo = talepDetay.KapakDolapNo;
                model.BildirimTarihi = talepDetay.BildirimTarihi;
                model.ServisAdı = talepDetay.ServisAdı;
                model.Marka = talepDetay.Marka;
                model.Model = talepDetay.Model;
                model.MotorTipi = talepDetay.MotorTipi;
                model.YakıtTipi = talepDetay.YakıtTipi.Value;
                model.SilindirSayisi = talepDetay.SilindirSayisi.Value;
                model.Garanti = talepDetay.Garanti.Value == 0 ? false : true;
                model.Revizyon = talepDetay.Revizyon.Value == 0 ? false : true;
                model.RevizyonAciklama = talepDetay.RevizyonAciklama;
                model.ServisNo = talepDetay.ServisNo;
                model.AlınanIs = talepDetay.AlınanIs.Value;
                model.Plaka = talepDetay.Plaka;
                model.KM = talepDetay.KM.Value;
                model.VinNo = talepDetay.VinNo;
                model.MotorNo = talepDetay.MotorNo;
                model.SupapSayisi = talepDetay.SupapSayisi;
                model.MusteriNot = talepDetay.MusteriNot;
                model.ArizaDiger = talepDetay.ArizaDiger;
                model.ParcaDiger = talepDetay.ParcaDiger;
                model.ParcaAdet = talepDetay.ParcaAdet;
                model.ParcalarText = new List<string>();
                var arizalist = talepDetay.ArizaList != null ? talepDetay.ArizaList.Split(',') : null;
                var tamirList = talepDetay.ParcaList != null ? talepDetay.ParcaList.Split(',') : null;
                talepDetay.ParcaListAdet = talepDetay.ParcaListAdet;
                //"1-1;3-5;5-3;13-9;"
                foreach (var item in ariza)
                {
                    if (arizalist != null)
                    {
                        foreach (var itemList in arizalist)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }
                var adetList = talepDetay.ParcaListAdet.Split(';');
                foreach (var item in tamir)
                {
                    if (tamirList != null)
                    {
                        foreach (var itemList in tamirList)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }

                    if (adetList.Any(x => x.Split('-')[0] == item.Value))
                        model.ParcalarText.Add(adetList.Where(x => x.Split('-')[0] == item.Value).FirstOrDefault().Split('-')[1]);
                    else
                        model.ParcalarText.Add("0");

                }
            }

            model.Ariza = ariza;
            model.Parcalar = tamir;

            #region PDF
            var templateFilePath = string.Empty;
            if (talepDetay.IsLogoEnable == 1)
            {
                templateFilePath = Server.MapPath("~/Content/PDF/FormArizaBildirim.pdf");
            }
            else
            {
                templateFilePath = Server.MapPath("~/Content/PDF/FormArizaBildirimLogosuz.pdf");
            }

            string newFilePath = Server.MapPath("~/Content/PDF/ArizaBildirimFormu.pdf");

            using (var pdfReader = new iTextSharp.text.pdf.PdfReader(templateFilePath))
            {
                using (var pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create)))
                {
                    AcroFields formFields = pdfStamper.AcroFields;

                    formFields.SetField("IsEmriNo", id.ToString());
                    if (!string.IsNullOrEmpty(model.KapakDolapNo))
                    {
                        formFields.SetField("KapakDolapNo", model.KapakDolapNo);
                    }
                    if (!string.IsNullOrEmpty(model.MotorDolapNo))
                    {
                        formFields.SetField("MotorDolapNo", model.MotorDolapNo);
                    }
                    if (!string.IsNullOrEmpty(model.BildirimTarihi.ToString()))
                    {
                        formFields.SetField("BildirimTarihi", model.BildirimTarihi.ToString());
                    }
                    if (!string.IsNullOrEmpty(model.ServisAdı))
                    {
                        formFields.SetField("ServisAdi", model.ServisAdı);
                    }
                    if (!string.IsNullOrEmpty(model.Marka))
                    {
                        formFields.SetField("MarkaModel", model.Marka);
                    }
                    if (!string.IsNullOrEmpty(model.MotorTipi))
                    {
                        formFields.SetField("MotorTipi", model.MotorTipi);
                    }
                    if (!string.IsNullOrEmpty(model.Plaka))
                    {
                        formFields.SetField("PlakaNo", model.Plaka);
                    }
                    if (!string.IsNullOrEmpty(model.KM.ToString()))
                    {
                        formFields.SetField("AracKm", model.KM.ToString());
                    }
                    if (!string.IsNullOrEmpty(model.VinNo))
                    {
                        formFields.SetField("SasiNo", model.VinNo);
                    }
                    if (!string.IsNullOrEmpty(model.MotorNo))
                    {
                        formFields.SetField("MotorNo", model.MotorNo);
                    }
                    if (!string.IsNullOrEmpty(model.SupapSayisi))
                    {
                        formFields.SetField("SupapSayisi", model.SupapSayisi);
                    }
                    if (!string.IsNullOrEmpty(model.RevizyonAciklama))
                    {
                        formFields.SetField("RevizyonAciklama", model.RevizyonAciklama);
                    }
                    if (!string.IsNullOrEmpty(model.ServisNo))
                    {
                        formFields.SetField("IKK", model.ServisNo);
                    }
                    if (!string.IsNullOrEmpty(model.MusteriNot))
                    {
                        formFields.SetField("MusteriOzelIstek", model.MusteriNot);
                    }


                    if (model.AlınanIs == 0)
                    {
                        formFields.SetField("AlinanIsMotor", "Yes");
                    }
                    else
                    {
                        formFields.SetField("AlinanIsKapak", "Yes");
                    }

                    if (model.Garanti)
                    {
                        formFields.SetField("FirmaGarantiEvet", "Yes");
                    }
                    else
                    {
                        formFields.SetField("FirmaGarantiHayir", "Yes");
                    }

                    if (model.Revizyon)
                    {
                        formFields.SetField("MotorRevizyonEvet", "Yes");
                    }
                    else
                    {
                        formFields.SetField("MotorRevizyonHayir", "Yes");
                    }

                    if (model.YakıtTipi == 0)
                    {
                        formFields.SetField("Benzin", "Yes");
                    }
                    else if (model.YakıtTipi == 1)
                    {
                        formFields.SetField("Dizel", "Yes");
                    }
                    else if (model.YakıtTipi == 2)
                    {
                        formFields.SetField("Lpg", "Yes");
                    }
                    else
                    {
                        //Hiçbiri
                    }

                    var SilindirList = new List<int> { 2, 4, 6, 8, 12 };
                    foreach (var item in SilindirList)
                    {
                        if (item == model.SilindirSayisi)
                        {
                            formFields.SetField(item.ToString(), "Yes");
                        }
                    }


                    var ArizaList = new List<string>
                    {
                        "YagEksiltme",
                        "KarterKacagi",
                        "YagaSuKaristirma",
                        "SuyaYagKaristirma",
                        "DigerArizalar",
                        "Tekleme",
                        "MotordaSes",
                        "HararetYapma",
                        "KapaktaSes",
                        "SuyaKompresyonKacagi"
                    };

                    for (int i = 0; i < ArizaList.Count; i++)
                    {
                        if (ariza[i].Selected)
                        {
                            formFields.SetField(ArizaList[i], "Yes");
                        }
                    }

                    var TamirList = new List<string>
                    {
                        "MotorBlogu",
                        "SilindirKapagi",
                        "KrankMili",
                        "EksantrikMili",
                        "BalansMili",
                        "Kompresor",
                        "Karter",
                        "KrankKamasi",
                        "Gomlek",
                        "Piston",
                        "BiyelKolu",
                        "TakımSupap",
                        "AnaYataklar",
                        "KolYataklari",
                        "KenarYataklar",
                        "AnaYatakKepleri",
                        "AnaYatakCivatalari"
                    };

                    var TamirListAdet = new List<string>
                    {
                        "MotorBloguAdet",
                        "SilindirKapagiAdet",
                        "KrankMiliAdet",
                        "EksantrikMiliAdet",
                        "BalansMiliAdet",
                        "KompresorAdet",
                        "KarterAdet",
                        "KrankKamasiAdet",
                        "GomlekAdet",
                        "PistonAdet",
                        "BiyelKoluAdet",
                        "TakımSupapAdet",
                        "AnaYataklarAdet",
                        "KolYataklariAdet",
                        "KenarYataklarAdet",
                        "AnaYatakKepleriAdet",
                        "AnaYatakCivatalariAdet"
                    };

                    for (int i = 0; i < TamirList.Count; i++)
                    {
                        if (tamir[i].Selected)
                        {
                            formFields.SetField(TamirList[i], "Yes"); //checkbox
                        }
                        formFields.SetField(TamirListAdet[i], model.ParcalarText[i]); //text
                    }

                    pdfStamper.Close();
                    pdfReader.Close();
                }
            }
            #endregion

            byte[] bytes = System.IO.File.ReadAllBytes(newFilePath);
            return File(bytes, "application/pdf", "ArizaBildirimFormu.pdf");
        }

        public void FillIsEmriCombos()
        {
            ViewBag.AracGrubu = new SelectList(db.AracGrubu.ToList(), "AracGrubuId", "Aciklamasi");
            ViewBag.Soforler = new SelectList(db.UserRoles.Where(x => x.Roles.RoleName == "DANISMAN").OrderBy(x => x.UserName).Select(x => new { UserName = x.UserName, DanismanAdi = x.Users.FirstName + " " + x.Users.FirstName }).ToList(), "UserName", "DanismanAdi");
            ViewBag.Parts = new SelectList(db.Parts.Select(x => new { PartId = x.PartId, PartName = x.Name }).ToList(), "PartId", "PartName");
            ViewBag.Markalar = db.Companies.ToList();
            ViewBag.TalepSekliId = new SelectList(db.TalepSekli.ToList(), "TalepSekliId", "TalepSekliAciklama");
            //ViewBag.TalepSekliId = new SelectList(db.TalepSekli.Select(x => new { TalepSekliId = x.TalepSekliId, TalepSekliAciklama = x.TalepSekliAciklama }).ToList(), "TalepSekliId", "TalepSekliAciklama");
        }

        private IList<SelectListItem> ArizaListesi()
        {
            var listitem = new List<SelectListItem>();

            var ariza = db.ArizaBildirim.ToList();

            foreach (var item in ariza)
            {
                listitem.Add(new SelectListItem { Text = item.Name, Value = item.ArizaBildirimId.ToString() });
            }
            return listitem;
        }
        private IList<SelectListItem> TamirListesi()
        {
            var listitem = new List<SelectListItem>();

            var tamir = db.TamirParca.ToList();

            foreach (var item in tamir)
            {
                listitem.Add(new SelectListItem { Text = item.Name, Value = item.TamirParcaId.ToString() });
            }
            return listitem;
        }

        #region FileProcess

        private static object ProcessFileInput(IEnumerable<HttpPostedFileBase> files)
        {
            List<object> processedFiles = new List<object>();
            foreach (var file in files)
            {
                processedFiles.Add(new
                {
                    ContentType = file.ContentType,
                    FileName = file.FileName,
                    InputStream = ReadFully(file.InputStream),
                    ContentLength = file.ContentLength
                });
            }
            return processedFiles;
        }

        private static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        #endregion
    }
}