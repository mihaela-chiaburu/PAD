using Common.Models;
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
        private readonly ISyncService<Movie> _movieSyncService;

        public MovieController(IRepository<Movie> movieRepository, ISyncService<Movie> movieSyncService)
        {
            _movieRepository = movieRepository;
            _movieSyncService = movieSyncService;
        }

        // GET ALL
        [HttpGet]
        public List<Movie> GetAllMovies()
        {
            return _movieRepository.GetAllRecord();
        }

        // GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetMovieById(Guid id)
        {
            var movie = _movieRepository.GetRecordById(id);
            if (movie == null)
                return NotFound();

            return Ok(movie);
        }

        // CREATE
        [HttpPost]
        public IActionResult Create([FromBody] Movie movie)
        {
            movie.LastChangedAt = DateTime.UtcNow;

            _movieRepository.InsertRecord(movie);
            _movieSyncService.Upsert(movie);

            return Ok(movie);
        }

        // UPDATE / UPSERT (main node)
        [HttpPut]
        public IActionResult Upsert([FromBody] Movie movie)
        {
            movie.LastChangedAt = DateTime.UtcNow;

            _movieRepository.UpsertRecord(movie);
            _movieSyncService.Upsert(movie);

            return Ok(movie);
        }

        // UPSERT SYNC (called between nodes)
        [HttpPut("sync")]
        public IActionResult UpsertSync([FromBody] Movie movie)
        {
            var existingMovie = _movieRepository.GetRecordById(movie.Id);

            // Accept if it doesn't exist OR incoming data is newer
            if (existingMovie == null || movie.LastChangedAt > existingMovie.LastChangedAt)
            {
                _movieRepository.UpsertRecord(movie);
            }

            return Ok(movie);
        }

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var movie = _movieRepository.GetRecordById(id);
            if (movie == null)
                return NotFound();

            // set timestamp BEFORE sending sync
            movie.LastChangedAt = DateTime.UtcNow;

            _movieRepository.DeleteRecord(id);
            _movieSyncService.Delete(movie);

            return Ok($"Deleted {id}");
        }

        // DELETE SYNC (called between nodes)
        [HttpDelete("sync")]
        public IActionResult DeleteSync([FromBody] Movie movie)
        {
            var existingMovie = _movieRepository.GetRecordById(movie.Id);

            // DELETE only if exists AND incoming delete is newer
            if (existingMovie != null && movie.LastChangedAt > existingMovie.LastChangedAt)
            {
                _movieRepository.DeleteRecord(movie.Id);
            }

            return Ok(movie);
        }
    }
}
