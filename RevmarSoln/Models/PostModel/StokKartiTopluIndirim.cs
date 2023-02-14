using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.PostModel
{
	public class StokKartiTopluIndirim
	{
		public decimal disCountRate { get; set; }
		public int ParcaTipId { get; set; }
		public string	userName{ get; set; }
		public string __RequestVerificationToken { get; set; }
	}
}