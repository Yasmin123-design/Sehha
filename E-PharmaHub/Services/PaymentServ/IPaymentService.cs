using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using Stripe;
using Stripe.Checkout;

namespace E_PharmaHub.Services.PaymentServ
{
    public interface IPaymentService
    {
        Task<List<PaymentReadDto>> GetDoctorRegistrationPaymentsAsync();
        Task<List<PaymentReadDto>> GetPharmacistRegistrationPaymentsAsync();
        Task<List<PaymentReadDto>> GetOrderPaymentsAsync();
        Task<List<PaymentReadDto>> GetAppointmentPaymentsAsync();
        Task HandleCheckoutSessionCompletedAsync(Session session);
        Task HandlePaymentCanceledAsync(PaymentIntent paymentIntent);
        Task DeletePaymentAsync(Payment model);
        Task<object> VerifySessionAsync(string sessionId);
        Task<Payment> GetByReferenceIdAsync(string referenceId);
    }
}
