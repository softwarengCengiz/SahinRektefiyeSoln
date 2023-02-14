using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Controllers
{
	[LogActionFilter]
	public class CompanyController : BaseController
	{
        // GET: Company
        public ActionResult Index()
        {
            return View();
        }
        
    }
}