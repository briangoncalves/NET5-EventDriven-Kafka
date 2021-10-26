using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventDrivenWebAPIExample.Domain;
using EventDrivenWebAPIExample.Domain.Mongo;
using EventDrivenWebAPIExample.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EventDrivenWebAPIExample.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> _logger;
        private readonly IMovieService _service;

        public MovieController(ILogger<MovieController> logger, IMovieService service)
        {
            this._logger = logger;
            this._service = service;
        }

        [HttpGet("/movie/search/{title}")]
        [ProducesResponseType(typeof(Movie), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetByTitle(string title)
        {
            _logger.LogInformation($"Retrieving movie: {title}");
            var movieReturn = await _service.FindMovieByTitleAsync(title);

            if (movieReturn is null)
            {
                return NotFound(new { message = "The given movie title doesn't match a valid resource !" });
            }
            else
            {
                return Ok(movieReturn);
            }
        }

        [HttpPost("/movie/add")]
        [ProducesResponseType(typeof(List<KafkaReturnValue>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult AddMovie(Movie movie)
        {
            _logger.LogInformation($"Sending movie: {movie.Title}");
            var movieReturn = _service.SendMovie(movie);

            if (movieReturn is null)
            {
                return NotFound(new { message = "The given movie title doesn't match a valid resource !" });
            }
            else
            {
                return Ok(movieReturn);
            }
        }
    }
}
