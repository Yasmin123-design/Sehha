using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Dtos
{
    public class PaymentReadDto
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public PaymentForType PaymentFor { get; set; }
        public string PaymentForName { get; set; }

        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime ProcessedAt { get; set; }

        // Payer
        public string PayerUserId { get; set; }
        public string PayerName { get; set; }
        public string PayerEmail { get; set; }
    }

}
