using JobPlatformBackend.Contracts.Contracts.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface IGeminiService
	{
		Task<AIAnalysisResult> AnalyzeResumeAsync(string resumeText);
	}
}
