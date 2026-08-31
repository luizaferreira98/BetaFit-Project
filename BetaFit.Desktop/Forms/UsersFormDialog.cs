using BetaFit.Application.DTOs;
using BetaFit.Desktop.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.Forms
{
    /// <summary>
    /// Formulário de criação/edição de usuarios.
    /// Retorna CreateUsersDto (novo) ou UpdateUsersDto (edição).
    /// </summary>
    public partial class UsersFormDialog : Form
    {
        // =====================================================================
        // PROPRIEDADES DE SAÍDA
        // =====================================================================
        /// <summary>DTO preenchido quando no modo de criação (OK)</summary>
        public CreateUsuarioDto? GameDto { get; private set; }

        /// <summary>DTO preenchido quando no modo de edição (OK)</summary>
        public UpdateUsuarioDto? UpdateDto { get; private set; }
        
        // =====================================================================
        // CAMPOS PRIVADOS
        // =====================================================================
        private List<CategoriaResponseDto> _categorias = new();
        private GameResponseDto? _gameExistente;

        public UsersFormDialog()
        {
            InitializeComponent();
        }
    }
}
