using FIAP.PosTech.ArqSistemas.PaymentsWS.Enums;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public string Usuario { get; set; }
        public int IdGame { get; set; }
        public string Game { get; set; }
        public decimal Preco { get; set; }
        public string EmailUser { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Rejected;
    }
}
