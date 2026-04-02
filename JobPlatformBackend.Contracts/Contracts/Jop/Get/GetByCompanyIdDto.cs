using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Contracts.Contracts.Jop.Get
{
	public record GetByCompanyIdDto(int companyId, int page, int pageSize);
	
	
}
