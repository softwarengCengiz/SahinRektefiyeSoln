using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.HelperModels
{
	public class IsEmriHelper
	{
	}

	public enum IsEmriTeklifStatu
	{
		ServisBekliyor = 5,
		TeklifIletilmesiBekleniyor = 10,
		TeklifIletildi = 20,
		TeklifOnaylandi = 30,
		Islemde = 35 , 
		IslemTamamlandi = 40
	}

	public enum FordSiparisStatu
	{
		SiparisOnayBekliyor = 10,
		BayiSiparisiIptalEtti= 20,
		RevmerSiparisiRedEtti= 30,
		KonsiyeAracBilgileriBekleniyor= 40,
		BayiSiparisIadeOnayBekleniyor = 45,
		BayiSiparisIadeOnaylandi = 47,
		SiparisTamamlandi = 50
	}

	public enum FordStokLog
	{
		StokArtti = 10 ,
		StokAzaldi = 20,
		StokManuelGuncellendi = 30
	}

	
}