using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Contracts.Contracts.Application.Get
{
	public record MyApplicationResponse(
		int ApplicationId,
		string JobTitle,
		string CompanyName,
		string ?CompanyLogoUrl,
		string Status,
	    DateTime AppliedAt,
	    string ?JobLocation,
	    int JobId,
		string Description,
		string ?LogoUrl,
		string? location,
		DateTime createdAt,
		decimal ?salary,
		string ExperinceLevel
		);
		
}
