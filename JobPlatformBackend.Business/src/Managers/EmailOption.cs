using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Managers
{
	public class EmailOption
	{
		public string SmtpServer { get; set; }
		public int Port { get; set; }
		public string FromEmail { get; set; }
		public string Password { get; set; }
	}
}
