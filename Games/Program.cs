using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Games.Api.Responses;
using Games.Infrastructure.Data;
using Games.Infrastructure.Filters;
using Games.Infrastructure.Mappings;
using Games.Infrastructure.Repositories;
using Games.Infrastructure.Validators;
using Gamess.Core.CustomEntities;
using Gamess.Core.Interfaces;
using Gamess.Core.Services;
using Gamess.Infraestructure.Data;
using Gamess.Infraestructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Configuración base
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true, reloadOnChange: true);

// Variables de entorno (para producción en Azure)
builder.Configuration.AddEnvironmentVariables();

// =====================================================
// CONFIGURACIÓN DE BASE DE DATOS (RAILWAY MYSQL)
// =====================================================
var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
builder.Services.AddDbContext<GamesContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// =====================================================
// AUTOMAPPER
// =====================================================
builder.Services.AddAutoMapper(typeof(MappingProfile));

// =====================================================
// REPOSITORIOS Y UNIT OF WORK
// =====================================================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ISecurityRepository, SecurityRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// =====================================================
// DAPPER
// =====================================================
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<IGameDapperRepository, GameDapperRepository>();

// =====================================================
// SERVICIOS DE NEGOCIO
// =====================================================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IGameDapperService, GameDapperService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();

// =====================================================
// SERVICIO DE HASHING DE CONTRASEÑAS
// =====================================================
builder.Services.Configure<PasswordOptions>(
    builder.Configuration.GetSection("PasswordOptions"));
builder.Services.AddSingleton<IPasswordService, PasswordService>();

// =====================================================
// CONTROLADORES Y FILTROS
// =====================================================
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<ValidationFilter>();
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling =
        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// =====================================================
// FLUENT VALIDATION
// =====================================================
builder.Services.AddValidatorsFromAssemblyContaining<GameDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetByIdRequestValidator>();
builder.Services.AddScoped<IValidationService, ValidationService>();

// =====================================================
// SWAGGER/OPENAPI
// =====================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Games API - Backend Tecnologías Web",
        Version = "v1",
        Description = "API para gestión de videojuegos y reseñas",
        Contact = new OpenApiContact
        {
            Name = "Equipo de Desarrollo",
            Email = "soporte@games.com"
        }
    });

    // Documentación XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.EnableAnnotations();

    // Configuración JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' [espacio] y luego su token en el campo de abajo.\n\nEjemplo: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// =====================================================
// AUTENTICACIÓN JWT
// =====================================================
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
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]!)
        )
    };
});

builder.Services.AddAuthorization();

// =====================================================
// CORS (si es necesario)
// =====================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// =====================================================
// PIPELINE HTTP
// =====================================================

// Swagger (siempre habilitado para desarrollo y producción)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Games API v1");
    options.RoutePrefix = string.Empty; // Swagger en la raíz
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();