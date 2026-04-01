using JobPlatformBackend.Business.src.Mappers;
using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Jop;
using JobPlatformBackend.Contracts.Contracts.Jop.Create;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Domain.src.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class JobService : IJobService
	{
		private readonly IJobRepository _jobRepository;
		private readonly ILogger<JobService> _logger;
		private readonly ISanitizerService _sanitizerService;
		private readonly ICompanyRepository _companyRepository;
		private readonly ISkillRepository _skillRepository;
		private readonly IUserRepository _userRepository;
		public JobService(ISkillRepository skillRepository,IUserRepository userRepository, ICompanyRepository companyRepository, IJobRepository jobRepository, ILogger<JobService> logger, ISanitizerService sanitizerService)
		{

			_jobRepository = jobRepository;
			_logger = logger;
			_sanitizerService = sanitizerService;
			_companyRepository = companyRepository;
			_skillRepository = skillRepository;
			_userRepository = userRepository;
			
		}
		public async Task<JobResponseDto> CreatJobAsync(CreateJobRequest request,int currentUserId)
		{
			var sanitizedDto = _sanitizerService.SanitizeDto(request);
			var Company = await _companyRepository.GetByIdAsync(request.CompanyId);

			if (Company == null)
			{
				throw new BadRequestException("Company not found");
			}
			var user = await _userRepository.GetByIdAsync(currentUserId);
			if (user == null)
			{
				throw new BadRequestException("User not found");
			}
			//var isAuthorized = await _companyRepository.IsUserAdminOfCompanyAsync(request.CompanyId, currentUserId);
			//if (!isAuthorized)
			//{
			//	throw new UnauthorizedAccessException("You are not Authorize by this company");
			//}


			if (string.IsNullOrWhiteSpace(sanitizedDto.Title))
			{
				throw new ArgumentException("Title is required");
			}

			if (string.IsNullOrWhiteSpace(sanitizedDto.Description))
			{
				throw new ArgumentException("Description is required");
			}
			if (string.IsNullOrWhiteSpace(sanitizedDto.ExperieceLevel))
			{
				throw new ArgumentException("ExperieceLevel is required");
			}

			if (sanitizedDto.Skills == null || !sanitizedDto.Skills.Any())
			{
				throw new ArgumentException("At least one skill is required");
			}
			if (!Enum.TryParse<TypeJob>(sanitizedDto.TypeJop, true, out var typeJob))
			{
				throw new ArgumentException("Invalid TypeJob");
			}
			var normalizedSkills = sanitizedDto.Skills
	  .Where(s => !string.IsNullOrWhiteSpace(s))
	  .Select(s => s.Trim().ToLower())
	  .Distinct()
	  .ToList();
			var existingSkills = await _skillRepository.GetByNamesAsync(request.Skills);
			var existingSkillNames = existingSkills.Select(s => s.Name.ToLower());

			var newSkillsNames = normalizedSkills.Where(s => !existingSkillNames.Contains(s)).ToList();
			var newSkills = newSkillsNames.Select(s => new Skill { Name = s }).ToList();
			if (newSkills.Any())
			{
				try
				{
					await _skillRepository.AddRangeAsync(newSkills);
					await _skillRepository.SaveChangesAsync();
				}
				catch
				{
					newSkills = await _skillRepository.GetByNamesAsync(newSkillsNames);
				}
			}
			var allSkills = existingSkills.Concat(newSkills).ToList();
			var jobEntity = new Job
			{
				Title = sanitizedDto.Title,
				Description = sanitizedDto.Description,
				Salary = sanitizedDto.Salary,
				Location = sanitizedDto.Location,
				TypeJop = typeJob,
				ExperieceLevel = sanitizedDto.ExperieceLevel,
				CompanyId = sanitizedDto.CompanyId,
				CreatedAt = DateTime.UtcNow,
				JobSkills = allSkills.Select(s => new JobSkill { SkillId = s.Id }).ToList()

			};
			await _jobRepository.AddAsync(jobEntity);
			await _jobRepository.SaveChangesAsync();
			return jobEntity.ToResponse();
		}
	}
}
