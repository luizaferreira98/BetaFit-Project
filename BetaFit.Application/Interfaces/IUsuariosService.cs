using BetaFit.Application.DTOs;


namespace BetaFit.Application.Interfaces
{
    public interface IUsuariosService
    {
        Task<IEnumerable<UsuarioDto>> GetAllAsync();

        Task<UsuarioDto?> GetByIdAsync(string id);

        Task<(bool Success, UsuarioDto? Usuario, string ErrorMessage)> CreateAsync(CreateUsuarioDto dto);

        Task<(bool Success, UsuarioDto? Usuario, string ErrorMessage)> UpdateAsync(string id, UpdateUsuarioDto dto);

        Task<(bool Success, string ErrorMessage)> DeleteAsync(string id);

        Task<IEnumerable<string>> GetPerfisAsync();
    }
}