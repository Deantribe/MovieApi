using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieApi.Controllers;
using MovieApi.Models;
using System.Reflection;
using static MovieApi.Enums.Enums;

namespace MovieApiUnitTests.Controllers
{
    public class MoviesControllerTests
    {
        private Mock<MovieContext> _moviesContext;

        [SetUp]
        public void Setup()
        {
            List<Movie> movies = new()
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

            _moviesContext = new Mock<MovieContext>();

            _moviesContext.Setup(a => a.GetMovies()).Returns(movies);
        }

        #region GetMovies

        [Test]
        public void GetMoviesSuccess()
        {
            var controller = new MoviesController(_moviesContext.Object);

            ActionResult<List<Movie>> result = controller.GetMovies("test", SearchBy.Title, 2, 1, OrderBy.Title);

            Assert.That(result.Value.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetMoviesFailValidate()
        {
            var controller = new MoviesController(_moviesContext.Object);
            int numberOfResults = -1;

            ActionResult<List<Movie>> result = controller.GetMovies("test", SearchBy.Title, numberOfResults, 1, OrderBy.Title);
            BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result.Result;

            Assert.That(badRequestObjectResult.Value, Is.EqualTo($"Number of Results must be more than 0 but was: {numberOfResults}"));
        }

        [Test]
        public void GetMoviesFailNotFound()
        {
            var controller = new MoviesController(_moviesContext.Object);
            string search = "tooSpecific";

            ActionResult<List<Movie>> result = controller.GetMovies(search, SearchBy.Title, 2, 1, OrderBy.Title);
            NotFoundObjectResult notFoundObjectResult = (NotFoundObjectResult)result.Result;

            Assert.That(notFoundObjectResult.Value, Is.EqualTo($"Could not find any movies for the search: {search}"));
        }

        #endregion

        #region ValidateInput

        [Test]
        public void ValidateInputSuccess()
        {
            Type type = typeof(MoviesController);
            var controller = Activator.CreateInstance(type, _moviesContext.Object);
            MethodInfo method = type.GetMethod("ValidateInput", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { 2, 1, false, string.Empty };

            Result<ActionResult> result = (Result<ActionResult>)method.Invoke(controller, parameters);

            Assert.True(result.Success);
        }

        [Test]
        public void ValidateInputFailNumberOfResults()
        {
            Type type = typeof(MoviesController);
            var controller = Activator.CreateInstance(type, _moviesContext.Object);
            MethodInfo method = type.GetMethod("ValidateInput", BindingFlags.NonPublic | BindingFlags.Instance);
            int numberOfResults = -1;
            object[] parameters = { numberOfResults, 1, false, string.Empty };

            Result<ActionResult> result = (Result<ActionResult>)method.Invoke(controller, parameters);
            BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result.Return;

            Assert.False(result.Success);
            Assert.That(badRequestObjectResult.Value, Is.EqualTo($"Number of Results must be more than 0 but was: {numberOfResults}"));
        }

        [Test]
        public void ValidateInputFailPageNumber()
        {
            Type type = typeof(MoviesController);
            var controller = Activator.CreateInstance(type, _moviesContext.Object);
            MethodInfo method = type.GetMethod("ValidateInput", BindingFlags.NonPublic | BindingFlags.Instance);
            int pageNumber = -1;
            object[] parameters = { 2, pageNumber, false, string.Empty };

            Result<ActionResult> result = (Result<ActionResult>)method.Invoke(controller, parameters);
            BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result.Return;

            Assert.False(result.Success);
            Assert.That(badRequestObjectResult.Value, Is.EqualTo($"Page Number must be more than 0 but was: {pageNumber}"));
        }

        [Test]
        public void ValidateInputFailMatchingGenre()
        {
            Type type = typeof(MoviesController);
            var controller = Activator.CreateInstance(type, _moviesContext.Object);
            MethodInfo method = type.GetMethod("ValidateInput", BindingFlags.NonPublic | BindingFlags.Instance);
            string search = "genre";
            object[] parameters = { 2, 1, true, search };

            Result<ActionResult> result = (Result<ActionResult>)method.Invoke(controller, parameters);
            NotFoundObjectResult notFoundObjectResult = (NotFoundObjectResult)result.Return;

            Assert.False(result.Success);
            Assert.That(notFoundObjectResult.Value, Is.EqualTo($"The requested genre was not found: {search}"));
        }

        #endregion

        #region SearchByType

        [Test]
        public void SearchByTypeTitleTrue()
        {
            Type type = typeof(MoviesController);
            var controller = Activator.CreateInstance(type, _moviesContext.Object);
            MethodInfo method = type.GetMethod("SearchByType", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters =
            { 
                SearchBy.Title, 
                "title", 
                new Movie()
                {
                    Release_Date = new DateTime(1999, 11, 02),
                    Title = "title",
                },
            };

            bool result = (bool)method.Invoke(controller, parameters);

            Assert.True(result);
        }

        [Test]
        public void SearchByTypeTitleFalse()
        {
            Type type = typeof(MoviesController);
            var controller = Activator.CreateInstance(type, _moviesContext.Object);
            MethodInfo method = type.GetMethod("SearchByType", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters =
            {
                SearchBy.Title,
                "alt",
                new Movie()
                {
                    Release_Date = new DateTime(1999, 11, 02),
                    Title = "title",
                },
            };

            bool result = (bool)method.Invoke(controller, parameters);

            Assert.False(result);
        }

        [Test]
        public void SearchByTypeGenreTrue()
        {
            Type type = typeof(MoviesController);
            var controller = Activator.CreateInstance(type, _moviesContext.Object);
            MethodInfo method = type.GetMethod("SearchByType", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters =
            {
                SearchBy.Genre,
                "genre",
                new Movie()
                {
                    Release_Date = new DateTime(1999, 11, 02),
                    Genre = "genre",
                },
            };

            bool result = (bool)method.Invoke(controller, parameters);

            Assert.True(result);
        }

        [Test]
        public void SearchByTypeGenreFalse()
        {
            Type type = typeof(MoviesController);
            var controller = Activator.CreateInstance(type, _moviesContext.Object);
            MethodInfo method = type.GetMethod("SearchByType", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters =
            {
                SearchBy.Genre,
                "alt",
                new Movie()
                {
                    Release_Date = new DateTime(1999, 11, 02),
                    Genre = "genre",
                },
            };

            bool result = (bool)method.Invoke(controller, parameters);

            Assert.False(result);
        }

        #endregion

        #region OrderByType

        [Test]
        public void OrderByTypeTitle()
        {
            Type type = typeof(MoviesController);
            var controller = Activator.CreateInstance(type, _moviesContext.Object);
            MethodInfo method = type.GetMethod("OrderByType", BindingFlags.NonPublic | BindingFlags.Instance);
            Movie movie = new()
            {
                Release_Date = new DateTime(1999, 11, 02),
                Title = "title",
            };
            object[] parameters =
            {
                OrderBy.Title,
                movie,
            };

            string result = (string)method.Invoke(controller, parameters);

            Assert.That(result, Is.EqualTo(movie.Title));
        }

        [Test]
        public void OrderByTypeReleaseDate()
        {
            Type type = typeof(MoviesController);
            var controller = Activator.CreateInstance(type, _moviesContext.Object);
            MethodInfo method = type.GetMethod("OrderByType", BindingFlags.NonPublic | BindingFlags.Instance);
            Movie movie = new()
            {
                Release_Date = new DateTime(1999, 11, 02),
                Title = "title",
            };
            object[] parameters =
            {
                OrderBy.Release_Date,
                movie,
            };

            string result = (string)method.Invoke(controller, parameters);

            Assert.That(result, Is.EqualTo(movie.Release_Date.Ticks.ToString()));
        }

        #endregion
    }
}