using Microsoft.AspNetCore.Mvc;
using PruebaInfodesign.Application.DTOs.Request;
using PruebaInfodesign.Application.Interfaces;

namespace PruebaInfodesign.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumosController : ControllerBase
    {
        private readonly IConsumoService _consumoService;
        private readonly ILogger<ConsumosController> _logger;

        public ConsumosController(IConsumoService consumoService, ILogger<ConsumosController> logger)
        {
            _consumoService = consumoService;
            _logger = logger;
        }

        [HttpGet("historico-por-tramos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerHistoricoPorTramos(
            [FromQuery] DateTime fechaInicial,
            [FromQuery] DateTime fechaFinal)
        {
            try
            {
                var request = new FechaRangoRequest
                {
                    FechaInicial = fechaInicial,
                    FechaFinal = fechaFinal
                };

                var resultado = await _consumoService.ObtenerHistoricoConsumosPorTramos(request);

                return Ok(new
                {
                    success = true,
                    message = "Historico obtenido exitosamente",
                    data = resultado,
                    total = resultado.Count
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida en historico-por-tramos");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historico-por-tramos");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }

        [HttpGet("historico-por-cliente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerHistoricoPorCliente(
            [FromQuery] DateTime fechaInicial,
            [FromQuery] DateTime fechaFinal)
        {
            try
            {
                var request = new FechaRangoRequest
                {
                    FechaInicial = fechaInicial,
                    FechaFinal = fechaFinal
                };

                var resultado = await _consumoService.ObtenerHistoricoConsumosPorCliente(request);

                return Ok(new
                {
                    success = true,
                    message = "Historico obtenido exitosamente",
                    data = resultado,
                    total = resultado.Count
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida en historico-por-cliente");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historico-por-cliente");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }

        [HttpGet("top20-peores-tramos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerTop20PeoresTramos(
            [FromQuery] DateTime fechaInicial,
            [FromQuery] DateTime fechaFinal)
        {
            try
            {
                var request = new FechaRangoRequest
                {
                    FechaInicial = fechaInicial,
                    FechaFinal = fechaFinal
                };

                var resultado = await _consumoService.ObtenerTop20PeoresTramos(request);

                return Ok(new
                {
                    success = true,
                    message = "Top 20 obtenido exitosamente",
                    data = resultado,
                    total = resultado.Count
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida en top20-peores-tramos");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener top20-peores-tramos");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }
    }
}
