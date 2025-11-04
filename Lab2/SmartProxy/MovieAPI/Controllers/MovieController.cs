using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Repositories;
using MovieAPI.Services;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly ISyncService<Movie> _moviesyncService;
        public MovieController(IRepository<Movie> movieRepository, ISyncService<Movie> movieSyncService)
        {
            _movieRepository = movieRepository;
            _moviesyncService = movieSyncService;
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

            _moviesyncService.Upsert(movie);
            return Ok(movie);
        }

        [HttpPut]
        public IActionResult Upsert(Movie movie)
        {
            movie.LastChangedAt = DateTime.UtcNow;
            _movieRepository.UpsertRecord(movie);
            _moviesyncService.Upsert(movie);
            return Ok(movie);
        }

        [HttpPut("sync")]
        public IActionResult UpsertSync(Movie movie)
        {
            var existingMovie = _movieRepository.GetRecordById(movie.Id);
            if (existingMovie == null || movie.LastChangedAt > existingMovie.LastChangedAt)
            {
                movie.LastChangedAt = DateTime.UtcNow;
                _movieRepository.UpsertRecord(movie);
            }
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
            movie.LastChangedAt = DateTime.UtcNow;
            _moviesyncService.Delete(movie);
            return Ok("Deteletd " + id);
        }

        [HttpDelete("sync")]
        public IActionResult DeleteSync(Movie movie)
        {
            var existingMovie = _movieRepository.GetRecordById(movie.Id);
            if (existingMovie != null || movie.LastChangedAt > existingMovie.LastChangedAt)
            {
                _movieRepository.DeleteRecord(movie.Id);
            }
            return Ok(movie);
        }
    }
}
