using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.FilterModels
{
    public class WorkOrderFilter
    {
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> VehicleId { get; set; }
        public Nullable<int> TaskId { get; set; }
        public string Plate { get; set; }
        public string VIN { get; set; }
        public DateTime dtWorkOrderStart { get; set; }
        public DateTime dtWorkOrderEnd { get; set; }
		public Nullable<int> OrderStatu { get; set; }
		public string NoWorkOrder { get; set; }
	    public bool SwApproved { get; set; }
		public Nullable<int> ModelYear { get; set; }
		public string FuelType { get; set; }
		//public Nullable<int> WorkOrderId { get; set; }
		public string SerialNo { get; set; }
		public string PartNo { get; set; }
		public bool SwShowCanceled { get; set; }
	}
}