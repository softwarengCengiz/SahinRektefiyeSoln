using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Admin
{
	public class CreateUserPageModel
	{
		public Users user { get; set; }
		public int RoleId { get; set; }
		public int ServiceId { get; set; }

	}
}