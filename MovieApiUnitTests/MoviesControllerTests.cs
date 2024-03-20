using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MovieApi.Controllers;
using MovieApi.Models;
using static MovieApi.Enums.Enums;

namespace MovieApiUnitTests
{
    public class MoviesControllerTests
    {
        private Mock<MovieContext> _moviesContext;

        [SetUp]
        public void Setup()
        {
            List<Movie> Movies = new()
            {
                new()
                {
                    Release_Date = new DateTime(2024, 3, 20),
                    Title = "Test",
                    Overview = "Test movie",
                    Popularity = 100,
                    Vote_Count = 1111,
                    Vote_Average = 7.545,
                    Original_Language = "en",
                    Genre = "Action",
                    Poster_Url = "Test.png",
                },
                new()
                {
                    Release_Date = new DateTime(2023, 3, 20),
                    Title = "A Test",
                    Overview = "Test movie",
                    Popularity = 100,
                    Vote_Count = 1111,
                    Vote_Average = 7.545,
                    Original_Language = "en",
                    Genre = "Action",
                    Poster_Url = "Test.png",
                },
                new()
                {
                    Release_Date = new DateTime(2022, 2, 22),
                    Title = "The Test",
                    Overview = "Test movie",
                    Popularity = 100,
                    Vote_Count = 1111,
                    Vote_Average = 7.545,
                    Original_Language = "en",
                    Genre = "Action",
                    Poster_Url = "Test.png",
                },
                new()
                {
                    Release_Date = new DateTime(1999, 11, 02),
                    Title = "A Test Movie",
                    Overview = "Test movie",
                    Popularity = 100,
                    Vote_Count = 1111,
                    Vote_Average = 7.545,
                    Original_Language = "en",
                    Genre = "Action",
                    Poster_Url = "Test.png",
                },
            };

            var moviesQueryable = Movies.AsQueryable();
            _moviesContext = new Mock<MovieContext>();

            var dbSetMock = new Mock<DbSet<Movie>>();

            dbSetMock.As<IQueryable<Movie>>().Setup(m => m.Expression).Returns(moviesQueryable.Expression);
            dbSetMock.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(moviesQueryable.ElementType);
            dbSetMock.As<IQueryable<Movie>>().Setup(m => m.GetEnumerator()).Returns(moviesQueryable.GetEnumerator());

            _moviesContext.Object.Movies = dbSetMock.Object;
        }

        [Test]
        public async Task GetWithBadNumberOfResults()
        {
            var controller = new MoviesController(_moviesContext.Object);
            int numberOfResults = -1;
            ActionResult<IEnumerable<Movie>> result = await controller.GetMovieByTitle("Test", numberOfResults, 1, SortBy.Title);
            
            Assert.IsNotNull(result);

            BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result.Result;
            Assert.That(400, Is.EqualTo(badRequestObjectResult.StatusCode));
            Assert.That($"Number of Results must be more than 0 but was: {numberOfResults}", Is.EqualTo(badRequestObjectResult.Value));
        }

        [Test]
        public async Task GetWithBadPageNumber()
        {
            var controller = new MoviesController(_moviesContext.Object);
            int pageNumber = -1;
            ActionResult<IEnumerable<Movie>> result = await controller.GetMovieByTitle("Test", 2, pageNumber, SortBy.Title);

            Assert.IsNotNull(result);

            BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result.Result;
            Assert.That(400, Is.EqualTo(badRequestObjectResult.StatusCode));
            Assert.That($"Page Number must be more than 0 but was: {pageNumber}", Is.EqualTo(badRequestObjectResult.Value));
        }

        //[Test]
        //[TestCase("Test", 2, 1, SortBy.Title)]
        //public async Task TestGetMovieByTitle(string title, int numberOfResults, int pageNumber, SortBy sortBy)
        //{
        //    var controller = new MoviesController(_moviesContext.Object);
        //    ActionResult<IEnumerable<Movie>> result = await controller.GetMovieByTitle(title, numberOfResults, pageNumber, sortBy);
        //    Assert.IsNotNull(result);
        //    Assert.That(2, Is.EqualTo(result.Value.Count()));
        //}
        //}
    }
}