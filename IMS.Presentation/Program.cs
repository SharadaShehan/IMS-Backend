using FluentValidation;
using IMS.Application.DTO;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using IMS.Infrastructure.Extensions;
using IMS.Infrastructure.Repositories;
using IMS.Presentation.Services;
using IMS.Presentation.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

// Get the JWT Key and Application Insights Connection String from environment variables
var JwtKey = Environment.GetEnvironmentVariable("AUTH_SERVER_CLIENT_SECRET");
var AppInsightsConnString = Environment.GetEnvironmentVariable("APPLICATION_INSIGHTS_CONNECTION_STRING");
if (JwtKey == null || AppInsightsConnString == null)
{
    throw new Exception("JWT Key or Application Insights Connection String not found in environment variables.");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin() // Allow all origins (or specify specific origins)
                .AllowAnyHeader() // Allow all headers
                .AllowAnyMethod(); // Allow all HTTP methods (GET, POST, etc.)
        }
    );
});

builder.Services.AddControllers();

builder.Services.AddScoped<IValidator<CreateEquipmentDTO>, CreateEquipmentValidator>();
builder.Services.AddScoped<IValidator<CreateItemDTO>, CreateItemValidator>();
builder.Services.AddScoped<IValidator<CreateLabDTO>, CreateLabValidator>();
builder.Services.AddScoped<IValidator<CreateMaintenanceDTO>, CreateMaintenanceValidator>();
builder.Services.AddScoped<IValidator<PresignedUrlRequestDTO>, PreSignedUrlGenValidator>();
builder.Services.AddScoped<IValidator<RequestEquipmentDTO>, RequestEquipmentValidator>();
builder.Services.AddScoped<IValidator<RespondReservationDTO>, RespondReservationValidator>();
builder.Services.AddScoped<IValidator<ReviewMaintenanceDTO>, ReviewMaintenanceValidator>();
builder.Services.AddScoped<IValidator<SubmitMaintenanceDTO>, SubmitMaintenanceValidator>();
builder.Services.AddScoped<IValidator<UpdateEquipmentDTO>, UpdateEquipmentValidator>();
builder.Services.AddScoped<IValidator<UpdateLabDTO>, UpdateLabValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer",
        }
    );

    opt.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );
});

// Add services from Infrastructure Layer
builder.Services.AddInfrastructure();
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
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JwtKey)
            ),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (config) =>
        config.ConnectionString = AppInsightsConnString,
    configureApplicationInsightsLoggerOptions: (options) => { }
);

builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Default", LogLevel.Trace);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
