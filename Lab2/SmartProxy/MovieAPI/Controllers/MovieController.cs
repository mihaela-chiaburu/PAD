using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Repositories;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IRepository<Movie> _movieRepository;
        public MovieController(IRepository<Movie> movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpGet]
        public List<Movie> GetAllMovies()
        {
            var records = _movieRepository.GetAllRecord();

            return records;
        }

        [HttpGet("{id}")]
        public IActionResult GetMovieById(Guid id)
        {
            var movie = _movieRepository.GetRecordById(id);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }

        [HttpPost]
        public IActionResult Create(Movie movie)
        {
            movie.LastChangedAt = DateTime.UtcNow;
            _movieRepository.InsertRecord(movie);
            return Ok(movie);
        }

        [HttpPut]
        public IActionResult Upsert(Movie movie)
        {
            movie.LastChangedAt = DateTime.UtcNow;
            _movieRepository.UpsertRecord(movie);
            return Ok(movie);
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var movie = _movieRepository.GetRecordById(id);
            if (movie == null)
            {
                return NotFound();
            }
            _movieRepository.DeleteRecord(id);
            return Ok("Deteletd " + id);
        }
    }
}
