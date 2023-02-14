using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.HelperModels
{
	public class Calculate
	{
		public IsEmriKalemHesaplanmisTutarlar kalemTutarlariGetir(IsEmriKalemleri kalem)
		{
			IsEmriKalemHesaplanmisTutarlar model = new IsEmriKalemHesaplanmisTutarlar();

			if (kalem.KalepTipi == "P")
			{
				model.NormalFiyat = (double)kalem.BirimSaatAdet * (double)kalem.StokKarti.SatisBirimFiyati;
				model.NormalKdvliFiyat = model.NormalFiyat + (model.NormalFiyat * (double)(Decimal.Divide(kalem.StokKarti.Kdv, 100)));
				model.NormalKdv = (double)kalem.BirimSaatAdet * (double)kalem.StokKarti.SatisBirimFiyati * (double)(Decimal.Divide(kalem.StokKarti.Kdv, 100));

				
			}
			else if(kalem.KalepTipi =="I")
			{
				model.NormalFiyat = (double)kalem.BirimSaatAdet * (double)kalem.Iscilikler.IscilikTipleri.Fiyat;
				model.NormalKdvliFiyat = model.NormalFiyat + (model.NormalFiyat * (double)(Decimal.Divide(18, 100)));
				model.NormalKdv = (double)kalem.BirimSaatAdet * (double)kalem.Iscilikler.IscilikTipleri.Fiyat * (double)(Decimal.Divide(18, 100));
				
			}
			
			// Iskonto Var ise 
			if (kalem.IskontoOrani != null || kalem.IskontoOrani != 0)
			{
				decimal iskontoOrani = kalem.IskontoOrani == null ? 0 : (decimal)kalem.IskontoOrani;
				model.IskontoluFiyat = model.NormalFiyat - (model.NormalFiyat * (double)(Decimal.Divide(iskontoOrani, 100)));
				model.IskontoluKdvliFiyat = model.NormalKdvliFiyat - (model.NormalKdvliFiyat * (double)(Decimal.Divide(iskontoOrani, 100)));
				model.IskontoluKdv = model.NormalKdv - (model.NormalKdv * (double)(Decimal.Divide(iskontoOrani, 100)));
			}
			return model;
		}

		

	}

	

	public class IsEmriKalemHesaplanmisTutarlar
	{
		public double NormalFiyat { get; set; }
		public double NormalKdvliFiyat { get; set; }
		public double NormalKdv { get; set; }

		public double IskontoluFiyat { get; set; }
		public double IskontoluKdvliFiyat { get; set; }
		public double IskontoluKdv { get; set; }
	}
}