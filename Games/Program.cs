using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Games.Infrastructure.Data;            // GamesContext (EF)
using Games.Infrastructure.Mappings;

using FluentValidation;

using Gamess.Core.Interfaces;               // IUnitOfWork, repos y servicios (Core)
using Games.Infrastructure.Repositories;    // EF repos
using Gamess.Core.Services;                 // Services EF

using Games.Infrastructure.Filters;         // ValidationFilter, GlobalExceptionFilter
using Games.Infrastructure.Validators;      // FluentValidation DTO validators

using Gamess.Infraestructure.Data;          
using Gamess.Infraestructure.Repositories;  
using Gamess.Core.Services;                
namespace Games
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) Base de Datos (MySQL)
            var csMySql = builder.Configuration.GetConnectionString("ConnectionMySql");
            builder.Services.AddDbContext<GamesContext>(options =>
                options.UseMySql(csMySql, ServerVersion.AutoDetect(csMySql)));

            // 2) MVC + JSON + Filtros
            builder.Services
                .AddControllers(opts =>
                {
                    opts.Filters.Add<ValidationFilter>();
                    opts.Filters.Add<GlobalExceptionFilter>();
                })
                .AddNewtonsoftJson(o =>
                {
                    o.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .ConfigureApiBehaviorOptions(o =>
                {
                    o.SuppressModelStateInvalidFilter = true;
                });

            // 3) AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // 4) Repos EF
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

            // 5) Servicios EF
            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();

            // 6) Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 7) FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<GameDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ReviewDtoValidator>();
            builder.Services.AddScoped<IValidationService, ValidationService>();

            // 8) Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Games API",
                    Version = "v1",
                    Description = "API de Games con Users y Reviews (Repositorio, UoW, Validación, Dapper)."
                });
            });

            // 9) Dapper & Factory
            builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<IDapperContext, DapperContext>();
            builder.Services.AddScoped<IGameDapperRepository, GameDapperRepository>();
            builder.Services.AddScoped<IGameDapperService, GameDapperService>();

            // 10) Pipeline HTTP
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Games API v1"));
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
