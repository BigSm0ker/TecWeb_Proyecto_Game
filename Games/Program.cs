using Microsoft.EntityFrameworkCore;
using Games.Infrastructure.Data;
using Gamess.Core.Entities;
using AutoMapper;
using Games.Infrastructure.Repositories;
using Gamess.Core.Interfaces;
using Games.Infrastructure.Mappings;
using FluentValidation;
using Games.Infrastructure.Filters;
using Games.Infrastructure.Validators;

namespace Games
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configurar la BD SqlServer
            // var connectionString = builder.Configuration.GetConnectionString("ConnectionSqlServer");
            // builder.Services.AddDbContext<GamesContext>(options => options.UseSqlServer(connectionString));
            #endregion

            #region Configurar la BD MySql
            var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
            builder.Services.AddDbContext<GamesContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            #endregion

            // =====================================
            // CONFIGURACIÓN DE SERVICIOS PRINCIPALES
            // =====================================

           
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    
                    options.SuppressModelStateInvalidFilter = true;
                });

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Repositorios
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IGameRepository, GameRepository>();
            builder.Services.AddTransient<IReviewRepository, ReviewRepository>();

            // Validadores (FluentValidation)
            builder.Services.AddValidatorsFromAssemblyContaining<GameDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ReviewDtoValidator>();

            // Servicio de validación (resuelve IValidator<T>)
            builder.Services.AddScoped<IValidationService, ValidationService>();

            // Filtro global de validación
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            // =====================================
            var app = builder.Build();

            // Middleware
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}

// Compare this snippet from Games.Infraestructure/Validators/ReviewDtoValidator.cs:
