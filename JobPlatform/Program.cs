using CloudinaryDotNet;
using Ganss.Xss;
using JobPlatformBackend.API.Middleware;
using JobPlatformBackend.Business.src.Managers;
using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Business.src.Services.Implementations;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Infrastructure.src.Database;
using JobPlatformBackend.Infrastructure.src.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.Configure<EmailOption>(builder.Configuration.GetSection("EmailSettings"));

 
builder.Services.AddTransient<LoggingMiddleWare>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped(typeof(BaseService<,>));
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<JwtManager>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<HtmlSanitizer>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddScoped<ISanitizerService,SanitizerService>();   
builder.Services.AddScoped<ISkillRepository,SkillRepository>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();  
builder.Services.AddScoped<ICloudinaryService,CloudinaryService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();  

var cloudinarySection = builder.Configuration.GetSection("CloudinarySettings");
var cloudName = cloudinarySection["CloudName"];
var apiKey = cloudinarySection["ApiKey"];
var apiSecret = cloudinarySection["ApiSecret"];

if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
{
	throw new Exception("Cloudinary settings are missing in appsettings.json");
}

var account = new Account(cloudName, apiKey, apiSecret);
builder.Services.AddSingleton(new Cloudinary(account));

 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("JwtOptions");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["SecretKey"])
                )
        };
        options.Events = new JwtBearerEvents { 
        OnAuthenticationFailed = context =>
        {
			Console.WriteLine("Jwt Auth Failed"+ context.Exception.Message);
            return Task.CompletedTask;
        }
        };
    });





builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy
			.AllowAnyOrigin()   
			.AllowAnyMethod()   
			.AllowAnyHeader();  
	});
});


var app = builder.Build();

// 1. √Ê· ‘Ì¡ œ«∆„« ÂÊ «·‹ Exception Middleware ⁄‘«‰ Ì„”ﬂ √Ì Œÿ√ »’Ì—  Õ ÌÂ
app.UseMiddleware<ExceptionMiddleware>();

//if (app.Environment.IsDevelopment())
//{
//	// ‘Ì·Â« √Ê Œ·ÌÂ« »⁄œ «·„Ìœ· ÊÌ—  »⁄ﬂ° »” «·√›÷·  ⁄ „œ ⁄·Ï  »⁄ﬂ ⁄‘«‰  ‘Ê› «·‹ JSON «··Ì »œﬂ «Ì«Â
//	 app.UseDeveloperExceptionPage();
//}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// «·ﬂÊ—” ﬁ»· «·‹ Auth œ«∆„«
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LoggingMiddleWare>(); // Œ·¯Ì «··ÊÃ‰Ã ¬Œ— ‘Ì¡ ﬁ»· «·ﬂ‰ —Ê·—“

app.MapControllers();

app.Run();