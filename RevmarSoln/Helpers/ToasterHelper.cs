using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Helpers
{
	public class ToasterHelper
	{
		public OperationStatus resultType { get; set; }
		public string headerText { get; set; }
		public string bodyText { get; set; }
	}

	public enum OperationStatus
	{
		Error = 0,
		Success = 1,
		Info = 2,
		Warning = 3
	}
}