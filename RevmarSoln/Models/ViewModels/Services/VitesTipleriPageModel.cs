using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class VitesTipleriPageModel
	{
		public VitesTipi filter { get; set; }
		public IPagedList<VitesTipi> vitesTipleri { get; set; }
	}
}