using JobPlatformBackend.Business.src.Mappers;
using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Jop;
using JobPlatformBackend.Contracts.Contracts.Jop.Create;
using JobPlatformBackend.Contracts.Contracts.Jop.Get;
using JobPlatformBackend.Contracts.Contracts.Jop.Update;
using JobPlatformBackend.Contracts.Contracts.Shared;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Domain.src.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
		public JobService(ISkillRepository skillRepository, IUserRepository userRepository, ICompanyRepository companyRepository, IJobRepository jobRepository, ILogger<JobService> logger, ISanitizerService sanitizerService)
		{

			_jobRepository = jobRepository;
			_logger = logger;
			_sanitizerService = sanitizerService;
			_companyRepository = companyRepository;
			_skillRepository = skillRepository;
			_userRepository = userRepository;

		}
		public async Task<JobResponseDto> CreatJobAsync(CreateJobRequest request, int currentUserId)
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
			var isAuthorized = await _companyRepository.IsUserAdminOfCompanyAsync(request.CompanyId, currentUserId);
			if (!isAuthorized)
			{
				throw new UnauthorizedAccessException("You are not Authorize by this company");
			}


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


		public async Task<PagedResponseDto<JobResponseDto>> GetAllJobsBySkillNameAsync(GetBySkillNameDto getBySkill)
		{
			int pagesize = getBySkill.pageSize, page = getBySkill.page;

			if (getBySkill.page <= 0) page = 1;
			if (getBySkill.pageSize <= 0) pagesize = 10;

			var (items, totalCount) = await _jobRepository.GetAllBySkillNameAsync(page, pagesize, getBySkill.skill);
			return new PagedResponseDto<JobResponseDto>
			{
				Items = items,
				TotalCount = totalCount,
				PageNumber = page,
				PageSize = pagesize,
			};
		}

		public async Task<PagedResponseDto<JobResponseDto>> GetAllJobsByCompanyIdAsync(GetByCompanyIdDto getByCompanyId)
		{
			var company = await _companyRepository.GetByIdAsync(getByCompanyId.companyId);
			if (company == null)
			{
				throw new BadRequestException("Company not found");
			}

			int pagesize = getByCompanyId.pageSize, page = getByCompanyId.page;
			if (getByCompanyId.page <= 0) page = 1;
			if (getByCompanyId.pageSize <= 0) pagesize = 10;
			var (items, totalCount) = await _jobRepository.GetByCompanyIdAsync(getByCompanyId.companyId, page, pagesize);
			return new PagedResponseDto<JobResponseDto>
			{
				Items = items,
				TotalCount = totalCount,
				PageNumber = page,
				PageSize = pagesize,
			};
		}
		public async Task<JobResponseDto> GetJobById(int id)
		{
			var job = await _jobRepository.Query()
		.Where(x => x.Id == id)
		.Select(job => new JobResponseDto(
			job.Id,
			job.Title,
			job.Description,
			job.Salary,
			job.Location,
			job.TypeJop.ToString(),
			job.ExperieceLevel.ToString(),
			job.CompanyId,
			job.Company.Name,
 			job.JobSkills.Select(js => js.Skill.Name).ToList(),
			job.Applications.Count,
			job.CreatedAt
		))
		.FirstOrDefaultAsync();

			return job;
		}
		public async Task EditJobAsync(int jobId,int adminId,UpdateRequestDto request)
		{
			var job = await _jobRepository.GetJobWithId(jobId);
			if (job is null)
				throw new BadRequestException("job not found");

			var isAuthorized = await _companyRepository.IsUserAdminOfCompanyAsync(job.CompanyId, adminId);
			if (!isAuthorized)
				throw new UnauthorizedAccessException("Not authorized to edit this job");

 			if (!string.IsNullOrWhiteSpace(request.Title))
				job.Title = request.Title;

			if (!string.IsNullOrWhiteSpace(request.Description))
				job.Description = request.Description;

 			if (request.Salary > 0)
				job.Salary = request.Salary;

			if (!string.IsNullOrWhiteSpace(request.Location))
				job.Location = request.Location;

			if (!string.IsNullOrWhiteSpace(request.ExperienceLevel))
				job.ExperieceLevel = request.ExperienceLevel;

 			if (!string.IsNullOrWhiteSpace(request.typeJob) &&
				 Enum.TryParse<TypeJob>(request.typeJob, true, out var parsedType))
			{
				job.TypeJop = parsedType;
			}
			var skillsChanged = await UpdateJobSkillsAsync(job, request.Skills);
			await _jobRepository.UpdateAsync(job);
			await _jobRepository.SaveChangesAsync();

		}

		private async Task<bool> UpdateJobSkillsAsync(Job job,List<string>? newSkillNames)
		{
			if (newSkillNames == null) return false;
			var normalizedNewSkills = newSkillNames.Where(s => !string.IsNullOrWhiteSpace(s))
				.Select(s => s.Trim().ToLower())
				.Distinct().OrderBy(s => s).ToList();

			var currentSkillNames = job.JobSkills.Select(js => js.Skill.Name.ToLower()).OrderBy(s => s).ToList();
			if (normalizedNewSkills.SequenceEqual(currentSkillNames))
			{
				return false; // اطلع من الميثود فوراً، ما في داعي لأي عملية DB
			}

			var skillsToRemove = job.JobSkills.Where(js => !normalizedNewSkills.Contains(js.Skill.Name.ToLower()))
				.ToList();
			foreach (var skillToRemove in skillsToRemove)
				job.JobSkills.Remove(skillToRemove);

			var skillsToAdd = normalizedNewSkills.Where(name => !currentSkillNames.Contains(name)).ToList();

			if (skillsToAdd.Any())
			{
				var existingSkillsInDb = await _skillRepository.GetByNamesAsync(skillsToAdd);
				var existingSkillNames = existingSkillsInDb.Select(s => s.Name.ToLower()).ToList();

				var brandNewSkills = skillsToAdd
							.Where(name => !existingSkillNames.Contains(name))
							.Select(name => new Skill { Name = name })
							.ToList();
				if (brandNewSkills.Any())
				{
					await _skillRepository.AddRangeAsync(brandNewSkills);
					await _skillRepository.SaveChangesAsync(); 
				}
				var allSkillsToConnect = existingSkillsInDb.Concat(brandNewSkills).ToList();
				foreach (var skill in allSkillsToConnect)
				{
					job.JobSkills.Add(new JobSkill { JobId = job.Id, SkillId = skill.Id });
				}
			}
			return true;
		}

		public async Task DeleteJobAsync(int idJob,int adminId) {

			var job = await _jobRepository.GetByIdAsync(idJob);
			if(job==null)
				throw new BadRequestException("This job is not exist");


			var isAuther = await _companyRepository.IsUserAdminOfCompanyAsync(job.CompanyId, adminId);
			if (!isAuther)
				throw new UnauthorizedAccessException("This user is not Authorized");
			await _jobRepository.DeleteAsync(job);
			await _jobRepository.SaveChangesAsync();
 
		}
	}
}
