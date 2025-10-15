using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamess.Core.Entities;

public partial class Review
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = null!;
    public byte Score { get; set; }           
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public Game Game { get; set; } = null!;
    public User User { get; set; } = null!;
}

