
namespace Gamess.Infraestructure.DTOs
{
    
    public class ReviewDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public byte Score { get; set; }     
        public DateTime CreatedAt { get; set; }
    }

}
