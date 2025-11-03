namespace Gamess.Core.QueryFilters
{
    public class GameQueryFilter
    {
        public string? Title { get; set; }
        public string? Genre { get; set; }
        public int? ReleaseYear { get; set; }
        public int? MinAge { get; set; }
        public string? AgeRating { get; set; }
        public bool? IsActive { get; set; }   
    }
}
