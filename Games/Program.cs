using Microsoft.EntityFrameworkCore;
using Games.Infrastructure.Data;

using AutoMapper;
using Games.Infrastructure.Mappings;

using Gamess.Core.Interfaces;
using Games.Infrastructure.Repositories;

using Gamess.Core.Services;

using FluentValidation;
using Games.Infrastructure.Filters;
using Games.Infrastructure.Validators;

using Microsoft.OpenApi.Models; // Swagger

namespace Games
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===========================
            // 1) Base de Datos (MySQL)
            // ===========================
            var csMySql = builder.Configuration.GetConnectionString("ConnectionMySql");
            builder.Services.AddDbContext<GamesContext>(options =>
                options.UseMySql(csMySql, ServerVersion.AutoDetect(csMySql)));

            // ===========================
            // 2) MVC + JSON + Filtro Validación
            // ===========================
            builder.Services
                .AddControllers(opts =>
                {
                    // Filtro global de FluentValidation
                    opts.Filters.Add<ValidationFilter>();
                    opts.Filters.Add<GlobalExceptionFilter>(); 
                })
                .AddNewtonsoftJson(options =>
                {
                    // Evita ciclos de navegación (Game -> Reviews -> Game ...)
                    options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // Dejamos a FluentValidation manejar errores de modelo
                    options.SuppressModelStateInvalidFilter = true;
                });

            // ===========================
            // 3) AutoMapper
            // ===========================
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // ===========================
            // 4) Repositorios (Scoped recomendado con EF Core)
            // ===========================
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

            // ===========================
            // 5) Servicios de Dominio
            // ===========================
            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();

            // ===========================
            // 6) Unit of Work
            // ===========================
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ===========================
            // 7) Validaciones (FluentValidation)
            // ===========================
            builder.Services.AddValidatorsFromAssemblyContaining<GameDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ReviewDtoValidator>();
            builder.Services.AddScoped<IValidationService, ValidationService>();

            // ===========================
            // 8) Swagger (PDF pide doc del controlador)
            // ===========================
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Games API",
                    Version = "v1",
                    Description = "API de Games con Users y Reviews (Repositorio, UoW, Validación)."
                });
            });

            // ===========================
            // 9) Pipeline HTTP
            // ===========================
            var app = builder.Build();

            // Swagger solo en Development (puedes activarlo siempre si quieres)
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Games API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
