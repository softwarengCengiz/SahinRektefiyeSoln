using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models
{
	public static class Parameters
	{
		public static int IsEmriAcikStatu
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["IsEmriAcikStatu"].ToString());
			}
		}
		public static int IsEmriIptalStatu
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["IsEmriIptalStatu"].ToString());
			}
		}
		public static int IsEmriTamamlandiStatu
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["IsEmriTamamlandiStatu"].ToString());
			}
		}
		public static int IadeOnayBekleniyor
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["IadeOnayBekleniyor"].ToString());
			}
		}
		public static int IptalOnayBekleniyor
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["IptalOnayBekleniyor"].ToString());
			}
		}
		public static int Iade
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["Iade"].ToString());
			}
		}

		public static int IsEmriFaturaBekleniyor
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["IsEmriFaturaBekleniyor"].ToString());
			}
		}

		public static int IsEmriBedelsizOnayBekliyor
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["IsEmriBedelsizOnayBekliyor"].ToString());
			}
		}


		public static int TeklifAcikStatu
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["TeklifAcikStatu"].ToString());
			}
		}
		public static int TeklifIptalStatu
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["TeklifIptalStatu"].ToString());
			}
		}
		public static int TeklifeDonusturuldu
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["TeklifeDonusturuldu"].ToString());
			}
		}



		public static int ServiceKlimaIsEmriTipId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ServiceKlimaIsEmriTipId"].ToString());
			}
		}

		public static int ServiceEgrIsEmriTipId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ServiceEgrIsEmriTipId"].ToString());
			}
		}

		public static int ServiceSanzimanIsEmriTipId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ServiceSanzimanIsEmriTipId"].ToString());
			}
		}

		public static int ServiceTurboIsEmriTipId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ServiceTurboIsEmriTipId"].ToString());
			}
		}

		public static int ServiceAlternatorIsEmriTipId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ServiceAlternatorIsEmriTipId"].ToString());
			}
		}

		public static int ServiceMarsMotorIsEmriTipId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ServiceMarsMotorIsEmriTipId"].ToString());
			}
		}

		public static int ServiceDireksiyonPompasiIsEmriTipId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ServiceDireksiyonPompasiIsEmriTipId"].ToString());
			}
		}

		public static int ServiceDireksiyonKutusuMekanikIsEmriTipId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ServiceDireksiyonKutusuMekanikIsEmriTipId"].ToString());
			}
		}

		public static int ServiceDireksiyonKutusuHidrolikIsEmriTipId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ServiceDireksiyonKutusuHidrolikIsEmriTipId"].ToString());
			}
		}

		public static int ServiceDpfIsEmriTipId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ServiceDpfIsEmriTipId"].ToString());
			}
		}

		public static int ServiceBankoSatisIsEmriTipId
		{
			get
			{
				return Convert.ToInt32(ConfigurationManager.AppSettings["ServiceBankoSatisIsEmriTipId"].ToString());
			}
		}


		public static List<int> ServiceIsEmriIds()
		{
			List<int> isEmriIds = new List<int>();

			isEmriIds.Add(ServiceKlimaIsEmriTipId);
			isEmriIds.Add(ServiceEgrIsEmriTipId);
			isEmriIds.Add(ServiceSanzimanIsEmriTipId);
			isEmriIds.Add(ServiceTurboIsEmriTipId);
			isEmriIds.Add(ServiceAlternatorIsEmriTipId);
			isEmriIds.Add(ServiceMarsMotorIsEmriTipId);
			isEmriIds.Add(ServiceDireksiyonPompasiIsEmriTipId);
			isEmriIds.Add(ServiceDireksiyonKutusuMekanikIsEmriTipId);
			isEmriIds.Add(ServiceDireksiyonKutusuHidrolikIsEmriTipId);
			isEmriIds.Add(ServiceDpfIsEmriTipId);
			isEmriIds.Add(ServiceBankoSatisIsEmriTipId);

			return isEmriIds;
		}

		public static bool ChechIsEmriChanged(int value)
		{
			List<int> isEmriIds = ServiceIsEmriIds();

			if (isEmriIds.Contains(value))
			{
				return false;
			}
			else {
				return true;
			}
		}

	}
}