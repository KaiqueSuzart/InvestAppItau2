using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InvestApp.Data;
using InvestApp.Models;
using System.Linq;

namespace InvestApp.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IRepository _repository;

        public AuthController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Senha))
                return BadRequest("Email e senha são obrigatórios.");

            var usuario = await _repository.GetUsuarioPorEmailAsync(request.Email);
            if (usuario == null || usuario.Senha != request.Senha)
                return Unauthorized("Usuário ou senha inválidos.");

            return Ok(new { usuario.Id });
        }

        [HttpGet("usuarios")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _repository.ListarUsuariosAsync();
            return Ok(usuarios.Select(u => new { u.Id, u.Nome }));
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
} 