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

var cloudinarySettings=builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
var account = new Account(
    cloudinarySettings?.CloudName,
    cloudinarySettings?.ApiKey,
        cloudinarySettings?.ApiSecret
    )
    ;

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









var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
});
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggingMiddleWare>();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage(); // »ÿ·⁄·ﬂ «·Œÿ√ »«· ›’Ì· «·„„· ›Ì Swagger
}
else
{
	app.UseExceptionHandler("/error");
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
