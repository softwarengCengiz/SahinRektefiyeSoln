using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.PostModel
{
	public class UpdateWorkOrderModel
	{
		public string Plate { get; set; }
		public int? ModelYear { get; set; }
		public int? VehicleKm { get; set; }
		public int? FuelType { get; set; }
		public string ServiceContactName { get; set; }
		public string ServiceContactPhoneNo { get; set; }
		public string NoWorkOrder { get; set; }
		public int WorkOrderId { get; set; }
		public string ComplaintsCustomerRequests { get; set; }
		public string VIN { get; set; }
		public bool Warranty { get; set; }
		public bool Revision { get; set; }
		public bool RepeatWork { get; set; }
		public bool ReportIsSend { get; set; }
		public string SerialNo { get; set; }
		public string PartNo { get; set; }
		public bool ReportApprovedByCustomer { get; set; }
	}
}