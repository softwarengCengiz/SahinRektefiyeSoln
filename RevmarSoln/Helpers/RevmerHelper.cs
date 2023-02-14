
using SahinRektefiyeSoln.Helpers.LogHelper;
using SahinRektefiyeSoln.JetSms;
using SahinRektefiyeSoln.Models;
using SahinRektefiyeSoln.Models.HelperModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace SahinRektefiyeSoln.Helpers
{
	public static class SFHelper
	{
		const string siparisMailAdress = "siparis@revmer.com";
		const string siparisMailSifre = "Sp548*724@";
		const string siparisMailHost = "smtp.office365.com";
		const int siparisMailPort = 587;
		const bool siparisMailEnableSsl = true;
		static SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();
		public static bool CheckMyPermission(string permission, string UserName)
		{
			if (UserName != null)
			{
				bool isPermitted = false;
				string userName = UserName.ToString();
				List<UserRoles> userRoles = db.UserRoles.Where(x => x.UserName == userName).ToList();

				foreach (UserRoles role in userRoles)
				{
					var perm = db.RolePermissions.Where(x => x.RoleId == role.RoleId && x.Permissions.PermissionName == permission).FirstOrDefault();//
					if (perm != null)
						isPermitted = true;
				}
				return isPermitted;
			}
			else
			{
				return false;
			}

		}

		public static Boolean MailGonder(ExceptionLog log, string konu)
		{
			string MailBody = "";
			MailBody += "<table cellspacing='5' cellpadding='3' style='width:100%'>";
			MailBody += "<tr><td>Message : </td> <td>" + log.Message + "</td></tr>";
			MailBody += "<tr><td>Parameters : </td> <td>" + log.Parameters + "</td></tr>";
			MailBody += "<tr><td>InnerMessage : </td> <td>" + log.InnerMessage + "</td></tr>";
			MailBody += "<tr><td>Stack Trace : </td> <td>" + log.StackTrace + "</td></tr>";
			MailBody += "</table>";
			try
			{
				//SmtpClient sc = new SmtpClient();
				//sc.Port = 587;
				//sc.Host = "smtp.isimtescil.net";
				//sc.EnableSsl = false;

				SmtpClient sc = new SmtpClient();
				sc.Port = siparisMailPort;
				sc.Host = siparisMailHost;// "mail.revmer.com";
				sc.EnableSsl = siparisMailEnableSsl;
				//sc.Credentials = new NetworkCredential("siparis@revmer.com", "");
				sc.Credentials = new NetworkCredential(siparisMailAdress, siparisMailSifre);  //new NetworkCredential("info@revmer.com", "");

				MailMessage mail = new MailMessage();
				string kimden = "Revmer Hata";
				mail.From = new MailAddress("siparis@revmer.com");
				mail.To.Add("@hotmail.com");
				//mail.To.Add("siparis@revmer.com");
				mail.Subject = "Revmer Hata! ";
				mail.Body = MailBody;
				mail.IsBodyHtml = true;
				sc.Send(mail);

				return true;
			}
			catch (Exception ex)
			{
				throw ex;

			}

		}

		public static Boolean CheckMailGonder(int port, string host, string mailAdress, string sifre, string konu)
		{


			string MailBody = konu;
			try
			{
				//Exchange 587 , ssl = true , host : smtp.office365.com

				SmtpClient sc = new SmtpClient();
				sc.Port = port; // Exchange 587;
				sc.Host = host; //"smtp.office365.com";
				sc.EnableSsl = true;
				sc.Credentials = new NetworkCredential(mailAdress, sifre);
				MailMessage mail = new MailMessage();

				mail.From = new MailAddress(mailAdress);//"siparis@revmer.com"
				mail.To.Add("@hotmail.com");
				mail.To.Add("siparis@revmer.com");
				mail.Subject = "Revmer Check Mail! ";
				mail.Body = MailBody;
				mail.IsBodyHtml = true;
				sc.Send(mail);

				return true;
			}
			catch (Exception ex)
			{
				throw ex;

			}



		}

		public static Boolean HtmlBodyMailGonder(string to, string subject, string htmlBody)
		{
			MailAddress addressFrom = new MailAddress("siparis@revmer.com");
			MailAddress addressTo = new MailAddress(to);
			MailMessage message = new MailMessage(addressFrom, addressTo);

			message.IsBodyHtml = true;
			message.Subject = subject;
			string htmlString = @"<html>
                      <body>
                     " + htmlBody + @"
                      </html>
                     ";
			message.Body = htmlString;

			SmtpClient client = new SmtpClient();
			client.Host = siparisMailHost;
			client.Port = siparisMailPort;
			client.EnableSsl = false;
			client.Credentials = new NetworkCredential(siparisMailAdress, siparisMailSifre);//			client.Credentials = new NetworkCredential("info@revmer.com", "Rv*6767*09@");

			client.Send(message);

			return true;
		}

		public static Boolean HtmlBodyFordKonsinyeMailGonder(string to, string subject, string htmlBody)
		{
			MailAddress addressFrom = new MailAddress("siparis@revmer.com");
			MailAddress addressTo = new MailAddress(to);
			MailMessage message = new MailMessage(addressFrom, addressTo);

			message.To.Add("@hotmail.com");
			message.To.Add("siparis@revmer.com");

			message.IsBodyHtml = true;
			message.Subject = subject;


			string htmlString = @"<html>
                      <body>
                     " + htmlBody + @"
                      </html>
                     ";
			message.Body = htmlString;

			SmtpClient client = new SmtpClient();
			client.Host = siparisMailHost;
			client.Port = siparisMailPort;
			client.EnableSsl = siparisMailEnableSsl;
			client.Credentials = new NetworkCredential(siparisMailAdress, siparisMailSifre);

			client.Send(message);

			return true;
		}

	
	
	
		public static bool CheckMyRole(string UserName, string RoleName)
		{
			if (UserName != null) // Giriş yapılmış mı?
			{
				bool isInRole = false;
				Roles role;


				try
				{
					role = db.Roles.Where(x => x.RoleName == RoleName).FirstOrDefault(); // Kontrol edilmek istenen Role'ü bul Id'si ile kontrol etmek için
				}
				catch (Exception ex)
				{
					throw ex;
				}

				if (role == null)
				{
					throw new Exception("Rol Bulunamadı ! \n Kullanıcı Adı : " + UserName + "\n Parameter :  " + RoleName);
				}


				List<UserRoles> roleList = db.UserRoles.Where(x => x.UserName == UserName).ToList(); //Kullanıcının rollerini bul

				if (roleList == null)
				{
					throw new Exception("Kullanıcı Rolleri Bulunamadı ! \n Kullanıcı Adı : " + UserName + "\n Parameter :  " + RoleName);
				}

				foreach (var rowRole in roleList)
				{
					if (rowRole.RoleId == role.RoleId) // Eğer kullanıcını herhangi bir rolü bizim kontrol ediceğimiz roleId ye uyuşur ise true dön
						isInRole = true;

				}

				return isInRole;
			}
			else
			{
				return false;
			}
		}

		public static bool isRevmerUser(string UserName)
		{
			Users currentUser = db.Users.Where(x => x.UserName == UserName).FirstOrDefault();
			return true;
		}

		public static bool isFordMerkezUser(string UserName)
		{
			Users currentUser = db.Users.Where(x => x.UserName == UserName).FirstOrDefault();

			bool isFordMerkezUser = false;

			foreach (var userRole in currentUser.UserRoles)
			{
				if (userRole.Roles.RoleName == "Ford Merkez Yetkilisi")
					isFordMerkezUser = true;
			}

			return isFordMerkezUser;


		}

		public static string GetIp()
		{
			string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (string.IsNullOrEmpty(ip))
			{
				ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
			}
			return ip;
		}

		public static string Limit(this String str, int count)
		{
			if (str != null)
			{


				if (str.Length <= count - 3)
					return str;
				else
				{
					int lastspace = str.Substring(0, count - 3).LastIndexOf(' ');
					if (lastspace > 0 && lastspace > count - 20)
					{
						// limits the backward search to a max of 20 chars
						return str.Substring(0, lastspace) + "...";
					}
					else
					{
						// No space in the last 20 chars, so get all the string minus 3
						return str.Substring(0, count - 3) + "...";
					}
				}
			}
			else return "";
		}

		public static int GetNullableInt(int? value)
		{
			int deger = 0;
			if (value != null)
			{
				deger = (int)value;
			}
			return deger;
		}

		public static int? ReturnNullableInt(int? value)
		{

			return value == 0 ? null : value;

		}

		private readonly static string reservedCharacters = "!*'();:@&=+$,/?%#[]";

		public static bool SendSms(string phoneNumber, string text)
		{
			//var request = (HttpWebRequest)WebRequest.Create("https://service.jetsms.com.tr/SMS-Web/HttpSmsSend?");


			//string test = UrlEncode_(text);

			//var postData = "Password=8741-Krz&Username=rev&Msisdns=905425753790&TransmissionID=REVMER&Messages=" + test;
			//var data = Encoding.GetEncoding("iso-8859-9").GetBytes(postData);

			//request.Method = "POST";
			//request.ContentType = "application/x-www-form-urlencoded";
			//request.ContentLength = data.Length;
			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			//using (var stream = request.GetRequestStream())
			//{
			//	stream.Write(data, 0, data.Length);
			//}

			//var response = (HttpWebResponse)request.GetResponse();

			//var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();


			//***'2 . NESİLL ***///

			var request = (HttpWebRequest)WebRequest.Create("https://api.jetsms.com.tr/SMS-Web/HttpSmsSend?");
			string test = "çiğüışŞÇİ ";
			//test = System.Web.HttpUtility.UrlEncode(test);
			test = UrlEncode_(text);
			var postData = "Password=8741-Krz&Username=rev&Msisdns=" + phoneNumber + "&TransmissionID=REV MER&Messages=" + test;
			var data = Encoding.GetEncoding("iso-8859-9").GetBytes(postData);

			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = data.Length;

			using (var stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}

			var response = (HttpWebResponse)request.GetResponse();

			var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();


			return true;

		}

		public static string UrlEncode_(string value)
		{
			if (String.IsNullOrEmpty(value))
				return String.Empty;

			var sb = new StringBuilder();

			foreach (char @char in value)
			{
				if (reservedCharacters.IndexOf(@char) == -1)
					sb.Append(@char);
				else
					sb.AppendFormat("%{0:X2}", (int)@char);
			}
			return sb.ToString();
		}

		public static bool IsNullOrZero(int? value)
		{
			bool result = false;
			if (value == null || value == 0)
				result = true;

			return result;
		}

		public static bool IsEmpty(this DateTime dateTime)
		{
			return dateTime == default(DateTime);
		}

		public static bool SendSmsBioTekno()
		{
			//ResponseFromHost responseFromBioTekno;
			//try
			//{
			//	//------------------SmsToOne Biotekno----------------------------
			//	SmsToOne smsToOne = new SmsToOne();
			//	smsToOne.Username = "rev";
			//	smsToOne.Password = "8741-Krz";
			//	smsToOne.Originator = "originator";

			//	//smsToOne.ExclusionTimeStart = "220000";
			//	//smsToOne.ExclusionTimeStop = "100000";

			//	smsToOne.setMessage("905425753790", "test mesaji i.e. Bir kisiye bir mesaj.");
			//	responseFromBioTekno = smsToOne.sendMessage();


			//}
			////------------------Sms Biotekno Exception----------------------------
			//catch (SmsCreationException sce)
			//{
			//	//Sms oluÅŸtururken alÄ±nan hata
			//	Console.WriteLine(sce.StackTrace);
			//}
			//catch (SmsSendException sse)
			//{
			//	//Sms Gönderilirken sistemsel olarak alınabilecek hata
			//	Console.WriteLine(sse.StackTrace);
			//}
			//catch (Exception ex)
			//{
			//	Console.WriteLine(ex.StackTrace);
			//}

			//SmsMultiSender smsMultiSender = new SmsMultiSender();
			//smsMultiSender.Username = "rev";
			//smsMultiSender.Password = "8741-Krz";
			//smsMultiSender.Originator = "originator"; //Set edilmeze hesapta tanımlı olan ilk Originator bilgisi kullanılır
			//smsMultiSender.AddMessage("905425753790", "test mesaji i.e. n kisiye n mesaj");
			//smsMultiSender.AddMessage("905425753790", "test mesaji i.e. n kisiye n mesaj");
			//// Gün içerisinde gitmesini istemediğiniz durumlarda kullanılır
			////smsMultiSender.ExclusionTimeStart = "220000";
			////smsMultiSender.ExclusionTimeStop = "100000";

			//responseFromBioTekno = smsMultiSender.sendMessage();


			//------------------ResponseFromBioTekno------------------------------
			//@return The response of the posting request from the clients to Biotekno. 
			//It include the code and message of the response gotten from Biotekno.   
			/*

			*/
			//if (responseFromBioTekno.Code == "00") //Code 00 ise başarılı demektir
			//	Console.WriteLine(responseFromBioTekno.Message); // Mesaj olarak, raporlarda kulllanılmak üzere, groupid döner
			//else // Diğer durumlarda yukarıdaki problemlerden biri oluşmuş demektir.
			//	Console.WriteLine("Error : " + responseFromBioTekno.Message);// Mesaj olarak, hatanın içeriği döner

			return true;
		}

		public static bool SendSmsSoap(string PhoneNumber, string SmsBody)
		{
			try
			{
				SMSServiceSoapClient client = new SMSServiceSoapClient();

				ArrayOfString message = new ArrayOfString { SmsBody };
				ArrayOfString receipents = new ArrayOfString { PhoneNumber };

				SendSMSRequest sendSMSRequest = new SendSMSRequest()
				{
					Body = {
					user = "rev",
					password="8741-Krz",
					messages = message,
					originator = "Revmer",
					receipents = receipents
				}
				};

				SendSMSResult result = client.SendSMS(
						 "rev",
						 "8741-Krz",
						  "REVMER",
						 "",
						 System.DateTime.Now.ToString("ddMMyyyyHHmmss"),
						 System.DateTime.Now.ToString("ddMMyyyyHHmmss"),
						 message,
						 receipents);

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}

		}

		public static string GetPhoneNumber(string phoneNumber)
		{
			return "9" + phoneNumber.Replace('(', ' ').Replace(')', ' ').Replace(" ", "");
		}

		public static string Get2LetterYear(int fourLetterYear)
		{
			return Convert.ToString(fourLetterYear.ToString().Substring(2, 2));
		}

		public static List<int> ConvertToIntListFromString(string value)
		{
			List<int> list = new List<int>();

			foreach (var item in value.Split(';'))
			{
				list.Add(Convert.ToInt32(item));
			}

			return list;
		}


		public static string getKonsinyeJobName()
		{
			return AppDomain.CurrentDomain.BaseDirectory + "\\JobLogs\\OtomatikKonsinye_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
		}


		public static void WriteToFileOtomatikKonsinyeLogs(string Message)
		{
			string path = AppDomain.CurrentDomain.BaseDirectory + "\\JobLogs";

			Message = string.Format("{0,-30} {1,-20}", System.DateTime.Now, Message);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}


			string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\JobLogs\\OtomatikKonsinye_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
			if (!File.Exists(filepath))
			{
				// Create a file to write to.   
				using (StreamWriter sw = File.CreateText(filepath))
				{
					sw.WriteLine(Message);
				}
			}
			else
			{
				using (StreamWriter sw = File.AppendText(filepath))
				{
					sw.WriteLine(Message);
				}
			}

			Console.WriteLine(Message);
		}


		public enum FaturaTipi
		{
			NormalFatura = 10,
			AcikFatura = 20
		}

		public static string ToQueryString(this object request, string separator = ",")
		{
			if (request == null)
				throw new ArgumentNullException("request");

			// Get all properties on the object
			var properties = request.GetType().GetProperties()
				.Where(x => x.CanRead)
				.Where(x => x.GetValue(request, null) != null)
				.ToDictionary(x => x.Name, x => x.GetValue(request, null));

			// Get names for all IEnumerable properties (excl. string)
			var propertyNames = properties
				.Where(x => !(x.Value is string) && x.Value is IEnumerable)
				.Select(x => x.Key)
				.ToList();

			// Concat all IEnumerable properties into a comma separated string
			foreach (var key in propertyNames)
			{
				var valueType = properties[key].GetType();
				var valueElemType = valueType.IsGenericType
										? valueType.GetGenericArguments()[0]
										: valueType.GetElementType();
				if (valueElemType.IsPrimitive || valueElemType == typeof(string))
				{
					var enumerable = properties[key] as IEnumerable;
					properties[key] = string.Join(separator, enumerable.Cast<object>());
				}
			}

			// Concat all key/value pairs into a string separated by ampersand
			return string.Join("&", properties
				.Select(x => string.Concat(
					Uri.EscapeDataString(x.Key), "=",
					Uri.EscapeDataString(x.Value.ToString()))));
		}


		public static int? GetValueFromNullableInt(int? value)
		{
			if (value == null || value == 0)
			{
				return null;
			}
			else
			{
				return value;
			}
		}

		public static string TurkishCharacterToEnglish(string text)
		{
			char[] turkishChars = { 'ı', 'ğ', 'İ', 'Ğ', 'ç', 'Ç', 'ş', 'Ş', 'ö', 'Ö', 'ü', 'Ü' };
			char[] englishChars = { 'i', 'g', 'I', 'G', 'c', 'C', 's', 'S', 'o', 'O', 'u', 'U' };

			// Match chars
			for (int i = 0; i < turkishChars.Length; i++)
				text = text.Replace(turkishChars[i], englishChars[i]);

			return text;
		}

		public static string ToRelativeDate(DateTime input)
		{
			TimeSpan oSpan = DateTime.Now.Subtract(input);
			double TotalMinutes = oSpan.TotalMinutes;
			string Suffix = " önce";

			if (TotalMinutes < 0.0)
			{
				TotalMinutes = Math.Abs(TotalMinutes);
				Suffix = " Şuandan itibaren";
			}

			var aValue = new SortedList<double, Func<string>>();
			aValue.Add(0.75, () => "bir dakikadan az");
			aValue.Add(1.5, () => "yaklaşık 1 dk önce");
			aValue.Add(45, () => string.Format("{0} dakika", Math.Round(TotalMinutes)));
			aValue.Add(90, () => "yaklaşık bir saat");
			aValue.Add(1440, () => string.Format("yaklaşık {0} saat", Math.Round(Math.Abs(oSpan.TotalHours)))); // 60 * 24
			aValue.Add(2880, () => "gün"); // 60 * 48
			aValue.Add(43200, () => string.Format("{0} gün", Math.Floor(Math.Abs(oSpan.TotalDays)))); // 60 * 24 * 30
			aValue.Add(86400, () => "yaklaşık bir ay"); // 60 * 24 * 60
			aValue.Add(525600, () => string.Format("{0} ay", Math.Floor(Math.Abs(oSpan.TotalDays / 30)))); // 60 * 24 * 365 
			aValue.Add(1051200, () => "yaklaşık 1 yıl"); // 60 * 24 * 365 * 2
			aValue.Add(double.MaxValue, () => string.Format("{0} yıl", Math.Floor(Math.Abs(oSpan.TotalDays / 365))));

			return aValue.First(n => TotalMinutes < n.Key).Value.Invoke() + Suffix;
		}
	}
}