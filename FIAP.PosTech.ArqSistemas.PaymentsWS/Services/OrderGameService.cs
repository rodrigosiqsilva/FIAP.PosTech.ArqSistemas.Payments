using FIAP.PosTech.ArqSistemas.PaymentsWS.DTOs;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Enums;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Models;
using System.Net;
using System.Net.Http.Json;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Services
{
    public class OrderGameService : IOrderGameService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private readonly string _orderEndpoint;

        public OrderGameService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _baseUrl = _configuration["ApiSettings:BaseUrl"];
            _orderEndpoint = _configuration["ApiSettings:OrderEndpoint"];
        }

        public async Task<(bool Sucesso, string Mensagem, Order Order)> AlterarStatusAsync(int id, OrderStatus newState)
        {
            // Alterar Status do Pedido 
            var orderUrl = $"{_baseUrl}{_orderEndpoint}{id}?newState={(int)newState}";

            // Utilizamos HttpRequestMessage para injetar o header de forma segura para esta requisição específica
            using var requestMessage = new HttpRequestMessage(HttpMethod.Put, orderUrl);

            var orderResponse = await _httpClient.SendAsync(requestMessage);

            // Verifica se o pedido não foi encontrado
            if (orderResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return (false, "Pedido não encontrado.", null);
            }

            // Garante que a requisição teve sucesso (2xx) antes de prosseguir
            orderResponse.EnsureSuccessStatusCode();

            // 1. Deserializa o JSON para o formato que a API responde (ApiResponse contendo OrderDto)
            var apiResponse = await orderResponse.Content.ReadFromJsonAsync<ApiResponse<OrderDto>>();

            if (apiResponse != null && apiResponse.Sucesso && apiResponse.Dados != null)
            {
                // 2. Mapeia o OrderDto/Dados vindo da API para o objeto de domínio Order esperado no retorno
                var order = new Order
                {
                    Id = apiResponse.Dados.Id,
                    IdUser = apiResponse.Dados.IdUser,
                    Usuario = apiResponse.Dados.Usuario,
                    IdGame = apiResponse.Dados.IdGame,
                    Game = apiResponse.Dados.Game,
                    Preco = apiResponse.Dados.Preco,
                    EmailUser = apiResponse.Dados.EmailUser,
                    Status = apiResponse.Dados.Status
                };

                return (true, apiResponse.Mensagem ?? "Status do pedido alterado com sucesso.", order);
            }

            // Caso a API responda 200 OK mas com flag de sucesso falso ou corpo vazio
            var erroMensagem = apiResponse?.Mensagem ?? "Erro desconhecido ao alterar status do pedido.";
            return (false, erroMensagem, null);
        }

        public async Task<ApiResponse<OrderDto>> GetOrderAsync(int orderId)
        {
            // Busca o Pedido 
            var orderUrl = $"{_baseUrl}{_orderEndpoint}{orderId}";

            // Utilizamos HttpRequestMessage para injetar o header de forma segura para esta requisição específica
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, orderUrl);

            var orderResponse = await _httpClient.SendAsync(requestMessage);

            // Verifica se o pedido não foi encontrado
            if (orderResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            orderResponse.EnsureSuccessStatusCode();

            // Retorna o JSON do pedido como string
            // return await orderResponse.Content.ReadAsStringAsync();
            return await orderResponse.Content.ReadFromJsonAsync<ApiResponse<OrderDto>>();
        }
    }
}
