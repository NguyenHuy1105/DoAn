using ComputerManagement.BO;
using ComputerManagement.BO.DTO;
using ComputerManagement.Common.Configs;
using ComputerManagement.Service;
using ComputerManagement.Service.Implement;
using ComputerManagerment.Repos.Implement;
using ComputerManagerment.Repos.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using AutoMapper;
using ComputerManagement.Data;
using ComputerManagement.BO.Lib.Implement;
using ComputerManagement.BO.Lib.Interface;
using ComputerManagement.Data.Seed;
using ComputerManagement.Api.Middlewares;
using ComputerManagement.Service.Interface;
using Newtonsoft.Json;
using ComputerManagement.Service.Worker;
using ComputerManagement.Service.Queue;
using Microsoft.AspNetCore.WebSockets;
using ComputerManagement.Service.Hubs;
using Microsoft.Extensions.Configuration;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");
//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseSqlServer(connectionString, sqlServerOptions =>
//    {
//        sqlServerOptions.MigrationsAssembly("ComputerManagement.Data");
//    });
//    options.EnableSensitiveDataLogging();
//});

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.MigrationsAssembly("ComputerManagement.Data");
    });
    options.EnableSensitiveDataLogging();
});
builder.Services.AddTransient<SeederData>();

// add di jwtconfig
builder.Services.Configure<JwtConfig>(options =>
{
    builder.Configuration.GetSection("Jwt").Bind(options);
});

// add di fileConfig
builder.Services.Configure<FileConfig>(options =>
{
    builder.Configuration.GetSection("FileConfig").Bind(options);
});

// add di emailConfig
builder.Services.Configure<EmailConfig>(options =>
{
    builder.Configuration.GetSection("EmailConfig").Bind(options);
});

// add di rabbitConfig
builder.Services.Configure<RabbitMQConfig>(options =>
{
    builder.Configuration.GetSection("RabbitMQConfig").Bind(options);
});

// add authen
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            //tự cấp token
            ValidateIssuer = false,
            ValidateAudience = false,

            // ký token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"])),
            ValidateLifetime = true,
        };

    });

// socket 
// add auto mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// add di
// Đọc cấu hình từ appsettings.json
var baseUrlConfig = builder.Configuration.GetSection("BaseUrlConfig").Get<BaseUrlConfig>();

// Đăng ký BaseUrlConfig như một singleton
builder.Services.AddSingleton(baseUrlConfig);

builder.Services.AddSingleton<ShareDb>();
builder.Services.AddSingleton<MonitorSessionHub>();
builder.Services.AddScoped<IEmailService,EmailService>();
builder.Services.AddScoped<ContextData>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserRepop, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IComputerRoomRepo, ComputerRoomRepo>();
builder.Services.AddScoped<IComputerRoomService, ComputerRoomService>();

builder.Services.AddScoped<IComputerRepo, ComputerRepo>();
builder.Services.AddScoped<IComputerService, ComputerService>();

builder.Services.AddScoped<IComputerHistoryRepo, ComputerHistoryRepo>();
builder.Services.AddScoped<IComputerHistoryService, ComputerHistoryService>();

builder.Services.AddScoped<ISoftwareService, SoftwareService>();
builder.Services.AddScoped<ISoftwareRepo, SoftwareRepo>();

builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IFileRepo, FileRepo>();

builder.Services.AddScoped<IMonitorSessionRepo, MonitorSessionRepo>();
builder.Services.AddScoped<IMonitorSessionService, MonitorSessionService>();

builder.Services.AddScoped<IComputerStateRepo, ComputerStateRepo>();

builder.Services.AddScoped<ICommandOptionService, CommandOptionService>();
builder.Services.AddScoped<ICommandOptionRepo, CommandOptionRepo>();

builder.Services.AddScoped<IComputerSoftwareRepo, ComputerSoftwareRepo>();
builder.Services.AddScoped<IComputerSoftwareService, ComputerSoftwareService>();

builder.Services.AddScoped<IConfigOptionRepo, ConfigOptionRepo>();
builder.Services.AddScoped<IConfigOptionService, ConfigOptionService>();

builder.Services.AddScoped<IAgentRepo, AgentRepo>();
builder.Services.AddScoped<IAgentService, AgentService>();

builder.Services.AddScoped<IStudentRepo, StudentRepo>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<IFileProofRepo, FileProofRepo>();
builder.Services.AddScoped<IFileProofService, FileProofService>();



builder.Services.AddHostedService<CommandOptionJob>();
// add cors

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy",
        policy =>
        {
            policy.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
            policy.WithOrigins(baseUrlConfig.Frontend) // baseUrlConfig.Frontend
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });

});

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    var scopedFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<SeederData>();
        service.Seed();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthMiddleware>();
app.MapControllers();
app.MapHub<MonitorSessionHub>("/ws");

app.Run();
