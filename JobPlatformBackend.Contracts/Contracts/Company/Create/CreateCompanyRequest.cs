using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Contracts.Contracts.Company.Create
{
	public record CreateCompanyRequest(string Name, String Email, String Description,string ?Location,string ?LogoUrl);
	
	
}
