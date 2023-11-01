using PayPal.Api;

namespace DrawClient.Helper
{
    public class PaypalServices : IPaypalServices
    {
        private readonly APIContext apiContext;
        private readonly Payment payment;
        private readonly IConfiguration configuration;

        public PaypalServices(IConfiguration configuration)
        {
            try
            {
                // Your PayPal initialization code
                this.configuration = configuration;

                var clientId = configuration["PayPal:ClientId"];
                var clientSecret = configuration["PayPal:ClientSecret"];

                var config = new Dictionary<string, string>
            {
                { "mode", "sandbox"},
                { "clientId", clientId},
                { "clientSecret", clientSecret },
            };

                OAuthTokenCredential tokenCredential = new OAuthTokenCredential(clientId, clientSecret, config);

                apiContext = new APIContext(tokenCredential.GetAccessToken());

                payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                };
            }
            catch (TypeInitializationException ex)
            {
                // Get the inner exception and log it for more details
                Exception innerException = ex.InnerException;
                // Log or inspect the innerException for more details
            }
        }

        public async Task<Payment> CreateOrderAsync(decimal amount, string returnUrl, string cancelUrl)
        {
            var apiContext = new APIContext(new OAuthTokenCredential(configuration["PayPal:ClientId"], configuration["PayPal:ClientSecret"]).GetAccessToken());

            var itemList = new ItemList()
            {
                items = new List<Item>()
                {
                    new Item()
                    {
                        name = "Courses Payment",
                        currency = "USD",
                        price = amount.ToString("0.00"),
                        quantity = "1",
                        sku = "membership"
                    }
                }
            };

            var transaction = new Transaction()
            {
                amount = new Amount()
                {
                    currency = "USD",
                    total = amount.ToString("0.00"),
                    details = new Details()
                    {
                        subtotal = amount.ToString("0.00")
                    },
                },
                item_list = itemList,
                description = "Courses Payment"
            };

            var payment = new Payment()
            {
                intent = "sale",
                payer = new Payer()
                {
                    payment_method = "paypal",
                },
                redirect_urls = new RedirectUrls()
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl,
                },
                transactions = new List<Transaction> { transaction }
            };

            var createdPayment = payment.Create(apiContext);

            return createdPayment;
        }
    }
}
