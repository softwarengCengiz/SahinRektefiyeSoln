using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class DepoRafTanimPageModel
	{
		public List<DepoRafTanim> depoRaflari { get; set; }
		public DepoRafTanim filter { get; set; }
	}
}