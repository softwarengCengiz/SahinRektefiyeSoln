using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SahinRektefiyeSoln.Models.ViewModels.Jobs;
using System.IO;

namespace SahinRektefiyeSoln.Controllers
{
	[LogActionFilter]
	public class JobsController : Controller
    {
		SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();

		[SessionAuthorization]
		public ActionResult Index()
        {
			JobsListPageModel pageModel = new JobsListPageModel();

			pageModel.isler = db.Jobs.ToList();
			return View(pageModel);
        }

		public ActionResult JobDetails(int JobDetailId)
		{
			JobDetails detay = db.JobDetails.Find(JobDetailId);
			string filepath = db.JobDetails.Find(JobDetailId).SpoolFilePath;
			byte[] filedata = System.IO.File.ReadAllBytes(filepath);
			string contentType = MimeMapping.GetMimeMapping(filepath);

			var cd = new System.Net.Mime.ContentDisposition
			{
				FileName = detay.Jobs.JobName +"_"+ detay.ExecutionDate.ToShortDateString(),
				Inline = true,
			};

			Response.AppendHeader("Content-Disposition", cd.ToString());
			Response.Charset = "utf-8";
			return File(filedata, contentType);

		}

	}
}