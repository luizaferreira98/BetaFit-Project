using BetaFit.Desktop.Forms;
namespace BetaFit.Desktop;
internal static class Program { [STAThread] static void Main(){ ApplicationConfiguration.Initialize(); System.Windows.Forms.Application.Run(new LoginForm()); } }
