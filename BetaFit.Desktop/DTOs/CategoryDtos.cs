namespace BetaFit.Desktop.DTOs
{
    /// <summary>
    /// DTO para representar uma Categoria retornada da API.
    /// </summary>
    public class CategoriaResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GameCount { get; set; }
    }

    /// <summary>
    /// DTO para criar uma nova Categoria.
    ///</summary>
    public class CreateCategoriaDto
    {
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para atualizar uma Categoria existente.
    ///</summary>
    public class UpdateCategoriaDto
    {
        public string Name { get; set; } = string.Empty;
    }


}
