using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Helpers;
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
            var talepler = db.Talepler.ToList();
            var model = new List<TicketListModel>();
            var isDanisman = SFHelper.CheckMyRole(currentUser, "DANISMAN");

            foreach (var item in talepler)
            {
                model.Add(new TicketListModel
                {
                    AtananKisi = item.AtananSofor,
                    Musteri = item.Musteri.MusteriTipi == "B" ? (item.Musteri.MusteriAdi.ToString() + " " + item.Musteri.MusteriSoyadi.ToString()) : item.Musteri.KurumAdi.ToString(),
                    MusteriAramaTarihi = item.MusteriAramaTarihi,
                    OlustuanKisi = item.Creator,
                    TalepNo = item.TalepId
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
                model.VehicleId = talepler.VehicleId.HasValue ? talepler.VehicleId.Value : 0; ;
                model.AracGrubuId = talepler.AracGrupId.HasValue ? talepler.AracGrupId.Value : 0;
                model.Id = talepler.TalepId;
            }

            FillIsEmriCombos();
            return View(model);
        }

        [SessionAuthorization]
        public ActionResult DetailEdit(int id)
        {
            var talepler = db.Talepler.FirstOrDefault(x => x.TalepId == id);
            TicketDetailViewModel model = new TicketDetailViewModel();
            model.TicketId = id;
            FillIsEmriCombos();
            model.Ariza = ArizaListesi();
            model.Parcalar = TamirListesi();

            return View(model);
        }


        [HttpPost]
        public ActionResult DetailEdit(TicketDetailViewModel model)
        {
            string parcaTextMap = "";

            for (int i = 0; i < model.ParcalarText.Count; i++)
            {
                if (model.ParcalarText[i] != "0")
                {
                    parcaTextMap = parcaTextMap + model.ParcalarId[i] + "-" + model.ParcalarText[i] + ";";
                }
            }

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
            yeniTalep.AracGrupId = model.AracGrubuId;
            yeniTalep.CreatedDate = model.KayitTarihi ?? System.DateTime.Now;
            yeniTalep.Creator = this.userName;
            yeniTalep.Modifier = null;
            yeniTalep.ModifiedDate = null;
            yeniTalep.Km = model.KM;
            yeniTalep.VinNo = model.SaseNo;
            yeniTalep.Plate = model.Plate;
            yeniTalep.PartId = model.PartId;
            yeniTalep.VehicleId = model.VehicleId;

            db.SaveChanges();
            FillIsEmriCombos();
            return View(model);
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
                    db.Talepler.Where(x => x.TalepId == model.Id).FirstOrDefault();
                    var musteriModel = context.Musteri.Find(model.HizliIsEmriVehicleMusteriId);
                    //2. Bilet 
                    Talepler yeniTalep = new Talepler();
                    yeniTalep.MusteriId = musteriModel.MusteriId;
                    yeniTalep.ArayanKisi = model.ArayanKisiAdSoyad;
                    yeniTalep.MusteriAramaTarihi = model.MusteriAramaTarihi;
                    yeniTalep.Note = model.Not;
                    yeniTalep.AtananSofor = model.SoforUserName;
                    yeniTalep.AracGrupId = model.AracGrubuId;
                    yeniTalep.CreatedDate = model.KayitTarihi ?? System.DateTime.Now;
                    yeniTalep.Creator = this.userName;
                    yeniTalep.Modifier = null;
                    yeniTalep.ModifiedDate = null;
                    yeniTalep.Km = model.KM;
                    yeniTalep.VinNo = model.SaseNo;
                    yeniTalep.Plate = model.Plate;
                    yeniTalep.PartId = model.PartId;
                    yeniTalep.VehicleId = model.VehicleId;

                    context.Talepler.Add(yeniTalep);
                    context.SaveChanges();
                    transaction.Commit();
                }
            }

            return View();
        }

        public void FillIsEmriCombos()
        {
            ViewBag.AracGrubu = new SelectList(db.AracGrubu.ToList(), "AracGrubuId", "Aciklamasi");
            ViewBag.Soforler = new SelectList(db.UserRoles.Where(x => x.Roles.RoleName == "DANISMAN").OrderBy(x => x.UserName).Select(x => new { UserName = x.UserName, DanismanAdi = x.Users.FirstName + " " + x.Users.FirstName }).ToList(), "UserName", "DanismanAdi");
            ViewBag.Parts = new SelectList(db.Parts.Select(x => new { PartId = x.PartId, PartName = x.Name }).ToList(), "PartId", "PartName");
            ViewBag.Markalar = db.Companies.ToList();
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
    }
}