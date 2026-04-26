using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Contracts.Contracts.AI
{
	public class AIAnalysisResult
	{
		public string headline { get; set; }
		public string experienceYears { get; set; }
		public string marketValue { get; set; }
		public string recommendation { get; set; }
		public string about { get; set; }          // أضفنا هاد
		public List<string> skills { get; set; }      // وأضفنا هاد
		public string targetTitle { get; set; }

		public bool isResume {  get; set; }
		// وأضفنا هاد
		public int progress { get; set; } = 0;

		public List<SkillGap> missingSkillsWithImpact { get; set; }
	}
	public class SkillGap
	{
 		public string skillName { get; set; }

 		public int impactPercentage { get; set; }
	}
}
