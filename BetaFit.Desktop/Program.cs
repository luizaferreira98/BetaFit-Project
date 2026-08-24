using BetaFit.Desktop.Forms;

namespace BetaFit.Desktop
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Mostra o login primeiro. Só abre a tela principal (Form1)
            // se o usuário conseguir autenticar (DialogResult.OK).
            //using var loginForm = new LoginForm();
            //if (loginForm.ShowDialog() == DialogResult.OK)
            //{
            //    System.Windows.Forms.Application.Run(new Form1());
            //}
            //System.Windows.Forms.Application.Run(new Form1());
        }
    }
}