// =============================================================================
// BetaFit.API - UsuariosController
// =============================================================================
// Controller REST para gestão de usuários (Identity), usado pelas telas
// administrativas de Website e Desktop.
//
// Endpoints:
// GET    /api/usuarios       Lista todos os usuários
// POST   /api/usuarios       Cria um novo usuário
// PUT    /api/usuarios/{id}  Atualiza um usuário existente
// DELETE /api/usuarios/{id}  Remove um usuário
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;

namespace BetaFit.API.Controllers
{
    [ApiController]            // Define que esta classe responde a requisições HTTP (JSON)
    [Route("api/[controller]")] // A rota será: localhost:porta/api/usuarios
    [Authorize(Roles = "Admin")] // Somente Admin gerencia outros usuários/funcionários
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosService _usuariosService;

        public UsuariosController(IUsuariosService usuariosService)
        {
            _usuariosService = usuariosService;
        }

        [HttpGet] // GET /api/usuarios
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuariosService.GetAllAsync();
            return Ok(usuarios); // Retorna HTTP 200 com a lista em JSON
        }

        [HttpPost] // POST /api/usuarios
        public async Task<IActionResult> Create([FromBody] CreateUsuarioDto dto)
        {
            var (success, usuario, error) = await _usuariosService.CreateAsync(dto);
            if (!success)
                return BadRequest(new { message = error }); // HTTP 400
            return Ok(usuario); // HTTP 200
        }

        [HttpDelete("{id}")] // DELETE /api/usuarios/{id}
        public async Task<IActionResult> Delete(string id)
        {
            var (success, error) = await _usuariosService.DeleteAsync(id);
            if (!success)
                return BadRequest(new { message = error }); // HTTP 400
            return NoContent(); // HTTP 204
        }

        [HttpPut("{id}")] // PUT /api/usuarios/{id}
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUsuarioDto dto)
        {
            var (success, usuario, error) = await _usuariosService.UpdateAsync(id, dto);
            if (!success)
                return BadRequest(new { message = error }); // HTTP 400
            return Ok(usuario); // HTTP 200
        }
    }
}