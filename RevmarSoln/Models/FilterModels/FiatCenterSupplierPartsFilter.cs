using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.FilterModels
{
	public class FiatCenterSupplierPartsFilter
	{
		public Nullable<int> PartReviewTypeId { get; set; }
		public string PartNumber { get; set; }
		public Nullable<int> CompanyId { get; set; }
		public Nullable<int> VehicleId { get; set; }
		public Nullable<int> ServiceId { get; set; }
		public string RevisionTrackNumber { get; set; }
		public bool SwShowInBayi { get; set; }
	}
}