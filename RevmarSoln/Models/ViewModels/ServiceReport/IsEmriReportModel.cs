using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.ServiceReport
{
	public class IsEmriReportPageModel
	{
		public IsEmriReportPageModel()
		{
			kalemler = new List<IsEmriReportModel>();
			detail = new ReportDetails();
		}

		public DateTime bsgRaporTarihi { get; set; }

		public DateTime btsRaporTarihi { get; set; }

		public List<IsEmriReportModel> kalemler { get; set; }

		public ReportDetails detail { get; set; }
	}


	public class IsEmriReportModel
	{
		public IsEmriReportModel()
		{
			kalemler = new List<IsEmriReportDetayKalem>();
		}
		public string isEmriTipiAdi { get; set; }

		public List<IsEmriReportDetayKalem> kalemler { get; set; }

	}

	public class IsEmriReportDetayKalem
	{
		public string AracGrubu { get; set; }
		public int Below100 { get; set; }

		public int ToplamIsEmriSayisi { get; set; }

		public decimal? ToplamYedekParca { get; set; }
		public decimal? ToplamIscilik { get; set; }
		public decimal? Toplam { get; set; }
		public decimal? ParcaMaliyet { get; set; }
		public decimal? DYI { get; set; }
	}

	public class ReportDetails
	{
		public decimal FaturaToplamAracGirisi { get; set; }//Fatura Toplam Araç Girişi		
		public decimal BsgYParcaMaliyeti { get; set; }//Başlangıç Y.Parça Maliyeti
		public int FaturaDahiliIsEmriAdedi { get; set; }    //Fatura Dahili İş Emri Adedi		
		public decimal DokuzAyHareketsizParcaAdedi { get; set; }                    //9Ay Hareketsiz Parça Adedi
		public decimal ToplamIscilikSatisiDYIDahil { get; set; }            //Toplam İşçilik Satışı(DYI Dahil)      
		public decimal DokuzAyHareketsizParcaMaliyeti { get; set; }        //9Ay Hareketsiz Parça Maliyeti
		public int ToplamYedekParcaSatisi { get; set; }  //Toplam Yedek Parça Satışı            
		public decimal ToplamParcaAlisMaliyeti { get; set; }  //Toplam Parça Alış Maliyeti
		public decimal DYIMaliyeti { get; set; } //DYI Maliyeti             
		public decimal ToplamParcaSatisMaliyeti { get; set; }         //Toplam Parça Satış Maliyeti
		public decimal AcikRandevuSayisi { get; set; }     //Açık Randevu Sayısı 
		public decimal ToplamSarfMalSatisi { get; set; } //Toplam Aksesuar/Sarf Mal. Satışı
		public decimal AcikIsEmriSayisi { get; set; }//Açık İş Emri Sayısı 
		public decimal ToplamYedekParcaIadeSatisi { get; set; }    //Toplam Yedet Parça İadesi/satışı

	}
}