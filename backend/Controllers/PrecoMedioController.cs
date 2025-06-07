using Microsoft.AspNetCore.Mvc;
using InvestApp.Services;
using InvestApp.Services.DTOs;

namespace InvestApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrecoMedioController : ControllerBase
    {
        private readonly IPrecoMedioService _precoMedioService;

        public PrecoMedioController(IPrecoMedioService precoMedioService)
        {
            _precoMedioService = precoMedioService;
        }

        /// <summary>
        /// Calcula o preço médio ponderado das operações de compra
        /// </summary>
        /// <param name="operacoes">Lista de operações de compra</param>
        /// <returns>Preço médio ponderado</returns>
        /// <response code="200">Retorna o preço médio calculado</response>
        /// <response code="400">Se a lista de operações for inválida</response>
        [HttpPost("calcular")]
        public ActionResult<decimal> CalcularPrecoMedio([FromBody] List<OperacaoDTO> operacoes)
        {
            try
            {
                var precoMedio = _precoMedioService.CalcularPrecoMedioPonderado(operacoes);
                return Ok(precoMedio);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 