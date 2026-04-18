using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Contracts.Contracts.Application.Get
{
	public record GetAllApplicationRequest(int JobId, int CompanyId, int PageNumber, int PageSize);



}
