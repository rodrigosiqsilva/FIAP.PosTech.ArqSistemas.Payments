using FIAP.PosTech.ArqSistemas.PaymentsWS.DTOs;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Enums;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Models;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Services
{
    public interface IOrderGameService
    {
        Task<ApiResponse<OrderDto>> GetOrderAsync(int orderId);

        Task<(bool Sucesso, string Mensagem, Order Order)> AlterarStatusAsync(int id, OrderStatus newState);
    }
}
