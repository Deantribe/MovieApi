using Microsoft.EntityFrameworkCore;

namespace MovieApi.Models
{
    [Keyless]
    public class Movie
    {
        public DateTime Release_Date { get; set; }
        public string? Title { get; set; }
        public string? Overview { get; set; }
        public double? Popularity { get; set; }
        public short? Vote_Count { get; set; }
        public double? Vote_Average { get; set; }
        public string? Original_Language { get; set; }
        public string? Genre { get; set; }
        public string? Poster_Url { get; set; }
    }
}