using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Games.Api.Responses;
using Gamess.Core.Entities;
using Gamess.Core.Enum;
using Gamess.Core.Interfaces;
using Gamess.Infraestructure.DTOs;

namespace Games.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public SecurityController(
            ISecurityService securityService,
            IMapper mapper,
            IPasswordService passwordService)
        {
            _securityService = securityService;
            _mapper = mapper;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        /// <param name="securityDto">Datos del usuario a registrar</param>
        /// <returns>Usuario registrado</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SecurityDto securityDto)
        {
            try
            {
                var security = _mapper.Map<Security>(securityDto);
                security.Password = _passwordService.Hash(securityDto.Password);

                await _securityService.RegisterUser(security);

                securityDto = _mapper.Map<SecurityDto>(security);
                var response = new ApiResponse<SecurityDto>(securityDto);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}