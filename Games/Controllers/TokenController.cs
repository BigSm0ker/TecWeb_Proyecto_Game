using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gamess.Core.Entities;
using Gamess.Core.Interfaces;
using System.Net;

namespace Games.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        private readonly IPasswordService _passwordService;

        public TokenController(
            IConfiguration configuration,
            ISecurityService securityService,
            IPasswordService passwordService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Autentica un usuario y genera un token JWT
        /// </summary>
        /// <param name="userLogin">Credenciales del usuario</param>
        /// <returns>Token JWT</returns>
        [HttpPost]
        public async Task<IActionResult> Authentication([FromBody] UserLogin userLogin)
        {
            try
            {
                var validation = await IsValidUser(userLogin);
                if (validation.Item1)
                {
                    var token = GenerateToken(validation.Item2);
                    return Ok(new { token });
                }
                return NotFound("Credenciales no válidas");
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Endpoint de diagnóstico para verificar configuración
        /// </summary>
        [HttpGet("Config")]
        public IActionResult GetConfig()
        {
            try
            {
                var connectionStringMySql = _configuration["ConnectionStrings:ConnectionMySql"];

                var result = new
                {
                    connectionStringMySql = connectionStringMySql ?? "MySQL NO CONFIGURADO",
                    AllConnectionStrings = _configuration.GetSection("ConnectionStrings")
                        .GetChildren().Select(x => new { Key = x.Key, Value = x.Value }),
                    Environment = _configuration["ASPNETCORE_ENVIRONMENT"] ??
                        "ASPNETCORE_ENVIRONMENT NO CONFIGURADO",
                    Authentication = _configuration.GetSection("Authentication")
                        .GetChildren().Select(x => new { Key = x.Key, Value = x.Value })
                };

                return Ok(result);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        private async Task<(bool, Security)> IsValidUser(UserLogin userLogin)
        {
            var user = await _securityService.GetLoginByCredentials(userLogin);

            if (user == null)
                return (false, null!);

            var isValidHash = _passwordService.Check(user.Password, userLogin.Password);
            return (isValidHash, user);
        }

        private string GenerateToken(Security security)
        {
            string secretKey = _configuration["Authentication:SecretKey"]!;

            // Header
            var symmetricSecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(
                symmetricSecurityKey,
                SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            // Claims (Body/Payload)
            var claims = new[]
            {
                new Claim("Name", security.Name),
                new Claim("Login", security.Login),
                new Claim(ClaimTypes.Role, security.Role.ToString())
            };

            var payload = new JwtPayload(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(60)
            );

            // Token
            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}