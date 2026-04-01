using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Contracts.Contracts.Jop.Create
{
	public record CreateJobRequest(
		string Title,
		string Description,
		decimal? Salary,
		string Location,
		string TypeJop,
		string ExperieceLevel,
		int CompanyId,
		List<string> Skills
	);
	
}
