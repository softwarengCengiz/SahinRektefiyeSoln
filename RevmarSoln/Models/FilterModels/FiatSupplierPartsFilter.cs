using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.FilterModels
{
	public class FiatSupplierPartsFilter
	{
		public Nullable<int> PartReviewTypeId { get; set; }
		public string PartNumber { get; set; }
		public Nullable<int> CompanyId { get; set; }
		public Nullable<int> VehicleId { get; set; }
		public string RevisionTrackNumber { get; set; }
	}
}