using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Mail
{
	public class MailGrupPageModel
	{
		public List<MailGruplari> mailGruplari { get; set; }
		public MailGruplari filter { get; set; }
	}
}