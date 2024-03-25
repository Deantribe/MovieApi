using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("Search")]
        public ActionResult<List<Movie>> GetMovies(string search, SearchBy searchBy = SearchBy.Title, int numberOfResults = 10, int pageNumber = 1, OrderBy orderBy = OrderBy.Title)
        {
            Result<ActionResult> validate = ValidateInput(numberOfResults, pageNumber, searchBy == SearchBy.Genre, search);
            if (!validate.Success) return validate.Return;

            List<Movie> movies = _context.GetMovies()
                .Where(movie => SearchByType(searchBy, search, movie))
                .OrderBy(movie => OrderByType(orderBy, movie))
                .Skip(numberOfResults * (pageNumber - 1))
                .Take(numberOfResults)
                .ToList();

            return movies.Count() == 0 
                ? NotFound($"Could not find any movies for the search: {search}")
                : movies;
        }

        private Result<ActionResult> ValidateInput(int numberOfResults, int pageNumber, bool searchByGenre, string search) 
        {
            if (numberOfResults <= 0)
            {
                return Result<ActionResult>.ReturnFail(BadRequest($"Number of Results must be more than 0 but was: {numberOfResults}"));
            }

            if (pageNumber <= 0)
            {
                return Result<ActionResult>.ReturnFail(BadRequest($"Page Number must be more than 0 but was: {pageNumber}"));
            }

            if (searchByGenre && !_context.MatchingGenre(search)) 
            {
                return Result<ActionResult>.ReturnFail(NotFound($"The requested genre was not found: {search}"));
            }

            return Result<ActionResult>.ReturnSuccess();
        }

        private bool SearchByType(SearchBy searchBy, string search, Movie movie) => searchBy switch
        {
            SearchBy.Title => movie.Title.Contains(search, StringComparison.InvariantCultureIgnoreCase),
            SearchBy.Genre => movie.Genre.Contains(search, StringComparison.InvariantCultureIgnoreCase),
            _ => throw new NotImplementedException(),
        };

        private string? OrderByType(OrderBy orderBy, Movie movie) => orderBy switch
        {
            OrderBy.Title => movie.Title,
            OrderBy.Release_Date => movie.Release_Date.Ticks.ToString(),
            _ => throw new NotImplementedException(),
        };
    }
}
