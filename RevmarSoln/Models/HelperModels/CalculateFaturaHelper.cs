using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.HelperModels
{
	public class CalculateFaturaHelper
	{

		public List<FaturaKalem> Hesapla(List<IsEmriKalemleri> kalemler,int? BelirtiKodu,string Tip)
		{
			List<FaturaKalem> faturaKalems = new List<FaturaKalem>();

			//if (BelirtiKodu != null)
			//{
			//	kalemler = kalemler.Where(x => x.IsEmriTipId == BelirtiKodu).ToList();
			//}
			if (Tip == "BelirtiKodu")
			{
				kalemler = kalemler.Where(x => x.IsEmriTipId == BelirtiKodu).ToList();
			}

			foreach (var kalem in kalemler)
			{
				FaturaKalem model = new FaturaKalem();
				model.IsEmriKalemId = kalem.IsEmriKalemId;
				model.KalepTipi = kalem.KalepTipi;
				model.IsEmriTipAciklamasi = kalem.IsEmriTipAciklamasi;
				model.IsEmriTipId = kalem.IsEmriTipId;
				model.IsKonsinye = kalem.IsKonsinye;
				if (kalem.KalepTipi == "P")
				{
					


					model.ParcaNo = kalem.StokKarti.ParcaNo;
					model.ParcaAdi = kalem.StokKarti.ParcaAdi;
					model.TeslimEdilenMiktar = kalem.TeslimEdilenMiktar;

					model.Miktar = (decimal)kalem.BirimSaatAdet;

					if (kalem.ParcaEklemeTipi == "M")
					{
						model.kdvHaricBirimFiyati = Convert.ToDecimal(kalem.ManuelSatisFiyat);
					}
					else
					{
						model.kdvHaricBirimFiyati = kalem.StokKarti.SatisBirimFiyati;
					}
					//model.kdvHaricBirimFiyati = kalem.StokKarti.SatisBirimFiyati;


					model.kalemIskontoOrani = (decimal)kalem.IskontoOrani / 100;
					model.kalemIskontoTutari = model.kdvHaricBirimFiyati * model.kalemIskontoOrani;

					model.kalemToplamIskontoTutari = model.kalemIskontoTutari * model.Miktar;

					model.KdvHaricIskontoluBFiyat = model.kdvHaricBirimFiyati - model.kalemIskontoTutari;
					model.KdvOrani = (decimal)kalem.StokKarti.Kdv / 100;
					model.KdvBirimTutari = model.KdvHaricIskontoluBFiyat * model.KdvOrani;
					model.KdvDahilBirimFiyat = model.KdvHaricIskontoluBFiyat * (1 + model.KdvOrani);

					if (!(kalem.IsKonsinye && kalem.KonsinyeIcinParcaGeldiMi))
					{
						// Bu kalem için konsinye gönderilmiş ve geriye revizyon edilecek parça gelmiş. 
						// Gönderilen parça için üret hesaplama
						model.ToplamKdvliTutari = model.Miktar * model.KdvBirimTutari;
						model.KdvHaricTutar = model.Miktar * model.KdvHaricIskontoluBFiyat;
					}

					

				}
				else if (kalem.KalepTipi == "I")
				{
					

					model.Miktar = (decimal)kalem.BirimSaatAdet;

					

					if (kalem.ParcaEklemeTipi == "M")
					{
						
						model.kdvHaricBirimFiyati = (decimal)kalem.ManuelSatisFiyat;
						model.IscilikKodu = kalem.ManuelIscilikKodu;
						model.IscilikAdi = kalem.IscilikAciklama;

					}
					else
					{
						model.IscilikAdi = kalem.Iscilikler.IscilikAdi;

						model.IscilikKodu = kalem.Iscilikler.IscilikKodu;
						if (kalem.Iscilikler.IscilikTipleri.Aciklamasi.ToUpper().Contains("DYI"))
							model.kdvHaricBirimFiyati = (decimal)kalem.DyiIlkSatisFiyati;
						else
							model.kdvHaricBirimFiyati = (decimal)kalem.Iscilikler.IscilikTipleri.Fiyat;

						if (kalem.Iscilikler.IscilikTipleri.Aciklamasi.ToUpper().Contains("DYI"))
						{
							model.isDYI = true;
						}
						else
						{
							model.isDYI = false;
						}
					}

					//if (kalem.Iscilikler.IscilikTipleri.Aciklamasi.ToUpper().Contains("DYI"))
					//	model.kdvHaricBirimFiyati = (decimal)kalem.DyiIlkSatisFiyati;
					//else
					//	model.kdvHaricBirimFiyati = (decimal)kalem.Iscilikler.IscilikTipleri.Fiyat;


					model.kalemIskontoOrani = (decimal)kalem.IskontoOrani / 100;
					model.kalemIskontoTutari = model.kdvHaricBirimFiyati * model.kalemIskontoOrani;

					model.kalemToplamIskontoTutari = model.kalemIskontoTutari * model.Miktar;

					model.KdvHaricIskontoluBFiyat = model.kdvHaricBirimFiyati - model.kalemIskontoTutari;
					model.KdvOrani = (decimal)18 / 100;
					model.KdvBirimTutari = model.KdvHaricIskontoluBFiyat * model.KdvOrani;
					model.KdvDahilBirimFiyat = model.KdvHaricIskontoluBFiyat * (1 + model.KdvOrani);
					model.ToplamKdvliTutari = model.Miktar * model.KdvBirimTutari;
					model.KdvHaricTutar = model.Miktar * model.KdvHaricIskontoluBFiyat;

					

				}
				faturaKalems.Add(model);
			}

			return faturaKalems;

		}

		
		public List<FaturaKalemTeklif> Hesapla(List<TeklifKalemleri> kalemler)
		{
			List<FaturaKalemTeklif> faturaKalems = new List<FaturaKalemTeklif>();

			foreach (var kalem in kalemler)
			{
				FaturaKalemTeklif model = new FaturaKalemTeklif();
				model.TeklifKalemId = kalem.TeklifKalemId;
				model.KalemTipi = kalem.KalemTipi;

				if (kalem.KalemTipi == "P")
				{
					model.ParcaNo = kalem.StokKarti.ParcaNo;
					model.ParcaAdi = kalem.StokKarti.ParcaAdi;

					model.Miktar = (decimal)kalem.BirimSaatAdet;
					model.kdvHaricBirimFiyati = kalem.StokKarti.SatisBirimFiyati;
					model.kalemIskontoOrani = (decimal)kalem.IskontoOrani / 100;
					model.kalemIskontoTutari = model.kdvHaricBirimFiyati * model.kalemIskontoOrani;

					model.kalemToplamIskontoTutari = model.kalemIskontoTutari * model.Miktar;

					model.KdvHaricIskontoluBFiyat = model.kdvHaricBirimFiyati - model.kalemIskontoTutari;
					model.KdvOrani = (decimal)kalem.StokKarti.Kdv / 100;
					model.KdvBirimTutari = model.KdvHaricIskontoluBFiyat * model.KdvOrani;
					model.KdvDahilBirimFiyat = model.KdvHaricIskontoluBFiyat * (1 + model.KdvOrani);
					model.ToplamKdvliTutari = model.Miktar * model.KdvBirimTutari;
					model.KdvHaricTutar = model.Miktar * model.KdvHaricIskontoluBFiyat;

				}
				else if (kalem.KalemTipi == "I")
				{
					model.IscilikKodu = kalem.Iscilikler.IscilikKodu;
					model.IscilikAdi = kalem.Iscilikler.IscilikAdi;
					model.Miktar = (decimal)kalem.BirimSaatAdet;
					//if (kalem.Iscilikler.IscilikTipleri.Aciklamasi.ToUpper().Contains("DYI"))
					//	model.kdvHaricBirimFiyati = (decimal)kalem.DyiIlkSatisFiyati;
					//else
					model.kdvHaricBirimFiyati = (decimal)kalem.Iscilikler.IscilikTipleri.Fiyat;


					model.kalemIskontoOrani = (decimal)kalem.IskontoOrani / 100;
					model.kalemIskontoTutari = model.kdvHaricBirimFiyati * model.kalemIskontoOrani;

					model.kalemToplamIskontoTutari = model.kalemIskontoTutari * model.Miktar;

					model.KdvHaricIskontoluBFiyat = model.kdvHaricBirimFiyati - model.kalemIskontoTutari;
					model.KdvOrani = (decimal)18 / 100;
					model.KdvBirimTutari = model.KdvHaricIskontoluBFiyat * model.KdvOrani;
					model.KdvDahilBirimFiyat = model.KdvHaricIskontoluBFiyat * (1 + model.KdvOrani);
					model.ToplamKdvliTutari = model.Miktar * model.KdvBirimTutari;
					model.KdvHaricTutar = model.Miktar * model.KdvHaricIskontoluBFiyat;

					if (kalem.Iscilikler.IscilikTipleri.Aciklamasi.ToUpper().Contains("DYI"))
					{
						model.isDYI = true;
					}
					else
					{
						model.isDYI = false;
					}

				}
				faturaKalems.Add(model);
			}

			return faturaKalems;

		}


	}

	public class FaturaKalemTeklif
	{
		public Guid TeklifKalemId { get; set; }
		public string KalemTipi { get; set; }
		public decimal Miktar { get; set; }
		public decimal kdvHaricBirimFiyati { get; set; }
		public decimal kalemIskontoOrani { get; set; }
		public decimal kalemIskontoTutari { get; set; }
		public decimal KdvHaricIskontoluBFiyat { get; set; }
		public decimal KdvOrani { get; set; }
		public decimal KdvBirimTutari { get; set; }
		public decimal KdvDahilBirimFiyat { get; set; }
		public decimal ToplamKdvliTutari { get; set; }
		public decimal KdvHaricTutar { get; set; }
		public bool isDYI { get; set; }
		public string IscilikKodu { get; set; }
		public string IscilikAdi { get; set; }
		public string ParcaNo { get; set; }
		public string ParcaAdi { get; set; }
		public decimal kalemToplamIskontoTutari { get; set; }

	}

	public class FaturaKalem
	{
		public int IsEmriKalemId { get; set; }
		public string KalepTipi { get; set; }
		public decimal Miktar { get; set; }
		public decimal kdvHaricBirimFiyati { get; set; }
		public decimal kalemIskontoOrani { get; set; }
		public decimal kalemIskontoTutari { get; set; }
		public decimal KdvHaricIskontoluBFiyat { get; set; }
		public decimal KdvOrani { get; set; }
		public decimal KdvBirimTutari { get; set; }
		public decimal KdvDahilBirimFiyat { get; set; }
		public decimal ToplamKdvliTutari { get; set; }
		public decimal KdvHaricTutar { get; set; }
		public bool isDYI { get; set; }
		public string IscilikKodu { get; set; }
		public string IscilikAdi { get; set; }
		public string ParcaNo { get; set; }
		public string ParcaAdi { get; set; }
		public decimal TeslimEdilenMiktar { get; set; }
		public decimal kalemToplamIskontoTutari { get; set; }
		public int? IsEmriTipId { get; set; }
		public string IsEmriTipAciklamasi { get; set; }
		public bool IsKonsinye { get; set; }

	}

	public class FaturaKalemGunluk
	{
		public int IsEmriKalemId { get; set; }
		public string KalepTipi { get; set; }
		public decimal Miktar { get; set; }
		public decimal kdvHaricBirimFiyati { get; set; }
		public decimal kalemIskontoOrani { get; set; }
		public decimal kalemIskontoTutari { get; set; }
		public decimal KdvHaricIskontoluBFiyat { get; set; }
		public decimal KdvOrani { get; set; }
		public decimal KdvBirimTutari { get; set; }
		public decimal KdvDahilBirimFiyat { get; set; }
		public decimal ToplamKdvliTutari { get; set; }
		public decimal KdvHaricTutar { get; set; }
		//public bool isDYI { get; set; }
		public string IscilikKodu { get; set; }
		public string IscilikAdi { get; set; }
		public string ParcaNo { get; set; }
		public string ParcaAdi { get; set; }
		public decimal TeslimEdilenMiktar { get; set; }
		public decimal kalemToplamIskontoTutari { get; set; }

	}

}