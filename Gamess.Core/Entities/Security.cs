using Gamess.Core.Enum;

namespace Gamess.Core.Entities
{
    public partial class Security : BaseEntity
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public RoleType Role { get; set; }
    }
}