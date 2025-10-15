using System;
using System.Collections.Generic;


namespace Gamess.Core.Entities;
public partial class Game
{
    public int Id { get; set; }
    public int UploaderUserId { get; set; }
    public string Title { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public DateTime? ReleaseDate { get; set; }
    public string? AgeRating { get; set; }   
    public int? MinAge { get; set; }
    public string? CoverUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public User Uploader { get; set; } = null!;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();



}
