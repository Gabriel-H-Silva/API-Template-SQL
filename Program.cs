using Microsoft.EntityFrameworkCore;
using ManagerIO.Model.Context;
using ManagerIO.Repository;
using ManagerIO.Repository.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Rewrite;
using ManagerIO.Business.Interfaces;
using Microsoft.IdentityModel.Tokens;
using ManagerIO.Business;
using ManagerIO.Services;
using ManagerIO.Configurations;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
var appName = "Minha Base de Api";
var appVersion = "v1";

builder.Services.AddRouting(option => option.LowercaseUrls = true);

var tokenConfigurations = new TokenConfiguration();

new ConfigureFromConfigurationOptions<TokenConfiguration>(
        builder.Configuration.GetSection("TokenConfiguration")
    )
    .Configure(tokenConfigurations);

builder.Services.AddSingleton(tokenConfigurations);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = tokenConfigurations.Issuer,
            ValidAudience = tokenConfigurations.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.Secret))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = appName,
            Version = "v1",
            Description = $"API Base Desenvolvida por Gabriel - H",
            Contact = new OpenApiContact
            {
                Name = "Gabriel H"
            }
        });
});

builder.Services.AddCors(options => options.AddDefaultPolicy(builder =>
{
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
}));

builder.Services.AddControllers();

var connection = builder.Configuration["MySQLConnection:MySQLConnectionString"];
builder.Services.AddDbContext<MySQLContext>(options => options.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 29))));

// Add services to the container.

builder.Services.AddApiVersioning();

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUsersBusiness, UsersBusiness>();
builder.Services.AddScoped<ILoginBusiness, LoginBusiness>();
builder.Services.AddScoped<UsersRepository>();

builder.Services.AddTransient<ITokenServices, TokenServices>();



var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{appName} - {appVersion}"); });
}


var option = new RewriteOptions();
option.AddRedirect("^$", "swagger");
app.UseRewriter(option);

app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute("DefaultApi", "{controller=values}/v{version=apiVersion}/{id?}");

app.Run();

