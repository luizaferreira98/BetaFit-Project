using BetaFit.Desktop.DTOs;

namespace BetaFit.Desktop.Helpers
{
    public sealed class SessionManager
    {
        // Instância única (lazy initialization)
        private static readonly Lazy<SessionManager> _instance =
            new(() => new SessionManager());

        ///<summary>
        ///Ponto de acesso global à instância única do SessionManager
        ///Uso: SessionManager.Instance.CurrentUser
        /// </summary>
        public static SessionManager Instance => _instance.Value;

        // Construtor privado: impede a criação de novas instâncias de fora
        private SessionManager() { }

        //===============================================
        // DADOS DA SESSÃO
        //===============================================

        ///<summary>
        ///Dados do usuário atualmente autenticado
        ///é null quando nenhum usuário está logado
        /// </summary>
        public UserResponseDto? CurrentUser { get; private set; }

        ///<summary>
        ///indica se tem algum usuário autenticado na sessão
        ///</summary>
        public bool IsAuthencticated => CurrentUser != null;

        ///<summary>
        ///indica se o usuário autenticado é um Administrador.
        ///usado para controlar acesso a módulos restritos
        ///</summary>
        public bool IsAdmin => CurrentUser?.IsAdmin ?? false;

        ///<summary>
        /// Define o usuário autenticado na sessão
        /// Chamado após o login bem-sucediso na API.
        /// <param name="user">Dados do usuário retornados pela API</param>
        /// </summary>
        public void SetUser(UserResponseDto user)
        {
            CurrentUser = user;
        }

        ///<summary>
        /// Limpa os dados da sessão (logout)
        /// Após este método, IsAutenticated retorna false
        ///</summary>   
        public void Clear()
        {
            CurrentUser = null;
        }

        ///<summary>
        ///Retorna o e-mail do usuário atual de forma segura
        ///Retorna string vazia se não houver usuário autenticado.
        /// </summary>
        public string GetEmail() => CurrentUser?.Email ?? string.Empty;

        ///<summary>
        ///Retorna o nome de exibição do usuário (parte antes do @).
        ///Exemplo: "luan.costa@senac.com" > luan.costa
        ///</summary>
        public string GetDisplayName()
        {
            var email = GetEmail();
            if (string.IsNullOrEmpty(email)) return "Usuário";

            // captura o que vem antes do @
            var at = email.IndexOf("@");
            // se houver ao menos 1 caractere antes do @ retorna o nome do usuário
            return at > 0 ? email[..at] : email; //
        }
    }
}
