using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.FilterModels
{
	public class CreateRolePermission
	{
		public int RoleId { get; set; }
		public int PermissionId { get; set; }
		public string PermissionName { get; set; }
		public string PermissionDesc { get; set; }
		public bool ischecked { get; set; }
	}
}