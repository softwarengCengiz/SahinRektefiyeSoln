using PagedList;
using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Infrastructure;
using SahinRektefiyeSoln.Models;
using SahinRektefiyeSoln.Models.ViewModels.IsEmri;
using SahinRektefiyeSoln.Models.ViewModels.Sayim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Controllers
{
	[LogActionFilter]
	public class SayimController : BaseController
	{
		
		SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();

		// GET: Sayim
		public ActionResult Index()
        {
            return View();
        }

		[SessionAuthorization]
		public ActionResult SayimBaslat()
		{
			SayimBaslatDropdownDoldur();
			return View();
		}

		[HttpPost]
		[SessionAuthorization]
		public ActionResult SayimBaslat(SayimBaslatViewModel model)
		{
			

			return View();
		}

		public void SayimBaslatDropdownDoldur()
		{
			ViewBag.SayimTipleri = new SelectList(db.SayimTipi.ToList(), "SayimTipId", "Aciklama");
		}

		public ActionResult SayimParcaSec(IsEmriParcaEkleViewModel model,int? MasterPage)
		{
			int pageIndex = model.Page ?? 1;

			List<StokKarti> parcalar = db.StokKarti.ToList();

			#region Filter

			if (model.ParcaNo != null)
			{
				parcalar = parcalar.Where(x => x.ParcaNo.ToLower().Contains(model.ParcaNo.ToLower())).ToList();
			}

			if (model.ParcaAdi != null)
			{
				parcalar = parcalar.Where(x => x.ParcaAdi.ToLower().Contains(model.ParcaAdi.ToLower())).ToList();
			}

			if (model.ParcaTipiId != null)
			{
				parcalar = parcalar.Where(x => x.ParcaTipId == model.ParcaTipiId).ToList();
			}

			#endregion

			model.parcalar = parcalar.ToPagedList(pageIndex, 10); //.OrderBy(x => x.FaturaStatu).ThenByDescending(x => x.DtFaturaBekleyen).

			if (Request.IsAjaxRequest() && (MasterPage == null || MasterPage != 1))
			{
				return PartialView("_GridSayimParcaEkle", model);
			}
			else
			{
				ViewBag.BelirtiKodlari = new SelectList(db.IsEmriTipleri.ToList(), "IsEmriTipId", "Aciklamasi");
				return View(model);
			}


		}

	}
}