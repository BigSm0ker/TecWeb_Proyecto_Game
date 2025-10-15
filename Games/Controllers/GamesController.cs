
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Gamess.Core.Interfaces;
using Gamess.Core.Entities;
using Gamess.Infraestructure.DTOs;

namespace Games.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameRepository _repo;
        private readonly IMapper _mapper;
        public GamesController(IGameRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var games = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var game = await _repo.GetByIdAsync(id);
            return game is null ? NotFound() : Ok(_mapper.Map<GameDto>(game));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GameDto dto)
        {
            var entity = _mapper.Map<Game>(dto);
            await _repo.InsertAsync(entity);
            var result = _mapper.Map<GameDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] GameDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(existing);
            return Ok(_mapper.Map<GameDto>(existing));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            await _repo.DeleteAsync(existing);
            return NoContent();
        }
        // GET /api/games/by-genre/{genre}
        [HttpGet("by-genre/{genre}")]
        public async Task<IActionResult> GetByGenre(string genre)
        {
            var games = await _repo.GetByGenreAsync(genre);
            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }

        // GET /api/games/top-rated?take=10
        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRated([FromQuery] int take = 10)
        {
            var top = await _repo.GetTopRatedAsync(take);

            
            var result = top.Select(t =>
            {
                var dto = _mapper.Map<GameDto>(t.Game);
                dto.AverageScore = Math.Round(t.AvgScore, 2);
                dto.ReviewsCount = t.ReviewsCount;
                return dto;
            });

            return Ok(result);
        }

        // GET /api/games/low-rated
        [HttpGet("low-rated")]
        public async Task<IActionResult> GetLowRated([FromQuery] int take = 10)
        {
            var bottom = await _repo.GetLowRatedAsync(take);  // nuevo método
            var result = bottom.Select(t =>
            {
                var dto = _mapper.Map<GameDto>(t.Game);
                dto.AverageScore = Math.Round(t.AvgScore, 2);
                dto.ReviewsCount = t.ReviewsCount;
                return dto;
            });
            return Ok(result);
        }
        // GET /api/games/search?title=valor
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string title)
        {
            var games = await _repo.SearchByTitleAsync(title);
            if (!games.Any())
                return NotFound($"No se encontraron juegos con el título que contenga: '{title}'");

            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }

        // GET /api/games/age?min=7&max=13&includeUnknown=false
        [HttpGet("age")]
        public async Task<IActionResult> GetByAge([FromQuery] int? min, [FromQuery] int? max, [FromQuery] bool includeUnknown = false)
        {
            var games = await _repo.GetByAgeRangeAsync(min, max, includeUnknown);
           
            if (!games.Any()) return NotFound("No hay juegos en ese rango de edad.");
            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }



    }
    [ApiController]
    [Route("api/games/{gameId:int}/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _repo;
        private readonly IGameRepository _games;
        private readonly IMapper _mapper;

        public ReviewsController(IReviewRepository repo, IGameRepository games, IMapper mapper)
        {
            _repo = repo; _games = games; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int gameId)
        {
            var game = await _games.GetByIdAsync(gameId);
            if (game is null) return NotFound("Game not found");
            var reviews = await _repo.GetByGameAsync(gameId);
            return Ok(_mapper.Map<IEnumerable<ReviewDto>>(reviews));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int gameId, int id)
        {
            var review = await _repo.GetByIdAsync(id);
            return review is null || review.GameId != gameId
                ? NotFound()
                : Ok(_mapper.Map<ReviewDto>(review));
        }

        [HttpPost]
        public async Task<IActionResult> Post(int gameId, [FromBody] ReviewDto dto)
        {
            var game = await _games.GetByIdAsync(gameId);
            if (game is null) return NotFound("Game not found");

            dto.GameId = gameId;
            dto.CreatedAt = DateTime.UtcNow;

            var entity = _mapper.Map<Review>(dto);
            await _repo.InsertAsync(entity);

            var result = _mapper.Map<ReviewDto>(entity);
            return CreatedAtAction(nameof(GetById),
                new { gameId, id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int gameId, int id, [FromBody] ReviewDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");
            var review = await _repo.GetByIdAsync(id);
            if (review is null || review.GameId != gameId) return NotFound();

            _mapper.Map(dto, review);
            await _repo.UpdateAsync(review);
            return Ok(_mapper.Map<ReviewDto>(review));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int gameId, int id)
        {
            var review = await _repo.GetByIdAsync(id);
            if (review is null || review.GameId != gameId) return NotFound();

            await _repo.DeleteAsync(review);
            return NoContent();
        }
        
        [HttpGet("~/api/reviews")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ReviewDto>>(list));
        }

        
        [HttpGet("~/api/reviews/{id:int}")]
        public async Task<IActionResult> GetAnyById(int id)
        {
            var review = await _repo.GetByIdAsync(id);
            return review is null ? NotFound() : Ok(_mapper.Map<ReviewDto>(review));
        }

    }
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository repo, IMapper mapper)
        {
            _repo = repo; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto dto)
        {
            if (await _repo.EmailExistsAsync(dto.Email))
                return BadRequest("Email ya está registrado.");

            var entity = _mapper.Map<User>(dto);
            await _repo.InsertAsync(entity);

            var result = _mapper.Map<UserDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");

            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            if (await _repo.EmailExistsAsync(dto.Email, excludeId: id))
                return BadRequest("Email ya está registrado por otro usuario.");

            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(existing);
            return Ok(_mapper.Map<UserDto>(existing));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            await _repo.DeleteAsync(existing);
            return NoContent();
        }
    }

}
