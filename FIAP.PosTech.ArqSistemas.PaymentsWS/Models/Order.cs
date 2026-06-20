
using FIAP.PosTech.ArqSistemas.PaymentsWS.Enums;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public decimal Price { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Rejected;
    }
}
