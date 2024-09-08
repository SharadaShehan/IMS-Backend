using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IMS.Infrastructure.Extensions;
using System.Diagnostics;
using IMS.Presentation.Services;
using IMS.Application.Services;
using IMS.Application.Interfaces;
using IMS.Infrastructure.Repositories;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services from Infrastructure Layer
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ITokenParser, TokenParser>();
builder.Services.AddScoped<IQRTokenProvider, QRTokenProvider>();

// Register Application Layer services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LabService>();
builder.Services.AddScoped<EquipmentService>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<MaintenanceService>();
builder.Services.AddScoped<ReservationService>();

// Register Infrastructure Layer repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILabRepository, LabRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

//Authentication
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateLifetime = false,
		ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
