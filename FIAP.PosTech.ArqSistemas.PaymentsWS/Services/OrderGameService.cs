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

        public async Task<ApiResponse<OrderDto>> AlterarStatusAsync(OrderDto order, OrderStatus newState)
        {
            // Alterar Status do Pedido 
            var orderUrl = $"{_baseUrl}{_orderEndpoint}{order.Id}?newState={(int)newState}";

            // Utilizamos HttpRequestMessage para injetar o header de forma segura para esta requisição específica
            using var requestMessage = new HttpRequestMessage(HttpMethod.Put, orderUrl);

            var orderResponse = await _httpClient.SendAsync(requestMessage);

            // Verifica se o pedido não foi encontrado
            if (orderResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return ApiResponse<OrderDto>.NotFound("Pedido não encontrado.");
            }

            // Garante que a requisição teve sucesso (2xx) antes de prosseguir
            orderResponse.EnsureSuccessStatusCode();

            var apiResponse = await orderResponse.Content.ReadFromJsonAsync<ApiResponse<Order>>();

            if (apiResponse != null && apiResponse.Sucesso && apiResponse.Dados != null)
            {
                 return ApiResponse<OrderDto>.SucessoOk(order, apiResponse.Mensagem ?? "Status do pedido alterado com sucesso.");
            }

            // Caso a API responda 200 OK mas com flag de sucesso falso ou corpo vazio
            var erroMensagem = apiResponse?.Mensagem ?? "Erro desconhecido ao alterar status do pedido.";

            // Se a API original mandou uma lista de erros, podemos repassá-la, senão usamos o erro único
            if (apiResponse?.ListaErros?.Count > 0)
            {
                return ApiResponse<OrderDto>.Erros(apiResponse.ListaErros, erroMensagem);
            }

            return ApiResponse<OrderDto>.Erro(erroMensagem, erroMensagem);
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
