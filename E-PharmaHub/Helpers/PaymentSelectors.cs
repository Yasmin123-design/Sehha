using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Helpers
{
    public static class PaymentSelectors
    {
        public static Expression<Func<Payment, AppUser, PaymentReadDto>> ToPaymentResponse
            => (p, u) => new PaymentReadDto
            {
                Id = p.Id,
                ReferenceId = p.ReferenceId,
                PaymentFor = p.PaymentFor,
                PaymentForName = p.PaymentFor.ToString(),
                Status = p.Status,
                Amount = p.Amount,
                ProcessedAt = p.ProcessedAt,

                PayerUserId = u.Id,
                PayerName = u.UserName,
                PayerEmail = u.Email
            };
    }

}
