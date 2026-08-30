namespace BetaFit.Desktop.DTOs
{
    /// <summary>
    /// DTO para representar uma Categoria retornada da API.
    /// </summary>
    public class CategoriaResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        /// <summary>
        /// Quantidade de produtos nesta categoria.
        /// Útil para mostrar no dashboard e na listagem.
        /// </summary>
        public int ProductCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO para criar uma nova Categoria.
    ///</summary>
    public class CreateCategoriaDto
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO para atualizar uma Categoria existente.
    ///</summary>
    public class UpdateCategoriaDto
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }


}
