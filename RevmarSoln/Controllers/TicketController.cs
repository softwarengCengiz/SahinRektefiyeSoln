using Newtonsoft.Json;
using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Helpers;
using SahinRektefiyeSoln.Helpers.Enums;
using SahinRektefiyeSoln.Infrastructure;
using SahinRektefiyeSoln.Models;
using SahinRektefiyeSoln.Models.ViewModels.IsEmri;
using SahinRektefiyeSoln.Models.ViewModels.Ticket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            var talepler = db.Talepler.ToList().OrderByDescending(x => x.TalepId);
            var model = new List<TicketListModel>();
            var isSofor = SFHelper.CheckMyRole(currentUser, "SOFOR");
            ViewBag.MusteriKabul = SFHelper.CheckMyRole(currentUser, "MUSTERIKABUL");
            ViewBag.MotorKabul = SFHelper.CheckMyRole(currentUser, "MOTORKABUL");
            ViewBag.Admin = SFHelper.CheckMyRole(currentUser, "ADMIN");
            foreach (var item in talepler)
            {
                bool tempBool = false;
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

            if (isSofor)
                model = model.Where(x => x.AtananKisi == currentUser && (x.Durum == "Şoför Atandı" || x.Durum == "Teslim Alındı-Yolda")).ToList();

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
                model.PartId = talepler.PartId;
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
            //yeniTalep.CreatedDate = model.KayitTarihi ?? System.DateTime.Now;
            //yeniTalep.Creator = this.userName;
            yeniTalep.Modifier = this.userName;
            yeniTalep.ModifiedDate = model.KayitTarihi ?? System.DateTime.Now;
            if (model.TalepSekliId != 3 && model.TalepSekliId != 4)
            {
                yeniTalep.PartId = model.PartId;
            }
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
        public ActionResult DetailEdit(int id)
        {
            var talepler = db.Talepler.FirstOrDefault(x => x.TalepId == id);
            var talepDetay = db.TalepDetay.FirstOrDefault(x => x.TalepId == id);
            var talepDosya = db.TalepDosya.FirstOrDefault(x => x.TalepDosyaId == talepler.TalepDosyaId);

            if (talepler.TalepSekliId == 3 || talepler.TalepSekliId == 4)
            {
                ViewBag.EditEnable = true;
            }

            TicketDetailViewModel model = new TicketDetailViewModel();
            model.TalepId = id;

            var ariza = ArizaListesi();
            var tamir = TamirListesi();

            model.KM = talepler.Km ?? 0;
            model.VinNo = talepler.VinNo ?? "";
            model.Plaka = talepler.Plate ?? "";
            //model.Marka = talepler.Vehicles.Companies.Name;
            //model.Model = talepler.Vehicles.Name;

            if (talepDetay != null)
            {
                model.MotorDolapNo = talepDetay.MotorDolapNo;
                model.KapakDolapNo = talepDetay.KapakDolapNo;
                model.BildirimTarihi = talepDetay.BildirimTarihi;
                model.ServisAdı = talepDetay.ServisAdı;
                model.BrandName = talepDetay.Marka;
                model.BrandModelName = talepDetay.Model;
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
                if (talepDetay.AlınanIs == 0)
                {
                    model.AlınanIsM = false;
                    model.AlınanIsK = false;
                }
                else if (talepDetay.AlınanIs == 1)
                {
                    model.AlınanIsM = true;
                    model.AlınanIsK = false;
                }
                else if (talepDetay.AlınanIs == 2)
                {
                    model.AlınanIsM = false;
                    model.AlınanIsK = true;
                }
                else
                {
                    model.AlınanIsM = true;
                    model.AlınanIsK = true;
                }

                //if (talepler.VehicleId != null)
                //{​​
                //model.Marka = talepler.Vehicles.Companies.Name;
                //    model.Model = talepler.Vehicles.Name;
                //}​​

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
                var adetList = talepDetay.ParcaListAdet != null ? talepDetay.ParcaListAdet.Split(';') : null;
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
                    if (adetList != null)
                    {
                        if (adetList.Any(x => x.Split('-')[0] == item.Value))
                            model.ParcalarText.Add(adetList.Where(x => x.Split('-')[0] == item.Value).FirstOrDefault().Split('-')[1]);
                        else
                            model.ParcalarText.Add("0");
                    }
                }
            }

            model.Ariza = ariza;
            model.Parcalar = tamir;

            ViewBag.Brands = new SelectList(db.Brands.ToList().OrderBy(x => x.BrandName), "BrandName", "BrandName");
            if (model.BrandName != null)
            {
                var brandId = db.Brands.FirstOrDefault(x => x.BrandName == model.BrandName).BrandId;
                ViewBag.BrandModels = new SelectList(db.BrandModels.Where(x => x.BrandId == brandId).ToList(), "BrandModelName", "BrandModelName");
            }
            else
            {
                ViewBag.BrandModels = null;
            }

            ViewBag.TalepStatu = talepler.Durum;

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
            if ((talep.TalepSekliId != 3 && talep.TalepSekliId != 4) && model.FlagSave == 0)
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
                        yeniTalep.Marka = model.BrandName;
                        yeniTalep.Model = model.BrandModelName;
                        yeniTalep.MotorTipi = model.MotorTipi;
                        yeniTalep.YakıtTipi = model.YakıtTipi;
                        yeniTalep.SilindirSayisi = model.SilindirSayisi;
                        yeniTalep.Garanti = model.Garanti == false ? 0 : 1;
                        yeniTalep.Revizyon = model.Revizyon == false ? 0 : 1;
                        yeniTalep.RevizyonAciklama = model.RevizyonAciklama;
                        yeniTalep.ServisNo = model.ServisNo;
                        if (!model.AlınanIsM && !model.AlınanIsK)
                        {
                            yeniTalep.AlınanIs = 0;
                        }
                        else if (model.AlınanIsM && !model.AlınanIsK)
                        {
                            yeniTalep.AlınanIs = 1;
                        }
                        else if (model.AlınanIsK && !model.AlınanIsM)
                        {
                            yeniTalep.AlınanIs = 2;
                        }
                        else
                        {
                            yeniTalep.AlınanIs = 3;
                        }
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
                talepDetay.Marka = model.BrandName;
                talepDetay.Model = model.BrandModelName;
                talepDetay.MotorTipi = model.MotorTipi;
                talepDetay.YakıtTipi = model.YakıtTipi;
                talepDetay.SilindirSayisi = model.SilindirSayisi;
                talepDetay.Garanti = model.Garanti == false ? 0 : 1;
                talepDetay.Revizyon = model.Revizyon == false ? 0 : 1;
                talepDetay.RevizyonAciklama = model.RevizyonAciklama;
                talepDetay.ServisNo = model.ServisNo;
                if (!model.AlınanIsM && !model.AlınanIsK)
                {
                    talepDetay.AlınanIs = 0;
                }
                else if (model.AlınanIsM && !model.AlınanIsK)
                {
                    talepDetay.AlınanIs = 1;
                }
                else if (model.AlınanIsK && !model.AlınanIsM)
                {
                    talepDetay.AlınanIs = 2;
                }
                else
                {
                    talepDetay.AlınanIs = 3;
                }
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
                return RedirectToAction("DetailEdit",new {id = model.TalepId });
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
                    //yeniTalep.AracGrupId = model.AracGrubuId;
                    //yeniTalep.VehicleId = model.VehicleId;
                    //yeniTalep.Km = model.KM;
                    //yeniTalep.VinNo = model.SaseNo;
                    //yeniTalep.Plate = model.Plate;
                    if (model.TalepSekliId != 3 && model.TalepSekliId != 4)
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
        public ActionResult RejectTicket(int id)
        {
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == id);

            if (talep != null)
            {
                talep.Durum = (int)TicketStatus.Rejected;
                db.SaveChanges();
            }

            return RedirectToAction("Tickets");
        }

        [HttpPost]
        public ActionResult ApproveReceived(int id)
        {
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == id);

            if (talep != null)
            {
                talep.Durum = (int)TicketStatus.EngineAcceptance;
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
                        yeniTalep.Marka = model.BrandName;
                        yeniTalep.Model = model.BrandModelName;
                        yeniTalep.MotorTipi = model.MotorTipi;
                        yeniTalep.YakıtTipi = model.YakıtTipi;
                        yeniTalep.SilindirSayisi = model.SilindirSayisi;
                        yeniTalep.Garanti = model.Garanti == false ? 0 : 1;
                        yeniTalep.Revizyon = model.Revizyon == false ? 0 : 1;
                        yeniTalep.RevizyonAciklama = model.RevizyonAciklama;
                        yeniTalep.ServisNo = model.ServisNo;
                        if (!model.AlınanIsM && !model.AlınanIsK)
                        {
                            yeniTalep.AlınanIs = 0;
                        }
                        else if (model.AlınanIsM && !model.AlınanIsK)
                        {
                            yeniTalep.AlınanIs = 1;
                        }
                        else if (model.AlınanIsK && !model.AlınanIsM)
                        {
                            yeniTalep.AlınanIs = 2;
                        }
                        else
                        {
                            yeniTalep.AlınanIs = 3;
                        }
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
                talepDetay.Marka = model.BrandName;
                talepDetay.Model = model.BrandModelName;
                talepDetay.MotorTipi = model.MotorTipi;
                talepDetay.YakıtTipi = model.YakıtTipi;
                talepDetay.SilindirSayisi = model.SilindirSayisi;
                talepDetay.Garanti = model.Garanti == false ? 0 : 1;
                talepDetay.Revizyon = model.Revizyon == false ? 0 : 1;
                talepDetay.RevizyonAciklama = model.RevizyonAciklama;
                talepDetay.ServisNo = model.ServisNo;
                if (!model.AlınanIsM && !model.AlınanIsK)
                {
                    talepDetay.AlınanIs = 0;
                }
                else if (model.AlınanIsM && !model.AlınanIsK)
                {
                    talepDetay.AlınanIs = 1;
                }
                else if (model.AlınanIsK && !model.AlınanIsM)
                {
                    talepDetay.AlınanIs = 2;
                }
                else
                {
                    talepDetay.AlınanIs = 3;
                }
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


        //public ActionResult CreatePDF(int id)
        //{
        //    var talepler = db.Talepler.FirstOrDefault(x => x.TalepId == id);
        //    var talepDetay = db.TalepDetay.FirstOrDefault(x => x.TalepId == id);
        //    TicketDetailViewModel model = new TicketDetailViewModel();
        //    model.TalepId = id;

        //    var ariza = ArizaListesi();
        //    var tamir = TamirListesi();

        //    model.KM = talepler.Km ?? 0;
        //    model.VinNo = talepler.VinNo ?? "";
        //    model.Plaka = talepler.Plate ?? "";
        //    model.Marka = talepler.Vehicles.Companies.Name;
        //    model.Model = talepler.Vehicles.Name;

        //    if (talepDetay != null)
        //    {
        //        model.MotorDolapNo = talepDetay.MotorDolapNo;
        //        model.KapakDolapNo = talepDetay.KapakDolapNo;
        //        model.BildirimTarihi = talepDetay.BildirimTarihi;
        //        model.ServisAdı = talepDetay.ServisAdı;
        //        model.Marka = talepDetay.Marka;
        //        model.Model = talepDetay.Model;
        //        model.MotorTipi = talepDetay.MotorTipi;
        //        model.YakıtTipi = talepDetay.YakıtTipi.Value;
        //        model.SilindirSayisi = talepDetay.SilindirSayisi.Value;
        //        model.Garanti = talepDetay.Garanti.Value == 0 ? false : true;
        //        model.Revizyon = talepDetay.Revizyon.Value == 0 ? false : true;
        //        model.RevizyonAciklama = talepDetay.RevizyonAciklama;
        //        model.ServisNo = talepDetay.ServisNo;
        //        model.AlınanIs = talepDetay.AlınanIs.Value;
        //        model.Plaka = talepDetay.Plaka;
        //        model.KM = talepDetay.KM.Value;
        //        model.VinNo = talepDetay.VinNo;
        //        model.MotorNo = talepDetay.MotorNo;
        //        model.SupapSayisi = talepDetay.SupapSayisi;
        //        model.MusteriNot = talepDetay.MusteriNot;
        //        model.ArizaDiger = talepDetay.ArizaDiger;
        //        model.ParcaDiger = talepDetay.ParcaDiger;
        //        model.ParcaAdet = talepDetay.ParcaAdet;
        //        model.ParcalarText = new List<string>();
        //        var arizalist = talepDetay.ArizaList != null ? talepDetay.ArizaList.Split(',') : null;
        //        var tamirList = talepDetay.ParcaList != null ? talepDetay.ParcaList.Split(',') : null;
        //        talepDetay.ParcaListAdet = talepDetay.ParcaListAdet;
        //        //"1-1;3-5;5-3;13-9;"
        //        foreach (var item in ariza)
        //        {
        //            if (arizalist != null)
        //            {
        //                foreach (var itemList in arizalist)
        //                {
        //                    if (item.Value == itemList)
        //                        item.Selected = true;
        //                }
        //            }
        //        }
        //        var adetList = talepDetay.ParcaListAdet.Split(';');
        //        foreach (var item in tamir)
        //        {
        //            if (tamirList != null)
        //            {
        //                foreach (var itemList in tamirList)
        //                {
        //                    if (item.Value == itemList)
        //                        item.Selected = true;
        //                }
        //            }

        //            if (adetList.Any(x => x.Split('-')[0] == item.Value))
        //                model.ParcalarText.Add(adetList.Where(x => x.Split('-')[0] == item.Value).FirstOrDefault().Split('-')[1]);
        //            else
        //                model.ParcalarText.Add("0");

        //        }
        //    }

        //    model.Ariza = ariza;
        //    model.Parcalar = tamir;

        //    #region PDF
        //    var templateFilePath = string.Empty;
        //    if (talepDetay.IsLogoEnable == 1)
        //    {
        //        templateFilePath = Server.MapPath("~/Content/PDF/FormArizaBildirim.pdf");
        //    }
        //    else
        //    {
        //        templateFilePath = Server.MapPath("~/Content/PDF/FormArizaBildirimLogosuz.pdf");
        //    }

        //    string newFilePath = Server.MapPath("~/Content/PDF/ArizaBildirimFormu.pdf");

        //    using (var pdfReader = new iTextSharp.text.pdf.PdfReader(templateFilePath))
        //    {
        //        using (var pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create)))
        //        {
        //            AcroFields formFields = pdfStamper.AcroFields;

        //            formFields.SetField("IsEmriNo", id.ToString());
        //            if (!string.IsNullOrEmpty(model.KapakDolapNo))
        //            {
        //                formFields.SetField("KapakDolapNo", model.KapakDolapNo);
        //            }
        //            if (!string.IsNullOrEmpty(model.MotorDolapNo))
        //            {
        //                formFields.SetField("MotorDolapNo", model.MotorDolapNo);
        //            }
        //            if (!string.IsNullOrEmpty(model.BildirimTarihi.ToString()))
        //            {
        //                formFields.SetField("BildirimTarihi", model.BildirimTarihi.ToString());
        //            }
        //            if (!string.IsNullOrEmpty(model.ServisAdı))
        //            {
        //                formFields.SetField("ServisAdi", model.ServisAdı);
        //            }
        //            if (!string.IsNullOrEmpty(model.Marka))
        //            {
        //                formFields.SetField("MarkaModel", model.Marka);
        //            }
        //            if (!string.IsNullOrEmpty(model.MotorTipi))
        //            {
        //                formFields.SetField("MotorTipi", model.MotorTipi);
        //            }
        //            if (!string.IsNullOrEmpty(model.Plaka))
        //            {
        //                formFields.SetField("PlakaNo", model.Plaka);
        //            }
        //            if (!string.IsNullOrEmpty(model.KM.ToString()))
        //            {
        //                formFields.SetField("AracKm", model.KM.ToString());
        //            }
        //            if (!string.IsNullOrEmpty(model.VinNo))
        //            {
        //                formFields.SetField("SasiNo", model.VinNo);
        //            }
        //            if (!string.IsNullOrEmpty(model.MotorNo))
        //            {
        //                formFields.SetField("MotorNo", model.MotorNo);
        //            }
        //            if (!string.IsNullOrEmpty(model.SupapSayisi))
        //            {
        //                formFields.SetField("SupapSayisi", model.SupapSayisi);
        //            }
        //            if (!string.IsNullOrEmpty(model.RevizyonAciklama))
        //            {
        //                formFields.SetField("RevizyonAciklama", model.RevizyonAciklama);
        //            }
        //            if (!string.IsNullOrEmpty(model.ServisNo))
        //            {
        //                formFields.SetField("IKK", model.ServisNo);
        //            }
        //            if (!string.IsNullOrEmpty(model.MusteriNot))
        //            {
        //                formFields.SetField("MusteriOzelIstek", model.MusteriNot);
        //            }


        //            if (model.AlınanIs == 0)
        //            {
        //                formFields.SetField("AlinanIsMotor", "Yes");
        //            }
        //            else
        //            {
        //                formFields.SetField("AlinanIsKapak", "Yes");
        //            }

        //            if (model.Garanti)
        //            {
        //                formFields.SetField("FirmaGarantiEvet", "Yes");
        //            }
        //            else
        //            {
        //                formFields.SetField("FirmaGarantiHayir", "Yes");
        //            }

        //            if (model.Revizyon)
        //            {
        //                formFields.SetField("MotorRevizyonEvet", "Yes");
        //            }
        //            else
        //            {
        //                formFields.SetField("MotorRevizyonHayir", "Yes");
        //            }

        //            if (model.YakıtTipi == 0)
        //            {
        //                formFields.SetField("Benzin", "Yes");
        //            }
        //            else if (model.YakıtTipi == 1)
        //            {
        //                formFields.SetField("Dizel", "Yes");
        //            }
        //            else if (model.YakıtTipi == 2)
        //            {
        //                formFields.SetField("Lpg", "Yes");
        //            }
        //            else
        //            {
        //                //Hiçbiri
        //            }

        //            var SilindirList = new List<int> { 2, 4, 6, 8, 12 };
        //            foreach (var item in SilindirList)
        //            {
        //                if (item == model.SilindirSayisi)
        //                {
        //                    formFields.SetField(item.ToString(), "Yes");
        //                }
        //            }


        //            var ArizaList = new List<string>
        //            {
        //                "YagEksiltme",
        //                "KarterKacagi",
        //                "YagaSuKaristirma",
        //                "SuyaYagKaristirma",
        //                "DigerArizalar",
        //                "Tekleme",
        //                "MotordaSes",
        //                "HararetYapma",
        //                "KapaktaSes",
        //                "SuyaKompresyonKacagi"
        //            };

        //            for (int i = 0; i < ArizaList.Count; i++)
        //            {
        //                if (ariza[i].Selected)
        //                {
        //                    formFields.SetField(ArizaList[i], "Yes");
        //                }
        //            }

        //            var TamirList = new List<string>
        //            {
        //                "MotorBlogu",
        //                "SilindirKapagi",
        //                "KrankMili",
        //                "EksantrikMili",
        //                "BalansMili",
        //                "Kompresor",
        //                "Karter",
        //                "KrankKamasi",
        //                "Gomlek",
        //                "Piston",
        //                "BiyelKolu",
        //                "TakımSupap",
        //                "AnaYataklar",
        //                "KolYataklari",
        //                "KenarYataklar",
        //                "AnaYatakKepleri",
        //                "AnaYatakCivatalari"
        //            };

        //            var TamirListAdet = new List<string>
        //            {
        //                "MotorBloguAdet",
        //                "SilindirKapagiAdet",
        //                "KrankMiliAdet",
        //                "EksantrikMiliAdet",
        //                "BalansMiliAdet",
        //                "KompresorAdet",
        //                "KarterAdet",
        //                "KrankKamasiAdet",
        //                "GomlekAdet",
        //                "PistonAdet",
        //                "BiyelKoluAdet",
        //                "TakımSupapAdet",
        //                "AnaYataklarAdet",
        //                "KolYataklariAdet",
        //                "KenarYataklarAdet",
        //                "AnaYatakKepleriAdet",
        //                "AnaYatakCivatalariAdet"
        //            };

        //            for (int i = 0; i < TamirList.Count; i++)
        //            {
        //                if (tamir[i].Selected)
        //                {
        //                    formFields.SetField(TamirList[i], "Yes"); //checkbox
        //                }
        //                formFields.SetField(TamirListAdet[i], model.ParcalarText[i]); //text
        //            }

        //            pdfStamper.Close();
        //            pdfReader.Close();
        //        }
        //    }
        //    #endregion

        //    byte[] bytes = System.IO.File.ReadAllBytes(newFilePath);
        //    return File(bytes, "application/pdf", "ArizaBildirimFormu.pdf");
        //}

        public void FillIsEmriCombos()
        {
            ViewBag.AracGrubu = new SelectList(db.AracGrubu.ToList(), "AracGrubuId", "Aciklamasi");
            ViewBag.Soforler = new SelectList(db.UserRoles.Where(x => x.Roles.RoleName == "SOFOR").OrderBy(x => x.UserName).Select(x => new { UserName = x.UserName, DanismanAdi = x.Users.FirstName + " " + x.Users.SurName }).ToList(), "UserName", "DanismanAdi");
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
        private IList<SelectListItemWithAttribute> TamirListesi()
        {
            var listitem = new List<SelectListItemWithAttribute>();

            var tamir = db.TamirParca.ToList();

            foreach (var item in tamir)
            {
                listitem.Add(new SelectListItemWithAttribute { Text = item.Name, Value = item.TamirParcaId.ToString(), IsContainPiece = item.IsContainPiece });
            }
            return listitem;
        }

        private IList<SelectListItemWithAttribute> EngineInformationDet()
        {
            var listItem = new List<SelectListItemWithAttribute>();

            var engineInformations = db.EngineInformationDet.ToList();

            foreach (var item in engineInformations)
            {
                listItem.Add(new SelectListItemWithAttribute { Text = item.EngineInfoDetDesc, Value = item.EngineInfoDetId.ToString(), HdrId = item.EngineInfoHdrId, IsContainPiece = item.IsContainPiece });
            }
            return listItem;
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

        public ActionResult CreatePdf(int id)
        {
            var model = GetTicketDetails(id);
            var htmlString = GeneratePdfFromView("~/Views/Forms/ArizaBildirimFormu.cshtml", model, ControllerContext);

            var htmlToPdfConverter = new NReco.PdfGenerator.HtmlToPdfConverter();
            htmlToPdfConverter.CustomWkHtmlArgs = "--dpi 1200 --encoding utf-8 --enable-local-file-access";
            var pdfBytes = htmlToPdfConverter.GeneratePdf(htmlString);

            return File(pdfBytes, "application/pdf", "ArizaBildirimFormu.pdf");
        }

        // Görünümü HTML kodlarına dönüştürmek için yardımcı yöntem
        public static string GeneratePdfFromView(string viewName, object model, ControllerContext controllerContext)
        {
            var viewEngineResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
            if (viewEngineResult.View == null)
                throw new FileNotFoundException("Görünüm bulunamadı.");

            var viewData = new ViewDataDictionary(model);
            var tempData = new TempDataDictionary();
            var viewContext = new ViewContext(controllerContext, viewEngineResult.View, viewData, tempData, TextWriter.Null);
            var htmlString = new StringWriter();
            viewEngineResult.View.Render(viewContext, htmlString);

            return htmlString.ToString();
        }

        private TicketDetailViewModel GetTicketDetails(int id)
        {
            var talepler = db.Talepler.FirstOrDefault(x => x.TalepId == id);
            var talepDetay = db.TalepDetay.FirstOrDefault(x => x.TalepId == id);
            var talepDosya = db.TalepDosya.FirstOrDefault(x => x.TalepDosyaId == talepler.TalepDosyaId);

            if (talepler.TalepSekliId == 3 || talepler.TalepSekliId == 4)
            {
                ViewBag.EditEnable = true;
            }

            TicketDetailViewModel model = new TicketDetailViewModel();
            model.TalepId = id;

            var ariza = ArizaListesi();
            var tamir = TamirListesi();

            model.KM = talepler.Km ?? 0;
            model.VinNo = talepler.VinNo ?? "";
            model.Plaka = talepler.Plate ?? "";
            //model.Marka = talepler.Vehicles.Companies.Name;
            //model.Model = talepler.Vehicles.Name;

            if (talepDetay != null)
            {
                model.MotorDolapNo = talepDetay.MotorDolapNo;
                model.KapakDolapNo = talepDetay.KapakDolapNo;
                model.BildirimTarihi = talepDetay.BildirimTarihi;
                model.ServisAdı = talepDetay.ServisAdı;
                model.BrandName = talepDetay.Marka;
                model.BrandModelName = talepDetay.Model;
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
                var adetList = talepDetay.ParcaListAdet != null ? talepDetay.ParcaListAdet.Split(';') : null;
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

                    if (adetList != null)
                    {
                        if (adetList.Any(x => x.Split('-')[0] == item.Value))
                            model.ParcalarText.Add(adetList.Where(x => x.Split('-')[0] == item.Value).FirstOrDefault().Split('-')[1]);
                        else
                            model.ParcalarText.Add("0");
                    }
                }
            }

            model.Ariza = ariza;
            model.Parcalar = tamir;

            return model;
        }


        public JsonResult GetBrandModels(string BrandsId, int? selectedId)
        {
            List<SelectListItem> PList = new List<SelectListItem>();
            var brandId = db.Brands.FirstOrDefault(x => x.BrandName == BrandsId).BrandId;
            List<BrandModels> list = db.BrandModels.Where(x => x.BrandId == brandId).ToList();

            PList.Add(new SelectListItem { Text = "Model Seçiniz", Value = "" });
            list.ForEach(x => PList.Add(new SelectListItem()
            {
                Text = x.BrandModelName,
                Value = x.BrandModelName
            })
            );

            return Json(PList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ShowTicketDetails(int id)
        {
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == id);
            var talepDetay = db.TalepDetay.FirstOrDefault(x => x.TalepId == id);
            //var talepDosya = db.TalepDosya.FirstOrDefault(x => x.TalepDosyaId == talep.TalepDosyaId);

            string hizmet = string.Empty;
            if (talep.PartId != null)
            {
                hizmet = db.Parts.FirstOrDefault(x => x.PartId == talep.PartId).Name;
            }
            string musteri = string.Empty;
            if (talep.Musteri.MusteriTipi == "B")
            {
                musteri = talep.Musteri.MusteriAdi + " " + talep.Musteri.MusteriSoyadi;
            }
            else if (talep.Musteri.MusteriTipi == "K")
            {
                musteri = talep.Musteri.KontakAdi + " " + talep.Musteri.KontakSoyadi;
            }
            string talepSekli = db.TalepSekli.FirstOrDefault(x => x.TalepSekliId == talep.TalepSekliId).TalepSekliAciklama;

            var model = new ShowTicketDetailViewModel();

            model.TalepNo = talep.TalepId;
            model.TalepSekliId = talep.TalepSekliId;
            model.TalepSekli = talepSekli;
            model.TalepTarihi = talep.CreatedDate;
            model.MusteriAramaTarihi = talep.MusteriAramaTarihi;
            model.MusteriAtolyeGelisTarihi = talep.MusteriAtolyeGelisTarihi;
            model.Sofor = talep.AtananSofor;
            model.Hizmet = hizmet;
            model.Musteri = musteri;
            model.MusteriTipi = talep.Musteri.MusteriTipi == "B" ? "Birey" : "Kurum";
            model.Telefon = talep.Musteri.MusteriTelefon.Select(x => x.TelefonNumarasi).ToList();
            model.Email = talep.Musteri.MusteriMail.Select(x => x.MailAdresi).ToList();
            model.Adres = talep.Musteri.Adres;
            if (talepDetay != null)
            {
                model.Notlar = talepDetay.MusteriNot != null ? talepDetay.MusteriNot : null;
                model.MarkaModel = talepDetay.Model != null ? talepDetay.Model : null;
                model.Plaka = talepDetay.Plaka != null ? talepDetay.Plaka : null;
            }
            model.KargoyaVerilisTarihi = talep.KargoyaVerilisTarihi;
            model.AramaTarihi = talep.AramaTarihi;
            model.KargoFirmasi = talep.KargoFirmasi;
            model.GonderiKodu = talep.GönderiKodu;

            var arizalar = ArizaListesi();
            if (talepDetay != null)
            {
                var arizalist = talepDetay.ArizaList != null ? talepDetay.ArizaList.Split(',') : null;
                foreach (var item in arizalar)
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
                model.ArizaBildirim = arizalar;

                var parcaList = TamirListesi();
                var parcalar = talepDetay.ParcaList != null ? talepDetay.ParcaList.Split(',') : null;
                foreach (var item in parcaList)
                {
                    if (parcalar != null)
                    {
                        foreach (var itemList in parcalar)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }
                model.Parcalar = parcaList;
            }

            var engineInfoDet = EngineInformationDet();
            var motorCıkısKaliteIscilik = db.EngineOutputQuality.FirstOrDefault(x => x.TalepId == id);
            if (motorCıkısKaliteIscilik != null)
            {
                var iscilikMotorCıkısKalite = motorCıkısKaliteIscilik.BlokKrankKolIsleri != null ? motorCıkısKaliteIscilik.BlokKrankKolIsleri.Split(',') : null;
                foreach (var item in engineInfoDet.Where(x => x.HdrId == 4).ToList())
                {
                    if (iscilikMotorCıkısKalite != null)
                    {
                        foreach (var itemList in iscilikMotorCıkısKalite)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }
                model.Iscilikler = engineInfoDet;
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult EngineInputDimensionalControl(int id)
        {
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == id);
            var talepDetay = db.TalepDetay.FirstOrDefault(x => x.TalepId == id);
            var motorOlcuselKontrol = db.MotorOlcuselKontrol.FirstOrDefault(x => x.TalepId == id);

            var model = new EngineDimensionalControlViewModel()
            {
                SilindirCaplari = new List<List<string>>(),
                GomlekFaturaTasma = new List<List<string>>(),
                GomlekYuvaCapi = new List<List<string>>(),
                PistonCapi = new List<List<string>>(),
                AnaMuylu = new List<List<string>>(),
                KolMuylu = new List<List<string>>()
            };

            var engineInfoDet = EngineInformationDet();

            model.EngineInfoDet = engineInfoDet;
            model.TalepId = talep.TalepId;
            if (talepDetay != null)
            {
                model.MotorDolapNo = talepDetay.MotorDolapNo;
                model.KapakDolapNo = talepDetay.KapakDolapNo;
            }
            if (motorOlcuselKontrol != null)
            {
                var engineInfoDetList = motorOlcuselKontrol.MotorIncelemeSonuc != null ? motorOlcuselKontrol.MotorIncelemeSonuc.Split(',') : null;
                foreach (var item in engineInfoDet)
                {
                    if (engineInfoDetList != null)
                    {
                        foreach (var itemList in engineInfoDetList)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }

                model.IzinVerilenMaxEgrilik = motorOlcuselKontrol.IzinVerilenMaxEgrilik;
                model.TespitEdilenEgrilik = motorOlcuselKontrol.TespitEdilenEgrilik;
                model.SilindirCaplariStdDeger = motorOlcuselKontrol.SilindirCaplariStdDeger;
                model.MaxAsinma = motorOlcuselKontrol.MaxAsinma;
                model.MaxOvallik = motorOlcuselKontrol.MaxOvallik;
                model.MaxKoniklik = motorOlcuselKontrol.MaxKoniklik;
                model.SilindirNo1 = motorOlcuselKontrol.SilindirNo1;
                model.SilindirNo2 = motorOlcuselKontrol.SilindirNo2;
                model.SilindirNo3 = motorOlcuselKontrol.SilindirNo3;
                model.GomlekFaturaStdDeger = motorOlcuselKontrol.GomlekFaturaStdDeger;
                model.PistonCapiStdDeger = motorOlcuselKontrol.PistonCapiStdDeger;
                model.AnaMuyluStdDeger = motorOlcuselKontrol.AnaMuyluStdDeger;
                model.KolMuyluStdDeger = motorOlcuselKontrol.KolMuyluStdDeger;
                model.KrankMiliSalgiDegeri = motorOlcuselKontrol.KrankMiliSalgiDegeri;
                model.EngineInfoDet = engineInfoDet;

                #region SilindirÇapları
                if (motorOlcuselKontrol.SilindirCaplari != null)
                {
                    string[] savedValues = motorOlcuselKontrol.SilindirCaplari.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 6; i++)
                    {
                        model.SilindirCaplari.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.SilindirCaplari[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        model.SilindirCaplari.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            model.SilindirCaplari[i].Add(string.Empty);
                        }
                    }
                }
                #endregion

                #region Gömlek Fatura Taşma
                if (motorOlcuselKontrol.GomlekFaturaTasma != null)
                {
                    string[] savedValues = motorOlcuselKontrol.GomlekFaturaTasma.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 1; i++)
                    {
                        model.GomlekFaturaTasma.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.GomlekFaturaTasma[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 1; i++)
                    {
                        model.GomlekFaturaTasma.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            model.GomlekFaturaTasma[i].Add(string.Empty);
                        }
                    }
                }
                #endregion

                #region Gömlek Yuva Çapı
                if (motorOlcuselKontrol.GomlekYuvaCapi != null)
                {
                    string[] savedValues = motorOlcuselKontrol.GomlekYuvaCapi.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 2; i++)
                    {
                        model.GomlekYuvaCapi.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.GomlekYuvaCapi[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        model.GomlekYuvaCapi.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            model.GomlekYuvaCapi[i].Add(string.Empty);
                        }
                    }
                }
                #endregion

                #region Piston Çapı
                if (motorOlcuselKontrol.PistonCapi != null)
                {
                    string[] savedValues = motorOlcuselKontrol.PistonCapi.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 1; i++)
                    {
                        model.PistonCapi.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.PistonCapi[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 1; i++)
                    {
                        model.PistonCapi.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            model.PistonCapi[i].Add(string.Empty);
                        }
                    }
                }
                #endregion

                #region Ana Muylu
                if (motorOlcuselKontrol.AnaMuylu != null)
                {
                    string[] savedValues = motorOlcuselKontrol.AnaMuylu.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 2; i++)
                    {
                        model.AnaMuylu.Add(new List<string>());

                        for (int j = 0; j < 10; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.AnaMuylu[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        model.AnaMuylu.Add(new List<string>());

                        for (int j = 0; j < 10; j++)
                        {
                            model.AnaMuylu[i].Add(string.Empty);
                        }
                    }
                }
                #endregion

                #region Kol Muylu
                if (motorOlcuselKontrol.KolMuylu != null)
                {
                    string[] savedValues = motorOlcuselKontrol.KolMuylu.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 2; i++)
                    {
                        model.KolMuylu.Add(new List<string>());

                        for (int j = 0; j < 10; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.KolMuylu[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        model.KolMuylu.Add(new List<string>());

                        for (int j = 0; j < 10; j++)
                        {
                            model.KolMuylu[i].Add(string.Empty);
                        }
                    }
                }
                #endregion
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    model.SilindirCaplari.Add(new List<string>());

                    for (int j = 0; j < 12; j++)
                    {
                        model.SilindirCaplari[i].Add(string.Empty);
                    }
                }

                for (int i = 0; i < 1; i++)
                {
                    model.GomlekFaturaTasma.Add(new List<string>());

                    for (int j = 0; j < 12; j++)
                    {
                        model.GomlekFaturaTasma[i].Add(string.Empty);
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    model.GomlekYuvaCapi.Add(new List<string>());

                    for (int j = 0; j < 12; j++)
                    {
                        model.GomlekYuvaCapi[i].Add(string.Empty);
                    }
                }

                for (int i = 0; i < 1; i++)
                {
                    model.PistonCapi.Add(new List<string>());

                    for (int j = 0; j < 12; j++)
                    {
                        model.PistonCapi[i].Add(string.Empty);
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    model.AnaMuylu.Add(new List<string>());

                    for (int j = 0; j < 10; j++)
                    {
                        model.AnaMuylu[i].Add(string.Empty);
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    model.KolMuylu.Add(new List<string>());

                    for (int j = 0; j < 10; j++)
                    {
                        model.KolMuylu[i].Add(string.Empty);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult EngineInputDimensionalControl(EngineDimensionalControlViewModel model)
        {
            var motorOlcuselKontrol = db.MotorOlcuselKontrol.FirstOrDefault(x => x.TalepId == model.TalepId);
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == model.TalepId);

            #region Silindir Çapları birleştirme
            List<string> combinedSilindirCaplari = new List<string>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    string cellValue = model.SilindirCaplari[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedSilindirCaplari.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesSC = string.Join(";", combinedSilindirCaplari);
            #endregion

            #region Gömlek Fatura Taşma Birleştirme
            List<string> combinedGomlekFaturaTasma = new List<string>();
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    string cellValue = model.GomlekFaturaTasma[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedGomlekFaturaTasma.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesGFT = string.Join(";", combinedGomlekFaturaTasma);
            #endregion

            #region Gömlek Yuva Çapı Birleştirme
            List<string> combinedGomlekYuvaCapi = new List<string>();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    string cellValue = model.GomlekYuvaCapi[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedGomlekYuvaCapi.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesGYC = string.Join(";", combinedGomlekYuvaCapi);
            #endregion

            #region Piston Çapı Birleştirme
            List<string> combinedPistonCapi = new List<string>();
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    string cellValue = model.PistonCapi[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedPistonCapi.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesPC = string.Join(";", combinedPistonCapi);
            #endregion

            #region Ana Muylu Birleştirme
            List<string> combinedAnaMuylu = new List<string>();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    string cellValue = model.AnaMuylu[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedAnaMuylu.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesAMA = string.Join(";", combinedAnaMuylu);
            #endregion

            #region Kol Muylu Birleştirme
            List<string> combinedKolMuylu = new List<string>();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    string cellValue = model.KolMuylu[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedKolMuylu.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesKMA = string.Join(";", combinedKolMuylu);
            #endregion

            if (motorOlcuselKontrol == null)
            {
                using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        MotorOlcuselKontrol yeniOlcuselKontrol = new MotorOlcuselKontrol()
                        {
                            TalepId = model.TalepId,
                            IzinVerilenMaxEgrilik = model.IzinVerilenMaxEgrilik,
                            TespitEdilenEgrilik = model.TespitEdilenEgrilik,
                            SilindirCaplariStdDeger = model.SilindirCaplariStdDeger,
                            SilindirCaplari = concatenatedValuesSC,
                            MaxAsinma = model.MaxAsinma,
                            MaxOvallik = model.MaxOvallik,
                            MaxKoniklik = model.MaxKoniklik,
                            SilindirNo1 = model.SilindirNo1,
                            SilindirNo2 = model.SilindirNo2,
                            SilindirNo3 = model.SilindirNo3,
                            GomlekFaturaStdDeger = model.GomlekFaturaStdDeger,
                            GomlekFaturaTasma = concatenatedValuesGFT,
                            GomlekYuvaCapi = concatenatedValuesGYC,
                            PistonCapiStdDeger = model.PistonCapiStdDeger,
                            PistonCapi = concatenatedValuesPC,
                            AnaMuyluStdDeger = model.AnaMuyluStdDeger,
                            AnaMuylu = concatenatedValuesAMA,
                            KolMuyluStdDeger = model.KolMuyluStdDeger,
                            KolMuylu = concatenatedValuesKMA,
                            KrankMiliSalgiDegeri = model.KrankMiliSalgiDegeri,
                            MotorIncelemeSonuc = model.MotorIncelemeSonuc != null ? string.Join(",", model.MotorIncelemeSonuc) : null
                        };

                        context.MotorOlcuselKontrol.Add(yeniOlcuselKontrol);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            else
            {
                motorOlcuselKontrol.TalepId = model.TalepId;
                motorOlcuselKontrol.IzinVerilenMaxEgrilik = model.IzinVerilenMaxEgrilik;
                motorOlcuselKontrol.TespitEdilenEgrilik = model.TespitEdilenEgrilik;
                motorOlcuselKontrol.SilindirCaplariStdDeger = model.SilindirCaplariStdDeger;
                motorOlcuselKontrol.SilindirCaplari = concatenatedValuesSC;
                motorOlcuselKontrol.MaxAsinma = model.MaxAsinma;
                motorOlcuselKontrol.MaxOvallik = model.MaxOvallik;
                motorOlcuselKontrol.MaxKoniklik = model.MaxKoniklik;
                motorOlcuselKontrol.SilindirNo1 = model.SilindirNo1;
                motorOlcuselKontrol.SilindirNo2 = model.SilindirNo2;
                motorOlcuselKontrol.SilindirNo3 = model.SilindirNo3;
                motorOlcuselKontrol.GomlekFaturaStdDeger = model.GomlekFaturaStdDeger;
                motorOlcuselKontrol.GomlekFaturaTasma = concatenatedValuesGFT;
                motorOlcuselKontrol.GomlekYuvaCapi = concatenatedValuesGYC;
                motorOlcuselKontrol.PistonCapiStdDeger = model.PistonCapiStdDeger;
                motorOlcuselKontrol.PistonCapi = concatenatedValuesPC;
                motorOlcuselKontrol.AnaMuyluStdDeger = model.AnaMuyluStdDeger;
                motorOlcuselKontrol.AnaMuylu = concatenatedValuesAMA;
                motorOlcuselKontrol.KolMuyluStdDeger = model.KolMuyluStdDeger;
                motorOlcuselKontrol.KolMuylu = concatenatedValuesKMA;
                motorOlcuselKontrol.KrankMiliSalgiDegeri = model.KrankMiliSalgiDegeri;
                motorOlcuselKontrol.MotorIncelemeSonuc = model.MotorIncelemeSonuc != null ? string.Join(",", model.MotorIncelemeSonuc) : null;

                db.SaveChanges();
            }

            var newRecord = db.MotorOlcuselKontrol.FirstOrDefault(x => x.TalepId == model.TalepId);
            talep.MotorOlcuselKontrolId = newRecord.MotorOlcuselKontrolId;
            db.SaveChanges();

            return RedirectToAction("EngineInputDimensionalControl", new { id = model.TalepId });
        }

        [HttpGet]
        public ActionResult EngineOutputQuality(int id)
        {
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == id);
            var talepDetay = db.TalepDetay.FirstOrDefault(x => x.TalepId == id);
            var motorCikisKalite = db.EngineOutputQuality.FirstOrDefault(x => x.TalepId == id);

            var model = new EngineOutputQualityViewModel()
            {
                TeslimAlinanYedekParcalar = new List<List<string>>(),
                SilindirCaplari = new List<List<string>>(),
                GomlekBlokYukseklikleri = new List<List<string>>(),
                AnaMuyluDetay = new List<List<string>>(),
                KolMuyluDetay = new List<List<string>>(),
                PistonYukseklikleri = new List<List<string>>(),
                GerekliParcaOlcu = new List<string>()
            };

            var engineInfoDet = EngineInformationDet();

            model.EngineInfoDet = engineInfoDet;
            model.TalepId = talep.TalepId;
            if (talepDetay != null)
            {
                model.MotorDolapNo = talepDetay.MotorDolapNo;
                model.KapakDolapNo = talepDetay.KapakDolapNo;
            }

            if (motorCikisKalite != null)
            {
                var engineInfoDetList = motorCikisKalite.BlokKrankKolIsleri != null ? motorCikisKalite.BlokKrankKolIsleri.Split(',') : null;
                foreach (var item in engineInfoDet.Where(x => x.HdrId == 4).ToList())
                {
                    if (engineInfoDetList != null)
                    {
                        foreach (var itemList in engineInfoDetList)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }

                var engineInfoDetList2 = motorCikisKalite.OzelIslemler != null ? motorCikisKalite.OzelIslemler.Split(',') : null;
                foreach (var item in engineInfoDet.Where(x => x.HdrId == 5).ToList())
                {
                    if (engineInfoDetList2 != null)
                    {
                        foreach (var itemList in engineInfoDetList2)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }

                var engineInfoDetList3 = motorCikisKalite.OzelUretimler != null ? motorCikisKalite.OzelUretimler.Split(',') : null;
                foreach (var item in engineInfoDet.Where(x => x.HdrId == 6).ToList())
                {
                    if (engineInfoDetList3 != null)
                    {
                        foreach (var itemList in engineInfoDetList3)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }

                var adetList = motorCikisKalite.GerekliParcaOlcu != null ? motorCikisKalite.GerekliParcaOlcu.Split(';') : null;
                var gerekliParca = motorCikisKalite.GerekliParca != null ? motorCikisKalite.GerekliParca.Split(',') : null;
                foreach (var item in engineInfoDet.Where(x => x.HdrId == 7).ToList())
                {
                    if (gerekliParca != null)
                    {
                        foreach (var itemList in gerekliParca)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }

                    if (adetList != null)
                    {
                        if (adetList.Any(x => x.Split('-')[0] == item.Value))
                            model.GerekliParcaOlcu.Add(adetList.Where(x => x.Split('-')[0] == item.Value).FirstOrDefault().Split('-')[1]);
                        else
                            model.GerekliParcaOlcu.Add("");
                    }
                }

                model.AlinanDigerParcalar = motorCikisKalite.AlinanDigerParcalar;
                model.ParcaGirisSaati = motorCikisKalite.ParcaGirisSaati;
                model.TeslimAlan = motorCikisKalite.TeslimAlan;
                model.SilindirCaplariStdDeger = motorCikisKalite.SilindirCaplariStdDeger;
                model.GomlekYuksekligiStdDeger = motorCikisKalite.GomlekYuksekligiStdDeger;
                model.BlokYuzeyiTaslamaMiktari = motorCikisKalite.BlokYuzeyiTaslamaMiktari;
                model.AnaMuyluStdDeger = motorCikisKalite.AnaMuyluStdDeger;
                model.KolMuyluStdDeger = motorCikisKalite.KolMuyluStdDeger;
                model.KrankMiliGezintiDegeri = motorCikisKalite.KrankMiliGezintiDegeri;
                model.KrankMiliStdDeger = motorCikisKalite.KrankMiliStdDeger;
                model.PistonYuksekligiStdDeger = motorCikisKalite.PistonYuksekligiStdDeger;
                model.ContaTipi = motorCikisKalite.ContaTipi;
                model.Aciklama = motorCikisKalite.Aciklama;
                model.EngineInfoDet = engineInfoDet;

                #region Teslim Alinan Yedek Parcalar
                if (motorCikisKalite.TeslimAlinanYedekParcalar != null)
                {
                    string[] savedValues = motorCikisKalite.TeslimAlinanYedekParcalar.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 2; i++)
                    {
                        model.TeslimAlinanYedekParcalar.Add(new List<string>());

                        for (int j = 0; j < 7; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.TeslimAlinanYedekParcalar[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        model.TeslimAlinanYedekParcalar.Add(new List<string>());

                        for (int j = 0; j < 7; j++)
                        {
                            model.TeslimAlinanYedekParcalar[i].Add(string.Empty);
                        }
                    }
                }
                #endregion

                #region Silindir Çapları
                if (motorCikisKalite.SilindirCaplari != null)
                {
                    string[] savedValues = motorCikisKalite.SilindirCaplari.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 6; i++)
                    {
                        model.SilindirCaplari.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.SilindirCaplari[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        model.SilindirCaplari.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            model.SilindirCaplari[i].Add(string.Empty);
                        }
                    }
                }
                #endregion

                #region Blok Gömlek Yükseklikleri
                if (motorCikisKalite.GomlekBlokYukseklikleri != null)
                {
                    string[] savedValues = motorCikisKalite.GomlekBlokYukseklikleri.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 1; i++)
                    {
                        model.GomlekBlokYukseklikleri.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.GomlekBlokYukseklikleri[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 1; i++)
                    {
                        model.GomlekBlokYukseklikleri.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            model.GomlekBlokYukseklikleri[i].Add(string.Empty);
                        }
                    }
                }
                #endregion

                #region Ana Muylu
                if (motorCikisKalite.AnaMuyluDetay != null)
                {
                    string[] savedValues = motorCikisKalite.AnaMuyluDetay.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 3; i++)
                    {
                        model.AnaMuyluDetay.Add(new List<string>());

                        for (int j = 0; j < 10; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.AnaMuyluDetay[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        model.AnaMuyluDetay.Add(new List<string>());

                        for (int j = 0; j < 10; j++)
                        {
                            model.AnaMuyluDetay[i].Add(string.Empty);
                        }
                    }
                }
                #endregion

                #region Kol Muylu
                if (motorCikisKalite.KolMuyluDetay != null)
                {
                    string[] savedValues = motorCikisKalite.KolMuyluDetay.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 3; i++)
                    {
                        model.KolMuyluDetay.Add(new List<string>());

                        for (int j = 0; j < 10; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.KolMuyluDetay[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        model.KolMuyluDetay.Add(new List<string>());

                        for (int j = 0; j < 10; j++)
                        {
                            model.KolMuyluDetay[i].Add(string.Empty);
                        }
                    }
                }
                #endregion

                #region Piston Yükseklikleri
                if (motorCikisKalite.PistonYukseklikleri != null)
                {
                    string[] savedValues = motorCikisKalite.PistonYukseklikleri.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 1; i++)
                    {
                        model.PistonYukseklikleri.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.PistonYukseklikleri[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 1; i++)
                    {
                        model.PistonYukseklikleri.Add(new List<string>());

                        for (int j = 0; j < 12; j++)
                        {
                            model.PistonYukseklikleri[i].Add(string.Empty);
                        }
                    }
                }
                #endregion
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    model.TeslimAlinanYedekParcalar.Add(new List<string>());

                    for (int j = 0; j < 7; j++)
                    {
                        model.TeslimAlinanYedekParcalar[i].Add(string.Empty);
                    }
                }

                for (int i = 0; i < 6; i++)
                {
                    model.SilindirCaplari.Add(new List<string>());

                    for (int j = 0; j < 12; j++)
                    {
                        model.SilindirCaplari[i].Add(string.Empty);
                    }
                }

                for (int i = 0; i < 1; i++)
                {
                    model.GomlekBlokYukseklikleri.Add(new List<string>());

                    for (int j = 0; j < 12; j++)
                    {
                        model.GomlekBlokYukseklikleri[i].Add(string.Empty);
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    model.AnaMuyluDetay.Add(new List<string>());

                    for (int j = 0; j < 10; j++)
                    {
                        model.AnaMuyluDetay[i].Add(string.Empty);
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    model.KolMuyluDetay.Add(new List<string>());

                    for (int j = 0; j < 10; j++)
                    {
                        model.KolMuyluDetay[i].Add(string.Empty);
                    }
                }

                for (int i = 0; i < 1; i++)
                {
                    model.PistonYukseklikleri.Add(new List<string>());

                    for (int j = 0; j < 12; j++)
                    {
                        model.PistonYukseklikleri[i].Add(string.Empty);
                    }
                }
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult EngineOutputQuality(EngineOutputQualityViewModel model)
        {
            var motorCikisKalite = db.EngineOutputQuality.FirstOrDefault(x => x.TalepId == model.TalepId);
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == model.TalepId);

            string parcaTextMap = "";
            if (model.GerekliParcaOlcu.Count > 0)
            {
                if (model.GerekliParca.Count < model.GerekliParcaOlcu.Count)
                {
                    model.GerekliParcaOlcu = model.GerekliParcaOlcu.Where(x => x != "").ToList();
                }
                for (int i = 0; i < model.GerekliParcaOlcu.Count; i++)
                {
                    if (model.GerekliParcaOlcu[i] != "")
                    {
                        parcaTextMap = parcaTextMap + model.GerekliParca[i] + "-" + model.GerekliParcaOlcu[i] + ";";
                    }
                }
            }
            //if (model.GerekliParca != null) 

            #region Teslim Alinan Yedek Parcalar Birleştirme
            List<string> combinedTeslimAlinanYedekParcalar = new List<string>();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    string cellValue = model.TeslimAlinanYedekParcalar[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedTeslimAlinanYedekParcalar.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesTAYP = string.Join(";", combinedTeslimAlinanYedekParcalar);
            #endregion

            #region Silindir Çapları Birleştirme
            List<string> combinedSilindirCaplari = new List<string>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    string cellValue = model.SilindirCaplari[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedSilindirCaplari.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesSC = string.Join(";", combinedSilindirCaplari);
            #endregion

            #region Gömlek Blok Yükseklikleri Birleştirme
            List<string> combinedGomlekBlokYukseklikleri = new List<string>();
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    string cellValue = model.GomlekBlokYukseklikleri[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedGomlekBlokYukseklikleri.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesGBY = string.Join(";", combinedGomlekBlokYukseklikleri);
            #endregion

            #region Ana Muylu Detay Birleştirme
            List<string> combinedAnaMuyluDetay = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    string cellValue = model.AnaMuyluDetay[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedAnaMuyluDetay.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesAMD = string.Join(";", combinedAnaMuyluDetay);
            #endregion

            #region Kol Muylu Detay Birleştirme
            List<string> combinedKolMuyluDetay = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    string cellValue = model.KolMuyluDetay[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedKolMuyluDetay.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesKMD = string.Join(";", combinedKolMuyluDetay);
            #endregion

            #region Piston Yükseklikleri Birleştirme
            List<string> combinedPistonYukseklikleri = new List<string>();
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    string cellValue = model.PistonYukseklikleri[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedPistonYukseklikleri.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesPY = string.Join(";", combinedPistonYukseklikleri);
            #endregion

            if (motorCikisKalite == null)
            {
                using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        EngineOutputQuality newEngineQuality = new EngineOutputQuality()
                        {
                            TalepId = model.TalepId,
                            BlokKrankKolIsleri = model.BlokKrankKolIsleri != null ? string.Join(",", model.BlokKrankKolIsleri) : null,
                            OzelIslemler = model.OzelIslemler != null ? string.Join(",", model.OzelIslemler) : null,
                            OzelUretimler = model.OzelIslemler != null ? string.Join(",", model.OzelIslemler) : null,
                            GerekliParca = model.GerekliParca != null ? string.Join(",", model.GerekliParca) : null,
                            GerekliParcaOlcu = parcaTextMap,
                            TeslimAlinanYedekParcalar = concatenatedValuesTAYP,
                            AlinanDigerParcalar = model.AlinanDigerParcalar,
                            ParcaGirisSaati = model.ParcaGirisSaati,
                            TeslimAlan = model.TeslimAlan,
                            SilindirCaplariStdDeger = model.SilindirCaplariStdDeger,
                            SilindirCaplari = concatenatedValuesSC,
                            GomlekYuksekligiStdDeger = model.GomlekYuksekligiStdDeger,
                            GomlekBlokYukseklikleri = concatenatedValuesGBY,
                            AnaMuyluDetay = concatenatedValuesAMD,
                            KolMuyluDetay = concatenatedValuesKMD,
                            PistonYukseklikleri = concatenatedValuesPY,
                            BlokYuzeyiTaslamaMiktari = model.BlokYuzeyiTaslamaMiktari,
                            AnaMuyluStdDeger = model.AnaMuyluStdDeger,
                            KolMuyluStdDeger = model.KolMuyluStdDeger,
                            KrankMiliGezintiDegeri = model.KrankMiliGezintiDegeri,
                            KrankMiliStdDeger = model.KrankMiliStdDeger,
                            PistonYuksekligiStdDeger = model.PistonYuksekligiStdDeger,
                            ContaTipi = model.ContaTipi,
                            Aciklama = model.Aciklama,
                        };

                        context.EngineOutputQuality.Add(newEngineQuality);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            else
            {
                motorCikisKalite.TalepId = model.TalepId;
                motorCikisKalite.BlokKrankKolIsleri = model.BlokKrankKolIsleri != null ? string.Join(",", model.BlokKrankKolIsleri) : null;
                motorCikisKalite.OzelIslemler = model.OzelIslemler != null ? string.Join(",", model.OzelIslemler) : null;
                motorCikisKalite.OzelUretimler = model.OzelUretimler != null ? string.Join(",", model.OzelUretimler) : null;
                motorCikisKalite.GerekliParca = model.GerekliParca != null ? string.Join(",", model.GerekliParca) : null;
                motorCikisKalite.GerekliParcaOlcu = parcaTextMap;
                motorCikisKalite.TeslimAlinanYedekParcalar = concatenatedValuesTAYP;
                motorCikisKalite.AlinanDigerParcalar = model.AlinanDigerParcalar;
                motorCikisKalite.ParcaGirisSaati = model.ParcaGirisSaati;
                motorCikisKalite.TeslimAlan = model.TeslimAlan;
                motorCikisKalite.SilindirCaplariStdDeger = model.SilindirCaplariStdDeger;
                motorCikisKalite.SilindirCaplari = concatenatedValuesSC;
                motorCikisKalite.GomlekYuksekligiStdDeger = model.GomlekYuksekligiStdDeger;
                motorCikisKalite.GomlekBlokYukseklikleri = concatenatedValuesGBY;
                motorCikisKalite.AnaMuyluDetay = concatenatedValuesAMD;
                motorCikisKalite.KolMuyluDetay = concatenatedValuesKMD;
                motorCikisKalite.PistonYukseklikleri = concatenatedValuesPY;
                motorCikisKalite.BlokYuzeyiTaslamaMiktari = model.BlokYuzeyiTaslamaMiktari;
                motorCikisKalite.AnaMuyluStdDeger = model.AnaMuyluStdDeger;
                motorCikisKalite.KolMuyluStdDeger = model.KolMuyluStdDeger;
                motorCikisKalite.KrankMiliGezintiDegeri = model.KrankMiliGezintiDegeri;
                motorCikisKalite.KrankMiliStdDeger = model.KrankMiliStdDeger;
                motorCikisKalite.PistonYuksekligiStdDeger = model.PistonYuksekligiStdDeger;
                motorCikisKalite.ContaTipi = model.ContaTipi;
                motorCikisKalite.Aciklama = model.Aciklama;

                db.SaveChanges();
            }

            var newRecord = db.EngineOutputQuality.FirstOrDefault(x => x.TalepId == model.TalepId);
            talep.MotorCikisKaliteId = newRecord.MotorCikisKaliteId;
            db.SaveChanges();

            return RedirectToAction("EngineOutputQuality", new { id = model.TalepId });
        }



        [HttpGet]
        public ActionResult CapInputQuality(int id)
        {
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == id);
            var talepDetay = db.TalepDetay.FirstOrDefault(x => x.TalepId == id);
            var kapakGirisKalite = db.CapInputQuality.FirstOrDefault(x => x.TalepId == id);

            var model = new CapInputQualityViewModel()
            {
                YedekParcaGirisKontrol = new List<List<string>>(),
                SupapDusukluguYuksekligi = new List<List<string>>(),
                GerekliParcaAdet = new List<string>()
            };

            var engineInfoDet = EngineInformationDet();

            model.EngineInfoDet = engineInfoDet;
            model.TalepId = talep.TalepId;
            if (talepDetay != null)
            {
                model.MotorDolapNo = talepDetay.MotorDolapNo;
                model.KapakDolapNo = talepDetay.KapakDolapNo;
            }

            if (kapakGirisKalite != null)
            {
                var engineInfoDetList = kapakGirisKalite.IncelemeSonuclari != null ? kapakGirisKalite.IncelemeSonuclari.Split(',') : null;
                foreach (var item in engineInfoDet.Where(x => x.HdrId == 8).ToList())
                {
                    if (engineInfoDetList != null)
                    {
                        foreach (var itemList in engineInfoDetList)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }

                var engineInfoDetList2 = kapakGirisKalite.OzelIslemler != null ? kapakGirisKalite.OzelIslemler.Split(',') : null;
                foreach (var item in engineInfoDet.Where(x => x.HdrId == 10).ToList())
                {
                    if (engineInfoDetList2 != null)
                    {
                        foreach (var itemList in engineInfoDetList2)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }

                var engineInfoDetList3 = kapakGirisKalite.OzelUretimler != null ? kapakGirisKalite.OzelUretimler.Split(',') : null;
                foreach (var item in engineInfoDet.Where(x => x.HdrId == 11).ToList())
                {
                    if (engineInfoDetList3 != null)
                    {
                        foreach (var itemList in engineInfoDetList3)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }

                var engineInfoDetList4 = kapakGirisKalite.YapilacakIslemler != null ? kapakGirisKalite.YapilacakIslemler.Split(',') : null;
                foreach (var item in engineInfoDet.Where(x => x.HdrId == 9).ToList())
                {
                    if (engineInfoDetList4 != null)
                    {
                        foreach (var itemList in engineInfoDetList4)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }

                var engineInfoDetList5 = kapakGirisKalite.YapilanIsler != null ? kapakGirisKalite.YapilanIsler.Split(',') : null;
                foreach (var item in engineInfoDet.Where(x => x.HdrId == 13).ToList())
                {
                    if (engineInfoDetList5 != null)
                    {
                        foreach (var itemList in engineInfoDetList5)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }
                }

                var adetList = kapakGirisKalite.GerekliParcaAdet != null ? kapakGirisKalite.GerekliParcaAdet.Split(';') : null;
                var engineInfoDetList6 = kapakGirisKalite.GerekliParcalar != null ? kapakGirisKalite.GerekliParcalar.Split(',') : null;

                foreach (var item in engineInfoDet.Where(x => x.HdrId == 12).ToList())
                {
                    if (engineInfoDetList6 != null)
                    {
                        foreach (var itemList in engineInfoDetList6)
                        {
                            if (item.Value == itemList)
                                item.Selected = true;
                        }
                    }

                    if (adetList != null)
                    {
                        if (adetList.Any(x => x.Split('-')[0] == item.Value))
                            model.GerekliParcaAdet.Add(adetList.Where(x => x.Split('-')[0] == item.Value).FirstOrDefault().Split('-')[1]);
                        else
                            model.GerekliParcaAdet.Add("");
                    }
                }

                model.IzınVerilenMinKapakKalinligi = kapakGirisKalite.IzinVerilenMinKapakKalinligi;
                model.IzınVerilenMaxKapakEgriligi = kapakGirisKalite.IzinVerilenMaxKapakEgriligi;
                model.OlculenKapakKalinligi = kapakGirisKalite.OlculenKapakKalinligi;
                model.OlculenKapakEgriligi = kapakGirisKalite.OlculenKapakEgriligi;
                model.GerekliParcaYorumlar = kapakGirisKalite.GerekliParcaYorumlar;
                model.ParcaGirisTarihi = kapakGirisKalite.ParcaGirisTarihi;
                model.TeslimAlan = kapakGirisKalite.TeslimAlan;
                model.GerceklestirilenTasmaMiktari = kapakGirisKalite.GerceklestirilenTasmaMiktari;
                model.RevizyonSonrasiKapakKalinligi = kapakGirisKalite.RevizyonSonrasiKapakKalinligi;
                model.SupapAyariEmme = kapakGirisKalite.SupapAyariEmme;
                model.SupapAyariEgzoz = kapakGirisKalite.SupapAyariEgzoz;
                model.YatakSikmaTorku1 = kapakGirisKalite.YatakSikmaTorku1;
                model.YatakSikmaTorku2 = kapakGirisKalite.YatakSikmaTorku2;
                model.YatakSikmaTorku3 = kapakGirisKalite.YatakSikmaTorku3;
                model.YatakSikmaTorku4 = kapakGirisKalite.YatakSikmaTorku4;
                model.KapakSikmaTorku1 = kapakGirisKalite.KapakSikmaTorku1;
                model.KapakSikmaTorku2 = kapakGirisKalite.KapakSikmaTorku2;
                model.KapakSikmaTorku3 = kapakGirisKalite.KapakSikmaTorku3;
                model.KapakSikmaTorku4 = kapakGirisKalite.KapakSikmaTorku4;
                model.Aciklama = kapakGirisKalite.Aciklama;
                model.EngineInfoDet = engineInfoDet;

                #region Yedek Parça Giriş Kontrol
                if (kapakGirisKalite.YedekParcaGirisKontrol != null)
                {
                    string[] savedValues = kapakGirisKalite.YedekParcaGirisKontrol.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 2; i++)
                    {
                        model.YedekParcaGirisKontrol.Add(new List<string>());

                        for (int j = 0; j < 6; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.YedekParcaGirisKontrol[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        model.YedekParcaGirisKontrol.Add(new List<string>());

                        for (int j = 0; j < 6; j++)
                        {
                            model.YedekParcaGirisKontrol[i].Add(string.Empty);
                        }
                    }
                }
                #endregion

                #region Supap Düşüklüğü Yüksekliği
                if (kapakGirisKalite.SupapDusukluguYuksekligi != null)
                {
                    string[] savedValues = kapakGirisKalite.SupapDusukluguYuksekligi.Split(new[] { "-;" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 4; i++)
                    {
                        model.SupapDusukluguYuksekligi.Add(new List<string>());

                        for (int j = 0; j < 8; j++)
                        {
                            string cellValue = savedValues.FirstOrDefault(v => v.StartsWith($"-{i + 1}-{j + 1}-"))?.Split('-')[3] ?? string.Empty;
                            model.SupapDusukluguYuksekligi[i].Add(cellValue);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        model.SupapDusukluguYuksekligi.Add(new List<string>());

                        for (int j = 0; j < 8; j++)
                        {
                            model.SupapDusukluguYuksekligi[i].Add(string.Empty);
                        }
                    }
                }
                #endregion
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    model.YedekParcaGirisKontrol.Add(new List<string>());

                    for (int j = 0; j < 6; j++)
                    {
                        model.YedekParcaGirisKontrol[i].Add(string.Empty);
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    model.SupapDusukluguYuksekligi.Add(new List<string>());

                    for (int j = 0; j < 8; j++)
                    {
                        model.SupapDusukluguYuksekligi[i].Add(string.Empty);
                    }
                }
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult CapInputQuality(CapInputQualityViewModel model)
        {
            var kapakGirisKalite = db.CapInputQuality.FirstOrDefault(x => x.TalepId == model.TalepId);
            var talep = db.Talepler.FirstOrDefault(x => x.TalepId == model.TalepId);

            string parcaTextMap = "";
            if (model.GerekliParcaAdet.Count > 0)
            {
                if (model.GerekliParcalar.Count < model.GerekliParcaAdet.Count)
                {
                    model.GerekliParcaAdet = model.GerekliParcaAdet.Where(x => x != "").ToList();
                }

                for (int i = 0; i < model.GerekliParcaAdet.Count; i++)
                {
                    if (model.GerekliParcaAdet[i] != "")
                    {
                        parcaTextMap = parcaTextMap + model.GerekliParcalar[i] + "-" + model.GerekliParcaAdet[i] + ";";
                    }
                }
            }

            #region Yedek Parca Giris Kontrol Birleştirme
            List<string> combinedYedekParcaGirisKontrol = new List<string>();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    string cellValue = model.YedekParcaGirisKontrol[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedYedekParcaGirisKontrol.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesYPGK = string.Join(";", combinedYedekParcaGirisKontrol);
            #endregion

            #region Supap Dusuklugu Yuksekligi Birleştirme
            List<string> combinedSupapDusukluguYuksekligi = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    string cellValue = model.SupapDusukluguYuksekligi[i][j];
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string combinedCell = $"-{i + 1}-{j + 1}-{cellValue}-";
                        combinedSupapDusukluguYuksekligi.Add(combinedCell);
                    }
                }
            }

            string concatenatedValuesSDY = string.Join(";", combinedSupapDusukluguYuksekligi);
            #endregion

            if (kapakGirisKalite == null)
            {
                using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        CapInputQuality yeniKapakGirisKalite = new CapInputQuality()
                        {
                            TalepId = model.TalepId,
                            IzinVerilenMinKapakKalinligi = model.IzınVerilenMinKapakKalinligi,
                            IzinVerilenMaxKapakEgriligi = model.IzınVerilenMaxKapakEgriligi,
                            OlculenKapakKalinligi = model.OlculenKapakKalinligi,
                            OlculenKapakEgriligi = model.OlculenKapakEgriligi,
                            IncelemeSonuclari = model.IncelemeSonuclari != null ? string.Join(",", model.IncelemeSonuclari) : null,
                            YapilacakIslemler = model.YapilacakIslemler != null ? string.Join(",", model.YapilacakIslemler) : null,
                            OzelIslemler = model.OzelIslemler != null ? string.Join(",", model.OzelIslemler) : null,
                            OzelUretimler = model.OzelUretimler != null ? string.Join(",", model.OzelUretimler) : null,
                            GerekliParcalar = model.GerekliParcalar != null ? string.Join(",", model.GerekliParcalar) : null,
                            GerekliParcaYorumlar = model.GerekliParcaYorumlar,
                            GerekliParcaAdet = parcaTextMap,
                            YedekParcaGirisKontrol = concatenatedValuesYPGK,
                            ParcaGirisTarihi = model.ParcaGirisTarihi,
                            TeslimAlan = model.TeslimAlan,
                            SupapDusukluguYuksekligi = concatenatedValuesSDY,
                            GerceklestirilenTasmaMiktari = model.GerceklestirilenTasmaMiktari,
                            RevizyonSonrasiKapakKalinligi = model.RevizyonSonrasiKapakKalinligi,
                            SupapAyariEmme = model.SupapAyariEmme,
                            SupapAyariEgzoz = model.SupapAyariEgzoz,
                            YatakSikmaTorku1 = model.YatakSikmaTorku1,
                            YatakSikmaTorku2 = model.YatakSikmaTorku2,
                            YatakSikmaTorku3 = model.YatakSikmaTorku3,
                            YatakSikmaTorku4 = model.YatakSikmaTorku4,
                            KapakSikmaTorku1 = model.KapakSikmaTorku1,
                            KapakSikmaTorku2 = model.KapakSikmaTorku2,
                            KapakSikmaTorku3 = model.KapakSikmaTorku3,
                            KapakSikmaTorku4 = model.KapakSikmaTorku4,
                            YapilanIsler = model.YapilanIsler != null ? string.Join(",", model.YapilanIsler) : null,
                            Aciklama = model.Aciklama
                        };

                        context.CapInputQuality.Add(yeniKapakGirisKalite);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            else
            {
                kapakGirisKalite.TalepId = model.TalepId;
                kapakGirisKalite.IzinVerilenMinKapakKalinligi = model.IzınVerilenMinKapakKalinligi;
                kapakGirisKalite.IzinVerilenMaxKapakEgriligi = model.IzınVerilenMaxKapakEgriligi;
                kapakGirisKalite.OlculenKapakKalinligi = model.OlculenKapakKalinligi;
                kapakGirisKalite.OlculenKapakEgriligi = model.OlculenKapakEgriligi;
                kapakGirisKalite.IncelemeSonuclari = model.IncelemeSonuclari != null ? string.Join(",", model.IncelemeSonuclari) : null;
                kapakGirisKalite.YapilacakIslemler = model.YapilacakIslemler != null ? string.Join(",", model.YapilacakIslemler) : null;
                kapakGirisKalite.OzelIslemler = model.OzelIslemler != null ? string.Join(",", model.OzelIslemler) : null;
                kapakGirisKalite.OzelUretimler = model.OzelUretimler != null ? string.Join(",", model.OzelUretimler) : null;
                kapakGirisKalite.GerekliParcalar = model.GerekliParcalar != null ? string.Join(",", model.GerekliParcalar) : null;
                kapakGirisKalite.GerekliParcaYorumlar = model.GerekliParcaYorumlar;
                kapakGirisKalite.GerekliParcaAdet = parcaTextMap;
                kapakGirisKalite.YedekParcaGirisKontrol = concatenatedValuesYPGK;
                kapakGirisKalite.ParcaGirisTarihi = model.ParcaGirisTarihi;
                kapakGirisKalite.TeslimAlan = model.TeslimAlan;
                kapakGirisKalite.SupapDusukluguYuksekligi = concatenatedValuesSDY;
                kapakGirisKalite.GerceklestirilenTasmaMiktari = model.GerceklestirilenTasmaMiktari;
                kapakGirisKalite.RevizyonSonrasiKapakKalinligi = model.RevizyonSonrasiKapakKalinligi;
                kapakGirisKalite.SupapAyariEmme = model.SupapAyariEmme;
                kapakGirisKalite.SupapAyariEgzoz = model.SupapAyariEgzoz;
                kapakGirisKalite.YatakSikmaTorku1 = model.YatakSikmaTorku1;
                kapakGirisKalite.YatakSikmaTorku2 = model.YatakSikmaTorku2;
                kapakGirisKalite.YatakSikmaTorku3 = model.YatakSikmaTorku3;
                kapakGirisKalite.YatakSikmaTorku4 = model.YatakSikmaTorku4;
                kapakGirisKalite.KapakSikmaTorku1 = model.KapakSikmaTorku1;
                kapakGirisKalite.KapakSikmaTorku2 = model.KapakSikmaTorku2;
                kapakGirisKalite.KapakSikmaTorku3 = model.KapakSikmaTorku3;
                kapakGirisKalite.KapakSikmaTorku4 = model.KapakSikmaTorku4;
                kapakGirisKalite.YapilanIsler = model.YapilanIsler != null ? string.Join(",", model.YapilanIsler) : null;
                kapakGirisKalite.Aciklama = model.Aciklama;

                db.SaveChanges();
            }

            var newRecord = db.CapInputQuality.FirstOrDefault(x => x.TalepId == model.TalepId);
            talep.KapakGirisKaliteId = newRecord.KapakGirisKaliteId;
            db.SaveChanges();

            return RedirectToAction("CapInputQuality", new { id = model.TalepId });
        }
    }
}