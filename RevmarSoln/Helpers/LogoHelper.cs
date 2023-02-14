using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Helpers
{
	public static class LogoHelper
	{
		public static int LogoAdetId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["UnitAdetId"]);
			}
		}

		public static int LogoSaatId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["UnitSaatId"]);
			}
		}

		public static int LogoKgId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["UnitKgId"]);
			}
		}
		public static int LogoUOMYagRefId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["UOMYagRefId"]);
			}
		}

		public static int LogoUOMAdetId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["UOMAdetId"]);
			}
		}

		public static int LogoUOMSaatId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["UOMSaatId"]);
			}
		}

		public static int GetUnitId(int stokKartTipi)
		{
			if (stokKartTipi == 17)
			{
				return LogoKgId;
			}
			else {
				return LogoAdetId;
			}
		}

		public static int GetUOMRefId(int stokKartTipi)
		{
			if (stokKartTipi == 17)
			{
				return LogoUOMYagRefId;
			}
			else {
				return LogoUOMAdetId;
			}
		}
	}
}