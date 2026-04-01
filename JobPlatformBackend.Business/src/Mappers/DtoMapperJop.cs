using JobPlatformBackend.Contracts.Contracts.Jop;
using JobPlatformBackend.Domain.src.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Mappers
{
	public static class DtoMapperJop
	{

    public static JobResponseDto ToResponse(this Job job)
    {
        return new JobResponseDto(
            job.Id,
            job.Title,
            job.Description,
            job.Salary,
            job.Location,
            job.TypeJop.ToString(), // enum -> string
            job.ExperieceLevel.ToString(),
            job.CompanyId,
            job.Company?.Name ?? "", // تأكد عامل Include
            job.JobSkills?
                .Select(js => js.Skill.Name)
                .ToList() ?? new List<string>(),
            job.Applications?.Count ?? 0,
            job.CreatedAt
        );
       }

	}
}
