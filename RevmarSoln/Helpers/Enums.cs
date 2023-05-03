using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Helpers.Enums
{
	public enum TicketStatus
	{
		TicketOpened, //Talep Açıldı
		DriverAppointed, //Şoför Atandı
		EngineAcceptance, //Motor Kabulde
		Received, //Teslim Alındı-Yolda
		Rejected //Talep İptal Edildi
	}

	public static class TicketStatusExtensions
	{
		private static readonly Dictionary<TicketStatus, string> TicketStatusText = new Dictionary<TicketStatus, string>
	{
		{ TicketStatus.TicketOpened, "Talep Açıldı" },
		{ TicketStatus.DriverAppointed, "Şoför Atandı" },
		{ TicketStatus.EngineAcceptance, "Motor Kabulde" },
		{ TicketStatus.Received, "Teslim Alındı-Yolda" },
		{ TicketStatus.Rejected, "İptal Edildi" }
	};

		public static string GetTicketStatusText(this TicketStatus ticketStatus)
		{
			return TicketStatusText[ticketStatus];
		}
	}
}