// Interfaces/IPaymentService.cs
public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(decimal amount);
}

