using SecurityStore.Application.Interfaces;

namespace SecurityStore.Infrastructure.Services;

// سرویس پرداخت تستی (برای توسعه)
// Fake payment service (for development)
public class FakePaymentService : IPaymentService
{
    public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        // شبیه‌سازی پردازش پرداخت
        // Simulate payment processing
        var result = new PaymentResult
        {
            Success = true,
            TransactionId = Guid.NewGuid().ToString(),
            Message = "پرداخت با موفقیت انجام شد - Payment successful (Simulated)",
            RedirectUrl = null
        };
        
        return Task.FromResult(result);
    }
    
    public Task<bool> VerifyPaymentAsync(string transactionId)
    {
        // همیشه تأیید می‌کند (برای تست)
        // Always verifies (for testing)
        return Task.FromResult(true);
    }
}
