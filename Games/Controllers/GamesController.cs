using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using Gamess.Core.Entities;
using Gamess.Core.Interfaces;
using Gamess.Core.QueryFilters;       // <-- filtros (Game/User/Review + Pagination)
using Gamess.Core.CustomEntities;     // <-- PagedList
using Games.Api.Responses;            // <-- ApiResponse
using Gamess.Infraestructure.DTOs;

namespace Games.Api.Controllers
{
    // =========================================================
    // GAMES
    // =========================================================
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _svc;
        private readonly IMapper _mapper;

        public GamesController(IGameService svc, IMapper mapper)
        {
            _svc = svc;
            _mapper = mapper;
        }

        // GET: /api/games/filter?title=...&genre=...&pageNumber=1&pageSize=10
        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] GameQueryFilter filters, [FromQuery] PaginationQueryFilter pagination)
        {
            var paged = await _svc.GetAllAsync(filters, pagination);
            var dto = _mapper.Map<IEnumerable<GameDto>>(paged);
            var response = new ApiResponse<IEnumerable<GameDto>>(dto)
            {
                Pagination = paged.Pagination
            };
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var games = await _svc.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<GameDto>>(games);
            return Ok(dto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var game = await _svc.GetByIdAsync(id);
            return game is null ? NotFound() : Ok(_mapper.Map<GameDto>(game));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GameDto dto)
        {
            try
            {
                var entity = _mapper.Map<Game>(dto);
                var created = await _svc.CreateAsync(entity);
                var result = _mapper.Map<GameDto>(created);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] GameDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");
            try
            {
                var existing = await _svc.GetByIdAsync(id);
                if (existing is null) return NotFound();
                _mapper.Map(dto, existing);
                var updated = await _svc.UpdateAsync(existing);
                return Ok(_mapper.Map<GameDto>(updated));
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _svc.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpGet("by-genre/{genre}")]
        public async Task<IActionResult> GetByGenre(string genre)
        {
            var games = await _svc.GetByGenreAsync(genre);
            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRated([FromQuery] int take = 10)
        {
            var top = await _svc.GetTopRatedAsync(take);
            var result = top.Select(t =>
            {
                var dto = _mapper.Map<GameDto>(t.Game);
                dto.AverageScore = Math.Round(t.AvgScore, 2);
                dto.ReviewsCount = t.ReviewsCount;
                return dto;
            });
            return Ok(result);
        }

        [HttpGet("low-rated")]
        public async Task<IActionResult> GetLowRated([FromQuery] int take = 10)
        {
            var bottom = await _svc.GetLowRatedAsync(take);
            var result = bottom.Select(t =>
            {
                var dto = _mapper.Map<GameDto>(t.Game);
                dto.AverageScore = Math.Round(t.AvgScore, 2);
                dto.ReviewsCount = t.ReviewsCount;
                return dto;
            });
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string title)
        {
            var games = await _svc.SearchByTitleAsync(title);
            if (!games.Any())
                return NotFound($"No se encontraron juegos con el título que contenga: '{title}'");
            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }

        [HttpGet("age")]
        public async Task<IActionResult> GetByAge([FromQuery] int? min, [FromQuery] int? max, [FromQuery] bool includeUnknown = false)
        {
            var games = await _svc.GetByAgeRangeAsync(min, max, includeUnknown);
            if (!games.Any()) return NotFound("No hay juegos en ese rango de edad.");
            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }
    }

    // =========================================================
    // USERS
    // =========================================================
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _svc;
        private readonly IMapper _mapper;

        public UsersController(IUserService svc, IMapper mapper)
        {
            _svc = svc;
            _mapper = mapper;
        }

        // GET: /api/users/filter?name=...&email=...&isActive=true&pageNumber=1&pageSize=10
        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] UserQueryFilter filters, [FromQuery] PaginationQueryFilter pagination)
        {
            var paged = await _svc.GetAllAsync(filters, pagination);
            var dto = _mapper.Map<IEnumerable<UserDto>>(paged);
            var response = new ApiResponse<IEnumerable<UserDto>>(dto)
            {
                Pagination = paged.Pagination
            };
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _svc.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _svc.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto dto)
        {
            try
            {
                var entity = _mapper.Map<User>(dto);
                var created = await _svc.CreateAsync(entity);
                var result = _mapper.Map<UserDto>(created);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");
            try
            {
                var existing = await _svc.GetByIdAsync(id);
                if (existing is null) return NotFound();
                _mapper.Map(dto, existing);
                var updated = await _svc.UpdateAsync(existing);
                return Ok(_mapper.Map<UserDto>(updated));
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _svc.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }
    }

    // =========================================================
    // REVIEWS
    // =========================================================
    [Route("api/games/{gameId:int}/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _svc;
        private readonly IMapper _mapper;

        public ReviewsController(IReviewService svc, IMapper mapper)
        {
            _svc = svc;
            _mapper = mapper;
        }

        // GET: /api/games/{gameId}/reviews
        [HttpGet]
        public async Task<IActionResult> Get(int gameId)
        {
            var reviews = await _svc.GetByGameAsync(gameId);
            return Ok(_mapper.Map<IEnumerable<ReviewDto>>(reviews));
        }

        // GET: /api/games/{gameId}/reviews/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int gameId, int id)
        {
            var review = await _svc.GetByIdAsync(id);
            return review is null || review.GameId != gameId
                ? NotFound()
                : Ok(_mapper.Map<ReviewDto>(review));
        }

        [HttpPost]
        public async Task<IActionResult> Post(int gameId, [FromBody] ReviewDto dto)
        {
            dto.GameId = gameId;
            dto.CreatedAt = DateTime.UtcNow;
            try
            {
                var entity = _mapper.Map<Review>(dto);
                var created = await _svc.CreateAsync(entity);
                var result = _mapper.Map<ReviewDto>(created);
                return CreatedAtAction(nameof(GetById), new { gameId, id = result.Id }, result);
            }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int gameId, int id, [FromBody] ReviewDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");
            var review = await _svc.GetByIdAsync(id);
            if (review is null || review.GameId != gameId) return NotFound();

            _mapper.Map(dto, review);
            try
            {
                var updated = await _svc.UpdateAsync(review);
                return Ok(_mapper.Map<ReviewDto>(updated));
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int gameId, int id)
        {
            var review = await _svc.GetByIdAsync(id);
            if (review is null || review.GameId != gameId) return NotFound();
            try
            {
                await _svc.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // ---------- FILTRO GLOBAL DE REVIEWS (ruta absoluta) ----------
        // GET: /api/reviews/filter?gameId=1&userId=...&minScore=...&pageNumber=1&pageSize=10
        [HttpGet("~/api/reviews/filter")]
        public async Task<IActionResult> FilterAll([FromQuery] ReviewQueryFilter filters, [FromQuery] PaginationQueryFilter pagination)
        {
            var paged = await _svc.GetAllAsync(filters, pagination);
            var dto = _mapper.Map<IEnumerable<ReviewDto>>(paged);
            var response = new ApiResponse<IEnumerable<ReviewDto>>(dto)
            {
                Pagination = paged.Pagination
            };
            return Ok(response);
        }
    }
}
