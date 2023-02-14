using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class AracGruplariPageModel
	{
		public AracGrubu filter { get; set; }
		public IPagedList<AracGrubu> aracGruplari { get; set; }
	}
}