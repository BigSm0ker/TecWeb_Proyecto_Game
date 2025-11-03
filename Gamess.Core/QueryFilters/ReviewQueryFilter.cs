using System;

namespace Gamess.Core.QueryFilters
{
    public class ReviewQueryFilter
    {
        public int? GameId { get; set; }
        public int? UserId { get; set; }
        public int? MinScore { get; set; }
        public int? MaxScore { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public bool? IsActive { get; set; }
    }
}
