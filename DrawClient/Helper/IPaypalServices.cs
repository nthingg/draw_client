using PayPal.Api;

namespace DrawClient.Helper
{
    public interface IPaypalServices
    {
        Task<Payment> CreateOrderAsync(decimal amount, string returnUrl, string cancelUrl);
    }
}
