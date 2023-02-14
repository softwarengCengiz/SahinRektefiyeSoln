using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class AracCinsleriPageModel
	{
		public AracTipi filter { get; set; }
		public IPagedList<AracTipi> aracTipleri{ get; set; }
	}
}