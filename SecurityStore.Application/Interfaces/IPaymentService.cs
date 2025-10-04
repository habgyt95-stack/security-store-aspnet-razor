namespace SecurityStore.Application.Interfaces;

// رابط سرویس پرداخت
// Payment service interface
public interface IPaymentService
{
    // پردازش پرداخت
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
    
    // تأیید پرداخت
    Task<bool> VerifyPaymentAsync(string transactionId);
}

// درخواست پرداخت
// Payment request
public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? Description { get; set; }
}

// نتیجه پرداخت
// Payment result
public class PaymentResult
{
    public bool Success { get; set; }
    public string? TransactionId { get; set; }
    public string? Message { get; set; }
    public string? RedirectUrl { get; set; }
}
