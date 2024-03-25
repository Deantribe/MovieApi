using Microsoft.EntityFrameworkCore;

namespace MovieApi.Models
{
    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options) 
            : base(options)
        {
        }

        public MovieContext()
        {
        }

        private DbSet<Movie> Movies { get; set; } = null!;

        private async Task<List<Movie>> GetAsyncMovies() => await Movies.ToListAsync();

        private List<string> GetGenres() => GetMovies()
            .SelectMany(movie => movie.Genre.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Distinct()
            .ToList();

        public virtual List<Movie> GetMovies() => GetAsyncMovies().Result;

        public bool MatchingGenre(string search) => GetGenres()
            .Any(genre => genre.Contains(search, StringComparison.InvariantCultureIgnoreCase));
    }
}
