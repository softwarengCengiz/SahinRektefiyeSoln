using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.HelperModels
{
	public class ExceptionLog
	{
		public ExceptionLog()
		{
			StackTrace = "";
			Message = "";
			InnerMessage = "";
			Parameters = null;
		}
		public string StackTrace { get; set; }
		public string Message{ get; set; }
		public string InnerMessage{ get; set; }
		public Object Parameters { get; set; }
	}

	
}