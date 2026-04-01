using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Contracts.Contracts.Jop
{
	public record JobResponseDto(
		int Id,
		string Title,
		string? Description,
		decimal? Salary,
		string? Location,		      
		string TypeJob,  
		string? ExperienceLevel,      
		int CompanyId,			      
		string CompanyName,   
		List<string> Skills,  
		int ApplicationsCount,    
		DateTime CreatedAt 
	);							      
}
