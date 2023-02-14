using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Helpers.LogHelper;
using SahinRektefiyeSoln.Infrastructure;
using SahinRektefiyeSoln.Models;
using SahinRektefiyeSoln.Models.ViewModels.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Controllers
{
	[LogActionFilter]
	public class MailController : BaseController
	{
		SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();
		//[LogActionFilter]
		[SessionAuthorization] // Yetkiye Bağla sonradan 
		public ActionResult TopluMailGonder()
		{

			ViewBag.MailGruplari = new SelectList(db.MailGruplari.ToList(), "MailGrupId", "MailGrupAdi");
			return View();
		}

		[SessionAuthorization]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult TopluMailGonder(TopluMailGonderPageModel model, HttpPostedFileBase files)
		{
			List<string> data = new List<string>();

			if (model != null)
			{
				//1. Mail kimlere atılacak bul.

				List<MailAdresleri> mailGonderileceklerListesi = db.MailGruplari.Find(model.MailGrupId).MailAdresleri.ToList();
				string url = string.Empty;
				if (files != null) // Dosya Var ise 
				{
					if (files.ContentType == "image/jpeg" || files.ContentType == "image/png")
					{
						//2. Resim geliyor Bu resmi kayıt et.
						string GUID = Guid.NewGuid().ToString();

						string filename = GUID.ToString() + "_" + files.FileName; // Dosya Adını Bulduk.
						string targetpath = Server.MapPath("~/MailResimleri/");
						files.SaveAs(targetpath + filename);

						string pathToExcelFile = targetpath + filename;

						url = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~/MailResimleri/" + filename);
						model.mail.Icerik = model.mail.Icerik + "</br></br><img src='" + url + "' alt='kampanyaResmi'/>";

					}
					else
					{

						data.Add("Jpeg yada png yükleyiniz.");
					}

				}
				else // dosya yok ise
				{

				}

				//3. Tek Tek bu maili kişilere atmaya başla.

				foreach (var gidecekKullanici in mailGonderileceklerListesi)
				{
					Mail yeniMail = new Mail();
					
					yeniMail.Kime = gidecekKullanici.Mail;
					yeniMail.Konu = model.mail.Konu;
					yeniMail.Icerik = model.mail.Icerik;
					yeniMail.Ek = url;
					yeniMail.Creator = Session["UserName"].ToString();
					yeniMail.CreatedDate = System.DateTime.Now;
					db.Mail.Add(yeniMail);

					try
					{
						Helpers.SFHelper.HtmlBodyMailGonder(gidecekKullanici.Mail, model.mail.Konu, model.mail.Icerik);
					}
					catch (Exception ex)
					{
						new WebErrorLogsHelper().Log2Db(ex);
						data.Add(ex.Message.ToString());
					}

				}
				db.SaveChanges();

				

			}
			else
			{
				data.Add("Sayfadan model boş geldi.");
			}



			//return RedirectToAction("TopluGonderilenMailler", new { TopluMailMessage = data });
			return RedirectToAction("TopluGonderilenMailler");
			//return Json(data, JsonRequestBehavior.AllowGet);
		}

		[SessionAuthorization]
		public ActionResult MailGruplari()
		{
			MailGrupPageModel pagemodel = new MailGrupPageModel();
			pagemodel.mailGruplari = db.MailGruplari.ToList();
			return View(pagemodel);
		}
		[SessionAuthorization]
		public ActionResult MailGrupTanimla()
		{
			return View();
		}

		[SessionAuthorization]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult MailGrupTanimla(MailGruplari model, HttpPostedFileBase files)
		{
			List<string> data = new List<string>();
			if (files != null)
			{
				if (files.ContentType == "application/vnd.ms-excel" || files.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
				{
					string GUID = Guid.NewGuid().ToString();

					string filename = GUID.ToString() + "_" + files.FileName; // Dosya Adını Bulduk.
					string targetpath = Server.MapPath("~/MailGruplariExceller/");
					files.SaveAs(targetpath + filename);

					string pathToExcelFile = targetpath + filename;

					var connectionString = "";
					if (filename.EndsWith(".xls"))
					{
						connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
					}
					else if (filename.EndsWith(".xlsx"))
					{
						connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
					}


					var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
					var ds = new DataSet();

					adapter.Fill(ds, "ExcelTable");

					DataTable dtable = ds.Tables["ExcelTable"];
					try
					{
						using (SahinRektefiyeDbEntities context = new SahinRektefiyeDbEntities())
						{
							using (var transaction = context.Database.BeginTransaction())
							//using blokları arasında transaction'ımızı açtık ve artık transaction'ımız bir commit() fonksiyonunu kullanana kadar işlem yaptığımız tabloyu kilitleyecek.
							{
								MailGruplari yeniMailGrubu = new MailGruplari();

								yeniMailGrubu.MailGrupAdi = model.MailGrupAdi;
								yeniMailGrubu.Creator = Session["UserName"].ToString();
								yeniMailGrubu.CreatedDate = System.DateTime.Now;
								yeniMailGrubu.ExcelPath = filename;

								context.MailGruplari.Add(yeniMailGrubu);
								context.SaveChanges();

								foreach (DataRow item in dtable.Rows)
								{
									if (!string.IsNullOrWhiteSpace(item[0].ToString())) // mail Dolu ise
									{
										MailAdresleri yeniMailAdresi = new MailAdresleri();
										yeniMailAdresi.Mail = item[0].ToString();
										yeniMailAdresi.MailGrupId = yeniMailGrubu.MailGrupId;
										context.MailAdresleri.Add(yeniMailAdresi);
									}
								}

								context.SaveChanges();

								transaction.Commit();
							}

						}

					}
					catch (Exception ex)
					{
						new WebErrorLogsHelper().Log2Db(ex);
						string a = ex.ToString();
					}


				}
				else
				{
					data.Add("<ul>");
					data.Add("<li>Sadece excel formatı uygundur.</li>");
					data.Add("</ul>");
					data.ToArray();
					return Json(data, JsonRequestBehavior.AllowGet);
				}
			}
			else
			{
				data.Add("<ul>");
				if (files == null) data.Add("<li>Lütfen excel dosyası seçiniz.</li>");
				data.Add("</ul>");
				data.ToArray();
				return Json(data, JsonRequestBehavior.AllowGet);
			}
			return RedirectToAction("MailGruplari");
		}
		[SessionAuthorization]
		public ActionResult MailGrupAdresleriGor(int MailGrupId)
		{
			return View(db.MailGruplari.Find(MailGrupId).MailAdresleri.ToList());
		}

		[SessionAuthorization]
		public ActionResult TopluGonderilenMailler(List<string> TopluMailMessage)
		{
			ViewBag.TopluMailMessage = TopluMailMessage;
			return View(db.Mail.ToList());
		}

		[SessionAuthorization]
		public ActionResult MailDetail(int MailId)
		{
			return View(db.Mail.Find(MailId));
		}

	}
}