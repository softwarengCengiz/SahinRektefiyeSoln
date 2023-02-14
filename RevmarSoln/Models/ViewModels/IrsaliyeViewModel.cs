using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels
{
	public  class IrsaliyeViewModel
	{
		public int FordIrsaliyeId { get; set; }
		public string IrsaliyeNo { get; set; }
		public Nullable<int> IdNo { get; set; }
		public string CdFordTypeType { get; set; }
		public Nullable<System.DateTime> IrsaliyeTarihi { get; set; }
		public Nullable<System.DateTime> RevizyonTarihi { get; set; }
		public Nullable<System.DateTime> SevkTarihi { get; set; }
		public string Creator { get; set; }
		public Nullable<int> CdIrsaliyeDurum { get; set; }
		public Nullable<int> CountOfPart { get; set; }
		public string ParcaTipiAciklama{ get; set; }
		public string ParcaDurumu { get; set; }
	}
}