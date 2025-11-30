using Gamess.Core.Enum;

namespace Gamess.Infraestructure.DTOs
{
    public class SecurityDto
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public RoleType? Role { get; set; }
    }
}