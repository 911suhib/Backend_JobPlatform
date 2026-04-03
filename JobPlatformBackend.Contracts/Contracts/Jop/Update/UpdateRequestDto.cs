using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Contracts.Contracts.Jop.Update
{
	public class UpdateRequestDto
	{
		public string? Title { get; set; }
		public string? Description { get; set; }
		public int? Salary { get; set; }
		public string? Location { get; set; }
		public string? typeJob { get; set; }
		public string? ExperienceLevel { get; set; }
		public List<string>? Skills { get; set; }
	}
}
