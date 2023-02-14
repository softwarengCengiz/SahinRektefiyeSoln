using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels
{
	public class GetApprovalWorkOrderPageModelcs
	{
		public List<WorkOrders> approvalWorkOrders { get; set; }
		public List<IsEmirleri> approvalWorkOrdersServis { get; set; }

	}
}