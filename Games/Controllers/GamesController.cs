using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using Gamess.Core.Entities;
using Gamess.Core.Interfaces;
using Gamess.Core.QueryFilters;
using Gamess.Core.CustomEntities;
using Games.Api.Responses;
using Gamess.Infraestructure.DTOs;

namespace Games.Api.Controllers
{

    // GAMES

    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _svc;
        private readonly IMapper _mapper;
        private readonly IGameDapperService _dapper;

        public GamesController(IGameService svc, IMapper mapper, IGameDapperService dapper)
        {
            _svc = svc;
            _mapper = mapper;
            _dapper = dapper;
        }

        /// <summary>
        /// Recupera todos los juegos con paginación y filtros.
        /// </summary>
        /// <remarks>Este endpoint obtiene una lista de juegos con base en los filtros proporcionados.</remarks>
        /// <param name="filters">Filtros para la búsqueda de juegos.</param>
        /// <param name="pagination">Parámetros de paginación.</param>
        /// <returns>Lista paginada de juegos.</returns>
        /// <response code="200">Retorna la lista de juegos.</response>
        /// <response code="400">Solicitud incorrecta.</response>
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

        /// <summary>
        /// Recupera todos los juegos.
        /// </summary>
        /// <remarks>Este endpoint obtiene una lista de todos los juegos disponibles.</remarks>
        /// <returns>Una lista de juegos.</returns>
        /// <response code="200">Retorna la lista de juegos.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var games = await _svc.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<GameDto>>(games);
            return Ok(dto);
        }

        /// <summary>
        /// Recupera un juego por su ID.
        /// </summary>
        /// <param name="id">ID del juego.</param>
        /// <returns>Un juego.</returns>
        /// <response code="200">Retorna el juego encontrado.</response>
        /// <response code="404">Si el juego no existe.</response>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var game = await _svc.GetByIdAsync(id);
            return game is null ? NotFound() : Ok(_mapper.Map<GameDto>(game));
        }

        /// <summary>
        /// Crea un nuevo juego.
        /// </summary>
        /// <param name="dto">Objeto del juego a crear.</param>
        /// <returns>Juego creado.</returns>
        /// <response code="201">Retorna el juego creado.</response>
        /// <response code="400">Si hay un error en la solicitud.</response>
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

        /// <summary>
        /// Actualiza un juego.
        /// </summary>
        /// <param name="id">ID del juego a actualizar.</param>
        /// <param name="dto">Datos del juego a actualizar.</param>
        /// <returns>Juego actualizado.</returns>
        /// <response code="200">Retorna el juego actualizado.</response>
        /// <response code="404">Si el juego no se encuentra.</response>
        /// <response code="400">Si los datos son incorrectos.</response>
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

        /// <summary>
        /// Elimina un juego.
        /// </summary>
        /// <param name="id">ID del juego a eliminar.</param>
        /// <response code="204">Juego eliminado correctamente.</response>
        /// <response code="404">Si el juego no se encuentra.</response>
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

        /// <summary>
        /// Recupera los juegos por género.
        /// </summary>
        /// <param name="genre">Género de los juegos.</param>
        /// <returns>Lista de juegos del género especificado.</returns>
        [HttpGet("by-genre/{genre}")]
        public async Task<IActionResult> GetByGenre(string genre)
        {
            var games = await _svc.GetByGenreAsync(genre);
            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }

        /// <summary>
        /// Recupera los juegos con mejores puntuaciones.
        /// </summary>
        /// <param name="take">Número de juegos a recuperar.</param>
        /// <returns>Lista de juegos con mejores puntuaciones.</returns>
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

        /// <summary>
        /// Recupera los juegos con peores puntuaciones.
        /// </summary>
        /// <param name="take">Número de juegos a recuperar.</param>
        /// <returns>Lista de juegos con peores puntuaciones.</returns>
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

        /// <summary>
        /// Busca juegos por título.
        /// </summary>
        /// <param name="title">Título del juego.</param>
        /// <returns>Lista de juegos que contienen el título proporcionado.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string title)
        {
            var games = await _svc.SearchByTitleAsync(title);
            if (!games.Any())
                return NotFound($"No se encontraron juegos con el título que contenga: '{title}'");
            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }

        /// <summary>
        /// Recupera juegos según la edad mínima recomendada.
        /// </summary>
        /// <param name="min">Edad mínima del jugador.</param>
        /// <param name="max">Edad máxima del jugador.</param>
        /// <param name="includeUnknown">Incluir juegos sin edad mínima definida.</param>
        /// <returns>Lista de juegos dentro del rango de edad especificado.</returns>
        [HttpGet("age")]
        public async Task<IActionResult> GetByAge([FromQuery] int? min, [FromQuery] int? max, [FromQuery] bool includeUnknown = false)
        {
            var games = await _svc.GetByAgeRangeAsync(min, max, includeUnknown);
            if (!games.Any()) return NotFound("No hay juegos en ese rango de edad.");
            return Ok(_mapper.Map<IEnumerable<GameDto>>(games));
        }

        /// <summary>Últimos juegos (Dapper)</summary>
        [HttpGet("dapper/latest")]
        public async Task<IActionResult> GetLatest([FromQuery] int take = 10)
        {
            var rows = await _dapper.GetLatestAsync(take);
            var dto = _mapper.Map<IEnumerable<GameDto>>(rows);
            return Ok(new ApiResponse<IEnumerable<GameDto>>(dto));
        }

        /// <summary>Búsqueda por título (Dapper)</summary>
        [HttpGet("dapper/search")]
        public async Task<IActionResult> SearchDapper([FromQuery] string title)
        {
            var rows = await _dapper.SearchAsync(title); // <-- corrected method name
            if (!rows.Any())
                return NotFound($"No se encontraron juegos con el título que contenga: '{title}'");
            var dto = _mapper.Map<IEnumerable<GameDto>>(rows);
            return Ok(new ApiResponse<IEnumerable<GameDto>>(dto));
        }

        /// <summary>Top mejores puntuados (Dapper)</summary>
        [HttpGet("dapper/top")]
        public async Task<IActionResult> TopDapper([FromQuery] int take = 5)
        {
            var rows = await _dapper.TopRatedAsync(take); // <-- cambiar nombre
            var result = rows.Select(t =>
            {
                var dto = _mapper.Map<GameDto>(t.Game);
                dto.AverageScore = Math.Round(t.AvgScore, 2);
                dto.ReviewsCount = t.ReviewsCount;
                return dto;
            });
            return Ok(new ApiResponse<IEnumerable<GameDto>>(result));
        }

        /// <summary>Peores puntuados (Dapper)</summary>
        [HttpGet("dapper/low")]
        public async Task<IActionResult> LowDapper([FromQuery] int take = 5)
        {
            var rows = await _dapper.LowRatedAsync(take);
            var result = rows.Select(t =>
            {
                var dto = _mapper.Map<GameDto>(t.Game);
                dto.AverageScore = Math.Round(t.AvgScore, 2);
                dto.ReviewsCount = t.ReviewsCount;
                return dto;
            });
            return Ok(new ApiResponse<IEnumerable<GameDto>>(result));
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

        /// <summary>
        /// Recupera todas las reseñas de un juego.
        /// </summary>
        /// <param name="gameId">ID del juego al que pertenecen las reseñas.</param>
        /// <returns>Lista de reseñas del juego.</returns>
        /// <response code="200">Retorna la lista de reseñas.</response>
        /// <response code="404">Si no se encuentran reseñas para el juego.</response>
        [HttpGet]
        public async Task<IActionResult> Get(int gameId)
        {
            var reviews = await _svc.GetByGameAsync(gameId);
            return Ok(_mapper.Map<IEnumerable<ReviewDto>>(reviews));
        }

        /// <summary>
        /// Recupera una reseña por su ID.
        /// </summary>
        /// <param name="gameId">ID del juego al que pertenece la reseña.</param>
        /// <param name="id">ID de la reseña.</param>
        /// <returns>Reseña encontrada.</returns>
        /// <response code="200">Retorna la reseña.</response>
        /// <response code="404">Si la reseña no existe.</response>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int gameId, int id)
        {
            var review = await _svc.GetByIdAsync(id);
            return review is null || review.GameId != gameId
                ? NotFound()
                : Ok(_mapper.Map<ReviewDto>(review));
        }

        /// <summary>
        /// Crea una nueva reseña para un juego.
        /// </summary>
        /// <param name="gameId">ID del juego al que pertenece la reseña.</param>
        /// <param name="dto">Objeto de la reseña a crear.</param>
        /// <returns>Reseña creada.</returns>
        /// <response code="201">Retorna la reseña creada.</response>
        /// <response code="400">Si hay un error en la solicitud.</response>
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

        /// <summary>
        /// Actualiza una reseña.
        /// </summary>
        /// <param name="gameId">ID del juego al que pertenece la reseña.</param>
        /// <param name="id">ID de la reseña a actualizar.</param>
        /// <param name="dto">Datos de la reseña a actualizar.</param>
        /// <returns>Reseña actualizada.</returns>
        /// <response code="200">Retorna la reseña actualizada.</response>
        /// <response code="404">Si la reseña no existe.</response>
        /// <response code="400">Si los datos son incorrectos.</response>
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

        /// <summary>
        /// Elimina una reseña.
        /// </summary>
        /// <param name="gameId">ID del juego al que pertenece la reseña.</param>
        /// <param name="id">ID de la reseña a eliminar.</param>
        /// <response code="204">Reseña eliminada correctamente.</response>
        /// <response code="404">Si la reseña no se encuentra.</response>
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

        /// <summary>Filtra las reseñas (con paginación y filtros)</summary>
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
