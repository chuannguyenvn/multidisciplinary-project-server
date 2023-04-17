using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using Server.Services;
using DbContext = Server.DbContext;

var builder = WebApplication.CreateBuilder(args);

var settings = new Settings();
builder.Configuration.Bind("Settings", settings);
builder.Services.AddSingleton(settings);

builder.Services.AddHttpClient();

builder.Services.AddDbContext<DbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IPlantManagementService, PlantManagementService>();
builder.Services.AddScoped<IPlantDataService, PlantDataService>();
builder.Services.AddSingleton<WateringRuleTestingService>();

builder.Services.AddSingleton<AdafruitMqttService>();
builder.Services.AddHostedService<AdafruitMqttService>(p => p.GetRequiredService<AdafruitMqttService>());
builder.Services.AddSingleton<WateringService>();
builder.Services.AddHostedService<WateringService>(p => p.GetRequiredService<WateringService>());

builder.Services.AddSingleton<HelperService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.BearerKey)),
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = false,
        };
    });

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();