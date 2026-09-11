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
    public partial class AlterarSenhaDialog : Form
    {
        public AlterarSenhaDialog()
        {
            InitializeComponent();
        }

        public string SenhaAtual => txtSenhaAtual.Text;
        public string NovaSenha => txtNovaSenha.Text;
        public string ConfirmarNovaSenha => txtConfirmarSenha.Text;

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSenhaAtual.Text))
            {
                BetaFitMessageBox.Aviso(this, "Informe sua senha atual.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNovaSenha.Text))
            {
                BetaFitMessageBox.Aviso(this, "Informe a nova senha.");
                return;
            }

            if (txtNovaSenha.Text != txtConfirmarSenha.Text)
            {
                BetaFitMessageBox.Aviso(this, "A nova senha e a confirmação não coincidem.");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void chkMostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            bool mostrar = chkMostrarSenha.Checked;

            txtSenhaAtual.UseSystemPasswordChar = !mostrar;
            txtNovaSenha.UseSystemPasswordChar = !mostrar;
            txtConfirmarSenha.UseSystemPasswordChar = !mostrar;
        }

        private void btnFecharSenha_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}