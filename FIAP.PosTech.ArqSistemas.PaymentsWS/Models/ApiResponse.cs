
namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Models
{
    public class ApiResponse<T>
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public T? Dados { get; set; }
        public List<string> ListaErros { get; set; }
        public string? CorrelationId { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiResponse()
        {
            Timestamp = DateTime.UtcNow;
            ListaErros = new List<string>();
        }

        public static ApiResponse<T> SucessoCreate(T dados, string mensagem = "Recurso criado com sucesso")
        {
            return new ApiResponse<T>
            {
                Sucesso = true,
                Mensagem = mensagem,
                Dados = dados
            };
        }

        public static ApiResponse<T> SucessoOk(T dados, string mensagem = "Operação realizada com sucesso")
        {
            return new ApiResponse<T>
            {
                Sucesso = true,
                Mensagem = mensagem,
                Dados = dados
            };
        }

        public static ApiResponse<T> SucessoList(T dados, string mensagem = "Dados recuperados com sucesso")
        {
            return new ApiResponse<T>
            {
                Sucesso = true,
                Mensagem = mensagem,
                Dados = dados
            };
        }

        public static ApiResponse<T> Erro(string erro, string mensagem = "Erro ao processar requisição")
        {
            return new ApiResponse<T>
            {
                Sucesso = false,
                Mensagem = mensagem,
                ListaErros = new List<string> { erro }
            };
        }

        public static ApiResponse<T> Erros(List<string> erros, string mensagem = "Erro ao processar requisição")
        {
            return new ApiResponse<T>
            {
                Sucesso = false,
                Mensagem = mensagem,
                ListaErros = erros
            };
        }

        public static ApiResponse<T> NotFound(string mensagem = "Recurso não encontrado")
        {
            return new ApiResponse<T>
            {
                Sucesso = false,
                Mensagem = mensagem,
                Dados = default
            };
        }
    }
}
