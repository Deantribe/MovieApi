using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.Models;
using static MovieApi.Enums.Enums;


namespace MovieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieContext _context;

        public MoviesController(MovieContext context)
        {
            _context = context;
        }

        [HttpGet("SearchByTitle/{title}")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovieByTitle(string title, int numberOfResults = 10, int pageNumber = 1, SortBy sortBy = SortBy.Title)
        {
            if (numberOfResults <= 0) return BadRequest($"Number of Results must be more than 0 but was: {numberOfResults}");
            if (pageNumber <= 0) return BadRequest($"Page Number must be more than 0 but was: {pageNumber}");
            if (_context.Movies == null) return NotFound();

            var movies = await _context.Movies
                .Where(a => EF.Functions.Like(a.Title, $"%{title}%"))
                .OrderBy(a => sortBy == SortBy.Title ? a.Title : a.Release_Date.ToString())
                .Skip(numberOfResults * (pageNumber - 1))
                .Take(numberOfResults)
                .ToListAsync();
            
            return movies.Count() == 0 ? NotFound() : movies;
        }
        
        [HttpGet("SearchByGenre/{genre}")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovieByGenre(string genre, int numberOfResults = 10, int pageNumber = 1, SortBy sortBy = SortBy.Title)
        {
            if (numberOfResults <= 0) return BadRequest($"Number of Results must be more than 0 but was: {numberOfResults}");
            if (pageNumber <= 0) return BadRequest($"Page Number must be more than 0 but was: {pageNumber}");
            if (_context.Movies == null) return NotFound();

            var genreMatch = _context.Movies.Any(a => a.Genre.ToLower().Contains(genre.ToLower()));
            if (!genreMatch) return NotFound($"The requested genre was not found: {genre}");
            
            var movies = await _context.Movies
                .Where(a => EF.Functions.Like(a.Genre, $"%{genre}%"))
                .OrderBy(a => sortBy == SortBy.Title ? a.Title : a.Release_Date.ToString())
                .Skip(numberOfResults * (pageNumber - 1))
                .Take(numberOfResults)
                .ToListAsync();
            
            return movies.Count() == 0 ? NotFound() : movies;
        }
    }
}
