using Newtonsoft.Json;
using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Helpers;
using SahinRektefiyeSoln.Helpers.LogHelper;
using SahinRektefiyeSoln.Infrastructure;
using SahinRektefiyeSoln.Models;
using SahinRektefiyeSoln.Models.HelperModels;
using SahinRektefiyeSoln.Models.PagingModels;
using SahinRektefiyeSoln.Models.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vereyon.Web;

namespace SahinRektefiyeSoln.Controllers
{
	[LogActionFilter]
	public class MusteriController : BaseController
	{

		const int aracSorgulamaPageCount = 10;

		public string currentUser
		{
			get
			{
				return Session["UserName"].ToString();
			}
		}

		SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();

		[SessionAuthorization]
		public ActionResult Index()
		{
			return View();
		}

		[SessionAuthorization]
		public ActionResult Create()
		{
			FillMusteriCreateCombos();
			return View();
		}

		[HttpPost]
		[SessionAuthorization]
		public ActionResult Create(CreateMusteriPageModel model)
		{
			try
			{
				int LogoMusteriId = 0;
				using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
				{
					using (var transaction = context.Database.BeginTransaction())
					{
						
						//Musteriyi Oluştur

						if (model.musteri.MusteriTipi == "B")
						{
							#region KurumsaldanGelenBilgilerTemizlenir
							model.musteri.KurumAdi = string.Empty;
							model.musteri.KontakAdi = string.Empty;
							model.musteri.KontakSoyadi = string.Empty;
							model.musteri.KontakKategoriId = null;

							#endregion

							model.musteri.IlId = SFHelper.ReturnNullableInt(model.IlId);
							model.musteri.IlceId = SFHelper.ReturnNullableInt(model.IlceId);
							model.musteri.MeslekId = SFHelper.ReturnNullableInt(model.MeslekId);
							model.musteri.IlId = SFHelper.ReturnNullableInt(model.IlId);
							model.musteri.IlceId = SFHelper.ReturnNullableInt(model.IlceId);
							//model.musteri.VergiDairesiIlId = RevmerHelper.ReturnNullableInt(model.VergiDaireIlId);
							//model.musteri.VergiDairesiIlceId = RevmerHelper.ReturnNullableInt(model.VergiDaireIlceId);


						}
						else if (model.musteri.MusteriTipi == "K")
						{
							#region BireyseldenGelenBilgilerTemizlenir

							model.musteri.MusteriAdi = string.Empty;
							model.musteri.MusteriSoyadi = string.Empty;
							model.musteri.TCKN = string.Empty;

							#endregion

							model.musteri.KurumTipId = SFHelper.ReturnNullableInt(model.KurumTipId);
							model.musteri.KontakKategoriId = SFHelper.ReturnNullableInt(model.KontakKatId);
							//model.musteri.VergiDairesiIlId = RevmerHelper.ReturnNullableInt(model.VergiDaireIlId);
							//model.musteri.VergiDairesiIlceId = RevmerHelper.ReturnNullableInt(model.VergiDaireIlceId);
							model.musteri.IlId = SFHelper.ReturnNullableInt(model.IlId);
							model.musteri.IlceId = SFHelper.ReturnNullableInt(model.IlceId);
						}

						model.musteri.CreatedDate = System.DateTime.Now;
						model.musteri.Creator = currentUser;

						// call the stored procedure function import   
						//var results = context.GetNextCariKod();

						// from the results, get the first/single value
						//long? nextSequenceValue = results.Single();

						model.musteri.CariKod = "C_" + Convert.ToString(123);

						context.Musteri.Add(model.musteri);
						context.SaveChanges();

						if (!string.IsNullOrEmpty(model.Mail))
						{
							//Musteri Mail Adresi Oluştur 
							MusteriMail yeniMusteriMail = new MusteriMail();
							yeniMusteriMail.SwOnay = false;
							yeniMusteriMail.MailAdresi = model.Mail;
							yeniMusteriMail.MusteriId = model.musteri.MusteriId;

							yeniMusteriMail.DtCreated = System.DateTime.Now;
							yeniMusteriMail.Creator = currentUser;

							context.MusteriMail.Add(yeniMusteriMail);
							context.SaveChanges();
						}

						if (!string.IsNullOrEmpty(model.Telefon))
						{
							//Müşteri Telefonu Oluştur

							MusteriTelefon yeniMusteriTelefon = new MusteriTelefon();
							yeniMusteriTelefon.TelefonNumarasi = model.Telefon;
							yeniMusteriTelefon.SwOnaySms = false;
							yeniMusteriTelefon.SwOnayArama = false;
							yeniMusteriTelefon.SwPrefered = true;
							yeniMusteriTelefon.MusteriId = model.musteri.MusteriId;

							yeniMusteriTelefon.Creator = currentUser;
							yeniMusteriTelefon.DtCreated = System.DateTime.Now;

							context.MusteriTelefon.Add(yeniMusteriTelefon);
							context.SaveChanges();
						}

						//LogoMusteriId = LogoNewMusteri(model);

						transaction.Commit();
					}
				}

				if (LogoMusteriId != 0)
				{
					Musteri musteri = db.Musteri.Find(model.musteri.MusteriId);
					musteri.MusteriLogoId = LogoMusteriId;
					db.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				ExceptionLog log = new ExceptionLog();
				log.Message = JsonConvert.SerializeObject(ex);

				SFHelper.MailGonder(log,"Musteri olusturulurken hata");

				FillMusteriCreateCombos();
				FlashMessage.Warning("Müşteri oluşturulurken hata oluştu, lütfen değerleri kontrol ediniz.");
				return View(model);
			}

			return RedirectToAction("MusteriDuzenle", "Musteri", new { MusteriId = model.musteri.MusteriId });
		}

		[SessionAuthorization]
		public ActionResult MusteriAra(int? page)
		{
			int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

			MusteriSearchPageModel model = new MusteriSearchPageModel();

			model.musteriler = db.Musteri.OrderByDescending(m => m.CreatedDate).ToPagedList<Musteri>(currentPageIndex, aracSorgulamaPageCount);

			return View(model);
		}

		[HttpPost]
		[SessionAuthorization]
		public ActionResult MusteriAra(MusteriSearchPageModel model, int? sayfaNo)
		{
			int _sayfaNo = sayfaNo ?? 0;

			List<Musteri> musteriler = new List<Musteri>();
			musteriler = db.Musteri.ToList();

			List<Musteri> result= new List<Musteri>();


			if (!string.IsNullOrEmpty(model.filter.MusteriAdi))
			{	
				List<Musteri> kurumlar = new List<Musteri>();
				kurumlar = musteriler.Where(x=>x.KurumAdi.ToUpper().Contains(model.filter.MusteriAdi.ToUpper())).ToList();

				List<Musteri> musteriAdi = new List<Musteri>();
				musteriAdi = musteriler.Where(x => (x.MusteriAdi != null ? x.MusteriAdi.ToUpper() : "" + x.MusteriSoyadi != null ? x.MusteriSoyadi.ToUpper() : "").Contains(model.filter.MusteriAdi.ToUpper())).ToList();

				List<Musteri> kontakAdi = new List<Musteri>();

				kontakAdi = musteriler.Where(x=>(x.KontakAdi != null ? x.KontakAdi.ToUpper() : " " ).Contains(model.filter.MusteriAdi.ToUpper())).ToList();

				result = kurumlar.Concat(musteriAdi).Concat(kontakAdi).ToList();

				//myList1 = myList1.Concat(myList2).ToList();
				//musteriler = musteriler.Where(x => x.KurumAdi.ToUpper().Contains(model.filter.MusteriAdi.ToUpper()) ||
				//								   (x.MusteriAdi != null ? x.MusteriAdi.ToUpper() : "" + x.MusteriSoyadi != null ? x.MusteriSoyadi.ToUpper() : "").Contains(model.filter.MusteriAdi.ToUpper()) ||
				//								   (x.KontakAdi != null ? x.KontakAdi.ToUpper() : "" + x.KontakSoyadi != null ? x.KontakSoyadi.ToUpper() : "").Contains(model.filter.MusteriAdi.ToUpper()

				//								   );

				//)
				//.ToList();
			}




			if (!string.IsNullOrEmpty(model.filter.TCKN))
			{
				List<Musteri> tcVergiNo = new List<Musteri>();
				tcVergiNo = musteriler.Where(x => x.TCKN.ToUpper().Contains(model.filter.TCKN.ToUpper()) ||
												   x.VergiNo.ToUpper().Contains(model.filter.TCKN.ToUpper())
												   ).ToList();

				result = result.Concat(tcVergiNo).ToList();
			}

			

			model.musteriler = result.OrderByDescending(m => m.CreatedDate).ToPagedList<Musteri>(_sayfaNo, aracSorgulamaPageCount);

			return View(model);
		}

		[SessionAuthorization]
		public ActionResult MusteriDuzenle(int MusteriId)
		{
			MusteriEditPageModel model = new MusteriEditPageModel();
			if (MusteriId != 0)
			{
				//1. Müşteri bulunacak
				Musteri editEdilecekMusteri = db.Musteri.Find(MusteriId);

				model.musteri = editEdilecekMusteri;

				model.IlId = editEdilecekMusteri.IlId;
				model.IlceId = editEdilecekMusteri.IlceId;
				model.KurumTipId = editEdilecekMusteri.KurumTipId;
				model.KontakKatId = editEdilecekMusteri.KontakKategoriId;
				model.MeslekId = editEdilecekMusteri.MeslekId;
				
				if (editEdilecekMusteri.MusteriMail != null)
				{
					if (editEdilecekMusteri.MusteriMail.Count > 0)
						model.Mail = editEdilecekMusteri.MusteriMail.First()?.MailAdresi;
				}


				if (editEdilecekMusteri.MusteriTelefon != null)
				{
					if (editEdilecekMusteri.MusteriTelefon.Count > 0)
						model.Telefon = editEdilecekMusteri.MusteriTelefon.First()?.TelefonNumarasi;
				}


				FillMusteriCreateCombos(model.IlId == null ? 0 : (int)model.IlId, model.VergiDaireIlId == null ? 0 : (int)model.VergiDaireIlId);

			}
			else
			{
				throw new Exception("Musteri Bulunamadı.");
			}

			return View(model);
		}

		[SessionAuthorization]
		[HttpPost]
		public ActionResult MusteriDuzenle(MusteriEditPageModel model)
		{
			Musteri musteri = db.Musteri.Where(x => x.MusteriId == model.musteri.MusteriId).FirstOrDefault();
			if (model.musteri.MusteriTipi == "B")
			{
				#region KurumsaldanGelenBilgilerTemizlenir
				musteri.KurumAdi = string.Empty;
				musteri.KontakAdi = string.Empty;
				musteri.KontakSoyadi = string.Empty;
				musteri.KontakKategoriId = null;

				#endregion
				musteri.MusteriAdi = model.musteri.MusteriAdi;
				musteri.MusteriSoyadi = model.musteri.MusteriSoyadi;
				musteri.TCKN = model.musteri.TCKN;


				musteri.IlId = SFHelper.ReturnNullableInt(model.IlId);
				musteri.IlceId = SFHelper.ReturnNullableInt(model.IlceId);

				musteri.IlId = SFHelper.ReturnNullableInt(model.IlId);
				musteri.IlceId = SFHelper.ReturnNullableInt(model.IlceId);
				//musteri.VergiDairesiIlId = RevmerHelper.ReturnNullableInt(model.VergiDaireIlId);
				//musteri.VergiDairesiIlceId = RevmerHelper.ReturnNullableInt(model.VergiDaireIlceId);


			}
			else if (model.musteri.MusteriTipi == "K")
			{
				#region BireyseldenGelenBilgilerTemizlenir

				musteri.MusteriAdi = string.Empty;
				musteri.MusteriSoyadi = string.Empty;
				musteri.TCKN = string.Empty;

				#endregion

				musteri.KurumAdi = model.musteri.KurumAdi;
				musteri.KontakAdi = model.musteri.KontakAdi;
				musteri.KontakSoyadi = model.musteri.KontakSoyadi;

				musteri.KurumTipId = SFHelper.ReturnNullableInt(model.KurumTipId);
				musteri.KontakKategoriId = SFHelper.ReturnNullableInt(model.KontakKatId);
				//musteri.VergiDairesiIlId = RevmerHelper.ReturnNullableInt(model.VergiDaireIlId);
				//musteri.VergiDairesiIlceId = RevmerHelper.ReturnNullableInt(model.VergiDaireIlceId);
				musteri.IlId = SFHelper.ReturnNullableInt(model.IlId);
				musteri.IlceId = SFHelper.ReturnNullableInt(model.IlceId);
			}
			musteri.MeslekId = SFHelper.ReturnNullableInt(model.MeslekId);
			musteri.DogumTarihi = model.musteri.DogumTarihi;
			musteri.Cinsiyet = model.musteri.Cinsiyet;

			musteri.VergiDairesi = model.musteri.VergiDairesi;
			musteri.Adres = model.musteri.Adres;
			musteri.SWEArsivMukellefi = model.musteri.SWEArsivMukellefi;
			musteri.SWEFaturaMukellefi = model.musteri.SWEFaturaMukellefi;
			musteri.VergiNo = model.musteri.VergiNo;
			musteri.MuhasebeKodu = model.musteri.MuhasebeKodu;
			musteri.Vade = model.musteri.Vade;
			musteri.CariKod = model.musteri.CariKod;
			musteri.ParcaIskonto = model.musteri.ParcaIskonto;
			musteri.IscilikIskonto = model.musteri.IscilikIskonto;
			musteri.Note = model.musteri.Note;

			musteri.DtModified = System.DateTime.Now;
			musteri.Modifier = currentUser;

			db.SaveChanges();


			return RedirectToAction("MusteriDuzenle", "Musteri", new { MusteriId = musteri.MusteriId });
		}

		public void FillMusteriCreateCombos(int ilId = 0, int VergiİlId = 0)
		{
			ViewBag.Meslekler = new SelectList(db.Meslekler.ToList(), "MeslekId", "Aciklamasi");
			ViewBag.KurumTipleri = new SelectList(db.KurumTipleri.ToList(), "KurumTipId", "Aciklamasi");
			ViewBag.KontakKategorileri = new SelectList(db.KontakKategorileri.ToList(), "KontakKatId", "Aciklamasi");
			ViewBag.Iller = new SelectList(db.iller.ToList().OrderBy(x => x.sehir), "id", "sehir");

			if (ilId != 0)
			{
				ViewBag.Ilceler = new SelectList(db.ilceler.Where(x => x.sehir == ilId).OrderBy(x => x.ilce).ToList(), "id", "ilce");
			}
			else
			{
				ViewBag.Ilceler = new SelectList(db.ilceler.OrderBy(x => x.ilce).ToList(), "id", "ilce");

			}

			//if (VergiİlId != 0)
			//{
			//	ViewBag.VergiIlceler = new SelectList(db.ilceler.Where(x => x.sehir == VergiİlId).OrderBy(x => x.ilce).ToList(), "id", "ilce");
			//}
			//else
			//{
			//	ViewBag.VergiIlceler = new SelectList(db.ilceler.OrderBy(x => x.ilce).ToList(), "id", "ilce");
			//}

		}

		[SessionAuthorization]
		public ActionResult MusteriKvkIzinYonet(int MusteriId)
		{
			MusteriKvkIzinPageModel model = new MusteriKvkIzinPageModel();
			model.MusteriId = MusteriId;
			model.musteriMailleri = db.MusteriMail.Where(x => x.MusteriId == MusteriId).ToList();
			model.musteriTelefonlari = db.MusteriTelefon.Where(x => x.MusteriId == MusteriId).ToList();

			return View(model);
		}

		[SessionAuthorization]
		public JsonResult MailKvkOnayGonder(int MusteriId, int MusteriMailId)
		{
			MusteriMail mail = db.MusteriMail.Find(MusteriMailId);

			try
			{
				Random r = new Random();
				int randNum = r.Next(1000000);
				string sixDigitNumber = randNum.ToString("D6");

				mail.MailOnayKodu = sixDigitNumber;
				db.SaveChanges();

				string htmlBody = "Onaylama Kodunuz :" + sixDigitNumber;

				SFHelper.HtmlBodyMailGonder(mail.MailAdresi, "Kvk Revmer - Mail izni", htmlBody);

				return Json(new { result = 1, message = "Başarılı." });
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				return Json(new { result = 0, message = "Başarısız, Mail Gönderilirken hata oluştu" });

			}

		}

		[SessionAuthorization]
		public JsonResult MailKvkApprove(int MusteriMailId, string MailApproveCode)
		{
			MusteriMail mail = db.MusteriMail.Find(MusteriMailId);

			try
			{
				if (mail.MailOnayKodu == MailApproveCode)
				{
					mail.ModifiedDate = System.DateTime.Now;
					mail.Modifier = currentUser;

					mail.SwOnay = true;
					mail.DtOnay = System.DateTime.Now;

					KvkMusteriLogs yenilog = new KvkMusteriLogs()
					{
						MusteriId = mail.Musteri.MusteriId,
						LogDesc = string.Format("{0,-10} | {1,-10} | {2,30}", "MailKvkApprove", MailApproveCode, "Mail :SwOnay " + mail.SwOnay.ToString() + " değerine çekildi, DtOnay " + Convert.ToString(mail.DtOnay) + " değerine çekildi."),
						LogDate = System.DateTime.Now,
						IP = SFHelper.GetIp()
					};
					db.KvkMusteriLogs.Add(yenilog);

					db.SaveChanges();

					return Json(new { result = 1, message = "Başarılı, İzin Verildi" });
				}
				else
				{
					return Json(new { result = 0, message = "Kodu Yanlış!" });
				}
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				return Json(new { result = 0, message = "Hata Oluştu!" });
			}

		}

		[SessionAuthorization]
		public ActionResult AddMusteriMail(MusteriKvkIzinPageModel model)
		{
			try
			{
				MusteriMail yeniMail = new MusteriMail();
				yeniMail.SwOnay = false;
				yeniMail.DtOnay = null;
				yeniMail.MailAdresi = model.mail.MailAdresi;
				yeniMail.MusteriId = model.MusteriId;

				yeniMail.Creator = currentUser;
				yeniMail.DtCreated = System.DateTime.Now;

				db.MusteriMail.Add(yeniMail);
				db.SaveChanges();

				FlashMessage.Confirmation("Mail başarıyla oluşturuldu.");
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Danger("Mail oluşturulurken hata oluştu.");
			}


			return RedirectToAction("MusteriKvkIzinYonet", "Musteri", new { MusteriId = model.MusteriId });
		}

		[SessionAuthorization]
		public ActionResult AddMusteriTelefon(MusteriKvkIzinPageModel model)
		{
			try
			{
				MusteriTelefon yeniTelefon = new MusteriTelefon();

				yeniTelefon.TelefonNumarasi = model.telefon.TelefonNumarasi;
				yeniTelefon.SwOnaySms = false;
				yeniTelefon.DtOnaySms = null;

				yeniTelefon.SwOnayArama = false;
				yeniTelefon.DtOnayArama = null;

				yeniTelefon.Creator = currentUser;
				yeniTelefon.DtCreated = System.DateTime.Now;
				yeniTelefon.MusteriId = model.MusteriId;

				db.MusteriTelefon.Add(yeniTelefon);
				db.SaveChanges();

				FlashMessage.Confirmation("Telefon başarıyla oluşturuldu.");
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Danger("Telefon oluşturulurken hata oluştu.");
			}


			return RedirectToAction("MusteriKvkIzinYonet", "Musteri", new { MusteriId = model.MusteriId });
		}

		[SessionAuthorization]
		public JsonResult SmsKvkOnayGonder(int MusteriId, int MusteriTelefonId)
		{
			MusteriTelefon telefon = db.MusteriTelefon.Find(MusteriTelefonId);

			try
			{
				Random r = new Random();
				int randNum = r.Next(1000000);
				string sixDigitNumber = randNum.ToString("D6");

				telefon.OnaySmsKodu = sixDigitNumber;
				db.SaveChanges();

				string KvkHtmlBody = "Onay Kodu: " + telefon.OnaySmsKodu + " , iletişim izinleri link üzerinden düzenleyebilirsiniz. ";
				KvkHtmlBody += HttpContext.Request.Url.Scheme + "://" + ConfigurationManager.AppSettings["LocalDomainName"] + "/Musteri/IBY?token=" + MusteriId + "-" + Convert.ToString(sixDigitNumber);

				//JET SMS GONDER 
				SFHelper.SendSms(SFHelper.GetPhoneNumber(telefon.TelefonNumarasi), KvkHtmlBody);

				return Json(new { result = 1, message = "Başarılı." });
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				return Json(new { result = 0, message = "Başarısız, Sms Gönderilirken hata oluştu" });
			}
		}

		public JsonResult SmsKvkApprove(int MusteriTelefonId, string SmsApproveCode)
		{
			MusteriTelefon telefon = db.MusteriTelefon.Find(MusteriTelefonId);

			try
			{
				if (telefon.OnaySmsKodu == SmsApproveCode)
				{
					telefon.ModifiedDate = System.DateTime.Now;
					telefon.Modifier = currentUser;

					telefon.SwOnaySms = true;
					telefon.DtOnaySms = System.DateTime.Now;

					KvkMusteriLogs yenilog = new KvkMusteriLogs()
					{
						MusteriId = telefon.Musteri.MusteriId,
						LogDesc = string.Format("{0,-10} | {1,-10} | {2,30}", "SmsKvkApprove", SmsApproveCode, "TELEFON :SwOnaySms " + telefon.SwOnaySms.ToString() + " değerine çekildi, DtOnaySms " + Convert.ToString(telefon.DtOnaySms) + " değerine çekildi."),
						LogDate = System.DateTime.Now,
						IP = SFHelper.GetIp()
					};
					db.KvkMusteriLogs.Add(yenilog);

					db.SaveChanges();

					return Json(new { result = 1, message = "Başarılı, İzin Verildi" });
				}
				else
				{
					return Json(new { result = 0, message = "Kodu Yanlış!" });
				}
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				return Json(new { result = 0, message = "Hata Oluştu!" });
			}

		}

		[SessionAuthorization]
		public JsonResult AramaKvkOnayGonder(int MusteriId, int MusteriTelefonId)
		{
			MusteriTelefon telefon = db.MusteriTelefon.Find(MusteriTelefonId);

			try
			{
				Random r = new Random();
				int randNum = r.Next(1000000);
				string sixDigitNumber = randNum.ToString("D6");

				telefon.OnayAramaKodu = sixDigitNumber;
				db.SaveChanges();

				string KvkHtmlBody = "Onay Kodu: " + telefon.OnayAramaKodu + " , iletişim izinleri link üzerinden düzenleyebilirsiniz. ";
				KvkHtmlBody += HttpContext.Request.Url.Scheme + "://" + ConfigurationManager.AppSettings["LocalDomainName"] + "/Musteri/IBY?token=" + MusteriId + "-" + Convert.ToString(sixDigitNumber);

				//JET SMS GONDER 
				SFHelper.SendSms(SFHelper.GetPhoneNumber(telefon.TelefonNumarasi), KvkHtmlBody);

				return Json(new { result = 1, message = "Başarılı." });
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				return Json(new { result = 0, message = "Başarısız, Sms Gönderilirken hata oluştu" });
			}
		}

		[SessionAuthorization]
		public JsonResult AramaKvkApprove(int MusteriTelefonId, string AramaApproveCode)
		{
			MusteriTelefon telefon = db.MusteriTelefon.Find(MusteriTelefonId);

			try
			{
				if (telefon.OnayAramaKodu == AramaApproveCode)
				{
					telefon.ModifiedDate = System.DateTime.Now;
					telefon.Modifier = currentUser;

					telefon.SwOnayArama = true;
					telefon.DtOnayArama = System.DateTime.Now;

					KvkMusteriLogs yenilog = new KvkMusteriLogs()
					{
						MusteriId = telefon.Musteri.MusteriId,
						LogDesc = string.Format("{0,-10} | {1,-10} | {2,30}", "AramaKvkApprove", AramaApproveCode, "TELEFON :SwOnayArama " + telefon.SwOnayArama.ToString() + " değerine çekildi, DtOnayArama " + Convert.ToString(telefon.DtOnayArama) + " değerine çekildi."),
						LogDate = System.DateTime.Now,
						IP = SFHelper.GetIp()
					};
					db.KvkMusteriLogs.Add(yenilog);

					db.SaveChanges();

					return Json(new { result = 1, message = "Başarılı, İzin Verildi" });
				}
				else
				{
					return Json(new { result = 0, message = "Kodu Yanlış!" });
				}
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				return Json(new { result = 0, message = "Hata Oluştu!" });
			}

		}

		[SessionAuthorization]
		public JsonResult KvkSozlesmeSmsGonder(int MusteriId)
		{
			MusteriTelefon telefon = db.MusteriTelefon.Where(x => x.MusteriId == MusteriId && x.SwPrefered == true).FirstOrDefault();

			try
			{
				Random r = new Random();
				int randNum = r.Next(1000000);
				string sixDigitNumber = randNum.ToString("D6");

				telefon.GenelOnayKodu = sixDigitNumber;
				telefon.DtGenelOnayKodu = System.DateTime.Now;
				db.SaveChanges();

				string KvkHtmlBody = "Onay Kodu: " + telefon.GenelOnayKodu + " , iletişim izinleri link üzerinden düzenleyebilirsiniz. ";
				KvkHtmlBody += HttpContext.Request.Url.Scheme + "://" + ConfigurationManager.AppSettings["LocalDomainName"] + "/Musteri/IBY?token=" + MusteriId + "-" + Convert.ToString(sixDigitNumber);

				//JET SMS GONDER 
				SFHelper.SendSms(SFHelper.GetPhoneNumber(telefon.TelefonNumarasi), KvkHtmlBody);




				return Json(new { result = 1, message = "Başarılı." });
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				return Json(new { result = 0, message = "Başarısız, Sms Gönderilirken hata oluştu" });
			}
		}

		[SessionAuthorization]
		public JsonResult KvkGenelApprove(int MusteriId, string Token)
		{
			MusteriTelefon telefon = db.MusteriTelefon.Where(x => x.MusteriId == MusteriId && x.SwPrefered == true).FirstOrDefault();

			try
			{
				if (telefon.GenelOnayKodu == Token)
				{
					if (telefon.DtGenelOnayKodu < System.DateTime.Now.AddDays(2))
					{
						// Tüm mail adresleri çekilir. 

						List<MusteriMail> mailler = db.MusteriMail.Where(x => x.MusteriId == MusteriId).ToList();

						foreach (var mail in mailler)
						{
							mail.SwOnay = true;
							mail.DtOnay = System.DateTime.Now;
							mail.ModifiedDate = System.DateTime.Now;
							mail.Modifier = currentUser;
						}


						//Tüm telefonlar çekilir.
						List<MusteriTelefon> telefonlar = db.MusteriTelefon.Where(x => x.MusteriId == MusteriId).ToList();

						foreach (var phone in telefonlar)
						{
							phone.SwOnaySms = true;
							phone.DtOnaySms = System.DateTime.Now;

							phone.SwOnayArama = true;
							phone.DtOnayArama = System.DateTime.Now;

							phone.ModifiedDate = System.DateTime.Now;
							phone.Modifier = currentUser;
						}

						db.SaveChanges();

						return Json(new { result = 0, message = "Tüm iletişim izinleri verildi." });


					}
					else
					{
						return Json(new { result = 0, message = "Kod 48 saat geçerlidir." });
					}

				}
				else
				{
					return Json(new { result = 0, message = "Kodu Yanlış!" });
				}
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				return Json(new { result = 0, message = "Hata Oluştu!" });
			}

		}

		public ActionResult HizliMusteriSec()
		{
			return View(db.Musteri.ToList());
		}

		public ActionResult IBY(string token)
		{
			try
			{
				int musteriId = Convert.ToInt32(token.Split('-')[0]);
				string code = Convert.ToString(token.Split('-')[1]);

				MusteriIletisimBilgileriYonetPageModel model = new MusteriIletisimBilgileriYonetPageModel();
				//MusteriId, Code Alınır
				Musteri musteri = db.Musteri.Where(x => x.MusteriId == musteriId).FirstOrDefault();

				MusteriTelefon musteriTelefon = musteri.MusteriTelefon.Where(x => x.SwPrefered == true).FirstOrDefault();

				if (musteriTelefon.GenelOnayKodu == code)
				{
					if (musteriTelefon.DtGenelOnayKodu < System.DateTime.Now.AddDays(2)) // 2 gün içinde link'e tıklandı ise
					{
						model.musteri = musteri;
						model.telefonlar = musteri.MusteriTelefon.ToList();
						model.mailler = musteri.MusteriMail.ToList();
						return View(model);
					}
					else
					{
						FlashMessage.Danger("Müşteri link süresi dolmuş!");
					}
				}
				else
				{
					FlashMessage.Warning("Müşteri kodu yanlış!");
				}

			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Warning("Müşteri bulunamadı!");
			}

			return View();
		}

		[HttpPost]
		public ActionResult IBY(MusteriIletisimBilgileriYonetPageModel model)
		{
			string code = string.Empty;
			int MusteriId = 0;

			

			try
			{
				MusteriId = Convert.ToInt32(Request["token"].Split('-')[0]);
				code  = Convert.ToString(Request["token"].Split('-')[1]);
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Danger("Code ve id bulunamadı.");
			}

			try
			{
				Musteri musteri = db.Musteri.Where(x => x.MusteriId == MusteriId).FirstOrDefault();

				MusteriTelefon musteriTelefonDoubleCheck = musteri.MusteriTelefon.Where(x => x.GenelOnayKodu == code && x.SwPrefered == true).FirstOrDefault();

				if (musteriTelefonDoubleCheck != null)
				{
					// Veritabanından Müşteri Telefonları çekilir.

					List<MusteriTelefon> musteriTelefonlari = db.MusteriTelefon.Where(x => x.MusteriId == musteriTelefonDoubleCheck.MusteriId).ToList();

					foreach (var dbTelefon in musteriTelefonlari)
					{
						bool pageSwOnayArama = model.telefonlar.Where(x => x.MusteriTelefonId == dbTelefon.MusteriTelefonId).FirstOrDefault().SwOnayArama;
						bool dbSwOnayArama = dbTelefon.SwOnayArama;

						if (pageSwOnayArama != dbSwOnayArama) // değişiklik var
						{
							dbTelefon.SwOnayArama = pageSwOnayArama;

							if (dbTelefon.SwOnayArama == true)
							{
								dbTelefon.DtOnayArama = System.DateTime.Now;
							}
							else
							{
								dbTelefon.DtOnayArama = null;
							}

							dbTelefon.ModifiedDate = System.DateTime.Now;
							dbTelefon.Modifier = SFHelper.GetIp();

							KvkMusteriLogs yenilog = new KvkMusteriLogs()
							{
								MusteriId = musteriTelefonDoubleCheck.MusteriId,
								LogDesc = string.Format("{0,-10} | {1,-10} | {2,30}", "IBY", code, "TELEFON :SwOnayArama " + dbTelefon.SwOnayArama.ToString() + " değerine çekildi, DtOnayArama " + Convert.ToString(dbTelefon.DtOnayArama) + " değerine çekildi."),
								LogDate = System.DateTime.Now,
								IP = SFHelper.GetIp()
							};
							db.KvkMusteriLogs.Add(yenilog);

							db.SaveChanges();

						}

						//SMS İZNİNE GEÇİLDİ

						bool pageSwOnaySms = model.telefonlar.Where(x => x.MusteriTelefonId == dbTelefon.MusteriTelefonId).FirstOrDefault().SwOnaySms;
						bool dbSwOnaySms = dbTelefon.SwOnaySms;

						if (pageSwOnaySms != dbSwOnaySms) // değişiklik var
						{
							dbTelefon.SwOnaySms = pageSwOnaySms;

							if (dbTelefon.SwOnaySms == true)
							{
								dbTelefon.DtOnaySms = System.DateTime.Now;
							}
							else
							{
								dbTelefon.DtOnaySms = null;
							}

							dbTelefon.ModifiedDate = System.DateTime.Now;
							dbTelefon.Modifier = SFHelper.GetIp();

							KvkMusteriLogs yenilog = new KvkMusteriLogs()
							{
								MusteriId = musteriTelefonDoubleCheck.MusteriId,
								LogDesc = string.Format("{0,-10} | {1,-10} | {2,30}", "IBY", code, "TELEFON :SwOnaySms " + dbTelefon.SwOnaySms.ToString() + " değerine çekildi, DtOnaySms " + Convert.ToString(dbTelefon.DtOnaySms) + " değerine çekildi."),
								LogDate = System.DateTime.Now,
								IP = SFHelper.GetIp()
							};
							db.KvkMusteriLogs.Add(yenilog);


							db.SaveChanges();

						}


					}

					List<MusteriMail> musteriMails = db.MusteriMail.Where(x => x.MusteriId == musteriTelefonDoubleCheck.MusteriId).ToList(); ;

					foreach (var dbmail in musteriMails)
					{
						bool pageSwOnay = model.mailler.Where(x => x.MusteriMailId == dbmail.MusteriMailId).FirstOrDefault().SwOnay;
						bool dbSwOnay = dbmail.SwOnay;

						if (pageSwOnay != dbSwOnay) // değişiklik var
						{
							dbmail.SwOnay = pageSwOnay;

							if (dbmail.SwOnay == true)
							{
								dbmail.DtOnay = System.DateTime.Now;
							}
							else
							{
								dbmail.DtOnay = null;
							}

							dbmail.ModifiedDate = System.DateTime.Now;
							dbmail.Modifier = SFHelper.GetIp();

							KvkMusteriLogs yenilog = new KvkMusteriLogs()
							{
								MusteriId = musteriTelefonDoubleCheck.MusteriId,
								LogDesc = string.Format("{0,-10} | {1,-10} | {2,30}", "IBY", code, "MAIL : SwOnay " + dbmail.SwOnay.ToString() + " değerine çekildi, DtOnay " + Convert.ToString(dbmail.DtOnay) + " değerine çekildi."),
								LogDate = System.DateTime.Now,
								IP = SFHelper.GetIp()
							};
							db.KvkMusteriLogs.Add(yenilog);

							db.SaveChanges();

						}
					}
					FlashMessage.Confirmation("İşleminiz Başarıyla Gerçekleşti.");
				}
				else
				{
					FlashMessage.Danger("Hatalı onay kodu.");
				}

			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				FlashMessage.Danger("Code ve id bulunamadı.");
			}

			return View();
		}

		#region GetJsons

		public JsonResult GetIlceler(int IlId, int? selectedId)
		{
			List<SelectListItem> PList = new List<SelectListItem>();
			List<ilceler> list = db.ilceler.Where(x => x.sehir == IlId).ToList();

			PList.Add(new SelectListItem { Text = "İlçe Seçiniz", Value = "" });
			list.ForEach(x => PList.Add(new SelectListItem()
			{
				Text = x.ilce,
				Value = x.id.ToString()
			})

			);

			return Json(PList, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region LogoBuild

		public void BuildYeniMusteriForLogo(ref LG_001_CLCARD musteri)
		{
			LogoEntities logoDb = new LogoEntities();
			//musteri.DISCRATE = 0;
			//musteri.EXTENREF = 0;
			//musteri.PAYMENTREF = 0;

			//musteri.WARNMETHOD = 0;

			//musteri.BLOCKED = 0;

			//musteri.CCURRENCY = 0;
			//musteri.TEXTINC = 0;
			//musteri.SITEID = 0;
			//musteri.RECSTATUS = 1;
			//musteri.ORGLOGICREF = 0;

			//musteri.CAPIBLOCK_CREATEDBY = 1;
			//musteri.CAPIBLOCK_CREADEDDATE = System.DateTime.Now;
			//musteri.CAPIBLOCK_CREATEDHOUR = (short)System.DateTime.Now.Hour;
			//musteri.CAPIBLOCK_CREATEDMIN = (short)System.DateTime.Now.Minute;
			//musteri.CAPIBLOCK_CREATEDSEC = (short)System.DateTime.Now.Second;
			//musteri.CAPIBLOCK_MODIFIEDBY = 0;

			//musteri.PAYMENTPROC = 0;
			//musteri.CRATEDIFFPROC = 0;
			//musteri.WFSTATUS = 0;

			//musteri.PPGROUPREF = 0;

			//musteri.ORDSENDMETHOD = 0;

			//musteri.DSPSENDMETHOD = 0;

			//musteri.INVSENDMETHOD = 0;

			//musteri.SUBSCRIBERSTAT = 0;

			//musteri.PAYMENTTYPE = 0;
			//musteri.LASTSENDREMLEV = 0;
			//musteri.EXTACCESSFLAGS = 0;
			//musteri.ORDSENDFORMAT = 0;
			//musteri.DSPSENDFORMAT = 0;
			//musteri.INVSENDFORMAT = 0;
			//musteri.REMSENDFORMAT = 0;

			//musteri.CLORDFREQ = 0;
			//musteri.ORDDAY = 0;

			//musteri.LIDCONFIRMED = 0;


			//musteri.EXPBUSTYPREF = 0;
			//musteri.INVPRINTCNT = 1;
			//musteri.PIECEORDINFLICT = 0;
			//musteri.COLLECTINVOICING = 0;
			//musteri.EBUSDATASENDTYPE = 0;
			//musteri.INISTATUSFLAGS = 0;
			//musteri.SLSORDERSTATUS = 0;
			//musteri.SLSORDERPRICE = 0;
			//musteri.LTRSENDMETHOD = 0;


			//musteri.LTRSENDFORMAT = 0;
			//musteri.IMAGEINC = 0;

			//musteri.SAMEITEMCODEUSE = 0;


			//musteri.WFLOWCRDREF = 0;
			//musteri.PARENTCLREF = 0;
			//musteri.LOWLEVELCODES1 = logoDb.LG_001_CLCARD.Max(x => x.LOWLEVELCODES1) + 1;
			//musteri.LOWLEVELCODES2 = 0;
			//musteri.LOWLEVELCODES3 = 0;
			//musteri.LOWLEVELCODES4 = 0;
			//musteri.LOWLEVELCODES5 = 0;
			//musteri.LOWLEVELCODES6 = 0;
			//musteri.LOWLEVELCODES7 = 0;
			//musteri.LOWLEVELCODES8 = 0;
			//musteri.LOWLEVELCODES9 = 0;
			//musteri.LOWLEVELCODES10 = 0;



			//musteri.PURCHBRWS = 1;
			//musteri.SALESBRWS = 1;
			//musteri.IMPBRWS = 1;
			//musteri.EXPBRWS = 1;
			//musteri.FINBRWS = 1;

			//musteri.ADDTOREFLIST = 0;
			//musteri.TEXTREFTR = 0;
			//musteri.TEXTREFEN = 0;
			//musteri.ARPQUOTEINC = 0;
			//musteri.CLCRM = 0;
			//musteri.GRPFIRMNR = 0;
			//musteri.CONSCODEREF = 0;

			//musteri.OFFSENDMETHOD = 0;

			//musteri.OFFSENDFORMAT = 0;
			//musteri.EBANKNO = 0;
			//musteri.LOANGRPCTRL = 0;

			//musteri.LDXFIRMNR = 0;


			//musteri.EXTSENDMETHOD = 0;


			//musteri.EXTSENDFORMAT = 0;

			//musteri.CASHREF = 0;
			//musteri.USEDINPERIODS = 0;


			//musteri.RSKLIMCR = 0;
			//musteri.RSKDUEDATECR = 0;
			//musteri.RSKAGINGCR = 0;
			//musteri.RSKAGINGDAY = 0;
			//musteri.ACCEPTEINV = 0;

			//musteri.PROFILEID = 2;


			//musteri.PURCORDERSTATUS = 0;
			//musteri.PURCORDERPRICE = 0;
			//musteri.ISFOREIGN = 0;
			//musteri.SHIPBEGTIME1 = 134217728;
			//musteri.SHIPBEGTIME2 = 0;
			//musteri.SHIPBEGTIME3 = 0;
			//musteri.SHIPENDTIME1 = 288817152;
			//musteri.SHIPENDTIME2 = 0;
			//musteri.SHIPENDTIME3 = 0;
			//musteri.DBSLIMIT1 = 0;
			//musteri.DBSLIMIT2 = 0;
			//musteri.DBSLIMIT3 = 0;
			//musteri.DBSLIMIT4 = 0;
			//musteri.DBSLIMIT5 = 0;
			//musteri.DBSLIMIT6 = 0;
			//musteri.DBSLIMIT7 = 0;
			//musteri.DBSTOTAL1 = 0;
			//musteri.DBSTOTAL2 = 0;
			//musteri.DBSTOTAL3 = 0;
			//musteri.DBSTOTAL4 = 0;
			//musteri.DBSTOTAL5 = 0;
			//musteri.DBSTOTAL6 = 0;
			//musteri.DBSTOTAL7 = 0;
			//musteri.DBSBANKNO1 = 0;
			//musteri.DBSBANKNO2 = 0;
			//musteri.DBSBANKNO3 = 0;
			//musteri.DBSBANKNO4 = 0;
			//musteri.DBSBANKNO5 = 0;
			//musteri.DBSBANKNO6 = 0;
			//musteri.DBSBANKNO7 = 0;
			//musteri.DBSRISKCNTRL1 = 0;
			//musteri.DBSRISKCNTRL2 = 0;
			//musteri.DBSRISKCNTRL3 = 0;
			//musteri.DBSRISKCNTRL4 = 0;
			//musteri.DBSRISKCNTRL5 = 0;
			//musteri.DBSRISKCNTRL6 = 0;
			//musteri.DBSRISKCNTRL7 = 0;
			//musteri.DBSBANKCURRENCY1 = 0;
			//musteri.DBSBANKCURRENCY2 = 0;
			//musteri.DBSBANKCURRENCY3 = 0;
			//musteri.DBSBANKCURRENCY4 = 0;
			//musteri.DBSBANKCURRENCY5 = 0;
			//musteri.DBSBANKCURRENCY6 = 0;
			//musteri.DBSBANKCURRENCY7 = 0;

			//musteri.EINVOICETYPE = 0;

			////musteri.GUID]

			//musteri.DUEDATELIMIT = 0;
			//musteri.DUEDATETRACK = 0;
			//musteri.DUEDATECONTROL1 = 0;
			//musteri.DUEDATECONTROL2 = 0;
			//musteri.DUEDATECONTROL3 = 0;
			//musteri.DUEDATECONTROL4 = 0;
			//musteri.DUEDATECONTROL5 = 0;
			//musteri.DUEDATECONTROL6 = 0;
			//musteri.DUEDATECONTROL7 = 0;
			//musteri.DUEDATECONTROL8 = 0;
			//musteri.DUEDATECONTROL9 = 0;
			//musteri.DUEDATECONTROL10 = 0;
			//musteri.DUEDATECONTROL11 = 0;
			//musteri.DUEDATECONTROL12 = 0;
			//musteri.DUEDATECONTROL13 = 0;
			//musteri.DUEDATECONTROL14 = 0;
			//musteri.DUEDATECONTROL15 = 0;

			//musteri.CLOSEDATECOUNT = 0;
			//musteri.CLOSEDATETRACK = 0;
			//musteri.DEGACTIVE = 0;
			//musteri.DEGCURR = 0;


			//musteri.LABELINFO = 0;
			//musteri.DEFBNACCREF = 0;
			//musteri.PROJECTREF = 0;
			//musteri.DISCTYPE = 0;
			//musteri.SENDMOD = 0;
			//musteri.ISPERCURR = 0;
			//musteri.CURRATETYPE = 0;
			//musteri.INSTEADOFDESP = 0;
			//musteri.EINVOICETYP = 0;
			//musteri.FBSSENDMETHOD = 0;

			//musteri.FBSSENDFORMAT = 0;
			//musteri.FBASENDMETHOD = 0;
			//musteri.FBASENDFORMAT = 0;
			//musteri.SECTORMAINREF = 0;
			//musteri.SECTORSUBREF = 0;
			//musteri.PERSONELCOSTS = 0;
			//musteri.FACTORYDIVNR = 0;
			//musteri.FACTORYNR = 0;
			//musteri.ININVENNR = 0;
			//musteri.OUTINVENNR = 0;
			//musteri.QTYDEPDURATION = 0;
			//musteri.QTYINDEPDURATION = 0;
			//musteri.OVERLAPTYPE = 0;
			//musteri.OVERLAPAMNT = 0;
			//musteri.OVERLAPPERC = 0;
			//musteri.BROKERCOMP = 0;
			//musteri.CREATEWHFICHE = 1;
			//musteri.EINVCUSTOM = 0;
			//musteri.SUBCONT = 0;
			//musteri.ORDPRIORITY = 0;
			//musteri.ACCEPTEDESP = 0;
			//musteri.PROFILEIDDESP = 1;
			//musteri.LABELINFODESP = 0;
			//musteri.ACCEPTEINVPUBLIC = 0;
			//musteri.PUBLICBNACCREF = 0;
			//musteri.PAYMENTPROCBRANCH = 0;
			//musteri.KVKKPERMSTATUS = 0;
			//musteri.KVKKBEGDATE = System.DateTime.Now;
			////musteri.KVKKENDDATE]
			////musteri.KVKKCANCELDATE]
			//musteri.KVKKANONYSTATUS = 0;
			////musteri.KVKKANONYDATE 
			//musteri.EXIMSENDMETHOD = 0;
			//musteri.EXIMSENDFORMAT = 0;

		}

		public int LogoNewMusteri(CreateMusteriPageModel model)
		{
			if (ConfigurationManager.AppSettings["UseLogo"].ToString() == "1")
			{
				LogoEntities logoDb = new LogoEntities();
				LG_001_CLCARD yeniMusteri = new LG_001_CLCARD();
				//BuildYeniMusteriForLogo(ref yeniMusteri);
				#region DefaultValues
				yeniMusteri.DISCRATE = 0;
				yeniMusteri.EXTENREF = 0;
				yeniMusteri.PAYMENTREF = 0;

				yeniMusteri.WARNMETHOD = 0;

				yeniMusteri.BLOCKED = 0;

				yeniMusteri.CCURRENCY = 0;
				yeniMusteri.TEXTINC = 0;
				yeniMusteri.SITEID = 0;
				yeniMusteri.RECSTATUS = 1;
				yeniMusteri.ORGLOGICREF = 0;

				yeniMusteri.CAPIBLOCK_CREATEDBY = 1;
				yeniMusteri.CAPIBLOCK_CREADEDDATE = System.DateTime.Now;
				yeniMusteri.CAPIBLOCK_CREATEDHOUR = (short)System.DateTime.Now.Hour;
				yeniMusteri.CAPIBLOCK_CREATEDMIN = (short)System.DateTime.Now.Minute;
				yeniMusteri.CAPIBLOCK_CREATEDSEC = (short)System.DateTime.Now.Second;
				yeniMusteri.CAPIBLOCK_MODIFIEDBY = 0;

				yeniMusteri.PAYMENTPROC = 0;
				yeniMusteri.CRATEDIFFPROC = 0;
				yeniMusteri.WFSTATUS = 0;

				yeniMusteri.PPGROUPREF = 0;

				yeniMusteri.ORDSENDMETHOD = 0;

				yeniMusteri.DSPSENDMETHOD = 0;

				yeniMusteri.INVSENDMETHOD = 0;

				yeniMusteri.SUBSCRIBERSTAT = 0;

				yeniMusteri.PAYMENTTYPE = 0;
				yeniMusteri.LASTSENDREMLEV = 0;
				yeniMusteri.EXTACCESSFLAGS = 0;
				yeniMusteri.ORDSENDFORMAT = 0;
				yeniMusteri.DSPSENDFORMAT = 0;
				yeniMusteri.INVSENDFORMAT = 0;
				yeniMusteri.REMSENDFORMAT = 0;

				yeniMusteri.CLORDFREQ = 0;
				yeniMusteri.ORDDAY = 0;

				yeniMusteri.LIDCONFIRMED = 0;


				yeniMusteri.EXPBUSTYPREF = 0;
				yeniMusteri.INVPRINTCNT = 1;
				yeniMusteri.PIECEORDINFLICT = 0;
				yeniMusteri.COLLECTINVOICING = 0;
				yeniMusteri.EBUSDATASENDTYPE = 0;
				yeniMusteri.INISTATUSFLAGS = 0;
				yeniMusteri.SLSORDERSTATUS = 0;
				yeniMusteri.SLSORDERPRICE = 0;
				yeniMusteri.LTRSENDMETHOD = 0;


				yeniMusteri.LTRSENDFORMAT = 0;
				yeniMusteri.IMAGEINC = 0;

				yeniMusteri.SAMEITEMCODEUSE = 0;

				yeniMusteri.WFLOWCRDREF = 0;
				yeniMusteri.PARENTCLREF = 0;
				yeniMusteri.LOWLEVELCODES1 = logoDb.LG_001_CLCARD.Max(x => x.LOWLEVELCODES1) + 1;
				yeniMusteri.LOWLEVELCODES2 = 0;
				yeniMusteri.LOWLEVELCODES3 = 0;
				yeniMusteri.LOWLEVELCODES4 = 0;
				yeniMusteri.LOWLEVELCODES5 = 0;
				yeniMusteri.LOWLEVELCODES6 = 0;
				yeniMusteri.LOWLEVELCODES7 = 0;
				yeniMusteri.LOWLEVELCODES8 = 0;
				yeniMusteri.LOWLEVELCODES9 = 0;
				yeniMusteri.LOWLEVELCODES10 = 0;

				yeniMusteri.PURCHBRWS = 1;
				yeniMusteri.SALESBRWS = 1;
				yeniMusteri.IMPBRWS = 1;
				yeniMusteri.EXPBRWS = 1;
				yeniMusteri.FINBRWS = 1;

				yeniMusteri.ADDTOREFLIST = 0;
				yeniMusteri.TEXTREFTR = 0;
				yeniMusteri.TEXTREFEN = 0;
				yeniMusteri.ARPQUOTEINC = 0;
				yeniMusteri.CLCRM = 0;
				yeniMusteri.GRPFIRMNR = 0;
				yeniMusteri.CONSCODEREF = 0;

				yeniMusteri.OFFSENDMETHOD = 0;

				yeniMusteri.OFFSENDFORMAT = 0;
				yeniMusteri.EBANKNO = 0;
				yeniMusteri.LOANGRPCTRL = 0;

				yeniMusteri.LDXFIRMNR = 0;

				yeniMusteri.EXTSENDMETHOD = 0;

				yeniMusteri.EXTSENDFORMAT = 0;

				yeniMusteri.CASHREF = 0;
				yeniMusteri.USEDINPERIODS = 0;

				yeniMusteri.RSKLIMCR = 0;
				yeniMusteri.RSKDUEDATECR = 0;
				yeniMusteri.RSKAGINGCR = 0;
				yeniMusteri.RSKAGINGDAY = 0;
				yeniMusteri.ACCEPTEINV = 0;

				yeniMusteri.PROFILEID = 2;

				yeniMusteri.PURCORDERSTATUS = 0;
				yeniMusteri.PURCORDERPRICE = 0;
				yeniMusteri.ISFOREIGN = 0;
				yeniMusteri.SHIPBEGTIME1 = 134217728;
				yeniMusteri.SHIPBEGTIME2 = 0;
				yeniMusteri.SHIPBEGTIME3 = 0;
				yeniMusteri.SHIPENDTIME1 = 288817152;
				yeniMusteri.SHIPENDTIME2 = 0;
				yeniMusteri.SHIPENDTIME3 = 0;
				yeniMusteri.DBSLIMIT1 = 0;
				yeniMusteri.DBSLIMIT2 = 0;
				yeniMusteri.DBSLIMIT3 = 0;
				yeniMusteri.DBSLIMIT4 = 0;
				yeniMusteri.DBSLIMIT5 = 0;
				yeniMusteri.DBSLIMIT6 = 0;
				yeniMusteri.DBSLIMIT7 = 0;
				yeniMusteri.DBSTOTAL1 = 0;
				yeniMusteri.DBSTOTAL2 = 0;
				yeniMusteri.DBSTOTAL3 = 0;
				yeniMusteri.DBSTOTAL4 = 0;
				yeniMusteri.DBSTOTAL5 = 0;
				yeniMusteri.DBSTOTAL6 = 0;
				yeniMusteri.DBSTOTAL7 = 0;
				yeniMusteri.DBSBANKNO1 = 0;
				yeniMusteri.DBSBANKNO2 = 0;
				yeniMusteri.DBSBANKNO3 = 0;
				yeniMusteri.DBSBANKNO4 = 0;
				yeniMusteri.DBSBANKNO5 = 0;
				yeniMusteri.DBSBANKNO6 = 0;
				yeniMusteri.DBSBANKNO7 = 0;
				yeniMusteri.DBSRISKCNTRL1 = 0;
				yeniMusteri.DBSRISKCNTRL2 = 0;
				yeniMusteri.DBSRISKCNTRL3 = 0;
				yeniMusteri.DBSRISKCNTRL4 = 0;
				yeniMusteri.DBSRISKCNTRL5 = 0;
				yeniMusteri.DBSRISKCNTRL6 = 0;
				yeniMusteri.DBSRISKCNTRL7 = 0;
				yeniMusteri.DBSBANKCURRENCY1 = 0;
				yeniMusteri.DBSBANKCURRENCY2 = 0;
				yeniMusteri.DBSBANKCURRENCY3 = 0;
				yeniMusteri.DBSBANKCURRENCY4 = 0;
				yeniMusteri.DBSBANKCURRENCY5 = 0;
				yeniMusteri.DBSBANKCURRENCY6 = 0;
				yeniMusteri.DBSBANKCURRENCY7 = 0;

				yeniMusteri.EINVOICETYPE = 0;

				yeniMusteri.DUEDATELIMIT = 0;
				yeniMusteri.DUEDATETRACK = 0;
				yeniMusteri.DUEDATECONTROL1 = 0;
				yeniMusteri.DUEDATECONTROL2 = 0;
				yeniMusteri.DUEDATECONTROL3 = 0;
				yeniMusteri.DUEDATECONTROL4 = 0;
				yeniMusteri.DUEDATECONTROL5 = 0;
				yeniMusteri.DUEDATECONTROL6 = 0;
				yeniMusteri.DUEDATECONTROL7 = 0;
				yeniMusteri.DUEDATECONTROL8 = 0;
				yeniMusteri.DUEDATECONTROL9 = 0;
				yeniMusteri.DUEDATECONTROL10 = 0;
				yeniMusteri.DUEDATECONTROL11 = 0;
				yeniMusteri.DUEDATECONTROL12 = 0;
				yeniMusteri.DUEDATECONTROL13 = 0;
				yeniMusteri.DUEDATECONTROL14 = 0;
				yeniMusteri.DUEDATECONTROL15 = 0;

				yeniMusteri.CLOSEDATECOUNT = 0;
				yeniMusteri.CLOSEDATETRACK = 0;
				yeniMusteri.DEGACTIVE = 0;
				yeniMusteri.DEGCURR = 0;


				yeniMusteri.LABELINFO = 0;
				yeniMusteri.DEFBNACCREF = 0;
				yeniMusteri.PROJECTREF = 0;
				yeniMusteri.DISCTYPE = 0;
				yeniMusteri.SENDMOD = 0;
				yeniMusteri.ISPERCURR = 0;
				yeniMusteri.CURRATETYPE = 0;
				yeniMusteri.INSTEADOFDESP = 0;
				yeniMusteri.EINVOICETYP = 0;
				yeniMusteri.FBSSENDMETHOD = 0;

				yeniMusteri.FBSSENDFORMAT = 0;
				yeniMusteri.FBASENDMETHOD = 0;
				yeniMusteri.FBASENDFORMAT = 0;
				yeniMusteri.SECTORMAINREF = 0;
				yeniMusteri.SECTORSUBREF = 0;
				yeniMusteri.PERSONELCOSTS = 0;
				yeniMusteri.FACTORYDIVNR = 0;
				yeniMusteri.FACTORYNR = 0;
				yeniMusteri.ININVENNR = 0;
				yeniMusteri.OUTINVENNR = 0;
				yeniMusteri.QTYDEPDURATION = 0;
				yeniMusteri.QTYINDEPDURATION = 0;
				yeniMusteri.OVERLAPTYPE = 0;
				yeniMusteri.OVERLAPAMNT = 0;
				yeniMusteri.OVERLAPPERC = 0;
				yeniMusteri.BROKERCOMP = 0;
				yeniMusteri.CREATEWHFICHE = 1;
				yeniMusteri.EINVCUSTOM = 0;
				yeniMusteri.SUBCONT = 0;
				yeniMusteri.ORDPRIORITY = 0;
				yeniMusteri.ACCEPTEDESP = 0;
				yeniMusteri.PROFILEIDDESP = 1;
				yeniMusteri.LABELINFODESP = 0;
				yeniMusteri.ACCEPTEINVPUBLIC = 0;
				yeniMusteri.PUBLICBNACCREF = 0;
				yeniMusteri.PAYMENTPROCBRANCH = 0;
				yeniMusteri.KVKKPERMSTATUS = 0;
				yeniMusteri.KVKKBEGDATE = System.DateTime.Now;


				yeniMusteri.KVKKANONYSTATUS = 0;

				yeniMusteri.EXIMSENDMETHOD = 0;
				yeniMusteri.EXIMSENDFORMAT = 0;
				#endregion

				model.musteri.iller = db.iller.Find(model.IlId);
				model.musteri.ilceler = db.ilceler.Find(model.IlceId);

				model.musteri.iller1 = db.iller.Find(model.VergiDaireIlId);
				model.musteri.ilceler1 = db.ilceler.Find(model.VergiDaireIlceId);

				yeniMusteri.CODE = model.musteri.CariKod;
				yeniMusteri.ACTIVE = 0;
				yeniMusteri.CARDTYPE = 3;
				if (model.musteri.MusteriTipi == "B")
				{
					yeniMusteri.DEFINITION_ = model.musteri.MusteriAdi + " " + model.musteri.MusteriSoyadi;
				}
				else if (model.musteri.MusteriTipi == "K")
				{
					yeniMusteri.DEFINITION_ = model.musteri.KurumAdi;
				}

				//yeniMusteri.SPECODE = ""; // Bu değer eğer belirli bir gruba indiirm falan yapılacak ise kullanılır.

				yeniMusteri.ADDR1 = model.musteri.Adres.ToUpper();
				yeniMusteri.CITY = model.musteri.iller.sehir.ToUpper();
				yeniMusteri.COUNTRY = "TÜRKİYE";

				if (!string.IsNullOrEmpty(model.Telefon))
				{
					yeniMusteri.TELNRS1 = model.Telefon;
				}

				yeniMusteri.TAXNR = model.musteri.VergiNo;
				yeniMusteri.TAXOFFCODE = model.musteri.ilceler1.ilce.ToUpper();

				yeniMusteri.TAXOFFCODE = model.musteri.ilceler1.ilce.ToUpper();

				if (model.musteri.MusteriTipi == "K")
				{
					yeniMusteri.INCHARGE = model.musteri.KontakAdi + " " + model.musteri.KontakSoyadi;
				}

				if (!string.IsNullOrEmpty(model.Mail))
				{
					yeniMusteri.EMAILADDR = model.Mail;
				}

				// PAYMENTREF NEDİR ??? 

				if (model.musteri.IlceId != null)
				{
					yeniMusteri.TOWN = model.musteri.ilceler.ilce.ToUpper();
				}

				yeniMusteri.COUNTRYCODE = "TR";

				if (model.musteri.MusteriTipi == "B")
				{
					yeniMusteri.TCKNO = model.musteri.TCKN;
				}

				if (model.musteri.MusteriTipi == "B")
					yeniMusteri.ISPERSCOMP = 1;
				else
					yeniMusteri.ISPERSCOMP = 0;

				logoDb.LG_001_CLCARD.Add(yeniMusteri);
				logoDb.SaveChanges();

				return yeniMusteri.LOGICALREF;
			}
			else
				return 0;
		}

		#endregion


		public ActionResult MusteriTelefonGuncelle(string telefonNumarasi, int MusteriTelefonId, bool primaryTelNo)
		{
			try
			{
				MusteriTelefon model = db.MusteriTelefon.Find(MusteriTelefonId);

				if (model != null)
				{
					model.TelefonNumarasi = telefonNumarasi;
					model.ModifiedDate = System.DateTime.Now;
					if (primaryTelNo)
					{
						List<MusteriTelefon> telefonlar = db.MusteriTelefon.Where(x => x.MusteriId == model.MusteriId).ToList();
						foreach (var telefon in telefonlar)
						{
							telefon.SwPrefered = false;
						}
						db.SaveChanges();
					}
					model.SwPrefered = primaryTelNo;
					model.Modifier= currentUser;
					db.SaveChanges();
					return Json(new { result = 1, message = "Başarılı." });
				}
				else {
					return Json(new { result = 1, message = "Hata." });
				}
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				return Json(new { result = 1, message = "Hata." });
			}

		}

		public ActionResult Deneme()
		{
			return View();
		}


		[HttpPost]
		public ActionResult Deneme(string value)
		{
			FlashMessage.Danger("Message title", "Your danger alert");
			return View();
		}
	}
}