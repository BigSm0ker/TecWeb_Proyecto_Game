namespace Gamess.Infraestructure.DTOs;
public class GameDto
{
    public int Id { get; set; }
    public int UploaderUserId { get; set; }
    public string Title { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public DateTime? ReleaseDate { get; set; }
    public string? AgeRating { get; set; }
    public int? MinAge { get; set; }
    public string? CoverUrl { get; set; }
    public double? AverageScore { get; set; }
    public int ReviewsCount { get; set; }
}
